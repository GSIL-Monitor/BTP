using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单查看DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityOrderSDTO
    {
        

        /// <summary>
        /// AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMemberAttribute]
        public string Code { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>  
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }
        /// <summary>
        /// 买家备注
        /// </summary> 
        [DataMemberAttribute()]
        public string Details { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        [DataMemberAttribute()]
        public int Payment { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 付款方式描述信息
        /// </summary>
        [DataMemberAttribute()]
        public string PaymentName { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [DataMemberAttribute()]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [DataMemberAttribute()]
        public string Street { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }

        /// <summary>
        /// 未付款到期关闭时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ExpirePayTime { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }
        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderListItemCDTO> ShoppingCartItemSDTO { get; set; }


        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ShipmentsTime { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        [DataMemberAttribute()]
        public string ShipExpCo { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        [DataMemberAttribute()]
        public string ExpOrderNo { get; set; }
        /// <summary>
        /// 物流子单号集合[字符串]
        /// </summary>
        [DataMemberAttribute()]
        public string SubExpressNos { get; set; }


        /// <summary>
        /// 退货金额（售后时存OrderRefundAfterSales.RefundMoney）
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundMoney { get; set; }
        /// <summary>
        /// 退款详细说明（售后时存OrderRefundAfterSales.RefundDesc）
        /// </summary>
        [DataMemberAttribute()]
        public string RefundDesc { get; set; }
        /// <summary>
        /// 收款人帐号
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiverAccount { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        [DataMemberAttribute()]
        public string Receiver { get; set; }
        /// <summary>
        /// 是否延长收获时间 
        /// </summary>
        [DataMemberAttribute()]
        public bool IsDelayConfirmTime { get; set; }
        /// <summary>
        /// 退款物流公司（售后时存OrderRefundAfterSales.RefundExpCo）
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 退款物流单号（售后时存OrderRefundAfterSales.RefundExpOrderNo）
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 客户取消订单原因
        /// </summary>
        [DataMemberAttribute()]
        public string MessageToBuyer { get; set; }
        /// <summary>
        /// 是否删除订单
        /// </summary>
        [DataMemberAttribute()]
        public int IsDel { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        [DataMemberAttribute()]
        public int RefundType { get; set; }
        /// <summary>
        /// 订单运费
        /// </summary>
        [DataMemberAttribute()]
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
        /// 发票类型
        /// </summary>
        [DataMemberAttribute()]
        [Obsolete("已废弃,参见InvoiceDTO", false)]
        public int InvoiceType { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMemberAttribute()]
        [Obsolete("已废弃,参见InvoiceDTO", false)]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 发票内容
        /// </summary>
        [DataMemberAttribute()]
        [Obsolete("已废弃,参见InvoiceDTO", false)]
        public string InvoiceContent { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        ///订单中使用金币抵用的金额。
        /// </summary>
        [DataMemberAttribute()]
        public Decimal GoldPrice { get; set; }

        /// <summary>
        ///订单中使用代金券抵用的金额。
        /// </summary>
        [DataMemberAttribute()]
        public Decimal GoldCoupon { get; set; }

        /// <summary>
        /// 优惠券抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public Decimal CouponValue { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CouponId { get; set; }

        /// <summary>
        /// 使用优惠券的商品Id,如果为空值，则没有优惠券或优惠券作用在店铺上
        /// </summary>
        [DataMemberAttribute()]
        public Guid CouponComId { get; set; }

        /// <summary>
        /// 自提标识
        /// </summary>
        [DataMemberAttribute()]
        public int SelfTakeFlag { get; set; }

        /// <summary>
        /// 订单提货码
        /// </summary>
        [DataMemberAttribute()]
        public string PickUpCode { get; set; }
        /// <summary>
        /// 订单提货人
        /// </summary>
        [DataMemberAttribute()]
        public string PickUpName { get; set; }
        /// <summary>
        /// 订单提货人手机号
        /// </summary>
        [DataMemberAttribute()]
        public string PickUpPhone { get; set; }
        /// <summary>
        /// 预约提货日期
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PickUpTime { get; set; }
        /// <summary>
        /// 预约提货开始时间
        /// </summary>
        [DataMemberAttribute()]
        public TimeSpan ? PickUpStartTime{ get; set; }
        /// <summary>
        /// 预约提货开始时间
        /// </summary>
        [DataMemberAttribute()]
        public TimeSpan ? PickUpEndTime { get; set; }
        /// <summary>
        /// 提货二维码Url
        /// </summary>
        [DataMemberAttribute()]
        public string PickUpCodeUrl { get; set; }
        /// <summary>
        /// 自提点地址
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeAddress { get; set; }
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeName { get; set; }
        /// <summary>
        /// 自提点联系电话
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakePhone { get; set; }

        /// <summary>
        /// 售后状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateAfterSales { get; set; }

        /// <summary>
        /// 售后流水状态
        /// </summary>
        [DataMemberAttribute()]
        public int OrderRefundAfterSalesState { get; set; }

        /// <summary>
        /// 0:出库中达成协议，1：已发货达成协议
        /// </summary>
        [DataMemberAttribute()]
        public int? AgreeFlag { get; set; }

        /// <summary>
        /// 申请表状态
        /// </summary>
        [DataMemberAttribute()]
        public int OrderRefundState { get; set; }

        /// <summary>
        /// 积分抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal ScorePrice { get; set; }

        /// <summary>
        /// 易捷币抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal? YJBPrice { get; set; }

        /// <summary>
        /// 易捷卡抵用资金
        /// </summary>
        [DataMemberAttribute()]
        public decimal? YJCardPrice { get; set; }

        

        /// <summary>
        /// 易捷抵用券抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal YJCouponPrice { get; set; }


        /// <summary>
        /// 退还积分抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundScoreMoney { get; set; }

        /// <summary>
        /// 订单类型:0实体商品订单；1虚拟商品订单
        /// </summary>
        [DataMemberAttribute]
        public int OrderType { get; set; }

        /// <summary>
        /// 上传图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        [DataMemberAttribute]
        public InvoiceDTO InvoiceDTO { get; set; }
        /// <summary>
        /// 餐饮订单流水号
        /// </summary>
        [DataMemberAttribute]
        public string Batch { get; set; }

        /// <summary>
        /// 餐盒费用
        /// </summary>
        [DataMemberAttribute]
        public decimal MealBoxFee { get; set; }

        /// <summary>
        /// 满免除运费
        /// </summary>
        [DataMemberAttribute]
        public decimal FreeAmount { get; set; }

        /// <summary>
        /// 配送费折扣信息费用
        /// </summary>
        [DataMemberAttribute]
        public decimal DeliveryFeeDiscount { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        [DataMemberAttribute]
        public decimal Duty { get; set; }

        /// <summary>
        /// 中石化电子发票编码
        /// </summary>
        [DataMemberAttribute]
        public string SwNo { get; set; }

        /// <summary>
        /// 优惠套装信息
        /// </summary>
        [DataMemberAttribute]
        public Jinher.AMP.ZPH.Deploy.CustomDTO.SetMealActivityCDTO SetMealActivity { get; set; }

        /// <summary>
        /// 是否是金采团购活动订单
        /// </summary>
        [DataMemberAttribute]
        public bool IsJcActivity { get; set; }

        /// <summary>
        /// 订单修改时间
        /// </summary>
        [DataMemberAttribute]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMemberAttribute]
        public short? AppType { get; set; }
    }

    /// <summary>
    /// 好运来活动订单DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class LotteryOrderInfoDTO
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMemberAttribute]
        public string Code { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>  
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [DataMemberAttribute()]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [DataMemberAttribute()]
        public string Street { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }

        /// <summary>
        /// 奖品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }

    }

    /// <summary>
    /// 订单查看DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityOrderStateDTO
    {

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 实收价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? RealPrice { get; set; }
        /// <summary>
        /// 订单运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }

        /// <summary>
        /// 是否修改过订单价格
        /// </summary>
        [DataMemberAttribute()]
        public bool IsModifiedPrice { get; set; }

    }

    [Serializable()]
    [DataContract]
    public class CommodityOrderShareDTO
    {
        /// <summary>
        /// AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 订单商品--订单分享时用到
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderItemShareCDTO> ShareOrderItemDTO { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

    }
}
