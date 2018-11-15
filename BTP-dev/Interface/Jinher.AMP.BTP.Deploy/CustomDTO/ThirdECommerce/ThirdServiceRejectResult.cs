
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后服务申请审核回调-拒绝售后
    /// </summary>
    public class ThirdServiceRejectResult
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
        /// 拒绝时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string RejectTime { get; set; }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RejectReason { get; set; }
    }
}
