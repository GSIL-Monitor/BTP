
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/6/2 14:59:34
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
        public System.Collections.Generic.List<int> GetSecuriedTransactionPayment()
        {
            //定义返回值
            System.Collections.Generic.List<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSecuriedTransactionPayment();

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
        public System.Collections.Generic.List<int> GetSecTransWithoutGoldPayment()
        {
            //定义返回值
            System.Collections.Generic.List<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSecTransWithoutGoldPayment();

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
        public System.Collections.Generic.List<int> GetDirectArrivalPayment()
        {
            //定义返回值
            System.Collections.Generic.List<int> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDirectArrivalPayment();

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
