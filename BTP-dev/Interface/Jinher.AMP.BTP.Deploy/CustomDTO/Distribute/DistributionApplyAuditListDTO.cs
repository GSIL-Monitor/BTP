using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商申请的审批历史记录
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionApplyAuditListDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 提交人Id
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }

        /// <summary>
        /// 标记
        /// </summary>
        [DataMember]
        public string Details { get; set; }

        /// <summary>
        /// 申请Id
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RefuseReason { get; set; }

        /// <summary>
        /// 审批通过
        /// </summary>
        [DataMember]
        public bool IsPass { get; set; }

        /// <summary>
        /// 处理日期 日期
        /// </summary>
        [DataMember]
        public string SubTimeDate
        {
            get { return SubTime.ToString("yyyy-M-d"); }
        }

        /// <summary>
        /// 处理日期 时间
        /// </summary>
        [DataMember]
        public string SubTimeTime
        {
            get { return SubTime.ToString("HH:mm"); }
        }

        /// <summary>
        /// 提交人
        /// </summary>
        [DataMember]
        public string SubName { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        [DataMember]
        public string AuditResult {
            get { return IsPass ? "审核通过" : "审核未通过：" + RefuseReason; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditDto"></param>
        /// <param name="subName"></param>
        public DistributionApplyAuditListDTO(DistributionApplyAuditDTO auditDto,string subName)
        {
            Id =auditDto.Id;
            SubTime = auditDto.SubTime;
            SubId = auditDto.SubId;
            Details = auditDto.Details;
            ApplyId = auditDto.ApplyId;
            RefuseReason = auditDto.RefuseReason;
            IsPass = auditDto.IsPass;
            SubName = subName;
        }
    }
}
