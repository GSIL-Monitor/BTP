
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/9/21 15:02:29
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

    public class MallApplyAgent : BaseBpAgent<IMallApply>, IMallApply
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyInfoList(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveMallApply(model);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateMallApply(model);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO GetMallApply(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApply(id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsHaveMallApply(System.Guid esAppId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.IsHaveMallApply(esAppId, appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppInfoDTO> GetMallApps(System.Guid esAppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApps(esAppId);

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

        public void GetMallAppsForJob(System.Guid esAppId)
        {            
            try
            {
                //调用代理方法
                base.Channel.GetMallAppsForJob(esAppId);

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
