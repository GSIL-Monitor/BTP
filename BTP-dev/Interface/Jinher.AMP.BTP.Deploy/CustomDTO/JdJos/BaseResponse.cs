using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdJos
{
    /// <summary>
    /// 进销存-京东jos接口返回值基类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class BaseResponse
    {
        /// <summary>
        /// 接口调用详细信息
        /// </summary>
        [DataMemberAttribute()]
        public string Message { get; set; }

        /// <summary>
        /// 接口是否成功
        /// </summary>
        [DataMemberAttribute()]
        public bool IsInterfaceSuccess { get; set; }

        /// <summary>
        /// 接口是否返回正确结果
        /// </summary>
        [DataMemberAttribute()]
        public bool IsResultSuccess { get; set; }
    }
}
