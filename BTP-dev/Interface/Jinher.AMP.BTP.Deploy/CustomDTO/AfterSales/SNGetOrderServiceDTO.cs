using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 苏宁服务类型获取
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNGetOrderServiceDTO
    {
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public string Price { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
