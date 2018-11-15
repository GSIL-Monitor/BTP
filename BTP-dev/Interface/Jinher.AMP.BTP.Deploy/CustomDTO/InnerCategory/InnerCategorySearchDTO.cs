using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分类查询dto
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InnerCategorySearchDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 是否显示搜索菜单
        /// </summary>
        [DataMember]
        public bool IsShowSearchMenu { get; set; }
    }
}
