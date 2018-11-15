using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 苏宁---整单退DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNApplyRejectedDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// Skus列表
        /// </summary>
        [DataMember]
        public List<SNApplyRejectedSkusDTO> SkusList { get; set; }
    }
}
