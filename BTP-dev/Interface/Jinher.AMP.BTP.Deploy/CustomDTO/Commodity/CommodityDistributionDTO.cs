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
    public class CommodityDistributionDTO
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 提交人
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// ModifiedOn
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
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
