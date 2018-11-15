using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 微信推送日志
    /// </summary>
    [Serializable()]
    [DataContract]
    [XmlRoot("xml")]
    public class WebChatMessageDTO
    {

        /// <summary>
        /// 公众号原始id
        /// </summary>
        [DataMemberAttribute()]
        public string ToUserName { get; set; }

        /// <summary>
        /// 用户openId或者审核系统等
        /// </summary>
        /// <LongDescription>
        /// 不同类型消息含义不同
        /// </LongDescription>
        [DataMemberAttribute()]
        public string FromUserName { get; set; }

        /// <summary>
        /// 推送时间
        /// </summary>
        [DataMemberAttribute()]
        public long CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [DataMemberAttribute()]
        public string MsgType { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        [DataMemberAttribute()]
        public string Event { get; set; }

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        [DataMemberAttribute()]
        public string EventKey { get; set; }

        /// <summary>
        /// 二维码ticket
        /// </summary>
        [DataMemberAttribute()]
        public string Ticket { get; set; }

    }
}
