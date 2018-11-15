using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 校验购物车商品接口参数
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CheckShopCommodityParam
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// 购物车商品列表
        /// </summary>
        [DataMember]
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        /// <summary>
        /// 拼团活动id
        /// </summary>
        [DataMember]
        public Guid DiygId { get; set; }

        /// <summary>
        /// 活动类型  => 0，普通活动;1，秒杀;2，预售; 3，拼团 ; -1：表示0、1、2;
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }

        /// <summary>
        /// 规格Id
        /// </summary>
        public Guid SpecificationsId { get; set; }

        /// <summary>
        /// 规格参数
        /// </summary>
        public int Specifications { get; set; }

    }
}
