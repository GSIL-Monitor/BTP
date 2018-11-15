using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// AppSet排序
    /// </summary>
    [Serializable]
    [DataContract]
    public class CateringCommodityDTO
    {
        /// <summary>
        /// 分类列表
        /// </summary>
        [DataMember]
        public List<CategorySDTO> CategoryList { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<CommodityListIICDTO> CommodityList { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }


    }
}
