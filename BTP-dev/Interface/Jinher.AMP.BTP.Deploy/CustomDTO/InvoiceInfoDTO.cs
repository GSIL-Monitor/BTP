using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 发票信息类
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceInfoDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// CommodityOrderId
        /// </summary>
        [DataMember]
        public Guid CommodityOrderId { get; set; }
        /// <summary>
        /// InvoiceTitle
        /// </summary>
        [DataMember]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// InvoiceContent
        /// </summary>
        [DataMember]
        public string InvoiceContent { get; set; }
        /// <summary>
        /// 0不开发票，1个人，2公司
        /// </summary>
        [DataMember]
        public Int32 InvoiceType { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 电子发票收票人手机号
        /// </summary>
        [DataMember]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 电子发票收票人邮箱
        /// </summary>
        [DataMember]
        public string ReceiptEmail { get; set; }
        /// <summary>
        /// 发票状态：0:待付款，1:待开票,2:已开票,3:已发出,4:已作废
        /// </summary>
        [DataMember]
        public Int32 State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 发票类型 0:增值税普通发票,1:电子发票,2:增值税专用发票
        /// </summary>
        [DataMember]
        public Int32 Category { get; set; }
        /// <summary>
        /// 提交人Id（即买家）
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 发票订单相关信息
        /// </summary>
        [DataMember]
        public InvoiceCommodityOrderInfo commodityOrderInfo { get; set; }
    }
    /// <summary>
    /// 发票类相关订单信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceCommodityOrderInfo
    {
        /// <summary>
        /// 售中订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 售后订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateAfterSales { get; set; }
        /// <summary>
        /// 售中申请状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateRefund { get; set; }
        /// <summary>
        /// 售后申请状态
        /// </summary>
        [DataMemberAttribute()]
        public int StateRefundAfterSales { get; set; }
      
        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 付款时间
        /// </summary>
        [DataMember]
        public DateTime PaymentTime { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMember]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>
        [DataMember]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        [DataMember]
        public string ReceiptAddress { get; set; }
        /// <summary>
        /// 付款价格，包含运费
        /// </summary>
        [DataMember]
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        [DataMember]
        public int Payment { get; set; }
        /// <summary>
        /// 金币支付
        /// </summary>
        [DataMember]
        public decimal GoldPrice { get; set; }
        /// <summary>
        /// 代金券金额
        /// </summary>
        [DataMember]
        public decimal GoldCoupon { get; set; }
        /// <summary>
        /// 优惠券金额
        /// </summary>
        [DataMember]
        public decimal CouponValue { get; set; }
        /// <summary>
        /// 积分抵现金额
        /// </summary>
        [DataMember]
        public decimal SpendScoreMoney { get; set; }
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
        /// 详细地址
        /// </summary>
        [DataMemberAttribute()]
        public string Address { get; set; }
        /// <summary>
        /// 自提标志 0：非自提，1：自提
        /// </summary>
        [DataMemberAttribute()]
        public int SelfTakeFlag { get; set; }

    }
}
