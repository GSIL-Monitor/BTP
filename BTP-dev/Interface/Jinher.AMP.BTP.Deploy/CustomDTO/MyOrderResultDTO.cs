using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单返回结果 --- 厂家直销
    /// </summary>
    [Serializable()]
    [DataContract]
    public class MyOrderResultDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMemberAttribute()]
        public string OrderCode { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }
        [DataMember]
        public Guid AppId { get; set; }
        [DataMember]
        public decimal RealPrice { get; set; }
    }
}
