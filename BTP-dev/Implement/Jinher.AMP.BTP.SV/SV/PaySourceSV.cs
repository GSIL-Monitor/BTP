
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/6/2 14:59:33
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class PaySourceSV : BaseSv, IPaySource
    {

        /// <summary>
        /// 获取所有支付方式和描述信息。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PaySourceDTO> GetAllPaySources()
        {
            base.Do(false);
            return this.GetAllPaySourcesExt();

        }
        /// <summary>
        /// 获取担保交易类支付方式。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetSecuriedTransactionPayment()
        {
            base.Do(false);
            return this.GetSecuriedTransactionPaymentExt();

        }
        /// <summary>
        /// 获取担保交易类支付方式,排除金币。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetSecTransWithoutGoldPayment()
        {
            base.Do(false);
            return this.GetSecTransWithoutGoldPaymentExt();

        }
        /// <summary>
        /// 获取直接到账类支付方式。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetDirectArrivalPayment()
        {
            base.Do(false);
            return this.GetDirectArrivalPaymentExt();

        }
    }
}