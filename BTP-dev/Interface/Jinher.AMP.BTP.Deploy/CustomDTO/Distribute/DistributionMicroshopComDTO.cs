using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 微小店下架商品
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionMicroshopComDTO
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 微小店id
        /// </summary>
        [DataMember]
        public Guid MicroshopId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
    }
}
