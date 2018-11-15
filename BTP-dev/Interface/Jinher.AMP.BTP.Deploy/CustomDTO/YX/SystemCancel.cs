using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选系统取消退货信息
    /// </summary>
    public class SystemCancel
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
        /// 系统错误原因
        /// </summary>
        public string errorMsg { get; set; }
    }
}
