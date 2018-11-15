
namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 生成sign参数
    /// </summary>
    public class YXSignParam
    {
        /// <summary>
        /// 接口方法名
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 渠道的AppKey
        /// </summary>
        public string appKey { get; set; }

        /// <summary>
        /// 请求时间戳
        /// </summary>
        public string timestamp { get; set; }
    }

    /// <summary>
    /// sign
    /// </summary>
    public class YXSign : YXSignParam
    {
        /// <summary>
        /// sign
        /// </summary>
        public string sign { get; set; }
    }
}
