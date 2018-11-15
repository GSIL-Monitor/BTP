
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后退货物流签收回调信息
    /// </summary>
    public class ThirdMailReceiveResult
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
        /// 签收时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string ReceiveTime { get; set; }
    }
}
