using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 退货包裹确认收货信息
    /// </summary>
    public class ExpressConfirm
    {
        /// <summary>
        /// 申请单Id
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string trackingCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string trackingNum { get; set; }

        /// <summary>
        /// 物流签收时间(单位为毫秒)
        /// </summary>
        public long trackingTime { get; set; }
    }
}
