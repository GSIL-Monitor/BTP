using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 下订单页自提点查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeStationSearchDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 类型：1 按Id查；2 按EsAppId,UserId查
        /// </summary>
        [DataMember]
        public int SearchType { get; set; }
    }
}
