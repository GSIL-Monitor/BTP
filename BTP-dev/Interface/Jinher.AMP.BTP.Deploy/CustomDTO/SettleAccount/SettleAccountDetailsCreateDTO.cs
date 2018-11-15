using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 生成结算单DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountDetailsCreateDTO
    {
        /// <summary>
        ///  商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 订单开始日期
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 订单截止日期
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
