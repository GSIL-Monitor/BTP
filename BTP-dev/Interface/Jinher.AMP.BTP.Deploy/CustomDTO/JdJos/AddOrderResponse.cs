using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdJos
{
    /// <summary>
    /// 进销存京东jos销售出库单接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AddOrderResponse
    {
        /// <summary>
        /// 京东订单编号
        /// </summary>
        [DataMemberAttribute()]
        public string eclpSoNo { get; set; }
    }

    /// <summary>
    /// 进销存京东jos销售出库单接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AddOrderResponseDTO : BaseResponse
    {
        [DataMemberAttribute()]
        public AddOrderResponse jingdong_eclp_order_addOrder_responce { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class AuthResponse : BaseResponse
    {
        /// <summary>
        /// 授权接口返回Code
        /// </summary>
        [DataMemberAttribute()]
        public string code { get; set; }

        /// <summary>
        ///  授权令牌
        /// </summary>
        [DataMemberAttribute()]
        public string access_token { get; set; }
    }
}
