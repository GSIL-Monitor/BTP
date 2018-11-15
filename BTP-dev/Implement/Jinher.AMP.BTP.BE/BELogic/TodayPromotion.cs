

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
namespace Jinher.AMP.BTP.BE
{
    public partial class TodayPromotion
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        #region Cache 框架暂时不支持取整个hash的方法，所以改为单个取hash

        /// <summary>
        /// 刷新缓存
        /// </summary>
        public void RefreshCache(EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    ResfreshCacheAdd();
                    break;
                case EntityState.Deleted:
                    RefreshCacheRemove();
                    break;
                case EntityState.Modified:
                    RefreshCacheUpdate();
                    break;
            }
        }
        /// <summary>
        /// 向商品详情缓存中增加Cache
        /// </summary>
        private void ResfreshCacheAdd()
        {
            if (StartTime >= DateTime.Today.AddDays(1) || EndTime <= DateTime.Today || StartTime >= EndTime) return;
            var list =
                GlobalCacheWrapper.GetData("G_CommodityPromotion:" + AppId, CommodityId.ToString(), CacheTypeEnum.redisSS,
                                           "BTPCache") as List<TodayPromotionDTO> ?? new List<TodayPromotionDTO>();
            list.Add(ToEntityData());
            GlobalCacheWrapper.Add("G_CommodityPromotion:" + AppId, CommodityId.ToString(), list, CacheTypeEnum.redisSS, "BTPCache");
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        private void RefreshCacheRemove()
        {
            var list =
                GlobalCacheWrapper.GetData("G_CommodityPromotion:" + AppId, CommodityId.ToString(), CacheTypeEnum.redisSS,
                                           "BTPCache") as List<TodayPromotionDTO>;
            if (list == null || !list.Any())
                return;
            list.RemoveAll(c => c.Id == Id);
            GlobalCacheWrapper.Add("G_CommodityPromotion:" + AppId, CommodityId.ToString(), list, CacheTypeEnum.redisSS, "BTPCache");
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        private void RefreshCacheUpdate()
        {
            if (StartTime >= DateTime.Today.AddDays(1) || EndTime <= DateTime.Today || StartTime >= EndTime) return;
            var list =
                GlobalCacheWrapper.GetData("G_CommodityPromotion:" + AppId, CommodityId.ToString(), CacheTypeEnum.redisSS,
                                           "BTPCache") as List<TodayPromotionDTO> ?? new List<TodayPromotionDTO>();
            list.RemoveAll(c => c.Id == Id);
            list.Add(ToEntityData());
            GlobalCacheWrapper.Add("G_CommodityPromotion:" + AppId, CommodityId.ToString(), list, CacheTypeEnum.redisSS, "BTPCache");
        }
        /// <summary>
        /// 获取指定app的所有今日优惠信息缓存
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [Obsolete("框架暂时不支持取整个hash的方法，所以改为单个取hash", true)]
        public static Dictionary<Guid, List<TodayPromotionDTO>> GetDictDTOByAppIdFromCache(Guid appId)
        {
            var dict = GlobalCacheWrapper.GetAllData<List<TodayPromotionDTO>>("G_CommodityPromotion:" + appId, CacheTypeEnum.redisSS, "BTPCache");
            Dictionary<Guid, List<TodayPromotionDTO>> result = new Dictionary<Guid, List<TodayPromotionDTO>>();
            if (dict != null && dict.Any())
            {
                foreach (var kv in dict)
                {
                    Guid commodityId;
                    if (Guid.TryParse(kv.Key, out commodityId))
                    {
                        result.Add(commodityId, kv.Value);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定商品的所有今日优惠信息（部分代码被注释，现在取db数据，临时方案）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public static List<TodayPromotionDTO> GetListDTOByCommodityIdFromCache(Guid appId, Guid commodityId)
        {
            var now = DateTime.Now;
            var result1 = TodayPromotion.ObjectSet().Where(c => c.AppId == appId && c.CommodityId == commodityId && c.StartTime < now && c.EndTime >= now).ToList();
            if (result1.Any())
            {
                return result1.Select(c => c.ToEntityData()).ToList();
            }
            return null;
            List<TodayPromotionDTO> result = GlobalCacheWrapper.GetData("G_CommodityPromotion:" + appId, commodityId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as List<TodayPromotionDTO>;
            if (result == null || !result.Any())
            {
                result = new List<TodayPromotionDTO>();
                var dbEntities = ObjectSet().Where(c => c.AppId == appId && c.CommodityId == commodityId).ToList();
                if (dbEntities.Any())
                {
                    result.AddRange(dbEntities.Select(todayPromotion => todayPromotion.ToEntityData()));
                }
                return result;
            }
            return result;
        }

        /// <summary>
        /// 添加指定app的优惠活动缓存
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="list"></param>
        public static void AddAppDTOCache(Guid appId, List<TodayPromotionDTO> list)
        {
            if (appId != Guid.Empty && list != null && list.Any())
            {
                var dict = list.GroupBy(
                    a => a.CommodityId,
                    (key, group) => new { comId = key, proList = group }).ToDictionary(a => a.comId, a => a.proList);

                foreach (var kv in dict)
                {
                    //LogHelper.Error(string.Format("今日优惠缓存,appId:{0}  ,key:{1}    , value:{2}", appId.ToString(), kv.Key.ToString(), JsonHelper.JsonSerializer(kv.Value.ToList())));
                    GlobalCacheWrapper.Add("G_CommodityPromotion:" + appId, kv.Key.ToString(), kv.Value.ToList(), CacheTypeEnum.redisSS, "BTPCache");
                }
            }
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="list">列表</param>
        public static void RemoveCaches(Guid appId, List<TodayPromotionDTO> list)
        {
            if (list != null && list.Any())
            {
                Dictionary<Guid, List<TodayPromotionDTO>> dict = new Dictionary<Guid, List<TodayPromotionDTO>>();
                foreach (var todayPromotionDto in list)
                {
                    if (!dict.ContainsKey(todayPromotionDto.CommodityId))
                        dict.Add(todayPromotionDto.CommodityId, new List<TodayPromotionDTO>());
                    dict[todayPromotionDto.CommodityId].Add(todayPromotionDto);
                }
                foreach (var kv in dict)
                {
                    var todayPromotions =
                        GlobalCacheWrapper.GetData("G_CommodityPromotion:" + appId, kv.Key.ToString(),
                                                   CacheTypeEnum.redisSS,
                                                   "BTPCache") as List<TodayPromotionDTO>;
                    if (todayPromotions == null || !todayPromotions.Any())
                        continue;
                    todayPromotions.RemoveAll(c => dict[kv.Key].Select(m => m.Id).Contains(c.Id));
                    GlobalCacheWrapper.Add("G_CommodityPromotion:" + appId, kv.Key.ToString(), todayPromotions, CacheTypeEnum.redisSS, "BTPCache");
                }
            }
        }
        /// <summary>
        /// 重置所有今日优惠活动缓存（每日计算时使用）
        /// </summary>
        /// <param name="dict"></param>
        public static void ResetTodayPromotionCache(Dictionary<Guid, List<TodayPromotionDTO>> dict)
        {
            var keys = GlobalCacheWrapper.GetAllKey(CacheTypeEnum.redisSS, "BTPCache");
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    if (key.StartsWith("G_CommodityPromotion:"))
                        GlobalCacheWrapper.RemoveCache(key, "BTPCache", CacheTypeEnum.redisSS);
                }
            }
            if (dict != null && dict.Any())
            {
                foreach (var app in dict)
                {

                    AddAppDTOCache(app.Key, app.Value);

                }
            }
        }
        #endregion
        /// <summary>
        /// 获取商品当前有效活动（包括预售）
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public static TodayPromotion GetCurrentPromotionWithPresell(Guid commodityId)
        {
            return ObjectSet().FirstOrDefault(c => c.CommodityId == commodityId &&
                                (c.StartTime <= DateTime.Now ||
                                c.PresellStartTime <= DateTime.Now) &&
                                c.EndTime >= DateTime.Now && c.PromotionType != 3);
        }
        /// <summary>
        /// 批量获取商品当前有效活动（包括预售）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <returns></returns>
        public static List<TodayPromotion> GetCurrentPromotionsWithPresell(List<Guid> commodityIds)
        {
            if (commodityIds == null || !commodityIds.Any())
                return new List<TodayPromotion>();
            return ObjectSet().Where(c => commodityIds.Contains(c.CommodityId) &&
                                (c.StartTime <= DateTime.Now ||
                                c.PresellStartTime <= DateTime.Now) &&
                                c.EndTime >= DateTime.Now && c.PromotionType != 3).ToList();
        }
        /// <summary>
        /// 获取商品当前有效活动
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public static TodayPromotion GetCurrentPromotion(Guid commodityId)
        {
            return ObjectSet().FirstOrDefault(c => c.CommodityId == commodityId &&
                                c.StartTime <= DateTime.Now &&
                                c.EndTime >= DateTime.Now && c.PromotionType != 3);
        }
        /// <summary>
        /// 批量获取商品当前有效活动
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <returns></returns>
        public static List<TodayPromotion> GetCurrentPromotions(List<Guid> commodityIds)
        {
            if (commodityIds == null || !commodityIds.Any())
                return new List<TodayPromotion>();
            return ObjectSet().Where(c => commodityIds.Contains(c.CommodityId) &&
                                c.StartTime <= DateTime.Now   &&
                                c.EndTime >= DateTime.Now && c.PromotionType != 3).ToList();
        }
    }
}



