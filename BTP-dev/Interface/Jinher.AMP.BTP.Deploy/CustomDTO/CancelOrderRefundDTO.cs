using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class CancelOrderRefundDTO
    {
        /// <summary>
        /// 商品订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        ///退款状态
        /// </summary>
        [DataMemberAttribute()]
        public string State { get; set; }

        /// <summary>
        /// 商品订单项ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderItemId { get; set; }
        
    }
}
