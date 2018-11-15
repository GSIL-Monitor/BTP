using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 订单起算状态
    /// </summary>
    [DataContract]
    public enum DistributeApplyStateEnum
    {
        /// <summary>
        /// 待提交审核
        /// </summary>
        [EnumMember]
        [Description("待提交审核")]
        Other = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [EnumMember]
        [Description("待审核")]
        PendingAudit = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        [EnumMember]
        [Description("审核通过")]
        Audit = 2,
        /// <summary>
        /// 审核不通过
        /// </summary>
        [EnumMember]
        [Description("审核不通过")]
        AuditRefuse = 3,
        /// <summary>
        /// 申请再审核
        /// </summary>
        [EnumMember]
        [Description("申请再审核")]
        AuditAgain = 4
    }
}
