
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后服务申请响应信息
    /// </summary>
    public class ThirdServiceCreateResult
    {
        /// <summary>
        /// 售后服务单状态(0:不支持售后，1:待审核)
        /// </summary>
        public int ServiceStatus { get; set; }

        /// <summary>
        /// 拒绝售后原因
        /// </summary>
        public string RejectReason { get; set; }
    }
}
