using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 排序字段
    /// </summary>
    [DataContract]
    public enum CommoditySortEnum
    {
        /// <summary>
        /// 销量
        /// </summary>
        [EnumMemberAttribute]
        Salesvolume = 0,
        /// <summary>
        /// 价格
        /// </summary>
        [EnumMemberAttribute]
        Price = 1,
        /// <summary>
        /// 发布时间
        /// </summary>
        [EnumMemberAttribute]
        SubTime = 2
    }
}
