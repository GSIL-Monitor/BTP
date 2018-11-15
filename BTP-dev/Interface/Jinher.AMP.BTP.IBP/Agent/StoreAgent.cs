
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/3/19 18:07:30
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

    public class StoreAgent : BaseBpAgent<IStore>, IStore
    {
        public ResultDTO AddStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.AddStore(storeDTO);

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
            return result;
        }
        public ResultDTO DelStore(System.Guid id)
        {

            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DelStore(id);

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
            return result;
        }


        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAppStore(Guid appId)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.StoreDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppStore(appId);

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


        public Jinher.AMP.BTP.Deploy.StoreDTO GetStoreDTO(System.Guid id, Guid appid)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.StoreDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetStoreDTO(id, appid);

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
        public ResultDTO UpdateStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.UpdateStore(storeDTO);

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
            return result;
        }
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStore(System.Guid sellerId, int pageSize, int pageIndex, out int rowCount)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.StoreDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllStore(sellerId, pageSize, pageIndex, out rowCount);

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

        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStoreByWhere(Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string provice, string city, string district)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.StoreDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllStoreByWhere(sellerId, pageSize, pageIndex, out rowCount, storeName, provice, city, district);

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
