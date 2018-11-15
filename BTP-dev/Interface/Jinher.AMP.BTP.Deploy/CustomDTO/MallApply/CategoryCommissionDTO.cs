using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.MallApply
{
    /// <summary>
    /// 分类佣金实体
    /// </summary>
    [DataContract]
    [Serializable]
    public class CategoryCommissionDTO : SearchBase
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime SubTime { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        [DataMember]
        public Guid MallApplyId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public DateTime EffectiveTime { get; set; }

        [DataMember]
        public Decimal Commission { get; set; }

        [DataMember]
        public Guid CategoryId { get; set; }

        [DataMember]
        public bool IsDel { get; set; }

       
    }
}
