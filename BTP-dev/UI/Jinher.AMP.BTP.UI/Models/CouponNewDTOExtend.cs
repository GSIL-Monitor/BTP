using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.UI.Controllers;
using System.Runtime.Serialization;
using Jinher.AMP.Coupon.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Models
{
    /// <summary>
    /// 券信息扩展
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CouponNewDTOExtend : CouponNewDTO
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid GoodId { get; set; }
    }
}