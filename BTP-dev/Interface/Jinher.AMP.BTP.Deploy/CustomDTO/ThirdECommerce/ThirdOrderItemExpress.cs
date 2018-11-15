using System;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商订单项物流信息
    /// </summary>
    public class ThirdOrderItemExpress
    {
        /// <summary>
        /// 订单项Id
        /// </summary>
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 子物流单号
        /// </summary>
        public string SubExpressNos { get; set; }
    }
}