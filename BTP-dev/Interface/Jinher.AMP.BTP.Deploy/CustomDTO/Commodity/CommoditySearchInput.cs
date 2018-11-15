using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品搜索DTO（YJB使用）
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySearchInput : SearchBase
    {
        /// <summary>
        /// 商城应用Id
        /// </summary>
        [DataMember]
        public Guid? EsAppId { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 包含的商品ID列表
        /// </summary>
        [DataMember]
        public List<Guid> ExistedCommodityId { get; set; }

        /// <summary>
        /// 不包含的商品ID列表
        /// </summary>
        [DataMember]
        public List<Guid> NotExistedCommodityId { get; set; }

        /// <summary>
        /// 最后修改时间（过滤指定时间后修改的商品）
        /// </summary>
        [DataMember]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 是否设置进货价
        /// </summary>
        [DataMember]
        public bool? HaveCostPrice { get; set; }

        [DataMember]
        public Guid CategoryId { get; set; }


        /// <summary>
        /// 用户多选的分类Id集合
        /// </summary>
        [DataMember]
        public List<Guid> CatgoryIdList { get; set; }
        [DataMember]
        public Guid AppId { get; set; }


        /// <summary>
        /// 最大毛利率
        /// </summary>
        [DataMember]
        public Decimal MaxRate { get; set; }

        /// <summary>
        /// 最小毛利率
        /// </summary>
        [DataMember]
        public Decimal MinRate { get; set; }

        /// <summary>
        /// 最大价格
        /// </summary>
        [DataMember]
        public Decimal MaxPrice { get; set; }

        /// <summary>
        /// 最小价格
        /// </summary>
        [DataMember]
        public Decimal MinPrice { get; set; }
    }

    /// <summary>
    /// 商品搜索结果
    /// </summary>
    public class CommodityListOutPut
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 商品Jd编码
        /// </summary>
        [DataMember]
        public string JdCode { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string Pic { get; set; }

        /// <summary>
        ///  商品进价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }

        /// <summary>
        ///  商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }

        /// <summary>
        /// 抵现比例
        /// </summary>
        public decimal Percent { get; set; }

        /// <summary>
        /// 油卡兑换券比例
        /// </summary>
        public decimal YouKaPercent { get; set; }

        public Guid Id { get; set; }

        public DateTime SubTime { get; set; }

        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 易捷币抵现金额（商品当前售价*商品易捷币抵现比例）
        /// </summary>
        public decimal YjbPrice { get; set; }

        /// <summary>
        /// 实际售价=当前售价-返油卡或易捷币可抵用金额
        /// </summary>
        public decimal RealPrice { get { return Price - YjbPrice; } }

        /// <summary>
        /// 实际毛利率=（实际售价-当前进价）/实际售价
        /// </summary>
        //public decimal GrossProfitRate { get { return CostPrice.HasValue ? Math.Truncate((RealPrice - CostPrice.Value) / RealPrice * 100) : 100; } }
        public decimal GrossProfitRate { get; set; }

        [DataMember]
        /// <summary>
        /// 商品分类Id
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 最大毛利率
        /// </summary>
        [DataMember]
        public Decimal MaxRate { get; set; }

        /// <summary>
        /// 最小毛利率
        /// </summary>
        [DataMember]
        public Decimal MinRate { get; set; }

        /// <summary>
        /// 最大价格
        /// </summary>
        [DataMember]
        public Decimal MaxPrice { get; set; }

        /// <summary>
        /// 最小价格
        /// </summary>
        [DataMember]
        public Decimal MinPrice { get; set; }
    }

    /// <summary>
    /// 商品搜索DTO（YJB使用）
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySearchByAppIdInput : SearchBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
    }
}
