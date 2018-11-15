using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 众销佣金统计情况
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShareOrderMoneySumDTO
    {
        /// <summary>
        /// 佣金累计
        /// </summary>
        [DataMember]
        public decimal CommissionAmount { get; set; }

        /// <summary>
        /// 待收益佣金
        /// </summary>
        [DataMember]
        public decimal CommmissionUnPay { get; set; }
    }
}
