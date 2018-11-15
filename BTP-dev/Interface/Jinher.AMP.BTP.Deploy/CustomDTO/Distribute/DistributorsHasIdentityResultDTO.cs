using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商是否有身份信息DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorsHasIdentityResultDTO
    {
        /// <summary>
        /// 分销商ID
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }

        /// <summary>
        /// 分销商申请记录ID
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 是否有身份信息
        /// </summary>
        [DataMember]
        public bool HasIdentity { get; set; }

        /// <summary>
        /// 所有身份信息
        /// </summary>
        [DataMember]
        public List<DistributionIdentityDTO> Identitys { get; set; }

        /// <summary>
        /// 个人图片
        /// </summary>
        [DataMember]
        public string PicturePath { get; set; }
    }
}
