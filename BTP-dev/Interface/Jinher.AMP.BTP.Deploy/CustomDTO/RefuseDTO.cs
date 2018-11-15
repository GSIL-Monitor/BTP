using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class RefuseDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [DataMemberAttribute()]
        public string RefuseReason { get; set; }
    }
}
