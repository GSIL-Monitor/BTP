using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class OrderListCDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderListItemCDTO> ShoppingCartItemSDTO { get; set; }
        /// <summary>
        /// 订单商品总数
        /// </summary>
        [DataMemberAttribute()]
        public int ItemAllCount { get; set; }
        //
        // 摘要:
        //     未付款订单数量
        [DataMember]
        public int totalState0 { get; set; }
        //
        // 摘要:
        //     未发货订单数量
        [DataMember]
        public int totalState1 { get; set; }
        //
        // 摘要:
        //     已发货订单数量
        [DataMember]
        public int totalState2 { get; set; }
        //
        // 摘要:
        //     已完成
        [DataMember]
        public int totalState3 { get; set; }
        //
        // 摘要:
        //     退款中的订单数量
        [DataMember]
        public int totalStateTui { get; set; }
        //
        // 摘要:
        //     订单状态
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        public decimal Freight { get; set; }

        /// <summary>
        /// 是否修改过订单价格
        /// </summary>
        [DataMemberAttribute()]
        public bool IsModifiedPrice { get; set; }

        /// <summary>
        /// 订单原始总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal OriginPrice { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ShipmentsTime { get; set; }

        /// <summary>
        ///  AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMemberAttribute()]
        public int AppType { get; set; }
        /// <summary>
        ///  UserID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        ///  应用名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }

        /// <summary>
        ///  支付方式
        /// </summary>
        [DataMemberAttribute()]
        public int PayType { get; set; }

        /// <summary>
        /// 自提标识
        /// </summary>
        [DataMemberAttribute()]
        public int SelfTakeFlag { get; set; }

        /// <summary>
        /// 售后订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateAfterSales { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        [DataMemberAttribute()]
        public int OrderRefundState { get; set; }

        /// <summary>
        /// 售后申请表状态
        /// </summary>
        [DataMemberAttribute()]
        public int OrderRefundAfterSalesState { get; set; }

        /// <summary>
        /// 订单类型:0实体商品订单；1虚拟商品订单
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [DataMember]
        public string Batch { get; set; }

        [DataMember]
        public Nullable<System.DateTime> PaymentTime { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [DataMemberAttribute]
        public string ShipExpCo { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        [DataMemberAttribute]
        public string ExpOrderNo { get; set; }
        /// <summary>
        /// 积分抵用金额
        /// </summary>
        [DataMemberAttribute]
        public decimal ScorePrice { get; set; }
        /// <summary>
        /// 易捷币抵用金额
        /// </summary>
        [DataMemberAttribute]
        public decimal? YJBPrice { get; set; }

        /// <summary>
        /// 第三方订单数据实体
        /// </summary>
        [DataMember]
        public string CommodityEntity { get; set; }

        /// <summary>
        /// 第三方订单状态名称(字符串类型)
        /// </summary>
        [DataMember]
        public string DSFStateName { get; set; }
        /// <summary>
        /// 优惠券抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public Decimal CouponValue { get; set; }
        /// <summary>
        /// 是否是第三方订单
        /// </summary>
        [DataMember]
        public bool IsThirdOrder { get; set; }

        /// <summary>
        /// 第三方订单详情页地址
        /// </summary>
        [DataMember]
        public string ThirdMobileUrl { get; set; }
    }
}
