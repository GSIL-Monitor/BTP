using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 导出订单DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ImportOrderDTO
    {
        [DataMember]
        public Guid CommodityOrderId { get; set; }
        [DataMember]
        public decimal? RealPrice { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public DateTime SubTime { get; set; }
        [DataMember]
        public string ReceiptUserName { get; set; }
        [DataMember]
        public int Payment { get; set; }
        [DataMember]
        public DateTime? PaymentTime { get; set; }
        [DataMember]
        public string ReceiptPhone { get; set; }
        [DataMember]
        public string RecipientsZipCode { get; set; }
        [DataMember]
        public string ReceiptAddress { get; set; }
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public string Province { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string District { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public int InvoiceType { get; set; }
        [DataMember]
        public string InvoiceTitle { get; set; }
        [DataMember]
        public string SellersRemark { get; set; }
        [DataMember]
        public decimal CrowdfundingPrice { get; set; }
        [DataMember]
        public decimal Commission { get; set; }
        [DataMember]
        public decimal Freight { get; set; }
        [DataMember]
        public decimal GoldCoupon { get; set; }
        [DataMember]
        public decimal CouponValue { get; set; }
        [DataMember]
        public decimal GoldPrice { get; set; }
        [DataMember]
        public decimal SpreadGold { get; set; }
        [DataMember]
        public decimal OwnerShare { get; set; }
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public int SelfTakeFlag { get; set; }
        [DataMember]
        public string SelfTakeAddress { get; set; }
        [DataMember]
        public decimal? RefundMoney { get; set; }

        [DataMember]
        public int StateAfterSales { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        /// <summary>
        /// 分销佣金
        /// </summary>
        [DataMember]
        public decimal DistributeMoney { get; set; }
        [DataMember]
        public decimal SpendScoreMoney { get; set; }
        [DataMember]
        public decimal RefundScoreMoney { get; set; }
        [DataMember]
        public Guid? EsAppId { get; set; }
        /// <summary>
        /// 渠道分享佣金
        /// </summary>
        [DataMember]
        public decimal ChannelShareMoney { get; set; }

        [DataMember]
        public string ShipExpCo { get; set; }

        [DataMember]
        public string ExpOrderNo { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 厂商Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 厂商类型
        /// </summary>
        [DataMember]
        public string AppType { get; set; }

        /// <summary>
        /// 易捷币抵现金额
        /// </summary>
        [DataMember]
        public decimal SpendYJBMoney { get; set; }

        /// <summary>
        /// 阳关餐饮字段1
        /// </summary>
        [DataMember]
        public string FirstContent { get; set; }


        /// <summary>
        /// 阳关餐饮字段2
        /// </summary>
        [DataMember]
        public string SecondContent { get; set; }

        /// <summary>
        /// 阳关餐饮字段3
        /// </summary>
        [DataMember]
        public string ThirdContent { get; set; }




    }
    /// <summary>
    /// 导出订单明细DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ImportOrderItemDTO
    {
        [DataMember]
        public Guid CommodityOrderId { get; set; }
        [DataMember]
        public decimal? Price { get; set; }
        [DataMember]
        public string CommodityCode { get; set; }
        [DataMember]
        public string CommodityName { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public string CommodityAttributes { get; set; }
        [DataMember]
        public Guid CommodityId { get; set; }
        [DataMember]
        public decimal ManufacturerClearingPrice { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        [DataMember]
        public string EsAppName { get; set; }
        [DataMember]
        public decimal CostPrice { get; set; }
    }
}
