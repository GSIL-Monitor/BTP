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
    public enum OrderSrcTypeEnum
    {
        /// <summary>
        /// 广告
        /// </summary>
        [EnumMemberAttribute]
        AD = 0,
        /// <summary>
        /// 好运来
        /// </summary>
        [EnumMemberAttribute]
        Happy = 1
        
    }
}
