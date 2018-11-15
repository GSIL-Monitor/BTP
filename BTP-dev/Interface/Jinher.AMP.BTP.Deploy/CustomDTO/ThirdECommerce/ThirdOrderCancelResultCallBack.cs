
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 取消订单审核回调结果
    /// </summary>
    public class ThirdOrderCancelResultCallBack : ThirdOrderCancelResult
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 审核完成时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string AuditTime { get; set; }
    }
}