using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 厂送状态返回
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNFactoryDeliveryReturnDTO
    {
        /// <summary>
        /// 成功状态 	true-成功；false-失败
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回集合
        /// </summary>
        [DataMember]
        public List<SNFactoryDeliveryReturnListDTO> ResultsList { get; set; }
    }

    /// <summary>
    /// 厂送状态返回集合
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNFactoryDeliveryReturnListDTO
    {
        /// <summary>
        /// true  厂送(快递寄回)  false  非厂送自营(上门取件)
        /// </summary>
        [DataMember]
        public bool IsFactorySend { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
