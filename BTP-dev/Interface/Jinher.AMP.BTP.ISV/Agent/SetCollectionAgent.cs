
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/5/30 16:02:36
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

    public class SetCollectionAgent : BaseBpAgent<ISetCollection>, ISetCollection
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityCollection(System.Guid commodityId, System.Guid userId, System.Guid channelId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodityCollection(commodityId, userId, channelId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppCollection(System.Guid appId, System.Guid userId, System.Guid channelId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveAppCollection(appId, userId, channelId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCollectionComs(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCollectionComs(search);

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
        public int GetCollectionComsCount(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCollectionComsCount(search);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> GetCollectionApps(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCollectionApps(search);

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
        public int GetCollectionAppsCount(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCollectionAppsCount(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollections(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCollections(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckAppCollected(System.Guid channelId, System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckAppCollected(channelId, userId, appId);

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
