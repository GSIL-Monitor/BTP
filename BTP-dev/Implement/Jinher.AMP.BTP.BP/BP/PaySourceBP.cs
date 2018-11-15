
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/6/1 16:16:22
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class PaySourceBP : BaseBP, IPaySource
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
        /// 获取所有支付方式对应的描述信息。
        /// </summary>
        /// <param name="payment">支付方式编号</param>
        /// <returns></returns>
        public string GetPaymentName(int payment)
        {
            base.Do(false);
            return this.GetPaymentNameExt(payment);
        }
    }
}