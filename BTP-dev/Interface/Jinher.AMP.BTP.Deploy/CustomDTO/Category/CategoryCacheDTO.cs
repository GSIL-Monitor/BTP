using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 类目(缓存使用)
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CategoryCacheDTO
    {
        /// <summary>
        /// appid
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [DataMemberAttribute()]
        public int CurrentLevel { get; set; }

        /// <summary>
        /// 父级全路径
        /// </summary>
        [DataMemberAttribute()]
        public string ParentCategoryPath { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMemberAttribute()]
        public int Sort { get; set; }
    }
}
