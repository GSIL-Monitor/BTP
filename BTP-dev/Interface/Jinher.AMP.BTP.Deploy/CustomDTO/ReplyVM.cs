using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{


    [Serializable()]
    [DataContract]
    public class ReplyVM
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid ReviewId { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [DataMemberAttribute()]
        public string ReviewContent { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ReviewTime { get; set; }
        /// <summary>
        /// 被回复人ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid PreId { get; set; }
        /// <summary>
        /// 评价人
        /// </summary>
        [DataMemberAttribute()]
        public string ReviewUserName { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [DataMemberAttribute()]
        public string ReplyerDetails { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        [DataMemberAttribute()]
        public string ReplyerUserName { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ReplyerSubTime { get; set; }
    }
}
