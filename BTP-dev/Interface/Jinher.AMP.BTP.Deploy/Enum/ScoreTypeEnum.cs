using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 积分类型
    /// </summary>
    [DataContract]
    public enum ScoreTypeEnum
    {

        /// <summary>
        /// 无积分
        /// </summary>
        [EnumMemberAttribute]
        None = 0,
        /// <summary>
        /// 店铺积分
        /// </summary>
        [EnumMemberAttribute]
        Self = 1,
        /// <summary>
        /// 通用积分
        /// </summary>
        [EnumMemberAttribute]
        Currency = 2,

    }
}
