using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 物流轨迹信息 by zbs 20180706
    /// </summary>
    public class DeliveryInfo
    {
        /// <summary>
        /// 物流公司
        /// </summary>
        public string company { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 轨迹信息
        /// </summary>
        public List<DeliveryDetailInfo> content { get; set; }
    }

    /// <summary>
    /// 轨迹信息
    /// </summary>
    public class DeliveryDetailInfo
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }

    }
}
