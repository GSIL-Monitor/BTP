using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选渠道下单接口参数
    /// </summary>
    public class OrderVO : OrderBaseVO
    {
        /// <summary>
        /// 订单SKU
        /// </summary>
        public List<OrderSkuVO> orderSkus { get; set; }
    }
}
