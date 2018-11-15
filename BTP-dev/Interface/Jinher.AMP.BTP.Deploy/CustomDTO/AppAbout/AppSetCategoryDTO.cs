using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 应用组-分类信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSetCategoryDTO
    {
        /// <summary>
        /// 分类id
        /// </summary>
        [DataMemberAttribute()]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [DataMemberAttribute()]
        public string CategoryName { get; set; }

        /// <summary>
        /// 分类下商品数
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityCount { get; set; }

        /// <summary>
        /// 子分类
        /// </summary>
        [DataMemberAttribute()]
        public List<AppSetCategoryDTO> ChildCategories { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 是否包含子节点
        /// </summary>
        [DataMemberAttribute()]
        public bool HasChildren { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
    }
}
