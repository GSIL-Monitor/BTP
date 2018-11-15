using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 物流状态
    /// </summary>
    [DataContract]
    public enum DeliverystatusEnum
    {
        /// <summary>
        /// 在途中
        /// </summary>
        [Description("在途中")]
        ZTZ = 1,

        /// <summary>
        /// 派件中
        /// </summary>
        [Description("派件中")]
        PJZ = 2,

        /// <summary>
        /// 已签收
        /// </summary>
        [Description("已签收")]
        YQS = 3,

         /// <summary>
        /// 派送失败
        /// </summary>
        [Description("派送失败")]
        PSSB = 4,


    }
}
