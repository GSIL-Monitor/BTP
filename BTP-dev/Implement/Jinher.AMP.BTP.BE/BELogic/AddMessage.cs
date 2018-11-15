using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Jinher.AMP.BTP.BE.BELogic
{
    public partial class AddMessage
    {
        public ResultDTO AddMessages(string id, string userids, Guid appId, string code, int? state, string messages, string type)
        {

            try
            {
                string contentCode = string.Empty;
                if (type == "order")
                {
                    if (state == 1)
                    {
                        messages = "您的订单号：" + code + "的订单已进入待发货状态";
                    }
                    if (state == 2)
                    {
                        messages = "您的订单号：" + code + "的订单已发货";
                    }
                    if (state == 3)
                    {
                        messages = "您的订单号：" + code + "的订单交易成功";
                    }
                    contentCode = "BTP002";

                    PublishOrderMessage(id, userids, appId, messages, contentCode);

                }
                else if (type == "yxCancelOrder")
                {
                    contentCode = "BTP002";
                    PublishOrderMessage(id, userids, appId, messages, contentCode);
                }
                else if (type == "orderAppOwner")
                {
                    contentCode = "BTP002";

                    PublishOrderMessage(id, userids, appId, messages, contentCode);
                }
                else if (type == "payingorder")
                {
                    contentCode = "BTP003";
                    messages = "您的订单号：" + code + "的订单支付失败，请重新支付";
                    PublishOrderMessage(id, userids, appId, messages, contentCode);
                }
                else if (type == "commodity")
                {
                    contentCode = "BTP001";
                    messages = "您收藏的商品：【" + code + "】 降价了";
                    PublishOrderMessage(id, userids, appId, messages, contentCode);
                }
                else if (type == "selfTakeManager")
                {
                    contentCode = "BTP005";

                    PublishOrderMessage(id, userids, appId, messages, contentCode);
                }
                else
                {
                    switch (type)
                    {
                        case "review":
                            contentCode = "BTP004";
                            messages = "您的评价有新的回复了，快去看看吧~";
                            PublishOrderMessage(id, userids, appId, messages, contentCode);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("推送消息服务异常。id：{0}，userids：{1}，appId：{2}，code：{3}，state：{4}，messages：{5}，type：{6}", id, userids, appId, code, state, messages, type), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }


        /// <summary>
        /// 众筹发消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="appId"></param>
        /// <param name="endTime"></param>
        /// <param name="content"></param>
        public void SendMessage(Guid msgId, Guid appId, DateTime? endTime, CrowdfundingMessageDTO content)
        {
            //给邀请人发送系统消息
            //调用消息中心发消息
            //定义消息内容 
            MobileMessageDTO messageDTO = new MobileMessageDTO();

            messageDTO.MessageType = JAP.BaseApp.MessageCenter.Deploy.Enum.MessageType.BUSI_MSG;

            messageDTO.AppId = appId.ToString().ToLower();
            messageDTO.BasicContentDTO = new BasicContentDTO();
            messageDTO.BasicContentDTO.Code = "CfStateChange";//"BTPShareRedEnvelope";

            messageDTO.ProductType = JAP.BaseApp.MessageCenter.Deploy.Enum.ProductType.BTP;
            //messageDTO.ProductSecondType = 3;

            DataContractJsonSerializer json = new DataContractJsonSerializer(content.GetType());
            string szJson = "";
            //序列化
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, content);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            }

            messageDTO.BasicContentDTO.Content = szJson.Replace("\"", "\\\"");
            //messageDTO.Content = szJson.Replace("\"", "\\\""); ;

            if (endTime != null)
                messageDTO.EndTime = (DateTime)endTime;
            //接收人邀请人Id
            messageDTO.UserIds = "[]";


            PublishMobileMessageFacade facade = new PublishMobileMessageFacade();
            try
            {
                facade.AddMessage(messageDTO);
                //LogHelper.Info("众筹状态改变消息内容：" + messageDTO.AppId + ":" + messageDTO.BasicContentDTO.Content);
                LogHelper.Info(string.Format("众筹状态改变消息内容。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("发送众筹状态改变消息异常。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)), ex);
                // JAP.Common.Loging.LogHelper.Error("发送众筹状态改变消息异常", ex);
            }
        }


        private static void PublishOrderMessage(string id, string userids, Guid appId, string messages, string contentCode)
        {
            MobileMessageDTO messageDTO = new MobileMessageDTO();
            messageDTO.AppId = appId.ToString();
            messageDTO.BasicContentDTO = new BasicContentDTO();
            messageDTO.BasicContentDTO.Id = id;
            messageDTO.BasicContentDTO.Code = contentCode;
            messageDTO.BasicContentDTO.Content = messages;
            messageDTO.ProductType = JAP.BaseApp.MessageCenter.Deploy.Enum.ProductType.BTP;

            string[] aa = userids.Split(',');
            messageDTO.UserIds = "[";
            foreach (var temp in aa)
            {
                messageDTO.UserIds += "\"" + temp + "\"" + ",";
            }
            messageDTO.UserIds = messageDTO.UserIds.Substring(0, messageDTO.UserIds.Length - 1);
            messageDTO.UserIds += "]";
            messageDTO.EndTime = DateTime.Now.AddDays(13);
            messageDTO.MessageType = JAP.BaseApp.MessageCenter.Deploy.Enum.MessageType.BUSI_MSG;
            try
            {
                LogHelper.Info(string.Format("推送消息服务参数。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)));
                PublishMobileMessageFacade publishMobileMessageFacade = new PublishMobileMessageFacade();
                publishMobileMessageFacade.AddMessage(messageDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("推送消息服务异常。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)), ex);
                // LogHelper.Error(string.Format("推送消息服务异常。Message：{0},UserIds:{1}，AppId:{2}", messageDTO.BasicContentDTO.Content, userids, appId), ex);
            }

            //LogHelper.Info(string.Format("推送消息服务成功。Message：{0},UserIds:{1}，AppId:{2}", messageDTO.BasicContentDTO.Content, userids, appId));
            LogHelper.Info(string.Format("推送消息服务成功。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)));
        }
    }
}
