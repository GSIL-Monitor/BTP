using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 苏宁物流订单输出
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderLogistOutPutDTO
    {
        /// <summary>
        /// 	订单号
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单行号
        /// </summary>
        [DataMember]
        public string OrderItemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<SNOrderLogistStatusResDTO> OrderLogisticStatus { get; set; }
        /// <summary>
        /// 	收货时间
        /// </summary>
        [DataMember]
        public string ReceiveTime { get; set; }
        /// <summary>
        /// 	发货时间
        /// </summary>
        [DataMember]
        public string ShippingTime { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
