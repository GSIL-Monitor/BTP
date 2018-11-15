
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商物流跟踪信息
    /// </summary>
    public class ThirdExpressTrace
    {
        /// <summary>
        /// 物流节点时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 物流节点描述
        /// </summary>
        public string Desc { get; set; }
    }
}