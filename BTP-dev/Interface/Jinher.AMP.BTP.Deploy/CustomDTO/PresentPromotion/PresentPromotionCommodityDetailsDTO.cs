using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 赠品活动商品DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionCommodityDetailsDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品SKU ID
        /// </summary>
        [DataMember]
        public Guid SKUId { get; set; }

        /// <summary>
        /// 商品SKU名称
        /// </summary>
        [DataMember]
        public string SKUName { get; set; }

        /// <summary>
        /// 商品SKU编码
        /// </summary>
        [DataMember]
        public string SKUCode { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品单次最少购买数量
        /// </summary>
        [DataMember]
        public int Limit { get; set; }
    }
}
