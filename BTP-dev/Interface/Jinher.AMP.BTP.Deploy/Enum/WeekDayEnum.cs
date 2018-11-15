using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{

    /// <summary>
    /// 订单来源类型
    /// </summary>
    [DataContract]
    public enum WeekDayEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [EnumMember]
        None = -1,
        /// <summary>
        /// 星期一
        /// </summary>
        [EnumMember]
        Monday = 1,
        /// <summary>
        /// 星期二
        /// </summary>
        [EnumMember]
        Tuesday = 2,
        /// <summary>
        /// 星期三
        /// </summary>
        [EnumMember]
        Wednesday = 4,
        /// <summary>
        /// 星期四
        /// </summary>
        [EnumMember]
        Thursday = 8,
        /// <summary>
        /// 星期五
        /// </summary>
        [EnumMember]
        Friday = 16,
        /// <summary>
        /// 星期六
        /// </summary>
        [EnumMember]
        Saturday = 32,
        /// <summary>
        /// 星期日
        /// </summary>
        [EnumMember]
        Sunday = 64

    }
}
