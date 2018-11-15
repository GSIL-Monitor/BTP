using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 严选
    /// </summary>
    public static class YXCommodityHelper
    {
        private static List<Guid> YXAppIdList;

        static bool isSyncSPU = false;
        static bool SyncStock = false;
        static bool isSyncYXComInfo = false;
        static bool isSyncStockByMessage = false;
        static string skuCheckStr = "";
        /// <summary>
        /// 全量获取严选SPU
        /// </summary>
        public static void AutoGetAllSPU()
        {
            if (isSyncSPU)
            {
                LogHelper.Info("JdOrderHelper.AutoGetAllSPU 正在全量获取严选SPU，跳过。。。");
                return;
            }
            isSyncSPU = true;
            LogHelper.Info("YXJobHelper.AutoGetAllSPU 开始写入严选SPU");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //全量取出SPU
                var SPUlist = YXSV.GetAllSPU();
                if (!SPUlist.Any())
                {
                    LogHelper.Info("JdOrderHelper.AutoGetAllSPU 未获取到严选SPU,跳出~");
                    return;
                }
                //清除表中全部数据
                if (SPUlist.Any())
                {
                    YXComInfo.ObjectSet().Context.ExecuteStoreCommand("DELETE FROM YXComInfo");
                }
                foreach (var item in SPUlist)
                {
                    YXComInfo YXCom = YXComInfo.CreateYXComInfo();
                    YXCom.Id = Guid.NewGuid();
                    YXCom.SPU = item;
                    YXCom.SubTime = DateTime.Now;
                    contextSession.SaveObject(YXCom);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoGetAllSPU 异常", ex);
                isSyncSPU = false;
                throw;
            }
            LogHelper.Info("YXJobHelper.AutoGetAllSPU 全量获取严选商品信息");
            isSyncSPU = false;
        }

        /// <summary>
        /// 全量同步严选库存信息
        /// </summary>
        public static Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AutoSyncAllStockNum()
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new Deploy.CustomDTO.ResultDTO() { isSuccess = false, ResultCode = 1 };
            LogHelper.Info("YXJobHelper.AutoSyncAllStockNum 开始写入严选库存信息");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid AppId = new Guid("1d769e14-f870-4b19-82ab-875a9e8678e4");
                //取出库存表中所有的skuid
                var stockList = (from m in Commodity.ObjectSet()
                                 join n in CommodityStock.ObjectSet() on m.Id equals n.CommodityId
                                 where m.AppId == AppId &&
                                        m.IsDel == false && n.IsDel == false
                                 select n).ToList();
                //取出所有SkuID
                var SkuidList = stockList.OrderBy(p => p.SubTime).Select(s => s.JDCode).ToList();
                List<StockDTO> YXstockList = new List<StockDTO>();
                for (int i = 0; i < SkuidList.Count; i += 99)
                {
                    LogHelper.Info(string.Format("严选sku:{0}", JsonHelper.JsonSerializer(SkuidList.Skip(i).Take(99).ToList())));
                    YXstockList.AddRange(YXSV.GetStockNum(SkuidList.Skip(i).Take(99).ToList()));
                    Thread.Sleep(1000);
                }
                if (!YXstockList.Any())
                {
                    LogHelper.Info("YXJobHelper.AutoSyncAllStockNum 未获取到严选库存信息,跳出~");
                    return result;
                }
                //更新库存
                foreach (var item in stockList)
                {
                    var YXStock = YXstockList.FirstOrDefault(p => p.skuId == item.JDCode);
                    if (YXStock != null)
                    {
                        item.Stock = YXStock.inventory;
                        item.State = 0;
                        item.IsDel = false;
                    }
                    else
                    {
                        item.Stock = 0;
                        item.State = 1;
                        item.IsDel = true;
                    }
                    item.ModifiedOn = DateTime.Now;
                }
                int countstock = contextSession.SaveChanges();
                LogHelper.Info(string.Format("严选库存更新库存保存条数:{0}", countstock));
                #region 重新计算Commodity表库存信息
                //出去所有严选商品信息
                var commodity = Commodity.ObjectSet().Where(p => CustomConfig.YxAppIdList.Contains(p.AppId) && p.IsDel == false).ToList();
                var comids = commodity.Select(s => s.Id).ToList();
                var commodityStock = CommodityStock.ObjectSet().Where(p => comids.Contains(p.CommodityId)).ToList();
                //var AuditModeApp = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();//库存自动审核appid集合
                //到货提醒商品Id集合
                List<Guid> NoticeComIds = new List<Guid>();
                foreach (var item in commodity)
                {
                    var StockNo = commodityStock.Where(p => p.CommodityId == item.Id).Sum(s => s.Stock);
                    if (StockNo == 0)//无库存商品下架处理
                    {
                        item.State = 1;
                    }
                    else if (item.State == 1 && StockNo > 0)//有库存上架处理审核处理
                    {
                        item.State = 0;
                    }
                    if (item.Stock == 0 && StockNo > 0)
                    {
                        NoticeComIds.Add(item.Id);
                    }
                    item.Stock = StockNo;
                    item.ModifiedOn = DateTime.Now;
                }
                int countCom = contextSession.SaveChanges();
                LogHelper.Info(string.Format("严选库存更新库存保存条数:{0}", countCom));
                result.isSuccess = true;
                result.ResultCode = 0;
                result.Message = "库存表保存条数:{0}" + countstock + "商品表保存条数:{1}" + countCom;
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoSyncAllStockNum 异常", ex);
                return result;
            }
        }
        /// <summary>
        /// 全量同步严选库存信息
        /// </summary>
        public static List<StockDTO> GetYXStockById(List<string> SkuidList)
        {     //取出所有SkuID 
            List<StockDTO> YXstockList = new List<StockDTO>();
            for (int i = 0; i < SkuidList.Count; i += 99)
            {
                YXstockList.AddRange(YXSV.GetStockNum(SkuidList.Skip(i).Take(99).ToList()));
                //Thread.Sleep(1000);
            }
            return YXstockList;
        }
        /// <summary>
        /// 获取网易严选商品详情
        /// </summary>
        public static List<YXComDetailDTO> GetYXComInfo(List<string> SkuList)
        {     //取出所有SkuID 
            List<YXComDetailDTO> YXList = new List<YXComDetailDTO>();
            for (int i = 0; i < SkuList.Count; i += 99)
            {
                YXList.AddRange(YXSV.GetComDetailList(SkuList.Skip(i).Take(99).ToList()));
                //Thread.Sleep(1000);
            }
            return YXList;
        }

        #region 回调

        /// <summary>
        /// 渠道SKU库存校准回调
        /// </summary>
        /// <param name="sku"></param>
        public static void SkuStockCheck(skuCheck sku)
        {
            if (skuCheckStr == sku.id)
            {
                LogHelper.Info("JdOrderHelper.SkuStockCheck 正在执行渠道SKU库存校准回调，跳过。。。");
                return;
            }
            skuCheckStr = sku.id;
            LogHelper.Info("YXJobHelper.SkuStockCheck 开始执行渠道SKU库存校准回调");
            try
            {
                SkuCheckStock(sku.skuChecks);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoSyncAllStockNum 异常", ex);
                skuCheckStr = "";
                throw;
            }
            LogHelper.Info("YXJobHelper.AutoSyncAllStockNum 全量更新严选库存信息成功");
            skuCheckStr = "";
        }

        /// <summary>
        /// 渠道SKU低库存预警通知
        /// </summary>
        /// <param name="sku"></param>
        public static void SkuStockAlarm(List<SkuCloseAlarmVO> sku)
        {
            if (!sku.Any())
            {
                LogHelper.Info("JdOrderHelper.SkuStockAlarm 不存在返回的库存信息，跳过。。。");
                return;
            }
            LogHelper.Info("YXJobHelper.SkuStockAlarm 开始执行渠道SKU低库存预警通知");
            try
            {
                List<SkuCheck> StockList = new List<SkuCheck>();
                foreach (var item in sku)
                {
                    SkuCheck stock = new SkuCheck();
                    stock.skuId = item.skuId;
                    stock.count = 0;
                    StockList.Add(stock);
                }
                SkuCheckStock(StockList);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.SkuStockAlarm 异常", ex);
                throw;
            }
            LogHelper.Info("YXJobHelper.SkuStockAlarm 执行渠道SKU低库存预警通知执行成功");
        }
        /// <summary>
        /// 渠道SKU再次开售通知
        /// </summary>
        /// <param name="sku"></param>
        public static void SkuStockReopen(List<SkuReopenVO> sku)
        {
            if (!sku.Any())
            {
                LogHelper.Info("JdOrderHelper.SkuStockReopen 不存在返回的库存信息，跳过。。。");
                return;
            }
            LogHelper.Info("YXJobHelper.SkuStockReopen 开始执行渠道SKU再次开售通知");
            try
            {
                List<SkuCheck> StockList = new List<SkuCheck>();
                foreach (var item in sku)
                {
                    SkuCheck stock = new SkuCheck();
                    stock.skuId = item.skuId;
                    stock.count = item.inventory;
                    StockList.Add(stock);
                }
                SkuCheckStock(StockList);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.SkuStockReopen 异常", ex);
                throw;
            }
            LogHelper.Info("YXJobHelper.SkuStockReopen 渠道SKU再次开售通知执行成功");
        }

        #endregion
        /// <summary>
        /// 严选库存回调执行方法
        /// </summary>
        /// <param name="sku"></param>
        public static void SkuCheckStock(List<SkuCheck> sku)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var SkuList = sku.Select(s => s.skuId).ToList();
            //取出库存表中所有的skuid
            var stockList = (from m in Commodity.ObjectSet()
                             join n in CommodityStock.ObjectSet() on m.Id equals n.CommodityId
                             where CustomConfig.YxAppIdList.Contains(m.AppId) &&n.IsDel==false&&
                                    m.IsDel == false && SkuList.Contains(n.JDCode)
                             select n).ToList();
            if (!stockList.Any())
            {
                LogHelper.Info("JdOrderHelper.SkuCheckStock 店铺中未找到商品，跳过~");
                return;
            }
            List<Guid> ComIds = new List<Guid>();
            //更新库存
            foreach (var item in sku)
            {
                var ComStock = stockList.FirstOrDefault(p => p.JDCode == item.skuId);
                if (ComStock != null)
                {
                    ComStock.Stock = item.count;
                    ComStock.ModifiedOn = DateTime.Now;
                    ComIds.Add(ComStock.CommodityId);
                }
            }
            int mmm = contextSession.SaveChanges();
            #region 重新计算Commodity表库存信息
            //出去所有严选商品信息
            var commodity = Commodity.ObjectSet().Where(p => ComIds.Distinct().Contains(p.Id) && p.IsDel == false).ToList();
            var comids = commodity.Select(s => s.Id).ToList();
            var commodityStock = CommodityStock.ObjectSet().Where(p => comids.Contains(p.CommodityId)).ToList();
            var AuditModeApp = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();//库存自动审核appid集合
            List<Guid> NoticeComIds = new List<Guid>();//到货提醒商品Id
            foreach (var item in commodity)
            {
                var StockNo = commodityStock.Where(p => p.CommodityId == item.Id).Sum(s => s.Stock);
                if (StockNo == 0)//无库存商品下架处理
                {
                    item.State = 1;
                }
                else if (item.State == 1 && StockNo > 0)//有库存上架处理审核处理
                {
                    item.State = 0;
                }
                if (item.Stock == 0 && StockNo > 0 && item.State == 0)
                {
                    NoticeComIds.Add(item.Id);
                }
                item.Stock = StockNo;
                item.ModifiedOn = DateTime.Now;
            }
            int nnn = contextSession.SaveChanges();
            //调用到货提醒接口
            if (NoticeComIds.Any())
            {
                for (int i = 0; i < NoticeComIds.Count; i += 30)
                {
                    ZPHSV.SendStockNotifications(NoticeComIds.Skip(i).Take(30).ToList());
                }
            }
            #endregion
        }

        /// <summary>
        /// 全量获取严选价格信息
        /// </summary>
        public static void AutoUpdateYXComInfo()
        {
            if (isSyncYXComInfo)
            {
                LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 正在全量获取严选价格信息，跳过。。。");
                return;
            }
            isSyncYXComInfo = true;
            LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 开始写入严选价格信息");
            try
            {
                AutoGetAllSPU();
                var YXComList = YXComInfo.ObjectSet().ToList();
                var UpdateSpuList = YXComList.Select(s => s.SPU).ToList();
                List<YXComDetailDTO> YXComLists = new List<YXComDetailDTO>();
                for (int i = 0; i < YXComList.Count; i += 30)
                {
                    YXComLists.AddRange(YXSV.GetComDetailList(UpdateSpuList.Skip(i).Take(30).ToList()));
                    //Thread.Sleep(1000);
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<string> SkuIds = new List<string>();
                foreach (var YXComlist in YXComLists)
                {
                    foreach (var item in YXComlist.skuList)
                    {
                        YXComInfo YXCom = YXComInfo.CreateYXComInfo();
                        YXCom.Id = Guid.NewGuid();
                        YXCom.SPU = YXComlist.id;
                        YXCom.SKU = item.id;
                        YXCom.Price = item.channelPrice;
                        YXCom.CostPrice = item.channelPrice * Convert.ToDecimal(0.8);
                        YXCom.SubTime = DateTime.Now;
                        contextSession.SaveObject(YXCom);
                        SkuIds.Add(item.id);
                    }
                }                
                int count1 = contextSession.SaveChanges();
                YXComInfo.ObjectSet().Context.ExecuteStoreCommand("delete from YXComInfo where SKU is null");
                #region
                //取出定时改价中严选商品               
                List<Guid> YXappids = CustomConfig.YxAppIdList;
                var timingComIds = Jinher.AMP.BTP.TPS.YJBSV.GetYXChangeComInfo(YXappids).Data.Select(s => s.CommodityId).ToList();
                //最新价格更新到YXComInfo,对比库存表和价格审核表 判断是否需要审核(过滤掉定时改价中严选的商品)
                var ComStockList = CommodityStock.ObjectSet().Where(p => SkuIds.Contains(p.JDCode) && !timingComIds.Contains(p.Id)).ToList();
                var ComIds = ComStockList.Select(s => s.CommodityId).Distinct().ToList();
                var ComList = Commodity.ObjectSet().Where(p => ComIds.Contains(p.Id)).ToList();
                var YXComInfoList = YXComInfo.ObjectSet().Where(p => SkuIds.Contains(p.SKU)).ToList();
                var AuditAuditModeApp = JDAuditMode.ObjectSet().Where(_ => _.PriceModeState == 0).Select(_ => _.AppId).ToList();
                List<Guid> autoAuditPriceIds = new List<Guid>();
                int count = 0;
                foreach (var group in ComList)
                {
                    int addCount = 0;
                    var Com = ComList.FirstOrDefault(p => p.Id == group.Id);
                    var auditCom = AddCommodityAudit(Com);

                    var ComStocks = ComStockList.Where(p => p.CommodityId == group.Id).ToList();
                    foreach (var item in ComStocks)
                    {
                        var YXComNew = YXComInfoList.FirstOrDefault(p => p.SKU == item.JDCode);
                        if (YXComNew != null && YXComNew.Price.HasValue && item.Price != YXComNew.Price)
                        {
                            var latestAuditData = JdAuditCommodityStock.ObjectSet().Where(p => p.CommodityStockId == item.Id && p.AuditType == 2).OrderByDescending(p => p.SubTime).FirstOrDefault();
                            if (latestAuditData == null || latestAuditData.Price != item.Price)
                            {
                                count++;
                                addCount++;
                                var auditStock = AddCommodityStockAudit(contextSession, group.AppId, item, Deploy.Enum.OperateTypeEnum.京东修改现价);
                                auditStock.AuditType = 2;
                                auditStock.JdAuditCommodityId = auditCom.Id;
                                auditStock.JdPrice = YXComNew.Price;
                                auditStock.CostPrice = YXComNew.Price * Convert.ToDecimal(0.8);
                                contextSession.SaveObject(auditStock);
                                if (AuditAuditModeApp.Contains(group.AppId))
                                {
                                    autoAuditPriceIds.Add(auditStock.Id);
                                }
                                LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 更新商品售价，商品Id: " + item.Id + "，SkuId: " + item.JDCode);
                            }
                        }
                    }
                    if (addCount > 0)
                    {
                        contextSession.SaveObject(auditCom);
                    }
                }
                int ccc = contextSession.SaveChanges();
                // 自动审核
                var auditComFacade = new JDAuditComFacade();
                auditComFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                if (autoAuditPriceIds.Count > 0)
                {
                    auditComFacade.AuditJDCostPrice(autoAuditPriceIds, 1, "自动审核", 0, 0);
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoUpdateYXComInfo 异常", ex);
                isSyncYXComInfo = false;
                throw;
            }
            LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 全量获取严选商品信息");
            isSyncYXComInfo = false;
        }
        /// <summary>
        /// 全量获取严选价格信息
        /// </summary>
        public static Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AutoUpdateYXComPrice()
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new Deploy.CustomDTO.ResultDTO() { isSuccess = false, ResultCode = 1 };
            if (isSyncYXComInfo)
            {
                LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 正在全量获取严选价格信息，跳过。。。");
                return result;
            }
            isSyncYXComInfo = true;
            LogHelper.Info("YXJobHelper.AutoUpdateYXComInfo 开始写入严选价格信息");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int count = 0;
                AutoGetAllSPU();
                var YXComList = YXComInfo.ObjectSet().ToList();
                var UpdateSpuList = YXComList.Select(s => s.SPU).ToList();
                List<YXComDetailDTO> YXComLists = new List<YXComDetailDTO>();
                for (int i = 0; i < YXComList.Count; i += 30)
                {
                    YXComLists.AddRange(YXSV.GetComDetailList(UpdateSpuList.Skip(i).Take(30).ToList()));
                    //Thread.Sleep(1000);
                }
                foreach (var YXComlist in YXComLists)
                {
                    foreach (var item in YXComlist.skuList)
                    {
                        YXComInfo YXCom = YXComInfo.CreateYXComInfo();
                        YXCom.Id = Guid.NewGuid();
                        YXCom.SPU = YXComlist.id;
                        YXCom.SKU = item.id;
                        YXCom.Price = item.channelPrice;
                        YXCom.CostPrice = item.channelPrice * Convert.ToDecimal(0.8);
                        YXCom.SubTime = DateTime.Now;
                        contextSession.SaveObject(YXCom);
                    }
                }
                count = contextSession.SaveChanges();
                YXComInfo.ObjectSet().Context.ExecuteStoreCommand("delete from YXComInfo where SKU is null");
                //取出最细你的严选价格信息
                var YXComNewList = YXComInfo.ObjectSet().ToList();
                //取出所有网易严选的商品信息
                Guid AppId = new Guid("1d769e14-f870-4b19-82ab-875a9e8678e4");
                var YXCom1 = Commodity.ObjectSet().Where(p => p.AppId == AppId).ToList();
                LogHelper.Info(string.Format("取出严选商品条数:{0}", YXCom1.Count()));
                int savecount = 0;
                //取出定时改价中严选商品               
                List<Guid> YXappids = CustomConfig.YxAppIdList;
                var timingComIds = Jinher.AMP.BTP.TPS.YJBSV.GetYXChangeComInfo(YXappids).Data.Select(s => s.CommodityId).ToList();
                for (int i = 0; i < YXCom1.Count; i += 100)
                {
                    var YxComList = YXCom1.Skip(i).Take(100).ToList();//取出商品id
                    var ComIds = YxComList.Select(s => s.Id);
                    LogHelper.Info(string.Format("严选商品Id:{0}", JsonHelper.JsonSerializer(ComIds)));
                    var YXComStock = CommodityStock.ObjectSet().Where(p => ComIds.Contains(p.CommodityId) && !timingComIds.Contains(p.Id)).ToList();
                    foreach (var item in YXComStock)
                    {
                        LogHelper.Info(string.Format("获取严选sku:{0}", item.JDCode));
                        var NewPriceInfo = YXComNewList.FirstOrDefault(p => p.SKU == item.JDCode);
                        if (NewPriceInfo == null)
                        {
                            LogHelper.Info(string.Format("不存在严选sku:{0}", item.JDCode));
                            item.IsDel = true;
                            item.ModifiedOn = DateTime.Now;
                        }
                        if (NewPriceInfo != null && NewPriceInfo.Price.HasValue && NewPriceInfo.CostPrice.HasValue && NewPriceInfo.Price > 0 && NewPriceInfo.CostPrice > 0)
                        {
                            LogHelper.Info(string.Format("获取严选商品sku:{0},售价:{1},进货价:{2}", NewPriceInfo.SKU, NewPriceInfo.Price, NewPriceInfo.CostPrice));
                            item.Price = NewPriceInfo.Price ?? item.Price;
                            item.CostPrice = NewPriceInfo.CostPrice ?? item.CostPrice;
                            item.IsDel = false;
                            item.ModifiedOn = DateTime.Now;
                        }
                    }
                    foreach (var item1 in YxComList)
                    {
                        var YXComMinPrice = YXComNewList.Where(p => p.SPU == item1.Barcode).OrderBy(p => p.Price).FirstOrDefault();
                        if (YXComMinPrice == null)
                        {
                            LogHelper.Info(string.Format("不存在严选SPU:{0}", item1.Barcode));
                            item1.IsDel = true;
                            item1.State = 0;
                        }
                        if (YXComMinPrice != null && YXComMinPrice.Price > 0 && YXComMinPrice.CostPrice > 0)
                        {
                            LogHelper.Info(string.Format("获取严选商品最小价格sku:{0},售价:{1},进货价:{2}", YXComMinPrice.SKU, YXComMinPrice.Price, YXComMinPrice.CostPrice));
                            item1.Price = YXComMinPrice.Price ?? item1.Price;
                            item1.CostPrice = YXComMinPrice.CostPrice ?? item1.CostPrice;
                            item1.ModifiedOn = DateTime.Now;
                        }
                    }
                    savecount += contextSession.SaveChanges();
                    LogHelper.Info(string.Format("获取苏宁价格保存条数:{0}", count));
                }
                result.isSuccess = true;
                result.Message = "严选接口保存成功" + count + "条;商品表修改条数:" + savecount;
                isSyncYXComInfo = false;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoUpdateYXComPrice 异常", ex);
                isSyncYXComInfo = false;
                return result;
            }
        }

        /// <summary>
        /// 全量同步严选库存信息
        /// </summary>
        public static Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AutoSynYXStockNum()
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new Deploy.CustomDTO.ResultDTO() { isSuccess = false, ResultCode = 1 };            
            LogHelper.Info("YXJobHelper.AutoSyncAllStockNum 开始写入严选库存信息");
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid AppId = new Guid("1d769e14-f870-4b19-82ab-875a9e8678e4");
                //取出库存表中所有的skuid
                var stockList = (from m in Commodity.ObjectSet()
                                 join n in CommodityStock.ObjectSet() on m.Id equals n.CommodityId
                                 where m.AppId == AppId &&
                                        m.IsDel == false&&n.IsDel==false
                                 select n).ToList();
                //取出所有SkuID
                var SkuidList = stockList.OrderBy(p => p.SubTime).Select(s => s.JDCode).ToList();
                List<StockDTO> YXstockList = new List<StockDTO>();
                for (int i = 0; i < SkuidList.Count; i += 99)
                {
                    LogHelper.Info(string.Format("严选sku:{0}", JsonHelper.JsonSerializer(SkuidList.Skip(i).Take(99).ToList())));
                    YXstockList.AddRange(YXSV.GetStockNum(SkuidList.Skip(i).Take(99).ToList()));
                    Thread.Sleep(1000);
                }
                if (!YXstockList.Any())
                {
                    LogHelper.Info("YXJobHelper.AutoSyncAllStockNum 未获取到严选库存信息,跳出~");
                    return result;
                }
                //更新库存
                foreach (var item in stockList)
                {
                    var YXStock = YXstockList.FirstOrDefault(p => p.skuId == item.JDCode);
                    if (YXStock != null)
                    {
                        item.Stock = YXStock.inventory;                        
                    }
                    else
                    {
                        item.Stock =0;                        
                    }
                    item.ModifiedOn = DateTime.Now;
                }
                int countstock = contextSession.SaveChanges();
                LogHelper.Info(string.Format("严选库存更新库存保存条数:{0}", countstock));
                #region 重新计算Commodity表库存信息
                //出去所有严选商品信息
                var commodity = Commodity.ObjectSet().Where(p => CustomConfig.YxAppIdList.Contains(p.AppId) && p.IsDel == false).ToList();
                var comids = commodity.Select(s => s.Id).ToList();
                var commodityStock = CommodityStock.ObjectSet().Where(p => comids.Contains(p.CommodityId)).ToList();
                //var AuditModeApp = JDAuditMode.ObjectSet().Where(_ => _.StockModeState == 0).Select(_ => _.AppId).ToList();//库存自动审核appid集合
                //到货提醒商品Id集合
                List<Guid> NoticeComIds = new List<Guid>();
                foreach (var item in commodity)
                {
                    var StockNo = commodityStock.Where(p => p.CommodityId == item.Id).Sum(s => s.Stock);
                    if (StockNo == 0)//无库存商品下架处理
                    {
                        item.State = 1;
                    }
                    else if (item.State == 1 && StockNo > 0)//有库存上架处理审核处理
                    {
                        item.State = 0;
                    }
                    if (item.Stock == 0 && StockNo > 0)
                    {
                        NoticeComIds.Add(item.Id);
                    }
                    item.Stock = StockNo;
                    item.ModifiedOn = DateTime.Now;
                }
                int countCom = contextSession.SaveChanges();
                LogHelper.Info(string.Format("严选库存更新库存保存条数:{0}", countCom));
                result.isSuccess = true;
                result.ResultCode = 0;
                result.Message = "库存表保存条数:{0}" + countstock + "商品表保存条数:{1}" + countCom;
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXJobHelper.AutoSyncAllStockNum 异常", ex);                
                return result;
            }
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
    }
}
