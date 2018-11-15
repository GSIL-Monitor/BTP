using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单钱款去向查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderMoneyToSearch
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// AppIds
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }
    }
}
