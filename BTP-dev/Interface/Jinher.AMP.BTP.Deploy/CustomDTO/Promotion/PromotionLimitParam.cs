using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 校验活动限购参数(下订单时商品只能参加最优的一个活动)
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PromotionLimitParam
    {
        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMember]
        public Guid PromotionId { get; set; }

      
        /// <summary>
        /// 全场限购数量
        /// </summary>
        [DataMember]
        public int PromtionLimitBuyTotal { get; set; } 

        /// <summary>
        /// 当前用户在当前活动中购买的商品的总数量。
        /// </summary> 
        [DataMember]
        public int BuyInPromotion { get; set; }


        //================商品相关====================

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }


        /// <summary>
        /// 商品限购数量
        /// </summary>
        [DataMember]
        public int LimitBuyEach { get; set; }

        /// <summary>
        /// 活动商品销量
        /// </summary>
        [DataMember]
        public int SurplusLimitBuyTotal { get; set; }

        /// <summary>
        /// 参加促销商品的总数
        /// </summary>
        [DataMember]
        public int LimitBuyTotal { get; set; } 

        /// <summary>
        /// 当前订单中购买该商品的数量。
        /// </summary>
        [DataMember]
        public int CommodityBuyCount { get; set; }


    }
}
