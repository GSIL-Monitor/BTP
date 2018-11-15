using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 查询应用交易金额信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QryOrderTradeMoneyDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 交易额
        /// </summary>
        [DataMember]
        public decimal TradeMoney { get; set; }

        /// <summary>
        /// 待入账总额
        /// </summary>
        [DataMember]
        public decimal PayTradeMoney { get; set; }
    }
}
