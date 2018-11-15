
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/11/23 10:08:09
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

    public class YJBJCardAgent : BaseBpAgent<IYJBJCard>, IYJBJCard
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Create(System.Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Create(orderId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJBJCardDTO> Get(System.Guid orderId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJBJCardDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.Get(orderId);

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
