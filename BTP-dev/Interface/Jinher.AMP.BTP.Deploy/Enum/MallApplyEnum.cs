using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 商城入驻状态类型
    /// </summary>
    [DataContract]
    public enum MallApplyEnum
    {
        /// <summary>
        /// 审核中【入驻申请】
        /// </summary>
        [Description("审核中【入驻申请】")]
        RZSQ = 0,

        /// <summary>
        /// 审核中【取消入驻】
        /// </summary>
        [Description("审核中【取消入驻】")]
        QXRZ = 1,

         /// <summary>
        /// 已审核【通过】
        /// </summary>
        [Description("已审核【通过】")]
        TG = 2,

        /// <summary>
        /// 已审核【不通过】
        /// </summary>
        [Description("已审核【不通过】")]
        BTG = 3,

         /// <summary>
        /// 已审核【挂起】
        /// </summary>
        [Description("已审核【挂起】")]
        GQ = 4,

        /// <summary>
        /// 取消入驻确认
        /// </summary>
        [Description("取消入驻申请")]
        QXRZQR=5
        

    }
}
