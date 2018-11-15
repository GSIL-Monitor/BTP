using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class SynchronizeJDServiceStateDTO : JDEclpOrderRefundAfterSalesJournalDTO
    {
        /// <summary>
        /// 服务单项列表
        /// </summary>
        [DataMemberAttribute()]
        public List<JDEclpOrderRefundAfterSalesItemDTO> ServiceItemList { get; set; }
    }
}
