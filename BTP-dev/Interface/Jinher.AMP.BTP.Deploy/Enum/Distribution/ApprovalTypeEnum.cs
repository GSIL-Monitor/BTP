using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 审核方式
    /// </summary>
    [DataContract]
    public enum ApprovalTypeEnum
    {
        /// <summary>
        /// 错误
        /// </summary>
        [EnumMember]
        Error = 0,
        /// <summary>
        /// 自动
        /// </summary>
        [EnumMember]
        Auto = 1,
        /// <summary>
        /// 手动
        /// </summary>
        [EnumMember]
        Manual = 2,
    }
}
