
namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    public class YXSubErrParam
    {
        public string TraceId { get; set; }
    }

    public class YXResult
    {
        /// <summary>
        /// 响应状态码，默认 200
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 响应详情说明
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 子错误码
        /// </summary>
        public string SubErrCode { get; set; }

        /// <summary>
        /// 子错误参数
        /// </summary>
        //public YXSubErrParam SubErrParam { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Successed { get { return Code == "200"; } }
    }

    public class YXResult<T> : YXResult where T : class
    {
        /// <summary>
        /// 响应详情数据
        /// </summary>
        public T Result { get; set; }
    }
}
