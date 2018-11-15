using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 赠品活动商品搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionCommoditySearchDTO : SearchBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }




        /// <summary>
        /// 商品所属类别
        /// </summary>
        [DataMember]
        public List<string> Categorys { get; set; }
        /// <summary>
        /// 商品最小毛利率
        /// </summary>
        [DataMember]
        public string MinInterestRate { get; set; }
        /// <summary>
        /// 商品最大毛利率
        /// </summary>
        [DataMember]
        public string MaxInterestRate { get; set; }
        /// <summary>
        /// 商品最小价格
        /// </summary>
        [DataMember]
        public string MinPrice { get; set; }
        /// <summary>
        /// 商品最大价格
        /// </summary>
        [DataMember]
        public string MaxPrice { get; set; }
    }

    /// <summary>
    /// 赠品活动商品搜索结果DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionCommoditySearchResultDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string Pic { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }

        /// <summary>
        /// 商品SKU编码
        /// </summary>
        [DataMember]
        public List<CommoditySKUModel> SKU { get; set; }

        [Serializable]
        [DataContract]
        public class CommoditySKUModel
        {
            /// <summary>
            /// 商品SKUId
            /// </summary>
            [DataMember]
            public Guid Id { get; set; }

            /// <summary>
            /// 商品SKU名称
            /// </summary>
            [DataMember]
            public string Name { get; set; }

            /// <summary>
            /// 商品SKU编码
            /// </summary>
            [DataMember]
            public string Code { get; set; }

            /// <summary>
            /// 商品SKU价格
            /// </summary>
            [DataMember]
            public decimal Price { get; set; }

            /// <summary>
            /// 商品SKU库存
            /// </summary>
            [DataMember]
            public int Stock { get; set; }
        }
    }
}
