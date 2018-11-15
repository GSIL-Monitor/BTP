using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销佣金查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorMoneyResultDTO
    {
        /// <summary>
        /// 分销佣金入账情况
        /// </summary>
        [DataMember]
        public List<DistributorMoneyDTO> DistributorMoneyList { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
