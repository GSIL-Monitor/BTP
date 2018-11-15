using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 售后申请商品编码
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNApplyRejectedSkusDTO
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
}
