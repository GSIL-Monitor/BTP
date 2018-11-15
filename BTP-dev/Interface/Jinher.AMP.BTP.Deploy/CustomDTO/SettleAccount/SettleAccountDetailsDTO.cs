using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算单详细信息DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountDetailsDTO : SettleAccountListDTO
    {
        /// <summary>
        /// 商城佣金总额 
        /// </summary>
        [DataMember]
        public decimal PromotionAmount { get; set; }

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
        /// 结算单号
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        ///   商城易捷币抵用金额
        /// </summary>
        [DataMember]
        public decimal OrderYJBAmount { get; set; }

        /// <summary>
        ///   结算开始时间
        /// </summary>
        [DataMember]
        public DateTime? AmountStartDate { get; set; }
    }
}
