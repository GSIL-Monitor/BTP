using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品促销信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PromotionItemShortCDTO
    {
        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }

        /// <summary>
        /// 促销开始时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 促销结束时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyEach { get; set; }

        /// <summary>
        /// 促销商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyTotal { get; set; }

        /// <summary>
        /// 促销商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int? SurplusLimitBuyTotal { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 活动来源Id(正品会Id)
        /// </summary>
        [DataMemberAttribute()]
        public Guid? ChannelId { get; set; }

        /// <summary>
        /// 外部活动Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid? OutsideId { get; set; }

        /// <summary>
        /// 预约、预售开始时间
        /// </summary>
        [DataMember]
        public DateTime? PresellStartTime { get; set; }

        /// <summary>
        /// 预约、预售结束时间
        /// </summary>
        [DataMember]
        public DateTime? PresellEndTime { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        [DataMemberAttribute()]
        public int PromotionType { get; set; }

        /// <summary>
        /// 全场限购数量(整个活动限购数量)
        /// </summary>
        [DataMemberAttribute()]
        public int? PromtionLimitBuyTotal { get; set; }

        /// <summary>
        /// 超时秒
        /// </summary>
        [DataMemberAttribute()]
        public int? ExpireSecond { get; set; }
        /// <summary>
        /// 超时秒
        /// </summary>
        [DataMemberAttribute()]
        public int? GroupMinVolume { get; set; }
        /// <summary>
        /// 活动描述
        /// </summary>
        [DataMemberAttribute()]
        public string Description { get; set; }
    }
}
