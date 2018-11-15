using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 活动已购买的促销数量Dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class PromotionSurplusLimitBuyTotalDto
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 已购买的促销数量
        /// </summary>
        [DataMember]
        public int? SurplusLimitBuyTotal { get; set; }
    }
}
