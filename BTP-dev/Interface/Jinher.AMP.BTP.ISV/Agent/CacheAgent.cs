
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/9/9 15:11:35
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
    /// <summary>
    /// 缓存服务
    /// </summary>
    public class CacheAgent : BaseBpAgent<ICache>, ICache
    {
        public void RemoveAppCache()
        {

            try
            {
                //调用代理方法
                base.Channel.RemoveAppCache();

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
        }
    }
}
