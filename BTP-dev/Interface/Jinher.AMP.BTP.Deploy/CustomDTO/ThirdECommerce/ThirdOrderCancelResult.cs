
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 取消订单申请结果
    /// </summary>
    public class ThirdOrderCancelResult
    {   
        /// <summary>
        /// 取消状态(0:不允许取消，1:允许取消，2:待审核)
        /// </summary>
        public int CancelStatus { get; set; }

        /// <summary>
        /// 拒绝取消原因
        /// </summary>
        public string RejectReason { get; set; }
    }
}