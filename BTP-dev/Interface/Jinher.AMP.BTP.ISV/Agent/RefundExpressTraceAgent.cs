
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/5/22 13:11:12
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

    public class RefundExpressTraceAgent : BaseBpAgent<IRefundExpressTrace>, IRefundExpressTrace
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRefundExpress(Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateRefundExpress(retd);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO>> GetRefundExpressTrace()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetRefundExpressTrace();

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
