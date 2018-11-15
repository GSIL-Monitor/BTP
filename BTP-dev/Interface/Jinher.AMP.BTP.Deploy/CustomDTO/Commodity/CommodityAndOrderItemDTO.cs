using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.Commodity
{
    /// <summary>
    /// 123
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityAndOrderItemDTO
    {
        /// <summary>
        /// 子订单ID
        /// </summary>
        [DataMember]
        public Guid OrderItemId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 商品的单价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 子订单编号
        /// </summary>
        [DataMember]
        public string code { get; set; }
    }
}
