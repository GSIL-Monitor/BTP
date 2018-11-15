
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商售后服务申请审核回调-允许售后，用户邮寄货物
    /// </summary>
    public class ThirdServiceAgreeResult
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
        /// 允许时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string AgreeTime { get; set; }

        /// <summary>
        /// 退货地址
        /// </summary>
        public ThirdAddress Address { get; set; }
    }
}
