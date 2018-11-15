using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.App.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract()]
    public class CommoditySearchResultDTO
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember()]
        public List<CommodityListDTO> CommodityList { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMember()]
        public int TotalCount { get; set; }
    }

    [Serializable()]
    [DataContract()]
    public class GetCommoditySearchResultDTO
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember()]
        public List<CommoditySearchListDTO> CommodityList { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMember()]
        public int TotalCount { get; set; }
    }

    [Serializable()]
    [DataContract()]
    public class CommoditySearchForAppsResultDTO
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember()]
        public List<CommodityListCategoryDTO> CommodityList { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMember()]
        public int TotalCount { get; set; }
    }

    [Serializable()]
    [DataContract()]
    public class CommodityNewSearchResultDTO
    {
        [DataMember]
        public List<CommodityListCDTO> CommodityList { get; set; }

        [DataMember]
        public List<BrandwallDTO> BrandWallList { get; set; }

        [DataMember]
        public List<CategoryDTO> CategoryList { get; set; }

        [DataMember]
        public List<AppIdNameIconDTO> AppInfoList { get; set; }
    }
}
