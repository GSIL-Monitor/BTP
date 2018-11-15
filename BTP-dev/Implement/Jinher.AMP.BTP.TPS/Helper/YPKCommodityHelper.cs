using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Threading.Tasks;
using System.Data;

namespace Jinher.AMP.BTP.TPS.Helper
{
    public static class YPKCommodityHelper
    {
        public static List<Guid> YPKAppIdList;
        static YPKCommodityHelper()
        {
            YPKAppIdList = CustomConfig.YPKAppIdList;
        }
        /// <summary>
        /// 同步商品价格
        /// </summary>
        public static void AutoUpdatePriceByMessage()
        {
            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 开始同步易派客商品价格");
            try
            {
                var messages = SuningSV.GetPriceMessage();
                if (messages == null || messages.Count == 0) return;
                LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 开始同步SN商品价格，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                var skuIds = messages.Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                // 
                // 易派客商品Ids
                var allCommodityIds = Commodity.ObjectSet().Where(_ => !_.IsDel && YPKAppIdList.Contains(_.AppId)).Select(_ => _.Id);
                var commodityStocks = CommodityStock.ObjectSet().Where(_ => allCommodityIds.Contains(_.CommodityId)
                    && skuIds.Contains(_.JDCode)).ToList();
                var stockCommodityIds = commodityStocks.Select(_ => _.CommodityId).Distinct();
                var stockCommodities = Commodity.ObjectSet().Where(_ => stockCommodityIds.Contains(_.Id)).ToList();

                List<Guid> autoAuditPriceIds = new List<Guid>();
                List<Guid> autoAuditCostPriceIds = new List<Guid>();
                var autoAuditPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
                var autoAuditCostPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.CostModeState == 0).Select(_ => _.AppId).ToList();
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(skuIds.Skip(i).Take(30).ToList()));
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int count = 0;
                foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
                {
                    var addCount = 0;
                    var com = stockCommodities.Where(_ => _.Id == group.Key).FirstOrDefault();
                    var auditCom = AddCommodityAudit(com);
                    foreach (var item in group)
                    {
                        var snprice = SNPrices.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                        if (snprice == null)
                        {
                            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage-SKU 未获取到易派客价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            continue;
                        }
                        // 进货价
                        if (!string.IsNullOrEmpty(snprice.price))
                        {
                            // 对比审核表
                            var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
                                && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
                            if (latestAuditData == null || latestAuditData.Value != Convert.ToDecimal(snprice.price))
                            {
                                count++;
                                addCount++;
                                var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改进货价);
                                auditStock.AuditType = 1;
                                auditStock.JdAuditCommodityId = auditCom.Id;
                                auditStock.JdPrice = string.IsNullOrEmpty(snprice.snPrice) ? Decimal.Zero : Convert.ToDecimal(snprice.snPrice);
                                auditStock.JdCostPrice = string.IsNullOrEmpty(snprice.price) ? Decimal.Zero : Convert.ToDecimal(snprice.price);
                                contextSession.SaveObject(auditStock);
                                if (autoAuditCostPriceAppIds.Contains(com.AppId))
                                {
                                    autoAuditCostPriceIds.Add(auditStock.Id);
                                }
                                LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 更新易派客商品进货价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            }
                        }

                        // 售价
                        if (item.Price != Convert.ToDecimal(snprice.snPrice))
                        {
                            // 对比审核表
                            var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
                                && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
                            if (latestAuditData == null || latestAuditData.Value != Convert.ToDecimal(snprice.snPrice))
                            {
                                count++;
                                addCount++;
                                var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改现价);
                                auditStock.AuditType = 2;
                                auditStock.JdAuditCommodityId = auditCom.Id;
                                auditStock.JdPrice = Convert.ToDecimal(snprice.snPrice);
                                contextSession.SaveObject(auditStock);
                                if (autoAuditPriceAppIds.Contains(com.AppId))
                                {
                                    autoAuditPriceIds.Add(auditStock.Id);
                                }
                                LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 更新易派客商品售价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            }
                        }
                    }

                    if (addCount > 0)
                    {
                        contextSession.SaveObject(auditCom);
                    }

                    if (count >= 200)
                    {
                        contextSession.SaveChanges();
                        count = 0;
                    }
                }


                // 处理无属性商品，并Stock表中无数据的情况
                count = 0;
                var commodityIds = allCommodityIds.Except(stockCommodityIds);
                var commodities = Commodity.ObjectSet().Where(_ => commodityIds.Contains(_.Id)
                    && (string.IsNullOrEmpty(_.ComAttribute) || _.ComAttribute == "[]")
                    && skuIds.Contains(_.JDCode)).ToList();
                foreach (var com in commodities)
                {
                    var snprice = SNPrices.Where(_ => _.skuId == com.JDCode).FirstOrDefault();
                    if (snprice == null)
                    {
                        LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage-SKU 未获取到 易派客价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
                        continue;
                    }
                    var addCount = 0;
                    var auditCom = AddCommodityAudit(com);

                    // 进货价
                    if (!string.IsNullOrEmpty(snprice.price) && com.CostPrice != Convert.ToDecimal(snprice.price))
                    {
                        // 对比审核表
                        var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
                            && _.AuditType == 1 && _.JdCostPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdCostPrice).FirstOrDefault();
                        if (latestAuditData == null || latestAuditData.Value != Convert.ToDecimal(snprice.price))
                        {
                            count++;
                            addCount++;
                            var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改进货价);
                            auditStock.AuditType = 1;
                            auditStock.JdAuditCommodityId = auditCom.Id;
                            auditStock.JdCostPrice = Convert.ToDecimal(snprice.price);
                            contextSession.SaveObject(auditStock);
                            if (autoAuditCostPriceAppIds.Contains(com.AppId))
                            {
                                autoAuditCostPriceIds.Add(auditStock.Id);
                            }
                            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 更新易派客商品进货价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
                        }
                    }

                    // 售价
                    if (com.Price != Convert.ToDecimal(snprice.snPrice))
                    {
                        // 对比审核表
                        var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityId == com.Id
                            && _.AuditType == 2 && _.JdPrice.HasValue).OrderByDescending(_ => _.SubTime).Select(_ => _.JdPrice).FirstOrDefault();
                        if (latestAuditData == null || latestAuditData.Value != Convert.ToDecimal(snprice.snPrice))
                        {
                            count++;
                            addCount++;
                            var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.京东修改现价);
                            auditStock.AuditType = 2;
                            auditStock.JdAuditCommodityId = auditCom.Id;
                            auditStock.JdPrice = Convert.ToDecimal(snprice.snPrice);
                            contextSession.SaveObject(auditStock);
                            if (autoAuditPriceAppIds.Contains(com.AppId))
                            {
                                autoAuditPriceIds.Add(auditStock.Id);
                            }
                            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 更新易派客商品售价，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
                        }
                    }

                    if (addCount > 0)
                    {
                        contextSession.SaveObject(auditCom);
                    }

                    if (count >= 200)
                    {
                        contextSession.SaveChanges();
                        count = 0;
                    }
                }

                contextSession.SaveChanges();

                // 自动审核
                var auditComFacade = new JDAuditComFacade();
                auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                if (autoAuditCostPriceIds.Count > 0)
                {
                    auditComFacade.AuditJDCostPrice(autoAuditCostPriceIds, 1, "自动审核", 0, 0);
                }
                if (autoAuditPriceIds.Count > 0)
                {
                    auditComFacade.AuditJDPrice(autoAuditPriceIds, 1, 0, "自动审核", 0);
                }

                // 删除消息
                //JDSV.DelMessage(messages.Select(_ => _.Id).ToList());
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdatePrice 异常", ex);
                throw;
            }
            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 结束同步易派客商品价格");
        }
        /// <summary>
        /// 同步商品上下架
        /// </summary>
        public static void AutoUpdateSNSkuStateByMessage()
        {
            try
            {
                LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 开始同步易派客商品上下架");
                var messages = SuningSV.suning_govbus_message_get("10");
                if (messages == null || messages.Count == 0) return;
                var delMsg = messages = messages.Where(_ => _.status == "1" || _.status == "2" || _.status == "0" || _.status == "4").ToList();
                if (messages == null || messages.Count == 0) return;
                LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 开始同步易派客商品上下架，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                //status 1上架 2下架 0 添加 4 删除
                // 0 1代表上架 2 4 代表下架
                var skuIds = messages.Where(_ => _.status == "1" || _.status == "2" || _.status == "0" || _.status == "4").Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                // 易派客商品Ids 
                var allCommodityIds = Commodity.ObjectSet().Where(_ => !_.IsDel && YPKAppIdList.Contains(_.AppId)).Select(_ => _.Id).ToList();
                List<CommodityStock> commodityStocks = new List<CommodityStock>();
                for (int i = 0; i < allCommodityIds.Count; i += 100)
                {
                    var currentCommodityIds = allCommodityIds.Skip(i).Take(100).ToList();
                    commodityStocks.AddRange(CommodityStock.ObjectSet().Where(_ => currentCommodityIds.Contains(_.CommodityId)
    && skuIds.Contains(_.JDCode)).ToList());
                }

                var stockCommodityIds = commodityStocks.Select(_ => _.CommodityId).Distinct();
                var stockCommodities = Commodity.ObjectSet().Where(_ => stockCommodityIds.Contains(_.Id)).ToList();

                List<SNSkuStateDto> snSkuStates = new List<SNSkuStateDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    snSkuStates.AddRange(SuningSV.GetSkuState(skuIds.Skip(i).Take(30).ToList()));
                }

                int count = 0;

                //List<Guid> autoAuditOnShelfIds = new List<Guid>();
                //List<Guid> autoAuditOffShelfIds = new List<Guid>();
                //var autoAuditAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
                {
                    var addCount = 0;
                    var com = stockCommodities.Where(_ => _.Id == group.Key).FirstOrDefault();
                    var auditCom = AddCommodityAudit(com);
                    foreach (var item in group)
                    {
                        var snState = snSkuStates.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                        if (snState == null)
                        {
                            LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 未获取到易派客上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            continue;
                        }
                        // 转换JD状态
                        var state = 0; // 上架
                        int auditState = 2; // 已上架
                        if (snState.state == "0") // 下架
                        {
                            state = 1;
                            auditState = 1;
                        }
                        if (item.State != state)
                        {
                            // 对比审核表
                            var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == item.Id
                                && _.AuditType == 3).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();

                            if (latestAuditState == 0 || latestAuditState != auditState)
                            {
                                count++;
                                addCount++;
                                var auditStock = AddCommodityStockAudit(contextSession, com.AppId, item, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
                                auditStock.AuditType = 3;
                                auditStock.JdAuditCommodityId = auditCom.Id;
                                auditStock.JdStatus = auditState;
                                contextSession.SaveObject(auditStock);
                                //if (autoAuditAppIds.Contains(com.AppId))
                                //{
                                //    if (auditState == 2)
                                //    {
                                //        autoAuditOnShelfIds.Add(auditStock.Id);
                                //    }
                                //    else
                                //    {
                                //        autoAuditOffShelfIds.Add(auditStock.Id);
                                //    }
                                //}
                                LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 更新易派客商品上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            }
                        }
                    }

                    if (addCount > 0)
                    {
                        contextSession.SaveObject(auditCom);
                    }

                    if (count >= 200)
                    {
                        contextSession.SaveChanges();
                        count = 0;
                    }
                }

                // 处理无属性商品，并Stock表中无数据的情况
                count = 0;
                var oldcommodityIds = allCommodityIds.Except(stockCommodityIds).ToList();

                for (int i = 0; i < oldcommodityIds.Count; i += 100)
                {
                    var commodityIds = oldcommodityIds.Skip(i).Take(100);

                    var commodities = Commodity.ObjectSet().Where(_ => commodityIds.Contains(_.Id)
                        && (string.IsNullOrEmpty(_.ComAttribute) || _.ComAttribute == "[]")
                        && skuIds.Contains(_.JDCode)).ToList();
                    foreach (var com in commodities)
                    {
                        var snState = snSkuStates.Where(_ => _.skuId == com.JDCode).FirstOrDefault();
                        if (snState == null)
                        {
                            LogHelper.Info("SNJobHelper.AutoUpdateSkuState 未获取到易派客上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
                            continue;
                        }
                        // 转换JD状态
                        var state = 0; // 上架
                        int auditState = 2; // 已上架
                        if (snState.state == "0") // 下架
                        {
                            state = 1;
                            auditState = 1;
                        }
                        // 售价
                        if (com.State != state)
                        {
                            // 对比审核表
                            var latestAuditState = JdAuditCommodityStock.ObjectSet().Where(_ => _.CommodityStockId == com.Id
                                && _.AuditType == 3).OrderByDescending(_ => _.SubTime).Select(_ => _.JdStatus).FirstOrDefault();
                            if (latestAuditState == 0 || latestAuditState != auditState)
                            {

                                var auditCom = AddCommodityAudit(com);
                                var auditStock = AddCommodityStockAudit(contextSession, com, Deploy.Enum.OperateTypeEnum.下架无货商品审核);
                                auditStock.AuditType = 3;
                                auditStock.JdAuditCommodityId = auditCom.Id;
                                auditStock.JdStatus = auditState;
                                contextSession.SaveObject(auditStock);
                                //if (auditState == 2)
                                //{
                                //    autoAuditOnShelfIds.Add(auditStock.Id);
                                //}
                                //else
                                //{
                                //    autoAuditOffShelfIds.Add(auditStock.Id);
                                //}
                                contextSession.SaveObject(auditCom);
                                count++;
                                LogHelper.Info("SNJobHelper.AutoUpdateSkuState 更新易派客商品上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
                            }
                        }

                        if (count >= 200)
                        {
                            contextSession.SaveChanges();
                            count = 0;
                        }
                    }

                    contextSession.SaveChanges();
                }

                //删除消息
                foreach (var item in delMsg)
                {
                    SuningSV.suning_govbus_message_delete(item.id);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdateSkuState 异常", ex);
                throw;
            }
            LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 结束同步Jd商品上下架");
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

    }
}
