using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 审核结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class AuditDistributionResultDTO
    {
        /// <summary>
        /// ApplyId
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 审核结果是通过还是拒绝
        /// </summary>
        [DataMember]
        public bool IsPass { get; set; }

        /// <summary>
        /// 分销商ID
        /// </summary>
        [DataMember]
        public Guid? DistributorId { get; set; }

        /// <summary>
        /// 微小店ID
        /// </summary>
        [DataMember]
        public Guid? MicroShopId { get; set; }

        /// <summary>
        /// 微小店Logo
        /// </summary>
        [DataMember]
        public string MicroShopLogo { get; set; }

        /// <summary>
        /// 微小店Url
        /// </summary>
        [DataMember]
        public string MicroShopUrl { get; set; }
    }
}
