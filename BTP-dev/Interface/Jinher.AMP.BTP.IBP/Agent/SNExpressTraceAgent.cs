using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class SNExpressTraceAgent : BaseBpAgent<ISNExpressTrace>, ISNExpressTrace
    {

        public List<SNExpressTraceDTO> GetExpressTrace(string orderId, string orderItemId)
        {
            //定义返回值
            List<SNExpressTraceDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetExpressTrace(orderId, orderItemId);
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
            }            
            //返回结果
            return result;
        }

        public bool ChangeLogistStatusForJob()
        {
            //定义返回值
            bool result;
            try
            {
                //调用代理方法
                result = base.Channel.ChangeLogistStatusForJob();
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
            }            
            //返回结果
            return result;
        }

      
    }
}
