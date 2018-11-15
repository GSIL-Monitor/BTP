
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/5/26 14:14:26
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

    public class WxMessageAgent : BaseBpAgent<IWxMessage>, IWxMessage
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto DealMessage(Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiDto message)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto result;

            try
            {
                //调用代理方法
                result = base.Channel.DealMessage(message);

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
