using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品列表WEB页面
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityPromVM
    {
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
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMemberAttribute()]
        public string Code { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int? Stock { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        [DataMemberAttribute()]
        public int? State { get; set; }
        /// <summary>
        /// 商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int? Total { get; set; }
        /// <summary>
        /// 收藏数量
        /// </summary>
        [DataMemberAttribute()]
        public int? CollectNum { get; set; }
        /// <summary>
        /// 评价数量
        /// </summary>
        [DataMemberAttribute()]
        public int? ReviewNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime Subtime { get; set; }
        /// <summary>
        /// 商品分类列表
        /// </summary>
        [DataMemberAttribute()]
        public List<CategoryDTO> Categorys { get; set; }

        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyEach { get; set; }
        /// <summary>
        /// 促销限购商品总数
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyTotal { get; set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// 是否有优惠
        /// </summary>
        [DataMemberAttribute()]
        public int IsPro { get; set; }



        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 进货价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? CostPrice { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }
    }
}
