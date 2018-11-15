using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdJos
{
    /// <summary>
    /// 进销存京东jos销售出库单包裹数据查询接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderPackage
    {
        /// <summary>
        /// 包裹重量 
        /// </summary>
        [DataMemberAttribute()]
        public decimal packWeight { get; set; }

        /// <summary>
        /// 包裹号
        /// </summary>
        [DataMemberAttribute()]
        public string packageNo { get; set; }
    }

    /// <summary>
    /// 进销存京东jos销售出库单包裹数据查询接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QueryOrderPacksResult
    {
        /// <summary>
        /// 金和订单号
        /// </summary>
        [DataMemberAttribute()]
        public string isvUUID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string shipperName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string shipperNo { get; set; }

        /// <summary>
        /// 京东订单号
        /// </summary>
        [DataMemberAttribute()]
        public string eclpSoNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderPackage> orderPackageList { get; set; }

        /// <summary>
        /// 金和运单号
        /// </summary>
        [DataMemberAttribute()]
        public string wayBill { get; set; }

        /// <summary>
        /// 包裹数量 
        /// </summary>
        [DataMemberAttribute()]
        public int packCount { get; set; }
    }

    /// <summary>
    /// 进销存京东jos销售出库单包裹数据查询接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QueryOrderPacksResponse
    {
        [DataMemberAttribute()]
        public string code { get; set; }

        [DataMemberAttribute()]
        public List<QueryOrderPacksResult> queryorderpacks_result { get; set; }
    }

    /// <summary>
    /// 进销存京东jos销售出库单包裹数据查询接口返回值
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QueryOrderPacksResponseDTO : BaseResponse
    {
        [DataMemberAttribute()]
        public QueryOrderPacksResponse jingdong_eclp_order_queryOrderPacks_responce { get; set; }
    }
}
