using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单查看DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderCheckDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }


        /// <summary>
        ///  流水号
        /// </summary>
        [DataMemberAttribute()]
        public string Batch { get; set; }
    }
}
