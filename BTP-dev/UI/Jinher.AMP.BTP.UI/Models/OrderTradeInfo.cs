using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.EDMX;

namespace Jinher.AMP.BTP.UI.Models
{
    public class BigAutocomplete
    {
        public string title { get; set; }
        public string result { get; set; }
    }


    [Serializable]
    [DataContract]
    public class OrderTradeInfos
    {
        [DataMember]
        public int Count { get; set; }
        
        [DataMember]
        public List<OrderTradeInfo> Data { get; set; }
    }

    [Serializable]
    [DataContract]
    public class OrderData
    {
        [DataMember]
        public int TotalCommodityNumber { get; set; }

        [DataMember]
        public decimal? TotalCommodityCostPrice { get; set; }

        [DataMember]
        public decimal? TotalCommodityCostpurchase { get; set; }

        [DataMember]
        public decimal TotalPayMoney { get; set; }

        [DataMember]
        public decimal TotalCommodityMoney { get; set; }

        [DataMember]
        public decimal TotalFreightMoney { get; set; }

        [DataMember]
        public decimal? TotalYJCouponUseMoney { get; set; }

        [DataMember]
        public decimal? TotalYJCouponRefundMoney { get; set; }
        
       
        
    }


    [Serializable]
    [DataContract]
    public class OrderTradeInfo : PayTransaction1
    {
        [DataMember]
        public string TradeId { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public decimal? CostPrice { get; set; }

        [DataMember]
        public string CommodityName { get; set; }

        [DataMember]
        public string AppTypeStr { get; set; }

        [DataMember]
        public string PayTypeStr { get; set; }

        [DataMember]
        public string CommodityInfo { get; set; }

        [DataMember]
        public string CommodityNumber { get; set; }

        [DataMember]
        public string CommodityCostPrice { get; set; }

        [DataMember]
        public string CommodityCostpurchase { get; set; }

        [DataMember]
        public decimal CommodityMoney { get; set; }

        [DataMember]
        public string TradeTimeStr { get; set; }

        [DataMember]
        public List<OrderItem1> CommodityList { get; set; }
    }


    [Serializable]
    [DataContract]
    public class OrderTradeInfoData
    {
        [DataMember]
        public string TradeId { get; set; }

        [DataMember]
        public string SupplierCode { get; set; }

        [DataMember]
        public string SupplierName { get; set; }

        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public Int16 AppType { get; set; }

        [DataMember]
        public string CommodityName { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public decimal? CostPrice { get; set; }

        [DataMember]
        public decimal? TotalPrice { get; set; }

        [DataMember]
        public string OrderState { get; set; }

        [DataMember]
        public string OrderCode { get; set; }

        [DataMember]
        public string JdOrderId { get; set; }

        [DataMember]
        public DateTime TradeTime { get; set; }

        [DataMember]
        public string TradeNum { get; set; }

        [DataMember]
        public decimal PayMoney { get; set; }

        [DataMember]
        public int PayType { get; set; }

        [DataMember]
        public decimal? CommodityMoney { get; set; }

        [DataMember]
        public decimal FreightMoney { get; set; }

        [DataMember]
        public decimal RefundFreightMoney { get; set; }

        [DataMember]
        public decimal RefundMoney { get; set; }
        
        [DataMember]
        public decimal? YJCouponMoney { get; set; }
    }
}
