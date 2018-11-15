using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.WCP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV.WeChat
{

    /// <summary>
    /// 微信回调处理类
    /// </summary>
    public class WxMessageWorkerBase
    {
        protected WebChatMessageDTO Log { get; set; }
        protected bool HasError { get; set; }
        //protected WxSettingDTO Setting { get; set; }

        /// <summary>
        /// 微信回调处理类
        /// </summary>
        /// <param name="log"></param>
        public WxMessageWorkerBase(WebChatMessageDTO log)
        {
            Log = log;
            if (log == null || string.IsNullOrEmpty(log.ToUserName))
            {
                HasError = true;
                return;
            }
            //Setting = WxSetting.GetDTOByWxOriginalId(log.ToUserName);
            //if (Setting == null)
            //{
            //    HasError = true;
            //    return;
            //}
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        public void Do()
        {
            if (HasError)
                return;
            if (!Init())
                return;
            DealEvent();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// 时间处理
        /// </summary>
        /// <returns></returns>
        protected virtual void DealEvent()
        {
            if (string.IsNullOrEmpty(Log.Ticket))
                return;
            var qrcode = WeChatQRCode.ObjectSet().FirstOrDefault(c => c.QRType.Value == (int)QrType.SpreadManager && c.WeChatTicket == Log.Ticket);
            if (qrcode == null ||  qrcode.SpreadInfoId == Guid.Empty)
                return;
            var url = SpreadInfo.ObjectSet().Where(c => c.Id == qrcode.SpreadInfoId).Select(c => c.SpreadUrl).FirstOrDefault();

            Jinher.AMP.WCP.Deploy.CustomDTO.CusNewsPushDTO msgDto = new CusNewsPushDTO()
                {
                    AppId = CustomConfig.WeChatSpreader.AppId.ToString(),
                    Content = getMessageContent(url),
                };
            WCPSV.Instance.PushSysMessageToUsers(msgDto);
        }
        private string getMessageContent(string spreadUrl)
        {
            ArticleNewsDTO message = new ArticleNewsDTO();
            message.touser = Log.FromUserName;
            message.msgtype = "news";
            message.news.articles.Add(new ArticleDTO
                {
                    title = CustomConfig.WeChatSpreader.MessageTitle,
                    description = CustomConfig.WeChatSpreader.MessageDesc,
                    url = spreadUrl,
                    picurl = CustomConfig.WeChatSpreader.MessagePic
                });
            return JsonHelper.JsSerializer(message);
        }

        /// <summary>
        /// 获取返回值
        /// </summary>
        public virtual object Result
        {
            get
            {
                //TODO 徐志刚给定结果，，在这里填写返回值
                return null;
            }
        }



    }
}
