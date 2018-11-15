using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品分享分成
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityDividendListDTO
    {
        /// <summary>
        /// 商品分享分成列表
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> CommodityList { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
