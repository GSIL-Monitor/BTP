using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 发票设置
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceSettingDTO
    {
        /// <summary>
        /// app列表
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 是否选择增值税普通发票
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool IsOrdinaryInvoice { get; set; }
        /// <summary>
        /// 是否选择电子发票
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool IsElectronicInvoice { get; set; }
        /// <summary>
        /// 是否选择增值税专用发票
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool IsVATInvoice { get; set; }
        /// <summary>
        /// 默认发票设置
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public InvoiceCategoryEnum InvoiceDefault { get; set; }
        /// <summary>
        /// 用户是否有增票资质
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool IsVatInvoiceProof { get; set; }

        /// <summary>
        /// 发票设置
        /// </summary>
        public InvoiceSettingDTO()
        {
            IsOrdinaryInvoice = true;
            InvoiceDefault = InvoiceCategoryEnum.Ordinary;
        }
    }
}
