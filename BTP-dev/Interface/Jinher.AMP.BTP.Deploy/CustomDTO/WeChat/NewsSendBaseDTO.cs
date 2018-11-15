using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 客服消息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class NewsSendBaseDTO
    {
        /// <summary>
        /// 普通用户openid
        /// </summary>
        [DataMember]
        public string touser { get; set; }

        /// <summary>
        /// 消息类型，文本为text，图片为image，语音为voice，视频消息为video，音乐消息为music，图文消息（点击跳转到外链）为news，图文消息（点击跳转到图文消息页面）为mpnews，卡券为wxcard
        /// </summary>
        [DataMember]
        public string msgtype { get; set; }

    }
}
