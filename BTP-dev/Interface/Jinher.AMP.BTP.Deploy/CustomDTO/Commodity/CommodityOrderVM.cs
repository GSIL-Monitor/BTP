using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class CommodityOrderVM
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMemberAttribute()]
        public string UserName { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid AppId { get; set; }

        /// <summary>
        /// 餐饮订单流水号
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityOrderCode { get; set; }
        /// <summary>
        /// 实收款
        /// </summary>
        [DataMemberAttribute()]
        public Decimal? CurrentPrice { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }

        /// <summary>
        /// 订单商品集合
        /// </summary>
        [DataMember]
        public List<OrderItemsVM> OrderItems { get; set; }

        /// <summary>
        /// 接收人Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReceiptUserId { get; set; }

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
        /// 卖家回复信息
        /// </summary>  
        [DataMemberAttribute()]
        public string MessageToBuyer { get; set; }
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
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string Street { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [DataMemberAttribute()]
        public string Address { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }
        /// <summary>
        /// 付款时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PaymentTime { get; set; }
        /// <summary>
        /// 确认收货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ConfirmTime { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ShipmentsTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal? Price { get; set; }

        /// <summary>
        /// 是否修改过价格
        /// </summary>
        [DataMemberAttribute()]
        public bool IsModifiedPrice { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }
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
        /// 退款物流公司
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 申请退款时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 达成协议时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? AgreementTime { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        [DataMemberAttribute()]
        public decimal Commission { get; set; }

        /// <summary>
        /// 众筹分红
        /// </summary>
        [DataMemberAttribute()]
        public decimal? CfDividend { get; set; }


        /// <summary>
        /// 是否众筹
        /// </summary>
        [DataMemberAttribute()]
        public bool IsCrowdfunding { get; set; }


        /// <summary>
        /// 卖家备注
        /// </summary>
        [DataMemberAttribute()]
        public string SellersRemark { get; set; }
        /// <summary>
        /// 原始物流公司
        /// </summary>
        [DataMember]
        public string OrgShipExpCo { get; set; }

        /// <summary>
        /// 原始快递单号
        /// </summary>
        [DataMember]
        public string OrgExpOrderNo { get; set; }

        /// <summary>
        /// 代金券支付金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal GoldCoupon { get; set; }
        /// <summary>
        /// 金币支付金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal GoldPrice { get; set; }
        /// <summary>
        /// 自提标志 0：非自提，1：自提
        /// </summary>
        [DataMemberAttribute()]
        public int SelfTakeFlag { get; set; }

        /// <summary>
        /// 推广者分成
        /// </summary>
        [DataMemberAttribute()]
        public long SpreadGold { get; set; }

        /// <summary>
        /// 使用优惠券金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal CouponValue { get; set; }
        /// <summary>
        /// 应用主分成
        /// </summary>
        [DataMemberAttribute()]
        public decimal OwnerShare { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        [DataMemberAttribute()]
        public int RefundType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundMoney { get; set; }
        /// <summary>
        /// 退货物流单号
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 售后订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateAfterSales { get; set; }
        /// <summary>
        /// 售后申请状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateRefundAfterSales { get; set; }

        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        [DataMemberAttribute()]
        public bool IsDelayConfirmTimeAfterSales { get; set; }
        /// <summary>
        /// 售中延长收货时间
        /// </summary>
        [DataMemberAttribute()]
        public bool IsDelayConfirmTime { get; set; }

        /// <summary>
        /// 自提地点所在省代码
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeProvinceCode { get; set; }

        /// <summary>
        /// 自提地点所在市代码
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeCityCode { get; set; }

        /// <summary>
        /// 自提地点所在区县代码
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeDistrictCode { get; set; }

        /// <summary>
        /// 自提地点所在省名称
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeProvinceName { get; set; }

        /// <summary>
        /// 自提地点所在市名称
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeCityName { get; set; }
        /// <summary>
        /// 自提地点所在区县名称
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeDistrictName { get; set; }
        /// <summary>
        /// 自提地址
        /// </summary>
        [DataMemberAttribute()]
        public string SelfTakeAddress { get; set; }

        /// <summary>
        /// 售中申请状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateRefund { get; set; }

        /// <summary>
        /// 售中申请状态
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 售后订单表最终修改时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ModifiedOnServiceAfterSales { get; set; }
        /// <summary>
        /// 分销商UserId
        /// </summary>
        [DataMember]
        public Guid? DistributorId { get; set; }
        /// <summary>
        /// 分销佣金
        /// </summary>
        [DataMember]
        public decimal DistributeMoney { get; set; }


        /// <summary>
        /// 花费积分抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal SpendScoreMoney { get; set; }

        /// <summary>
        /// 退还积分抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundScoreMoney { get; set; }

        /// <summary>
        /// 花费易捷币抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal SpendYJBMoney { get; set; }

        /// <summary>
        /// 易捷抵现卷金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal SpendYJCouponMoney { get; set; }

        /// <summary>
        /// 退还易捷币抵现金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal RefundYJBMoney { get; set; }

        /// <summary>
        /// OrderItemId 用户判断是否是单品退
        /// </summary>
        [DataMember]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 订单类型:0实体商品订单；1虚拟商品订单
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }

        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid? EsAppId { get; set; }

        /// <summary>
        /// 上传图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        [DataMemberAttribute()]
        public InvoiceDTO InvoiceInfo { get; set; }
        /// <summary>
        /// 下单昵称
        /// </summary>
        [DataMemberAttribute()]
        public string Uname { get; set; }
        /// <summary>
        /// 下单账号
        /// </summary>
        [DataMemberAttribute()]
        public string Ucode { get; set; }
        /// <summary>
        /// 获取OrderShipping表的条数，判断是否修改过物流
        /// </summary>
        [DataMember]
        public int Wlsl { get; set; }
        /// <summary>
        /// 渠道分享佣金
        /// </summary>
        [DataMember]
        public decimal ChannelShareMoney { get; set; }

        /// <summary>
        /// App自提点id
        /// </summary>
        [DataMember]
        public Guid? SelfTakeStationId { get; set; }
        /// <summary>
        /// 提货人姓名
        /// </summary>
        [DataMember]
        public string PickUpName { get; set; }
        /// <summary>
        /// 提货人联系方式
        /// </summary>
        [DataMember]
        public string PickUpPhone { get; set; }
        /// <summary>
        /// 预约提货日期
        /// </summary>
        [DataMember]
        public DateTime? PickUpBookDate { get; set; }
        /// <summary>
        /// 预约提货开始时间
        /// </summary>
        [DataMember]
        public TimeSpan? PickUpBookStartTime { get; set; }
        /// <summary>
        /// 预约提货截止时间
        /// </summary>
        [DataMember]
        public TimeSpan? PickUpBookEndTime { get; set; }

        /// <summary>
        /// 运费减免
        /// </summary>
        [DataMember]
        public decimal FreightDiscount { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }


        /// <summary>
        ///发货单打印次数
        /// </summary>
        [DataMember]
        public int? InvoicePrintCount { get; set; }

        /// <summary>
        /// 快递单打印次数
        /// </summary>
        [DataMember]
        public int? ExpressPrintCount { get; set; }

        /// <summary>
        /// AppName
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>  
        [DataMember]
        public string AppType { get; set; }


        /// <summary>
        ///中石化电子发票编码
        /// </summary>
        [DataMember]
        public string SwNo { get; set; }

        /// <summary>
        ///是否开红票
        /// </summary>
        [DataMember]
        public bool IsRefund { get; set; }

        public short? CancelReasonCode { get; set; }

        /// <summary>
        /// 优惠套装Id
        /// </summary>
        [DataMember]
        public Guid SetMealId { get; set; }


        /// <summary>
        /// 金采团购活动Id
        /// </summary>
        [DataMember]
        public Guid JcActivityId { get; set; }


        /// <summary>
        ///  规格设置
        /// </summary>
        public int Specifications { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public int IsExistence { get; set; }


        /// <summary>
        /// 传的url地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 获取或设置 返还的油卡兑换券价值
        /// </summary>
        [DataMember]
        public decimal? ReturnYoukaMoney { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// RealPrice
        /// </summary>
        [DataMember]
        public decimal? RealPrice { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }

        /// <summary>
        /// PaymentState
        /// </summary>
        [DataMember]
        public bool PaymentState { get; set; }

        /// <summary>
        /// IsCrowdfunding
        /// </summary>
        [DataMember]
        public int Crowdfunding { get; set; }

        /// <summary>
        /// CrowdfundingPrice
        /// </summary>
        [DataMember]
        public decimal? CrowdfundingPrice { get; set; }

        /// <summary>
        /// ScoreState
        /// </summary>
        [DataMember]
        public int ScoreState { get; set; }

        /// <summary>
        /// FirstContent
        /// </summary>
        [DataMember]
        public string FirstContent { get; set; }

        /// <summary>
        /// SecondContent
        /// </summary>
        [DataMember]
        public string SecondContent { get; set; }

        /// <summary>
        /// ThirdContent
        /// </summary>
        [DataMember]
        public string ThirdContent { get; set; }

        /// <summary>
        /// SrcType
        /// </summary>
        [DataMember]
        public int? SrcType { get; set; }

        /// <summary>
        /// SrcTagId
        /// </summary>
        [DataMember]
        public Guid? SrcTagId { get; set; }

        /// <summary>
        /// SrcTagId
        /// </summary>
        [DataMember]
        public string CPSId { get; set; }


        /// <summary>
        /// CancelReason
        /// </summary>
        [DataMember]
        public string CancelReason { get; set; }

        /// <summary>
        /// IsDel
        /// </summary>
        [DataMember]
        public int IsDel { get; set; }

        /// <summary>
        /// SpreaderId
        /// </summary>
        [DataMember]
        public Guid? SpreaderId { get; set; }

        /// <summary>
        /// SpreadCode
        /// </summary>
        [DataMember]
        public Guid? SpreadCode { get; set; }

        /// <summary>
        /// SrcAppId
        /// </summary>
        [DataMember]
        public Guid? SrcAppId { get; set; }

        /// <summary>
        /// ServiceId
        /// </summary>
        [DataMember]
        public Guid? ServiceId { get; set; }

        /// <summary>
        /// SaiCode
        /// </summary>
        [DataMember]
        public string SaiCode { get; set; }

        /// <summary>
        /// MealBoxFee
        /// </summary>
        [DataMember]
        public decimal? MealBoxFee { get; set; }

        /// <summary>
        /// SupplierName
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// SupplierName
        /// </summary>
        [DataMember]
        public string SupplierCode { get; set; }

        /// <summary>
        /// SupplierName
        /// </summary>
        [DataMember]
        public short? SupplierType { get; set; }

        /// <summary>
        /// SupplierName
        /// </summary>
        [DataMember]
        public short? ShipperType { get; set; }

        /// <summary>
        /// HasStatisYJInfo
        /// </summary>
        [DataMember]
        public bool? HasStatisYJInfo { get; set; }

        /// <summary>
        /// TechSpecs
        /// </summary>
        [DataMember]
        public string TechSpecs { get; set; }

        /// <summary>
        /// SaleService
        /// </summary>
        [DataMember]
        public string SaleService { get; set; }

        /// <summary>
        /// DeliveryTime
        /// </summary>
        [DataMember]
        public DateTime? DeliveryTime { get; set; }

        /// <summary>
        /// DeliveryDays
        /// </summary>
        [DataMember]
        public int? DeliveryDays { get; set; }

        /// <summary>
        /// MQAppType
        /// </summary>
        [DataMember]
        public short? MQAppType { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        ///金彩支付回款时间
        /// </summary>
        public DateTime? JCZFRefundDate { get; set; }

        /// <summary>
        /// 金彩支付最后回款类型
        /// </summary>
        public int? JCZFRefundType { get; set; }

        /// <summary>
        /// 金彩支付回款总金额
        /// </summary>
        public decimal? JCZFRefundAmount { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        [DataMember]
        public Guid? CustomerInfo { get; set; }
        /// <summary>
        /// 易捷卡抵用金额
        /// </summary>
        [DataMember]
        public decimal? YJCardPrice { get; set; }
    }
}
