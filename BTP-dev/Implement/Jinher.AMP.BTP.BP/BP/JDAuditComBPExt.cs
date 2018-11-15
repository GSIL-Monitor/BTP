
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/2/27 10:48:37
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS;
using System.Data;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Common;


namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class JDAuditComBP : BaseBP, IJDAuditCom
    {
        /// <summary>
        /// 京东售价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetEditPriceListExt(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            try
            {
                var query = from m in AuditManage.ObjectSet()
                            join q in JdAuditCommodityStock.ObjectSet() on m.Id equals q.Id
                            join n in JdAuditCommodity.ObjectSet() on q.JdAuditCommodityId equals n.Id
                            where m.AppId == AppId && m.Action == Action && n.IsDel == false
                            select new CommodityAndCategoryDTO
                            {
                                AuditId = m.Id,
                                CommodityId = n.CommodityId,
                                AppId = n.AppId,
                                PicturesPath = n.PicturesPath,
                                JDCode = q.JDCode,
                                Name = n.Name,
                                ComAttribute = q.ComAttribute,
                                Price = q.Price,
                                CostPrice = q.CostPrice,
                                JdPrice = q.JdPrice,
                                JdCostPrice = q.JdCostPrice,
                                AuditRemark = m.AuditRemark,
                                AuditState = m.Status,
                                ApplyTime = m.ApplyTime,
                                AuditUserId = m.AuditUserId,
                                AuditTime = m.AuditTime,
                                ComStockId = q.CommodityStockId
                            };
                if (!string.IsNullOrEmpty(JdCode))
                {
                    query = query.Where(p => JdCode.Contains(p.JDCode) || p.JDCode.Contains(JdCode));
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name));
                }
                if (AuditState > -1)
                {
                    query = query.Where(p => p.AuditState == AuditState);
                }
                if (!string.IsNullOrEmpty(EditStartime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(EditStartime);
                    query = query.Where(p => p.ApplyTime >= StartTime);
                }
                if (!string.IsNullOrEmpty(EditEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(EditEndTime).AddDays(1);
                    query = query.Where(p => p.ApplyTime <= EndTime);
                }
                List<CommodityAndCategoryDTO> result = new List<CommodityAndCategoryDTO>();
                if (MinRate == 0 && MaxRate == 0)
                {
                    result = query.OrderByDescending(p => p.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    result = query.ToList();
                }
                //匹配审核人,供应商名称,App名称,商品属性处理
                if (result.Any())
                {

                    //获取商品最新售价和进货价  
                    List<Guid> stockIds = result.Select(p => p.ComStockId).ToList();
                    var ComStockInfo = CommodityStock.ObjectSet().Where(p => stockIds.Contains(p.Id)).Select(s => new { s.Id, s.CostPrice, s.Price }).ToList();
                    var CommodityInfo = Commodity.ObjectSet().Where(p => stockIds.Contains(p.Id)).Select(s => new { s.Id, s.CostPrice, s.Price }).ToList();

                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    //获取商铺名称
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();
                    //获取审核人ids
                    List<Guid> AuditUserIds = (from n in result where n.AuditUserId.HasValue select n.AuditUserId.Value).Distinct().ToList();
                    var Userinfo = CBCSV.GetUserNameAndCodes(AuditUserIds);
                    foreach (var item in result)
                    {
                        if (Action == 9)
                        {
                            //获取商品的最新售价
                            var CommodityStockInfo = ComStockInfo.FirstOrDefault(s => s.Id == item.ComStockId);
                            if (CommodityStockInfo != null)
                            {
                                item.NewCostPrice = CommodityStockInfo.CostPrice;
                            }
                            else
                            {
                                var commodityInfo = CommodityInfo.FirstOrDefault(s => s.Id == item.CommodityId);
                                if (commodityInfo != null)
                                {
                                    item.NewCostPrice = commodityInfo.CostPrice;
                                }
                                else
                                {
                                    item.NewCostPrice = null;
                                }
                            }
                            decimal n = (((item.Price - item.NewCostPrice) / item.Price) * 100) ?? 0;
                            decimal m = (((item.JdPrice - item.NewCostPrice) / item.JdPrice) * 100) ?? 0;
                            item.NowPriceProfit = Math.Round(n, 2).ToString();
                            item.NewPriceProfit = Math.Round(m, 2).ToString();
                        }
                        if (Action == 10)
                        {
                            //获取商品的最新售价
                            var CommodityStockInfo = ComStockInfo.FirstOrDefault(s => s.Id == item.Id);
                            if (CommodityStockInfo != null)
                            {
                                item.NewPrice = CommodityStockInfo.Price;
                            }
                            else
                            {
                                var commodityInfo = CommodityInfo.FirstOrDefault(s => s.Id == item.CommodityId);
                                if (commodityInfo != null)
                                {
                                    item.NewPrice = commodityInfo.Price;
                                }
                                else
                                {
                                    item.NewPrice = null;
                                }
                            }
                            decimal x = (((item.NewPrice - item.CostPrice) / item.NewPrice) * 100) ?? 0;
                            decimal y = (((item.NewPrice - item.JdCostPrice) / item.NewPrice) * 100) ?? 0;
                            item.NowCostPriceProfit = Math.Round(x, 2).ToString();
                            item.NewCostPriceProfit = Math.Round(y, 2).ToString();
                        }
                        if (item.AuditState != 0)
                        {
                            Guid UserId = item.AuditUserId ?? new Guid();
                            var NameAndCodes = Userinfo[UserId];
                            item.AuditUserCode = NameAndCodes.Item1;
                            item.AuditUserName = NameAndCodes.Item2;
                        }
                        //匹配供应商名称
                        item.SupplyName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //处理商品属性
                        string str = null;
                        if (!string.IsNullOrEmpty(item.ComAttribute))
                        {
                            JArray comjson = JArray.Parse(item.ComAttribute);
                            if (comjson.ToString() != "[]")
                            {
                                foreach (var it in comjson)
                                {
                                    JObject obj = JObject.Parse(it.ToString());
                                    str += obj["SecondAttribute"] + ",";
                                }
                                str = str.Remove(str.Length - 1, 1);
                                item.ComAttribute = str;
                            }
                            else
                            {
                                item.ComAttribute = str;
                            }
                        }
                        else
                        {
                            item.ComAttribute = str;
                        }
                    }
                    if (MinRate != 0 && MaxRate != 0)
                    {
                        if (Action == 9)
                        {
                            if (MinRate != 0)
                            {
                                result = result.Where(p => Convert.ToDecimal(p.NewPriceProfit) >= MinRate).ToList();
                            }
                            if (MaxRate != 0)
                            {
                                result = result.Where(p => Convert.ToDecimal(p.NewPriceProfit) <= MaxRate).ToList();
                            }
                        }
                        if (Action == 10)
                        {
                            if (MinRate != 0)
                            {
                                result = result.Where(p => Convert.ToDecimal(p.NewCostPriceProfit) >= MinRate).ToList();
                            }
                            if (MaxRate != 0)
                            {
                                result = result.Where(p => Convert.ToDecimal(p.NewCostPriceProfit) <= MaxRate).ToList();
                            }
                        }
                    }
                }
                if (MinRate != 0 || MaxRate != 0)
                {
                    result = result.OrderByDescending(p => p.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取京东售价审核列表信息异常。GetEditPriceListExt：AppId{0}", AppId), ex);
                return null;
            }
        }
        /// <summary>
        /// 设置售价审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditPriceModeExt(System.Guid Appid, int ModeStatus)
        {
            try
            {
                var JDAudit = JDAuditMode.ObjectSet().Where(p => p.AppId == Appid).FirstOrDefault();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //不存在则新建
                if (JDAudit == null)
                {
                    JDAuditMode JDAuditModeInfo = JDAuditMode.CreateJDAuditMode();
                    JDAuditModeInfo.Id = Guid.NewGuid();
                    JDAuditModeInfo.PriceModeState = ModeStatus;
                    JDAuditModeInfo.CostModeState = 1;
                    JDAuditModeInfo.StockModeState = 1;
                    JDAuditModeInfo.AppId = Appid;
                    JDAuditModeInfo.SubId = this.ContextDTO.LoginUserID;
                    JDAuditModeInfo.SubTime = DateTime.Now;
                    contextSession.SaveObject(JDAuditModeInfo);
                }
                else //存在则修改
                {
                    JDAudit.PriceModeState = ModeStatus;
                    JDAudit.ModifiedId = this.ContextDTO.LoginUserID;//修改人id
                    JDAudit.ModifiedOn = DateTime.Now;
                }
                int result = contextSession.SaveChange();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("设置售价审核方式服务异常。SetEditPriceModeExt：AppId{0},ModeStatus{1}", Appid, ModeStatus), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
        }
        /// <summary>
        /// 设置进货价审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditCostPriceModeExt(System.Guid Appid, int ModeStatus)
        {
            try
            {
                var JDAudit = JDAuditMode.ObjectSet().Where(p => p.AppId == Appid).FirstOrDefault();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //不存在则新建
                if (JDAudit == null)
                {
                    JDAuditMode JDAuditModeInfo = JDAuditMode.CreateJDAuditMode();
                    JDAuditModeInfo.Id = Guid.NewGuid();
                    JDAuditModeInfo.PriceModeState = 1;
                    JDAuditModeInfo.CostModeState = ModeStatus;
                    JDAuditModeInfo.StockModeState = 1;
                    JDAuditModeInfo.AppId = Appid;
                    JDAuditModeInfo.SubId = this.ContextDTO.LoginUserID;
                    JDAuditModeInfo.SubTime = DateTime.Now;
                    contextSession.SaveObject(JDAuditModeInfo);
                }
                else //存在则修改
                {
                    JDAudit.CostModeState = ModeStatus;
                    JDAudit.ModifiedId = this.ContextDTO.LoginUserID;//修改人id
                    JDAudit.ModifiedOn = DateTime.Now;
                }
                int result = contextSession.SaveChange();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("设置售价审核方式服务异常。SetEditCostPriceModeExt：AppId{0},ModeStatus{1}", Appid, ModeStatus), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
        }
        /// <summary>
        /// 审核京东售价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDPriceExt(System.Collections.Generic.List<System.Guid> ids, int state, decimal SetPrice, string AuditRemark, int JdAuditMode)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var AuditManageList = AuditManage.ObjectSet().Where(p => ids.Contains(p.Id)).ToList();
                if (AuditManageList.Any())
                {
                    if (state == 1)   //审核通过
                    {
                        EditCommodityAndStock(ids, JdAuditMode, contextSession);
                        foreach (var item in AuditManageList)
                        {
                            item.Status = state;
                            item.AuditRemark = AuditRemark;//审核意见
                            item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                            item.AuditTime = DateTime.Now;
                        }
                    }
                    else   //审核不通过
                    {
                        LogHelper.Info("审核京东售价不通过，商品Ids: " + ids + "，SetPrice: " + SetPrice);
                        RefuseEditStock(ids, SetPrice, contextSession);
                        foreach (var item in AuditManageList)
                        {
                            item.Status = state;
                            item.AuditRemark = AuditRemark;//审核意见
                            item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                            item.AuditTime = DateTime.Now;
                        }
                    }
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    if (!(state == 2 && SetPrice == 0))
                    {
                        //执行成功后将数据同步到商品变动表中
                        var StockIds = JdAuditCommodityStock.ObjectSet().Where(p => ids.Contains(p.Id)).Select(s => s.CommodityStockId).ToList();
                        LogHelper.Info("京东售价审核同步数据到商品变动明细报表，商品StockIds: " + StockIds + "，DateTime: " + DateTime.Now);
                        SaveCommodityChange(StockIds);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核京东售价服务异常。SetEditPriceModeExt：ids{0},state{1}", ids.ToString(), state), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
        }
        /// <summary>
        /// 审核通过根据京东最新价格修改商品价格
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void EditCommodityAndStock(List<Guid> Ids, int JdAuditMode, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {               
                //取出定时改价中严选商品               
                List<Guid> YXappids = CustomConfig.YxAppIdList;
                var timingComIds = Jinher.AMP.BTP.TPS.YJBSV.GetYXChangeComInfo(YXappids).Data.Select(s => s.CommodityId).ToList();               
                
                //取出京东最新的价格
                var JdAuditStockList = JdAuditCommodityStock.ObjectSet().Where(p => Ids.Contains(p.Id))
                                       .Select(s => new { commoditystockid = s.CommodityStockId, JdPrice = s.JdPrice }).ToList();
                List<Guid> StockIds = JdAuditStockList.Select(s => s.commoditystockid).Except(timingComIds).ToList();//过滤掉网易严选在定时改价中的商品
                var StockList = CommodityStock.ObjectSet().Where(p => StockIds.Contains(p.Id)).ToList();

                var Comids = StockList.GroupBy(p => p.CommodityId).Select(p => p.FirstOrDefault()).Select(p => p.CommodityId).ToList();
                var ComList = Commodity.ObjectSet().Where(p =>Comids.Contains(p.Id)).ToList();                
                foreach (var item in StockIds)
                {
                    var StockInfo = StockList.FirstOrDefault(p => p.Id == item);
                    //取出最新的京东价格
                    decimal? JdPrice = JdAuditStockList.Where(p => p.commoditystockid == item).Select(s => s.JdPrice).FirstOrDefault();
                    if (StockInfo != null)
                    {
                        if (JdPrice.HasValue && JdPrice != StockInfo.Price)
                        {
                            Guid appid = ComList.FirstOrDefault(p => p.Id == StockInfo.CommodityId).AppId;
                            StockInfo.Price = JdPrice ?? StockInfo.Price;
                            if (YXappids.Contains(appid))
                            {
                                StockInfo.CostPrice = StockInfo.Price * Convert.ToDecimal(0.8);
                            }
                            StockInfo.ModifiedOn = DateTime.Now;                                                       
                        }
                    }
                    else  //库存表中不存在,则需修改商品表中数据
                    {
                        var ComInfo = ComList.FirstOrDefault(p => p.Id == item);
                        if (JdPrice.HasValue && JdPrice != ComInfo.Price)
                        {
                            ComInfo.Price = JdPrice ?? ComInfo.Price;
                            if (YXappids.Contains(ComInfo.AppId))
                            {
                                ComInfo.CostPrice = ComInfo.Price * Convert.ToDecimal(0.8);
                            }
                            ComInfo.ModifiedOn = DateTime.Now;
                            if (JdAuditMode == 0)
                            {
                                ComInfo.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                            }
                            if (JdAuditMode == 1)
                            {
                                ComInfo.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                            }                            
                            ComInfo.RefreshCache(EntityState.Modified);                            
                        }
                    }
                    //取出库存表中最小的价格更新到commodity表中
                    var CommodityStocklist = CommodityStock.ObjectSet().Where(p => Comids.Contains(p.CommodityId)).ToList();
                    foreach (var Com in ComList)
                    {
                        decimal MinPrice = CommodityStocklist.Where(p => p.CommodityId == Com.Id).Select(s => s.Price).Min();
                        var NewMinPrice = StockList.Where(p => p.CommodityId == Com.Id).Select(s => s.Price).Min();
                        if (NewMinPrice!=null&&NewMinPrice<=MinPrice)
                        {
                            Com.Price = NewMinPrice;
                        }
                        else
                        {                            
                            Com.Price = MinPrice;
                        }                       
                        if (YXappids.Contains(Com.AppId))
                        {
                            Com.CostPrice = Com.Price * Convert.ToDecimal(0.8);
                        }
                        Com.ModifiedOn = DateTime.Now;
                        if (JdAuditMode == 0)
                        {
                            Com.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                        }
                        if (JdAuditMode == 1)
                        {
                            Com.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                        }
                        Com.RefreshCache(EntityState.Modified);
                    }
                } 
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核通过更新商品价格服务异常。EditCommodityAndStock"), ex);
            }
        }
        /// <summary>
        /// 审核不通过根据设置的价格更新商品价格
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void RefuseEditStock(List<Guid> Ids, decimal SetPrice, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                //取出定时改价中严选商品               
                List<Guid> YXappids = CustomConfig.YxAppIdList;
                var timingComIds = Jinher.AMP.BTP.TPS.YJBSV.GetYXChangeComInfo(YXappids).Data.Select(s => s.CommodityId).ToList();
                //取出京东最新的价格
                var JdAuditStockList = JdAuditCommodityStock.ObjectSet().Where(p => Ids.Contains(p.Id))
                                       .Select(s => new { commoditystockid = s.CommodityStockId, JdPrice = s.JdPrice }).ToList();
                List<Guid> StockIds = JdAuditStockList.Select(s => s.commoditystockid).Except(timingComIds).ToList();//过滤掉网易严选在定时改价中的商品

                var ComStocks = CommodityStock.ObjectSet().Where(p => StockIds.Contains(p.Id)).ToList();
                var ComIds = ComStocks.GroupBy(p => p.CommodityId).Select(p => p.FirstOrDefault()).Select(p => p.CommodityId).ToList();
                var Comdty = Commodity.ObjectSet().Where(p => ComIds.Contains(p.Id)).ToList();
                if (SetPrice > 0)
                {
                    foreach (var item in StockIds)
                    {                        
                        decimal? JdPrice = JdAuditStockList.Where(p => p.commoditystockid == item).Select(s => s.JdPrice).FirstOrDefault();
                        var StockInfo = ComStocks.FirstOrDefault(p => p.Id == item);
                        if (StockInfo != null)
                        {
                            Guid appid = Comdty.FirstOrDefault(p => p.Id == StockInfo.CommodityId).AppId;                            
                            if (YXappids.Contains(appid))
                            {
                                StockInfo.CostPrice = JdPrice * Convert.ToDecimal(0.8);
                            }
                            StockInfo.Price = SetPrice;
                            StockInfo.ModifiedOn = DateTime.Now;                            
                            LogHelper.Info("JDAuditComBPExt.RefuseEditStock 更新商品价格，商品Id: " + StockInfo.Id + "，SetPrice: " + SetPrice);
                        }
                        else
                        {
                            var ComInfo = Comdty.FirstOrDefault(p => p.Id == item);
                            if (ComInfo != null)
                            {
                                if (YXappids.Contains(ComInfo.AppId))
                                {
                                    ComInfo.CostPrice = JdPrice * Convert.ToDecimal(0.8);
                                }
                                ComInfo.Price = SetPrice;
                                ComInfo.ModifiedOn = DateTime.Now;                            
                                ComInfo.RefreshCache(EntityState.Modified);
                            }
                        }
                    }                   
                    //取出库存表中最小的价格更新到commodity表中  
                    var ComStockList = CommodityStock.ObjectSet().Where(p => ComIds.Contains(p.CommodityId)).ToList();
                    foreach (var Com in Comdty)
                    {
                        decimal MinPrice = ComStockList.Where(p=>p.CommodityId==Com.Id).Select(s => s.Price).Min();
                        var NewMinPrice = ComStocks.Where(p => p.CommodityId == Com.Id).Select(s => s.Price).Min();
                        if (MinPrice != Com.Price)
                        {                            
                            if (NewMinPrice != null && NewMinPrice <= MinPrice)
                            {
                                Com.Price = NewMinPrice;
                            }
                            else
                            {
                                Com.Price = MinPrice;
                            }
                            if (YXappids.Contains(Com.AppId))
                            {
                                Com.CostPrice = Com.Price * Convert.ToDecimal(0.8);
                            }
                            Com.ModifiedOn = DateTime.Now;
                            Com.ModifieId = this.ContextDTO.LoginUserID;//修改人id                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核不通过根据设置的价格更新商品价格服务异常。RefuseEditStock"), ex);
            }
        }
        /// <summary>
        /// 审核京东进货价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDCostPriceExt(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark, int Dispose, int JdAuditMode)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var AuditManageList = AuditManage.ObjectSet().Where(p => ids.Contains(p.Id)).ToList();
                if (AuditManageList.Any())
                {
                    if (state == 1)   //审核通过
                    {
                        EditComCostPrice(ids, JdAuditMode, contextSession);//修改进货价
                        foreach (var item in AuditManageList)
                        {
                            item.Status = state;
                            item.AuditRemark = AuditRemark;//审核意见
                            item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                            item.AuditTime = DateTime.Now;
                        }
                    }
                    else   //审核不通过
                    {
                        List<Guid> commodityIds = JdAuditCommodityStock.ObjectSet().Where(P => ids.Contains(P.Id)).Select(s => s.CommodityId).Distinct().ToList();
                        if (Dispose == 1)//下架处理
                        {
                            OffShelves(commodityIds, contextSession);
                        }
                        if (Dispose == 2) //售罄
                        {
                            EditComStock(ids, contextSession);
                        }
                        foreach (var item in AuditManageList)
                        {
                            item.Status = state;
                            item.AuditRemark = AuditRemark;//审核意见
                            item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                            item.AuditTime = DateTime.Now;                          
                        }
                    }
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    //执行成功后将数据同步到商品变动表中
                    var StockIds = JdAuditCommodityStock.ObjectSet().Where(p => ids.Contains(p.Id)).Select(s => s.CommodityStockId).ToList();                    
                    SaveCommodityChange(StockIds);
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核京东售价服务异常。SetEditPriceModeExt：ids{0},state{1},JDprice{2}", ids.ToString(), state, Dispose), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
        }
        /// <summary>
        /// 审核通过根据京东进货价修改
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void EditComCostPrice(List<Guid> Ids, int JdAuditMode, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {                
                //取出京东最新的价格
                var JdAuditStockList = JdAuditCommodityStock.ObjectSet().Where(p => Ids.Contains(p.Id))
                                       .Select(s => new { commoditystockid = s.CommodityStockId, JdCostPrice = s.JdCostPrice }).ToList();
                List<Guid> StockIds = JdAuditStockList.Select(s => s.commoditystockid).ToList();

                var ComStock = CommodityStock.ObjectSet().Where(p => StockIds.Contains(p.Id)).ToList();
                var ComIds = ComStock.GroupBy(p => p.CommodityId).Select(p => p.FirstOrDefault()).Select(p => p.CommodityId).ToList();
                var ComList = Commodity.ObjectSet().Where(p => ComIds.Contains(p.Id)).ToList();
               
                foreach (var item in StockIds)
                {
                    var StockInfo = ComStock.FirstOrDefault(p => p.Id == item);
                    //取出最新的京东价格
                    decimal? JdCostPrice = JdAuditStockList.Where(p => p.commoditystockid == item).Select(s => s.JdCostPrice).FirstOrDefault();
                    if (StockInfo != null)
                    {
                        if (JdCostPrice.HasValue && JdCostPrice != StockInfo.CostPrice)
                        {
                            StockInfo.CostPrice = JdCostPrice ?? StockInfo.CostPrice;
                            StockInfo.ModifiedOn = DateTime.Now;
                            contextSession.SaveObject(StockInfo);
                        }
                    }
                    else  //库存表中不存在的商品
                    {
                        var ComInfo = ComList.FirstOrDefault(p => p.Id == item);
                        if (ComInfo != null)
                        {
                            ComInfo.CostPrice = JdCostPrice ?? ComInfo.CostPrice;
                            ComInfo.ModifiedOn = DateTime.Now;
                            if (JdAuditMode == 0)
                            {
                                ComInfo.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                            }
                            if (JdAuditMode == 1)
                            {
                                ComInfo.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                            }                           
                            ComInfo.RefreshCache(EntityState.Modified);
                        }
                    }
                }
                //多属性最小价格的进货价更新到商品表
                var comstockList = CommodityStock.ObjectSet().Where(p => ComIds.Contains(p.CommodityId)).ToList();
                foreach (var Com in ComList)
                {
                    var MinData = comstockList.Where(p => p.CommodityId == Com.Id).OrderBy(p => p.Price).FirstOrDefault();
                    Com.CostPrice = MinData.CostPrice;
                    Com.ModifiedOn = DateTime.Now;
                    if (JdAuditMode == 0)
                    {
                        Com.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                    }
                    if (JdAuditMode == 1)
                    {
                        Com.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    }
                    Com.RefreshCache(EntityState.Modified);
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核通过更新商品进货价服务异常。EditComCostPrice"), ex);
            }
        }
        /// <summary>
        /// 审核不通过将商品设置为售罄(修改库存)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void EditComStock(List<Guid> Ids, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {                
                var StockIds = JdAuditCommodityStock.ObjectSet().Where(p => Ids.Contains(p.Id)).Select(s => s.CommodityStockId).ToList();
                var Comstocks = CommodityStock.ObjectSet().Where(p => StockIds.Contains(p.Id)).ToList();
                var ComIds = Comstocks.GroupBy(p => p.CommodityId).Select(p => p.FirstOrDefault()).Select(p => p.CommodityId).ToList();
                var ComList = Commodity.ObjectSet().Where(p => ComIds.Contains(p.Id)).ToList();


                foreach (var StockId in StockIds)
                {
                    var commoditystock = Comstocks.FirstOrDefault(p => p.Id == StockId);
                    if (commoditystock != null)
                    {
                        commoditystock.ModifiedOn = DateTime.Now;
                        commoditystock.Stock = 0;
                        //更新商品表库存
                        var commodity = ComList.FirstOrDefault(p => p.Id == commoditystock.CommodityId);
                        commodity.Stock = commodity.Stock - commoditystock.Stock;
                        commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id                        
                        commodity.ModifiedOn = DateTime.Now;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                    else //库存表中数据不存在 在商品表中查找
                    {
                        var commodity = ComList.FirstOrDefault(p => p.Id == StockId);
                        if (commodity!=null)
                        {
                            commodity.Stock = 0;
                            commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id                        
                            commodity.ModifiedOn = DateTime.Now;
                            commodity.RefreshCache(EntityState.Modified);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核不通过将商品设置为售罄服务异常。EditComStock"), ex);
            }
        }
        void OffShelves(System.Collections.Generic.List<System.Guid> ids, ContextSession contextSession)
        {
            try
            {               
                ids.RemoveAll(c => c == Guid.Empty);
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                var commoditys = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                foreach (var commodity in commoditys)
                {                    
                    commodity.State = 1;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    needRefreshCacheCommoditys.Add(commodity);                    
                    commodity.RefreshCache(EntityState.Modified);
                }
                //删除热门商品表
                var hotCommodity = HotCommodity.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (HotCommodity hc in hotCommodity)
                {
                    contextSession.Delete(hc);
                }
                //删除今日促销表信息
                var todayPromotion = TodayPromotion.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (TodayPromotion pro in todayPromotion)
                {
                    contextSession.Delete(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }
                //删除促销缓存
                for (int i = 0; i < ids.Count; i++)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", ids[i].ToString(), "BTPCache");
                }
                //删除促销商品表
                var promotionItems = PromotionItems.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (PromotionItems items in promotionItems)
                {
                    contextSession.Delete(items);
                }
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("下架商品服务异常。ids：{0}", ids), ex);
            }

        }
        /// <summary>
        /// 获取商铺审核方式
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDAuditModeDTO GetAuditModeExt(System.Guid AppId)
        {
            try
            {
                //根据EsAppId获取馆信息
                var JDAuditState = JDAuditMode.ObjectSet().Where(p => p.AppId == AppId).FirstOrDefault();
                if (JDAuditState == null)
                {
                    return new JDAuditModeDTO() { PriceModeState = 1, CostModeState = 1, StockModeState = 1 };
                }
                return JDAuditState.ToEntityData();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据AppId获取审核方式服务异常。EsAppId{0} datetime：{1}", AppId, DateTime.Now), ex);
                return new JDAuditModeDTO() { PriceModeState = 1, CostModeState = 1, StockModeState = 1 };
            }
        }
        /// <summary>
        /// 导出京东售价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportPriceListExt(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action)
        {
            try
            {
                var query = (from m in AuditManage.ObjectSet()
                             join q in JdAuditCommodityStock.ObjectSet() on m.Id equals q.Id
                             join n in JdAuditCommodity.ObjectSet() on q.JdAuditCommodityId equals n.Id
                             where m.AppId == AppId & m.Action == Action
                             select new CommodityAndCategoryDTO
                             {
                                 AuditId = m.Id,
                                 CommodityId = n.CommodityId,
                                 AppId = n.AppId,
                                 PicturesPath = n.PicturesPath,
                                 JDCode = n.JDCode,
                                 Name = n.Name,
                                 ComAttribute = q.ComAttribute,
                                 Price = q.Price,
                                 CostPrice = q.CostPrice,
                                 JdPrice = q.JdPrice,
                                 JdCostPrice = q.JdCostPrice,
                                 AuditRemark = m.AuditRemark,
                                 AuditState = m.Status,
                                 ApplyTime = m.ApplyTime,
                                 AuditUserId = m.AuditUserId,
                                 AuditTime = m.AuditTime,
                                 ComStockId = q.CommodityStockId
                             }).ToList();
                if (!string.IsNullOrEmpty(JdCode))
                {
                    query = query.Where(p => JdCode.Contains(p.JDCode) || p.JDCode.Contains(JdCode)).ToList();
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name)).ToList();
                }
                if (AuditState > -1)
                {
                    query = query.Where(p => p.AuditState == AuditState).ToList();
                }
                if (!string.IsNullOrEmpty(EditStartime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(EditStartime);
                    query = query.Where(p => p.ApplyTime >= StartTime).ToList();
                }
                if (!string.IsNullOrEmpty(EditEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(EditEndTime).AddDays(1);
                    query = query.Where(p => p.ApplyTime <= EndTime).ToList();
                }
                var result = query.OrderByDescending(p => p.ApplyTime).ToList();
                //匹配审核人,供应商名称,App名称,商品属性处理
                if (result.Any())
                {
                    //获取商品最新售价和进货价  
                    List<Guid> stockIds = result.Select(p => p.ComStockId).ToList();
                    var ComStockInfo = CommodityStock.ObjectSet().Where(p => stockIds.Contains(p.Id)).Select(s => new { s.Id, s.CostPrice, s.Price }).ToList();
                    var CommodityInfo = Commodity.ObjectSet().Where(p => stockIds.Contains(p.Id)).Select(s => new { s.Id, s.CostPrice, s.Price }).ToList();

                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    //获取商铺名称
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();
                    //获取审核人ids
                    List<Guid> AuditUserIds = (from n in result where n.AuditUserId.HasValue select n.AuditUserId.Value).Distinct().ToList();
                    var Userinfo = CBCSV.GetUserNameAndCodes(AuditUserIds);
                    foreach (var item in result)
                    {
                        if (Action == 9)
                        {
                            //获取商品的最新售价
                            var CommodityStockInfo = ComStockInfo.FirstOrDefault(s => s.Id == item.Id);
                            if (CommodityStockInfo != null)
                            {
                                item.NewCostPrice = CommodityStockInfo.CostPrice;
                            }
                            else
                            {
                                var commodityInfo = CommodityInfo.FirstOrDefault(s => s.Id == item.CommodityId);
                                if (commodityInfo != null)
                                {
                                    item.NewCostPrice = commodityInfo.CostPrice;
                                }
                                else
                                {
                                    item.NewCostPrice = null;
                                }
                            }
                            decimal n = (((item.Price - item.NewCostPrice) / item.Price) * 100) ?? 0;
                            decimal m = (((item.JdPrice - item.NewCostPrice) / item.JdPrice) * 100) ?? 0;
                            item.NowPriceProfit = Math.Round(n, 2).ToString();
                            item.NewPriceProfit = Math.Round(m, 2).ToString();
                        }
                        if (Action == 10)
                        {
                            //获取商品的最新售价
                            var CommodityStockInfo = ComStockInfo.FirstOrDefault(s => s.Id == item.Id);
                            if (CommodityStockInfo != null)
                            {
                                item.NewPrice = CommodityStockInfo.Price;
                            }
                            else
                            {
                                var commodityInfo = CommodityInfo.FirstOrDefault(s => s.Id == item.CommodityId);
                                if (commodityInfo != null)
                                {
                                    item.NewPrice = commodityInfo.Price;
                                }
                                else
                                {
                                    item.NewPrice = null;
                                }
                            }
                            decimal x = (((item.NewPrice - item.CostPrice) / item.NewPrice) * 100) ?? 0;
                            decimal y = (((item.NewPrice - item.JdCostPrice) / item.NewPrice) * 100) ?? 0;
                            item.NowCostPriceProfit = Math.Round(x, 2).ToString();
                            item.NewCostPriceProfit = Math.Round(y, 2).ToString();
                        }
                        if (item.AuditState != 0)
                        {
                            Guid UserId = item.AuditUserId ?? new Guid();
                            var NameAndCodes = Userinfo[UserId];
                            item.AuditUserCode = NameAndCodes.Item1;
                            item.AuditUserName = NameAndCodes.Item2;
                        }
                        //匹配供应商名称
                        item.SupplyName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //处理商品属性
                        string str = null;
                        if (!string.IsNullOrEmpty(item.ComAttribute))
                        {
                            JArray comjson = JArray.Parse(item.ComAttribute);
                            if (comjson.ToString() != "[]")
                            {
                                foreach (var it in comjson)
                                {
                                    JObject obj = JObject.Parse(it.ToString());
                                    str += obj["SecondAttribute"] + ",";
                                }
                                str = str.Remove(str.Length - 1, 1);
                                item.ComAttribute = str;
                            }
                            else
                            {
                                item.ComAttribute = str;
                            }
                        }
                        else
                        {
                            item.ComAttribute = str;
                        }
                        //审核状态
                        if (item.AuditState == 0)
                        {
                            item.AuditStateName = "未审核";
                        }
                        else if (item.AuditState == 1)
                        {
                            item.AuditStateName = "审核通过";
                        }
                        else
                        {
                            item.AuditStateName = "审核不通过";
                        }
                    }
                    if (Action == 9)
                    {
                        if (MinRate != 0)
                        {
                            result = result.Where(p => Convert.ToDecimal(p.NewPriceProfit) >= MinRate).ToList();
                        }
                        if (MaxRate != 0)
                        {
                            result = result.Where(p => Convert.ToDecimal(p.NewPriceProfit) <= MaxRate).ToList();
                        }
                    }
                    if (Action == 10)
                    {
                        if (MinRate != 0)
                        {
                            result = result.Where(p => Convert.ToDecimal(p.NewCostPriceProfit) >= MinRate).ToList();
                        }
                        if (MaxRate != 0)
                        {
                            result = result.Where(p => Convert.ToDecimal(p.NewCostPriceProfit) <= MaxRate).ToList();
                        }
                    }
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("导出京东售价审核列表信息异常。GetEditPriceListExt：AppId{0}", AppId), ex);
                return new ResultDTO<List<CommodityAndCategoryDTO>>();
            }
        }

        /// <summary>
        /// 获取下架无货商品审核列表
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetOffSaleAndNoStockListExt(System.Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            try
            {
                var query = from m in AuditManage.ObjectSet()
                            join q in JdAuditCommodityStock.ObjectSet() on m.Id equals q.Id
                            join n in JdAuditCommodity.ObjectSet() on q.JdAuditCommodityId equals n.Id
                            where m.AppId == AppId & m.Action == Action
                            select new CommodityAndCategoryDTO
                            {
                                AuditId = m.Id,
                                CommodityId = n.CommodityId,
                                AppId = n.AppId,
                                PicturesPath = n.PicturesPath,
                                JDCode = q.JDCode,
                                Name = n.Name,
                                ComAttribute = q.ComAttribute,
                                Price = q.Price,
                                CostPrice = q.CostPrice,
                                Stock = q.Stock,
                                JdStatus = q.JdStatus,
                                AuditState = m.Status,
                                ApplyTime = m.ApplyTime,
                                AuditUserId = m.AuditUserId,
                                AuditTime = m.AuditTime
                            };
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name));
                }
                if (!string.IsNullOrEmpty(JdCode))
                {
                    query = query.Where(p => JdCode.Contains(p.JDCode) || p.JDCode.Contains(JdCode));
                }
                if (AuditState > -1)
                {
                    query = query.Where(p => p.AuditState == AuditState);
                }
                if (JdStatus > -1)
                {
                    query = query.Where(p => p.JdStatus == JdStatus);
                }
                if (!string.IsNullOrEmpty(EditStartime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(EditStartime);
                    query = query.Where(p => p.ApplyTime >= StartTime);
                }
                if (!string.IsNullOrEmpty(EditEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(EditEndTime).AddDays(1);
                    query = query.Where(p => p.ApplyTime <= EndTime);
                }
                var result = query.OrderByDescending(p => p.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                //匹配审核人,供应商名称,App名称,商品属性处理
                if (result.Any())
                {
                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    //获取商铺名称
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();
                    //获取审核人ids
                    List<Guid> AuditUserIds = (from n in result where n.AuditUserId.HasValue select n.AuditUserId.Value).Distinct().ToList();
                    var Userinfo = CBCSV.GetUserNameAndCodes(AuditUserIds);
                    foreach (var item in result)
                    {
                        if (item.AuditState != 0)
                        {
                            Guid UserId = item.AuditUserId ?? new Guid();
                            var NameAndCodes = Userinfo[UserId];
                            item.AuditUserCode = NameAndCodes.Item1;
                            item.AuditUserName = NameAndCodes.Item2;
                        }
                        //匹配供应商名称
                        item.SupplyName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //处理商品状态 京东状态（1-已下架 2-已上架 3-无货 4-有货）
                        if (item.JdStatus == 1)
                        {
                            item.JdStatusName = "已下架";
                        }
                        else if (item.JdStatus == 2)
                        {
                            item.JdStatusName = "已上架";
                        }
                        else if (item.JdStatus == 3)
                        {
                            item.JdStatusName = "无货";
                        }
                        else
                        {
                            item.JdStatusName = "有货";
                        }
                        //处理方式  (11-置为下架 12-置为售罄 13-置为上架 14-置为有货)
                        if (item.AuditState == 0)
                        {
                            item.AuditStateName = "待审核";
                        }
                        else if (item.AuditState == 11)
                        {
                            item.AuditStateName = "置为下架";
                        }
                        else if (item.AuditState == 12)
                        {
                            item.AuditStateName = "置为售罄";
                        }
                        else if (item.AuditState == 13)
                        {
                            item.AuditStateName = "置为上架";
                        }
                        else
                        {
                            item.AuditStateName = "置为有货";
                        }
                        //处理商品属性
                        string str = null;
                        if (!string.IsNullOrEmpty(item.ComAttribute))
                        {
                            JArray comjson = JArray.Parse(item.ComAttribute);
                            if (comjson.ToString() != "[]")
                            {
                                foreach (var it in comjson)
                                {
                                    JObject obj = JObject.Parse(it.ToString());
                                    str += obj["SecondAttribute"] + ",";
                                }
                                str = str.Remove(str.Length - 1, 1);
                                item.ComAttribute = str;
                            }
                            else
                            {
                                item.ComAttribute = str;
                            }
                        }
                        else
                        {
                            item.ComAttribute = str;
                        }
                    }
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取下架无货商品审核列表服务异常。GetOffSaleAndNoStockListExt：AppId{0}", AppId), ex);
                return null;
            }
        }
        /// <summary>
        /// 设置下架无货商品审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffAndNoStockModeExt(System.Guid Appid, int ModeStatus)
        {
            try
            {
                var JDAudit = JDAuditMode.ObjectSet().Where(p => p.AppId == Appid).FirstOrDefault();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //不存在则新建
                if (JDAudit == null)
                {
                    JDAuditMode JDAuditModeInfo = JDAuditMode.CreateJDAuditMode();
                    JDAuditModeInfo.Id = Guid.NewGuid();
                    JDAuditModeInfo.PriceModeState = 1;
                    JDAuditModeInfo.CostModeState = 1;
                    JDAuditModeInfo.StockModeState = ModeStatus;
                    JDAuditModeInfo.AppId = Appid;
                    JDAuditModeInfo.SubId = this.ContextDTO.LoginUserID;
                    JDAuditModeInfo.SubTime = DateTime.Now;
                    contextSession.SaveObject(JDAuditModeInfo);
                }
                else //存在则修改
                {
                    JDAudit.StockModeState = ModeStatus;
                    JDAudit.ModifiedId = this.ContextDTO.LoginUserID;//修改人id
                    JDAudit.ModifiedOn = DateTime.Now;
                }
                int result = contextSession.SaveChange();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("设置下架无货商品审核方式服务异常。SetOffAndNoStockModeExt：AppId{0},ModeStatus{1}", Appid, ModeStatus), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
        }
        /// <summary>
        /// 导出下架无货商品审核列表数据
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportOffSaleAndNoStockDataExt(Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action)
        {
            try
            {
                var query = from m in AuditManage.ObjectSet()
                            join q in JdAuditCommodityStock.ObjectSet() on m.Id equals q.Id
                            join n in JdAuditCommodity.ObjectSet() on q.JdAuditCommodityId equals n.Id
                            where m.AppId == AppId & m.Action == Action
                            select new CommodityAndCategoryDTO
                            {
                                AuditId = m.Id,
                                CommodityId = n.CommodityId,
                                AppId = n.AppId,
                                PicturesPath = n.PicturesPath,
                                JDCode = q.JDCode,
                                Name = n.Name,
                                ComAttribute = q.ComAttribute,
                                Price = q.Price,
                                CostPrice = q.CostPrice,
                                Stock = q.Stock,
                                JdStatus = q.JdStatus,
                                AuditState = m.Status,
                                ApplyTime = m.ApplyTime,
                                AuditUserId = m.AuditUserId,
                                AuditTime = m.AuditTime
                            };
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name));
                }
                if (!string.IsNullOrEmpty(JdCode))
                {
                    query = query.Where(p => JdCode.Contains(p.JDCode) || p.JDCode.Contains(JdCode));
                }
                if (AuditState > -1)
                {
                    query = query.Where(p => p.AuditState == AuditState);
                }
                if (JdStatus > -1)
                {
                    query = query.Where(p => p.JdStatus == JdStatus);
                }
                if (!string.IsNullOrEmpty(EditStartime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(EditStartime);
                    query = query.Where(p => p.ApplyTime >= StartTime);
                }
                if (!string.IsNullOrEmpty(EditEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(EditEndTime).AddDays(1);
                    query = query.Where(p => p.ApplyTime <= EndTime);
                }
                var result = query.OrderByDescending(p => p.ApplyTime).ToList();
                //匹配审核人,供应商名称,App名称,商品属性处理
                if (result.Any())
                {
                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    //获取商铺名称
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();
                    //获取审核人ids
                    List<Guid> AuditUserIds = (from n in result where n.AuditUserId.HasValue select n.AuditUserId.Value).Distinct().ToList();
                    var Userinfo = CBCSV.GetUserNameAndCodes(AuditUserIds);
                    foreach (var item in result)
                    {
                        if (item.AuditState != 0)
                        {
                            Guid UserId = item.AuditUserId ?? new Guid();
                            var NameAndCodes = Userinfo[UserId];
                            item.AuditUserCode = NameAndCodes.Item1;
                            item.AuditUserName = NameAndCodes.Item2;
                        }
                        //匹配供应商名称
                        item.SupplyName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //处理商品状态 京东状态（1-已下架 2-已上架 3-无货 4-有货）
                        if (item.JdStatus == 1)
                        {
                            item.JdStatusName = "已下架";
                        }
                        else if (item.JdStatus == 2)
                        {
                            item.JdStatusName = "已上架";
                        }
                        else if (item.JdStatus == 3)
                        {
                            item.JdStatusName = "无货";
                        }
                        else
                        {
                            item.JdStatusName = "有货";
                        }
                        //处理方式  (11-置为下架 12-置为售罄 13-置为上架 14-置为有货)
                        if (item.AuditState == 0)
                        {
                            item.AuditStateName = "待审核";
                        }
                        else if (item.AuditState == 11)
                        {
                            item.AuditStateName = "置为下架";
                        }
                        else if (item.AuditState == 12)
                        {
                            item.AuditStateName = "置为售罄";
                        }
                        else if (item.AuditState == 13)
                        {
                            item.AuditStateName = "置为上架";
                        }
                        else
                        {
                            item.AuditStateName = "置为有货";
                        }
                        //处理商品属性
                        string str = null;
                        if (!string.IsNullOrEmpty(item.ComAttribute))
                        {
                            JArray comjson = JArray.Parse(item.ComAttribute);
                            if (comjson.ToString() != "[]")
                            {
                                foreach (var it in comjson)
                                {
                                    JObject obj = JObject.Parse(it.ToString());
                                    str += obj["SecondAttribute"] + ",";
                                }
                                str = str.Remove(str.Length - 1, 1);
                                item.ComAttribute = str;
                            }
                            else
                            {
                                item.ComAttribute = str;
                            }
                        }
                        else
                        {
                            item.ComAttribute = str;
                        }
                    }
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("导出下架无货商品审核列表服务异常。GetOffSaleAndNoStockListExt：AppId{0}", AppId), ex);
                return null;
            }
        }
        /// <summary>
        /// 置为有货
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInStoreExt(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            try
            {
                var StockIds = JdAuditCommodityStock.ObjectSet().Where(p => ids.Contains(p.Id)).Select(s => s.CommodityStockId).ToList();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<Guid> NoticeComIds = new List<Guid>();//到货提醒商品Id
                foreach (var StockId in StockIds)
                {
                    var commoditystock = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                    if (commoditystock != null)
                    {
                        commoditystock.ModifiedOn = DateTime.Now;
                        commoditystock.Stock = 999;
                        //更新商品表库存
                        var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == commoditystock.CommodityId);
                        commodity.Stock = commodity.Stock + 999;
                        if (commodity.Stock==0)
                        {
                            NoticeComIds.Add(commodity.Id);
                        }
                        if (JdAuditMode == 0)
                        {
                            commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                        }
                        if (JdAuditMode == 1)
                        {
                            commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                        }
                        commodity.ModifiedOn = DateTime.Now;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                    else //库存表中数据不存在 在商品表中查找
                    {
                        var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                        if (commodity.Stock == 0)
                        {
                            NoticeComIds.Add(commodity.Id);
                        }
                        commodity.Stock = 999;                        
                        if (JdAuditMode == 0)
                        {
                            commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                        }
                        if (JdAuditMode == 1)
                        {
                            commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                        }
                        commodity.ModifiedOn = DateTime.Now;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
                if (JdAuditMode == 1)
                {
                    var AuditManageList = AuditManage.ObjectSet().Where(p => ids.Contains(p.Id));
                    foreach (var item in AuditManageList)
                    {
                        item.Status = 14;
                        item.AuditRemark = "";//审核意见
                        item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                        item.AuditTime = DateTime.Now;
                    }
                }
                int count = contextSession.SaveChanges();
                //调用到货提醒接口
                if (NoticeComIds.Any())
                {
                    ZPHSV.SendStockNotifications(NoticeComIds);
                }
                //执行成功后将数据同步到商品变动表中                    
                LogHelper.Info("京东置为有货同步数据到商品变动明细报表，商品StockIds: " + StockIds + "，DateTime: " + DateTime.Now);
                SaveCommodityChange(StockIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核置为有货服务异常。SetInStoreExt"), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 置为上架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetPutawayExt(System.Collections.Generic.List<System.Guid> AuditIds, int JdAuditMode)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var StockIdList = JdAuditCommodityStock.ObjectSet().Where(p => AuditIds.Contains(p.Id)).Select(s => s.CommodityStockId).Distinct().ToList();
                List<Guid> ids = new List<Guid>();//下架的商品id
                List<Guid> NoticeComIds = new List<Guid>();//到货提醒商品id
                foreach (var StockId in StockIdList)
                {
                    var StockInfo = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                    if (StockInfo != null)
                    {
                        LogHelper.Info("上架商品的id" + StockInfo.Id);
                        StockInfo.Stock = 999;
                        StockInfo.ModifiedOn = DateTime.Now;
                        StockInfo.State = 0;
                        contextSession.SaveChange();
                        ids.Add(StockInfo.CommodityId);
                        var commoditystock = CommodityStock.ObjectSet().Where(p => p.CommodityId == StockInfo.CommodityId & (p.State == 1 || p.State == null));

                        foreach (var item in commoditystock)
                        {
                            LogHelper.Info("修改未上架商品id" + item.Id);
                            item.Stock = 0;
                            item.ModifiedOn = DateTime.Now;
                        }
                        contextSession.SaveChange();//保存商品属性后重新计算商品库存
                        int totalstock = CommodityStock.ObjectSet().Where(p => p.CommodityId == StockInfo.CommodityId).Select(s => s.Stock).Sum();
                        var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == StockInfo.CommodityId);
                        if (commodity.Stock==0&&totalstock>0)
                        {
                            NoticeComIds.Add(commodity.Id);
                        }
                        commodity.Stock = totalstock;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                    else
                    {
                        ids.Add(StockId);
                    }
                }
                if (JdAuditMode == 1)
                {
                    var AuditManageList = AuditManage.ObjectSet().Where(p => AuditIds.Contains(p.Id));
                    foreach (var item in AuditManageList)
                    {
                        item.Status = 13;
                        item.AuditRemark = "";//审核意见
                        item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                        item.AuditTime = DateTime.Now;
                    }
                }
                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };

                Guid appId = Guid.Empty;
                Guid userId = Guid.Empty;
                string photoUrl = string.Empty;
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                var commoditys = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                if (!commoditys.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                foreach (var commodity in commoditys)
                {
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.State = 0;
                    commodity.GroundTime = DateTime.Now;
                    needRefreshCacheCommoditys.Add(commodity);

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                }
                if (commoditys.Count > 0)
                {
                    appId = commoditys[0].AppId;
                    userId = commoditys[0].SubId;
                    photoUrl = commoditys[0].PicturesPath;
                }
                contextSession.SaveChange();
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }

                #region 上架发布广场

                IUS.Deploy.CustomDTO.PicFromUrlCDTO addDataDTO = new IUS.Deploy.CustomDTO.PicFromUrlCDTO();
                addDataDTO.AppId = appId;
                addDataDTO.Content = APPSV.GetAppName(appId) + "有新品上架了哦，快来看看吧~";
                addDataDTO.PhotoUrl = photoUrl;
                addDataDTO.ShareUrl = string.Format("{0}Mobile/CommodityList?AppId={1}&sortType=New&type=tuwen",
                                                    Jinher.AMP.BTP.Common.CustomConfig.BtpDomain, appId);
                addDataDTO.Source = Jinher.AMP.IUS.Deploy.Enum.SourceEnum.EBusinessInfo;
                addDataDTO.Title = addDataDTO.Content;
                addDataDTO.UserId = userId;
                addDataDTO.UserName = (ContextDTO != null && ContextDTO.LoginUserName != null)
                                          ? ContextDTO.LoginUserName
                                          : "btp";
                var result = Jinher.AMP.BTP.TPS.IUSSV.Instance.AddPicFromUrl(addDataDTO);

                #endregion
                //调用到货提醒接口
                if (NoticeComIds.Any())
                {
                    ZPHSV.SendStockNotifications(NoticeComIds);
                }
                //将上架商品插入Commoditychange表
                SaveCommodityChange(StockIdList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("上架商品服务异常。ids：{0}", JsonHelper.JsonSerializer(AuditIds)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 置为售罄
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetNoStockExt(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            try
            {
                var StockIds = JdAuditCommodityStock.ObjectSet().Where(p => ids.Contains(p.Id)).Select(s => s.CommodityStockId).ToList();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var StockId in StockIds)
                {
                    var commoditystock = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                    if (commoditystock != null)
                    {
                        commoditystock.ModifiedOn = DateTime.Now;
                        commoditystock.Stock = 0;
                        //更新商品表库存
                        var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == commoditystock.CommodityId);
                        commodity.Stock = commodity.Stock - commoditystock.Stock;
                        if (JdAuditMode == 0)
                        {
                            commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                        }
                        if (JdAuditMode == 1)
                        {
                            commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                        }
                        commodity.ModifiedOn = DateTime.Now;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                    else //库存表中数据不存在 在商品表中查找
                    {
                        var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                        commodity.Stock = 0;
                        if (JdAuditMode == 0)
                        {
                            commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                        }
                        if (JdAuditMode == 1)
                        {
                            commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                        }
                        commodity.ModifiedOn = DateTime.Now;
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
                if (JdAuditMode == 1)
                {
                    var AuditManageList = AuditManage.ObjectSet().Where(p => ids.Contains(p.Id));
                    foreach (var item in AuditManageList)
                    {
                        item.Status = 12;
                        item.AuditRemark = "";//审核意见
                        item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                        item.AuditTime = DateTime.Now;
                    }
                }

                int count = contextSession.SaveChanges();
                //执行成功后将数据同步到商品变动表中                    
                LogHelper.Info("京东置为售罄同步数据到商品变动明细报表，商品StockIds: " + StockIds + "，DateTime: " + DateTime.Now);
                SaveCommodityChange(StockIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核置为售罄服务异常。SetInStoreExt"), ex);
                return new ResultDTO { ResultCode = 1, Message = "false" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 置为下架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffShelfExt(System.Collections.Generic.List<System.Guid> AuditIds, int JdAuditMode)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var StockIdList = JdAuditCommodityStock.ObjectSet().Where(p => AuditIds.Contains(p.Id)).Select(s => s.CommodityStockId).Distinct().ToList();
                List<Guid> ids = new List<Guid>();//下架的商品id
                foreach (var StockId in StockIdList)
                {
                    var StockInfo = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == StockId);
                    if (StockInfo != null)
                    {
                        StockInfo.ModifiedOn = DateTime.Now;
                        StockInfo.State = 1;
                        int count = contextSession.SaveChanges();
                        var StockState = CommodityStock.ObjectSet().Where(p => p.CommodityId == StockInfo.CommodityId).Select(s => s.State);
                        if (StockState.All(p => p.Value == 1))   //库存表状态全部为下架  则商品下架
                        {
                            ids.Add(StockInfo.CommodityId);
                        }
                        else //库存表状态不是全部为下架  库存表对应的商品库存修改为0
                        {

                            StockInfo.ModifiedOn = DateTime.Now;
                            StockInfo.Stock = 0;
                            //商品表同时更新库存数量
                            var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == StockInfo.CommodityId);
                            commodity.Stock = commodity.Stock - StockInfo.Stock;
                            if (JdAuditMode == 0)
                            {
                                commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                            }
                            if (JdAuditMode == 1)
                            {
                                commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                            }
                            commodity.ModifiedOn = DateTime.Now;
                            commodity.RefreshCache(EntityState.Modified);
                        }
                    }
                    else
                    {
                        ids.Add(StockId);
                    }
                }
                if (JdAuditMode == 1)
                {
                    var AuditManageList = AuditManage.ObjectSet().Where(p => AuditIds.Contains(p.Id));
                    foreach (var item in AuditManageList)
                    {
                        item.Status = 11;
                        item.AuditRemark = "";//审核意见
                        item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                        item.AuditTime = DateTime.Now;
                    }
                }
                //商品下架
                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();

                var commoditys = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                if (!commoditys.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                foreach (var commodity in commoditys)
                {
                    commodity.State = 1;
                    commodity.ModifiedOn = DateTime.Now;
                    if (JdAuditMode == 0)
                    {
                        commodity.ModifieId = new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9");//京东自动修改
                    }
                    if (JdAuditMode == 1)
                    {
                        commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    }
                    needRefreshCacheCommoditys.Add(commodity);

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                }

                //删除热门商品表
                var hotCommodity = HotCommodity.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (HotCommodity hc in hotCommodity)
                {
                    contextSession.Delete(hc);
                }

                //删除今日促销表信息
                var todayPromotion = TodayPromotion.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (TodayPromotion pro in todayPromotion)
                {
                    contextSession.Delete(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }

                //删除促销缓存
                for (int i = 0; i < ids.Count; i++)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", ids[i].ToString(), "BTPCache");
                }

                //删除促销商品表
                var promotionItems = PromotionItems.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (PromotionItems items in promotionItems)
                {
                    contextSession.Delete(items);
                }

                int count2 = contextSession.SaveChange();
                //将下架商品插入Commoditychange表
                SaveCommodityChange(StockIdList.ToList());
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("下架商品服务异常。ids：{0}", AuditIds), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 修的的数据插入commoditychange表中
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <summary>
        /// 修的的数据插入commoditychange表中
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChange(List<System.Guid> ids)
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
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DTO = ChangeFacade.SaveCommodityChange(list);
            //LogHelper.Info("京东商品审核同步数据到商品变动明细报表，入参: " + JsonHelper.JsonSerializer(list) + "，DateTime: " + DateTime.Now);
            return DTO;
        }
    }
}
