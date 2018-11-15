using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 导出订单(excel)
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExportResultDTO
    {
        /// <summary>
        /// ID(序列号)
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 订单Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 购买数量/件
        /// </summary>
        [DataMember]
        public int BuyNumber { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime OrdersTime { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [DataMember]
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [DataMember]
        public string PaymentType { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }

        /// <summary>
        /// 实付款
        /// </summary>
        [DataMember]
        public decimal? PracticalPayment { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Payer { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [DataMember]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMember]
        public string Postcode { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        [DataMember]
        public string remark { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        [DataMember]
        public string SellersRemark { get; set; }

        /// <summary>
        /// 商品集合
        /// </summary>
        [DataMember]
        public List<ProductList> Products { get; set; }

        /// <summary>
        /// 含众筹佣金
        /// </summary>
        [DataMember]
        public decimal CrowdfundingPrice { get; set; }

        /// <summary>
        /// 众销佣金
        /// </summary>
        [DataMember]
        public decimal Commission { get; set; }

        /// <summary>
        /// 含运费
        /// </summary>
        [DataMember]
        public decimal Freight { get; set; }

        /// <summary>
        /// 使用代金券金额
        /// </summary>
        [DataMember]
        public decimal GoldCoupon { get; set; }

        /// <summary>
        /// 使用优惠券金额
        /// </summary>
        [DataMember]
        public decimal CouponValue { get; set; }

        /// <summary>
        /// 使用金币金额
        /// </summary>
        [DataMember]
        public decimal GoldPrice { get; set; }

        /// <summary>
        /// 推广主佣金
        /// </summary>
        [DataMember]
        public decimal SpreadGold { get; set; }

        /// <summary>
        /// 应用主佣金
        /// </summary>
        [DataMember]
        public decimal OwnerShare { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 是否自提
        /// </summary>
        [DataMember]
        public int SelfTakeFlag { get; set; }

        /// <summary>
        /// 售后订单状态
        /// </summary>
        [DataMember]
        public int StateAfterSales { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMember]
        public decimal? RefundMoney { get; set; }
        /// <summary>
        /// 自提地址
        /// </summary>
        [DataMember]
        public string SelfTakeAddress { get; set; }
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
        /// 分销佣金
        /// </summary>
        [DataMember]
        public decimal DistributeMoney { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
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

        /// <summary>
        /// 订单来源
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }

        /// <summary>
        /// 厂商名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

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
        /// 下单昵称
        /// </summary>
        [DataMember]
        public string Uname { get; set; }

        /// <summary>
        /// 下单账号
        /// </summary>
        [DataMember]
        public string Ucode { get; set; }


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

        /// <summary>
        /// 金采支付分期付款总金额
        /// </summary>
        [DataMember]
        public decimal JczfAmonut { get; set; }
    }

    /// <summary>
    /// 订单包含的商品
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductList
    {
        /// <summary>
        /// 实际付款
        /// </summary>
        [DataMember]
        public decimal? PracticalPayment { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品的购买数量
        /// </summary>
        [DataMember]
        public int BuyNumber { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>
        [DataMember]
        public decimal? ProductPric { get; set; }
        /// <summary>
        /// 厂家结算价
        /// </summary>
        [DataMember]
        public decimal ManufacturerClearingPrice { get; set; }

        /// <summary>
        /// 进货价
        /// </summary>
        [DataMember]
        public decimal CostPrice { get; set; }
    }


}
