using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.JAP.BF.BE.Deploy.Base;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 促销活动dto
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(PromotionItemOutsideDTO))]
    public class PromotionOutSideVM
    {
        /// <summary>
        /// 正品会Id
        /// </summary>
        [DataMember]
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 外部活动Id
        /// </summary>
        [DataMember]
        public Guid OutsideId { get; set; }

        /// <summary>
        /// 促销名称
        /// </summary>
        [DataMember]
        public string PromotionName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 力度/折扣
        /// </summary>
        [DataMember]
        public decimal Intensity { get; set; }

        /// <summary>
        /// 优惠价格
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }

        /// <summary>
        /// 是否启用（暂不使用）
        /// </summary>
        [DataMember]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 活动类型 0：普通活动，1：秒杀，2预售，3，拼团，5、预售(不用预约)
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }

        /// <summary>
        /// 订单未支付过期时间（以秒为单位），不过期传0
        /// </summary>
        [DataMember]
        [Obsolete("已过期，超时时间统一设置", false)]
        public long ExpireSeconds { get; set; }
        /// <summary>
        /// 预约，预售开始时间
        /// </summary>
        [DataMember]
        public DateTime? PresellStartTime { get; set; }
        /// <summary>
        /// 预约，预售结束时间
        /// </summary>
        [DataMember]
        public DateTime? PresellEndTime { get; set; }
        /// <summary>
        /// 活动商品列表
        /// </summary>
        [DataMember]
        public List<PromotionItemOutsideDTO> CommodityList { get; set; }
        /// <summary>
        /// 成团人数
        /// </summary>
        [DataMember]
        public Int32 GroupMinVolume { get; set; }
        /// <summary>
        /// 超时时间单位(秒)
        /// </summary>
        [DataMember]
        public Int32 ExpireSecond { get; set; }
        /// <summary>
        /// 活动描术
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 预售前是否销售
        /// </summary>
        [DataMember]
        public bool? IsSell { get; set; }

    }
    [Serializable]
    [DataContract]
    public class PromotionItemOutsideDTO
    {
        /// <summary>
        /// 商品所在appId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 优惠价格
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }

        /// <summary>
        /// 每人限购数量
        /// </summary>
        [DataMember]
        public int? LimitBuyEach { get; set; }

        /// <summary>
        /// 参加活动商品总数
        /// </summary>
        [DataMember]
        public int LimitBuyTotal { get; set; }
    }
}