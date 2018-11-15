using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 部分退货Skus列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNReturnPartOrderAddSkusDTO
    {
        /// <summary>
        /// 部分退货数量
        /// </summary>
        [DataMember]
        public string Num { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
