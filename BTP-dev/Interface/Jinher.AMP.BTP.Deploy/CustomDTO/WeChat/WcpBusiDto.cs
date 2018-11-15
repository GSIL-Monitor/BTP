using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// WCP消息  请参考：Jinher.AMP.WCP.Deploy.CustomDTO.BusiDto
    /// </summary>
    [Serializable]
    [DataContract]
    public class WcpBusiDto
    {
        /// <summary>
        /// 平台appid
        /// </summary>
        [DataMember]
        public string appId;
        /// <summary>
        /// 微信appid
        /// </summary>
        [DataMember]
        public string appKey;
        /// <summary>
        /// 微信消息xml
        /// </summary>
        [DataMember]
        public string WXContent;
    }
}
