
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/11 17:24:56
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

    public class PromotionAgent : BaseBpAgent<IPromotion>, IPromotion
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.PromotionHotSDTO GetNewPromotion(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.PromotionHotSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetNewPromotion(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetYJDianDi(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetYJDianDi(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetPromotionItems(System.Guid promotionId, System.Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPromotionItems(promotionId, appId, pageIndex, pageSize);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO> GetAllPromotionItems()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllPromotionItems();

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
        public System.Collections.Generic.Dictionary<System.Guid, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO>> GetAppPromotions()
        {
            //定义返回值
            System.Collections.Generic.Dictionary<System.Guid, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppPromotions();

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
        public void AddHotCommodity()
        {

            try
            {
                //调用代理方法
                base.Channel.AddHotCommodity();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreCacheDTO> GetAllStores()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreCacheDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllStores();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttributeCacheDTO> GetAllCommAttributes()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttributeCacheDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommAttributes();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCacheDTO> GetAllCateGories()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCacheDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCateGories();

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
        public void PromotionPush()
        {

            try
            {
                //调用代理方法
                base.Channel.PromotionPush();

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
        public void UpdateUserInfo()
        {

            try
            {
                //调用代理方法
                base.Channel.UpdateUserInfo();

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
        public void PromotionPushIUS()
        {

            try
            {
                //调用代理方法
                base.Channel.PromotionPushIUS();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO GetSecKillPromotion(System.Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSecKillPromotion(commodityId);

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
        public bool CheckSecKillBuy(System.Guid commodityId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckSecKillBuy(commodityId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddOutsidePromotion(discountsDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelOutsidePromotion(System.Guid outsideId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelOutsidePromotion(outsideId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateOutsidePromotion(discountsDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetExpireSeconds(System.Guid appId, long seconds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetExpireSeconds(appId, seconds);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CommodityDataAndRedisDataSynchronization()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CommodityDataAndRedisDataSynchronization();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO PromotionRedis(System.Guid guid)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.PromotionRedis(guid);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO> GeTodayPromotions(System.Collections.Generic.List<System.Guid?> outsideId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GeTodayPromotions(outsideId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionSurplusLimitBuyTotalDto> GetSurplusLimitBuyTotal(System.Collections.Generic.List<System.Guid> outsideId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionSurplusLimitBuyTotalDto> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSurplusLimitBuyTotal(outsideId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryList(System.Guid AppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryList(AppId);

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
        /// 获取应用的一级商品分类
        /// <para>
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1(Guid appId)
        {

            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryL1(appId);

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
