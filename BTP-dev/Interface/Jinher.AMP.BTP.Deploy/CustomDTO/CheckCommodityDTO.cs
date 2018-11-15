using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品校验
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CheckCommodityDTO
    {
        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMemberAttribute()]
        public int Type { get; set; }
        /// <summary>
        /// 商品价格 实际价格 有可能是优惠价 折扣价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 状态(0 上架，1下架，3删除，4属性改变)
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        [DataMemberAttribute()]
        public decimal OPrice { get; set; }

        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int LimitBuyEach { get; set; }

        /// <summary>
        /// 促销商品总数
        /// </summary>
        [DataMemberAttribute()]
        public int LimitBuyTotal { get; set; }

        /// <summary>
        /// 促销商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int SurplusLimitBuyTotal { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public Guid? CommodityStockId { get; set; }

        /// <summary>
        /// 是否需要预约
        /// </summary>
        [DataMemberAttribute()]
        public bool IsNeedPresell { get; set; }

        /// <summary>
        /// 是否已经预约
        /// </summary>
        [DataMemberAttribute()]
        public bool IsPreselled { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

        /// <summary>
        /// 剩余可入团人数
        /// </summary>
        [DataMemberAttribute()]
        public int DiyJoinCountSurplus { get; set; }

        /// <summary>
        ///距离参团结束剩余时间 单位(秒)
        /// </summary>
        [DataMemberAttribute()]
        public int DiySecondSurplus { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品使用的活动类型
        /// </summary>
        [DataMemberAttribute()]
        public ComPromotionStatusEnum ComPromotionStatusEnum { get; set; }
 
        /// <summary>
        /// 属性
        /// </summary>
        [DataMemberAttribute()]
        public string ColorAndSize { get; set; }

        /// <summary>
        /// 包装规格设置
        /// </summary>
        public int Specifications { get; set; }
    }

    /// <summary>
    /// 商品校验
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityIdAndStockId
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public Guid? CommodityStockId { get; set; }

        /// <summary>
        /// 外部活动Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid? OutPrommotionId { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        [DataMemberAttribute()]
        public string ColorAndSize { get; set; }

        /// <summary>
        /// 规格Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid SpecificationsId { get; set; }


        /// <summary>
        /// 包装规格
        /// </summary>
        [DataMemberAttribute()]
        public int? Specifications { get; set; }

    }

    /// <summary>
    /// 购物车商品校验
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ShoppingCartItem
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShoppingCartItemId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public Guid? CommodityStockId { get; set; }

        /// <summary>
        /// 外部活动Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid? OutPrommotionId { get; set; }
    }

    /// <summary>
    /// 购物车商品校验
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CheckShopCommodityDTO : CheckCommodityDTO
    {
        /// <summary>
        /// 购物车Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShoppingCartItemId { get; set; }

        /// <summary>
        /// 金采团购活动Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid JcActivityId { get; set; }

        /// <summary>
        /// 购物车商品状态
        /// </summary>
        [DataMemberAttribute()]
        public ShopCartStateEnum ShopCartState { get; set; }

        public CheckShopCommodityDTO Clone()
        {
            return this.MemberwiseClone() as CheckShopCommodityDTO;
        }
    }
}
