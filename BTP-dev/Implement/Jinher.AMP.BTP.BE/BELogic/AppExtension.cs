

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
namespace Jinher.AMP.BTP.BE
{
    public partial class AppExtension
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }

        #endregion
        /// <summary>
        /// 预制设置：默认发票类型
        /// </summary>
        private const int InvoiceDefaultConst = (int)InvoiceCategoryEnum.Ordinary;
        /// <summary>
        /// 预制设置：发票设置
        /// </summary>
        private const int InvoiceValuesConst = (int)InvoiceCategoryEnum.Ordinary;
        /// <summary>
        /// 是否选择增值税普通发票
        /// </summary>
        /// <returns></returns>
        public bool IsOrdinaryInvoice()
        {
            return (InvoiceValues & (int)InvoiceCategoryEnum.Ordinary) > 0;
        }
        /// <summary>
        /// 是否选择电子发票
        /// </summary>
        /// <returns></returns>
        public bool IsElectronicInvoice()
        {
            return (InvoiceValues & (int)InvoiceCategoryEnum.Electronic) > 0;
        }
        /// <summary>
        /// 是否选择增值税专用发票
        /// </summary>
        /// <returns></returns>
        public bool IsVATInvoice()
        {
            return (InvoiceValues & (int)InvoiceCategoryEnum.VAT) > 0;
        }
        /// <summary>
        /// 设置发票
        /// </summary>
        /// <param name="isOrdinaryInvoice"></param>
        /// <param name="isElectronicInvoice"></param>
        /// <param name="isVATInvoice"></param>
        public void SetInvoiceValues(bool isOrdinaryInvoice, bool isElectronicInvoice, bool isVATInvoice)
        {
            if (isOrdinaryInvoice)
                this.InvoiceValues = this.InvoiceValues | (int)InvoiceCategoryEnum.Ordinary;
            if (isElectronicInvoice)
                this.InvoiceValues = this.InvoiceValues | (int)InvoiceCategoryEnum.Electronic;
            if (isVATInvoice)
                this.InvoiceValues = this.InvoiceValues | (int)InvoiceCategoryEnum.VAT;
        }
    }
}



