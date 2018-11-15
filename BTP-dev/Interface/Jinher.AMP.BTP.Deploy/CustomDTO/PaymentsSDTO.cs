using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 支付方式
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PaymentsSDTO
    {
        /// <summary>
        /// 支付方式名称
        /// </summary>
        [DataMemberAttribute()]
        public string PaymentsName { get; set; }

        /// <summary>
        /// 是否可用 可用=true 不可用=false
        /// </summary>
        [DataMemberAttribute()]
        public bool IsOnUse { get; set; }
    }
}
