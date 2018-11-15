using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 查询基类
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReviewSearchDTO : SearchBase
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 最后评价时间
        /// </summary>
        [DataMember]
        public DateTime? LastReviewTime { get; set; }

        /// <summary>
        /// 最后一条评价
        /// </summary>
        [DataMember]
        public Guid? LastReviewId { get; set; }
    }
}
