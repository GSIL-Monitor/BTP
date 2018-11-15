using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Threading.Tasks;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;
using System.Data;
using System.Text;

namespace Jinher.AMP.BTP.TPS.Helper
{
    public static class JdJobHelper
    {
        public static List<Guid> JDAppIdList;

        static bool isSyncPrice = false;
        static bool isSyncPriceByMessage = false;
        static bool isSyncSkuState = false;
        static bool isSyncSkuStateByMessage = false;
        static bool isSyncStock = false;
        static bool isSyncStockByMessage = false;
        static bool isAuditing = false;

        static JdJobHelper()
        {
            JDAppIdList = new List<Guid>();
            if (!string.IsNullOrEmpty(CustomConfig.AppIds))
            {
                foreach (string v in CustomConfig.AppIds.Split(','))
                {
                    JDAppIdList.Add(Guid.Parse(v));
                }
            }
        }

        /// <summary>
        /// 自动审核
        /// </summary>
        public static void AutoAudit()
        {
            if (isAuditing)
            {
                LogHelper.Info("JdOrderHelper.AutoAudit 正在审核中，跳过。。。");
                return;
            }
            LogHelper.Info("JdOrderHelper.AutoAudit Begin。。。");
            isAuditing = true;
            try
            {
                var auditComFacade = new JDAuditComFacade();
                auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var autoAuditCostPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.CostModeState == 0).Select(_ => _.AppId).ToList();
                var autoAuditPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
                var autoAuditStockAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();

                var takeNum = 300;
                // 进价
                foreach (var jdAppId in JDAppIdList.Intersect(autoAuditCostPriceAppIds))
                {
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核进价。。。AppId:" + jdAppId);
                    var hasData = true;
                    //while (hasData)
                    {
                        var auditQuery = AuditManage.ObjectSet().Where(_ => _.Status == 0 && _.AppId == jdAppId).Select(_ => _.Id);
                        var autoAuditCostPriceIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 1 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditCostPriceIds.Count > 0)
                        {
                            auditComFacade.AuditJDCostPrice(autoAuditCostPriceIds, 1, "自动审核", 0, 0);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核进价。。。AppId:" + jdAppId);
                }

                // 售价
                foreach (var jdAppId in JDAppIdList.Intersect(autoAuditPriceAppIds))
                {
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核售价。。。AppId:" + jdAppId);
                    var hasData = true;
                    //while (hasData)
                    {
                        var auditQuery = AuditManage.ObjectSet().Where(_ => _.Status == 0 && _.AppId == jdAppId).Select(_ => _.Id);
                        var autoAuditPriceIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 2 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditPriceIds.Count > 0)
                        {
                            auditComFacade.AuditJDPrice(autoAuditPriceIds, 1, 0, "自动审核", 0);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核售价。。。AppId:" + jdAppId);
                }

                // 上下架库存
                foreach (var jdAppId in JDAppIdList.Intersect(autoAuditStockAppIds))
                {
                    var auditQuery = AuditManage.ObjectSet().Where(_ => _.Status == 0 && _.AppId == jdAppId).Select(_ => _.Id);
                    // 上架
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核上架。。。AppId:" + jdAppId);
                    var hasData = true;
                    //while (hasData)
                    {
                        var autoAuditOnShelfIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 3 && _.JdStatus == 2 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditOnShelfIds.Count > 0)
                        {
                            auditComFacade.SetPutaway(autoAuditOnShelfIds, 1);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核上架。。。AppId:" + jdAppId);
                    // 下架
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核下架。。。AppId:" + jdAppId);
                    hasData = true;
                    //while (hasData)
                    {
                        var autoAuditOffShelfIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 3 && _.JdStatus == 1 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditOffShelfIds.Count > 0)
                        {
                            auditComFacade.SetOffShelf(autoAuditOffShelfIds, 1);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核下架。。。AppId:" + jdAppId);

                    // 有货
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核有货。。。AppId:" + jdAppId);
                    hasData = true;
                    //while (hasData)
                    {
                        var autoAuditHaveStockIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 4 && _.JdStatus == 4 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditHaveStockIds.Count > 0)
                        {
                            auditComFacade.SetInStore(autoAuditHaveStockIds, 1);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核有货。。。AppId:" + jdAppId);
                    // 无货
                    LogHelper.Info("JdOrderHelper.AutoAudit 开始审核无货。。。AppId:" + jdAppId);
                    hasData = true;
                    //while (hasData)
                    {
                        var autoAuditSelloutIds = JdAuditCommodityStock.ObjectSet().Where(_ => _.AuditType == 4 && _.JdStatus == 3 && auditQuery.Contains(_.Id)).OrderBy(_ => _.SubTime).Select(_ => _.Id).Take(takeNum).ToList();
                        if (autoAuditSelloutIds.Count > 0)
                        {
                            auditComFacade.SetNoStock(autoAuditSelloutIds, 1);
                        }
                        else
                        {
                            hasData = false;
                        }
                    }
                    LogHelper.Info("JdOrderHelper.AutoAudit 结束审核无货。。。AppId:" + jdAppId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.AutoAudit 异常", ex);
            }
            finally
            {
                isAuditing = false;
                LogHelper.Info("JdOrderHelper.AutoAudit End。。。");
            }
        }

        /// <summary>
        /// 更新京东商品品牌墙信息---TODO
        /// </summary>
        public static void AutoUpdateBrand()
        {
            try
            {
                BrandFacade bf = new BrandFacade();
                CommodityInnerBrandFacade cibf = new CommodityInnerBrandFacade();
                int n;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var jdAppId in JDAppIdList)
                {
                    var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).ToList();
                    List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
                    var commodityIds = commodities.Select(_ => _.Id);
                    var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
                    skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
                    skuIds = skuIds.Distinct().ToList();
                    for (int i = 0; i < skuIds.Count; i++)
                    {
                        JdComDetailDto jdDetail = new JdComDetailDto();
                        jdDetail = JDSV.GetJdDetail(skuIds[i]);
                        if (jdDetail != null)
                        {
                            //存在该品牌，判断该商品与该品牌是否有关联，没有则创建关联，有关联无操作
                            //不存在创建该品牌，并将该商品与该品牌建立关联 
                            var brand = Brandwall.ObjectSet().FirstOrDefault(_ => _.Brandname == jdDetail.brandName && _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                            Commodity com = commodities.FirstOrDefault(_ => _.JDCode == skuIds[i]);
                            if (brand != null)
                            {
                                var cominbrand = CommodityInnerBrand.ObjectSet().Where(_ => _.CommodityId == com.Id);
                                if (cominbrand == null)
                                {
                                    #region 添加商品品牌
                                    CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                    comBrand.BrandId = brand.Id;
                                    comBrand.Name = brand.Brandname;
                                    comBrand.CommodityId = com.Id;
                                    comBrand.SubTime = DateTime.Now;
                                    comBrand.ModifiedOn = comBrand.SubTime;
                                    comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                    comBrand.CrcAppId = 0;
                                    contextSession.SaveObject(comBrand);
                                    contextSession.SaveChanges();
                                    #endregion
                                }
                            }
                            else
                            {
                                #region 添加品牌
                                brand = Brandwall.CreateBrandwall();
                                brand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                brand.Brandname = jdDetail.brandName;
                                brand.BrandLogo = "";
                                brand.Brandstatu = 1;
                                brand.SubTime = DateTime.Now;
                                brand.ModifiedOn = brand.SubTime;
                                contextSession.SaveObject(brand);
                                contextSession.SaveChanges();
                                #endregion
                                #region 添加商品品牌
                                CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                comBrand.BrandId = brand.Id;
                                comBrand.Name = brand.Brandname;
                                comBrand.CommodityId = com.Id;
                                comBrand.SubTime = DateTime.Now;
                                comBrand.ModifiedOn = comBrand.SubTime;
                                comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comBrand.CrcAppId = 0;
                                contextSession.SaveObject(comBrand);
                                contextSession.SaveChanges();
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.AutoUpdateBrand 异常", ex);
            }
        }

        ///// <summary>
        ///// 全量同步商品价格
        ///// </summary>
        //public static void AutoUpdateJdPrice()
        //{
        //    if (isSyncPrice)
        //    {
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 正在同步Jd商品价格，跳过。。。");
        //        return;
        //    }
        //    isSyncPrice = true;
        //    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 开始同步Jd商品价格");
        //    try
        //    {
        //        // 查询商品
        //        int n;
        //        ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //        bool hasData = true;
        //        int pageIndex = 0;
        //        foreach (var jdAppId in JDAppIdList)
        //        {
        //            hasData = true;
        //            pageIndex = 0;
        //            while (hasData)
        //            {
        //                var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).OrderBy(_ => _.Id)
        //.Skip(pageIndex * 1000).Take(1000).ToList();
        //                hasData = commodities.Count > 0;
        //                pageIndex++;
        //                if (hasData)
        //                {
        //                    List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
        //                    var commodityIds = commodities.Select(_ => _.Id);
        //                    var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //                    skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
        //                    skuIds = skuIds.Distinct().ToList();
        //                    List<JdPriceDto> jdPrices = new List<JdPriceDto>();
        //                    for (int i = 0; i < skuIds.Count; i += 99)
        //                    {
        //                        jdPrices.AddRange(JDSV.GetPrice(skuIds.Skip(i).Take(99).ToList()));
        //                    }
        //                    int count = 0;
        //                    //List<Guid> autoAuditPriceIds = new List<Guid>();
        //                    //List<Guid> autoAuditCostPriceIds = new List<Guid>();
        //                    //var autoAuditPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
        //                    //var autoAuditCostPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.CostModeState == 0).Select(_ => _.AppId).ToList();

        //                    // 处理有属性商品
        //                    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 开始处理Stock数据。。。" + jdAppId);
        //                    //Parallel.ForEach(commodityStocks.GroupBy(_ => _.CommodityId), group =>
        //                    foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
        //                    {
        //                        var addCount = 0;
        //                        var com = commodities.Where(_ => _.Id == group.Key).FirstOrDefault();
        //                        var auditCom = AddCommodityAudit(com);
        //                        foreach (var item in group)
        //                        {
        //                            var jdprice = jdPrices.Where(_ => _.SkuId == item.JDCode).FirstOrDefault();
        //                            if (jdprice == null)
        //                            {
        //                                LogHelper.Info("JdOrderHelper.AutoUpdatePrice-SKU 未获取到京东价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                continue;
        //                            }
        //                            // 进货价
        //                            if (jdprice.Price.HasValue && item.CostPrice != jdprice.Price.Value)
        //                            {
        //                                // 对比审核表
        //                                var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                                    && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
        //                                if (latestAuditData == null || latestAuditData.Value != jdprice.Price.Value)
        //                                {
        //                                    count++;
        //                                    addCount++;
        //                                    var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改进货价);
        //                                    auditStock.AuditType = 1;
        //                                    auditStock.JdAuditCommodityId = auditCom.Id;
        //                                    auditStock.JdPrice = jdprice.JdPrice;
        //                                    auditStock.JdCostPrice = jdprice.Price.Value;
        //                                    contextSession.SaveObject(auditStock);
        //                                    //if (autoAuditCostPriceAppIds.Contains(com.AppId))
        //                                    //{
        //                                    //    autoAuditCostPriceIds.Add(auditStock.Id);
        //                                    //}
        //                                    LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品进货价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                }
        //                            }

        //                            // 售价
        //                            if (item.Price != jdprice.JdPrice)
        //                            {
        //                                // 对比审核表
        //                                var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                                    && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
        //                                if (latestAuditData == null || latestAuditData.Value != jdprice.JdPrice)
        //                                {
        //                                    count++;
        //                                    addCount++;
        //                                    var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改现价);
        //                                    auditStock.AuditType = 2;
        //                                    auditStock.JdAuditCommodityId = auditCom.Id;
        //                                    auditStock.JdPrice = jdprice.JdPrice;
        //                                    if (jdprice.Price.HasValue)
        //                                    {
        //                                        auditStock.JdCostPrice = jdprice.Price.Value;
        //                                    }
        //                                    contextSession.SaveObject(auditStock);
        //                                    //if (autoAuditPriceAppIds.Contains(com.AppId))
        //                                    //{
        //                                    //    autoAuditPriceIds.Add(auditStock.Id);
        //                                    //}
        //                                    LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品售价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                }
        //                            }
        //                        }

        //                        if (addCount > 0)
        //                        {
        //                            contextSession.SaveObject(auditCom);
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }
        //                    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 结束处理Stock数据。。。" + jdAppId);

        //                    // 处理无属性商品，并Stock表中无数据的情况
        //                    count = 0;
        //                    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 开始处理Commodity数据。。。" + jdAppId);
        //                    foreach (var com in commodities.Where(c =>
        //                        (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
        //                        (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
        //                        !commodityStocks.Any(s => s.CommodityId == c.Id)))
        //                    {
        //                        var jdprice = jdPrices.Where(_ => _.SkuId == com.JDCode).FirstOrDefault();
        //                        if (jdprice == null)
        //                        {
        //                            LogHelper.Info("JdOrderHelper.AutoUpdatePrice-SKU 未获取到京东价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            continue;
        //                        }
        //                        var addCount = 0;
        //                        var auditCom = AddCommodityAudit(com);

        //                        // 进货价
        //                        if (jdprice.Price.HasValue && com.CostPrice != jdprice.Price.Value)
        //                        {
        //                            // 对比审核表
        //                            var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
        //                                && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
        //                            if (latestAuditData == null || latestAuditData.Value != jdprice.Price.Value)
        //                            {
        //                                count++;
        //                                addCount++;
        //                                var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改进货价);
        //                                auditStock.AuditType = 1;
        //                                auditStock.JdAuditCommodityId = auditCom.Id;
        //                                auditStock.JdPrice = jdprice.JdPrice;
        //                                auditStock.JdCostPrice = jdprice.Price.Value;
        //                                contextSession.SaveObject(auditStock);
        //                                //if (autoAuditCostPriceAppIds.Contains(com.AppId))
        //                                //{
        //                                //    autoAuditCostPriceIds.Add(auditStock.Id);
        //                                //}
        //                                LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品进货价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            }
        //                        }

        //                        // 售价
        //                        if (com.Price != jdprice.JdPrice)
        //                        {
        //                            // 对比审核表
        //                            var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
        //                                && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
        //                            if (latestAuditData == null || latestAuditData.Value != jdprice.JdPrice)
        //                            {
        //                                count++;
        //                                addCount++;
        //                                var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改现价);
        //                                auditStock.AuditType = 2;
        //                                auditStock.JdAuditCommodityId = auditCom.Id;
        //                                auditStock.JdPrice = jdprice.JdPrice;
        //                                if (jdprice.Price.HasValue)
        //                                {
        //                                    auditStock.JdCostPrice = jdprice.Price.Value;
        //                                }
        //                                contextSession.SaveObject(auditStock);
        //                                //if (autoAuditPriceAppIds.Contains(com.AppId))
        //                                //{
        //                                //    autoAuditPriceIds.Add(auditStock.Id);
        //                                //}
        //                                LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品售价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            }
        //                        }

        //                        if (addCount > 0)
        //                        {
        //                            contextSession.SaveObject(auditCom);
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }
        //                    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 结束处理Commodity数据。。。" + jdAppId);

        //                    contextSession.SaveChanges();

        //                    // 自动审核
        //                    //var auditComFacade = new JDAuditComFacade();
        //                    //auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
        //                    //if (autoAuditCostPriceIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.AuditJDCostPrice(autoAuditCostPriceIds, 1, "自动审核", 0, 0);
        //                    //}
        //                    //if (autoAuditPriceIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.AuditJDPrice(autoAuditPriceIds, 1, 0, "自动审核", 0);
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("JdOrderHelper.AutoUpdatePrice 异常", ex);
        //        isSyncPrice = false;
        //        throw;
        //    }
        //    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice 结束同步Jd商品价格");
        //    isSyncPrice = false;
        //}

        ///// <summary>
        ///// 全量同步商品价格
        ///// </summary>
        //public static void AutoUpdateJdPrice(Guid jdAppId)
        //{
        //    if (isSyncPrice)
        //    {
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 正在同步Jd商品价格，跳过。。。");
        //        return;
        //    }
        //    isSyncPrice = true;
        //    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始同步Jd商品价格");
        //    try
        //    {
        //        // 查询商品
        //        int n;
        //        ContextSession contextSession = ContextFactory.CurrentThreadContext;

        //        var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //        List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
        //        var commodityIds = commodities.Select(_ => _.Id);
        //        var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //        skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
        //        skuIds = skuIds.Distinct().ToList();
        //        List<JdPriceDto> jdPrices = new List<JdPriceDto>();
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始读取京东数据。。。");
        //        for (int i = 0; i < skuIds.Count; i += 99)
        //        {
        //            jdPrices.AddRange(JDSV.GetPrice(skuIds.Skip(i).Take(99).ToList()));
        //        }
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 结束读取京东数据。。。");
        //        int count = 0;

        //        //List<Guid> autoAuditPriceIds = new List<Guid>();
        //        //List<Guid> autoAuditCostPriceIds = new List<Guid>();
        //        //var autoAuditPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
        //        //var autoAuditCostPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.CostModeState == 0).Select(_ => _.AppId).ToList();

        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始处理Stock数据。。。");
        //        //Parallel.ForEach(commodityStocks.GroupBy(_ => _.CommodityId), group =>
        //        foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
        //        {
        //            var addCount = 0;
        //            var com = commodities.Where(_ => _.Id == group.Key).FirstOrDefault();
        //            var auditCom = AddCommodityAudit(com);
        //            foreach (var item in group)
        //            {
        //                var jdprice = jdPrices.Where(_ => _.SkuId == item.JDCode).FirstOrDefault();
        //                if (jdprice == null)
        //                {
        //                    LogHelper.Info("JdOrderHelper.AutoUpdatePrice-SKU 未获取到京东价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                    continue;
        //                }
        //                // 进货价
        //                if (jdprice.Price.HasValue && item.CostPrice != jdprice.Price.Value)
        //                {
        //                    // 对比审核表
        //                    var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                        && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
        //                    if (latestAuditData == null || latestAuditData.Value != jdprice.Price.Value)
        //                    {
        //                        count++;
        //                        addCount++;
        //                        var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改进货价);
        //                        auditStock.AuditType = 1;
        //                        auditStock.JdAuditCommodityId = auditCom.Id;
        //                        auditStock.JdPrice = jdprice.JdPrice;
        //                        auditStock.JdCostPrice = jdprice.Price.Value;
        //                        contextSession.SaveObject(auditStock);
        //                        //if (autoAuditCostPriceAppIds.Contains(com.AppId))
        //                        //{
        //                        //    autoAuditCostPriceIds.Add(auditStock.Id);
        //                        //}
        //                        LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品进货价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                    }
        //                }

        //                // 售价
        //                if (item.Price != jdprice.JdPrice)
        //                {
        //                    // 对比审核表
        //                    var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                        && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
        //                    if (latestAuditData == null || latestAuditData.Value != jdprice.JdPrice)
        //                    {
        //                        count++;
        //                        addCount++;
        //                        var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改现价);
        //                        auditStock.AuditType = 2;
        //                        auditStock.JdAuditCommodityId = auditCom.Id;
        //                        auditStock.JdPrice = jdprice.JdPrice;
        //                        if (jdprice.Price.HasValue)
        //                        {
        //                            auditStock.JdCostPrice = jdprice.Price.Value;
        //                        }
        //                        contextSession.SaveObject(auditStock);
        //                        //if (autoAuditPriceAppIds.Contains(com.AppId))
        //                        //{
        //                        //    autoAuditPriceIds.Add(auditStock.Id);
        //                        //}
        //                        LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品售价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                    }
        //                }
        //            }

        //            if (addCount > 0)
        //            {
        //                contextSession.SaveObject(auditCom);
        //            }

        //            if (count >= 200)
        //            {
        //                contextSession.SaveChanges();
        //                count = 0;
        //            }
        //        }
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 结束处理Stock数据。。。");

        //        // 处理无属性商品，并Stock表中无数据的情况
        //        count = 0;

        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始处理Commodity数据。。。");
        //        Parallel.ForEach(commodities.Where(c =>
        //            (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
        //            (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
        //            !commodityStocks.Any(s => s.CommodityId == c.Id)), (com, loopState) =>
        //            //foreach (var com in commodities.Where(c =>
        //            //    (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
        //            //    (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
        //            //    !commodityStocks.Any(s => s.CommodityId == c.Id)))
        //            {
        //                var jdprice = jdPrices.Where(_ => _.SkuId == com.JDCode).FirstOrDefault();
        //                if (jdprice == null)
        //                {
        //                    LogHelper.Info("JdOrderHelper.AutoUpdatePrice-SKU 未获取到京东价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                    loopState.Break();
        //                }
        //                var addCount = 0;
        //                var auditCom = AddCommodityAudit(com);

        //                // 进货价
        //                if (jdprice.Price.HasValue && com.CostPrice != jdprice.Price.Value)
        //                {
        //                    // 对比审核表
        //                    var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
        //                        && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
        //                    if (latestAuditData == null || latestAuditData.Value != jdprice.Price.Value)
        //                    {
        //                        count++;
        //                        addCount++;
        //                        var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改进货价);
        //                        auditStock.AuditType = 1;
        //                        auditStock.JdAuditCommodityId = auditCom.Id;
        //                        auditStock.JdPrice = jdprice.JdPrice;
        //                        auditStock.JdCostPrice = jdprice.Price.Value;
        //                        contextSession.SaveObject(auditStock);
        //                        //if (autoAuditCostPriceAppIds.Contains(com.AppId))
        //                        //{
        //                        //    autoAuditCostPriceIds.Add(auditStock.Id);
        //                        //}
        //                        LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品进货价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                    }
        //                }

        //                // 售价
        //                if (com.Price != jdprice.JdPrice)
        //                {
        //                    // 对比审核表
        //                    var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
        //                        && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
        //                    if (latestAuditData == null || latestAuditData.Value != jdprice.JdPrice)
        //                    {
        //                        count++;
        //                        addCount++;
        //                        var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改现价);
        //                        auditStock.AuditType = 2;
        //                        auditStock.JdAuditCommodityId = auditCom.Id;
        //                        auditStock.JdPrice = jdprice.JdPrice;
        //                        if (jdprice.Price.HasValue)
        //                        {
        //                            auditStock.JdCostPrice = jdprice.Price.Value;
        //                        }
        //                        contextSession.SaveObject(auditStock);
        //                        //if (autoAuditPriceAppIds.Contains(com.AppId))
        //                        //{
        //                        //    autoAuditPriceIds.Add(auditStock.Id);
        //                        //}
        //                        LogHelper.Info("JdOrderHelper.AutoUpdatePrice 更新商品售价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                    }
        //                }

        //                if (addCount > 0)
        //                {
        //                    contextSession.SaveObject(auditCom);
        //                }

        //                if (count >= 200)
        //                {
        //                    contextSession.SaveChanges();
        //                    count = 0;
        //                }
        //            });
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 结束处理Commodity数据。。。");

        //        contextSession.SaveChanges();

        //        // 自动审核
        //        //LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始处理自动审核。。。autoAuditCostPriceIds:" + autoAuditCostPriceIds.Count + ", autoAuditPriceIds:" + autoAuditPriceIds.Count);
        //        //var auditComFacade = new JDAuditComFacade();
        //        //auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
        //        //if (autoAuditCostPriceIds.Count > 0)
        //        //{
        //        //    auditComFacade.AuditJDCostPrice(autoAuditCostPriceIds, 1, "自动审核", 0, 0);
        //        //}
        //        //if (autoAuditPriceIds.Count > 0)
        //        //{
        //        //    auditComFacade.AuditJDPrice(autoAuditPriceIds, 1, 0, "自动审核", 0);
        //        //}
        //        //LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 开始处理自动审核。。。");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("JdOrderHelper.AutoUpdatePrice-SingleApp 异常", ex);
        //        isSyncPrice = false;
        //        throw;
        //    }
        //    LogHelper.Info("JdOrderHelper.AutoUpdateJdPrice-SingleApp 结束同步Jd商品价格");
        //    isSyncPrice = false;
        //}

        /// <summary>
        /// 同步商品价格
        /// </summary>
        public static void AutoUpdatePriceByMessage()
        {
            if (isSyncPriceByMessage)
            {
                LogHelper.Info("JdOrderHelper.AutoUpdatePriceByMessage 正在同步Jd商品价格，跳过。。。");
                return;
            }
            isSyncPriceByMessage = true;

            LogHelper.Info("JdOrderHelper.AutoUpdatePriceByMessage 开始同步Jd商品价格");
            try
            {
                var messages = JDSV.GetMessage("2");
                if (messages == null || messages.Count == 0)
                {
                    isSyncPriceByMessage = false;
                    return;
                }
                LogHelper.Info("JdOrderHelper.AutoUpdatePriceByMessage 开始同步Jd商品价格，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                var skuIds = messages.Select(_ => _.Result.SkuId).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                List<JdPriceDto> jdPrices = new List<JdPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    jdPrices.AddRange(JDSV.GetPrice(skuIds.Skip(i).Take(99).ToList()));
                }
                var priceDtos = jdPrices.Select(p => new Deploy.CustomDTO.SN.SNPriceDto
                {
                    skuId = p.SkuId,
                    price = (p.Price ?? 0).ToString(),
                    snPrice = p.JdPrice.ToString()
                }).ToList();
                AutoCommodityHelper.UpdateCommodityPrice(skuIds, priceDtos, ThirdECommerceTypeEnum.JingDongDaKeHu);
                // 删除消息
                JDSV.DelMessage(messages.Select(_ => _.Id).ToList());
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.AutoUpdatePrice 异常", ex);
                isSyncPriceByMessage = false;
            }
            LogHelper.Info("JdOrderHelper.AutoUpdatePriceByMessage 结束同步Jd商品价格");
            isSyncPriceByMessage = false;
        }

        ///// <summary>
        ///// 全量同步商品上下架
        ///// </summary>
        //public static void AutoUpdateJdSkuState()
        //{
        //    try
        //    {
        //        if (isSyncSkuState)
        //        {
        //            LogHelper.Info("JdOrderHelper.AutoUpdateJdSkuState 正在同步Jd商品上下架，跳过。。。");
        //            return;
        //        }
        //        isSyncSkuState = true;

        //        // 查询商品
        //        int n;
        //        ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //        bool hasData = true;
        //        int pageIndex = 0;
        //        foreach (var jdAppId in JDAppIdList)
        //        {
        //            hasData = true;
        //            pageIndex = 0;
        //            while (hasData)
        //            {
        //                var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //                hasData = commodities.Count > 0;
        //                pageIndex++;
        //                if (hasData)
        //                {
        //                    List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
        //                    var commodityIds = commodities.Select(_ => _.Id);
        //                    var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //                    skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
        //                    skuIds = skuIds.Distinct().ToList();
        //                    List<JdSkuStateDto> jdSkuStates = new List<JdSkuStateDto>();
        //                    for (int i = 0; i < skuIds.Count; i += 99)
        //                    {
        //                        jdSkuStates.AddRange(JDSV.GetSkuState(skuIds.Skip(i).Take(99).ToList()));
        //                    }

        //                    int count = 0;

        //                    //List<Guid> autoAuditOnShelfIds = new List<Guid>();
        //                    //List<Guid> autoAuditOffShelfIds = new List<Guid>();
        //                    //var autoAuditAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();

        //                    //Parallel.ForEach(commodityStocks.GroupBy(_ => _.CommodityId), group =>
        //                    foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
        //                    {
        //                        var addCount = 0;
        //                        var com = commodities.Where(_ => _.Id == group.Key).FirstOrDefault();
        //                        var auditCom = AddCommodityAudit(com);
        //                        foreach (var item in group)
        //                        {
        //                            var jdState = jdSkuStates.Where(_ => _.Sku == item.JDCode).FirstOrDefault();
        //                            if (jdState == null)
        //                            {
        //                                LogHelper.Info("JdOrderHelper.AutoUpdateSkuState-SKU 未获取到京东上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                continue;
        //                            }
        //                            // 转换JD状态
        //                            var state = 0; // 上架
        //                            int auditState = 2; // 已上架
        //                            if (jdState.State == 0) // 下架
        //                            {
        //                                state = 1;
        //                                auditState = 1;
        //                            }
        //                            if (item.State != state)
        //                            {
        //                                // 对比审核表
        //                                var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                                    && _.AuditType == 3).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();

        //                                if (latestAuditState == 0 || latestAuditState != auditState)
        //                                {
        //                                    count++;
        //                                    addCount++;
        //                                    var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
        //                                    auditStock.AuditType = 3;
        //                                    auditStock.JdAuditCommodityId = auditCom.Id;
        //                                    auditStock.JdStatus = auditState;
        //                                    contextSession.SaveObject(auditStock);
        //                                    //if (autoAuditAppIds.Contains(com.AppId))
        //                                    //{
        //                                    //    if (auditState == 2)
        //                                    //    {
        //                                    //        autoAuditOnShelfIds.Add(auditStock.Id);
        //                                    //    }
        //                                    //    else
        //                                    //    {
        //                                    //        autoAuditOffShelfIds.Add(auditStock.Id);
        //                                    //    }
        //                                    //}
        //                                    LogHelper.Info("JdOrderHelper.AutoUpdateSkuState-SKU 更新商品上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                }
        //                            }
        //                        }

        //                        if (addCount > 0)
        //                        {
        //                            contextSession.SaveObject(auditCom);
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }

        //                    // 处理无属性商品，并Stock表中无数据的情况
        //                    count = 0;
        //                    foreach (var com in commodities.Where(c =>
        //                        (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
        //                        (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
        //                        !commodityStocks.Any(s => s.CommodityId == c.Id)))
        //                    {
        //                        var jdState = jdSkuStates.Where(_ => _.Sku == com.JDCode).FirstOrDefault();
        //                        if (jdState == null)
        //                        {
        //                            LogHelper.Info("JdOrderHelper.AutoUpdateSkuState 未获取到京东上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            continue;
        //                        }
        //                        // 转换JD状态
        //                        var state = 0; // 上架
        //                        int auditState = 2; // 已上架
        //                        if (jdState.State == 0) // 下架
        //                        {
        //                            state = 1;
        //                            auditState = 1;
        //                        }
        //                        if (com.State != state)
        //                        {
        //                            // 对比审核表
        //                            var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == com.Id
        //                                && _.AuditType == 3).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();
        //                            if (latestAuditState == 0 || latestAuditState != auditState)
        //                            {

        //                                var auditCom = AddCommodityAudit(com);
        //                                var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
        //                                auditStock.AuditType = 3;
        //                                auditStock.JdAuditCommodityId = auditCom.Id;
        //                                auditStock.JdStatus = auditState;
        //                                contextSession.SaveObject(auditStock);
        //                                //if (auditState == 2)
        //                                //{
        //                                //    autoAuditOnShelfIds.Add(auditStock.Id);
        //                                //}
        //                                //else
        //                                //{
        //                                //    autoAuditOffShelfIds.Add(auditStock.Id);
        //                                //}
        //                                contextSession.SaveObject(auditCom);
        //                                count++;
        //                                LogHelper.Info("JdOrderHelper.AutoUpdateSkuState 更新商品上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            }
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }

        //                    contextSession.SaveChanges();

        //                    //// 自动审核
        //                    //var auditComFacade = new JDAuditComFacade();
        //                    //auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
        //                    //if (autoAuditOnShelfIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.SetPutaway(autoAuditOnShelfIds, 0);
        //                    //}
        //                    //if (autoAuditOffShelfIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.SetOffShelf(autoAuditOffShelfIds, 0);
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("JdOrderHelper.AutoUpdateSkuState 异常", ex);
        //        isSyncSkuState = false;
        //        throw;
        //    }
        //    isSyncSkuState = false;
        //}

        /// <summary>
        /// 同步商品上下架
        /// </summary>
        public static void AutoUpdateJdSkuStateByMessage()
        {
            if (isSyncSkuStateByMessage)
            {
                LogHelper.Info("JdOrderHelper.AutoUpdateJdSkuStateByMessage 正在同步Jd商品上下架，跳过。。。");
                return;
            }
            isSyncSkuStateByMessage = true;

            try
            {
                LogHelper.Info("JdOrderHelper.AutoUpdateJdSkuStateByMessage 开始同步Jd商品上下架");
                var messages = JDSV.GetMessage("4");
                if (messages == null || messages.Count == 0)
                {
                    isSyncSkuStateByMessage = false;
                    return;
                }
                LogHelper.Info("JdOrderHelper.AutoUpdateJdSkuStateByMessage 开始同步Jd商品上下架，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                var skuIds = messages.Select(_ => _.Result.SkuId).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                //上下架状态
                List<JdSkuStateDto> jdSkuStates = new List<JdSkuStateDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    jdSkuStates.AddRange(JDSV.GetSkuState(skuIds.Skip(i).Take(99).ToList()));
                }
                var onSalesSkuIds = jdSkuStates.Where(s => s.State == 1).Select(s => s.Sku).ToList();
                var offSalesSkuIds = jdSkuStates.Where(s => s.State == 0).Select(s => s.Sku).ToList();
                //商品价格
                List<JdPriceDto> jdPrices = new List<JdPriceDto>();
                for (int i = 0; i < onSalesSkuIds.Count; i += 99)
                {
                    jdPrices.AddRange(JDSV.GetPrice(onSalesSkuIds.Skip(i).Take(99).ToList()));
                }
                var priceDtos = jdPrices.Select(p => new Deploy.CustomDTO.SN.SNPriceDto
                {
                    skuId = p.SkuId,
                    price = (p.Price ?? 0).ToString(),
                    snPrice = p.JdPrice.ToString()
                }).ToList();
                var validSkuid = jdPrices.Where(p => (p.Price ?? 0) > 0 && p.JdPrice > 0).Select(p => p.SkuId).Distinct().ToList();
                AutoCommodityHelper.UpdateCommodityPrice(onSalesSkuIds, priceDtos, ThirdECommerceTypeEnum.JingDongDaKeHu);
                AutoCommodityHelper.OnSalesCommodity(validSkuid, ThirdECommerceTypeEnum.JingDongDaKeHu);
                AutoCommodityHelper.OffSalesCommodity(offSalesSkuIds, ThirdECommerceTypeEnum.JingDongDaKeHu);
                // 删除消息
                JDSV.DelMessage(messages.Select(_ => _.Id).ToList());
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.AutoUpdateSkuState 异常", ex);
                isSyncSkuStateByMessage = false;
            }
            LogHelper.Info("JdOrderHelper.AutoUpdateJdSkuStateByMessage 结束同步Jd商品上下架");
            isSyncSkuStateByMessage = false;
        }

        ///// <summary>
        ///// 全量同步商品库存
        ///// </summary>
        //public static void AutoUpdateJdStock()
        //{
        //    if (isSyncStock)
        //    {
        //        LogHelper.Info("JdOrderHelper.AutoUpdateJdStock 正在同步Jd商品库存，跳过。。。");
        //        return;
        //    }
        //    isSyncStock = true;
        //    try
        //    {
        //        // 查询商品
        //        int n;
        //        ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //        bool hasData = true;
        //        int pageIndex = 0;
        //        foreach (var jdAppId in JDAppIdList)
        //        {
        //            hasData = true;
        //            pageIndex = 0;
        //            while (hasData)
        //            {
        //                var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //                hasData = commodities.Count > 0;
        //                pageIndex++;
        //                if (hasData)
        //                {
        //                    var commodityIds = commodities.Select(_ => _.Id);
        //                    List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
        //                    var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
        //                    skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
        //                    skuIds = skuIds.Distinct().ToList();
        //                    List<JdStockDto> jdStocks = new List<JdStockDto>();
        //                    for (int i = 0; i < skuIds.Count; i += 99)
        //                    {
        //                        jdStocks.AddRange(JDSV.GetStockById(skuIds.Skip(i).Take(99).ToList(), "1_0_0"));
        //                    }

        //                    int count = 0;
        //                    //List<Guid> autoAuditHaveStockIds = new List<Guid>();
        //                    //List<Guid> autoAuditSelloutIds = new List<Guid>();
        //                    //var autoAuditAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();
        //                    foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
        //                    {
        //                        var addCount = 0;
        //                        var com = commodities.Where(_ => _.Id == group.Key).FirstOrDefault();
        //                        var auditCom = AddCommodityAudit(com);
        //                        foreach (var item in group)
        //                        {
        //                            var jdStock = jdStocks.Where(_ => _.Sku == item.JDCode).FirstOrDefault();
        //                            if (jdStock == null)
        //                            {
        //                                LogHelper.Info("JdOrderHelper.AutoUpdateJdStock-SKU 未获取到京东商品库存，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                continue;
        //                            }
        //                            // 转换JD状态
        //                            var jdHaveStock = jdStock.HaveStock;
        //                            var comHaveStock = item.Stock > 0;
        //                            int auditState = 3; //无货
        //                            if (jdHaveStock) auditState = 4;
        //                            if (jdHaveStock != comHaveStock)
        //                            {
        //                                // 对比审核表
        //                                var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
        //                                    && _.AuditType == 4).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();

        //                                if (latestAuditState == 0 || latestAuditState != auditState)
        //                                {
        //                                    count++;
        //                                    addCount++;
        //                                    var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
        //                                    auditStock.AuditType = 4;
        //                                    auditStock.JdAuditCommodityId = auditCom.Id;
        //                                    auditStock.JdStatus = auditState;
        //                                    contextSession.SaveObject(auditStock);
        //                                    //if (autoAuditAppIds.Contains(com.AppId))
        //                                    //{
        //                                    //    if (jdHaveStock)
        //                                    //    {
        //                                    //        autoAuditHaveStockIds.Add(auditStock.Id);
        //                                    //    }
        //                                    //    else
        //                                    //    {
        //                                    //        autoAuditSelloutIds.Add(auditStock.Id);
        //                                    //    }
        //                                    //}
        //                                    LogHelper.Info("JdOrderHelper.AutoUpdateJdStock-SKU 更新商品库存，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
        //                                }
        //                            }
        //                        }

        //                        if (addCount > 0)
        //                        {
        //                            contextSession.SaveObject(auditCom);
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }

        //                    // 处理无属性商品，并Stock表中无数据的情况
        //                    count = 0;
        //                    foreach (var com in commodities.Where(c =>
        //                        (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
        //                        (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
        //                        !commodityStocks.Any(s => s.CommodityId == c.Id)))
        //                    {
        //                        var jdStock = jdStocks.Where(_ => _.Sku == com.JDCode).FirstOrDefault();
        //                        if (jdStock == null)
        //                        {
        //                            LogHelper.Info("JdOrderHelper.AutoUpdateSkuState 未获取到京东商品库存，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            continue;
        //                        }

        //                        // 转换JD状态
        //                        var jdHaveStock = jdStock.HaveStock;
        //                        var comHaveStock = com.Stock > 0;
        //                        int auditState = 3; //无货
        //                        if (jdHaveStock) auditState = 4;
        //                        if (jdHaveStock != comHaveStock)
        //                        {
        //                            // 对比审核表
        //                            var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == com.Id
        //                                && _.AuditType == 4).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();
        //                            if (latestAuditState == 0 || latestAuditState != auditState)
        //                            {
        //                                count++;
        //                                var auditCom = AddCommodityAudit(com);
        //                                var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
        //                                auditStock.AuditType = 4;
        //                                auditStock.JdAuditCommodityId = auditCom.Id;
        //                                auditStock.JdStatus = auditState;
        //                                contextSession.SaveObject(auditStock);
        //                                //if (autoAuditAppIds.Contains(com.AppId))
        //                                //{
        //                                //    if (jdHaveStock)
        //                                //    {
        //                                //        autoAuditHaveStockIds.Add(auditStock.Id);
        //                                //    }
        //                                //    else
        //                                //    {
        //                                //        autoAuditSelloutIds.Add(auditStock.Id);
        //                                //    }
        //                                //}
        //                                contextSession.SaveObject(auditCom);
        //                                LogHelper.Info("JdOrderHelper.AutoUpdateJdStock 更新商品商品库存，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
        //                            }
        //                        }

        //                        if (count >= 200)
        //                        {
        //                            contextSession.SaveChanges();
        //                            count = 0;
        //                        }
        //                    }

        //                    contextSession.SaveChanges();

        //                    //// 自动审核
        //                    //var auditComFacade = new JDAuditComFacade();
        //                    //auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
        //                    //if (autoAuditHaveStockIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.SetInStore(autoAuditHaveStockIds, 0);
        //                    //}
        //                    //if (autoAuditSelloutIds.Count > 0)
        //                    //{
        //                    //    auditComFacade.SetNoStock(autoAuditSelloutIds, 0);
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("JdOrderHelper.AutoUpdateSkuState 异常", ex);
        //        isSyncStock = false;
        //        throw;
        //    }
        //    isSyncStock = false;
        //}

        /// <summary>
        /// 同步商品库存
        /// </summary>
        public static void AutoUpdateJdStockByMessage()
        {
            if (isSyncStockByMessage)
            {
                LogHelper.Info("JdOrderHelper.AutoUpdateJdStockByMessage 正在同步Jd商品库存，跳过。。。");
                return;
            }
            isSyncStockByMessage = true;

            try
            {
                // 查询商品
                LogHelper.Info("JdOrderHelper.AutoUpdateJdStockByMessage 开始同步Jd商品库存");
                var messages = JDSV.GetMessage("3");
                if (messages == null || messages.Count == 0)
                {
                    isSyncStockByMessage = false;
                    return;
                }
                LogHelper.Info("JdOrderHelper.AutoUpdateJdStockByMessage 开始同步Jd商品库存，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                var skuIds = messages.Select(_ => _.Result.SkuId).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                List<JdStockDto> jdStocks = new List<JdStockDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    jdStocks.AddRange(JDSV.GetStockById(skuIds.Skip(i).Take(99).ToList(), "1_0_0"));
                }
                var skuStockDir = jdStocks.ToDictionary(p => p.Sku, p => p.yjStock);
                AutoCommodityHelper.UpdateCommodityStock(skuStockDir, ThirdECommerceTypeEnum.JingDongDaKeHu);
                // 删除消息
                JDSV.DelMessage(messages.Select(_ => _.Id).ToList());
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.AutoUpdateJdStockByMessage 异常", ex);
                isSyncStockByMessage = false;
            }

            LogHelper.Info("JdOrderHelper.AutoUpdateJdStockByMessage 结束同步Jd商品库存");
            isSyncStockByMessage = false;
        }

        /// <summary>
        /// 拒收自动退款
        /// </summary>
        /// <returns></returns>
        public static string AutoRefund()
        {
            List<string> orderIds = new List<string>();
            try
            {
                LogHelper.Info("京东商品拒收自动退款，Begin....................");
                var commodityOrderFacade = new CommodityOrderFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };
                var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };
                var orderAfterSalesFacade = new CommodityOrderAfterSalesFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var jdOrders = JdOrderItem.ObjectSet().Where(_ => _.State == 3 && (_.IsRefund == null || !_.IsRefund.Value)).ToList();
                foreach (var order in jdOrders)
                {
                    if (!order.CommodityOrderItemId.HasValue)
                    {
                        continue;
                    }

                    var commodityOrder = CommodityOrder.FindByID(new Guid(order.CommodityOrderId));
                    if (commodityOrder.State != 3)
                    {
                        continue;
                    }

                    var orderItem = OrderItem.ObjectSet().Where(_ => _.Id == order.CommodityOrderItemId.Value).FirstOrDefault();
                    if (orderItem == null)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，未找到订单Item, 提交申请：CommodityOrderItemId=" + order.CommodityOrderItemId.Value);
                        continue;
                    }

                    // 检查是否已申请退款
                    if (OrderRefundAfterSales.ObjectSet().Any(_ => _.OrderId == orderItem.CommodityOrderId && _.OrderItemId == orderItem.Id))
                    {
                        continue;
                    }

                    orderIds.Add(order.CommodityOrderId);
                    SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                    modelParam.Id = modelParam.commodityorderId = Guid.Parse(order.CommodityOrderId);
                    modelParam.RefundDesc = "京东商品拒收自动退款";
                    //modelParam.RefundExpCo = RefundExpCo;
                    //modelParam.RefundExpOrderNo = RefundExpOrderNo;

                    var CurrPic = orderItem.RealPrice * orderItem.Number;
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.Number);
                    }
                    //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额-关税-易捷抵用券金额
                    //modelParam.RefundMoney = CurrPic.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - (orderItem.YJCouponPrice ?? 0);
                    // 20180823 修改，增加抵用券退款
                    modelParam.RefundMoney = CurrPic.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty;
                    if (modelParam.RefundMoney <= 0)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，退款金额为零, 提交申请：CommodityOrderItemId=" + order.CommodityOrderItemId.Value + "CurrPic: " + CurrPic.Value + "CouponPrice: " + (orderItem.CouponPrice ?? 0) + "ChangeRealPrice: " + (orderItem.ChangeRealPrice ?? 0) + "Duty: " + orderItem.Duty + "YJCouponPrice: " + orderItem.YJCouponPrice);
                        //continue;
                    }

                    modelParam.State = 3;
                    modelParam.RefundReason = "其他";
                    // 仅退款
                    modelParam.RefundType = 0;
                    modelParam.OrderRefundImgs = "";
                    modelParam.OrderItemId = order.CommodityOrderItemId.Value;

                    LogHelper.Info("京东商品拒收自动退款，提交申请：CommodityOrderId=" + modelParam.commodityorderId + "CommodityOrderItemId=" + modelParam.OrderItemId);

                    var result = orderSV.SubmitOrderRefundAfterSales(modelParam);
                    if (result.ResultCode != 0)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，提交申请：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                        if (result.ResultCode == 110)
                        {
                            OrderSV.UnLockOrder(modelParam.commodityorderId);
                        }
                        //throw new Exception("退款失败");
                    }
                    else
                    {
                        // 退款申请成功后标记为已退款
                        order.IsRefund = true;
                        order.ModifiedOn = DateTime.Now;
                        contextSession.SaveObject(order);
                    }

                    try
                    {
                        // 同意退款
                        CancelTheOrderDTO model = new CancelTheOrderDTO
                        {
                            OrderId = modelParam.Id,
                            OrderItemId = modelParam.OrderItemId,
                            State = 21,
                            Message = "自动同意",
                            UserId = Guid.Empty
                        };
                        var cancelResult = orderAfterSalesFacade.CancelTheOrderAfterSales(model);
                        if (cancelResult.ResultCode == 1)
                        {
                            LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                            //throw new Exception("退款失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId, ex);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("京东商品拒收自动退款异常", ex);
            }
            LogHelper.Info("京东商品拒收自动退款，End...................." + JsonHelper.JsonSerializer(orderIds));
            return string.Join(",", orderIds);
        }

        /// <summary>
        /// 同意自动退款
        /// </summary>
        /// <returns></returns>
        public static string AutoRefundAgree()
        {
            List<string> orderIds = new List<string>();
            try
            {
                LogHelper.Info("京东商品拒收自动退款同意退款，Begin....................");
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade();
                var cf = new CommodityOrderAfterSalesFacade();
                orderSV.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var orderRefunds = OrderRefundAfterSales.ObjectSet().Where(_ => _.State == 0 && _.OrderItemId.HasValue).ToList();
                foreach (var orderRefund in orderRefunds)
                {
                    string strOrderId = orderRefund.OrderId.ToString();
                    // 判断是否为京东拒收订单
                    if (orderRefunds.Where(_ => _.OrderItemId == orderRefund.OrderItemId).Count() == 0
                        && JdOrderItem.ObjectSet().Any(_ => _.State == 3 && _.IsRefund.Value && _.CommodityOrderId == strOrderId
                           && _.CommodityOrderItemId == orderRefund.OrderItemId))
                    {
                        orderIds.Add(strOrderId);
                        // 同意退款
                        CancelTheOrderDTO model = new CancelTheOrderDTO();
                        model.OrderId = orderRefund.OrderId;
                        model.OrderItemId = orderRefund.OrderItemId.Value;
                        model.State = 21;
                        model.Message = "";
                        model.UserId = Guid.Empty;
                        var cancelResult = cf.CancelTheOrderAfterSales(model);
                        if (cancelResult.ResultCode == 1)
                        {
                            LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + orderRefund.OrderItemId.Value + ", Message=" + cancelResult.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("京东商品拒收自动退款同意退款异常", ex);
            }
            LogHelper.Info("京东商品拒收自动退款同意退款，End...................." + JsonHelper.JsonSerializer(orderIds));
            return string.Join(",", orderIds);
        }

        /// <summary>
        /// 拒收自动退款
        /// </summary>
        /// <returns></returns>
        public static string AutoRefundById(string orderId)
        {
            List<string> orderIds = new List<string>();
            try
            {
                LogHelper.Info("京东商品拒收自动退款，Begin....................");
                var commodityOrderFacade = new CommodityOrderFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };
                var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };
                var orderAfterSalesFacade = new CommodityOrderAfterSalesFacade
                {
                    ContextDTO = AuthorizeHelper.InitAuthorizeInfo()
                };

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var jdOrders = JdOrderItem.ObjectSet().Where(_ => _.CommodityOrderId == orderId && _.State == 3 && (_.IsRefund == null || !_.IsRefund.Value)).ToList();
                foreach (var order in jdOrders)
                {
                    if (!order.CommodityOrderItemId.HasValue)
                    {
                        continue;
                    }

                    var commodityOrder = CommodityOrder.FindByID(new Guid(order.CommodityOrderId));
                    if (commodityOrder.State != 3)
                    {
                        continue;
                    }

                    var orderItem = OrderItem.ObjectSet().Where(_ => _.Id == order.CommodityOrderItemId.Value).FirstOrDefault();
                    if (orderItem == null)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，未找到订单Item, 提交申请：CommodityOrderItemId=" + order.CommodityOrderItemId.Value);
                        continue;
                    }

                    // 检查是否已申请退款
                    if (OrderRefundAfterSales.ObjectSet().Any(_ => _.OrderId == orderItem.CommodityOrderId && _.OrderItemId == orderItem.Id))
                    {
                        continue;
                    }

                    orderIds.Add(order.CommodityOrderId);
                    SubmitOrderRefundDTO modelParam = new SubmitOrderRefundDTO();
                    modelParam.Id = modelParam.commodityorderId = Guid.Parse(order.CommodityOrderId);
                    modelParam.RefundDesc = "京东商品拒收自动退款";
                    //modelParam.RefundExpCo = RefundExpCo;
                    //modelParam.RefundExpOrderNo = RefundExpOrderNo;

                    var CurrPic = orderItem.RealPrice * orderItem.Number;
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.Number);
                    }
                    //已发货时默认退款金额=该商品实际售价-该商品承担的优惠券金额-该商品承担的改价商品金额-关税-易捷抵用券金额
                    modelParam.RefundMoney = CurrPic.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - (orderItem.YJCouponPrice ?? 0);
                    if (modelParam.RefundMoney <= 0)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，退款金额为零, 提交申请：CommodityOrderItemId=" + order.CommodityOrderItemId.Value + "CurrPic: " + CurrPic.Value + "CouponPrice: " + (orderItem.CouponPrice ?? 0) + "ChangeRealPrice: " + (orderItem.ChangeRealPrice ?? 0) + "Duty: " + orderItem.Duty + "YJCouponPrice: " + orderItem.YJCouponPrice);
                        //continue;
                    }

                    modelParam.State = 3;
                    modelParam.RefundReason = "其他";
                    // 仅退款
                    modelParam.RefundType = 0;
                    modelParam.OrderRefundImgs = "";
                    modelParam.OrderItemId = order.CommodityOrderItemId.Value;

                    LogHelper.Info("京东商品拒收自动退款，提交申请：CommodityOrderId=" + modelParam.commodityorderId + "CommodityOrderItemId=" + modelParam.OrderItemId);

                    var result = orderSV.SubmitOrderRefundAfterSales(modelParam);
                    if (result.ResultCode != 0)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，提交申请：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                        if (result.ResultCode == 110)
                        {
                            OrderSV.UnLockOrder(modelParam.commodityorderId);
                        }
                        //throw new Exception("退款失败");
                    }
                    else
                    {
                        // 退款申请成功后标记为已退款
                        order.IsRefund = true;
                        order.ModifiedOn = DateTime.Now;
                        contextSession.SaveObject(order);
                    }

                    try
                    {
                        // 同意退款
                        CancelTheOrderDTO model = new CancelTheOrderDTO
                        {
                            OrderId = modelParam.Id,
                            OrderItemId = modelParam.OrderItemId,
                            State = 21,
                            Message = "自动同意",
                            UserId = Guid.Empty
                        };
                        var cancelResult = orderAfterSalesFacade.CancelTheOrderAfterSales(model);
                        if (cancelResult.ResultCode == 1)
                        {
                            LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                            //throw new Exception("退款失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId, ex);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("京东商品拒收自动退款异常", ex);
            }
            LogHelper.Info("京东商品拒收自动退款，End...................." + JsonHelper.JsonSerializer(orderIds));
            return string.Join(",", orderIds);
        }

        /// <summary>
        /// 同意自动退款
        /// </summary>
        /// <returns></returns>
        public static string AutoRefundAgreeById(Guid orderId)
        {
            List<string> orderIds = new List<string>();
            try
            {
                LogHelper.Info("京东商品拒收自动退款同意退款，Begin....................");
                CommodityOrderFacade commodityOrderFacade = new CommodityOrderFacade();
                var orderSV = new ISV.Facade.CommodityOrderAfterSalesFacade();
                var cf = new CommodityOrderAfterSalesFacade();
                orderSV.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var orderRefunds = OrderRefundAfterSales.ObjectSet().Where(_ => _.OrderId == orderId && _.State == 0 && _.OrderItemId.HasValue).ToList();
                foreach (var orderRefund in orderRefunds)
                {
                    string strOrderId = orderRefund.OrderId.ToString();
                    // 判断是否为京东拒收订单
                    if (orderRefunds.Where(_ => _.OrderItemId == orderRefund.OrderItemId).Count() == 0
                        && JdOrderItem.ObjectSet().Any(_ => _.State == 3 && _.IsRefund.Value && _.CommodityOrderId == strOrderId
                           && _.CommodityOrderItemId == orderRefund.OrderItemId))
                    {
                        orderIds.Add(strOrderId);
                        // 同意退款
                        CancelTheOrderDTO model = new CancelTheOrderDTO();
                        model.OrderId = orderRefund.OrderId;
                        model.OrderItemId = orderRefund.OrderItemId.Value;
                        model.State = 21;
                        model.Message = "";
                        model.UserId = Guid.Empty;
                        var cancelResult = cf.CancelTheOrderAfterSales(model);
                        if (cancelResult.ResultCode == 1)
                        {
                            LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + orderRefund.OrderItemId.Value + ", Message=" + cancelResult.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("京东商品拒收自动退款同意退款异常", ex);
            }
            LogHelper.Info("京东商品拒收自动退款同意退款，End...................." + JsonHelper.JsonSerializer(orderIds));
            return string.Join(",", orderIds);
        }

        /// <summary>
        /// 同步退款状态
        /// </summary>
        public static void SyncRefundStatus()
        {
            LogHelper.Info("JdOrderHelper.SyncRefundStatus 开始");
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var afterSalesBpFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
                afterSalesBpFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var afterSalesSvFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
                afterSalesSvFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.AfsServiceStep != 50 && _.AfsServiceStep != 60).ToList();
                foreach (var item in jdOrderRefundAfterSales)
                {
                    var serviceListPage = JDSV.GetServiceListPage(item.JdOrderId);
                    if (serviceListPage == null || serviceListPage.Count == 0)
                    {
                        Console.WriteLine("同步京东状态异常，JdOrderId：" + item.JdOrderId);
                        continue;
                    }
                    AfsServicebyCustomerPin serviceInfo = null;
                    if (string.IsNullOrEmpty(item.AfsServiceId))
                    {
                        serviceInfo = serviceListPage.FirstOrDefault(_ => _.wareId == item.SkuId);
                        if (item.CommodityNum.HasValue)
                        {
                            item.AfsServiceIds = string.Join(",", serviceListPage.Where(_ => _.wareId == item.SkuId && _.afsApplyTime == serviceInfo.afsApplyTime)
                                .Take(item.CommodityNum.Value).Select(_ => _.afsServiceId));
                        }
                    }
                    else
                    {
                        serviceInfo = serviceListPage.FirstOrDefault(_ => _.afsServiceId == item.AfsServiceId);
                    }
                    if (serviceInfo == null)
                    {
                        continue;
                    }
                    //OrderRefundAfterSales refund = OrderRefundAfterSales.ObjectSet().Where(_ => _.Id == item.OrderRefundAfterSalesId).FirstOrDefault();
                    if (string.IsNullOrEmpty(item.AfsServiceId) || item.AfsServiceStep != serviceInfo.afsServiceStep)
                    {
                        bool isSuccess = true;

                        if (item.AfsServiceStep == 10)
                        {
                            if (serviceInfo.afsServiceStep == 40 || serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 34;
                            else if (serviceInfo.afsServiceStep == 32 || serviceInfo.afsServiceStep == 33 || serviceInfo.afsServiceStep == 34) serviceInfo.afsServiceStep = 31;
                        }
                        else if (item.AfsServiceStep == 31)
                        {
                            if (serviceInfo.afsServiceStep == 40 || serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 34;
                        }
                        else if (item.AfsServiceStep == 34)
                        {
                            if (serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 40;
                        }
                        switch (serviceInfo.afsServiceStep)
                        {
                            case 10:
                                //refund.State = 0;
                                break;
                            case 20://审核不通过
                                var serviceDetails = JDSV.GetServiceDetailInfo(serviceInfo.afsServiceId);
                                var result0 = afterSalesBpFacade.RefuseRefundOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 2, RefuseReason = serviceDetails.approveNotes });
                                if (result0.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 审核不通过，失败，" + result0.Message);
                                }
                                break;
                            case 21://客服审核
                            case 22://商家审核 - 对应金和待商家处理状态
                                //refund.State = 0;
                                break;
                            case 31://京东收货 - 对应金和待用户发货状态，但用户查看时显示状态名称为待京东上门取件 // 审核通过?
                                //refund.State = 10;
                                var result1 = afterSalesBpFacade.CancelTheOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 10 });
                                if (result1.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 京东收货 对应金和待用户发货状态，失败，" + result1.Message);
                                }
                                break;
                            case 32://商家收货
                            case 33://京东处理
                            case 34://商家处理 - 对应金和待商家确认收货状态  
                                //refund.State = 11;
                                var result2 = afterSalesSvFacade.AddOrderRefundExpAfterSales(new AddOrderRefundExpDTO { CommodityOrderId = item.OrderId, OrderItemId = item.OrderItemId, RefundExpCo = "京东快递", RefundExpOrderNo = "" });
                                if (result2.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 商家处理 对应金和待商家确认收货状态，失败，" + result2.Message);
                                }
                                break;
                            case 40://用户确认 对应金和商家确认收货，给用户打款
                                //refund.State = 12;
                                var result = afterSalesBpFacade.CancelTheOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 21 });
                                if (result.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 用户确认 对应金和商家确认收货，给用户打款失败，" + result.Message);
                                }
                                break;
                            case 50://完成
                            case 60://取消
                                break;
                        }

                        if (isSuccess)
                        {
                            item.AfsServiceId = serviceInfo.afsServiceId;
                            item.AfsServiceStep = serviceInfo.afsServiceStep;
                            item.AfsServiceStepName = serviceInfo.afsServiceStepName;
                            item.Cancel = (short)serviceInfo.cancel;
                            contextSession.SaveObject(item);
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.SyncRefundStatus 异常", ex);
                throw;
            }
        }

        /// <summary>
        /// 同步退款状态
        /// </summary>
        public static void SyncRefundStatusById(Guid id)
        {
            LogHelper.Info("JdOrderHelper.SyncRefundStatus 开始");
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var afterSalesBpFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
                afterSalesBpFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var afterSalesSvFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
                afterSalesSvFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.Id == id).ToList();
                foreach (var item in jdOrderRefundAfterSales)
                {
                    var serviceListPage = JDSV.GetServiceListPage(item.JdOrderId);
                    if (serviceListPage == null || serviceListPage.Count == 0)
                    {
                        Console.WriteLine("同步京东状态异常，JdOrderId：" + item.JdOrderId);
                        continue;
                    }
                    AfsServicebyCustomerPin serviceInfo = null;
                    if (string.IsNullOrEmpty(item.AfsServiceId))
                    {
                        serviceInfo = serviceListPage.FirstOrDefault(_ => _.wareId == item.SkuId);
                        if (item.CommodityNum.HasValue)
                        {
                            item.AfsServiceIds = string.Join(",", serviceListPage.Where(_ => _.wareId == item.SkuId && _.afsApplyTime == serviceInfo.afsApplyTime)
                                .Take(item.CommodityNum.Value).Select(_ => _.afsServiceId));
                        }
                    }
                    else
                    {
                        serviceInfo = serviceListPage.FirstOrDefault(_ => _.afsServiceId == item.AfsServiceId);
                    }
                    if (serviceInfo == null)
                    {
                        continue;
                    }
                    //OrderRefundAfterSales refund = OrderRefundAfterSales.ObjectSet().Where(_ => _.Id == item.OrderRefundAfterSalesId).FirstOrDefault();
                    if (string.IsNullOrEmpty(item.AfsServiceId) || item.AfsServiceStep != serviceInfo.afsServiceStep)
                    {
                        bool isSuccess = true;

                        if (item.AfsServiceStep == 10)
                        {
                            if (serviceInfo.afsServiceStep == 40 || serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 34;
                            else if (serviceInfo.afsServiceStep == 32 || serviceInfo.afsServiceStep == 33 || serviceInfo.afsServiceStep == 34) serviceInfo.afsServiceStep = 31;
                        }
                        else if (item.AfsServiceStep == 31)
                        {
                            if (serviceInfo.afsServiceStep == 40 || serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 34;
                        }
                        else if (item.AfsServiceStep == 34)
                        {
                            if (serviceInfo.afsServiceStep == 50) serviceInfo.afsServiceStep = 40;
                        }
                        switch (serviceInfo.afsServiceStep)
                        {
                            case 10:
                                //refund.State = 0;
                                break;
                            case 20://审核不通过
                                var serviceDetails = JDSV.GetServiceDetailInfo(serviceInfo.afsServiceId);
                                var result0 = afterSalesBpFacade.RefuseRefundOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 2, RefuseReason = serviceDetails.approveNotes });
                                if (result0.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 审核不通过，失败，" + result0.Message);
                                }
                                break;
                            case 21://客服审核
                            case 22://商家审核 - 对应金和待商家处理状态
                                //refund.State = 0;
                                break;
                            case 31://京东收货 - 对应金和待用户发货状态，但用户查看时显示状态名称为待京东上门取件 // 审核通过?
                                //refund.State = 10;
                                var result1 = afterSalesBpFacade.CancelTheOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 10 });
                                if (result1.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 京东收货 对应金和待用户发货状态，失败，" + result1.Message);
                                }
                                break;
                            case 32://商家收货
                            case 33://京东处理
                            case 34://商家处理 - 对应金和待商家确认收货状态  
                                //refund.State = 11;
                                var result2 = afterSalesSvFacade.AddOrderRefundExpAfterSales(new AddOrderRefundExpDTO { CommodityOrderId = item.OrderId, OrderItemId = item.OrderItemId, RefundExpCo = "京东快递", RefundExpOrderNo = "" });
                                if (result2.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 商家处理 对应金和待商家确认收货状态，失败，" + result2.Message);
                                }
                                break;
                            case 40://用户确认 对应金和商家确认收货，给用户打款
                                //refund.State = 12;
                                var result = afterSalesBpFacade.CancelTheOrderAfterSales(new CancelTheOrderDTO { OrderId = item.OrderId, OrderItemId = item.OrderItemId, State = 21 });
                                if (result.ResultCode != 0)
                                {
                                    isSuccess = false;
                                    LogHelper.Error("JdJobHelper.SyncRefundStatus: 用户确认 对应金和商家确认收货，给用户打款失败，" + result.Message);
                                }
                                break;
                            case 50://完成
                            case 60://取消
                                break;
                        }

                        if (isSuccess)
                        {
                            item.AfsServiceId = serviceInfo.afsServiceId;
                            item.AfsServiceStep = serviceInfo.afsServiceStep;
                            item.AfsServiceStepName = serviceInfo.afsServiceStepName;
                            item.Cancel = (short)serviceInfo.cancel;
                            contextSession.SaveObject(item);
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderHelper.SyncRefundStatus 异常", ex);
                throw;
            }
        }

        public static List<Guid> GetRefundErrorOrders()
        {
            var orderRefundQuery = OrderRefundAfterSales.ObjectSet();
            var jdOrderRefunds = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.AfsServiceStep == 40 || _.AfsServiceStep == 50).Join(orderRefundQuery, jr => jr.OrderRefundAfterSalesId, r => r.Id, (jr, r) => new { jr, r }).Where(_ => _.r.State != 1).Select(_ => _.jr).ToList();
            var result = new List<Guid>();
            result.AddRange(jdOrderRefunds.Select(_ => _.Id));
            return result;
        }

        public static void FixRefundStatusById(Guid id)
        {
            var jdOrderRefund = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.Id == id).FirstOrDefault();
            jdOrderRefund.AfsServiceStep = 10;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            contextSession.SaveChanges();
        }

        private static JdAuditCommodity AddCommodityAudit(Commodity com)
        {
            JdAuditCommodity auditCom = JdAuditCommodity.CreateJdAuditCommodity();
            auditCom.CommodityId = com.Id;
            auditCom.Name = com.Name;
            auditCom.Code = com.Code;
            auditCom.SubTime = com.SubTime;
            auditCom.SubId = com.SubId;
            auditCom.No_Number = com.No_Number;
            auditCom.Price = com.Price;
            auditCom.Stock = com.Stock;
            auditCom.PicturesPath = com.PicturesPath;
            auditCom.Description = com.Description;
            auditCom.State = com.State;
            auditCom.IsDel = com.IsDel;
            auditCom.AppId = com.AppId;
            auditCom.No_Code = com.No_Code;
            auditCom.TotalCollection = com.TotalCollection;
            auditCom.TotalReview = com.TotalReview;
            auditCom.Salesvolume = com.Salesvolume;
            auditCom.ModifiedOn = DateTime.Now;
            auditCom.GroundTime = com.GroundTime;
            auditCom.ComAttribute = com.ComAttribute;
            auditCom.CategoryName = com.CategoryName;
            auditCom.SortValue = com.SortValue;
            auditCom.FreightTemplateId = com.FreightTemplateId;
            auditCom.MarketPrice = com.MarketPrice;
            auditCom.IsEnableSelfTake = com.IsEnableSelfTake;
            auditCom.Weight = com.Weight;
            auditCom.PricingMethod = com.PricingMethod;
            auditCom.SaleAreas = com.SaleAreas;
            auditCom.SharePercent = com.SharePercent;
            auditCom.CommodityType = com.CommodityType;
            auditCom.HtmlVideoPath = com.HtmlVideoPath;
            auditCom.MobileVideoPath = com.MobileVideoPath;
            auditCom.VideoPic = com.VideoPic;
            auditCom.VideoName = com.VideoName;
            auditCom.ScorePercent = com.ScorePercent;
            auditCom.Duty = com.Duty;
            auditCom.SpreadPercent = com.SpreadPercent;
            auditCom.ScoreScale = com.ScoreScale;
            auditCom.TaxRate = com.TaxRate;
            auditCom.TaxClassCode = com.TaxClassCode;
            auditCom.Unit = com.Unit;
            auditCom.InputRax = com.InputRax;
            auditCom.Barcode = com.Barcode;
            auditCom.JDCode = com.JDCode;
            auditCom.CostPrice = com.CostPrice;
            auditCom.IsAssurance = com.IsAssurance;
            auditCom.TechSpecs = com.TechSpecs;
            auditCom.SaleService = com.SaleService;
            auditCom.IsReturns = com.IsReturns;
            auditCom.ServiceSettingId = com.ServiceSettingId;
            auditCom.Type = com.CommodityType;
            auditCom.YJCouponActivityId = com.YJCouponActivityId;
            auditCom.YJCouponType = com.YJCouponType;
            auditCom.ModifieId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
            auditCom.FieldName = "";
            return auditCom;
        }

        private static JdAuditCommodityStock AddCommodityStockAudit(ContextSession contextSession, Guid appId, CommodityStock stock, Jinher.AMP.BTP.Deploy.Enum.OperateTypeEnum type)
        {
            //添加审核表信息
            AuditManage AuditInfo = AuditManage.CreateAuditManage();
            AuditInfo.Status = 0; //0 待审核   1 审核通过  2 3审核不通过  4 已撤销
            AuditInfo.EsAppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
            AuditInfo.AppId = appId;
            AuditInfo.ApplyUserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
            AuditInfo.ApplyTime = DateTime.Now;
            AuditInfo.Action = (int)type;
            contextSession.SaveObject(AuditInfo);

            JdAuditCommodityStock auditComStock = JdAuditCommodityStock.CreateJdAuditCommodityStock();
            auditComStock.Id = AuditInfo.Id;
            auditComStock.CommodityStockId = stock.Id;
            auditComStock.CommodityId = stock.CommodityId;
            auditComStock.ComAttribute = stock.ComAttribute;
            auditComStock.Price = stock.Price;
            auditComStock.Stock = stock.Stock;
            auditComStock.MarketPrice = stock.MarketPrice;
            auditComStock.SubTime = stock.SubTime;
            auditComStock.ModifiedOn = DateTime.Now;
            auditComStock.Duty = stock.Duty;
            auditComStock.Barcode = stock.Barcode;
            auditComStock.No_Code = stock.Code;
            auditComStock.JDCode = stock.JDCode;
            auditComStock.CostPrice = stock.CostPrice;
            auditComStock.ThumImg = stock.ThumImg;
            auditComStock.CarouselImgs = stock.CarouselImgs;
            contextSession.SaveObject(AuditInfo);
            return auditComStock;
        }

        private static JdAuditCommodityStock AddCommodityStockAudit(ContextSession contextSession, Commodity com, Jinher.AMP.BTP.Deploy.Enum.OperateTypeEnum type)
        {
            //添加审核表信息
            AuditManage AuditInfo = AuditManage.CreateAuditManage();
            AuditInfo.Status = 0; //0 待审核   1 审核通过  2 3审核不通过  4 已撤销
            AuditInfo.EsAppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
            AuditInfo.AppId = com.AppId;
            AuditInfo.ApplyUserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;
            AuditInfo.ApplyTime = DateTime.Now;
            AuditInfo.Action = (int)type;
            contextSession.SaveObject(AuditInfo);

            JdAuditCommodityStock auditComStock = JdAuditCommodityStock.CreateJdAuditCommodityStock();
            auditComStock.Id = AuditInfo.Id;
            auditComStock.CommodityStockId = com.Id;
            auditComStock.CommodityId = com.Id;
            auditComStock.ComAttribute = com.ComAttribute;
            auditComStock.Price = com.Price;
            auditComStock.Stock = com.Stock;
            auditComStock.MarketPrice = com.MarketPrice;
            auditComStock.SubTime = com.SubTime;
            auditComStock.ModifiedOn = DateTime.Now;
            auditComStock.Duty = com.Duty;
            auditComStock.Barcode = com.Barcode;
            auditComStock.No_Code = com.Code;
            auditComStock.JDCode = com.JDCode;
            auditComStock.CostPrice = com.CostPrice;
            //auditComStock.ThumImg = com.ThumImg;
            //auditComStock.CarouselImgs = com.CarouselImgs;
            contextSession.SaveObject(AuditInfo);
            return auditComStock;
        }



        /// <summary>
        /// 京东已经确认收货，btp未确认交易完成情况
        /// </summary>
        public static void AutoModifyOrderState()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var commoityOrder = CommodityOrder.ObjectSet().Context.ExecuteStoreQuery<CommodityOrder>("select *from CommodityOrder where Id in (select CommodityOrderId from JdOrderItem where State in (3,4) group by  CommodityOrderId) and State in (1,2)").ToList();
                if (commoityOrder.Any())
                {
                    foreach (var item in commoityOrder)
                    {
                        string CommodityOrderId = item.Id.ToString();
                        var jdorderItem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId == CommodityOrderId).OrderByDescending(p => p.ModifiedOn).FirstOrDefault();
                        var commodityOrderService = CommodityOrderService.ObjectSet().Where(p => p.Code == item.Code).ToList();
                        if (!commodityOrderService.Any())
                        {
                            //已发货状态更新订单状态其他的不跟新
                            var result = facade.UpdateJobCommodityOrder(jdorderItem.ModifiedOn, item.Id, Guid.Empty, Guid.Parse("8b4d3317-6562-4d51-bef1-0c05694ac3a6"), item.Payment, "000000", "");
                            LogHelper.Debug(string.Format("成功与否:{0},{1}", result.isSuccess, result.Message));
                        }
                        else
                        {
                            item.State = 3;
                            item.ConfirmTime = jdorderItem.ModifiedOn;
                            item.EntityState = EntityState.Modified;
                        }

                    }
                    contextSession.SaveChanges();
                }

            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("AutoModifyOrderState异常信息:{0}", e.Message), e);
            }
        }




    }
}
