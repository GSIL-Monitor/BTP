using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品积分信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityScoreDTO
    {
        /// <summary>
        /// 店铺Id(应用id)
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 是否全部商品相同积分抵用比例
        /// </summary>
        [DataMember]
        public bool? IsAll { get; set; }

        /// <summary>
        /// 积分抵用比例
        /// </summary>
        [DataMember]
        public decimal ScorePercent { get; set; }

        /// <summary>
        /// 商品积分抵用比例列表。
        /// </summary>
        [DataMember]
        public List<CommodityScorePercentDTO> CScoreList { get; set; }


    }
    /// <summary>
    /// 商品积分抵用比例
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityScorePercentDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品积分抵用比例。
        /// </summary>
        [DataMember]
        public decimal ScorePercent { get; set; }
    }
}
