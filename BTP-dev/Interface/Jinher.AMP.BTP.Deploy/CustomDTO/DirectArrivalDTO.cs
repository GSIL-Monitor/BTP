using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class DirectArrivalDTO
    {
        /// <summary>
        /// 0 货到付款 1 在线支付(担保交易) 2 直接到账
        /// </summary>
        [DataMember]
        public int Pattern { get; set; }
        /// <summary>
        /// 金币余额
        /// </summary>
        [DataMember]
        public ulong GoldBal { get; set; }
        /// <summary>
        /// 代金券个数
        /// </summary>
        [DataMember]
        public int GoldCouponCount { get; set; }

        /// <summary>
        /// 是否为正品会应用
        /// </summary>
        [DataMember]
        public bool IsAllAppInZPH { get; set; }
        /// <summary>
        /// 0金币担保，1直接到账
        /// </summary>
        [DataMember]
        public int TradeType { get; set; }
        /// <summary>
        /// 是否支持货到付款
        /// </summary>
        [DataMember]
        public bool IsHdfk { get; set; }

    }
}
