using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 购物车商品属性
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartCommodityAttrDTO
    {
        /// <summary>
        /// 图片
        /// </summary>
        [DataMember()]
        public string Pic { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal? Intensity { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int? Stock { get; set; }

        /// <summary>
        /// 商品属性列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttributeDTO> ComAttibutes { get; set; }


        /// <summary>
        ///  包装规格设置
        /// </summary>
        //[DataMemberAttribute()]
        //public List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specifications { get; set; }

        /// <summary>
        /// 优惠价 
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiscountPrice { get; set; }
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
        /// 商品组合属性集合 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityAttrStockDTO> CommodityStocks { get; set; }

        /// <summary>
        /// 活动类型 0：普通活动，1：秒杀，2：预约，3：拼团
        /// </summary>
        [DataMemberAttribute()]
        public ComPromotionStatusEnum PromotionTypeNew { get; set; }

        /// <summary>
        /// 活动sku属性集合
        /// </summary>
        [DataMemberAttribute()]
        public List<SkuActivityCDTO> SkuActivityCdtos { get; set; }
    }
}
