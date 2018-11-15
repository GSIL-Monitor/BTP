using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品分享分成信息。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityShareInfoDTO
    {
        /// <summary>
        /// 店铺Id(应用id)
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 是否全部商品分成
        /// </summary>
        [DataMember]
        public bool? IsDividendAll { get; set; }

        /// <summary>
        /// 众销分成比例
        /// </summary>
        [DataMember]
        public decimal SharePercent { get; set; }

        /// <summary>
        /// 商品分享分成比例列表。
        /// </summary>
        [DataMember]
        public List<CommoditySharePercentDTO> CShareList { get; set; }


    }
    /// <summary>
    ///商品分成比例，ShareMoney为空，或为0 表示不分成。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommoditySharePercentDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品分成比例。
        /// </summary>
        [DataMember]
        public decimal SharePercent { get; set; }
    }
}
