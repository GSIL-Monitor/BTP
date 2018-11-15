using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 应用组-商品信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSetCommodity2DTO : AppSetCommodityDTO
    {
        /// <summary>
        /// 排序号
        /// </summary>
        [DataMember]
        public double SetCategorySort { get; set; }

        /// <summary>
        /// 销售区域
        /// </summary>
        [DataMember]
        public string SaleAreas { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        [DataMember]
        public int Salesvolume { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public string ComAttribute { get; set; }
        
        /// <summary>
        /// 已上架分类
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
        public Guid CategoryID { get; set; }

        [DataMember]
        public decimal OrderWeight { get; set; }

        /// <summary>
        /// 仅为油卡排序提供数据
        /// </summary>
        [DataMember]
        public decimal? YouKaPercent { get; set; }

        [DataMember]
        public List<String> Tags { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class AppSetCommodityGrid2DTO
    {
        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMemberAttribute()]
        public int TotalCommodityCount { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMemberAttribute()]
        public List<AppSetCommodity2DTO> CommodityList { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMemberAttribute()]
        public List<ComdtyList4SelCDTO> ComdtyList4SelCdtos { get; set; }
    }
}
