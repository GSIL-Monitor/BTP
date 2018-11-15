using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 资料值类型
    /// </summary>
    [DataContract]
    public enum ApplyInfoTypeEnum
    {
        /// <summary>
        /// 无效
        /// </summary>
        [EnumMember]
        None = 0,
        /// <summary>
        /// 文本
        /// </summary>
        [EnumMember]
        Text = 1,
        /// <summary>
        /// 文件
        /// </summary>
        [EnumMember]
        File = 2,
    }
}
