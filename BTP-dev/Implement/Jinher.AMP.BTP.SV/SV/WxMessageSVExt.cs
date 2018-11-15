
/***************
功能描述: BTPSV
作    者: 
创建时间: 2017/5/26 14:14:24
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.SV.WeChat;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class WxMessageSV : BaseSv, IWxMessage
    {

        /// <summary>
        /// 处理微信消息推送
        /// <para>Service URL：
        /// <a href="http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage">
        /// http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage
        /// </a></para>
        /// </summary>
        /// <param name="message">微信消息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto DealMessageExt(Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiDto message)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto result = new WcpBusiRetDto();
            if (message == null || string.IsNullOrEmpty(message.WXContent))
                return result;
            var messageDto = XmlUtil.Deserialize<WebChatMessageDTO>(message.WXContent);
            if (messageDto == null)
                return result;
            WxMessageWorkerBase worker;

            switch (messageDto.Event)
            {
                //关注公众号
                case "subscribe":
                    worker = new ScanSubscribeWorker(messageDto);
                    break;
                //进入公众号事件推送
                case "SCAN":
                    worker = new ScanWorker(messageDto);
                    break;
                default:
                    return result;
            }
            worker.Do();
            return result;
        }
    }
}