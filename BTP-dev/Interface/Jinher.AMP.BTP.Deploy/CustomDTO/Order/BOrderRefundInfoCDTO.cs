using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 部分退单商品退款信息，后台管理对象
    /// </summary>
    [DataContract]
    [Serializable]
    public class BOrderRefundInfoCDTO : DBBase
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid orderId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid itemId{get;set;}
        /// <summary>
        /// 退款
        /// </summary>
        [DataMember]
        public decimal refund { get; set; }
        /// <summary>
        /// 是否运费
        /// </summary>
        [DataMember]
        public bool isDelivery { get; set; }
        /// <summary>
        /// 订单退款Id
        /// </summary>
        [DataMember]
        public Guid orderRefundId { get; set; }
    }
}
