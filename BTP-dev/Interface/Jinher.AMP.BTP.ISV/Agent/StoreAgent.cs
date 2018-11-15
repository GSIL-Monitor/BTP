
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/1/6 16:25:15
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

    public class StoreAgent : BaseBpAgent<IStore>, IStore
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStore(System.Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetStore(appId, pageIndex, pageSize);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetStoreByProvince(string province, System.Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetStoreByProvince(province, appId, pageIndex, pageSize);

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
        public System.Collections.Generic.List<string> GetProvince(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetProvince(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStoreByLocation(Jinher.AMP.BTP.Deploy.CustomDTO.StoreLocationParam slp)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetStoreByLocation(slp);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetCateringPlatformStore(Jinher.AMP.BTP.Deploy.CustomDTO.StoreLocationParam param)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCateringPlatformStore(param);

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
        public void InitMongoFromSql()
        {

            try
            {
                //调用代理方法
                base.Channel.InitMongoFromSql();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetOnlyStoreInApp(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOnlyStoreInApp(appId);

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
        public Jinher.AMP.ZPH.Deploy.CustomDTO.AppPavilionInfoIICDTO GetAppPavilionInfo(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam param)
        {
            //定义返回值
            Jinher.AMP.ZPH.Deploy.CustomDTO.AppPavilionInfoIICDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppPavilionInfo(param);

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

        public ResultDTO<StoreSResultDTO> GetAppStores(StoreLocationParam search)
        {
            //定义返回值
            ResultDTO<StoreSResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppStores(search);

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

        public ResultDTO<List<StoreSDTO>> GetAppStoresByLocation(StoreLocationParam search)
        {
            //定义返回值
            ResultDTO<List<StoreSDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppStoresByLocation(search);

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
