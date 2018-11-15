using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 京东发货流程类型
    /// </summary>
    [DataContract]
    public enum JdEnum
    {
        /// <summary>
        /// 预占
        /// </summary>
        [Description("预占")]
        YZ= 1,

         /// <summary>
        /// 被拆分
        /// </summary>
        [Description("被拆分")]
        BCF = 2,

        /// <summary>
        /// 拒收
        /// </summary>
        [Description("拒收")]
        JS = 3,

        /// <summary>
        /// 确认收货
        /// </summary>
        [Description("确认收货")]
        QRSH = 4

       

    }



}
