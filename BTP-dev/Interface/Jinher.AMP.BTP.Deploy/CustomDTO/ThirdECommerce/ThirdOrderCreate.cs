using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商订单提交参数
    /// </summary>
    public class ThirdOrderCreate
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单生成时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string SubTime { get; set; }

        /// <summary>
        /// 订单支付时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string PayTime { get; set; }

        /// <summary>
        /// 订单sku
        /// </summary>
        public List<ThirdOrderCreateSku> OrderSkus { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public ThirdAddress Address { get; set; }
    }
}