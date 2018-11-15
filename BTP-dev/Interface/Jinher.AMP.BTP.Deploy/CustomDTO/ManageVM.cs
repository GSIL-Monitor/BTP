using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.JAP.BF.BE.Deploy.Base;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class ManageVM : SearchBase
    {
        /// <summary>
        /// 上级分销商
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 分销商昵称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 排序字段：0：注册时间，1：销量，2：佣金，3：下级分销商数
        /// </summary>
        [DataMember]
        public int SortCol { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否查询
        /// </summary>
        [DataMember]
        public int Ynos { get; set; }
    }
}
