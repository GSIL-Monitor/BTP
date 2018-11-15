using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 电商BTP的APP消息的ContentCode
    /// </summary>
    [DataContract]
    public enum MobileMsgCodeEnum
    {
        /// <summary>
        /// 通用消息
        /// </summary>
        [EnumMemberAttribute]
        Common = 100
    }
}
