using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 发票类型
    /// </summary>
    [DataContract]
    public enum InvoiceCategoryEnum
    {
        /// <summary>
        /// 增值税普通发票
        /// </summary>
        [EnumMemberAttribute]
        Ordinary = 1,
        /// <summary>
        /// 电子发票
        /// </summary>
        [EnumMemberAttribute]
        Electronic = 2,
        /// <summary>
        /// 增值税专用发票
        /// </summary>
        [EnumMemberAttribute]
        VAT = 4
    }
}
