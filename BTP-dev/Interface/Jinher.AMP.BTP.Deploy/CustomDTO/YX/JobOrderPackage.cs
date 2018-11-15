using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选系统中包裹信息
    /// </summary>
    public class JobOrderPackage
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 包裹号
        /// </summary>
        public long packageId { get; set; }
        /// <summary>
        /// 物流详细信息（带上sku）
        /// </summary>
        public List<ExpressDetailInfo> expressDetailInfos { get; set; }
        /// <summary>
        /// 发货时间	
        /// </summary>
        public long expCreateTime { get; set; }
    }

}
