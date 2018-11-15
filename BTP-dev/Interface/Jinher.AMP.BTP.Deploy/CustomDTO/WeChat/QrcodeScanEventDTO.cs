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
    [XmlRoot("xml")]
    public class QrcodeScanEventDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ToUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string FromUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MsgType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string EventKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Ticket { get; set; }

        /// <summary>
        /// 是“要求用户关注”事件
        /// </summary>
        public bool IsSubscribeEvent
        {
            get
            {
                //Event值为"subscribe"表示已关注，否则值为"SCAN"表示
                return Event.ToLower() == "subscribe";
            }
        }

        /// <summary>
        /// 二维码参数，即scene_str
        /// </summary>
        public string QrcodeParameter
        {
            get
            {
                var tmp = EventKey;

                //去掉qrscene_前缀
                if (IsSubscribeEvent && tmp.StartsWith("qrscene_"))
                    tmp = tmp.Substring(8);
                return tmp;
            }
        }
    }
}
