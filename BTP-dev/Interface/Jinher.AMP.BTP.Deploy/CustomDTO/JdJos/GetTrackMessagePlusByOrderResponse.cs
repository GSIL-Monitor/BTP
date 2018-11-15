using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdJos
{
    /// <summary>
    /// 进销存京东jos查询物流信息接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class GetTrackMessagePlusByOrderData
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string opeTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string opeRemark { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMemberAttribute()]
        public string opeTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string opeName { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        [DataMemberAttribute()]
        public string waybillCode { get; set; }
    }

    /// <summary>
    /// 进销存京东jos查询物流信息接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class GetTrackMessagePlusByOrderResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public int resultCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public List<GetTrackMessagePlusByOrderData> resultData { get; set; }
    }

    /// <summary>
    /// 进销存京东jos查询物流信息接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class GetTrackMessagePlusByOrderResponse
    {
        [DataMemberAttribute()]
        public string code { get; set; }

        [DataMemberAttribute()]
        public GetTrackMessagePlusByOrderResult getTrackMessagePlusByOrder_result { get; set; }
    }

    /// <summary>
    /// 进销存京东jos查询物流信息接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class GetTrackMessagePlusByOrderResponseDTO : BaseResponse
    {
        [DataMemberAttribute()]
        public GetTrackMessagePlusByOrderResponse jingdong_eclp_order_getTrackMessagePlusByOrder_responce { get; set; }
    }
}
