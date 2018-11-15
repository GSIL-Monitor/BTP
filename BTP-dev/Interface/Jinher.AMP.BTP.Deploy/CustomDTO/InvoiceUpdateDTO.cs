using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 修改发票类
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceUpdateDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 发票状态：0:待付款，1:待开票,2:已开票,3:已发出,4:已作废
        /// </summary>
        [DataMember]
        public Int32 State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 修改内容类型：1：修改状态；2：修改备注
        /// </summary>
        [DataMember]
        public Int32 ModifyType { get; set; }

    }
}
