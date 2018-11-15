using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    ///订单支付详情状态
    /// </summary>
    [DataContract]
    public enum OrderPayDetailStateEnum:byte
    {
        /// <summary>
        /// 默认值
        /// </summary>
        [DataMember]
        Default = 0,


        /// <summary>
        /// 已退回
        /// </summary>
        [DataMember]
        Retreat = 10

    }
}
