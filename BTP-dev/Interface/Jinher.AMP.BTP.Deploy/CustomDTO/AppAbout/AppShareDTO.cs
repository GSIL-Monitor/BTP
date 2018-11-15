using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppShareDTO
    {
        /// <summary>
        /// 图标，可空
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
        /// <summary>
        /// 共享内容,非空
        /// </summary>
        [DataMember]
        public string ShareContent { get; set; }
        /// <summary>
        /// 分享的描述，可空
        /// </summary>
        [DataMember]
        public string ShareDesc { get; set; }
        /// <summary>
        /// 跳转页地址，可空
        /// </summary>
        [DataMember]
        public string ShareGotoUrl { get; set; }
        /// <summary>
        /// 消息出处，可空
        /// </summary>
        [DataMember]
        public string ShareMessSrc { get; set; }
        /// <summary>
        /// 分享标题，可空
        /// </summary>
        [DataMember]
        public string ShareTopic { get; set; }
    }
}
