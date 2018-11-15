
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/6/2 14:59:32
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
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
        /// 获取担保交易类支付方式。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetSecuriedTransactionPayment()
        {
            base.Do();
            return this.Command.GetSecuriedTransactionPayment();
        }
        /// <summary>
        /// 获取担保交易类支付方式,排除金币。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetSecTransWithoutGoldPayment()
        {
            base.Do();
            return this.Command.GetSecTransWithoutGoldPayment();
        }
        /// <summary>
        /// 获取直接到账类支付方式。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<int> GetDirectArrivalPayment()
        {
            base.Do();
            return this.Command.GetDirectArrivalPayment();
        }
    }
}