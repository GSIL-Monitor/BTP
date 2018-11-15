using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算单订单信息DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountOrderDTO
    {
        /// <summary>
        ///  主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderCode { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime OrderSubTime { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 订单真实价格
        /// </summary>
        [DataMember]
        public decimal OrderRealAmount { get; set; }

        /// <summary>
        /// 是否为商城优惠卷 
        /// </summary>
        [DataMember]
        public bool IsMallCoupon { get; set; }

        /// <summary>
        /// 商城优惠券总金额 
        /// </summary>
        [DataMember]
        public decimal CouponAmount { get; set; }

        /// <summary>
        /// 退款总金额 
        /// </summary>
        [DataMember]
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 推广佣金总额 
        /// </summary>
        [DataMember]
        public decimal PromotionCommissionAmount { get; set; }

        /// <summary>
        /// 商城佣金总额 
        /// </summary>
        [DataMember]
        public decimal PromotionAmount { get; set; }

        /// <summary>
        /// 订单结算金额 
        /// </summary>
        [DataMember]
        public decimal SellerAmount { get; set; }

        /// <summary>
        /// 是否结算成功 
        /// </summary>
        [DataMember]
        public bool Successed
        {
            get
            {
                // 如果商城优惠券金额大于商城佣金 如果结算金额小于0，则结算异常
                return ((IsMallCoupon && CouponAmount > PromotionAmount) || SellerAmount < 0) ? false : true;
            }
        }

        /// <summary>
        ///   商城易捷币抵用金额
        /// </summary>
        [DataMember]
        public decimal OrderYJBAmount { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        public decimal OrderFreight { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        [DataMember]
        public decimal SettleAmount { get; set; }
    }
}
