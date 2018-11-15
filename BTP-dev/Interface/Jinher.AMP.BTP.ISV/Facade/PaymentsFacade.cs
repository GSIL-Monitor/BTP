
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/2/22 16:10:01
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
    public class PaymentsFacade : BaseFacade<IPayments>
    {

        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetPayments(System.Guid appId)
        {
            base.Do();
            return this.Command.GetPayments(appId);
        }
        /// <summary>
        /// 获取支付方式 --- 厂家直销
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetSetPayments()
        {
            base.Do();
            return this.Command.GetSetPayments();
        }
        /// <summary>
        /// 获取商家收款ID
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Guid GetPayeeId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetPayeeId(appId);
        }
        /// <summary>
        /// 获取支付宝信息
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AlipayDTO GetAlipayInfo(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAlipayInfo(appId);
        }
        /// <summary>
        /// 是不是所有店铺app都支持“货到付款”。
        /// </summary>
        /// <param name="appIds">店铺appId</param>
        /// <returns>是不是所有店铺app都支持“货到付款”</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> IsAllAppSupportCOD(System.Collections.Generic.List<System.Guid> appIds)
        {
            base.Do();
            return this.Command.IsAllAppSupportCOD(appIds);
        }
    }
}