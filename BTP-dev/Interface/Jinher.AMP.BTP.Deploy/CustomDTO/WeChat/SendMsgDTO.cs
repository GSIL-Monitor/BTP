using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.WeChat
{
    /// <summary>
    /// 发送消息
    /// </summary>
    [DataContract]
    [Serializable]
    public class SendMsgDTO : AccessCertifDTO
    {
        /// <summary>
        /// 接受者的OPENID
        /// </summary>
        [DataMember]
        public string ToUser { get; set; }

        /// <summary>
        /// 消息类型，目前只支持text
        /// </summary>
        [DataMember]
        public string MsgType { get; set; }

        /// <summary>
        /// text消息的消息体
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        //text外的其它类型暂不支持

        /// <summary>
        /// 
        /// </summary>
        public string GetPostJson
        {
            get
            {
                if (MsgType != "text")
                    throw new Exception("MsgType必须是text!");

                return "{\"touser\":\"" + ToUser + "\",\"msgtype\":\"" + MsgType + "\",\"text\":{\"content\":\"" +
                       Content + "\"}}";
            }
        }
    }
}
