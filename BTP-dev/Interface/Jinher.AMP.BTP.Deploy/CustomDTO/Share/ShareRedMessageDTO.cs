using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    
    /// <summary>
    /// 新红包消息体定义
    /// </summary>
    [DataContract]
    public class ShareRedMessageDTO
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        [DataMember]
        public Guid msgId
        {
            get;
            set;
        }

        /// <summary>
        /// 消息标题
        /// IOS版本作为内容显示
        /// </summary>
        [DataMember]
        public string userName
        {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// android，用户消息内容的现实
        /// Ios不用显示
        /// </summary>
        [DataMember]
        public string message
        {
            get;
            set;
        }

        /// <summary>
        /// 消息url
        /// android，用户消息Url
        /// Ios不用显示
        /// </summary>
        [DataMember]
        public string url
        {
            get;
            set;
        }
    }
}
