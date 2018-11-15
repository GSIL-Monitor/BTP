using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 订单状态
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderStatusDTO
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单状态。1:审核中; 2:待发货; 3:待收货; 4:已完成; 5:已取消; 6:已退货; 7:待处理; 8：审核不通过，订单已取消; 9：待支付
        /// </summary>
        [DataMember]
        public string OrderStatus { get; set; }
        /// <summary>
        /// 订单状态集合
        /// </summary>
        [DataMember]
        public List<SNOrderItemInfo> OrderItemInfoList { get; set; }
    }
    /// <summary>
    /// 订单状态子集
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderItemInfo
    {
        /// <summary>
        /// 订单行号
        /// </summary>
        [DataMember]
        public string OrderItemId { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
        /// <summary>
        /// 订单行状态码： 1:审核中; 2:待发货; 3:待收货; 4:已完成; 5:已取消; 6:已退货; 7:待处理; 8：审核不通过，订单已取消; 9：待支付
        /// </summary>
        [DataMember]
        public string StatusName { get; set; }
    }
}
