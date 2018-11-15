
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后服务申请请求信息
    /// </summary>
    public class ThirdServiceCreate
    {
        /// <summary>
        /// 售后服务单Id	
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 售后联系人
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 售后联系人电话
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 申请售后的skuId
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// 申请售后的sku数量	
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 申请售后原因
        /// </summary>
        public string RefundReason { get; set; }

        /// <summary>
        /// 申请售后详细说明
        /// </summary>
        public string RefundDesc { get; set; }

        /// <summary>
        /// 申请售后的图片信息
        /// </summary>
        public string RefundImgs { get; set; }
    }
}
