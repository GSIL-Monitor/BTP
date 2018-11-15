using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.WCP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.Enum;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    /// <summary>
    /// BTP 发消息
    /// </summary>
    public class BTPMessageSV
    {
        /// <summary>
        /// 售后发送消息
        /// </summary>
        /// <param name="afterSalesMessages">售后消息发送实体</param>
        /// <returns></returns>
        public ResultDTO AddMessagesAfterSales(AfterSalesMessages afterSalesMessages)
        {
            try
            {
                string contentCode = string.Empty;

                //发送到：1卖家，2买家，3卖家与买家
                int sendTO = 0;
                int sendToTool = 0;

                //卖家延长3天时
                if (afterSalesMessages.IsSellerDelayTime)
                {
                    sendTO = 2;
                    afterSalesMessages.Messages = "编号为" + afterSalesMessages.Code + "订单商家申请延长收货时间3天！";
                }
                //申请时
                else if (afterSalesMessages.State == 5 && afterSalesMessages.orderRefundAfterSalesState == 0)
                {
                    sendTO = 1;
                    sendToTool = 1;
                    //仅退款
                    if (afterSalesMessages.RefundType == 0)
                    {
                        afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "，客户已申请退款，请关注！";
                    }
                    //退款退货
                    else if (afterSalesMessages.RefundType == 1)
                    {
                        afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "，客户已申请退款/退货，请关注！";
                    }
                }
                //撤消
                else if (afterSalesMessages.State == 3 && afterSalesMessages.orderRefundAfterSalesState == 3)
                {
                    sendTO = 1;
                    afterSalesMessages.Messages = "订单" + afterSalesMessages.Code + "，客户已撤销退款/退货申请，请关注！";
                }
                //拒绝申请
                else if (afterSalesMessages.State == 3 && afterSalesMessages.orderRefundAfterSalesState == 2)
                {
                    sendTO = 2;
                    //仅退款
                    if (afterSalesMessages.RefundType == 0)
                    {
                        afterSalesMessages.Messages = "由于" + afterSalesMessages.RefuseReason + "，您的订单" + afterSalesMessages.Code + "，商家拒绝了您的退款申请，请关注！";
                    }
                    //退款退货
                    else if (afterSalesMessages.RefundType == 1)
                    {
                        afterSalesMessages.Messages = "由于" + afterSalesMessages.RefuseReason + "，您的订单" + afterSalesMessages.Code + "，商家拒绝了您的退款/退货申请，请关注！";
                    }
                }
                //拒绝收货
                else if (afterSalesMessages.State == 3 && afterSalesMessages.orderRefundAfterSalesState == 4)
                {
                    sendTO = 2;
                    afterSalesMessages.Messages = "由于" + afterSalesMessages.RefuseReason + "，您的订单" + afterSalesMessages.Code + "，商家拒绝接受您的退货，请关注！";
                }
                //退款
                else if (afterSalesMessages.State == 7 && afterSalesMessages.orderRefundAfterSalesState == 1)
                {
                    sendTO = 2;
                    //仅退款
                    if (afterSalesMessages.RefundType == 0)
                    {
                        if (afterSalesMessages.PayType == 0)   //金币=0
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到金币账户确认！";
                        }
                        else if (afterSalesMessages.PayType == 3)   //支付宝担保
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到支付宝账户确认！";
                        }
                        else if (afterSalesMessages.PayType == 4)   //U付
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到支付账户确认！";
                        }

                    }
                    //退款退货
                    else if (afterSalesMessages.RefundType == 1)
                    {
                        if (afterSalesMessages.PayType == 0)   //金币=0
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到金币账户确认！";
                        }
                        else if (afterSalesMessages.PayType == 3)   //支付宝担保
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到支付宝账户确认！";
                        }
                        else if (afterSalesMessages.PayType == 4)   //U付
                        {
                            afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "已完成退款，退款金额" + afterSalesMessages.RefundMoney + "元，请到支付账户确认！";
                        }
                    }
                }
                //退货申请 同意
                else if (afterSalesMessages.State == 10 && afterSalesMessages.orderRefundAfterSalesState == 10)
                {
                    sendTO = 2;
                    afterSalesMessages.Messages = "您的订单" + afterSalesMessages.Code + "商家已同意退款/退货，请在7天内发货！";

                }
                //填写退货信息后
                else if (afterSalesMessages.State == 10 && afterSalesMessages.orderRefundAfterSalesState == 11)
                {
                    sendTO = 1;
                    afterSalesMessages.Messages = "退款/退货中的订单" + afterSalesMessages.Code + "，客户已发货，请注意查收！";
                }

                if (sendTO == 1)
                {
                    //发给商家
                    string _appName = APPSV.GetAppName(afterSalesMessages.AppId);
                    afterSalesMessages.Messages = "【" + _appName + "】" + afterSalesMessages.Messages;
                    SendMessageCommon(afterSalesMessages.Messages, afterSalesMessages.Messages, afterSalesMessages.Messages, 3, afterSalesMessages.AppId, afterSalesMessages.Id, afterSalesMessages.Code, null, true);
                }
                else if (sendTO == 2)
                {
                    //发给买家
                    contentCode = "BTP002";
                    PublishOrderMessage(afterSalesMessages.Id, afterSalesMessages.UserIds, afterSalesMessages.EsAppId, afterSalesMessages.Messages, contentCode, true);
                    ////正品会发送消息
                    //if (new ZPHSV().CheckIsAppInZPH(afterSalesMessages.AppId))
                    //{
                    //    //addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "", type);
                    //    PublishOrderMessage(afterSalesMessages.Id, afterSalesMessages.UserIds, CustomConfig.ZPHAppId, afterSalesMessages.Messages, contentCode, true);
                    //}
                }

                //发送运营工具
                if (sendToTool == 1)
                {
                    //只有自提，并且代运营时才发
                    if (afterSalesMessages.SelfTakeFlag == 1)
                    {
                        contentCode = "BTP006";
                        if (ZPHSV.Instance.CheckIsAppInZPH(afterSalesMessages.AppId))
                        {
                            SendMessageToSelfTakeManager(afterSalesMessages.Id, afterSalesMessages.SelfTakeManagerIds, CustomConfig.SelfTakeAppId, afterSalesMessages.Messages, contentCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("推送消息服务异常。afterSalesMessages：{0}", JsonHelper.JsonSerializer(afterSalesMessages)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">后台接收消息的标题</param>
        /// <param name="content">后台接收消息的内容</param>
        /// <param name="mobilemess">手机接收的消息</param>
        /// <param name="state">1.只给手机发送2.只给后台发送3.同时给手机和后台发消息</param>
        /// <param name="appId"></param>
        /// <param name="afterSalesMessages"></param>
        public void SendMessageCommon(string title, string content, string mobilemess, int state, System.Guid appId, string Id, string code, ContextDTO contextDTO, bool isAfterSales)
        {
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = null;
            try
            {
                applicationDTO = APPBP.Instance.GetAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SendMessageCommon AppManagerSV服务异常:获取应用信息异常。title：{0}，content：{1}，mobilemess：{2}，state：{3}，appId：{4}，Id：{5}，code：{6}", title, content, mobilemess, state, appId, Id, code), ex);
            }

            try
            {
                if (applicationDTO != null && applicationDTO.OwnerId != null && applicationDTO.OwnerId.Value != Guid.Empty)
                {
                    List<Guid> LGuid = new List<Guid>();
                    string messages = mobilemess;
                    LGuid = EBCSV.GetOrderMenuUsers(applicationDTO);
                    if (LGuid == null || !LGuid.Any())
                        return;
                    System.Text.StringBuilder strOrgUserIds = new System.Text.StringBuilder();
                    foreach (Guid orgUserId in LGuid)
                    {
                        strOrgUserIds.Append(orgUserId).Append(",");
                    }
                    strOrgUserIds.Remove(strOrgUserIds.Length - 1, 1);
                    AddMessages(Id, strOrgUserIds.ToString(), appId, code, state, messages, "orderAppOwner", isAfterSales);

                    //后台发消息 
                    Jinher.AMP.Info.Deploy.CustomDTO.MessageForAddDTO messageAdd = new Info.Deploy.CustomDTO.MessageForAddDTO();
                    //推送消息
                    messageAdd.PublishTime = DateTime.Now;
                    messageAdd.ReceiverUserId = LGuid;

                    //messageAdd.SenderOrgId = employeeDTO.EBCOrganizationId;
                    messageAdd.SenderType = Info.Deploy.Enum.SenderType.System;
                    messageAdd.Title = title;
                    messageAdd.Content = content;
                    //messageAdd.MessageType = "OrgDisableMessage";
                    messageAdd.ReceiverRange = Info.Deploy.Enum.ReceiverRange.SpecifiedUser;

                    var retret = Jinher.AMP.BTP.TPS.InfoSV.Instance.AddSystemMessage(messageAdd);
                    LogHelper.Info(string.Format("SendMessageCommon后台发送消息服务成功。messageAdd：{0}", JsonHelper.JsonSerializer(messageAdd)));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SendMessageCommon发送消息服务异常:。title：{0}，content：{1}，mobilemess：{2}，state：{3}，appId：{4}，Id：{5}，code：{6}", title, content, mobilemess, state, appId, Id, code), ex);
            }

        }

        public ResultDTO AddMessages(string id, string userids, Guid appId, string code, int? state, string messages, string type, bool isAfterSales)
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

                    PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);

                }
                else if (type == "orderAppOwner")
                {
                    contentCode = "BTP002";

                    PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);
                }
                else if (type == "payingorder")
                {
                    contentCode = "BTP003";
                    messages = "您的订单号：" + code + "的订单支付失败，请重新支付";
                    PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);
                }
                else if (type == "commodity")
                {
                    contentCode = "BTP001";
                    messages = "您收藏的商品：【" + code + "】 降价了";
                    PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);
                }
                else if (type == "selfTakeManager")
                {
                    contentCode = "BTP005";

                    PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);
                }
                else
                {
                    switch (type)
                    {
                        case "review":
                            contentCode = "BTP004";
                            messages = "您的评价有新的回复了，快去看看吧~";
                            PublishOrderMessage(id, userids, appId, messages, contentCode, isAfterSales);
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

        public void PublishOrderMessage(string id, string userids, Guid appId, string messages, string contentCode, bool isAfterSales)
        {
            MobileMessageDTO messageDTO = new MobileMessageDTO();
            messageDTO.AppId = appId.ToString();
            messageDTO.BasicContentDTO = new BasicContentDTO();
            messageDTO.BasicContentDTO.Id = id;
            messageDTO.BasicContentDTO.Code = contentCode;
            messageDTO.BasicContentDTO.Content = messages;
            messageDTO.ProductType = JAP.BaseApp.MessageCenter.Deploy.Enum.ProductType.BTP;
            //售后的需要设置成5
            if (isAfterSales)
            {
                messageDTO.ProductSecondType = 5;
            }

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
                Jinher.AMP.BTP.TPS.MessageCenter.Instance.AddMessage(messageDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("推送消息服务异常。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)), ex);
            }
            //messageDTO.BeginTime = DateTime.Now;
            //messageDTO.EndTime = DateTime.Now;
            LogHelper.Info(string.Format("推送消息服务成功。messageDTO：{0}", JsonHelper.JsonSerializer(messageDTO)));
        }

        public void SendMessageToSelfTakeManager(string id, List<Guid> userids, Guid appId, string messages, string contentCode)
        {
            if (userids == null || userids.Count() < 1)
            {
                return;
            }
            PublishOrderMessage(id, string.Join(",", userids), appId, messages, contentCode, false);
        }

        /// <summary>
        /// 售中发送消息
        /// </summary>
        /// <param name="commodityOrderMessages">售中消息发送实体</param>
        /// <returns></returns>
        public ResultDTO AddMessagesCommodityOrder(CommodityOrderMessages commodityOrderMessages)
        {
            try
            {
                string contentCode = string.Empty;

                //发送到：1卖家，2买家，3卖家与买家
                int sendTO = 0;
                int sendToTool = 0;

                string refundTypeText = "";
                if (commodityOrderMessages.RefundType == 0)
                {
                    refundTypeText = "退款";
                    //commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "，客户已申请退款，请关注！";
                }
                //退款退货
                else if (commodityOrderMessages.RefundType == 1)
                {
                    refundTypeText = "退款/退货";
                    //commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "，客户已申请退款/退货，请关注！";
                }

                //买家延长3天时
                if (commodityOrderMessages.IsBuyersDelayTime)
                {
                    sendTO = 1;
                    commodityOrderMessages.Messages = "编号为" + commodityOrderMessages.Code + "订单买家申请延长收货时间3天！";
                }
                //卖家延长3天时
                else if (commodityOrderMessages.IsSellerDelayTime)
                {
                    sendTO = 2;
                    commodityOrderMessages.Messages = "编号为" + commodityOrderMessages.Code + "订单商家申请延长收货时间3天！";
                }
                //申请时
                else if ((commodityOrderMessages.State == 8 || commodityOrderMessages.State == 9 || commodityOrderMessages.State == 14) && commodityOrderMessages.orderRefundState == 0)
                {
                    sendTO = 1;
                    //sendToTool = 1;
                    commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "，客户已申请" + refundTypeText + "，请关注！";
                }
                //撤消
                else if ((commodityOrderMessages.State == 1 || commodityOrderMessages.State == 2 || commodityOrderMessages.State == 13) && commodityOrderMessages.orderRefundState == 3)
                {
                    sendTO = 1;
                    commodityOrderMessages.Messages = "订单" + commodityOrderMessages.Code + "，客户已撤销" + refundTypeText + "申请，请关注！";
                }
                //拒绝申请
                else if ((commodityOrderMessages.State == 1 || commodityOrderMessages.State == 2 || commodityOrderMessages.State == 13) && commodityOrderMessages.orderRefundState == 2)
                {
                    sendTO = 2;
                    commodityOrderMessages.Messages = "由于" + commodityOrderMessages.RefuseReason + "，您的订单" + commodityOrderMessages.Code + "，商家拒绝了您的" + refundTypeText + "申请，请关注！";
                }
                //拒绝收货
                else if ((commodityOrderMessages.State == 2 || commodityOrderMessages.State == 13) && commodityOrderMessages.orderRefundState == 4)
                {
                    sendTO = 2;
                    commodityOrderMessages.Messages = "由于" + commodityOrderMessages.RefuseReason + "，您的订单" + commodityOrderMessages.Code + "，商家拒绝接受您的退货，请关注！";
                }
                //退款
                else if (commodityOrderMessages.State == 7 && commodityOrderMessages.orderRefundState == 1)
                {
                    sendTO = 2;
                    //仅退款
                    if (commodityOrderMessages.RefundType == 0)
                    {
                        if (commodityOrderMessages.PayType == 0)   //金币=0
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到金币账户确认！";
                        }
                        else if (commodityOrderMessages.PayType == 3)   //支付宝担保
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到支付宝账户确认！";
                        }
                        else if (commodityOrderMessages.PayType == 4)   //U付
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到支付账户确认！";
                        }

                    }
                    //退款退货
                    else if (commodityOrderMessages.RefundType == 1)
                    {
                        if (commodityOrderMessages.PayType == 0)   //金币=0
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到金币账户确认！";
                        }
                        else if (commodityOrderMessages.PayType == 3)   //支付宝担保
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到支付宝账户确认！";
                        }
                        else if (commodityOrderMessages.PayType == 4)   //U付
                        {
                            commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "已完成退款，退款金额" + commodityOrderMessages.RefundMoney + "元，请到支付账户确认！";
                        }
                    }
                }
                //退货申请 同意
                else if (commodityOrderMessages.State == 10 && commodityOrderMessages.orderRefundState == 10)
                {
                    sendTO = 2;
                    commodityOrderMessages.Messages = "您的订单" + commodityOrderMessages.Code + "商家已同意退款/退货，请在7天内发货！";
                }
                //填写退货信息后
                else if (commodityOrderMessages.State == 10 && commodityOrderMessages.orderRefundState == 11)
                {
                    sendTO = 1;
                    commodityOrderMessages.Messages = "退款/退货中的订单" + commodityOrderMessages.Code + "，客户已发货，请注意查收！";
                }


                if (sendTO == 1)
                {
                    //发给商家
                    string _appName = APPSV.GetAppName(commodityOrderMessages.AppId);
                    commodityOrderMessages.Messages = "【" + _appName + "】" + commodityOrderMessages.Messages;
                    SendMessageCommon(commodityOrderMessages.Messages, commodityOrderMessages.Messages, commodityOrderMessages.Messages, 3, commodityOrderMessages.AppId, commodityOrderMessages.Id, commodityOrderMessages.Code, null, false);
                }
                else if (sendTO == 2)
                {
                    //发给买家
                    contentCode = "BTP002";
                    PublishOrderMessage(commodityOrderMessages.Id, commodityOrderMessages.UserIds, commodityOrderMessages.EsAppId, commodityOrderMessages.Messages, contentCode, false);
                    ////正品会发送消息
                    //if (new ZPHSV().CheckIsAppInZPH(commodityOrderMessages.AppId))
                    //{
                    //    //addmassage.AddMessages(odid, usid, CustomConfig.ZPHAppId, order.Code, order.State, "", type);
                    //    PublishOrderMessage(commodityOrderMessages.Id, commodityOrderMessages.UserIds, CustomConfig.ZPHAppId, commodityOrderMessages.Messages, contentCode, false);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("推送消息服务异常。commodityOrderMessages：{0}", JsonHelper.JsonSerializer(commodityOrderMessages)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <returns></returns>
        public ResultDTO SendPayWxMessage(CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO() { };

            if (commodityOrder == null)
                return result;
            //TODO yjz  是不是要改成微信中下单给公众号发消息，，否则不发？？
            if (commodityOrder.OrderType != 2)
                return result;
            var opendId = CBCSV.Instance.GetThirdBind(commodityOrder.UserId);
            if (string.IsNullOrEmpty(opendId))
                return result;
            var message = new PayWxMessageDTO()
                {
                    touser = opendId,
                    msgtype = "news"
                };
            message.news.articles.Add(new WxArticles()
                {
                    title = "下单成功",
                    description = "下单成功，商家正火速处理中，请耐心等待……",
                    url = string.Format("{0}{1}?orderId={2}", CustomConfig.BtpDomain, "Mobile/CYMyOrderDetailShow", commodityOrder.Id),
                    picurl = ""
                });
            Jinher.AMP.WCP.Deploy.CustomDTO.CusNewsPushDTO messDTO = new CusNewsPushDTO()
                {
                    AppId = commodityOrder.EsAppId.ToString(),
                    Content = JsonHelper.JsonSerializer(message),
                };
            WCPSV.Instance.PushSysMessageToUsers(messDTO);

            return new ResultDTO();
        }

        /// <summary>
        /// 推送 msgtype="news" 的微信消息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ResultDTO SendNewsMessageToWx(Guid appId, PayWxMessageDTO msg)
        {
            var messDTO = new CusNewsPushDTO()
            {
                AppId = appId.ToString(),
                Content = JsonHelper.JsonSerializer(msg),
            };
            WCPSV.Instance.PushSysMessageToUsers(messDTO);

            return new ResultDTO();
        }

        /// <summary>
        /// 向App推送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ResultDTO SendMobileMessage(MobileMsgDTO msg)
        {
            var mobileMessage = new MobileMessageDTO
            {
                AppId = msg.AppId.ToString(),
                BasicContentDTO = new BasicContentDTO
                {
                    Id = msg.MsgId,
                    Code = msg.ContentCode.ToString(),
                    Content = msg.MsgContent
                },
                ProductType = ProductType.BTP,
                ProductSecondType = (int)msg.SecondType,
                UserIds = "[" + string.Join(",", msg.ToUsers.Select(x => "\"" + x + "\"")) + "]",
                EndTime = DateTime.Now.AddDays(13),
                MessageType = (MessageType)(int)(msg.MobileMsgType)
            };
            try
            {
                MessageCenter.Instance.AddMessage(mobileMessage);
            }
            catch (Exception ex)
            {
                var errMsg = string.Format("推送消息服务异常。messageDTO：{0}", JsonHelper.JsonSerializer(mobileMessage));
                LogHelper.Error(errMsg, ex);
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = errMsg };
            }
            return new ResultDTO { isSuccess = true, ResultCode = 0, Message = "Success" };
        }
    }


    /// <summary>
    /// 售后消息发送实体
    /// </summary>
    public class AfterSalesMessages
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// UserIds
        /// </summary>
        public string UserIds { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        public Guid AppId { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13，售后交易成功=15
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 退款类型：仅退款=0，退货退款=1
        /// </summary>
        public int? RefundType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundMoney { get; set; }
        /// <summary>
        /// 金币=0， 到付=1， 支付宝=2， 在线支付=3， 支付宝担保4:U付
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Messages { get; set; }
        /// <summary>
        /// 是否为Job：false是后台操作；true是Job调用
        /// </summary>
        public bool IsAuto { get; set; }
        /// <summary>
        /// 退款中=0，已退款=1，已拒绝=2，已撤销=3，售后退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13
        /// </summary>
        public int orderRefundAfterSalesState { get; set; }

        /// <summary>
        /// 退款中=0，已退款=1，已拒绝=2，已撤销=3，售后退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13
        /// </summary>
        public int oldOrderRefundAfterSalesState { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RefuseReason { get; set; }

        /// <summary>
        /// 延长3天
        /// </summary>
        public bool IsDelayConfirmTimeAfterSales { get; set; }

        /// <summary>
        /// 是否自提(用于对运营工具发消息，其它情况下无用) 0：非自提；1：自提
        /// </summary>
        public int SelfTakeFlag { get; set; }

        /// <summary>
        /// 自提点负责人
        /// </summary>
        public List<Guid> SelfTakeManagerIds { get; set; }

        /// <summary>
        /// 卖家是否延长时间
        /// </summary>
        public bool IsSellerDelayTime { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        public Guid EsAppId { get; set; }
    }

    /// <summary>
    /// 售中消息发送实体
    /// </summary>
    public class CommodityOrderMessages
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// UserIds
        /// </summary>
        public string UserIds { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        public Guid AppId { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 订单状态（必填）：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 退款类型：仅退款=0，退货退款=1
        /// </summary>
        public int? RefundType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundMoney { get; set; }
        /// <summary>
        /// 金币=0， 到付=1， 支付宝=2， 在线支付=3， 支付宝担保4:U付
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Messages { get; set; }
        /// <summary>
        /// 是否为Job：false是后台操作；true是Job调用
        /// </summary>
        public bool IsAuto { get; set; }
        /// <summary>
        /// 退款中=0，已退款=1，已拒绝=2，已撤销=3，售后退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13
        /// </summary>
        public int orderRefundState { get; set; }

        /// <summary>
        /// 退款中=0，已退款=1，已拒绝=2，已撤销=3，售后退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12,买家发货超时，商家未收到货=13
        /// </summary>
        public int oldOrderRefundState { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RefuseReason { get; set; }

        /// <summary>
        /// 延长3天
        /// </summary>
        public bool IsDelayConfirmTimeAfterSales { get; set; }

        /// <summary>
        /// 是否自提(用于对运营工具发消息，其它情况下无用) 0：非自提；1：自提
        /// </summary>
        public int SelfTakeFlag { get; set; }
        /// <summary>
        ///  售中不用给运营工具发消息
        /// </summary>
        public List<Guid> SelfTakeManagerIds { get; set; }

        /// <summary>
        /// 买家是否延长时间
        /// </summary>
        public bool IsBuyersDelayTime { get; set; }

        /// <summary>
        /// 卖家是否延长时间
        /// </summary>
        public bool IsSellerDelayTime { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        public Guid EsAppId { get; set; }
    }
}
