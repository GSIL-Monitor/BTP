using System;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 购物车商品项
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartCommoditySDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// App名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int? Stock { get; set; }
        /// <summary>
        /// 商品尺寸
        /// </summary>
        [DataMemberAttribute()]
        public string Size { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        [DataMemberAttribute()]
        public int? State { get; set; }

        /// <summary>
        /// 商品数量 - 购物车显示
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }
        /// <summary>
        /// 添加购物车时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime AddShopCartTime { get; set; }
        /// <summary>
        /// 购物车ID --获取购物车时返回
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShopCartItemId { get; set; }
        /// <summary>
        /// 属性ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityStockId { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal? Intensity { get; set; }
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
        /// 购物车商品状态
        /// </summary>
        [DataMemberAttribute()]
        public ShopCartStateEnum ShopCartState { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        [DataMemberAttribute()]
        public string ShopCartStateDesc { get; set; }

        /// <summary>
        /// 是否参与sku价格
        /// </summary>
        [DataMemberAttribute()]
        public bool IsJoin { get; set; }

        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMemberAttribute()]
        public int Type { get; set; }

        /// <summary>
        /// 赠品信息
        /// </summary>
        [DataMember]
        public ShopCartCommodiyPresentDTO Present { get; set; }

        /// <summary>
        /// 金采团购活动
        /// </summary>
        [DataMemberAttribute()]
        public Guid JcActivityId { get; set; }


        /// <summary>
        ///包装规格设置信息
        /// </summary>
        [DataMember]
        public Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO Specifications { get; set; }


        /// <summary>
        ///包装规格全部信息
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist { get; set; }

        /// <summary>
        /// 商品标签
        /// </summary>
        [DataMember]
        public List<string> Tags { get; set; }

        /// <summary>
        /// 商品组合属性集合 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityAttrStockDTO> CommodityStocks { get; set; }

        /// <summary>
        /// 商品属性列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttributeDTO> ComAttibutes { get; set; }
    }

    /// <summary>
    /// 购物车列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartListDTO
    {
        /// <summary>
        /// 店铺商品列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ShopCartOfShopDto> ShopList { get; set; }

        /// <summary>
        /// 失效商品列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ShopCartCommoditySDTO> InvalidList { get; set; }
    }

    /// <summary>
    /// 购物车店铺商品列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartOfShopDto
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }

        /// <summary>
        /// 是否有可领取优惠券
        /// </summary>
        [DataMemberAttribute()]
        public bool IsHasCoupon { get; set; }

        /// <summary>
        /// 优惠券列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ShopCartCouponDTO> CouponList { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ShopCartCommoditySDTO> List { get; set; }
    }

    /// <summary>
    /// 商品赠品
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopCartCommodiyPresentDTO
    {
        ///// <summary>
        ///// 赠品标题
        ///// </summary>
        //[DataMember]
        //public string Title { get; set; }

        /// <summary>
        /// 商品单次最少购买数量
        /// </summary>
        [DataMember]
        public int Limit { get; set; }

        ///// <summary>
        ///// 开始时间
        ///// </summary>
        //[DataMember]
        //public DateTime BeginTime { get; set; }

        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //[DataMember]
        //public DateTime EndTime { get; set; }

        /// <summary>
        /// 参加活动的SKUId
        /// </summary>
        [DataMember]
        public List<Guid> CommodityStockIds { get; set; }

        ///// <summary>
        ///// 是否全部赠送
        ///// </summary>
        //[DataMember]
        //public bool IsAll { get; set; }

        /// <summary>
        /// 赠品商品
        /// </summary>
        [DataMember]
        public List<CommodiyPresentItem> Items { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ShopCartCouponDTO : Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplatDetailDTO
    {
        /// <summary>
        /// 时间格式化
        /// </summary>
        [DataMember]
        public string EndTimeStr { get; set; }

        ///// <summary>
        /////已领张数
        ///// </summary>
        //[DataMember]
        //public int UseNum { get; set; }
    }
}
