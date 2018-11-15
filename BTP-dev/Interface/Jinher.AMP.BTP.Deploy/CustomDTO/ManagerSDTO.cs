using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
     public class ManagerSDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 分销商ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Esappid { get; set; }
        /// <summary>
        /// 上级分销商ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ParentId { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }
        /// <summary>
        /// 分销商昵称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 分销商账号
        /// </summary>
        [DataMemberAttribute()]
        public string UserCode { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        [DataMemberAttribute()]
        public Decimal OrderAmount { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        [DataMemberAttribute()]
        public Decimal CommissionAmount { get; set; }
        /// <summary>
        /// 下级分销商数
        /// </summary>
        [DataMemberAttribute()]
        public int UnderlingCount { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime UserSubTime { get; set; }
        /// <summary>
        /// 分销关系Id，（层级关系从上到下分销商Id以.分割组成的字符串）
        /// </summary>
        [DataMemberAttribute()]
        public string Key { get; set; }
        /// <summary>
        /// 分销商层级
        /// </summary>
        [DataMemberAttribute()]
        public int Level { get; set; }
        /// <summary>
        /// 分销商层级
        /// </summary>
        [DataMemberAttribute()]
        public bool HasIdentityInfo { get; set; }
        /// <summary>
        /// 分销商申请ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ApplyId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMemberAttribute()]
        public string Remarks { get; set; }
    }
}
