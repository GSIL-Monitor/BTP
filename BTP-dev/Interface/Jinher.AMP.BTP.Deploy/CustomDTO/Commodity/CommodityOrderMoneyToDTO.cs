using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单钱款去向
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderMoneyToDTO
    {
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        [DataMember]
        public List<CommodityOrderMoneyToModelDTO> Data { get; set; }
    }

    /// <summary>
    /// 订单钱款去向
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderMoneyToModelDTO
    {
        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// EsAppName
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }
    }
}
