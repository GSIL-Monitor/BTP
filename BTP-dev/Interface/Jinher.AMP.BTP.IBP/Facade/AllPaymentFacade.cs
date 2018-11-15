
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/8 16:23:05
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
    public class AllPaymentFacade : BaseFacade<IAllPayment>
    {

        /// <summary>
        /// 由ID得到支付名称
        /// </summary>
        /// <param name="allPaymentId">支付ID</param>
        /// <returns></returns>
        public string GetNameById(System.Guid allPaymentId)
        {
            base.Do();
            return this.Command.GetNameById(allPaymentId);
        }
    }
}