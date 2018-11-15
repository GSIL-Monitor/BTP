using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InnerCategoryListDTO
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [DataMember]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [DataMember]
        public int CurrentLevel { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Sort { get; set; }

        /// <summary>
        /// 次级分类
        /// </summary>
        [DataMember]
        public List<InnerCategoryListDTO> Children { get; set; }
    }
}
