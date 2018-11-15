using System;
using System.Collections.Generic;
using System.Web;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS.Cache
{
    /// <summary>
    /// ZPH Cache Helper
    /// </summary>
    public class ZPHCache
    {
        /// <summary>
        /// 获取店铺720云景地址
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public string GetCloudViewUrl(Guid appId)
        {
            var key = "A_CloudViewUrl:AI:" + appId.ToString();
            var data = HttpRuntime.Cache.Get(key) as string;
            if (data == null)
            {
                data = ZPHSV.GetCloudViewUrl(appId);
                if (string.IsNullOrEmpty(data))
                {
                    data = string.Empty;
                }
                HttpRuntime.Cache.Add(key, data, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            return data;
        }

        public string GetCreateOrderTip(Guid appId)
        {
            var key = "A_CreateOrderTip:" + appId.ToString();
            var data = HttpRuntime.Cache.Get(key) as string;
            if (data == null)
            {
                data = ZPHSV.GetCreateOrderTip(appId);
                if (string.IsNullOrEmpty(data))
                {
                    data = string.Empty;
                }
                HttpRuntime.Cache.Add(key, data, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                LogHelper.Info("ZPHCache.GetCreateOrderTip from db......");
            }
            else
            {
                LogHelper.Info("ZPHCache.GetCreateOrderTip from cache......");
            }
            return data;
        }

        public ZPH.Deploy.CustomDTO.SetMealActivityCDTO GetSetMealActivitysById(Guid activityId)
        {
            try
            {
                var key = "A_SetMealActivitysById:" + activityId.ToString();
                var data = RedisHelper.Get<ZPH.Deploy.CustomDTO.SetMealActivityCDTO>(key);
                if (data == null)
                {
                    data = ZPHSV.Instance.GetSetMealActivitysById(activityId);
                    if (data == null)
                    {
                        data = new ZPH.Deploy.CustomDTO.SetMealActivityCDTO() { Id = Guid.Empty };
                    }
                    RedisHelper.Set(key, data, TimeSpan.FromMinutes(10));
                }
                else
                {
                    LogHelper.Info("ZPHCache.GetSetMealActivitysById from Cache......");
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHCache.GetSetMealActivitysById Excepiton", ex);
                return ZPHSV.Instance.GetSetMealActivitysById(activityId);
            }
        }

        public List<ZPH.Deploy.CustomDTO.SkuActivityCDTO> GetSkuActivityList(Guid activityId)
        {
            try
            {
                var key = "A_SkuActivity:" + activityId.ToString();
                var data = RedisHelper.Get<List<ZPH.Deploy.CustomDTO.SkuActivityCDTO>>(key);
                if (data == null)
                {
                    data = ZPHSV.Instance.GetSkuActivityList(activityId);
                    if (data == null)
                    {
                        data = new List<ZPH.Deploy.CustomDTO.SkuActivityCDTO>();
                    }
                    RedisHelper.Set(key, data, TimeSpan.FromMinutes(10));
                }
                else
                {
                    LogHelper.Info("ZPHCache.GetSkuActivityList from Cache......");
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHCache.GetSkuActivityList Excepiton", ex);
                return ZPHSV.Instance.GetSkuActivityList(activityId);
            }
        }

        /// <summary>
        /// 根据商品ID获取商品参与的优惠套装
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO> GetSetMealActivitysByCommodityId(Guid commodityId, Guid appId)
        {
            var key = "SetMealActivitys:CI:" + commodityId.ToString();
            var data = RedisHelper.Get<List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO>>(key);
            if (data == null)
            {
                data = ZPHSV.Instance.GetSetMealActivitysByCommodityId(commodityId, appId, true);
                if (data == null)
                {
                    data = new List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO>();
                }
                RedisHelper.Set(key, data, TimeSpan.FromMinutes(20));
                LogHelper.Info("ZPHCache.GetSetMealActivitysByCommodityId from DB......");
            }
            return data;
        }
    }
}