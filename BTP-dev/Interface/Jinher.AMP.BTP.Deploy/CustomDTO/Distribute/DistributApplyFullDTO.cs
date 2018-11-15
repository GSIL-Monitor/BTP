using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商申请设置
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributApplyFullDTO
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
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 是否有条件
        /// </summary>
        [DataMember]
        public bool HasIdentity { get; set; }

        /// <summary>
        /// 审核设置Id
        /// </summary>
        [DataMember]
        public Guid RuleId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [DataMember]
        public string PicturePath { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remarks { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public DistributeApplyStateEnum State { get; set; }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        [DataMember]
        public string RefuseReason { get; set; }

        /// <summary>
        /// 审核人Id
        /// </summary>
        [DataMember]
        public Guid AuditorId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime AuditTime { get; set; }

        /// <summary>
        /// 父级分销id
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 申请资料集合
        /// </summary>
        [DataMember]
        public List<DistributionIdentityFullDTO> DistributionIdentityFullDtos { get; set; }

        /// <summary>
        /// 是否是编辑状态
        /// </summary>
        [DataMember]
        public bool IsModified { get; set; }
    }
}
