using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 修改总代分佣比例
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadUpdateDividendPercentDTO
    {
        /// <summary>
        /// 推广主ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 分佣比例
        /// </summary>
        [DataMember]
        public decimal Percent { get; set; }
    }
}
