using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderShippingExtDTO
    {
        /// <summary>
        ///  
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [DataMember]
        public string ShipExpCo { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        [DataMember]
        public string ExpOrderNo { get; set; }

        /// <summary>
        /// 原始物流公司
        /// </summary>
        [DataMember]
        public string OrgShipExpCo { get; set; }

        /// <summary>
        /// 原始快递单号
        /// </summary>
        [DataMember]
        public string OrgExpOrderNo { get; set; }
    }
}
