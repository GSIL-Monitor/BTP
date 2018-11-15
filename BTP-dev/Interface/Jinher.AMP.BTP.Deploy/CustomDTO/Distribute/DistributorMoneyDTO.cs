using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销佣金入账情况
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorMoneyDTO
    {
        /// <summary>
        /// 分销者Id
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }
        /// <summary>
        /// 时间，已收益为EndTime，待收益为支付时间
        /// </summary>
        [DataMember]
        public DateTime SortTime { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [DataMember]
        public decimal Money { get; set; }

        /// <summary>
        /// 状态：0 已入积分账户，1 待入账
        /// </summary>
        public int State { get; set; }
    }
}
