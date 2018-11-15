using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商身份设置值 
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionIdentityFullDTO
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
        /// 字段名
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 审核设置id
        /// </summary>
        [DataMember]
        public Guid RuleId { get; set; }

        /// <summary>
        /// 审核资料id
        /// </summary>
        [DataMember]
        public Guid IdentitySetId { get; set; }

        /// <summary>
        /// 申请人信息id
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// 资料值类型
        /// </summary>
        [DataMember]
        public ApplyInfoTypeEnum ValueType { get; set; }

        /// <summary>
        /// NameCategory
        /// </summary>
        [DataMember]
        public int NameCategory { get; set; }
    }
}
