using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdJos
{
    /// <summary>
    /// 进销存京东jos售后服务单接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CreateServiceOrderResult
    {
        /// <summary>
        /// 京东服务单编号
        /// </summary>
        [DataMemberAttribute()]
        public string serivcesNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string errorCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string errorMsg { get; set; }
    }

    /// <summary>
    /// 进销存京东jos售后服务单接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CreateServiceOrderResponse
    {
        [DataMemberAttribute()]
        public CreateServiceOrderResult servicesResult { get; set; }
    }

    /// <summary>
    /// 进销存京东jos售后服务单接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CreateServiceOrderResponseDTO : BaseResponse
    {
        [DataMemberAttribute()]
        public CreateServiceOrderResponse jingdong_eclp_afs_createServiceOrder_response { get; set; }
    }
}
