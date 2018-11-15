using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 商品表中的属性列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComAttributeDTO
    {
        /// <summary>
        /// 属性分类
        /// </summary>
        [DataMember()]
        public string Attribute { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [DataMember()]
        public string SecondAttribute { get; set; }
    }


    [DataContract]
    [Serializable]
    public class ComAttributeHaveIdDTO : ComAttributeDTO
    {

        /// <summary>
        /// 属性ID
        /// </summary>
        [DataMember()]
        public Guid AttributeId { get; set; }

        /// <summary>
        /// 属性值ID
        /// </summary>
        [DataMember()]
        public Guid SecondAttributeId { get; set; }

    }

    [DataContract]
    [Serializable]
    public class ComAttributeOrder : ComAttributeHaveIdDTO
    {
        /// <summary>
        ///属性值排序时间
        /// </summary>
        [DataMember()]
        public DateTime OrderTime { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class AttrNameAndId
    {
        /// <summary>
        /// 属性分类
        /// </summary>
        [DataMember()]
        public Guid Id { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [DataMember()]
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class AttrNameAndIdOrder : AttrNameAndId
    {
        /// <summary>
        ///属性值排序时间
        /// </summary>
        [DataMember()]
        public DateTime OrderTime { get; set; }
    }

    /// <summary>
    /// 商品属性列表  //zgx-modify
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommodityStockDTO
    {
        /// <summary>
        /// 商品组合属性
        /// </summary>
        [DataMember()]
        public List<ComAttributeDTO> ComAttribute { get; set; }

        /// <summary>
        /// 商品属性ID集合
        /// </summary>
        [DataMember()]
        public List<ComAttributeHaveIdDTO> ComAttributeIds { get; set; }

        /// <summary>
        /// 商品属性ID集合
        /// </summary>
        [DataMember()]
        public List<ComAttributeOrder> ComAttributeIdOrders { get; set; }

        /// <summary>
        /// 组合价格
        /// </summary>
        [DataMember()]
        public decimal Price { get; set; }

        /// <summary>
        /// 组合数量
        /// </summary>
        [DataMember()]
        public int Stock { get; set; }

        /// <summary>
        /// 属性组合ID
        /// </summary>
        [DataMember()]
        public Guid Id { get; set; }
        /// <summary>
        /// 组合价格
        /// </summary>
        [DataMember()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        [DataMember()]
        public decimal? Duty { get; set; }


        /// <summary>
        /// 进货价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        [DataMember]
        public string BarCode { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 京东商品编码
        /// </summary>
        [DataMember]
        public string JDCode { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [DataMember]
        public string ThumImg { get; set; }

        /// <summary>
        /// 轮播图（多个使用","分割）
        /// </summary>
        [DataMember]
        public string CarouselImgs { get; set; }

        /// <summary>
        /// 二期系统商品编码
        /// </summary>
        [DataMember]
        public string ErQiCode { get; set; }
    }


    /// <summary>
    /// 商品属性列表  //zgx-modify
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommodityAttrStockDTO
    {
        /// <summary>
        /// 商品组合属性
        /// </summary>
        [DataMember()]
        public List<ComAttributeDTO> ComAttribute { get; set; }

        /// <summary>
        /// 组合价格
        /// </summary>
        [DataMember()]
        public decimal Price { get; set; }

        /// <summary>
        /// 组合数量
        /// </summary>
        [DataMember()]
        public int Stock { get; set; }

        /// <summary>
        /// 属性组合ID
        /// </summary>
        [DataMember()]
        public Guid Id { get; set; }

        /// <summary>
        /// 组合市场价
        /// </summary>
        [DataMember()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        [DataMember()]
        public decimal Duty { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [DataMember]
        public string ThumImg { get; set; }

        /// <summary>
        /// 轮播图（多个使用","分割）
        /// </summary>
        [DataMember]
        public string[] CarouselImgs { get; set; }

        /// <summary>
        /// 商品skuId
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
    }
    /// <summary>
    /// 商品库存属性
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComAttStockDTO
    {
        /// <summary>
        /// 商品组合属性集合 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityAttrStockDTO> CommodityStocks { get; set; }
        /// <summary>
        /// 商品属性列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttributeDTO> ComAttibutes { get; set; }
    }

}
