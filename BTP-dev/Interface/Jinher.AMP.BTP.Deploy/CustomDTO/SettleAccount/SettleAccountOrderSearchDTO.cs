using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算管理订单列表搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountOrderSearchDTO : SearchBase
    {
        /// <summary>
        /// 结算ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
    }
}
