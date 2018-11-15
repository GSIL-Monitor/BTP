using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderItemsVM
    {
        [DataMemberAttribute()]
        public Guid Id { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品名字
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityIdName { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 商品原价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品积分抵现价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal ScorePrice { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int Number { get; set; }
        /// <summary>
        /// 商品尺寸颜色ID
        /// </summary>
        [DataMemberAttribute()]
        public string SizeAndColorId { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        [Obsolete("已过期")]
        [DataMemberAttribute()]
        public List<string> CommodityCategorys { get; set; }

        /// <summary>
        /// 已选商品属性列表 
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttibuteDTO> SelectedComAttibutes { get; set; }

        /// <summary>
        /// 商品实际卖出价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? RealPrice { get; set; }

        /// <summary>
        /// 是否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        [DataMember]
        public string PromotionDesc { get; set; }

        /// <summary>
        /// 商品下单分类
        /// </summary>
        [DataMember]
        public string ComCategoryName { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }

        /// <summary>
        /// 京东商品编号
        /// </summary>
        [DataMember]
        public string JdCode { get; set; }

        /// <summary>
        /// 京东订单号或第三方电商物流单号
        /// </summary>
        [DataMember]
        public string JdorderId { get; set; }

        /// <summary>
        /// 京东发货时间或第三方电商物流发货时间
        /// </summary>
        [DataMember]
        public DateTime JdfhTime { get; set; }

        /// <summary>
        /// 第三方电商物流公司
        /// </summary>
        [DataMember]
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 易捷卡密信息列表
        /// </summary>
        [DataMember]
        public List<Deploy.YJBJCardDTO> YJBJCardList { get; set; }

        /// <summary>
        /// 赠品信息
        /// </summary>
        [DataMember]
        public List<OrderItemPresentsVM> Presents { get; set; }

        /// <summary>
        /// 优惠券拆分金额
        /// </summary>
        [DataMember]
        public decimal CouponPrice { get; set; }

        /// <summary>
        /// 运费拆分金额
        /// </summary>
        [DataMember]
        public decimal FreightPrice { get; set; }

        /// <summary>
        /// 易捷币拆分金额
        /// </summary>
        [DataMember]
        public decimal YjbPrice { get; set; }

        /// <summary>
        /// 改价运费拆分金额
        /// </summary>
        [DataMember]
        public decimal ChangeFreightPrice { get; set; }

        /// <summary>
        /// 商品改价拆分金额
        /// </summary>
        [DataMember]
        public decimal ChangeRealPrice { get; set; }

        /// <summary>
        /// 订单项退款状态 0未退款 1退款中 2已退款 3达成退货协议
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 退货物流公司
        /// </summary>
        [DataMember]
        public string RefundExpCo { get; set; }

        /// <summary>
        /// 退货物流单号
        /// </summary>
        [DataMember]
        public string RefundExpOrderNo { get; set; }

        /// <summary>
        /// 京东订单状态(1-预占 2-拆分 3-拒收 4-妥投)
        /// </summary>
        [DataMember]
        public int JdState { get; set; }

        /// <summary>
        /// 订单项ID 若存在退款则赋值 否则为
        /// </summary>
        [DataMemberAttribute()]
        public Guid RefundOrderItemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int StateRefund { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int StateAfterSales { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int RefundType { get; set; }

        //实时订单数据新加DTO
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// SubTime
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }

        /// <summary>
        /// CurrentPrice
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// AlreadyReview
        /// </summary>
        [DataMember]
        public bool AlreadyReview { get; set; }

        /// <summary>
        /// ModifiedOn
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// PromotionId
        /// </summary>
        [DataMember]
        public Guid? PromotionId { get; set; }

        /// <summary>
        /// PromotionType
        /// </summary>
        [DataMember]
        public int? PromotionType { get; set; }
        /// <summary>
        /// ShareMoney
        /// </summary>
        [DataMember]
        public decimal ShareMoney { get; set; }
        /// <summary>
        /// Specifications
        /// </summary>
        [DataMember]
        public int? Specifications { get; set; }
        /// <summary>
        /// State_Value
        /// </summary>
        [DataMember]
        public string State_Value { get; set; }
        /// <summary>
        /// SubCode
        /// </summary>
        [DataMember]
        public string SubCode { get; set; }
        /// <summary>
        /// TaxRate
        /// </summary>
        [DataMember]
        public decimal? TaxRate { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        [DataMember]
        public int? Type { get; set; }
        /// <summary>
        /// Unit
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// VipLevelId
        /// </summary>
        [DataMember]
        public Guid? VipLevelId { get; set; }
        /// <summary>
        /// YJCouponActivityId
        /// </summary>
        [DataMember]
        public string YJCouponActivityId { get; set; }
        /// <summary>
        /// YJCouponPrice
        /// </summary>
        [DataMember]
        public decimal? YJCouponPrice { get; set; }
        /// <summary>
        /// YouKaPercent
        /// </summary>
        [DataMember]
        public decimal? YouKaPercent { get; set; }
        /// <summary>
        /// Barcode
        /// </summary>
        [DataMember]
        public string Barcode { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// ComAttributeIds
        /// </summary>
        [DataMember]
        public string ComAttributeIds { get; set; }
        /// <summary>
        /// ComCategoryId
        /// </summary>
        [DataMember]
        public Guid? ComCategoryId { get; set; }
        /// <summary>
        /// CommodityAttributes
        /// </summary>
        [DataMember]
        public string CommodityAttributes { get; set; }
        /// <summary>
        /// CommodityStockId
        /// </summary>
        [DataMember]
        public Guid? CommodityStockId { get; set; }
        /// <summary>
        /// CostPrice
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// DeliveryDays
        /// </summary>
        [DataMember]
        public int? DeliveryDays { get; set; }
        /// <summary>
        /// DeliveryTime
        /// </summary>
        [DataMember]
        public DateTime? DeliveryTime { get; set; }
        /// <summary>
        /// DiscountPrice
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// ErQiCode
        /// </summary>
        [DataMember]
        public string ErQiCode { get; set; }
        /// <summary>
        /// HasPresent
        /// </summary>
        [DataMember]
        public bool? HasPresent { get; set; }
        /// <summary>
        /// InnerCatetoryIds
        /// </summary>
        [DataMember]
        public string InnerCatetoryIds { get; set; }
        /// <summary>
        /// InputRax
        /// </summary>
        [DataMember]
        public decimal? InputRax { get; set; }
        /// <summary>
        /// Intensity
        /// </summary>
        [DataMember]
        public decimal? Intensity { get; set; }
        /// <summary>
        /// No_Code
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 方正商品SN码
        /// </summary>
        [DataMember]
        public string SNCode { get; set; }

    }

    [Serializable()]
    [DataContract]
    public class OrderItemPresentsVM
    {
        [DataMember]
        public string PicturesPath { get; set; }

        [DataMember]
        public string CommodityName { get; set; }

        [DataMember]
        public string ComCategoryName { get; set; }

        [DataMember]
        public string SizeAndColorId { get; set; }

        [DataMember]
        public decimal? RealPrice { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int Number { get; set; }
    }
}
