using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.JAP.BF.BE.Deploy.Base;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class DiscountsVM : BusinessDTO
    {
        /// <summary>
        /// 促销名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 手机图片地址
        /// </summary>
        [DataMember]
        public string PhonePicturesPath { get; set; }
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
        /// 是否启用
        /// </summary>
        [DataMember]
        public bool IsEnable { get; set; }
        /// <summary>
        /// 商家ID
        /// </summary>
        [DataMember]
        public Guid SellerId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMember]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 全部商品
        /// </summary>
        [DataMember]
        public bool? IsAll { get; set; }
        /// <summary>
        /// 商品ID集合
        /// </summary>
        [DataMember]
        public string CommodityIds { get; set; }
        /// <summary>
        /// 折扣商品集合
        /// </summary>
        [DataMember]
        public List<string> Commoditys { get; set; }
        /// <summary>
        /// 商品编号集合
        /// </summary>
        [DataMember]
        public List<string> No_Codes { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityNames { get; set; }

        /// <summary>
        /// 商品ID集合
        /// </summary>
        [DataMember]
        public List<Guid> CommodityIdList { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 优惠价格
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// 商品促销信息集合
        /// </summary>
        [DataMember]
        public string[] ComPro { get; set; }
        /// <summary>
        /// 活动类型 0：普通活动，1：秒杀活动
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }

        /// <summary>
        /// 全场限购数量（整个活动，某用户可购买的商品的数量）
        /// </summary>
        [DataMember]
        public int? LimitBuyTotal { get; set; }
    }
}