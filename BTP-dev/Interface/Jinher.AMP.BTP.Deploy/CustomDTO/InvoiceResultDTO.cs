using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 发票信息结果类
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceResultDTO
    {
        /// <summary>
        /// 发票信息列表
        /// </summary>
        [DataMember]
        public List<InvoiceInfoDTO> InvoiceInfoList { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
