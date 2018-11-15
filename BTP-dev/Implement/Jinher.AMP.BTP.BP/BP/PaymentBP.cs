
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/30 13:00:42
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
    public partial class PaymentBP : BaseBP, IPayment
    {

        /// <summary>
        /// 获取所有支付方式
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsForEditDTO> GetAllPayment(System.Guid appId)
        {
            base.Do();
            return this.GetAllPaymentExt(appId);
        }
        /// <summary>
        /// 修改用户支付方式
        /// </summary>
        /// <param name="paymentsDTO">支付方式实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePayment(Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsVM paymentsVM)
        {
            base.Do();
            return this.UpdatePaymentExt(paymentsVM);
        }
        /// <summary>
        /// 是否可以取消积分 (平台启用了分销并且设置了值，或启用了众销且设置了值，就不能取消。)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool IsEnableCancelScore(Guid appId)
        {
            base.Do(false);
            return this.IsEnableCancelScoreExt(appId);
        }
    }
}