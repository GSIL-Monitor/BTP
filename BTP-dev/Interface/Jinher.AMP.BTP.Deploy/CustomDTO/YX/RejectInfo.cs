using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选拒绝退货信息
    /// </summary>
    public class RejectInfo
    {
        /// <summary>
        /// 申请单Id
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string rejectReason { get; set; }
    }
}
