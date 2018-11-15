using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 获取苏宁物流信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderLogistInputDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单行号
        /// </summary>
        [DataMember]
        public string OrderItemId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
