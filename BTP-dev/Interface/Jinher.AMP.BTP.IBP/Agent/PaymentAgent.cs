
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/30 13:00:44
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class PaymentAgent : BaseBpAgent<IPayment>, IPayment
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsForEditDTO> GetAllPayment(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsForEditDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllPayment(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePayment(Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsVM paymentsVM)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdatePayment(paymentsVM);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 是否可以取消积分 (平台启用了分销并且设置了值，或启用了众销且设置了值，就不能取消。)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool IsEnableCancelScore(Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsEnableCancelScore(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
