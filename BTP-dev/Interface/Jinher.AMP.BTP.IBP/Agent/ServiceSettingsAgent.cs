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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class ServiceSettingsAgent : BaseBpAgent<IServiceSettings>, IServiceSettings
    {

        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetALLServiceSettingsList(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetALLServiceSettingsList(model);

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



        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetServiceSettingsList(List<Guid> ids)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetServiceSettingsList(ids);

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


        public ResultDTO SaveServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveServiceSettings(model);

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


        public ResultDTO UpdateServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateServiceSettings(model);

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


        public ResultDTO DeleteServiceSettings(Guid id)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DeleteServiceSettings(id);

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


        public Jinher.AMP.BTP.Deploy.ServiceSettingsDTO GetServiceSettings(Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.ServiceSettingsDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetServiceSettings(AppId);

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
