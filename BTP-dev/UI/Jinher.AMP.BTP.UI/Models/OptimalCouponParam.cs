using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.UI.Models
{

    /// <summary>
    /// “获取最优优惠券”参数实体.
    /// </summary>
    [DataContract]
    public class OptimalCouponParam
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
    }
}