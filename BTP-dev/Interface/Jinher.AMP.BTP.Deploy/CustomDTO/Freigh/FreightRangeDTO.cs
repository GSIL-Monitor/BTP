using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 针对区间运费的扩展
    /// </summary>
    public partial class FreightRangeDTO
    {
        /// <summary>
        /// 默认运费
        /// </summary>
        [DataMemberAttribute()]
        public string DefaultFreight { get; set; }

        /// <summary>
        /// 运送城市
        /// </summary>
        [DataMemberAttribute()]
        public string ExpressProvinces { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public string FreightCosts { get; set; }
    }
}
