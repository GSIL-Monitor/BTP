using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
   /// <summary>
   /// 购物车更新
   /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartCommodityUpdateDTO
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShopCartItemId { get; set; }
        /// <summary>
        /// 商品数量        
        /// </summary>
        [DataMemberAttribute()]
        public int Number { get; set; }
        /// <summary>
        /// 用户名        
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 电商馆        
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
    }
}
