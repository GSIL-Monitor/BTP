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
    public static class SNJobHelper
    {
        public static List<Guid> SNAppIdList;

        static bool isSyncPrice = false;
        static bool isSyncPriceByMessage = false;
        static bool isSyncSkuState = false;
        static bool isSyncSkuStateByMessage = false;
        static bool isSyncStock = false;
        static bool isSyncStockByMessage = false;
        static bool isAuditing = false;
        static bool isSyncComByMessage = false;
        static SNJobHelper()
        {
            SNAppIdList = CustomConfig.SnAppIdList;
        }

        /// <summary>
        /// 自动审核
        /// </summary>
        public static void AutoAuditSNCommodity()
        {
            try
            {
                LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始。。。");
                var auditComFacade = new JDAuditComFacade();
                auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var autoAuditCostPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.CostModeState == 0).Select(_ => _.AppId).ToList();
                var autoAuditPriceAppIds = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
                var autoAuditStockAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();

                var takeNum = 300;
                // 进价                
                foreach (var jdAppId in SNAppIdList.Intersect(autoAuditCostPriceAppIds))
                {
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核进价。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核进价。。。AppId:" + jdAppId);
                }

                // 售价               
                foreach (var jdAppId in SNAppIdList.Intersect(autoAuditPriceAppIds))
                {
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核售价。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核售价。。。AppId:" + jdAppId);
                }

                // 上下架库存               
                foreach (var jdAppId in SNAppIdList.Intersect(autoAuditStockAppIds))
                {
                    var auditQuery = AuditManage.ObjectSet().Where(_ => _.Status == 0 && _.AppId == jdAppId).Select(_ => _.Id);
                    // 上架
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核上架。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核上架。。。AppId:" + jdAppId);
                    // 下架
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核下架。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核下架。。。AppId:" + jdAppId);

                    // 有货
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核有货。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核有货。。。AppId:" + jdAppId);
                    // 无货
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 开始审核无货。。。AppId:" + jdAppId);
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
                    LogHelper.Info("SNJobHelper.AutoAuditSNCommodity 结束审核无货。。。AppId:" + jdAppId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoAuditSNCommodity 异常", ex);
            }
            finally
            {
                LogHelper.Info("SNJobHelper.AutoAuditSNCommodity End。。。");
            }
        }

        /// <summary>
        /// 同步商品价格
        /// </summary>
        public static void AutoUpdatePriceByMessage()
        {
            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 开始同步苏宁易购商品价格");
            try
            {
                var messages = SuningSV.GetPriceMessage();
                LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 开始同步苏宁易购商品价格，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                if (messages == null || messages.Count == 0) return;
                var skuIds = messages.Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                //商品价格
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(skuIds.Skip(i).Take(30).ToList()));
                }
                AutoCommodityHelper.UpdateCommodityPrice(skuIds, SNPrices, Deploy.Enum.ThirdECommerceTypeEnum.SuNingYiGou);
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdatePrice 异常", ex);
            }
            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 结束同步苏宁易购商品价格");
        }

        /// <summary>
        /// 全量同步商品上下架
        /// </summary>
        public static void AutoUpdateSNSkuState()
        {
            try
            {
                if (isSyncSkuState)
                {
                    LogHelper.Info("SNJobHelper.AutoUpdateJdSkuState 正在同步苏宁易购商品上下架，跳过。。。");
                    return;
                }
                isSyncSkuState = true;

                // 查询商品
                int n;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                bool hasData = true;
                int pageIndex = 0;
                foreach (var jdAppId in SNAppIdList)
                {
                    hasData = true;
                    pageIndex = 0;
                    while (hasData)
                    {
                        var commodities = Commodity.ObjectSet().Where(_ => !_.IsDel && _.AppId == jdAppId && !string.IsNullOrEmpty(_.JDCode)).ToList();
                        hasData = commodities.Count > 0;
                        pageIndex++;
                        if (hasData)
                        {
                            List<string> skuIds = commodities.Where(_ => _.JDCode.StartsWith("J_") || int.TryParse(_.JDCode, out n)).Select(_ => _.JDCode).Distinct().ToList();
                            var commodityIds = commodities.Select(_ => _.Id);
                            var commodityStocks = CommodityStock.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId) && !string.IsNullOrEmpty(_.JDCode)).ToList();
                            skuIds.AddRange(commodityStocks.Select(_ => _.JDCode).Where(_ => _.StartsWith("J_") || int.TryParse(_, out n)).Distinct());
                            skuIds = skuIds.Distinct().ToList();
                            List<SNSkuStateDto> snSkuStates = new List<SNSkuStateDto>();
                            for (int i = 0; i < skuIds.Count; i += 30)
                            {
                                snSkuStates.AddRange(SuningSV.GetSkuState(skuIds.Skip(i).Take(30).ToList()));
                            }

                            int count = 0;

                            //List<Guid> autoAuditOnShelfIds = new List<Guid>();
                            //List<Guid> autoAuditOffShelfIds = new List<Guid>();
                            //var autoAuditAppIds = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();

                            //Parallel.ForEach(commodityStocks.GroupBy(_ => _.CommodityId), group =>
                            foreach (var group in commodityStocks.GroupBy(_ => _.CommodityId))
                            {
                                var addCount = 0;
                                var com = commodities.Where(_ => _.Id == group.Key).FirstOrDefault();
                                var auditCom = AddCommodityAudit(com);
                                foreach (var item in group)
                                {
                                    var snState = snSkuStates.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                                    if (snState == null)
                                    {
                                        LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 未获取到苏宁易购上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
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
                                            LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 更新苏宁易购商品上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
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
                            foreach (var com in commodities.Where(c =>
                                (c.JDCode.StartsWith("J_") || int.TryParse(c.JDCode, out n)) &&
                                (string.IsNullOrEmpty(c.ComAttribute) || c.ComAttribute == "[]") &&
                                !commodityStocks.Any(s => s.CommodityId == c.Id)))
                            {
                                var snState = snSkuStates.Where(_ => _.skuId == com.JDCode).FirstOrDefault();
                                if (snState == null)
                                {
                                    LogHelper.Info("SNJobHelper.AutoUpdateSkuState 未获取到苏宁易购上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
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
                                        LogHelper.Info("SNJobHelper.AutoUpdateSkuState 更新苏宁易购商品上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
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
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdateSkuState 异常", ex);
                isSyncSkuState = false;
                throw;
            }
            isSyncSkuState = false;
        }

        /// <summary>
        /// 同步商品上下架
        /// </summary>
        public static void AutoUpdateSNSkuStateByMessage()
        {
            try
            {
                LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 开始同步苏宁易购商品上下架");
                var messages = SuningSV.suning_govbus_message_get("10");
                messages = null;
                if (messages == null || messages.Count == 0) return;
                var delMsg = messages = messages.Where(_ => _.status == "1" || _.status == "2" || _.status == "0" || _.status == "4").ToList();
                if (messages == null || messages.Count == 0) return;
                LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 开始同步苏宁易购商品上下架，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                //status 1上架 2下架 0 添加 4 删除
                // 0 1代表上架 2 4 代表下架
                var skuIds = messages.Where(_ => _.status == "1" || _.status == "2" || _.status == "0" || _.status == "4").Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                // 苏宁易购商品Ids 
                var allCommodityIds = Commodity.ObjectSet().Where(_ => !_.IsDel && SNAppIdList.Contains(_.AppId)).Select(_ => _.Id).ToList();
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
                            LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 未获取到苏宁易购上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
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
                                LogHelper.Info("SNJobHelper.AutoUpdateSkuState-SKU 更新苏宁易购商品上下架状态，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
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
                            LogHelper.Info("SNJobHelper.AutoUpdateSkuState 未获取到苏宁易购上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
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
                                LogHelper.Info("SNJobHelper.AutoUpdateSkuState 更新苏宁易购商品上下架状态，商品Id: " + com.Id + "，SkuId: " + com.JDCode);
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
                isSyncSkuStateByMessage = false;
                throw;
            }
            LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 结束同步Jd商品上下架");
            isSyncSkuStateByMessage = false;
        }

        /// <summary>
        /// 同步商品变更
        /// </summary>
        public static void AutoUpdateSNCommodityByMessage()
        {
            try
            {
                LogHelper.Info("SNJobHelper.AutoUpdateSNCommodityByMessage 开始同步苏宁易购商品变更");
                var messages = SuningSV.suning_govbus_message_get("10");
                messages = null;
                if (messages == null || messages.Count == 0) return;
                var delMsg = messages = messages.Where(_ => _.status == "3" || _.status == "4").ToList();
                if (messages == null || messages.Count == 0) return;
                LogHelper.Info("SNJobHelper.AutoUpdateJdSkuStateByMessage 开始同步苏宁易购商品变更，获取结果如下：" + JsonHelper.JsonSerializer(messages));
                //status 3修改 4删除
                var UpdSkuIds = messages.Where(_ => _.status == "3").Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                var DelSkuIds = messages.Where(_ => _.status == "4").Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                // 苏宁易购商品Ids 
                var allCommodityIds = Commodity.ObjectSet().Where(_ => !_.IsDel && SNAppIdList.Contains(_.AppId)).Select(_ => _.Id).ToList();
                //要删除的商品
                var DelComSkuIds = allCommodityIds.Where(_ => DelSkuIds.Contains(_.ToString())).ToList();
                CommodityFacade cf = new CommodityFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO delResult = cf.DeleteCommoditys(DelComSkuIds);
                if (!delResult.isSuccess)
                {
                    LogHelper.Error("SNJobHelper.AutoUpdateSNCommodityByMessage 苏宁易购商品变更失败", delResult.Message);
                }
                //要修改的商品
                var UpdComSkuIds = allCommodityIds.Where(_ => UpdSkuIds.Contains(_.ToString())).Select(_ => _.ToString()).ToList();
                List<SNComPicturesDto> SNComPics = new List<SNComPicturesDto>();
                for (int i = 0; i < UpdComSkuIds.Count; i += 30)
                {
                    SNComPics.AddRange(SuningSV.GetComPictures(UpdComSkuIds.Skip(i).Take(30).ToList()));
                }
                //商品价格
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < UpdComSkuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(UpdComSkuIds.Skip(i).Take(30).ToList()));
                }
                //商品扩展信息
                List<SNComExtendDto> SNComExtendList = new List<SNComExtendDto>();
                SNComExtendList = SuningSV.GetComExtend(SNPrices);
                //获取苏宁易购商品详情
                List<SNComDetailDto> SNComDetailList = new List<SNComDetailDto>();
                foreach (var item in UpdComSkuIds)
                {
                    var SNComDetailInfo = SuningSV.GetSNDetail(item);
                    if (string.IsNullOrEmpty(SNComDetailInfo.name))
                    {
                        LogHelper.Error("SNJobHelper.AutoUpdateSNCommodityByMessage 苏宁易购变更商品详情为空skuId", item.ToString());
                        continue;
                    }
                    SNComDetailList.Add(SNComDetailInfo);
                }
                var UpdCommodity = Commodity.ObjectSet().Where(_ => !_.IsDel && UpdComSkuIds.Contains(_.JDCode)).ToList();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var item in UpdCommodity)
                {
                    var SNComPic = SNComPics.Where(p => p.skuId == item.JDCode);
                    var SNComDetailInfo = SNComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();
                    var snPrice = SNPrices.Where(p => p.skuId == item.JDCode).FirstOrDefault();
                    Commodity Com = new Commodity();
                    //售后信息
                    var SNComExtendInfo = SNComExtendList.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                    if (SNComExtendInfo != null)
                    {
                        if (SNComExtendInfo.returnGoods == "01")
                        {
                            Com.IsAssurance = true;
                            Com.IsReturns = false;
                            Com.Isnsupport = false;
                        }
                        if (SNComExtendInfo.returnGoods == "02")
                        {
                            Com.IsAssurance = false;
                            Com.IsReturns = true;
                            Com.Isnsupport = true;
                        }
                    }
                    Com.Id = item.Id;
                    Com.Name = "苏宁易购 " + SNComDetailInfo.name;
                    Com.Price = string.IsNullOrEmpty(snPrice.price) ? Decimal.Zero : Convert.ToDecimal(snPrice.price);
                    Com.PicturesPath = SNComPic.FirstOrDefault(p => p.primary == "1").path;
                    if (SNComDetailInfo.introduction != null && SNComDetailInfo.introduction != "")
                    {
                        string Div = SNComDetailInfo.introduction.Substring(0, 10);
                        if (Div.Contains("<div"))
                        {
                            Com.Description = "<div class=\"JD-goods\">" + SNComDetailInfo.introduction + "</div>";
                        }
                        else
                        {
                            Com.Description = SNComDetailInfo.introduction;
                        }
                    }
                    else
                    {
                        Com.Description = string.Empty;
                    }
                    //Com.State = 1; 只更新商品信息,不更新商品上下架状态
                    Com.IsDel = false;
                    Com.ModifiedOn = DateTime.Now;
                    Com.Weight = string.IsNullOrEmpty(SNComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(SNComDetailInfo.weight);
                    Com.Unit = string.IsNullOrEmpty(SNComDetailInfo.saleUnit) ? "件" : SNComDetailInfo.saleUnit;
                    Com.JDCode = SNComDetailInfo.skuId;
                    Com.CostPrice = Com.Price;
                    Com.TechSpecs = string.Empty;// JdComDetailInfo.prodParams; 
                    Com.Type = 0;
                    Com.EntityState = EntityState.Modified;
                    contextSession.SaveObject(Com);
                    Com.RefreshCache(EntityState.Modified);
                    //更新库存表
                    UpdateCommodityStock(Com, contextSession);
                    int count1 = contextSession.SaveChanges();
                    #region 商品图片
                    //删除图片
                    ProductDetailsPictureFacade pdpbp = new ProductDetailsPictureFacade();
                    pdpbp.DeletePicture(item.Id);
                    //添加图片
                    int sort = 1;
                    foreach (var itempic in SNComPic)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = Guid.NewGuid();
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = itempic.path;
                        pic.CommodityId = Com.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);
                        sort++;
                    }
                    int count = contextSession.SaveChange();
                    #endregion
                }
                //删除消息
                foreach (var item in delMsg)
                {
                    SuningSV.suning_govbus_message_delete(item.id);
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 根据消息池同步商品上下架，信息变更
        /// </summary>
        public static void AutoUpdateSNByMessage()
        {
            var messages = SuningSV.suning_govbus_message_get("10");
            LogHelper.Info("SNJobHelper.AutoUpdateSNByMessage 开始同步苏宁易购商品上下架、信息变更，获取结果如下：" + JsonHelper.JsonSerializer(messages));
            try
            {
                if (messages == null || messages.Count == 0) return;
                var skuIds = messages.Select(_ => _.cmmdtyCode).Where(_ => !string.IsNullOrEmpty(_)).Distinct().ToList();
                //上下架状态
                List<SNSkuStateDto> SNSkuStates = new List<SNSkuStateDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    SNSkuStates.AddRange(SuningSV.GetSkuState(skuIds.Skip(i).Take(30).ToList()));
                }
                var onSalesSkuIds = SNSkuStates.Where(s => s.state == "1").Select(s => s.skuId).ToList();
                var offSalesSkuIds = SNSkuStates.Where(s => s.state == "0").Select(s => s.skuId).ToList();
                //商品价格
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < onSalesSkuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(onSalesSkuIds.Skip(i).Take(30).ToList()));
                }
                var validSkuid = SNPrices.Where(p => !string.IsNullOrEmpty(p.price) && !string.IsNullOrEmpty(p.snPrice)
                                                    && p.price != "0" && p.snPrice != "0").Select(p => p.skuId).Distinct().ToList();
                AutoCommodityHelper.UpdateCommodityPrice(onSalesSkuIds, SNPrices, Deploy.Enum.ThirdECommerceTypeEnum.SuNingYiGou);
                AutoCommodityHelper.OnSalesCommodity(validSkuid, Deploy.Enum.ThirdECommerceTypeEnum.SuNingYiGou);
                AutoCommodityHelper.OffSalesCommodity(offSalesSkuIds, Deploy.Enum.ThirdECommerceTypeEnum.SuNingYiGou);
                var updateSkuids = messages.Where(_ => _.status == "3").Select(_ => _.cmmdtyCode).Distinct().ToList();
                AutoUpdateSNCommodity(updateSkuids);
                //删除消息
                foreach (var item in messages)
                {
                    SuningSV.suning_govbus_message_delete(item.id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdateSNByMessage 同步苏宁易购商品上下架、信息变更异常", ex);
            }
        }
        /// <summary>
        /// 同步商品信息变更
        /// </summary>
        public static void AutoUpdateSNCommodity(List<string> skuIds)
        {
            try
            {
                LogHelper.Info("SNJobHelper.AutoUpdateSNByMessage 开始同步商品信息变更，获取skuId如下：" + JsonHelper.JsonSerializer(skuIds));
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                // 要修改的商品 
                var UpdCommodity = Commodity.ObjectSet().Where(_ => _.State == 0 && !_.IsDel && SNAppIdList.Contains(_.AppId) && skuIds.Contains(_.JDCode)).ToList();
                //要修改的商品skuId
                var UpdComSkuIds = UpdCommodity.Select(_ => _.JDCode).ToList();
                //商品图片
                List<SNComPicturesDto> SNComPics = new List<SNComPicturesDto>();
                for (int i = 0; i < UpdComSkuIds.Count; i += 30)
                {
                    SNComPics.AddRange(SuningSV.GetComPictures(UpdComSkuIds.Skip(i).Take(30).ToList()));
                }
                //商品价格
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < UpdComSkuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(UpdComSkuIds.Skip(i).Take(30).ToList()));
                }
                //商品扩展信息
                List<SNComExtendDto> SNComExtendList = new List<SNComExtendDto>();
                for (int i = 0; i < SNPrices.Count; i += 30)
                {
                    SNComExtendList = SuningSV.GetComExtend(SNPrices.Skip(i).Take(30).ToList());
                }
                //获取苏宁易购商品详情
                List<SNComDetailDto> SNComDetailList = new List<SNComDetailDto>();
                foreach (var item in UpdComSkuIds)
                {
                    var SNComDetailInfo = SuningSV.GetSNDetail(item);
                    if (string.IsNullOrEmpty(SNComDetailInfo.name))
                    {
                        LogHelper.Error("SNJobHelper.AutoUpdateSNCommodity 苏宁易购变更商品详情为空skuId", item.ToString());
                        continue;
                    }
                    SNComDetailList.Add(SNComDetailInfo);
                }
                UpdCommodity.ForEach(item =>
                {
                    var priceDto = SNPrices.Where(p => p.skuId == item.JDCode).FirstOrDefault();
                    var price = Decimal.Zero;
                    var snPrice = Decimal.Zero;
                    if (priceDto != null)
                    {
                        if (!string.IsNullOrEmpty(priceDto.price))
                        {
                            price = Convert.ToDecimal(priceDto.price);
                        }
                        if (!string.IsNullOrEmpty(priceDto.snPrice))
                        {
                            snPrice = Convert.ToDecimal(priceDto.snPrice);
                        }
                    }
                    if (priceDto == null)
                    {
                        LogHelper.Info("未获取到商品价格信息，商品下架：CommodityId=" + item.Id);
                        item.State = 1;
                        item.ModifiedOn = DateTime.Now;
                        return;
                    }
                    if (price <= 0 || snPrice <= 0)
                    {
                        LogHelper.Info("进价或售价为0，商品下架：CommodityId=" + item.Id);
                        item.State = 1;
                        item.ModifiedOn = DateTime.Now;
                        return;
                    }
                    var SNComPic = SNComPics.Where(p => p.skuId == item.JDCode);
                    var SNComDetailInfo = SNComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();
                    //售后信息
                    var SNComExtendInfo = SNComExtendList.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                    if (SNComExtendInfo != null)
                    {
                        if (SNComExtendInfo.returnGoods == "01")
                        {
                            item.IsAssurance = true;
                            item.IsReturns = false;
                            item.Isnsupport = false;
                        }
                        if (SNComExtendInfo.returnGoods == "02")
                        {
                            item.IsAssurance = false;
                            item.IsReturns = true;
                            item.Isnsupport = true;
                        }
                    }
                    item.Name = SNComDetailInfo.name.Replace("苏宁", "").Replace("超市", "").Replace("易购", "").Replace("自营", "").Replace("【", "").Replace("】", "").Replace("[", "").Replace("]", "");
                    item.Price = snPrice;
                    item.CostPrice = price;
                    item.PicturesPath = SNComPic.FirstOrDefault(p => p.primary == "1").path;
                    if (SNComDetailInfo.introduction != null && SNComDetailInfo.introduction != "")
                    {
                        string Div = SNComDetailInfo.introduction.Substring(0, 10);
                        if (Div.Contains("<div"))
                        {
                            item.Description = "<div class=\"JD-goods\">" + SNComDetailInfo.introduction + "</div>";
                        }
                        else
                        {
                            item.Description = SNComDetailInfo.introduction;
                        }
                    }
                    else
                    {
                        item.Description = string.Empty;
                    }
                    item.ModifiedOn = DateTime.Now;
                    item.Weight = string.IsNullOrEmpty(SNComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(SNComDetailInfo.weight);
                    item.Unit = string.IsNullOrEmpty(SNComDetailInfo.saleUnit) ? "件" : SNComDetailInfo.saleUnit;
                    item.RefreshCache(EntityState.Modified);
                    #region 更新库存
                    var cs = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == item.Id);
                    cs.ComAttribute = "[]";
                    cs.Price = item.Price;
                    cs.MarketPrice = item.MarketPrice;
                    cs.Stock = item.Stock;
                    cs.Duty = item.Duty;
                    cs.Barcode = item.Barcode;
                    cs.No_Code = item.No_Code;
                    cs.JDCode = item.JDCode;
                    cs.ErQiCode = item.ErQiCode;
                    cs.CostPrice = item.CostPrice;
                    #endregion
                    #region 商品图片
                    //删除图片
                    ProductDetailsPictureFacade pdpbp = new ProductDetailsPictureFacade();
                    pdpbp.DeletePicture(item.Id);
                    //添加图片
                    int sort = 1;
                    foreach (var itempic in SNComPic)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = Guid.NewGuid();
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = itempic.path;
                        pic.CommodityId = item.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);
                        sort++;
                    }
                    #endregion
                });
                int count = contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNJobHelper.AutoUpdateSNCommodity 苏宁易购变更商品异常", ex.Message);
            }
        }
        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        private static void UpdateCommodityStock(Commodity item, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                var cs = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == item.Id);
                cs.ComAttribute = "[]";
                cs.Price = item.Price;
                cs.MarketPrice = item.MarketPrice;
                cs.Stock = item.Stock;
                cs.Duty = item.Duty;
                cs.Barcode = item.Barcode;
                cs.No_Code = item.No_Code;
                cs.JDCode = item.JDCode;
                cs.ErQiCode = item.ErQiCode;
                cs.CostPrice = item.CostPrice;
                if (isUpdate == true)
                {
                    cs.EntityState = EntityState.Modified;
                }
                cs.EntityState = EntityState.Modified;
                contextSession.SaveObject(cs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNJobHelper.UpdateCommodityStock 更新苏宁易购库存异常。"), ex);
            }
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
        /// <summary>
        /// 全量更新苏宁商品税率
        /// </summary>
        public static void AutoUpdateSNTax()
        {
            LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 开始同步苏宁易购商品税率");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid appId = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                var SNComList = Commodity.ObjectSet().Where(p => p.AppId == appId && p.IsDel == false).OrderBy(p => p.SubTime).ToList();
                int count = 0;
                for (int j = 0; j < SNComList.Count; j += 1000)
                {
                    List<Commodity> SNCom = new List<Commodity>();
                    SNCom = SNComList.Skip(j).Take(1000).ToList();
                    var JdCodeList = SNCom.Select(s => s.JDCode).ToList();
                    List<SNPriceDto> jdPrices = new List<SNPriceDto>();
                    for (int i = 0; i < JdCodeList.Count; i += 30)
                    {
                        jdPrices.AddRange(SuningSV.GetPrice(JdCodeList.Skip(i).Take(30).ToList()));
                    }
                    for (int i = 0; i < SNCom.Count; i += 100)
                    {
                        var SnComList = SNCom.Skip(i).Take(100).ToList();//取出商品id
                        SNComList.ForEach(s =>
                        {
                            var jdPrice = jdPrices.Where(p => p.skuId == s.JDCode).FirstOrDefault();
                            if (jdPrice != null && !string.IsNullOrEmpty(jdPrice.tax))
                            {
                                var tax = Convert.ToDecimal(jdPrice.tax) * 100;
                                if (s.TaxRate != tax || s.InputRax != tax)
                                {
                                    s.TaxRate = tax;
                                    s.InputRax = tax;
                                    s.ModifiedOn = DateTime.Now;
                                }
                            }
                        });
                        count += contextSession.SaveChanges();
                    }
                }
                LogHelper.Info(string.Format("更新苏宁易购税率条数:{0}", count));
            }
            catch (Exception ex)
            {
                LogHelper.Info("SNJobHelper.AutoUpdatePriceByMessage 同步苏宁易购商品税率异常", ex.Message);
            }
        }
    }
}
