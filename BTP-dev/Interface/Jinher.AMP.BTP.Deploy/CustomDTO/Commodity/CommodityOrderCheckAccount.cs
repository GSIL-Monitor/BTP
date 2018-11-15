using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单对账类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityOrderCheckAccount
    {
        /// <summary>
        /// 对账订单ID
        /// </summary>
        [DataMember()]
        public Guid AccountId { get; set; }

        /// <summary>
        /// 对账订单ID，用于显示
        /// </summary>
        [DataMember()]
        public string AccountIdString { get; set; }

        /// <summary>
        /// 电商订单ID
        /// </summary>
        [DataMember()]
        public Guid O2OId { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMember()]
        public Guid AppId { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        [DataMember()]
        public string AppName { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [DataMember()]
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        [DataMember()]
        public decimal RealPrice { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMember()]
        public decimal RefundPrice { get; set; }
    }
}
