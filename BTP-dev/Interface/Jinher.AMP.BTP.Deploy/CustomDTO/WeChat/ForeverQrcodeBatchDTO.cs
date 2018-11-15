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
    public class ForeverQrcodeBatchDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<string> Scenes { get; set; }
    }
}
