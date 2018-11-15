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
    public class CategoryCache2DTO : CategoryCacheDTO
    {
        /// <summary>
        /// 图标
        /// </summary>
        [DataMemberAttribute()]
        public string icno { get; set; }
    }
}
