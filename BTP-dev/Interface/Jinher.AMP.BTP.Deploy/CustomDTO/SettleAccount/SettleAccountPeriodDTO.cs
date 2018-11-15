using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算周期DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountPeriodDTO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 商城ID
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 天数
        /// </summary>
        [DataMember]
        public int NumOfDay { get; set; }

        /// <summary>
        /// 是否使用售后完成时间结算
        /// </summary>
        [DataMember]
        public bool UseAfterSalesEndTime { get; set; }
    }
}
