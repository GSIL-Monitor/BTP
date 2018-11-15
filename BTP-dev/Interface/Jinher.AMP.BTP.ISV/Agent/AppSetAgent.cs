
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/1/8 9:01:58
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

    public class AppSetAgent : BaseBpAgent<IAppSet>, IAppSet
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.QryCommodityDTO qryCommodityDTO)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;
            int tmp = 0;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityList(qryCommodityDTO);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategory(appId);

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
        /// <summary>
        /// 按关键字获取商品列表
        /// </summary>
        /// <param name="want">关键字</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetWantCommodity(string want, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetWantCommodity(want, pageIndex, pageSize);

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
        /// <summary>
        /// 厂家直营app查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public AppSetAppGridDTO GetAppSet(AppSetSearchDTO search)
        {
            //定义返回值
            AppSetAppGridDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppSet(search);

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

        public List<AppSetAppDTO> GetCategoryAppList(AppSetSearchDTO search)
        {
            //定义返回值
            List<AppSetAppDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryAppList(search);

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

        public UserInfoCountDTO GetUserInfoCount(Guid userId, Guid esAppId)
        {
            //定义返回值
            UserInfoCountDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserInfoCount(userId,esAppId);

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

        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns>结果</returns>        
        public ResultDTO RemoveAppInZPHCache()
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RemoveAppInZPHCache();

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
        /// <summary>
        /// 根据独立电商是否属于平台获取商品列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListV2(search);

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
        /// <summary>
        /// 浏览过的店铺（20个）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> GetBrowseAppInfo(Guid userId, Guid appId)
        {

            //定义返回值
            List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrowseAppInfo(userId, appId);

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

        /// <summary>
        /// 分页获取浏览商品记录
        /// </summary>
        /// <param name="par"></param>
        public List<BTP.Deploy.CustomDTO.CommodityListCDTO> GetBrowseCommdity(BrowseParameter par)
        {
            //定义返回值
            List<BTP.Deploy.CustomDTO.CommodityListCDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetBrowseCommdity(par);

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
        /// <summary>
        /// 删除商品浏览记录
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <param name="CommdityId"></param>
        /// <returns></returns>

        public Deploy.CustomDTO.ResultDTO DeletebrowseCommdity(Guid AppId, Guid UserId, Guid CommdityId)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DeletebrowseCommdity(AppId,UserId,CommdityId);

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
