
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后超过X天未收到退货物流回调信息
    /// </summary>
    public class ThirdMailNotReceiveResult
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
        /// 通知时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string NotifyTime { get; set; }

        /// <summary>
        /// 超过天数
        /// </summary>
        public int Days { get; set; }
    }
}
