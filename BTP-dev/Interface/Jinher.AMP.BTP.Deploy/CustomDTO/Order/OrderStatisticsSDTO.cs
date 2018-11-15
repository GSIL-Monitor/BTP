using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单统计表
    /// </summary>
    [Serializable()]
    [DataContract]
     public  class OrderStatisticsSDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMemberAttribute()]
        public string Account { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [DataMemberAttribute()]
        public Guid SubId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 一年订单额
        /// </summary>
        [DataMemberAttribute()]
        public decimal SumRealPrice { get; set; }
        /// <summary>
        /// 一年订单量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderCount { get; set; }
        /// <summary>
        /// 一年内最后一笔订单时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime LastSubTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ModifiedOn { get; set; }
    }
}
