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
    public class CommodityVM
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
        /// 商品价格最小值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 商品价格最大值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 判断商品是否有属性 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public int HaveAttr { get; set; }


        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

        /// <summary>
        /// 是否为分销商品
        /// </summary>
        [DataMemberAttribute()]
        public int IsDistribute { get; set; } 

        /// <summary>
        /// 直接上级分成比例
        /// </summary>
        [DataMemberAttribute()]
        public decimal? L1Percent { get; set; }

        /// <summary>
        /// 2级上级分成比例
        /// </summary>
        [DataMemberAttribute()]
        public decimal? L2Percent { get; set; }

        /// <summary>
        /// 3级上级分成比例
        /// </summary>
        [DataMemberAttribute()]
        public decimal? L3Percent { get; set; }

        /// <summary>
        /// 商品进货价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? CostPrice { get; set; }
    }



    /// <summary>
    /// 商品字段(openapi)
    /// </summary>
    [Serializable()]
    [DataContract]
    public class Commoditydto:CommodityDTO
    {
        /// <summary>
        /// 商城类目
        /// </summary>
        [DataMemberAttribute()]
        public string CityCategory { get; set; }

    }

}

