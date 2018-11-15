using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
   /// <summary>
   /// 购物车属性更新
   /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopAttributeCommodityUpdateDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShopCartItemId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        /// 电商馆ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; } 

        /// <summary>
        /// 商品属性值对     
        /// </summary>
        [DataMemberAttribute()]
        public string StrComAttributes { get; set; }

        /// <summary>
        /// 规格设置
        /// </summary>
        [DataMemberAttribute()]
        public int? Specifications { get; set; }
    }
}
