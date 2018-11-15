using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class AddOrderRefundExpDTO
    {             
        /// <summary>
        /// 商品订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }
        
        /// <summary>
        /// 物流公司 
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }

        /// <summary>
        /// 商品订单项ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderItemId { get; set; }

    }
}
