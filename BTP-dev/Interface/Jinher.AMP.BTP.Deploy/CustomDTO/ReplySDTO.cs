using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 回复
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ReplySDTO
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReviewId { get; set; }
        /// <summary>
        /// 回复人ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReplyerId { get; set; }
        /// <summary>
        /// 回复人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReplyerName { get; set; }
        /// <summary>
        /// 回复人头像
        /// </summary>
        [DataMemberAttribute()]
        public string ReplyerHead{ get; set; }
        /// <summary>
        /// 被回复人ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid PreId { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }

        /// <summary>
        /// 显示时间
        /// </summary>
        [DataMember]
        public string ShowTime { get; set; }
    }
}
