using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 三级分销类
    /// </summary>
    [Serializable]
    [DataContract]
    public class MicroshopCommodityDistributionDTO : CommodityDTO
    {
        /// <summary>
        /// 直接上级分成比例
        /// </summary>
        [DataMember]
        public Decimal L1Percent { get; set; }
        /// <summary>
        /// 2级上级分成比例
        /// </summary>
        [DataMember]
        public Decimal L2Percent { get; set; }
        /// <summary>
        /// 3级上级分成比例
        /// </summary>
        [DataMember]
        public Decimal L3Percent { get; set; }

    }
}
