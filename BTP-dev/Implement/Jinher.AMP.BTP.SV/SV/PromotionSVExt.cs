using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using CommodityListCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 促销接口类
    /// </summary>
    public partial class PromotionSV : BaseSv, IPromotion
    {
        private const string YJDianDiKey = "YJDianDi";

        /// <summary>
        /// 获取最新促销(不返回外部活动)D:\01-AMP\开发库\08-Program\Code\Biz\BTP-bugfix\UI\Jinher.AMP.BTP.UI\Controllers\LoginController.cs
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.PromotionHotSDTO GetNewPromotionExt(System.Guid appId)
        {
            //System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
            //string cacheKey = "PromotionList" + appId.ToString();
            try
            {
                //PromotionHotSDTO cacheData = cache.Get(cacheKey) as PromotionHotSDTO;
                //if (cacheData != null)
                //{
                //    if (cacheData.commoditySDTO != null && cacheData.commoditySDTO.Count > 0)
                //        return cacheData;
                //    if (cacheData.promotionSDTO != null && cacheData.promotionSDTO.Count > 0)
                //        return cacheData;
                //}

                PromotionHotSDTO prohots = new PromotionHotSDTO();
                DateTime now = DateTime.Now;

                //查询在促销时间内商品
                var promotion = Promotion.ObjectSet()
                    .Where(n => n.AppId == appId && n.EndTime > now && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                    //.OrderBy(n => n.EndTime)
                    .OrderBy(n => n.StartTime)
                    .Select(n => new PromotionSDTO
                    {
                        EndTime = n.EndTime,
                        StartTime = n.StartTime,
                        PromotionId = n.Id,
                        Name = n.Name,
                        PicPath = n.PicturesPath,
                        Intensity = n.Intensity,
                        IsEnable = n.IsEnable,
                        CurrentTime = now,
                        DiscountPrice = n.DiscountPrice,

                    }).Take(5).ToList();
                //取最小的优惠价格
                foreach (var pro in promotion)
                {
                    if (pro.Intensity == 10.00M)
                    {
                        pro.DiscountPrice = (from p in PromotionItems.ObjectSet()
                                             where p.PromotionId == pro.PromotionId
                                             select p.DiscountPrice).Min();
                    }
                }

                //有促销商品
                if (promotion.Count > 0)
                {
                    prohots.promotionSDTO = promotion;
                }
                //如果无促销商品，则从商品表中根据销量、收藏数、提交时间倒序显示5条
                else
                {
                    var commoditys = HotCommodity.ObjectSet()
                        .Where(n => n.AppId == appId)
                        .OrderByDescending(n => n.Salesvolume)
                        .ThenByDescending(n => n.TotalCollection)
                        .Select(n => new CommodityListCDTO
                        {
                            Name = n.Name,
                            Id = n.Id,
                            Pic = n.PicturesPath,
                            Price = n.Price,
                            State = n.State,
                            Stock = n.Stock,
                            Intensity = 10,
                            DiscountPrice = -1
                        }).Take(5).ToList();

                    if (commoditys == null || commoditys.Count == 0)
                    {
                        var hotCom = Commodity.ObjectSet()
                         .Where(n => n.AppId == appId && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                         .OrderByDescending(n => n.Salesvolume)
                         .ThenByDescending(n => n.TotalCollection)
                         .ThenByDescending(n => n.SubTime)
                         .Take(5).ToList();

                        ContextSession contextSession = ContextFactory.CurrentThreadContext;
                        foreach (Commodity hot in hotCom)
                        {
                            HotCommodity model = new HotCommodity
                            {
                                Name = hot.Name,
                                Id = hot.Id,
                                CommodityId = hot.Id,
                                PicturesPath = hot.PicturesPath,
                                Price = hot.Price,
                                TotalReview = hot.TotalReview,
                                TotalCollection = hot.TotalCollection,
                                State = hot.State,
                                Stock = hot.Stock,
                                Salesvolume = hot.Salesvolume,
                                AppId = hot.AppId
                            };
                            commoditys.Add(new CommodityListCDTO
                            {
                                Name = hot.Name,
                                Id = hot.Id,
                                Pic = hot.PicturesPath,
                                Price = hot.Price,
                                State = hot.State,
                                Stock = hot.Stock,
                                Intensity = 10,
                                DiscountPrice = -1
                            });
                            model.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(model);
                        }
                        contextSession.SaveChanges();
                    }

                    prohots.commoditySDTO = commoditys;
                }

                //if (prohots != null)//设置1.5秒的缓存，可缓解1.5秒内的多并发压力
                //{

                //    cache.Add(cacheKey,
                //        prohots,
                //        null,
                //        DateTime.Now.AddSeconds(1.5),
                //        System.Web.Caching.Cache.NoSlidingExpiration,
                //        System.Web.Caching.CacheItemPriority.Default,
                //        null);
                //}
                return prohots;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取最新促销服务器异常。appId：{0}", appId), ex);

                return null;
            }
        }


        /// <summary>
        /// 易捷点滴接口
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetYJDianDiExt(System.Guid appId)
        {
            try
            {
                //List<CommodityListCDTO> commodityCDTO = new List<CommodityListCDTO>();
                //DateTime now = DateTime.Now;

                ////获取热销商品
                //commodityCDTO = HotCommodity.ObjectSet()
                //    .Where(n => n.AppId == appId)
                //    .OrderByDescending(n => n.Salesvolume)
                //    .ThenByDescending(n => n.TotalCollection)
                //    .Select(n => new CommodityListCDTO
                //    {
                //        Name = n.Name,
                //        Id = n.CommodityId,
                //        Pic = n.PicturesPath,
                //        Price = n.Price,
                //        State = n.State,
                //        Stock = n.Stock,
                //        Intensity = 10,
                //        DiscountPrice = -1
                //    }).Take(4).ToList();

                ////如果没有数据商品表里取出5个放到热销表中 
                //if (commodityCDTO == null || commodityCDTO.Count == 0)
                //{
                //    var appIds = MallApply.GetTGQuery(appId).Select(_ => _.AppId).ToList();
                //    if (!appIds.Contains(appId)) appIds.Add(appId);
                //    var hotCom = Commodity.ObjectSet()
                //     .Where(n => appIds.Contains(n.AppId) && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                //     .OrderByDescending(n => n.Salesvolume)
                //     .ThenByDescending(n => n.TotalCollection)
                //     .ThenByDescending(n => n.SubTime)
                //     .Take(5).ToList();

                //    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //    foreach (Commodity hot in hotCom)
                //    {
                //        HotCommodity model = new HotCommodity
                //        {
                //            Name = hot.Name,
                //            Id = Guid.NewGuid(),
                //            CommodityId = hot.Id,
                //            PicturesPath = hot.PicturesPath,
                //            Price = hot.Price,
                //            TotalReview = hot.TotalReview,
                //            TotalCollection = hot.TotalCollection,
                //            State = hot.State,
                //            Stock = hot.Stock,
                //            Salesvolume = hot.Salesvolume,
                //            AppId = appId
                //        };
                //        commodityCDTO.Add(new CommodityListCDTO
                //        {
                //            Name = hot.Name,
                //            Id = hot.Id,
                //            Pic = hot.PicturesPath,
                //            Price = hot.Price,
                //            State = hot.State,
                //            Stock = hot.Stock,
                //            Intensity = 10,
                //            DiscountPrice = -1
                //        });
                //        model.EntityState = System.Data.EntityState.Added;
                //        contextSession.SaveObject(model);
                //    }
                //    contextSession.SaveChanges();
                //}
                //return commodityCDTO.Take(4).ToList();

                List<CommodityListCDTO> data;
                data = RedisHelper.Get<List<CommodityListCDTO>>(YJDianDiKey + "_" + appId);
                if (data == null)
                {
                    var appIds = MallApply.GetTGQuery(appId).Select(_ => _.AppId).ToList();
                    if (!appIds.Contains(appId)) appIds.Add(appId);
                    data = Commodity.ObjectSet()
                      .Where(n => appIds.Contains(n.AppId) && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                      .OrderByDescending(n => n.Salesvolume)
                      .ThenByDescending(n => n.TotalCollection)
                      .ThenByDescending(n => n.SubTime)
                      .Take(4).Select(hot => new CommodityListCDTO
                      {
                          Name = hot.Name,
                          Id = hot.Id,
                          Pic = hot.PicturesPath,
                          Price = hot.Price,
                          State = hot.State,
                          Stock = hot.Stock,
                          Intensity = 10,
                          DiscountPrice = -1
                      }).ToList();
                    //LogHelper.Info("PromotionSV.GetYJDianDi FromSqlserver.....");
                    RedisHelper.Set(YJDianDiKey + "_" + appId, data);
                }
                else
                {
                    //LogHelper.Info("PromotionSV.GetYJDianDi FromRedis.....");
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取热销商品服务器异常。appId：{0}", appId), ex);
                return null;
            }
        }
        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        //public void GetYJDDBrowseInfo(Guid appId,Guid userId)
        //{

        //}




        /// <summary>
        /// 根据促销ID获取促销商品
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetPromotionItemsExt
            (System.Guid promotionId, System.Guid appId, int pageIndex, int pageSize)
        {
            try
            {
                DateTime now = DateTime.Now;
                //decimal Intensity = 10;
                //decimal DiscountPrice = -1;
                //if (promotion.Intensity > 0)
                //{
                //    Intensity = Convert.ToDecimal(promotion.Intensity);
                //}
                //if (promotion.DiscountPrice > -1)
                //{
                //    DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                //}
                var quary = from n in PromotionItems.ObjectSet()
                            join m in Promotion.ObjectSet() on n.PromotionId equals m.Id
                            join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                            where n.PromotionId == promotionId && !m.IsDel && b.CommodityType == 0
                            orderby b.SubTime descending
                            select new CommodityListCDTO
                            {
                                Name = b.Name,
                                Price = b.Price,
                                Pic = b.PicturesPath,
                                Id = b.Id,
                                Stock = b.Stock,
                                State = b.State,
                                Intensity = (decimal)n.Intensity,
                                DiscountPrice = (decimal)n.DiscountPrice,
                                LimitBuyEach = n.LimitBuyEach,
                                LimitBuyTotal = n.LimitBuyTotal,
                                SurplusLimitBuyTotal = n.SurplusLimitBuyTotal,
                                ComAttribute = b.ComAttribute
                            };
                List<CommodityListCDTO> pivm = quary
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
                if (pivm.Any())
                {
                    foreach (var com in pivm)
                    {
                        com.IsMultAttribute = Commodity.CheckComMultAttribute(com.ComAttribute);
                    }
                }

                return pivm;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据促销ID获取促销商品异常。promotionId：{0}，appId：{1}，pageIndex：{2}，pageSize：{3}，", promotionId, appId, pageIndex, pageSize), ex);

                return null;
            }
        }

        /// <summary>
        /// 获取当日商品促销信息
        /// </summary>
        /// <returns></returns>
        [Obsolete("已过时,刷新每日优惠请参见方法GetAppPromotions", false)]
        public List<PromotionItemShortCDTO> GetAllPromotionItemsExt()
        {
            try
            {
                DateTime now = DateTime.Now.Date;
                DateTime tomorrow = now.AddDays(1);

                var promotionDic = (from p in PromotionItems.ObjectSet()
                                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                    where pro.EndTime > now && pro.StartTime < tomorrow && !pro.IsDel && pro.IsEnable
                                    select new PromotionItemShortCDTO
                                    {
                                        PromotionId = p.PromotionId,
                                        CommodityId = p.CommodityId,
                                        Intensity = (decimal)p.Intensity,
                                        StartTime = pro.StartTime,
                                        EndTime = pro.EndTime,
                                        DiscountPrice = (decimal)p.DiscountPrice,
                                        LimitBuyEach = p.LimitBuyEach,
                                        LimitBuyTotal = p.LimitBuyTotal,
                                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal
                                    })
                                    .ToList();
                //更新当日缓存数据表
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //清空原有数据
                TodayPromotion.ObjectSet().Context.ExecuteStoreCommand("delete from TodayPromotion");
                foreach (PromotionItemShortCDTO p in promotionDic)
                {
                    TodayPromotion model = new TodayPromotion();
                    model.Id = Guid.NewGuid();
                    model.PromotionId = p.PromotionId;
                    model.CommodityId = p.CommodityId;
                    model.Intensity = p.Intensity;
                    model.StartTime = p.StartTime;
                    model.EndTime = p.EndTime;
                    model.DiscountPrice = p.DiscountPrice;
                    model.LimitBuyTotal = p.LimitBuyTotal;
                    model.LimitBuyEach = p.LimitBuyEach;
                    model.SurplusLimitBuyTotal = p.SurplusLimitBuyTotal;
                    model.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(model);
                }
                contextSession.SaveChanges();
                return promotionDic;

            }
            catch (Exception ex)
            {
                LogHelper.Error("获取当日商品促销信息异常。", ex);

                return null;
            }

        }
        /// <summary>
        /// 获取当日商品促销信息()
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, List<TodayPromotionDTO>> GetAppPromotionsExt()
        {
            try
            {
                Dictionary<Guid, List<TodayPromotionDTO>> result = new Dictionary<Guid, List<TodayPromotionDTO>>();
                DateTime now = DateTime.Now.Date;
                DateTime tomorrow = now.AddDays(1);

                var promotionDic = (from p in PromotionItems.ObjectSet()
                                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                    where pro.EndTime > now && (pro.StartTime < tomorrow || pro.PresellStartTime < tomorrow) && !pro.IsDel && pro.IsEnable
                                    select new PromotionItemShortCDTO
                                    {
                                        PromotionId = p.PromotionId,
                                        CommodityId = p.CommodityId,
                                        Intensity = (decimal)p.Intensity,
                                        StartTime = pro.StartTime,
                                        EndTime = pro.EndTime,
                                        DiscountPrice = (decimal)p.DiscountPrice,
                                        LimitBuyEach = p.LimitBuyEach,
                                        LimitBuyTotal = p.LimitBuyTotal,
                                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                        AppId = pro.AppId,
                                        ChannelId = pro.ChannelId,
                                        OutsideId = pro.OutsideId,
                                        PresellStartTime = pro.PresellStartTime,
                                        PresellEndTime = pro.PresellEndTime,
                                        PromotionType = pro.PromotionType,
                                        ExpireSecond = pro.ExpireSecond,
                                        GroupMinVolume = pro.GroupMinVolume,
                                        Description = pro.Description

                                    })
                                    .ToList();
                //更新当日缓存数据表
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //清空原有数据
                TodayPromotion.ObjectSet().Context.ExecuteStoreCommand("truncate table TodayPromotion");
                foreach (PromotionItemShortCDTO p in promotionDic)
                {
                    TodayPromotion model = new TodayPromotion();
                    model.Id = Guid.NewGuid();
                    model.PromotionId = p.PromotionId;
                    model.CommodityId = p.CommodityId;
                    model.Intensity = p.Intensity;
                    model.StartTime = p.StartTime;
                    model.EndTime = p.EndTime;
                    model.DiscountPrice = p.DiscountPrice;
                    model.LimitBuyTotal = p.LimitBuyTotal;
                    model.LimitBuyEach = p.LimitBuyEach;
                    model.SurplusLimitBuyTotal = p.SurplusLimitBuyTotal;
                    model.AppId = p.AppId;
                    model.ChannelId = p.ChannelId;
                    model.OutsideId = p.OutsideId;
                    model.PresellStartTime = p.PresellStartTime;
                    model.PresellEndTime = p.PresellEndTime;
                    model.PromotionType = p.PromotionType;
                    model.ExpireSecond = p.ExpireSecond;
                    model.GroupMinVolume = p.GroupMinVolume;
                    model.Description = p.Description;
                    model.EntityState = System.Data.EntityState.Added;

                    contextSession.SaveObject(model);

                    if (!result.ContainsKey(model.AppId))
                        result.Add(model.AppId, new List<TodayPromotionDTO>());
                    result[model.AppId].Add(model.ToEntityData());
                }
                contextSession.SaveChanges();

                TodayPromotion.ResetTodayPromotionCache(result);
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.Error("获取当日商品促销信息异常。", ex);

                return null;
            }

        }

        //处理热门商品
        public void AddHotCommodityExt()
        {
            try
            {
                LogHelper.Info("处理热门商品开始");
                var appIds = Commodity.ObjectSet().Where(n => n.CommodityType == 0).Select(n => n.AppId).Distinct().ToList();
                List<Commodity> hots = new List<Commodity>();
                foreach (Guid appId in appIds)
                {
                    var commoditys = Commodity.ObjectSet()
                            .Where(n => n.AppId == appId && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                            .OrderByDescending(n => n.Salesvolume)
                            .ThenByDescending(n => n.TotalCollection)
                            .ThenByDescending(n => n.SubTime)
                            .Take(5).ToList();
                    hots.AddRange(commoditys);
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //清空原有数据
                HotCommodity.ObjectSet().Context.ExecuteStoreCommand("truncate table HotCommodity");

                Thread.Sleep(3000);

                //var oldhots = HotCommodity.ObjectSet().ToList();
                //foreach (HotCommodity mod in oldhots)
                //{
                //    contextSession.Delete(mod);
                //    contextSession.SaveChanges();
                //}
                //插入新的热门商品(效率太低，看是否有批量插入的方法)

                foreach (Commodity hot in hots)
                {
                    HotCommodity model = new HotCommodity
                    {
                        Name = hot.Name,
                        Id = hot.Id,
                        CommodityId = hot.Id,
                        PicturesPath = hot.PicturesPath,
                        Price = hot.Price,
                        TotalReview = hot.TotalReview,
                        TotalCollection = hot.TotalCollection,
                        State = hot.State,
                        Stock = hot.Stock,
                        Salesvolume = hot.Salesvolume,
                        AppId = hot.AppId
                    };
                    model.EntityState = System.Data.EntityState.Added;

                    contextSession.SaveObject(model);
                }
                contextSession.SaveChanges();
                LogHelper.Info("处理热门商品完成");
                #region 热门商品发送广场
                //var commoditydds = HotCommodity.ObjectSet()
                //        .OrderByDescending(n => n.Salesvolume)
                //        .ThenByDescending(n => n.TotalCollection)
                //        .Select(n => new
                //        {
                //            Name = n.Name,
                //            Id = n.Id,
                //            AppId = n.AppId,
                //            PicturesPath = n.PicturesPath
                //        }).Take(2).ToList();

                //if (commoditydds == null || commoditydds.Count == 0)
                //{
                //    commoditydds = Commodity.ObjectSet()
                //     .Where(n => n.IsDel == false && n.State == 0 && n.Stock > 0)
                //     .OrderByDescending(n => n.Salesvolume)
                //     .ThenByDescending(n => n.TotalCollection)
                //     .Select(n => new
                //     {
                //         Name = n.Name,
                //         Id = n.Id,
                //         AppId = n.AppId,
                //         PicturesPath = n.PicturesPath
                //     })
                //     .Take(2).ToList();
                //}
                //List<IUS.Deploy.CustomDTO.HotDataCDTO> hotDataDTOs = new List<IUS.Deploy.CustomDTO.HotDataCDTO>();
                //foreach (var commoditydd in commoditydds)
                //{
                //    IUS.Deploy.CustomDTO.HotDataCDTO hotDataDTO = new IUS.Deploy.CustomDTO.HotDataCDTO();
                //    //hotDataDTO.Content = "";
                //    hotDataDTO.LinkUrl = string.Format("{0}btp.iuoooo.com/Mobile/CommodityView?appId={1}&commodityIds={2}&type=tuwen", Jinher.AMP.BTP.Common.CustomConfig.UrlPrefix, commoditydd.AppId, commoditydd.Id);
                //    hotDataDTO.PhotoUrl = commoditydd.PicturesPath;
                //    hotDataDTO.SmPhotoUrl = commoditydd.PicturesPath;
                //    hotDataDTO.Title = commoditydd.Name;
                //    hotDataDTO.Id = commoditydd.Id;
                //    hotDataDTO.Source = IUS.Deploy.Enum.SourceEnum.HotTrade;
                //    hotDataDTO.AppId = commoditydd.AppId;
                //    hotDataDTOs.Add(hotDataDTO);
                //}

                //AuthorizeHelper.InitAuthorizeInfo();
                ////热门商品发送广场
                //try
                //{
                //    Jinher.AMP.IUS.ISV.Facade.HotDataFacade hotDataSV = new IUS.ISV.Facade.HotDataFacade();


                //    IUS.Deploy.CustomDTO.ReturnInfoCDTO IUSResult = hotDataSV.AddHotData(hotDataDTOs);
                //    if (!IUSResult.IsSuccess)
                //    {
                //        LogHelper.Error(string.Format("热门商品发送广场异常。Message：{0}",//        IUSResult.Message));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    string errStack = ex.Message + ex.StackTrace;
                //    while (ex.InnerException != null)
                //    {
                //        errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                //        ex = ex.InnerException;
                //    }
                //    LogHelper.Error(string.Format("热门商品发送广场异常。",ex,//        ex.Message, errStack));
                //}
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理热门商品服务异常。", ex);
            }
        }


        /// <summary>
        /// 获取所有门店信息
        /// </summary>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        public List<StoreCacheDTO> GetAllStoresExt()
        {
            //var list = Store.ObjectSet().Select(
            //    a => new StoreCacheDTO
            //    {
            //        Id = a.Id,
            //        Name = a.Name,
            //        Address = a.Address,
            //        City = a.City,
            //        District = a.District,
            //        Phone = a.Phone,
            //        picture = a.picture,
            //        Province = a.Province,
            //        SubTime = a.SubTime

            //    }).ToList();

            //return list;
            return null;
        }

        public List<ComAttributeCacheDTO> GetAllCommAttributesExt()
        {
            //return ComAttibute.ObjectSet().OrderByDescending(n => n.AttributeId).Select(
            //    a => new ComAttributeCacheDTO
            //    {
            //        AttributeId = a.AttributeId,
            //        CommodityId = a.CommodityId,
            //        AttributeName = a.AttributeName,
            //        Name = a.Name,
            //        Code = a.Code,
            //        SecondAttributeId = a.SecondAttributeId,
            //        SecondAttributeName = a.SecondAttributeName,
            //        SubTime = a.SubTime
            //    }).ToList();


            List<Jinher.AMP.BTP.BE.Attribute> attributeDTOList = Jinher.AMP.BTP.BE.Attribute.ObjectSet().ToList();
            List<ComAttributeCacheDTO> list = SecondAttribute.ObjectSet().OrderBy(n => n.SubTime).Select(
                a => new ComAttributeCacheDTO
                {
                    Id = a.Id,
                    AttributeId = a.AttributeId,
                    Name = a.Name,
                    SubTime = a.SubTime
                }).ToList();

            if (list == null)
                list = new List<ComAttributeCacheDTO>();

            attributeDTOList.ForEach(a =>
            {
                list.Add(new ComAttributeCacheDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                });
            });

            return list;
        }

        public List<CategoryCacheDTO> GetAllCateGoriesExt()
        {
            return Category.ObjectSet().Where(a => a.IsDel == false).OrderBy(a => a.Sort).Select(
                a => new CategoryCacheDTO
                {
                    AppId = a.AppId,
                    CurrentLevel = a.CurrentLevel,
                    Id = a.Id,
                    Name = a.Name,
                    ParentId = a.ParentId,
                    Sort = a.Sort
                }).ToList();
        }

        /// <summary>
        /// 如果收藏的商品有促销，则发消息
        /// </summary>
        public void PromotionPushExt()
        {
            DateTime now = DateTime.Now;
            DateTime outdate = DateTime.Now.AddMinutes(-10);
            try
            {
                promotionPushOld(now, outdate);
                promotionPushFixed(now, outdate);
            }
            catch (Exception ex)
            {
                LogHelper.Error("PromotionSV.PromotionPushExt异常", ex);
            }
        }

        /// <summary>
        /// Collection收藏的商品有促销，则发消息
        /// </summary>
        /// <param name="now"></param>
        /// <param name="outdate"></param>
        private void promotionPushOld(DateTime now, DateTime outdate)
        {
            //取得Collection促销的商品
            var list = (from c in Collection.ObjectSet()
                        join p in TodayPromotion.ObjectSet()
                        on c.CommodityId equals p.CommodityId
                        where p.StartTime <= now && p.EndTime > now
                        && p.StartTime >= outdate//促销开始10分钟后就不在推送
                        select new
                        {
                            c.CommodityId,
                            c.UserId,
                            c.AppId
                        }).ToList();

            Console.WriteLine("查询促销商品Collection，总计:" + list.Count);


            List<Guid> commodityIds = list.Select(a => a.CommodityId).Distinct().ToList();

            if (commodityIds != null && commodityIds.Count > 0)
            {
                //取商品编号
                Dictionary<Guid, string> dic = (Commodity.ObjectSet().Where(c => commodityIds.Contains(c.Id) && c.CommodityType == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.No_Code
                    })).ToDictionary(a => a.Id, a => a.No_Code);

                BE.BELogic.AddMessage message = new BE.BELogic.AddMessage();

                //推送促销消息
                list.ForEach(
                    a =>
                    {
                        string code = "";
                        if (dic.ContainsKey(a.CommodityId))
                        {
                            code = dic[a.CommodityId];
                        }
                        if (!string.IsNullOrEmpty(code))
                        {
                            message.AddMessages(a.CommodityId.ToString(), a.UserId.ToString(), a.AppId, code, null, null, "commodity");
                        }
                    });
            }
        }

        /// <summary>
        /// SetCollection收藏的商品有促销，则发消息
        /// </summary>
        /// <param name="now"></param>
        /// <param name="outdate"></param>
        private void promotionPushFixed(DateTime now, DateTime outdate)
        {
            //取得SetCollection促销的商品
            var list = (from c in SetCollection.ObjectSet()
                        join p in TodayPromotion.ObjectSet()
                        on c.ColKey equals p.CommodityId
                        where c.ColType == 1 && p.StartTime <= now && p.EndTime > now
                        && p.StartTime >= outdate//促销开始10分钟后就不在推送
                        select new
                        {
                            c.ColKey,
                            c.UserId,
                            c.ChannelId
                        }).ToList();

            Console.WriteLine("查询促销商品SetCollection，总计:" + list.Count);


            List<Guid> commodityIds = list.Select(a => a.ColKey).Distinct().ToList();

            if (commodityIds != null && commodityIds.Count > 0)
            {
                //取商品编号
                Dictionary<Guid, string> dic = (Commodity.ObjectSet().Where(c => commodityIds.Contains(c.Id) && c.CommodityType == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.No_Code
                    })).ToDictionary(a => a.Id, a => a.No_Code);

                BE.BELogic.AddMessage message = new BE.BELogic.AddMessage();

                //推送促销消息
                list.ForEach(
                    a =>
                    {
                        string code = "";
                        if (dic.ContainsKey(a.ColKey))
                        {
                            code = dic[a.ColKey];
                        }
                        if (!string.IsNullOrEmpty(code))
                        {
                            message.AddMessages(a.ColKey.ToString(), a.UserId.ToString(), a.ChannelId, code, null, null, "commodity");
                        }
                    });
            }
        }
        /// <summary>
        /// 更新评价表用户信息
        /// </summary>
        public void UpdateUserInfoExt()
        {
            try
            {
                DateTime today = DateTime.Now.Date;
                DateTime yesterday = DateTime.Now.AddDays(-1).Date;

                //获取所有用户
                var users = (from u in CommodityUser.ObjectSet()
                             where u.ModifiedOn < today && u.ModifiedOn >= yesterday
                             select new
                             {
                                 u.UserId,
                                 u.AppId,
                                 u.UserName,
                                 u.HeadPic
                             }).ToList();

                if (users != null)
                {
                    ContextSession context = ContextFactory.CurrentThreadContext;

                    var userids = users.Select(u => u.UserId).Distinct();
                    var reviews = (from r in Review.ObjectSet()
                                   where userids.Contains(r.UserId)
                                   select r).ToList();
                    foreach (Review review in reviews)
                    {
                        foreach (var user in users)
                        {
                            if (user.UserId == review.UserId && user.AppId == review.AppId)
                            {
                                review.UserName = user.UserName;
                                review.UserHeader = user.HeadPic;
                            }
                        }

                        context.SaveObject(review);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("更新评价表用户信息异常。", ex);
            }
        }

        public void PromotionPushIUSExt()
        {
            DateTime queryStartDate = DateTime.Now.Date.AddDays(-3);
            DateTime queryEndDate = queryStartDate.AddDays(1);

            //取得有促销的商品
            var promotionAppIds = (from p in Promotion.ObjectSet()
                                   where p.StartTime < queryEndDate && p.EndTime >= queryStartDate && !p.IsDel && p.IsEnable && p.PromotionType == 0
                                   select new { AppId = p.AppId, SubId = p.SubId }
                        ).Distinct().ToDictionary(x => x.AppId, x => x.SubId);

            if (promotionAppIds == null || promotionAppIds.Count == 0)
            {
                return;
            }

            try
            {


                IUS.Deploy.CustomDTO.PicFromUrlCDTO addDataDTO = new IUS.Deploy.CustomDTO.PicFromUrlCDTO();
                foreach (Guid appId in promotionAppIds.Keys)
                {
                    addDataDTO.AppId = appId;
                    addDataDTO.Content = APPSV.GetAppName(appId) + "有新促销活动了，快去参加吧~";
                    //addDataDTO.AppType = Jinher.AMP.IUS.Deploy.Enum.AppTypeEnum.trade;
                    addDataDTO.PhotoUrl = "";
                    addDataDTO.ShareUrl = string.Format("{0}Mobile/PromotionList?AppId={1}&type=tuwen", Jinher.AMP.BTP.Common.CustomConfig.BtpDomain, appId);
                    addDataDTO.Source = Jinher.AMP.IUS.Deploy.Enum.SourceEnum.EBusinessInfo;
                    addDataDTO.Title = addDataDTO.Content;
                    addDataDTO.UserId = promotionAppIds[appId];
                    addDataDTO.UserName = (ContextDTO != null && ContextDTO.LoginUserName != null) ? ContextDTO.LoginUserName : "btp";

                    var result = Jinher.AMP.BTP.TPS.IUSSV.Instance.AddPicFromUrl(addDataDTO);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("促销发布广场消息异常。", ex);

            }
        }
        /// <summary>
        /// 查询商品即将开始的秒杀活动
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public PromotionItemShortCDTO GetSecKillPromotionExt(Guid commodityId)
        {
            DateTime now = DateTime.Now;
            DateTime end = now.AddMinutes(30);

            return (from p in PromotionItems.ObjectSet()
                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                    where pro.PromotionType == 1 && pro.StartTime > now && pro.StartTime < end && !pro.IsDel && pro.IsEnable
                    && p.CommodityId == commodityId
                    orderby pro.StartTime
                    select new PromotionItemShortCDTO
                    {
                        PromotionId = p.PromotionId,
                        CommodityId = p.CommodityId,
                        Intensity = (decimal)p.Intensity,
                        StartTime = pro.StartTime,
                        EndTime = pro.EndTime,
                        DiscountPrice = (decimal)p.DiscountPrice,
                        LimitBuyEach = p.LimitBuyEach,
                        LimitBuyTotal = p.LimitBuyTotal,
                        SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                        AppId = pro.AppId
                    }).FirstOrDefault();

        }
        /// <summary>
        /// 判断是否可以购买 商品活动进行中，或者没有即将开始的秒杀活动可以购买
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public bool CheckSecKillBuyExt(Guid commodityId)
        {
            DateTime now = DateTime.Now;
            var promotion = TodayPromotion.ObjectSet().FirstOrDefault(c => c.CommodityId == commodityId && c.StartTime <= now && c.EndTime > now);
            if (promotion != null)
            {
                return true;
            }
            var proPre = GetSecKillPromotionExt(commodityId);
            if (proPre == null)
                return true;
            return false;

        }

        /// <summary>
        /// 添加外部折扣或拼团
        /// </summary>
        /// <param name="discountsDTO">自定义折扣属性</param>
        public ResultDTO AddOutsidePromotionExt(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            PromotionDTO promotionDTO;
            DateTime now = DateTime.Now;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            string commodityNames = string.Empty;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

            try
            {
                LogHelper.Info(string.Format("添加外部折扣或拼团: \r\n{0}", JsonHelper.JsonSerializer(discountsDTO)));
                if (discountsDTO == null || discountsDTO.OutsideId == Guid.Empty)
                    return new ResultDTO { ResultCode = 2, Message = "添加失败，活动Id为空" };
                //活动来源，现在都是正品会，暂不启用
                //if (discountsDTO == null || discountsDTO.ChannelId == Guid.Empty)
                //    return new ResultDTO { ResultCode = 2, Message = "添加失败，活动Id为空" };
                if (discountsDTO.CommodityList == null || !discountsDTO.CommodityList.Any())
                    return new ResultDTO { ResultCode = 2, Message = "活动未添加任何商品" };
                if (discountsDTO.PromotionType == 2 && !discountsDTO.PresellStartTime.HasValue)
                    return new ResultDTO { ResultCode = 2, Message = "预约活动预约开始时间不能为空" };
                if (discountsDTO.PromotionType == 2 && !discountsDTO.PresellEndTime.HasValue)
                    return new ResultDTO { ResultCode = 2, Message = "预约活动预约结束时间不能为空" };

                // 预售
                if (discountsDTO.PromotionType == 5)
                {
                    if (!discountsDTO.PresellStartTime.HasValue)
                    {
                        return new ResultDTO { ResultCode = 2, Message = "预售活动开始时间不能为空" };
                    }
                    if (!discountsDTO.PresellEndTime.HasValue)
                    {
                        return new ResultDTO { ResultCode = 2, Message = "预售活动结束时间不能为空" };
                    }
                }


                var comIds = discountsDTO.CommodityList.Select(c => c.CommodityId).ToList();

                var coms = Commodity.ObjectSet().Where(c => c.IsDel == false && c.State == 0 && c.Stock > 0 && comIds.Contains(c.Id) && c.CommodityType == 0).ToList();
                if (coms.Count < discountsDTO.CommodityList.Count)
                    return new ResultDTO { ResultCode = 2, Message = "添加失败，选择的商品已删除、下架或库存为0" };

                DateTime startTime = discountsDTO.PresellStartTime.HasValue
                                         ? discountsDTO.PresellStartTime.Value
                                         : discountsDTO.StartTime;
                if (discountsDTO.PromotionType == 3)
                {
                    //拼团
                    var pros = (from item in PromotionItems.ObjectSet()
                                join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                                where
                                (comIds.Contains(item.CommodityId) && pro.PromotionType == 3 && !pro.IsDel && pro.EndTime >= startTime && pro.StartTime <= discountsDTO.EndTime)
                                ||
                                (pro.OutsideId == discountsDTO.OutsideId && !pro.IsDel
                                )
                                select pro.Id).Count();
                    if (pros > 0)
                        return new ResultDTO { ResultCode = 2, Message = "添加失败，选择的商品中存在同一时间段内已参加拼团的商品" };
                }
                else
                {
                    LogHelper.Debug(String.Format("discountsDTO:{0},comIds:{1}", JsonHelper.JsSerializer(discountsDTO), JsonHelper.JsSerializer(comIds)));

                    //非拼团
                    var pros = (from item in PromotionItems.ObjectSet()
                                join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                                where
                                (comIds.Contains(item.CommodityId) && pro.PromotionType != 3 && !pro.IsDel && pro.EndTime >= startTime && pro.StartTime <= discountsDTO.EndTime)
                                ||
                                (pro.OutsideId == discountsDTO.OutsideId && !pro.IsDel
                                )
                                select pro.Id).Count();
                    if (pros > 0)
                        return new ResultDTO { ResultCode = 2, Message = "添加失败，选择的商品中存在同一时间段内已参加活动的商品" };
                }

                //更新传如appId
                foreach (var promotionItemOutsideDTO in discountsDTO.CommodityList)
                {
                    promotionItemOutsideDTO.AppId = coms.First(c => c.Id == promotionItemOutsideDTO.CommodityId).AppId;
                }

                var dict = discountsDTO.CommodityList.GroupBy(c => c.AppId)
                            .Select(g => new { key = g.Key, list = g.ToList() })
                            .ToDictionary(d => d.key, d => d.list);
                //不同app的活动分组保存活动
                foreach (var app in dict)
                {
                    addPromotionOutside(discountsDTO, app.Key, app.Value, contextSession, needRefreshCacheTodayPromotions);
                }
                contextSession.SaveChanges();

                if (needRefreshCacheTodayPromotions.Any())
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Added));

                LogHelper.Info(string.Format("添加外部折扣完成: outsideid{0}", discountsDTO.OutsideId));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("PromotionSV.AddOutsidePromotionExt添加折扣服务异常。discountsDTO：{0}", JsonHelper.JsonSerializer(discountsDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        private bool addPromotionOutside(PromotionOutSideVM discountsDTO, Guid appId, List<PromotionItemOutsideDTO> commodityList, ContextSession contextSession, List<TodayPromotion> needRefreshCacheTodayPromotions)
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);
            if (needRefreshCacheTodayPromotions == null)
                needRefreshCacheTodayPromotions = new List<TodayPromotion>();

            decimal intensity = 10;
            if (discountsDTO.Intensity > 0 && discountsDTO.Intensity <= 10)
                intensity = discountsDTO.Intensity;

            Promotion promotion = new Promotion();
            promotion.Id = Guid.NewGuid();
            promotion.Name = string.IsNullOrEmpty(discountsDTO.PromotionName) ? "外部促销" : discountsDTO.PromotionName;
            promotion.PicturesPath = "";
            promotion.StartTime = discountsDTO.StartTime;
            promotion.EndTime = discountsDTO.EndTime;
            promotion.Intensity = intensity;
            promotion.DiscountPrice = !discountsDTO.DiscountPrice.HasValue ? -1 : discountsDTO.DiscountPrice;
            promotion.AppId = appId;
            promotion.IsAll = false;
            promotion.PromotionType = discountsDTO.PromotionType;
            promotion.IsEnable = discountsDTO.IsEnable;
            promotion.ChannelId = discountsDTO.ChannelId;
            promotion.OutsideId = discountsDTO.OutsideId;
            promotion.IsDel = false;
            promotion.PresellStartTime = discountsDTO.PresellStartTime;
            promotion.PresellEndTime = discountsDTO.PresellEndTime;
            promotion.GroupMinVolume = discountsDTO.GroupMinVolume;
            promotion.ExpireSecond = discountsDTO.ExpireSecond;
            promotion.Description = discountsDTO.Description;
            promotion.IsSell = discountsDTO.IsSell;
            promotion.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(promotion);

            //判断是否今日促销
            bool isToday = ((promotion.StartTime < tomorrow || promotion.PresellStartTime < tomorrow) && promotion.EndTime > DateTime.Today);

            for (int i = 0; i < commodityList.Count; i++)
            {
                PromotionItems promotionItemsDTO = new PromotionItems();
                promotionItemsDTO.Id = Guid.NewGuid();
                promotionItemsDTO.Name = "促销商品";
                promotionItemsDTO.PromotionId = promotion.Id;
                promotionItemsDTO.AppId = appId;
                promotionItemsDTO.SubId = discountsDTO.ChannelId; //正品会Id
                promotionItemsDTO.Intensity = intensity; //折扣取活动的折扣
                promotionItemsDTO.CommodityId = commodityList[i].CommodityId;
                promotionItemsDTO.DiscountPrice = !commodityList[i].DiscountPrice.HasValue ? -1 : commodityList[i].DiscountPrice;
                promotionItemsDTO.LimitBuyEach = commodityList[i].LimitBuyEach;
                promotionItemsDTO.LimitBuyTotal = commodityList[i].LimitBuyTotal;
                promotionItemsDTO.SurplusLimitBuyTotal = 0;
                promotionItemsDTO.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(promotionItemsDTO);
                if (isToday)
                {
                    TodayPromotion pro = new TodayPromotion();
                    pro.Id = Guid.NewGuid();
                    pro.Intensity = intensity;
                    pro.CommodityId = promotionItemsDTO.CommodityId;
                    pro.StartTime = promotion.StartTime;
                    pro.EndTime = promotion.EndTime;
                    pro.PromotionId = promotion.Id;
                    pro.DiscountPrice = promotionItemsDTO.DiscountPrice;
                    pro.LimitBuyEach = promotionItemsDTO.LimitBuyEach;
                    pro.LimitBuyTotal = promotionItemsDTO.LimitBuyTotal;
                    pro.SurplusLimitBuyTotal = 0;
                    pro.AppId = appId;
                    pro.PromotionType = discountsDTO.PromotionType;
                    pro.ChannelId = discountsDTO.ChannelId;
                    pro.OutsideId = discountsDTO.OutsideId;
                    pro.PresellStartTime = discountsDTO.PresellStartTime;
                    pro.PresellEndTime = discountsDTO.PresellEndTime;
                    pro.GroupMinVolume = discountsDTO.GroupMinVolume;
                    pro.ExpireSecond = discountsDTO.ExpireSecond;
                    pro.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }

                // 预售商品下架判断
                if (promotion.PromotionType == 5)
                {
                    Jinher.AMP.BTP.TPS.Helper.PromotionHelper.CommoditySoldOut(contextSession, promotion, promotionItemsDTO.CommodityId);
                }
            }
            return true;
        }

        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="outsideId">外部活动id</param>
        public ResultDTO DelOutsidePromotionExt(Guid outsideId)
        {
            try
            {
                LogHelper.Info("删除外部活动：outsideId：" + outsideId);
                var pros = Promotion.ObjectSet().Where(n => n.OutsideId == outsideId).ToList();
                if (pros.Any())
                {
                    foreach (var promotion in pros)
                    {
                        delPromotion(promotion);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("PromotionSV.DelOutsidePromotionExt 删除折扣服务异常。outsideId：{0}", outsideId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="promotion"></param>
        private bool delPromotion(Promotion promotion)
        {
            //if (promotion != null && promotion.PromotionType != 0)
            if (promotion != null)
            {
                //查询数据
                List<Guid> proCache = PromotionItems.ObjectSet().
                    Where(n => n.PromotionId.Equals(promotion.Id))
                    .Select(n => n.CommodityId).ToList();

                var todayPromotions =
                    TodayPromotion.ObjectSet()
                    .Where(c => c.PromotionId == promotion.Id)
                    .Select(c => new TodayPromotionDTO { Id = c.Id, CommodityId = c.CommodityId, AppId = c.AppId })
                    .ToList();

                //删除缓存表数据
                TodayPromotion.ObjectSet().Context.ExecuteStoreCommand("delete from TodayPromotion where PromotionId ='" + promotion.Id + "'");

                //删除促销商品表数据
                PromotionItems.ObjectSet().Context.ExecuteStoreCommand(
                    "delete from PromotionItems where PromotionId='" + promotion.Id + "'");

                //删除促销信息
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                promotion.IsDel = true;
                promotion.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveChanges();
                TodayPromotion.RemoveCaches(promotion.AppId, todayPromotions);
            }
            return true;
        }
        /// <summary>
        /// 修改折扣
        /// </summary>
        /// <param name="discountsDTO">自定义属性</param>
        public ResultDTO UpdateOutsidePromotionExt(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            List<TodayPromotion> needAddCacheTodayPromotions = new List<TodayPromotion>();
            List<TodayPromotion> needRemoveCacheTodayPromotions = new List<TodayPromotion>();

            //记录原有的促销是否是今天的
            bool isPreToday = false;

            DateTime now = DateTime.Now;
            DateTime tomorrow = DateTime.Today.AddDays(1);

            try
            {
                LogHelper.Info(string.Format("修改外部折扣: \r\n{0}", JsonHelper.JsonSerializer(discountsDTO)));
                if (discountsDTO == null || discountsDTO.OutsideId == Guid.Empty)
                    return new ResultDTO { ResultCode = 2, Message = "添加失败，活动Id为空" };
                //活动来源，现在都是正品会，暂不启用
                //if (discountsDTO == null || discountsDTO.ChannelId == Guid.Empty)
                //    return new ResultDTO { ResultCode = 2, Message = "添加失败，活动Id为空" };
                if (discountsDTO.CommodityList == null || !discountsDTO.CommodityList.Any())
                    return new ResultDTO { ResultCode = 2, Message = "活动未添加任何商品" };
                if (discountsDTO.PromotionType == 2 && !discountsDTO.PresellStartTime.HasValue)
                    return new ResultDTO { ResultCode = 2, Message = "预约活动预约开始时间不能为空" };
                if (discountsDTO.PromotionType == 2 && !discountsDTO.PresellEndTime.HasValue)
                    return new ResultDTO { ResultCode = 2, Message = "预约活动预约结束时间不能为空" };

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var promotions = Promotion.ObjectSet().Where(c => c.OutsideId == discountsDTO.OutsideId && !c.IsDel).ToList();
                if (!promotions.Any())
                    return new ResultDTO { ResultCode = 0, Message = "修改失败，活动不存在" };

                //if (promotions.Any(c => c.StartTime < now))
                //{
                //    return new ResultDTO { ResultCode = 2, Message = "修改失败，活动已开始或活动已结束" };
                //}

                var dict = discountsDTO.CommodityList.GroupBy(c => c.AppId)
                        .Select(g => new { key = g.Key, list = g.ToList() })
                        .ToDictionary(d => d.key, d => d.list);

                //不同app的活动分组保存活动
                foreach (var app in dict)
                {
                    var promotion = promotions.FirstOrDefault(c => c.AppId == app.Key);
                    if (promotion == null)
                    {
                        addPromotionOutside(discountsDTO, app.Key, app.Value, contextSession, needAddCacheTodayPromotions);
                        continue;
                    }
                    updatePromotionOutside(promotion, discountsDTO, app.Key, app.Value, contextSession, needAddCacheTodayPromotions, needRemoveCacheTodayPromotions);
                    promotions.RemoveAll(c => c.Id == promotion.Id);
                }

                //统一提交到数据库
                contextSession.SaveChanges();
                if (needRemoveCacheTodayPromotions.Any())
                {
                    needRemoveCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                if (needAddCacheTodayPromotions.Any())
                {
                    needAddCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Added));
                }
                LogHelper.Info(string.Format("修改外部折扣完成: outsideid{0}", discountsDTO.OutsideId));

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改折扣服务异常。discountsDTO：{0}", JsonHelper.JsonSerializer(discountsDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        private bool updatePromotionOutside(Promotion promotion, PromotionOutSideVM discountsDTO, Guid appId, List<PromotionItemOutsideDTO> commodityList, ContextSession contextSession, List<TodayPromotion> needAddCacheTodayPromotions, List<TodayPromotion> needRemoveCacheTodayPromotions)
        {
            if (needAddCacheTodayPromotions == null)
                needAddCacheTodayPromotions = new List<TodayPromotion>();
            if (needRemoveCacheTodayPromotions == null)
                needRemoveCacheTodayPromotions = new List<TodayPromotion>();

            //记录原有的促销是否是今天的
            bool isToday = false;

            DateTime now = DateTime.Now.Date;
            DateTime tomorrow = now.AddDays(1);
            decimal intensity = 10;
            if (discountsDTO.Intensity > 0 && discountsDTO.Intensity <= 10)
                intensity = discountsDTO.Intensity;

            //原有的促销是今日的
            if ((promotion.StartTime < tomorrow || promotion.PresellStartTime < tomorrow) && promotion.EndTime > DateTime.Today)
            {
                isToday = true;
                needRemoveCacheTodayPromotions = TodayPromotion.ObjectSet().Where(c => c.PromotionId == promotion.Id).ToList();
            }
            promotion.EntityState = System.Data.EntityState.Modified;
            promotion.StartTime = discountsDTO.StartTime;
            promotion.EndTime = discountsDTO.EndTime;
            promotion.Intensity = intensity;
            promotion.DiscountPrice = !discountsDTO.DiscountPrice.HasValue ? -1 : discountsDTO.DiscountPrice;
            promotion.PromotionType = discountsDTO.PromotionType;
            promotion.CommodityNames = discountsDTO.PromotionName;
            promotion.Name = string.IsNullOrEmpty(discountsDTO.PromotionName) ? "外部促销" : discountsDTO.PromotionName;
            promotion.IsEnable = discountsDTO.IsEnable;
            promotion.ChannelId = discountsDTO.ChannelId;
            promotion.OutsideId = discountsDTO.OutsideId;
            promotion.IsDel = false;
            promotion.PresellStartTime = discountsDTO.PresellStartTime;
            promotion.PresellEndTime = discountsDTO.PresellEndTime;
            promotion.GroupMinVolume = discountsDTO.GroupMinVolume;
            promotion.ExpireSecond = discountsDTO.ExpireSecond;
            promotion.Description = discountsDTO.Description;
            promotion.IsSell = discountsDTO.IsSell;
            promotion.IsStatis = false;
            contextSession.SaveObject(promotion);

            if (commodityList != null && commodityList.Count > 0)
            {
                for (int i = 0; i < commodityList.Count; i++)
                {
                    PromotionItems promotionItemsDTO = new PromotionItems();
                    promotionItemsDTO.Id = Guid.NewGuid();
                    promotionItemsDTO.Name = "促销商品";
                    promotionItemsDTO.PromotionId = promotion.Id;
                    promotionItemsDTO.AppId = appId;
                    promotionItemsDTO.SubId = discountsDTO.ChannelId; //正品会Id
                    promotionItemsDTO.Intensity = intensity; //折扣取活动的折扣
                    promotionItemsDTO.CommodityId = commodityList[i].CommodityId;
                    promotionItemsDTO.DiscountPrice = !commodityList[i].DiscountPrice.HasValue ? -1 : commodityList[i].DiscountPrice;
                    promotionItemsDTO.LimitBuyEach = commodityList[i].LimitBuyEach;
                    promotionItemsDTO.LimitBuyTotal = commodityList[i].LimitBuyTotal;
                    promotionItemsDTO.SurplusLimitBuyTotal = 0;
                    promotionItemsDTO.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(promotionItemsDTO);
                    if (promotion.StartTime < tomorrow & promotion.EndTime > DateTime.Today)
                    {
                        TodayPromotion pro = new TodayPromotion();
                        pro.Id = Guid.NewGuid();
                        pro.Intensity = intensity;
                        pro.CommodityId = promotionItemsDTO.CommodityId;
                        pro.StartTime = promotion.StartTime;
                        pro.EndTime = promotion.EndTime;
                        pro.PromotionId = promotion.Id;
                        pro.DiscountPrice = promotionItemsDTO.DiscountPrice;
                        pro.LimitBuyEach = promotionItemsDTO.LimitBuyEach;
                        pro.LimitBuyTotal = promotionItemsDTO.LimitBuyTotal;
                        pro.SurplusLimitBuyTotal = 0;
                        pro.AppId = appId;
                        pro.PromotionType = discountsDTO.PromotionType;
                        pro.ChannelId = discountsDTO.ChannelId;
                        pro.OutsideId = discountsDTO.OutsideId;
                        pro.PresellStartTime = discountsDTO.PresellStartTime;
                        pro.PresellEndTime = discountsDTO.PresellEndTime;
                        pro.GroupMinVolume = discountsDTO.GroupMinVolume;
                        pro.ExpireSecond = discountsDTO.ExpireSecond;

                        pro.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(pro);
                        needAddCacheTodayPromotions.Add(pro);
                    }

                    // 预售商品下架判断
                    if (promotion.PromotionType == 5)
                    {
                        Jinher.AMP.BTP.TPS.Helper.PromotionHelper.CommoditySoldOut(contextSession, promotion, promotionItemsDTO.CommodityId);
                    }
                }
            }
            //不做删除操作 防止拼团订单管理查询不到已经结束的订单信息
            if(promotion.PromotionType != 3)
            {
                PromotionItems.ObjectSet().Context.ExecuteStoreCommand("delete from PromotionItems where PromotionId='" + promotion.Id + "'");
            }
            if (isToday)
                TodayPromotion.ObjectSet().Context.
                            ExecuteStoreCommand("delete from todaypromotion where promotionid='" + promotion.Id + "'");
            return true;
        }

        /// <summary>
        /// 设置外部活动订单不支付过期时间
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public ResultDTO SetExpireSecondsExt(Guid appId, long seconds)
        {
            LogHelper.Info("外部活动同步时间：" + seconds);
            int i = 0;
            while (i < 3)
            {
                GlobalCacheWrapper.Add("G_OrderExpireSeconds", appId.ToString(), seconds.ToString(CultureInfo.InvariantCulture), CacheTypeEnum.redisSS, "BTPCache");
                var cacheData = GlobalCacheWrapper.GetData("G_OrderExpireSeconds", "zph", CacheTypeEnum.redisSS, "BTPCache") as string;
                if (cacheData == seconds.ToString(CultureInfo.InvariantCulture))
                {
                    LogHelper.Info("外部活动同步时间成功：" + seconds);
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }

                i++;
            }

            return new ResultDTO { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        ///活动订单，未支付超时时间（秒）默认2小时
        /// </summary>
        /// <returns></returns>
        public static long GetExpirePaySeconds(Guid appId)
        {
            SeckillConfigCDTO seckillConfig = ZPHSV.Instance.GetSeckillConfig(appId);
            if (seckillConfig != null)
            {
                return seckillConfig.AutoCancelMin * 60;
            }
            return 7200;
        }

        /// <summary>
        /// 数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        /// <returns></returns>
        public ResultDTO CommodityDataAndRedisDataSynchronizationExt()
        {
            int result = Promotion.CommodityDataAndRedisDataSynchronization();

            return new ResultDTO { ResultCode = result, Message = "Error" };
        }

        /// <summary>
        /// 数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        /// <returns></returns>
        public ResultDTO PromotionRedisExt(Guid promotionId)
        {
            int result = Promotion.PromotionRedis(promotionId);

            return new ResultDTO { ResultCode = result, Message = "Error" };
        }


        /// <summary>
        /// 获取当日商品促销信息()
        /// </summary>
        /// <returns></returns>
        public List<TodayPromotionDTO> GeTodayPromotionsExt(List<Guid?> outsideId)
        {
            try
            {
                var proList = (from p in PromotionItems.ObjectSet()
                               join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                               where !pro.IsDel && pro.IsEnable && outsideId.Contains(pro.OutsideId)
                               orderby pro.PromotionType descending
                               select new TodayPromotionDTO()
                               {
                                   PromotionId = p.PromotionId,
                                   CommodityId = p.CommodityId,
                                   Intensity = (decimal)p.Intensity,
                                   StartTime = pro.StartTime,
                                   EndTime = pro.EndTime,
                                   DiscountPrice = (decimal)p.DiscountPrice,
                                   LimitBuyEach = p.LimitBuyEach,
                                   LimitBuyTotal = p.LimitBuyTotal,
                                   SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                   AppId = pro.AppId,
                                   ChannelId = pro.ChannelId,
                                   OutsideId = pro.OutsideId,
                                   PresellStartTime = pro.PresellStartTime,
                                   PresellEndTime = pro.PresellEndTime,
                                   PromotionType = pro.PromotionType,
                                   GroupMinVolume = pro.GroupMinVolume,
                                   ExpireSecond = pro.ExpireSecond,
                                   Description = pro.Description
                               }).ToList();
                return proList;
            }
            catch (Exception ex)
            {
                LogHelper.Error("PromotionSVExt.GeTodayPromotionsExt 获取当日商品促销信息异常。", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取当日商品促销购买数量
        /// </summary>
        /// <param name="outsideId"></param>
        /// <returns></returns>
        public List<PromotionSurplusLimitBuyTotalDto> GetSurplusLimitBuyTotalExt(List<Guid> outsideId)
        {
            try
            {
                using (StopwatchLogHelper.BeginScope("PromotionSV.GetSurplusLimitBuyTotal"))
                {
                    var proList = (from p in PromotionItems.ObjectSet()
                                   join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                   where !pro.IsDel && pro.IsEnable && pro.OutsideId.HasValue && outsideId.Contains(pro.OutsideId.Value)
                                   select new PromotionSurplusLimitBuyTotalDto()
                                   {
                                       Id = pro.OutsideId.Value,
                                       SurplusLimitBuyTotal = p.SurplusLimitBuyTotal
                                   }).ToList();
                    return proList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("PromotionSVExt.GetSurplusLimitBuyTotalExt 获取当日商品促销购买数量。", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取商城品类
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public List<CategoryDTO> GetCategoryListExt(System.Guid AppId)
        {
            var ParentIds = Category.ObjectSet().Where(p => p.ParentId != Guid.Empty && p.AppId == AppId && p.IsDel == false).Select(s => s.ParentId).ToList();
            var query = Category.ObjectSet().Where(p => !ParentIds.Contains(p.Id) && p.AppId == AppId && p.IsDel == false).Select(s => new CategoryDTO { Id = s.Id, Name = s.Name,CurrentLevel=s.CurrentLevel }).ToList();
            return query;
        }



        /// <summary>
        /// 获取应用的一级商品分类
        /// <para>
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1Ext(Guid appId)
        {
           
            //获取类目信息
            var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false && n.CurrentLevel == 1).OrderBy(n => n.Sort);
            var query = from n in category
                        select new Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort
                        };
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> categorylist = query.ToList<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO>();
            if (categorylist == null)
            {
                categorylist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO>();
            }
            return categorylist;
        }
    }
}


