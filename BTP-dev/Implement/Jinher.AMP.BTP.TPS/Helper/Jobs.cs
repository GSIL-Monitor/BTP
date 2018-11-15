using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using System.Data;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// Job
    /// </summary>
    public static class Jobs
    {
        public static string RepairOrderItemYjbPrice(int days)
        {
            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，开始时间：" + DateTime.Now);
            HashSet<Guid> orderIds = new HashSet<Guid>();
            HashSet<Guid> errorOrderIds = new HashSet<Guid>();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var lastTime = DateTime.Now.AddDays(-days);
                var commodityOrders = CommodityOrder.ObjectSet().Where(t => t.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && t.SubTime >= lastTime);
                LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，商品ids：" + JsonHelper.JsSerializer(commodityOrders.Select(t => t.Id)));
                foreach (var commodityOrder in commodityOrders)
                {
                    // 查询商品易捷币抵用数量
                    var yjInfo = YJBSV.GetOrderItemYJInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                    if (yjInfo.IsSuccess)
                    {
                        var yjbInfo = yjInfo.Data.YJBInfo;
                        if (yjbInfo == null)
                        {
                            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，使用易捷币抵现订单Id：" + commodityOrder.Id + "，Items: null");
                        }
                        else
                        {
                            LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，使用易捷币抵现订单Id：" + commodityOrder.Id + "，Items:" + JsonHelper.JsSerializer(yjbInfo.Items));
                        }

                        var yjCouponInfo = yjInfo.Data.YJCouponInfo;
                        if (yjCouponInfo == null)
                        {
                            LogHelper.Debug("进入易捷抵现劵抵现订单，按照商品进行拆分，使用易捷抵现劵抵现订单Id：" + commodityOrder.Id + "，Items: null");
                        }
                        else
                        {
                            LogHelper.Debug("进入易捷抵现劵抵现订单，按照商品进行拆分，使用易捷抵现劵抵现订单Id：" + commodityOrder.Id + "，Items:" + JsonHelper.JsSerializer(yjCouponInfo.Items));
                        }

                        var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id).ToList().OrderBy(_ => _.CommodityId).ThenByDescending(_ => _.Number).ToList();
                        var dirCommodityIndex = orderItems.Select(_ => _.CommodityId).Distinct().ToDictionary(_ => _, _ => 0);
                        foreach (var orderItem in orderItems)
                        {
                            bool needUpdate = false;
                            if (yjbInfo != null && yjbInfo.Items != null)
                            {
                                var yjbInfoItems = yjbInfo.Items.Where(c => c.CommodityId == orderItem.CommodityId).ToList();
                                if (yjbInfoItems.Count > dirCommodityIndex[orderItem.CommodityId])
                                {
                                    var currentCommodityYjbInfo = yjbInfoItems[dirCommodityIndex[orderItem.CommodityId]];
                                    if (currentCommodityYjbInfo != null /*&& currentCommodityYjbInfo.IsMallYJB*/ && currentCommodityYjbInfo.InsteadCashAmount > 0)
                                    {
                                        if (orderItem.YjbPrice != currentCommodityYjbInfo.InsteadCashAmount)
                                        {
                                            needUpdate = true;
                                            orderItem.YjbPrice = currentCommodityYjbInfo.InsteadCashAmount;
                                        }
                                    }
                                    dirCommodityIndex[orderItem.CommodityId]++;
                                }
                            }
                            if (yjCouponInfo != null && yjCouponInfo.Items != null)
                            {
                                if (yjCouponInfo.Items.Count == 0)
                                {
                                    errorOrderIds.Add(orderItem.CommodityOrderId);
                                    continue;
                                }
                                var yjCouponPrice = orderItem.YJCouponPrice;
                                if (yjCouponInfo.Items.FirstOrDefault().OrderItemId != Guid.Empty)
                                {
                                    yjCouponPrice = yjCouponInfo.Items.Where(_ => _.OrderItemId == orderItem.Id).Sum(_ => _.InsteadCashAmount);
                                }
                                else
                                {
                                    yjCouponPrice = yjCouponInfo.Items.Where(_ => _.CommodityId == orderItem.CommodityId).Sum(_ => _.InsteadCashAmount);
                                }
                                if (yjCouponPrice != orderItem.YJCouponPrice)
                                {
                                    needUpdate = true;
                                    orderItem.YJCouponPrice = yjCouponPrice;
                                }
                            }
                            if (needUpdate)
                            {
                                orderItem.ModifiedOn = DateTime.Now;
                                orderIds.Add(orderItem.CommodityOrderId);
                            }
                        }
                    }
                }
                var count = contextSession.SaveChanges();
                LogHelper.Debug("进入易捷币抵现订单，按照商品进行拆分，结束时间：" + DateTime.Now);
            }
            catch (Exception ex)
            {
                LogHelper.Error(String.Format("易捷币抵现订单，按照商品进行拆分：" + ex.Message), ex);
            }
            return "错误订单：" + string.Join(",", errorOrderIds) + "---------修复订单：" + string.Join(",", orderIds);
        }

        public static void AutoSettingCommodityPrice()
        {
            LogHelper.Info("Jobs.AutoSettingCommodityPrice Begin......................................................");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var apps = CommodityPriceFloat.ObjectSet().Where(p => !p.IsDel && p.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId).ToList();
                LogHelper.Info("Begin update price, AppIds:" + JsonHelper.JsonSerializer(apps.Select(_ => _.AppIds)));
                var appIds = apps.SelectMany(_ => _.AppIds.Split(',')).Select(_ => new Guid(_));
                LogHelper.Info("Begin update price, AppGuidIds:" + JsonHelper.JsonSerializer(appIds));

                var commodityQuery = Commodity.ObjectSet().Where(_ => appIds.Contains(_.AppId));
                var commoditys = commodityQuery.Where(_ => _.CostPrice >= _.Price).ToList();
                var commodityIds = commoditys.Select(_ => _.Id).ToList();
                var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityQuery.Any(c => c.Id == _.CommodityId) && _.CostPrice >= _.Price).ToList();
                List<Guid> updateCommodityIds = new List<Guid>();
                LogHelper.Info("Begin Commodity.........." + commoditys.Count + "............................................");
                foreach (var com in commoditys)
                {
                    LogHelper.Info("Begin update price, CommodityId:" + com.Id + ", AppId:" + com.AppId);
                    var priceFloat = apps.Find(_ => _.AppIds.Split(',').Select(appId => new Guid(appId)).Contains(com.AppId));

                    if (com.CostPrice >= com.Price)
                    {
                        LogHelper.Info("CostPrice:" + com.CostPrice + ", FloatPrice:" + priceFloat.FloatPrice);
                        com.Price = (com.CostPrice ?? 0) + priceFloat.FloatPrice;
                        com.EntityState = EntityState.Modified;
                        contextSession.SaveObject(com);
                    }

                    // stock
                    var stocks = commodityStocks.Where(_ => _.CommodityId == com.Id).ToList();
                    foreach (var stock in stocks)
                    {
                        stock.Price = (stock.CostPrice ?? 0) + priceFloat.FloatPrice;
                        stock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(stock);
                    }

                    updateCommodityIds.Add(com.Id);

                    // 保存到商品明细报表
                    if (updateCommodityIds.Count >= 100)
                    {
                        SaveCommodityChange(updateCommodityIds, contextSession);
                        contextSession.SaveChanges();
                        updateCommodityIds = new List<Guid>();
                    }
                }

                // 商品表价格没问题，但是SKU表价格有问题的商品
                var otherCommodityIds = commodityStocks.Select(_ => _.CommodityId).Distinct().Except(commodityIds).ToList();
                var otherCommodities = Commodity.ObjectSet().Where(_ => otherCommodityIds.Contains(_.Id)).ToList();
                LogHelper.Info("Begin CommodityStock.........." + otherCommodities.Count + "............................................");
                foreach (var com in otherCommodities)
                {
                    LogHelper.Info("Begin update price, CommodityId:" + com.Id + ", AppId:" + com.AppId);
                    var priceFloat = apps.Find(_ => _.AppIds.Split(',').Select(appId => new Guid(appId)).Contains(com.AppId));

                    // stock
                    var stocks = commodityStocks.Where(_ => _.CommodityId == com.Id).ToList();
                    foreach (var stock in stocks)
                    {
                        stock.Price = (stock.CostPrice ?? 0) + priceFloat.FloatPrice;
                        stock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(stock);
                    }

                    updateCommodityIds.Add(com.Id);

                    // 保存到商品明细报表
                    if (updateCommodityIds.Count >= 100)
                    {
                        SaveCommodityChange(updateCommodityIds, contextSession);
                        contextSession.SaveChanges();
                        updateCommodityIds = new List<Guid>();
                    }
                }

                SaveCommodityChange(updateCommodityIds, contextSession);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("Jobs.AutoSettingCommodityPrice 异常", ex);
            }
            LogHelper.Info("Jobs.AutoSettingCommodityPrice End......................................................");
        }

        private static void SaveCommodityChange(List<Guid> ids, ContextSession contextSession)
        {
            foreach (var item in GenegrateCommodityChangeDto(ids))
            {
                CommodityChange ChangedCommodity = new CommodityChange();
                ChangedCommodity.Id = Guid.NewGuid();
                ChangedCommodity.CommodityId = item.CommodityId;
                ChangedCommodity.Name = item.Name;
                ChangedCommodity.Code = item.Code;
                ChangedCommodity.No_Number = item.No_Number;
                ChangedCommodity.SubId = item.SubId;
                ChangedCommodity.Price = item.Price;
                ChangedCommodity.Stock = item.Stock;
                ChangedCommodity.PicturesPath = item.PicturesPath;
                ChangedCommodity.Description = item.Description;
                ChangedCommodity.State = item.State;
                ChangedCommodity.IsDel = item.IsDel;
                ChangedCommodity.AppId = item.AppId;
                ChangedCommodity.No_Code = item.No_Code;
                ChangedCommodity.TotalCollection = item.TotalCollection;
                ChangedCommodity.TotalReview = item.TotalReview;
                ChangedCommodity.Salesvolume = item.Salesvolume;
                ChangedCommodity.ModifiedOn = item.ModifiedOn;
                ChangedCommodity.GroundTime = item.GroundTime;
                ChangedCommodity.ComAttribute = item.ComAttribute;
                ChangedCommodity.CategoryName = item.CategoryName;
                ChangedCommodity.SortValue = item.SortValue;
                ChangedCommodity.FreightTemplateId = item.FreightTemplateId;
                ChangedCommodity.MarketPrice = item.MarketPrice;
                ChangedCommodity.IsEnableSelfTake = item.IsEnableSelfTake;
                ChangedCommodity.Weight = item.Weight;
                ChangedCommodity.PricingMethod = item.PricingMethod;
                ChangedCommodity.SaleAreas = item.SaleAreas;
                ChangedCommodity.SharePercent = item.SharePercent;
                ChangedCommodity.CommodityType = item.CommodityType;
                ChangedCommodity.HtmlVideoPath = item.HtmlVideoPath;
                ChangedCommodity.MobileVideoPath = item.MobileVideoPath;
                ChangedCommodity.VideoPic = item.VideoPic;
                ChangedCommodity.VideoName = item.VideoName;
                ChangedCommodity.ScorePercent = item.ScorePercent;
                ChangedCommodity.Duty = item.Duty;
                ChangedCommodity.SpreadPercent = item.SpreadPercent;
                ChangedCommodity.ScoreScale = item.ScoreScale;
                ChangedCommodity.TaxRate = item.TaxRate;
                ChangedCommodity.TaxClassCode = item.TaxClassCode;
                ChangedCommodity.Unit = item.Unit;
                ChangedCommodity.InputRax = item.InputRax;
                ChangedCommodity.Barcode = item.Barcode;
                ChangedCommodity.JDCode = item.JDCode;
                ChangedCommodity.CostPrice = item.CostPrice;
                ChangedCommodity.IsAssurance = item.IsAssurance;
                ChangedCommodity.TechSpecs = item.TechSpecs;
                ChangedCommodity.SaleService = item.SaleService;
                ChangedCommodity.IsReturns = item.IsReturns;
                ChangedCommodity.ServiceSettingId = item.ServiceSettingId;
                ChangedCommodity.Type = item.Type;
                ChangedCommodity.YJCouponActivityId = item.YJCouponActivityId;
                ChangedCommodity.YJCouponType = item.YJCouponType;
                ChangedCommodity.SubOn = item.SubOn;
                ChangedCommodity.ModifiedId = Guid.Empty;
                ChangedCommodity.AuditState = 0;
                ChangedCommodity.EntityState = EntityState.Added;
                contextSession.SaveObject(ChangedCommodity);
            }
        }

        private static List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GenegrateCommodityChangeDto(List<Guid> ids)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list = new List<Deploy.CustomDTO.CommodityChangeDTO>();
            foreach (var item in ids)
            {
                #region //取出商品变动明细插入CommodityChange
                //取出Commodity表中编辑的数据
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list1 = (from n in Commodity.ObjectSet()
                                                                                  where n.Id == item
                                                                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                  {
                                                                                      CommodityId = n.Id,
                                                                                      Name = n.Name,
                                                                                      Code = n.Code,
                                                                                      No_Number = n.No_Number,
                                                                                      SubId = n.SubId,
                                                                                      Price = n.Price,
                                                                                      Stock = n.Stock,
                                                                                      PicturesPath = n.PicturesPath,
                                                                                      Description = n.Description,
                                                                                      State = n.State,
                                                                                      IsDel = n.IsDel,
                                                                                      AppId = n.AppId,
                                                                                      No_Code = n.No_Code,
                                                                                      TotalCollection = n.TotalCollection,
                                                                                      TotalReview = n.TotalReview,
                                                                                      Salesvolume = n.Salesvolume,
                                                                                      ModifiedOn = n.ModifiedOn,
                                                                                      GroundTime = n.GroundTime,
                                                                                      ComAttribute = n.ComAttribute,
                                                                                      CategoryName = n.CategoryName,
                                                                                      SortValue = n.SortValue,
                                                                                      FreightTemplateId = n.FreightTemplateId,
                                                                                      MarketPrice = n.MarketPrice,
                                                                                      IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                      Weight = n.Weight,
                                                                                      PricingMethod = n.PricingMethod,
                                                                                      SaleAreas = n.SaleAreas,
                                                                                      SharePercent = n.SharePercent,
                                                                                      CommodityType = n.CommodityType,
                                                                                      HtmlVideoPath = n.HtmlVideoPath,
                                                                                      MobileVideoPath = n.MobileVideoPath,
                                                                                      VideoPic = n.VideoPic,
                                                                                      VideoName = n.VideoName,
                                                                                      ScorePercent = n.ScorePercent,
                                                                                      Duty = n.Duty,
                                                                                      SpreadPercent = n.SpreadPercent,
                                                                                      ScoreScale = n.ScoreScale,
                                                                                      TaxRate = n.TaxRate,
                                                                                      TaxClassCode = n.TaxClassCode,
                                                                                      Unit = n.Unit,
                                                                                      InputRax = n.InputRax,
                                                                                      Barcode = n.Barcode,
                                                                                      JDCode = n.JDCode,
                                                                                      CostPrice = n.CostPrice,
                                                                                      IsAssurance = n.IsAssurance,
                                                                                      TechSpecs = n.TechSpecs,
                                                                                      SaleService = n.SaleService,
                                                                                      IsReturns = n.IsReturns,
                                                                                      Isnsupport = n.Isnsupport,
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = n.ModifieId
                                                                                  }).ToList();
                //取出CommodityStock表中编辑的数据
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list2 = (from n in Commodity.ObjectSet()
                                                                                  join m in CommodityStock.ObjectSet() on n.Id equals m.CommodityId
                                                                                  where m.CommodityId == item
                                                                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                  {
                                                                                      CommodityId = m.Id,
                                                                                      Name = n.Name,
                                                                                      Code = n.Code,
                                                                                      No_Number = n.No_Number,
                                                                                      SubId = n.SubId,
                                                                                      Price = m.Price,
                                                                                      Stock = m.Stock,
                                                                                      PicturesPath = n.PicturesPath,
                                                                                      Description = n.Description,
                                                                                      State = n.State,
                                                                                      IsDel = n.IsDel,
                                                                                      AppId = n.AppId,
                                                                                      No_Code = m.No_Code,
                                                                                      TotalCollection = n.TotalCollection,
                                                                                      TotalReview = n.TotalReview,
                                                                                      Salesvolume = n.Salesvolume,
                                                                                      ModifiedOn = n.ModifiedOn,
                                                                                      GroundTime = n.GroundTime,
                                                                                      ComAttribute = n.ComAttribute,
                                                                                      CategoryName = n.CategoryName,
                                                                                      SortValue = n.SortValue,
                                                                                      FreightTemplateId = n.FreightTemplateId,
                                                                                      MarketPrice = m.MarketPrice,
                                                                                      IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                      Weight = n.Weight,
                                                                                      PricingMethod = n.PricingMethod,
                                                                                      SaleAreas = n.SaleAreas,
                                                                                      SharePercent = n.SharePercent,
                                                                                      CommodityType = n.CommodityType,
                                                                                      HtmlVideoPath = n.HtmlVideoPath,
                                                                                      MobileVideoPath = n.MobileVideoPath,
                                                                                      VideoPic = n.VideoPic,
                                                                                      VideoName = n.VideoName,
                                                                                      ScorePercent = n.ScorePercent,
                                                                                      Duty = m.Duty,
                                                                                      SpreadPercent = n.SpreadPercent,
                                                                                      ScoreScale = n.ScoreScale,
                                                                                      TaxRate = n.TaxRate,
                                                                                      TaxClassCode = n.TaxClassCode,
                                                                                      Unit = n.Unit,
                                                                                      InputRax = n.InputRax,
                                                                                      Barcode = m.Barcode,
                                                                                      JDCode = m.JDCode,
                                                                                      CostPrice = m.CostPrice,
                                                                                      IsAssurance = n.IsAssurance,
                                                                                      TechSpecs = n.TechSpecs,
                                                                                      SaleService = n.SaleService,
                                                                                      IsReturns = n.IsReturns,
                                                                                      Isnsupport = n.Isnsupport,
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = n.ModifieId
                                                                                  }).ToList();
                if (list2.Count() > 0)
                {
                    list.AddRange(list2);
                }
                else
                {
                    list.AddRange(list1);
                }
                #endregion
            }
            //Jinher.AMP.BTP.IBP.Facade.CommodityChangeFacade ChangeFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityChangeFacade();
            //ChangeFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            //Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DTO = ChangeFacade.SaveCommodityChange(list);
            return list;
        }

        public static void RepairUnPayOrder(Guid orderId)
        {
            var commodityOrderDTO = CommodityOrder.FindByID(orderId);
            var facade = new ISV.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            if (commodityOrderDTO.RealPrice <= 0 && commodityOrderDTO.State == 0)
            {
                var rdto = facade.PayUpdateCommodityOrder(commodityOrderDTO.Id, commodityOrderDTO.UserId, commodityOrderDTO.AppId, 0, 0, 0, 0);
                if (rdto != null && rdto.ResultCode == 0)
                {
                    #region 易捷卡密订单
                    if (commodityOrderDTO.OrderType == 3)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            const string message = "易捷卡密订单调用盈科接口生成卡信息:";
                            var rdto1 = new IBP.Facade.YJBJCardFacade().Create(commodityOrderDTO.Id);
                            if (rdto1.isSuccess) LogHelper.Info(message + rdto1.Message);
                            else LogHelper.Error(message + rdto1.Message);
                        });
                    }
                    #endregion
                    #region 进销存订单
                    if (commodityOrderDTO.AppType.HasValue && new List<short> { 2, 3 }.Contains(commodityOrderDTO.AppType.Value))
                    {
                        new IBP.Facade.JdEclpOrderFacade().CreateOrder(commodityOrderDTO.Id, string.Empty);
                        new IBP.Facade.JdEclpOrderFacade().SendPayInfoToHaiXin(commodityOrderDTO.Id);
                    }
                    #endregion
                    new IBP.Facade.CommodityOrderFacade().SendPayInfoToYKBDMq(commodityOrderDTO.Id);//盈科大数据mq
                    YXOrderHelper.CreateOrder(commodityOrderDTO.Id);//网易严选订单
                }
            }
        }
    }
}
