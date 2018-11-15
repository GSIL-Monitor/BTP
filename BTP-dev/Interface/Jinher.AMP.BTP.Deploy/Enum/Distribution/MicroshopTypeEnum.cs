using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 微小店类型
    /// </summary>
    [DataContract]
    public enum MicroshopTypeEnum
    {
        /// <summary>
        /// 其他
        /// </summary>
        [EnumMember]
        Other = 0,
        /// <summary>
        /// 分销
        /// </summary>
        [EnumMember]
        Distribution = 1

    }
}
