using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 订单起算状态
    /// </summary>
    [DataContract]
    public enum CalcOrderStateEnum
    {
        /// <summary>
        /// 其他
        /// </summary>
        [EnumMember]
        Other = 0,
        /// <summary>
        /// 支付后
        /// </summary>
        [EnumMember]
        Payed = 1,
        /// <summary>
        /// 订单完成
        /// </summary>
        [EnumMember]
        Finished = 2,
    }
}
