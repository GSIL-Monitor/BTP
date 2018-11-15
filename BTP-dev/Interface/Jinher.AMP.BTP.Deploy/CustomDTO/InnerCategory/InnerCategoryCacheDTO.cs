using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 类目(缓存使用)
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InnerCategoryCacheDTO
    {
        /// <summary>
        /// appid
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

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
        /// 分类编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

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
        /// 父级全路径
        /// </summary>
        [DataMember]
        public string ParentCategoryPath { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Sort { get; set; }
    }
}
