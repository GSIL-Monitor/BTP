using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 抵用券退款搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundCouponSearchDTO : SearchBase
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderNo{ get; set; }
        /// <summary>
        /// 收款账号（登录账户）
        /// </summary>
        [DataMember]
        public string ReceiveAccount{ get; set; }
        /// <summary>
        /// 收款人姓名/昵称
        /// </summary>
        [DataMember]
        public string ReceiveName{ get; set; }
        /// <summary>
        /// 退款起始时间
        /// </summary>
        [DataMember]
        public DateTime RefundStartTime{ get; set; }
        /// <summary>
        /// 退款终止时间
        /// </summary>
        [DataMember]
        public DateTime RefundEndTime { get; set; }
    }
}
