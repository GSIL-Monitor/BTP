using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class SetCommodityOrderDTO
    {
        /// <summary>
        /// SetCommodity Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [DataMember]
        public double RankNo { get; set; }
    }
}
