using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class PromotionItemsVM
    {
        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 商品发布时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime PromotionSubTime { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// APPID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 收藏数
        /// </summary>
        [DataMemberAttribute()]
        public int TotalCollection { get; set; }
        /// <summary>
        /// 评价数
        /// </summary>
        [DataMemberAttribute()]
        public int TotalReview { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        [DataMemberAttribute()]
        public int Salesvolume { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMemberAttribute()]
        public string No_Codes { get; set; }

        /// <summary>
        /// 优惠价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyEach { get; set; }
        /// <summary>
        /// 促销商品限购数量
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyTotal { get; set; }
        /// <summary>
        /// 促销商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int? SurplusLimitBuyTotal { get; set; }
        /// <summary>
        /// 商品分类列表。
        /// </summary>
        [DataMemberAttribute()]
        public IEnumerable<string> CommodityCategorys { get; set; }

        /// <summary>
        /// 商品价格最小值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 商品价格最大值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MaxPrice { get; set; }
    }
}
