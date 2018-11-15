using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 部分退货
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNReturnPartOrderDTO
    {
        /// <summary>
        /// 退款订单Id
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 退货的Skus列表
        /// </summary>
        [DataMember]
        public List<SNReturnPartOrderAddSkusDTO> SkusList { get; set; }
    }
}
