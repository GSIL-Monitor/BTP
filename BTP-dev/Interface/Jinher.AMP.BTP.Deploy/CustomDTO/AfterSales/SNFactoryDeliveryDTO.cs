using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 厂送
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNFactoryDeliveryDTO
    {
        /// <summary>
        /// 送货地址(市) 编码
        /// </summary>
        [DataMember]
        public string CityId { get; set; }
        /// <summary>
        /// 9位或者11位商品编码
        /// </summary>
        [DataMember]
        public List<SNApplyRejectedSkusDTO> SkuIds { get; set; }
    }
}
