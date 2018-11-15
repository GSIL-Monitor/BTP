
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/6/1 16:16:23
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

    public class PaySourceAgent : BaseBpAgent<IPaySource>, IPaySource
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PaySourceDTO> GetAllPaySources()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PaySourceDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllPaySources();

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
        public string GetPaymentName(int payment)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPaymentName(payment);

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
