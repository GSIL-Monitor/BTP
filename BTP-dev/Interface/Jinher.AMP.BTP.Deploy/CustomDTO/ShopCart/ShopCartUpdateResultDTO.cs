using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
   /// <summary>
   /// 购物车项数量 修改返回结果
   /// </summary>
    [Serializable()]
    [DataContract]
    public class ShopCartUpdateResultDTO
    {
        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }

        /// <summary>
        ///  带属性的商品库存
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityAttrStock { get; set; }

        /// <summary>
        /// 参与活动个人限购数量 
        /// </summary>
        [DataMemberAttribute()]
        public int LimitBuyEach { get; set; }

        /// <summary>
        /// 参与活动个人限购总数量 
        /// </summary>
        [DataMemberAttribute()]
        public int LimitBuyTotal { get; set; }

        /// <summary>
        ///  
        /// </summary>
        [DataMemberAttribute()]
        public int SurplusLimitBuyTotal { get; set; }     

        /// <summary>
        /// 当前购物车商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }
    }
}
