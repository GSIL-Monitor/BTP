using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商申请 身份资料
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionIdentitySetFullDTO
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
        /// 字段值
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        [DataMember]
        public bool IsRequired { get; set; }

        /// <summary>
        /// RuleId
        /// </summary>
        [DataMember]
        public Guid RuleId { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [DataMember]
        public ApplyInfoTypeEnum ValueType { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }

        /// <summary>
        /// 字段名类型：姓名，身份证号等
        /// </summary>
        [DataMember]
        public int NameCategory { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
