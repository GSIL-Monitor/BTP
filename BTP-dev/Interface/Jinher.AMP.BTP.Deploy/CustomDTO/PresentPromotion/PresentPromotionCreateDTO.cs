using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 赠品活动列表DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionCreateDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 商品单次最少购买数量
        /// </summary>
        [DataMember]
        public int? Limit { get; set; }

        /// <summary>
        /// 主商品
        /// </summary>
        [DataMember]
        public List<PresentPromotionCommodityDetailsDTO> Commodities { get; set; }

        /// <summary>
        /// 赠品
        /// </summary>
        [DataMember]
        public List<PresentPromotionCommodityDetailsDTO> Gifts { get; set; }
    }
}
