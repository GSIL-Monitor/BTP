using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderListItemCDTO
    {
        /// <summary>
        /// 订单ID --获取订单商品时
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单项id（OrderItemId） 
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
        /// 商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int Number { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        //
        // 摘要:
        //     是否评论 1-已评论，0-未评论
        //[DataMember]
        //public int IsComments { get; set; }
        //
        // 摘要:
        //     商品数量--订单和购物车显示
        [DataMember]
        public int CommodityNumber { get; set; }
        //
        // 摘要:
        //     商品图片
        [DataMember]
        public string Pic { get; set; }
        //
        // 摘要:
        //     商品尺寸
        [DataMember]
        public string Size { get; set; }
        /// <summary>
        /// 是否已评价
        /// </summary>
        [DataMember]
        public bool HasReview { get; set; }

        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 实际支付价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal RealPrice { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品下单分类
        /// </summary>
        [DataMember]
        public string ComCategoryName { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public string CommodityAttributes { get; set; }

        /// <summary>
        /// 商品下单分类
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }

        /// <summary>
        /// 京东订单id
        /// </summary>
        [DataMember]
        public string JdOrderid { get; set; }

        /// <summary>
        /// 京东SkuId
        /// </summary>
        [DataMember]
        public string JdSkuId { get; set; }

        /// <summary>
        /// 京东订单id
        /// </summary>
        [DataMember]
        public Jinher.AMP.BTP.Deploy.Enum.JdEnum JdOrderStatus { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public DateTime? _DeliveryTime { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public int? _DeliveryDays { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMember]
        public string DeliveryTime { get; set; }

        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 易捷卡密信息列表
        /// </summary>
        [DataMember]
        public List<YJBJCardDTO> YJBJCardList { get; set; }

        /// <summary>
        /// 赠品信息
        /// </summary>
        [DataMember]
        public List<OrderListItemPresentDTO> Presents { get; set; }


        /// <summary>
        /// 规格设置
        /// </summary>
        public int? Specifications { get; set; }

        /// <summary>
        /// 优惠券抵用金额
        /// </summary>
        [DataMember]
        public decimal CouponPrice { get; set; }

        /// <summary>
        /// 运费拆分金额
        /// </summary>
        [DataMember]
        public decimal FreightPrice { get; set; }

        /// <summary>
        /// 易捷币抵用金额
        /// </summary>
        [DataMember]
        public decimal YjbPrice { get; set; }

        /// <summary>
        /// 运费改价拆分金额
        /// </summary>
        [DataMember]
        public decimal ChangeFreightPrice { get; set; }

        /// <summary>
        /// 商品改价拆分金额
        /// </summary>
        [DataMember]
        public decimal ChangeRealPrice { get; set; }

        /// <summary>
        /// 商品退款状态
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 易捷抵用券
        /// </summary>
        [DataMember]
        public decimal YJCouponPrice { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        [DataMember]
        public string ExpressNo { get; set; }
        /// <summary>
        /// 物流子单号
        /// </summary>
        [DataMember]
        public string SubExpressNos { get; set; }



        /// <summary>
        /// 苏宁订单Idid
        /// </summary>
        [DataMember]
        public string SnOrderid { get; set; }
        /// <summary>
        /// 苏宁订单列表Id
        /// </summary>
        [DataMember]
        public string SnOrderItemId { get; set; }
        /// <summary>
        /// 苏宁SkuId
        /// </summary>
        [DataMember]
        public string SnSkuId { get; set; }
        /// <summary>
        /// 苏宁物流信息，对应
        /// 1.商品出库，2. 商品妥投  3.商品拒收 ， 4.商品退货 
        /// https://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.message.get
        /// </summary>
        [DataMember]
        public int SnExpressStatus { get; set; }
        /// <summary>
        /// 苏宁订单状态
        ///  1:审核中; 2:待发货; 3:待收货; 4:已完成; 5:已取消; 6:已退货; 7:待处理; 8：审核不通过，订单已取消; 9：待支付
        /// </summary>
        [DataMember]
        public int SNOrderStatus { get; set; }
        

    }

    [Serializable()]
    [DataContract]
    public class OrderItemShareCDTO : OrderListItemCDTO
    {
        /// <summary>
        ///  获得商品展示价格
        /// </summary>
        [DataMember]
        public decimal ShowRealPrice { get; set; }

        /// <summary>
        /// 获得商品展示原价（中划线价格）
        /// </summary>
        [DataMember]
        public decimal? ShowOriPrice { get; set; }


        /// <summary>
        ///商品市场价格
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        ///商品价格
        /// </summary>
        [DataMember]
        public decimal CommodityPrice { get; set; }
    }

    [Serializable]
    [DataContract]
    public class OrderListItemPresentDTO
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        [DataMember]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 订单ID --获取订单商品时
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单商品ID 
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
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        //
        // 摘要:
        //     商品数量--订单和购物车显示
        [DataMember]
        public int CommodityNumber { get; set; }
        //
        // 摘要:
        //     商品图片
        [DataMember]
        public string Pic { get; set; }
        //
        // 摘要:
        //     商品尺寸
        [DataMember]
        public string Size { get; set; }
        /// <summary>
        /// 是否已评价
        /// </summary>
        [DataMember]
        public bool HasReview { get; set; }

        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 实际支付价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal RealPrice { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品下单分类
        /// </summary>
        [DataMember]
        public string ComCategoryName { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public string CommodityAttributes { get; set; }

        /// <summary>
        /// 商品下单分类
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }

        /// <summary>
        /// 京东订单id
        /// </summary>
        [DataMember]
        public string JdOrderid { get; set; }

        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMember]
        public int Type { get; set; }
    }
}
