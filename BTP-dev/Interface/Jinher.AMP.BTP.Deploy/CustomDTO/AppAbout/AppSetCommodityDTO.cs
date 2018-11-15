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
    public class AppSetCommodityDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }

        /// <summary>
        /// 应用图标
        /// </summary>
        [DataMemberAttribute()]
        public string AppIcon { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        ///商品的店铺ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid appId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityPic { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal CommodityPrice { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityStock { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

        /// <summary>
        /// 参加的商品分类名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityCategory { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class AppSetCommodityGridDTO
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
        public List<AppSetCommodityDTO> CommodityList { get; set; }
    }
}
