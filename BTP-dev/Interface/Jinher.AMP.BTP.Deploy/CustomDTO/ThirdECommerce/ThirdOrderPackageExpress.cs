using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商包裹物流信息
    /// </summary>
    public class ThirdOrderPackageExpress
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 物流跟踪信息集合
        /// </summary>
        public List<ThirdExpressTrace> ExpressTraceList { get; set; }
    }
}