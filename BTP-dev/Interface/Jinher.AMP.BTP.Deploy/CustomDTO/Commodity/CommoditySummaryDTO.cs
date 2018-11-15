using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySummaryDTO
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品SKU
        /// </summary>
        [DataMember]
        public string Sku { get; set; }

        [DataMember]
        public Guid ShopCartItemId { get; set; }
    }

}
