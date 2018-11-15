using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 生成结算单DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountCreateDTO
    {
        /// <summary>
        ///  商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// AppIds
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        [DataMember]
        public DateTime AmountDate { get; set; }
    }
}
