using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商订单物流(包裹)信息
    /// </summary>
    public class ThirdOrderPackage
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
        /// 此物流包裹包含的skuId列表
        /// </summary>
        public List<string> SkuIdList { get; set; }

        /// <summary>
        /// 发货时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string ExpCreateTime { get; set; }

        /// <summary>
        /// 收货时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string ConfirmTime { get; set; }
    }
}