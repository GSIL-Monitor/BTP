
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/2/22 16:10:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class PaymentsAgent : BaseBpAgent<IPayments>, IPayments
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetPayments(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPayments(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> GetSetPayments()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PaymentsSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSetPayments();

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
        public System.Guid GetPayeeId(System.Guid appId)
        {
            //定义返回值
            System.Guid result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPayeeId(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.AlipayDTO GetAlipayInfo(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AlipayDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAlipayInfo(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> IsAllAppSupportCOD(System.Collections.Generic.List<System.Guid> appIds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> result;

            try
            {
                //调用代理方法
                result = base.Channel.IsAllAppSupportCOD(appIds);

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
