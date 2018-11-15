
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/6/1 16:16:20
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
    public class PaySourceFacade : BaseFacade<IPaySource>
    {

        /// <summary>
        /// 获取所有支付方式和描述信息。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PaySourceDTO> GetAllPaySources()
        {
            base.Do();
            return this.Command.GetAllPaySources();
        }
        /// <summary>
        /// 获取所有支付方式对应的描述信息。
        /// </summary>
        /// <param name="payment">支付方式编号</param>
        /// <returns></returns>
        public string GetPaymentName(int payment)
        {
            base.Do();
            return this.Command.GetPaymentName(payment);
        }
    }
}