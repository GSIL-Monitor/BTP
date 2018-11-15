using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商请求DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionSearchDTO : SearchBase
    {
        /// <summary>
        /// appid
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否是查看默认
        /// </summary>
        [DataMember]
        public bool IsLook { get; set; }
    }
}
