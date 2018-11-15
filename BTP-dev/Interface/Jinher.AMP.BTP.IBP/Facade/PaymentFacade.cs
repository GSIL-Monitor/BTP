
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/30 13:00:40
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class PaymentFacade : BaseFacade<IPayment>
    {

        /// <summary>
        /// 获取所有支付方式
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsForEditDTO> GetAllPayment(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAllPayment(appId);
        }

        /// <summary>
        /// 修改用户支付方式
        /// </summary>
        /// <param name="paymentsDTO">支付方式实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePayment(Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsVM paymentsVM)
        {
            base.Do();
            return this.Command.UpdatePayment(paymentsVM);
        }

        /// <summary>
        /// 是否可以取消积分 (平台启用了分销并且设置了值，或启用了众销且设置了值，就不能取消。)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool IsEnableCancelScore(Guid appId)
        {
            base.Do();
            return this.Command.IsEnableCancelScore(appId);
        }
    }
}