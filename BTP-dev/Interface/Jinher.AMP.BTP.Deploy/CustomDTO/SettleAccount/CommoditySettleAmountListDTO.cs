using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品结算价
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySettleAmountListDTO
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品结算价
        /// </summary>
        [DataMember]
        public decimal? SettlePrice { get; set; }

        /// <summary>
        /// 是否有一级属性
        /// </summary>
        [DataMember]
        public bool HasAttributes { get; set; }

        /// <summary>
        /// 是否有二级属性
        /// </summary>
        [DataMember]
        public bool HasSecondAttribute { get; set; }

        /// <summary>
        /// 一级属性名
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// 二级属性名
        /// </summary>
        [DataMember]
        public string SecondAttributeName { get; set; }

        /// <summary>
        /// 属性数据
        /// </summary>
        [DataMember]
        public List<CommodityAttribute> Attributes { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        [DataMember]
        public DateTime? EffectiveTime { get; set; }
    }

    /// <summary>
    /// 商品SKU
    /// </summary>
    public class CommodityAttribute
    {
        ///// <summary>
        ///// 属性
        ///// </summary>
        //[DataMember]
        //public string Attribute { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        [DataMember]
        public string AttributeValue { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品结算价
        /// </summary>
        [DataMember]
        public decimal? SettlePrice { get; set; }


        /// <summary>
        /// 是否有一级属性
        /// </summary>
        [DataMember]
        public bool HasAttributes { get; set; }

        /// <summary>
        /// 二级属性
        /// </summary>
        [DataMember]
        public List<CommodityAttribute> Attributes { get; set; }
    }

    /// <summary>
    /// 商品SKU结算价
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityAttributePrice
    {
        //[DataMember]
        //public string Attribute { get; set; }

        [DataMember]
        public string AttributeName { get; set; }

        [DataMember]
        public string AttributeValue { get; set; }

        [DataMember]
        public string SecAttributeName { get; set; }

        [DataMember]
        public string SecAttributeValue { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public decimal? SettlePrice { get; set; }
    }
}
