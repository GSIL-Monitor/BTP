
namespace Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce
{
    /// <summary>
    /// 第三方电商标准接口返回参数
    /// </summary>
    public class ThirdResponse
    {
        /// <summary>
        /// 响应状态码，默认 200
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应详情说明
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Successed { get { return Code == 200; } }
    }

    /// <summary>
    /// 第三方电商标准接口返回参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThirdResponse<T> : ThirdResponse where T : class
    {
        /// <summary>
        /// 响应详情数据
        /// </summary>
        public T Result { get; set; }
    }
}