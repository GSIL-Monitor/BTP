using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.Commodity
{
    [Serializable]
    [DataContract]
    public class ComOrderExpressNew
    {        
        /// <summary>
        ///订单id
        /// </summary>
        [DataMember]
        public Guid CommodityOrderId { get; set; }
        /// <summary>
        ///订单物流状态 
        /// </summary>
        [DataMember]
        public string state { get; set; }
        /// <summary>
        ///商品图片
        /// </summary>
        [DataMember]
        public string Pic { get; set; }
        /// <summary>
        ///发货时间
        /// </summary>
        [DataMember]
        public DateTime? shipmentsTime { get; set; }
        /// <summary>
        ///提示语
        /// </summary>
        [DataMember]
        public string Remarked { get; set; }  
    }
}
