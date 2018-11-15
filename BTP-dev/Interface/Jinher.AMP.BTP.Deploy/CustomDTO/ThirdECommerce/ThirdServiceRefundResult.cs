
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后服务退货结果回调信息
    /// </summary>
    public class ThirdServiceRefundResult
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
        /// 申请售后的skuId
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// 审核完成时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string AuditTime { get; set; }

        /// <summary>
        /// 退货状态(0:允许退货，1:拒绝退货)
        /// </summary>
        public int RefundStatus { get; set; }

        /// <summary>
        /// 拒绝退货原因
        /// </summary>
        public string RejectReason { get; set; }
    }
}
