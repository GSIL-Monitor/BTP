using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityListInputDTO : SearchBase
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// Area Code
        /// </summary>
        [DataMember]
        public string AreaCode { get; set; }

        /// <summary>
        /// 是否查询促销
        /// </summary>
        [DataMember]
        public int IsChkTime { get; set; }

        /// <summary>
        /// 促销开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 促销结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }






        /// <summary>
        /// 商品所属类别
        /// </summary>
        [DataMember]
        public string Categorys { get; set; }
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
    /// 商品列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityListDTO
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        [DataMember]
        public string CommodityCategory { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string CommodityPicture { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }
    }

    /// <summary>
    /// 商品列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySearchListDTO
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }
    }

    /// <summary>
    /// 商品列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityInfoListDTO
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [DataMember]
        public string Pic { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }
    }
}
