
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/8/2 15:47:29
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class AppExtensionAgent : BaseBpAgent<IAppExtension>, IAppExtension
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtDTO GetBTPAppInfo(Jinher.AMP.BTP.Deploy.CustomDTO.AppSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBTPAppInfo(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppExtensionByAppId(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO> GetAppDownLoadInfo(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppDownLoadInfo(appId);

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
