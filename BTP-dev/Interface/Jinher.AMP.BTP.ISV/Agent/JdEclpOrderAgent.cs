
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/4/8 19:46:33
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

    public class JdEclpOrderAgent : BaseBpAgent<IJdEclpOrder>, IJdEclpOrder
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDOrderState(Jinher.AMP.BTP.Deploy.JDEclpOrderJournalDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SynchronizeJDOrderState(arg);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDServiceState(Jinher.AMP.BTP.Deploy.CustomDTO.SynchronizeJDServiceStateDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SynchronizeJDServiceState(arg);

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
