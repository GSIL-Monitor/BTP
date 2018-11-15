using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.WeChat
{

    /// <summary>
    /// 二维码扫描后的事件消息
    /// </summary>
    [DataContract]
    [Serializable]
    public class WeChatSearchDTO
    {
        /// <summary>
        /// 二维码的ticket
        /// </summary>
        [DataMember]
        public string Ticket { get; set; }

    }
}
