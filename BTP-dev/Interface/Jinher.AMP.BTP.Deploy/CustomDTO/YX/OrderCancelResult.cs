using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 取消订单结果
    /// </summary>
    public class OrderCancelResult
    {
        /// <summary>
        /// 取消状态(0:不允许取消，1:允许取消，2:待审核)
        /// </summary>
        public int cancelStatus { get; set; }

        /// <summary>
        /// 拒绝取消原因
        /// </summary>
        public string rejectReason { get; set; }
    }

    /// <summary>
    /// 取消订单结果
    /// </summary>
    public class OrderCancelResultCallBack : OrderCancelResult
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string orderId { get; set; }
    }
}
