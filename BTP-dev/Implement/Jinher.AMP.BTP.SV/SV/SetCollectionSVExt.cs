using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 正品会收藏
    /// </summary>
    public partial class SetCollectionSV : BaseSv, ISetCollection
    {
        /// <summary>
        /// 添加商品收藏
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId">渠道Id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityCollectionExt(System.Guid commodityId, System.Guid userId, System.Guid channelId)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                List<Commodity> needRefreshCacheCommodityList = new List<Commodity>();
                int collections = SetCollection.ObjectSet().Count(n => n.ColType == 1 && n.UserId == userId && n.ColKey == commodityId && n.ChannelId == channelId);
                if (collections == 0)
                {


                    SetCollection collection = SetCollection.CreateSetCollection();
                    collection.ChannelId = channelId;
                    collection.ColKey = commodityId;
                    collection.SubId = userId;
                    collection.ColType = 1;
                    collection.UserId = userId;
                    contextSession.SaveObject(collection);
                    Commodity com = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (com != null)
                    {
                        com.EntityState = System.Data.EntityState.Modified;
                        com.TotalCollection += 1;
                        contextSession.SaveObject(com);
                        needRefreshCacheCommodityList.Add(com);
                    }
                    contextSession.SaveChanges();

                    if (needRefreshCacheCommodityList.Any())
                        needRefreshCacheCommodityList.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                else
                {
                    result = new ResultDTO { ResultCode = 0, Message = "已有收藏" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加收藏服务异常。commodityId：{0}，userId：{1}，channelId：{2}，", commodityId, userId, channelId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return result;
        }

        /// <summary>
        /// 店铺收藏
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppCollectionExt(System.Guid appId, System.Guid userId, System.Guid channelId)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                int collections = SetCollection.ObjectSet().Count(n => n.ColType == 2 && n.UserId == userId && n.ColKey == appId && n.ChannelId == channelId);
                if (collections == 0)
                {
                    SetCollection collection = SetCollection.CreateSetCollection();
                    collection.ChannelId = channelId;
                    collection.ColKey = appId;
                    collection.SubTime = DateTime.Now;
                    collection.SubId = userId;
                    collection.ColType = 2;
                    collection.UserId = userId;
                    contextSession.SaveObject(collection);

                    contextSession.SaveChanges();
                }
                else
                {
                    result = new ResultDTO { ResultCode = 0, Message = "已有收藏" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加收藏服务异常。appId：{0}，userId：{1}，channelId：{2}，", appId, userId, channelId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return result;
        }

        /// <summary>
        /// 根据用户ID查询收藏商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCollectionComsExt(SetCollectionSearchDTO search)
        {
            DateTime now = DateTime.Now;
            List<CommodityListCDTO> result = new List<CommodityListCDTO>();
            try
            {
                if (search == null || search.UserId == Guid.Empty || search.ChannelId == Guid.Empty || search.PageIndex < 1 || search.PageSize < 1)
                    return result;

                var commodityList = (from setCollection in SetCollection.ObjectSet()
                                     join commodity in Commodity.ObjectSet() on setCollection.ColKey equals commodity.Id
                                     where setCollection.ColType == 1 && setCollection.UserId == search.UserId && commodity.IsDel == false && commodity.State == 0 && commodity.CommodityType == 0 && setCollection.ChannelId == search.ChannelId
                                     orderby setCollection.SubTime descending
                                     select new CommodityListCDTO
                                     {
                                         Id = commodity.Id,
                                         Pic = commodity.PicturesPath,
                                         Price = commodity.Price,
                                         State = commodity.State,
                                         Stock = commodity.Stock,
                                         Name = commodity.Name,
                                         MarketPrice = commodity.MarketPrice,
                                         AppId = commodity.AppId,
                                         ComAttrType = (commodity.ComAttribute == "[]" || commodity.ComAttribute == null) ? 1 : 3
                                     }).Skip((search.PageIndex - 1) * search.PageSize)
                                  .Take(search.PageSize).ToList();
                if (!commodityList.Any())
                    return result;
                var appIds = commodityList.Select(c => c.AppId).Distinct().ToList();

                #region 众筹
                if (CustomConfig.CrowdfundingFlag)
                {
                    var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();
                    if (cfAppIds.Any())
                    {
                        foreach (var commodityListCdto in commodityList)
                        {
                            if (cfAppIds.Any(c => c == commodityListCdto.AppId))
                                commodityListCdto.IsActiveCrowdfunding = true;
                        }
                    }
                }
                #endregion

                List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();
                var promotionDic = (from p in PromotionItems.ObjectSet()
                                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                    where commodityIds.Contains(p.CommodityId) && !pro.IsDel && pro.IsEnable && (pro.StartTime <= now || pro.PresellStartTime <= now) && pro.EndTime >= now
                                    select new
                                    {
                                        ComId = p.CommodityId,
                                        Intensity = (decimal)p.Intensity,
                                        DiscountPrice = (decimal)p.DiscountPrice,
                                        StartTime = pro.StartTime,
                                        EndTime = pro.EndTime,
                                        PresellStartTime = pro.PresellStartTime,
                                        PresellEndTime = "",
                                        LimitBuyEach = p.LimitBuyEach,
                                        LimitBuyTotal = p.LimitBuyTotal,
                                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                        PromotionType = pro.PromotionType
                                    }
                                ).Distinct();

                foreach (var commodity in commodityList)
                {

                    var promotion = promotionDic.FirstOrDefault(c => c.ComId == commodity.Id);
                    if (promotion != null)
                    {
                        commodity.LimitBuyEach = promotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = promotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal ?? 0;
                        if (promotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                            commodity.Intensity = 10;
                            continue;

                        }

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = promotion.Intensity;
                        commodity.PromotionType = promotion.PromotionType;
                    }
                    else
                    {
                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                        commodity.PromotionType = 9999;
                    }
                }
                return commodityList;
            }
            catch (Exception e)
            {
                LogHelper.Error("SetCollectionSV.GetCollectionComsExt,获取收藏商品列表查询错误" + e);
                return new List<CommodityListCDTO>();
            }
        }

        /// <summary>
        /// 根据用户ID查询收藏商品数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionComsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int GetCollectionComsCountExt(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            try
            {
                if (search == null || search.UserId == Guid.Empty || search.ChannelId == Guid.Empty) {

                    LogHelper.Debug("SetCollectionSV.GetCollectionComsCountExt,获取收藏商品数量参数不完整");
                    return 0;
                }
                var commodityCount = (from setCollection in SetCollection.ObjectSet()
                                     join commodity in Commodity.ObjectSet() on setCollection.ColKey equals commodity.Id
                                     where setCollection.ColType == 1 && setCollection.UserId == search.UserId && commodity.IsDel == false && commodity.State == 0 && commodity.CommodityType == 0 && setCollection.ChannelId == search.ChannelId
                                     select new CommodityListCDTO
                                     {
                                         Id = commodity.Id,
                                         Pic = commodity.PicturesPath,
                                         Price = commodity.Price,
                                         State = commodity.State,
                                         Stock = commodity.Stock,
                                         Name = commodity.Name,
                                         MarketPrice = commodity.MarketPrice,
                                         AppId = commodity.AppId,
                                         ComAttrType = (commodity.ComAttribute == "[]" || commodity.ComAttribute == null) ? 1 : 3
                                     }).Count();
                return commodityCount;
            }
            catch (Exception e)
            {
                LogHelper.Error("SetCollectionSV.GetCollectionComsCountExt,获取收藏商品数量异常" + e);
                return 0;
            }

        }

        /// <summary>
        /// 根据用户ID查询收藏商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> GetCollectionAppsExt(SetCollectionSearchDTO search)
        {
            DateTime now = DateTime.Now;
            List<AppSetAppDTO> result = new List<AppSetAppDTO>();
            if (search == null || search.UserId == Guid.Empty || search.ChannelId == Guid.Empty || search.PageIndex < 1 || search.PageSize < 1)
                return result;
            try
            {

                var appIds =
                    SetCollection.ObjectSet()
                                 .Where(c => c.ColType == 2 && c.UserId == search.UserId && c.ChannelId == search.ChannelId).OrderByDescending(c => c.SubTime)
                                 .Select(c => c.ColKey).Skip((search.PageIndex - 1) * search.PageSize)
                                  .Take(search.PageSize)
                                 .ToList();
                if (appIds.Any())
                {
                    var applist = APPSV.GetAppListByIds(appIds);
                    if (applist != null && applist.Any())
                    {
                        foreach (var appIdNameIconDTO in applist)
                        {
                            result.Add(new AppSetAppDTO
                                {
                                    AppId = appIdNameIconDTO.AppId,
                                    AppIcon = appIdNameIconDTO.AppIcon,
                                    AppName = appIdNameIconDTO.AppName,
                                    AppCreateOn = appIdNameIconDTO.CreateDate > DateTime.MinValue ? appIdNameIconDTO.CreateDate : new DateTime(1970, 1, 1)
                                });
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SetCollectionSV.GetCollectionAppsExt,获取收藏商品列表查询错误search：{0}，", JsonHelper.JsonSerializer(search)), ex);
                return new List<AppSetAppDTO>();
            }
        }

        /// <summary>
        /// 根据用户ID查询收藏店铺数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionAppsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int GetCollectionAppsCountExt(Jinher.AMP.BTP.Deploy.CustomDTO.SetCollectionSearchDTO search)
        {
            if (search == null || search.UserId == Guid.Empty || search.ChannelId == Guid.Empty)
            {
                LogHelper.Debug(string.Format("SetCollectionSV.GetCollectionAppsCountExt,获取收藏店铺数量参数不全，search：{0}，", JsonHelper.JsonSerializer(search)));
                return 0;
            }
            try
            {
                var appIds =
                    SetCollection.ObjectSet()
                                 .Where(c => c.ColType == 2 && c.UserId == search.UserId && c.ChannelId == search.ChannelId).OrderByDescending(c => c.SubTime)
                                 .Select(c => c.ColKey)
                                 .ToList();
                if (appIds.Any())
                {
                    var applist = APPSV.GetAppListByIds(appIds);
                    if (applist != null && applist.Any())
                    {
                        return applist.Count;
                    }
                }
                LogHelper.Debug(string.Format("SetCollectionSV.GetCollectionAppsCountExt,获取收藏店铺数量位未查到appids，search：{0}，", JsonHelper.JsonSerializer(search)));
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SetCollectionSV.GetCollectionAppsCountExt,获取收藏店铺数量异常，search：{0}，", JsonHelper.JsonSerializer(search)), ex);
                return 0;
            }

        }

        /// <summary>
        /// 删除正品会收藏
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollectionsExt(SetCollectionSearchDTO search)
        {
            if (search == null || search.UserId == Guid.Empty || search.ColKeyList == null || !search.ColKeyList.Any())
                return new ResultDTO { ResultCode = -1, Message = "参数非法" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<Commodity> needRefreshCacheCommodityList = new List<Commodity>();

                var cols = SetCollection.ObjectSet()
                    .Where(c => c.UserId == search.UserId && c.ColType == search.ColType && search.ColKeyList.Contains(c.ColKey) && c.ChannelId == search.ChannelId)
                    .Distinct();
                if (cols.Any())
                {
                    foreach (var collection in cols)
                    {
                        //删除收藏
                        collection.EntityState = System.Data.EntityState.Deleted;
                    }

                    //商品收藏,回退商品收藏数量
                    if (search.ColType == 1)
                    {
                        var comIds = cols.Select(c => c.ColKey).ToList();
                        var coms = Commodity.ObjectSet().Where(n => comIds.Contains(n.Id) && n.CommodityType == 0).ToList();
                        foreach (var commodity in coms)
                        {
                            commodity.TotalCollection -= 1;
                            commodity.EntityState = EntityState.Modified;
                            needRefreshCacheCommodityList.Add(commodity);
                        }
                    }
                    contextSession.SaveChanges();
                    if (needRefreshCacheCommodityList.Any())
                        needRefreshCacheCommodityList.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("删除收藏服务异常。search：{0}，", JsonHelper.JsonSerializer(search)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 校验是否收藏店铺
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckAppCollectedExt(Guid channelId, Guid userId, Guid appId)
        {
            try
            {
                var cnt = SetCollection.ObjectSet().Count(c => c.UserId == userId && c.ColType == 2 && c.ColKey == appId && c.ChannelId == channelId);
                if (cnt <= 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("SetCollectionSV.CheckAppCollectedExt服务异常。channelId：{0}，userId：{1}，appId：{2}，", channelId, userId, appId), ex);
                return new ResultDTO { ResultCode = -1, Message = "false" };
            }
            return new ResultDTO { ResultCode = 0, Message = "true" };
        }
    }
}
