using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 促销展示+热门商品
    /// </summary>
    [Serializable]
    [DataContract]
    public class PromotionHotSDTO
    {
        /// <summary>
        /// 促销展示
        /// </summary>
        [DataMember]
        public List<PromotionSDTO> promotionSDTO { get; set; }
        /// <summary>
        /// 热门商品展示
        /// </summary>
        [DataMember]
        public List<CommodityListCDTO> commoditySDTO { get; set; }
    }
}
