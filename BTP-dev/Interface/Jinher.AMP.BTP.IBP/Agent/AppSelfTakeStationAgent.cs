
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/9/12 14:48:07
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

    public class AppSelfTakeStationAgent : BaseBpAgent<IAppSelfTakeStation>, IAppSelfTakeStation
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveAppSelfTakeStation(model);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateAppSelfTakeStation(model);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStations(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteAppSelfTakeStations(ids);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchResultSDTO GetAppSelfTakeStationList(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchSDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchResultSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppSelfTakeStationList(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO GetAppSelfTakeStationById(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppSelfTakeStationById(id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckUserIdExists(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckUserIdExists(userId, appId);

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
