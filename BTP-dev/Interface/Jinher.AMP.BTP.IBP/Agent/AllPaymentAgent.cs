
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/8 16:23:11
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

    public class AllPaymentAgent : BaseBpAgent<IAllPayment>, IAllPayment
    {
        public string GetNameById(System.Guid allPaymentId)
        {
            //定义返回值
            string result;

            try
            {
                //调用代理方法
                result = base.Channel.GetNameById(allPaymentId);

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
