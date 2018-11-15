using System;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 订单异常信息
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid orderId { get; set; }

        /// <summary>
        /// 异常说明
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 额外补充数据
        /// </summary>
        public string extData { get; set; }
    }
}
