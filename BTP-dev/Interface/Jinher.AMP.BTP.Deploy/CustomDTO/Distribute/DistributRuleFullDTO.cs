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
    public class DistributRuleFullDTO
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
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }

        /// <summary>
        /// 修改者Id
        /// </summary>
        [DataMember]
        public Guid ModifiedId { get; set; }

        /// <summary>
        /// 是否有条件
        /// </summary>
        [DataMember]
        public bool HasCondition { get; set; }

        /// <summary>
        /// 是否需要身份验证信息
        /// </summary>
        [DataMember]
        public bool NeedIdentity { get; set; }

        /// <summary>
        /// 最小订单购买次数，-1代表不限
        /// </summary>
        [DataMember]
        public int OrderTimeLimit { get; set; }

        /// <summary>
        /// 最小购买金额,-1代表不限
        /// </summary>
        [DataMember]
        public decimal OrderAmountLimit { get; set; }

        /// <summary>
        /// 是否包含自订单商品
        /// </summary>
        [DataMember]
        public bool HasCustomComs { get; set; }

        /// <summary>
        /// 身份条件标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public ApprovalTypeEnum TiApprovalType { get; set; }

        /// <summary>
        /// 有效订单状态
        /// </summary>
        [DataMember]
        public DistributeApplyStateEnum StartCalcState { get; set; }

        /// <summary>
        /// 分销商规则说明
        /// </summary>
        [DataMember]
        public string RuleDesc { get; set; }

        /// <summary>
        /// 分销商 身份资料集合
        /// </summary>
        [DataMember]
        public List<DistributionIdentitySetFullDTO> DbIdentitySets { get; set; }
    }
}
