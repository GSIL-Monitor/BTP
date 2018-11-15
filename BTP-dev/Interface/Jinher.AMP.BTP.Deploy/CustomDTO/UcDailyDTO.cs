using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 众筹每日计算分账明细DTO
    /// </summary>    
    [Serializable]
    [DataContract]
    public class UcDailyDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsModifiedPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

    }

}
