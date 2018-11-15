﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BE
{
    public partial class InvoiceJounal
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
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="subId"></param>
        /// <param name="stateFrom"></param>
        /// <param name="stateTo"></param>
        public InvoiceJounal(Guid invoiceId, Guid subId, int stateFrom, int stateTo)
        {
            CreateInvoiceJounal();
            InvoiceId = invoiceId;
            SubTime = DateTime.Now;
            ModifiedOn = DateTime.Now;
            SubId = subId;
            StateFrom = stateFrom;
            StateTo = stateTo;
        }
        public InvoiceJounal()
        {
            
        }
    }
}



