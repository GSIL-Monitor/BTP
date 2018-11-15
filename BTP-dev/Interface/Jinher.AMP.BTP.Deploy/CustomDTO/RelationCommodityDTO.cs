using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class RelationCommodityDTO
    {

        /// <summary>
        /// 关联商品的ID
        /// </summary>
        [DataMember()]
        public Guid RelationCommodityId { get; set; }

        /// <summary>
        /// 关联商品的图片
        /// </summary>
        [DataMember()]
        public string CommodityPicturesPath { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyEach { get; set; }
        /// <summary>
        /// 促销商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyTotal { get; set; }
        /// <summary>
        /// 剩余促销商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int? SurplusLimitBuyTotal { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否进行中的众筹
        /// </summary>
        [DataMember]
        public bool IsActiveCrowdfunding { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        [DataMemberAttribute()]
        public int? State { get; set; }
        /// <summary>
        /// 商品市场价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

    }
}
