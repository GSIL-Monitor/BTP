using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.ISV.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Context;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Cache;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.Common.Context;
using Jinher.AMP.BTP.UI.Util;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.PL;
using System.Data;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;
using System.Text;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using System.Globalization;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class TestController : BaseController
    {
        [HttpGet]
        public ActionResult GetRefundErrorOrders()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JdJobHelper.GetRefundErrorOrders()));
        }

        [HttpGet]
        public ActionResult FixRefundStatusById(Guid orderReufndId)
        {
            JdJobHelper.FixRefundStatusById(orderReufndId);
            return Content("ok");
        }

        [HttpGet]
        public ActionResult SyncRefundStatusById(Guid orderReufndId)
        {
            JdJobHelper.SyncRefundStatusById(orderReufndId);
            return Content("ok");
        }

        [HttpGet]
        public ActionResult RepairJdOrder(string jdPorderId)
        {
            return Content(JdOrderHelper.RepairJdOrder(jdPorderId));
        }

        [HttpGet]
        public ActionResult RepairUnPayOrder(Guid orderId)
        {
            Jobs.RepairUnPayOrder(orderId);
            return Content("ok");
        }

        [HttpGet]
        public ActionResult AutoRefundByItemId(Guid commodityOrderItemId)
        {
            return Content(JdOrderHelper.AutoRefundByItemId(commodityOrderItemId));
        }

        [HttpGet]
        public ActionResult ImportOrder()
        {
            var dt = TestHelper.ImportOrder();
            return File(ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        [HttpGet]
        public ActionResult UpdateOrderItemYjbPriceJob(int days)
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
            return Content("错误订单：" + string.Join(",", errorOrderIds) + "---------修复订单：" + string.Join(",", orderIds));
        }

        [HttpGet]
        public ActionResult RefreshCommodity(Guid commodityId)
        {
            var commodity = Commodity.ObjectSet().Where(_ => _.Id == commodityId).FirstOrDefault();
            if (commodity != null)
            {
                commodity.RefreshCache(System.Data.EntityState.Deleted);
            }
            return Content("ok");
        }


        [HttpGet]
        public ActionResult TestRedis()
        {
            try
            {
                var key = "G_Test_0001";
                var value = "TestTestABCDEFG";
                RedisHelper.Set(key, value);
                for (int i = 0; i < 9999; i++)
                {
                    var val = RedisHelper.Get<string>(key);
                    if (val != value)
                    {
                        return Content(i + ":" + val);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("TestController.TestRedis异常", ex);
                throw;
            }
            return Content("ok");
        }


        //// GET: /Test/
        //[HttpGet]
        //public ActionResult TestUnFreezeGold()
        //{
        //    GoldPayFacade goldPayFacade = new GoldPayFacade();
        //    goldPayFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
        //    goldPayFacade.ContextDTO.LoginOrg = Guid.Empty;
        //    return Json(goldPayFacade.UnFreezeGold(new Jinher.AMP.FSP.Deploy.CustomDTO.UnFreezeGoldDTO()));
        //}

        public ActionResult ImportMallApp(Guid appId)
        {
            //Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.ImportNotSettleGoldOrder();
            //return Content("ok");
            return Content(Jinher.AMP.BTP.TPS.TestHelper.ImportMallApp(appId));
        }
        public ActionResult Index()
        {

            //Jinher.AMP.BTP.TPS.Invoic.InvoicManage invoicManage = new Jinher.AMP.BTP.TPS.Invoic.InvoicManage();
            //invoicManage.aaaaaa();

            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            var a = facade.GetCommodityOrderSdtoByOrderItemId(new Guid("02a47dd8-3ae3-43e7-809c-ac68a3bb82a0"));
            LogHelper.Debug("获取GetCommodityOrderSdtoByOrderItemId方法：返回：" + JsonHelper.JsSerializer(a));

            //if (base.ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Redirect("/Test/Login?returnUrl=Index");
            //}
            return View();
        }

        public ActionResult DataSynchronization(string guid)
        {
            //待自提订单 
            //      Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new CommodityOrderFacade();
            //facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            //List<OrderListCDTO> orderList = facade.GetOrderListByManagerId(Guid.Parse("B86DCCD0-1FD5-43C0-9FEC-E6B82AAABB47"), 1, 10);
            //售中自动确认收货 
            //   facade.AutoDealOrder();

            //售中9天自动确认收货
            //   facade.AutoDealOrderConfirm();

            //Jinher.AMP.BTP.ISV.Facade.BTPUserFacade userFacade = new BTPUserFacade();
            //ResultDTO result = userFacade.GetSelfTakeManager(Guid.Parse("B86DCCD0-1FD5-43C0-9FEC-E6B82AAABB47"));

            //Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade coFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
            //售后9天自动确认收货
            //coFacade.AutoDealOrderConfirmAfterSales();
            //List<OrderListCDTO> orderList = coFacade.GetSelfTakeOrderListAfterSales(Guid.Parse("B419D585-F258-4387-BF6E-E943ADD2847F"), 1, 10, "0");
            //ResultDTO result = coFacade.GetSelfTakeManagerAfterSales(Guid.Parse("B419D585-F258-4387-BF6E-E943ADD2847F"));

            //Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade facace = new ISV.Facade.CommodityOrderAfterSalesFacade();

            //自动满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间，排除退款退货申请和卖家同意并未超时未收到货之间的时间）
            //facace.AutoDealOrderAfterSales();

            //// 处理的退款处理订单 5天内未响应 交易状态变为 7 已退款
            //facace.AutoDaiRefundOrderAfterSales();

            ////// 买家7天不发出退货，自动恢复交易成功天数计时，满7天自动处理售后
            //facace.AutoRefundAndCommodityOrderAfterSales();

            ////// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
            //facace.AutoYiRefundOrderAfterSales();

            Jinher.AMP.BTP.ISV.Facade.PromotionFacade proFacade = new ISV.Facade.PromotionFacade();

            if (string.IsNullOrEmpty(guid))
            {
                var result = proFacade.CommodityDataAndRedisDataSynchronization();
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = -1, Messages = "失败" });
                }
            }
            else
            {
                var result1 = proFacade.PromotionRedis(Guid.Parse(guid));

                if (result1.ResultCode == 0)
                {
                    return Json(new { Result = -1, Messages = "失败" });
                }
            }


            return Json(new { Result = 0, Messages = "成功" });
        }

        public ActionResult ChangeDate(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            int day = 1;
            if (day > 0)
            {
                var result = srefacade.ChangeDate(appId, day);
                if (!result)
                {
                    return Json(new { Result = -1, Messages = "失败" });
                }
            }

            return Json(new { Result = 0, Messages = "成功" });
        }

        [HttpPost]
        public ActionResult CalcUcDaily(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            if (startDate > endDate)
                return Json(new { Result = -1, Messages = "开始日期不能大于截止日期" });

            if (startDate > DateTime.Today || endDate > DateTime.Today)
                return Json(new { Result = -1, Messages = "计算最大日期为昨天" });

            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            while (startDate <= endDate)
            {

                var result = srefacade.CalcUserCrowdfundingDaily(startDate.Date);
                if (!result)
                {
                    return Json(new { Result = -1, Messages = "计算失败，在计算 " + startDate.ToString("yyyy-MM-dd") + "  时出错" });
                }

                startDate = startDate.AddDays(1);
            }
            return Json(new { Result = 0, Messages = "计算成功" });
        }

        [HttpPost]
        public ActionResult CalcDividend()
        {

            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.CalcCfDividend();
            if (!result)
            {
                return Json(new { Result = -1, Messages = "计算失败" });
            }
            result = srefacade.CalcCfStatistics();
            if (!result)
            {
                return Json(new { Result = -1, Messages = "计算失败" });
            }


            return Json(new { Result = 0, Messages = "计算成功" });
        }

        [HttpPost]
        public ActionResult Calc()
        {

            Jinher.AMP.BTP.ISV.Facade.CommodityFacade srefacade = new ISV.Facade.CommodityFacade();

            try
            {
                srefacade.RemoveCache();
                return Json(new { Result = 0, Messages = "清理成功" });
            }
            catch
            {
                return Json(new { Result = 0, Messages = "清理失败" });
            }

            //Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();


            //var result = srefacade.CalcUserCrowdfundingDaily(DateTime.Today.AddDays(-1));
            //if (!result)
            //{
            //    return Json(new { Result = -1, Messages = "计算股东失败" });
            //}
            //result = srefacade.CalcCfDividend();
            //if (!result)
            //{
            //    return Json(new { Result = -1, Messages = "计算订单分红失败" });
            //}
            //result = srefacade.CalcCfStatistics();
            //if (!result)
            //{
            //    return Json(new { Result = -1, Messages = "计算汇总失败" });
            //}

            //return Json(new { Result = 0, Messages = "计算成功" });
        }


        [HttpPost]
        public ActionResult DelCf(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            int day = 1;
            if (day > 0)
            {
                var result = srefacade.DelCf(appId);
                if (!result)
                {
                    return Json(new { Result = -1, Messages = "失败" });
                }
            }

            return Json(new { Result = 0, Messages = "成功" });
        }

        [HttpPost]
        public ActionResult SendRedEnvelope()
        {
            Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade shareRedEnvelopeFacade = new ShareRedEnvelopeFacade();
            shareRedEnvelopeFacade.SendCfRedEnvelope();
            return Json(new { Result = 0, Messages = "成功" });
        }

        [HttpPost]
        public ActionResult ChangeCfStartTimeEarlier(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            int day = 1;
            if (day > 0)
            {
                var result = srefacade.ChangeCfStartTimeEarlier(appId);
                if (!result)
                {
                    return Json(new { Result = -1, Messages = "失败" });
                }
            }

            return Json(new { Result = 0, Messages = "成功" });
        }

        public ActionResult CacheManager()
        {
            if (base.ContextDTO.LoginUserID == Guid.Empty)
            {
                return Redirect("/Test/Login?returnUrl=CacheManager");
            }
            return View();
        }

        /// <summary>
        /// 清空商品信息的缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveCache()
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade srefacade = new ISV.Facade.CommodityFacade();

            try
            {
                srefacade.RemoveCache();
                return Json(new { Result = 0, Messages = "清空商品信息的缓存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "清空商品信息的缓存失败" });
            }
        }


        [HttpGet]
        public ActionResult ReturnJson()
        {
            string callback = Request.QueryString["callback"];
            string json = "{'name':'张三','age':'20'}";
            string result = string.Format("{0}({1})", callback, json);
            return Content(result);
        }

        public ActionResult ViewPage1()
        {
            Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:6444");
            //Response.AddHeader("Access-Control-Allow-Origin", "http://www.baidu.com");
            //Response.AddHeader("Access-Control-Allow-Origin", "*");

            return View();
        }

        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveZPHAPPCache()
        {
            Jinher.AMP.BTP.ISV.Facade.AppSetFacade appSetfacade = new ISV.Facade.AppSetFacade();

            try
            {
                appSetfacade.RemoveAppInZPHCache();
                return Json(new { Result = 0, Messages = "清理正品会APP缓存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "清理正品会APP缓存失败" });
            }
        }

        /// <summary>
        /// 清理商品列表缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveCommodityListCache(Guid? appId)
        {
            try
            {
                if (appId != null && appId != Guid.Empty)
                {
                    GlobalCacheWrapper.RemoveCache("G_CommodityList:" + appId, "BTPCache", CacheTypeEnum.redisSS);
                }
                else
                {
                    string[] keys = GlobalCacheWrapper.GetAllKey(CacheTypeEnum.redisSS, "BTPCache");

                    if (keys != null && keys.Length > 0)
                    {
                        List<string> tmpKey = keys.Where(t => t.StartsWith("G_CommodityList:")).ToList();
                        if (tmpKey != null && tmpKey.Count > 0)
                        {
                            foreach (string key in tmpKey)
                            {
                                GlobalCacheWrapper.RemoveCache(key, "BTPCache", CacheTypeEnum.redisSS);
                            }
                        }
                    }
                }

                return Json(new { Result = 0, Messages = "清理商品列表缓存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "清理商品列表缓存失败" });
            }
        }

        /// <summary>
        /// 清理商品分类缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveCategoryCache(Guid? appId)
        {
            try
            {
                if (appId != null && appId != Guid.Empty)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CategoryInfo", appId.ToString(), "BTPCache");
                }
                else
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache("G_CategoryInfo", "BTPCache", CacheTypeEnum.redisSS);
                }

                return Json(new { Result = 0, Messages = "清理商品分类缓存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "清理商品分类缓存失败" });
            }


        }

        /// <summary>
        /// 清理商品活动缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveCommodityPromotionCache(Guid? appId)
        {
            try
            {
                if (appId != null && appId != Guid.Empty)
                {
                    GlobalCacheWrapper.RemoveCache("G_CommodityPromotion:" + appId, "BTPCache", CacheTypeEnum.redisSS);
                }
                else
                {
                    string[] keys = GlobalCacheWrapper.GetAllKey(CacheTypeEnum.redisSS, "BTPCache");

                    if (keys != null && keys.Length > 0)
                    {
                        List<string> tmpKey = keys.Where(t => t.StartsWith("G_CommodityPromotion:")).ToList();
                        if (tmpKey != null && tmpKey.Count > 0)
                        {
                            foreach (string key in tmpKey)
                            {
                                GlobalCacheWrapper.RemoveCache(key, "BTPCache", CacheTypeEnum.redisSS);
                            }
                        }
                    }
                }

                return Json(new { Result = 0, Messages = "清理商品活动缓存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "清理商品活动缓存失败" });
            }
        }

        public ActionResult ResetContext()
        {
            AuthorizeHelper.ResetContextDTO();
            //OtherSV.UpdataOldDataInCommodityCategory();
            return View();
        }

        public ActionResult RemoveBehaviorRecordUrlCache()
        {
            try
            {
                RedisHelper.Remove(RedisKeyConst.BehaviorRecordUrl);
                //SessionCache.Current.Remove("BehaviorRecordUrl");
                return Json(new { Result = 0, Messages = "清理行为记录url缓存成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = -1, Messages = "清理行为记录url缓存失败！" });
            }

        }

        /// <summary>
        /// 删除折扣商品
        /// </summary>
        /// <param name="outSideId">外部活动Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemovePromotionByOutsideId(string outSideId)
        {
            try
            {
                var outsideid = Guid.Parse(outSideId);

                if (outsideid != null && outsideid != Guid.Empty)
                {
                    Jinher.AMP.BTP.ISV.Facade.PromotionFacade proFacade = new ISV.Facade.PromotionFacade();

                    var result = proFacade.DelOutsidePromotion(outsideid);

                    return Json(new { Result = result.ResultCode, Message = result.Message });
                }
                else
                {
                    return Json(new { Result = 0, Message = "请输入要删除的外部活动Id(outSideId)" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = -1, Messages = "删除这款商品失败！" });
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 处理登录
        /// </summary>
        /// <returns></returns>
        public JsonResult DoLogin()
        {
            ResultDTO rtData = new ResultDTO();
            var json = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            json.Data = rtData;

            string acnt = this.Request["acnt"];
            string pwd = this.Request["pwd"];
            if (string.IsNullOrEmpty(acnt) || string.IsNullOrEmpty(pwd))
            {
                rtData.ResultCode = 1;
                rtData.Message = "用户名或密码不能为空";
                return json;
            }
            else if (this.CheckPermission(acnt) == false)
            {
                rtData.ResultCode = 2;
                rtData.Message = "对不起！请求的资源不存在。";
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("未授权的用户{0}试图操作电商缓存被拒绝。", acnt));
                return json;
            }
            LoginReturnInfoDTO result = AuthorizeHelper.Login(acnt, pwd);
            if (result.IsSuccess)
            {
                //保存用户信息。
                ApplicationContext.Current[ApplicationContext.ContextKey] = result.ContextDTO;
                SessionCache.Current.AddCache(Jinher.JAP.Common.TypeDefine.Constant.UserKeyName,
                                              result.ContextDTO.LoginUserID.ToString());
                SessionCache.Current.AddCache(result.ContextDTO.LoginUserID.ToString(), result.ContextDTO);
                SessionCache.Current.AddCache("UserDepartment", result.ContextDTO.LoginDepartment.ToString());
                SessionCache.Current.AddCache("mUserId", result.ContextDTO.LoginUserID);

                rtData.ResultCode = 0;
                rtData.Message = result.Message;
            }
            else
            {
                rtData.ResultCode = 3;
                rtData.Message = result.Message;
            }
            return json;

        }




        /// <summary>
        /// 检查指定用户是否有权限访问。
        /// </summary>
        /// <param name="acnt">用户账号</param>
        /// <returns></returns>
        public bool CheckPermission(string acnt)
        {
            string users = Jinher.AMP.BTP.Common.CustomConfig.BtpCacheManagerUsers;
            if (users == null)
            {
                return false;
            }

            return users.Contains(acnt);
        }

        public ActionResult TestTmp()
        {
            return View();
        }

        public ActionResult PublishOrderMessage(string id, string userids, Guid appId, string messages,
                                                string contentCode)
        {
            //Jinher.AMP.BTP.TPS.BTPMessageSV btpSV = new TPS.BTPMessageSV();
            //btpSV.PublishOrderMessage(id, userids, appId, messages, contentCode);
            return View();
        }

        public ActionResult KuaiDi100Html2Json()
        {
            string url = "http://m.kuaidi100.com/index_all.html?type=yuantong&postid=710155965039&callbackurl={0}";
            url = string.Format(url, HttpUtility.UrlEncode("http://testbtp.iuoooo.com/Mobile/MyOrderDetail"));
            string responseText = WebRequestHelper.SendGetRequest(url);

            return View(responseText);
        }

        public ActionResult updateCrcAppId()
        {
            try
            {
                OtherSV.InsertDataInShoppingCartItems();
                return Json(new { Result = 0, Messages = "更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "更新失败" });
            }
        }

        public ActionResult GetAppName(Guid appId)
        {
            string name = APPSV.GetAppName(appId);
            return Json(new { Result = 0, Messages = name });
        }

        public ActionResult UpdateData()
        {
            try
            {
                OtherSV.InsertDataInShoppingCartItems();
                return Json(new { Result = 0, Messages = "更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "更新失败" });
            }
        }

        public ActionResult UpdateSkinData(Guid appid)
        {
            try
            {
                APPSV.Instance.GetAppPackFaceStartImg(appid);
                return Json(new { Result = 0, Messages = "更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Messages = "更新失败" });
            }
        }

        [HttpPost]
        public ActionResult UpdateLbsResult(string province, bool isRight)
        {
            LogHelper.Info(string.Format("校验当前省份结果{0},用户反馈结果为：{1}", province, isRight ? "正确" : "错误"));
            long rightCnt = 0;
            long errorCnt = 0;
            string hashId = "G_LbsResult";
            if (isRight)
            {
                rightCnt = RedisHelper.HashIncr(hashId, "1");
                errorCnt = RedisHelper.GetHashValue<int>(hashId, "0");
            }
            else
            {
                rightCnt = RedisHelper.GetHashValue<int>(hashId, "1");
                errorCnt = RedisHelper.HashIncr(hashId, "0");
            }

            return Json(new { Result = 0, Messages = "计算成功", RightCount = rightCnt, ErrorCount = errorCnt });
        }

        [HttpPost]
        public ActionResult FinishOrder(Guid orderId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade orderFacade = new ISV.Facade.CommodityOrderFacade();
            var updateOrderResult = orderFacade.UpdateOrderServiceTime(new OrderQueryParamDTO() { OrderId = orderId });
            if (updateOrderResult == null)
                return Json(new { Result = 1, Messages = "计算出错" });
            if (!updateOrderResult.isSuccess)
                return Json(new { Result = 1, Messages = updateOrderResult.Message });

            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderServiceFacade =
                        new ISV.Facade.CommodityOrderAfterSalesFacade();
                    orderServiceFacade.AutoDealOrderAfterSales();


                });
            return Json(new { Result = 0, Messages = "计算即将完成，请稍后" });

        }
        public ActionResult ComRevirew()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetReviewUrl(Guid uId, Guid comId)
        {
            ResultDTO rtData = new ResultDTO();
            var json = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            json.Data = rtData;

            if (uId == Guid.Empty)
            {
                rtData.ResultCode = 1;
                rtData.Message = "用户Id不正确";
                return json;
            }
            if (comId == Guid.Empty)
            {
                rtData.ResultCode = 1;
                rtData.Message = "请输入商品Id";
                return json;
            }

            ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
            var comInfo = facade.GetCommodityDetailsZPH(comId, Guid.Empty, Guid.Empty, "北京市", null);
            if (comInfo == null)
            {
                rtData.ResultCode = 1;
                rtData.Message = "未找到该商品";
                return json;
            }
            if (comInfo.ResultCode != 0 || comInfo.Data == null)
            {
                rtData.ResultCode = 1;
                rtData.Message = comInfo.Message;
                return json;
            }


            var callbackURL = string.Format("http://{0}btp.iuoooo.com/Review/ReviewSuccessNotifyComOnly", getPrefix());
            callbackURL = HttpUtility.UrlEncode(callbackURL);
            var url = string.Format("http://{0}sns.iuoooo.com/Evaluate/Index?productType=21&appId={1}&userId={2}&businessId={3}&productId={4}&title={5}&callbackURL={6}",
                getPrefix(), comInfo.Data.AppId, uId, Guid.NewGuid(), comId, comInfo.Data.Name, callbackURL);

            rtData.ResultCode = 0;
            rtData.Message = url;

            return json;


        }
        private string getPrefix()
        {
            var str = Request.Url.ToString();
            if (str.StartsWith("http://dev") || str.StartsWith("https://dev"))
            {
                return "dev";
            }
            if (str.StartsWith("http://test") || str.StartsWith("https://test"))
            {
                return "test";
            }
            return "";
        }

        /// <summary>
        /// 测试易捷卡密虚拟商品卡信息生成
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult TestYJBJCardCreate(Guid orderId)
        {
            var facade = new YJBJCardFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return Json(facade.Create(orderId));
        }

        /// <summary>
        /// 测试京东jos系统接口
        /// </summary>
        /// <returns></returns>
        public JsonResult TestJdJos()
        {
            //var result = JdJosHelper.QuerySpSource();
            //var result = JdJosHelper.CancelOrder("ESL4398046641940");
            //var result = JdJosHelper.QueryOrderStatus("ESL4398046641940");
            //return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }

        #region 进销存

        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="eclpOrderNo">京东订单编号,京东接口失败补录数据用</param>
        public void CreateJdEclpOrder(Guid orderId, string eclpOrderNo)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.CreateOrder(orderId, eclpOrderNo);
        }

        /// <summary>
        /// 发送支付数据到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendPayInfoToHaiXin(Guid orderId)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendPayInfoToHaiXin(orderId);
        }

        /// <summary>
        /// 发送售中整单退款(拒收)信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToHaiXin(Guid orderId)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendRefundInfoToHaiXin(orderId);
        }

        /// <summary>
        /// 创建进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <param name="servicesNo">京东服务单编号,京东接口失败补录数据用</param>
        public void CreateJDEclpRefundAfterSales(Guid orderId, Guid orderItemId, string servicesNo)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.CreateJDEclpRefundAfterSales(orderId, orderItemId, servicesNo);
        }

        /// <summary>
        /// 发送售后单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASSingleRefundInfoToHaiXin(Guid orderId, Guid orderItemId)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendASSingleRefundInfoToHaiXin(orderId, orderItemId);
        }

        /// <summary>
        /// 获取订单物流单号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult GetExpOrderNo(Guid orderId)
        {
            var facade = new IBP.Facade.JdEclpOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            var result = facade.GetExpOrderNo(orderId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 盈科大数据平台

        /// <summary>
        /// 发送支付数据到盈科大数据平台
        /// </summary>
        /// <param name="orderId"></param>
        public void SendPayInfoToYKBDMq(Guid orderId)
        {
            var facade = new IBP.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendPayInfoToYKBDMq(orderId);
        }

        /// <summary>
        /// 发送售中退款信息到盈科大数据平台
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToYKBDMq(Guid orderId)
        {
            var facade = new IBP.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendRefundInfoToYKBDMq(orderId);
        }

        /// <summary>
        /// 发送售后退款信息到盈科大数据平台
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASRefundInfoToYKBDMq(Guid orderId)
        {
            var facade = new IBP.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.SendASRefundInfoToYKBDMq(orderId);
        }

        #endregion

        #region 网易严选

        /// <summary>
        /// 店铺(商家)是否属于网易严选
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public JsonResult IsWangYiYanXuan(Guid appId)
        {
            return Json(ThirdECommerceHelper.IsWangYiYanXuan(appId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建网易严选订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        public void CreateYXOrder(Guid orderId, bool isExists)
        {
            YXOrderHelper.CreateOrder(orderId, isExists);
        }

        /// <summary>
        /// 渠道自助注册回调
        /// </summary>
        /// <param name="methods">多个方法名用英文逗号分隔，覆盖原来所有方法</param>
        /// <returns></returns>
        public JsonResult RegisterYXCallbackMethod(string methods)
        {
            return Json(YXSV.RegisterCallbackMethod(methods), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取渠道已注册的回调方法名
        /// </summary>
        /// <returns></returns>
        public JsonResult GetYXCallbackMethodList()
        {
            return Json(YXSV.GetCallbackMethodList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取网易严选订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult GetYXOrder(Guid orderId)
        {
            var jsonStr = string.Empty;
            var result = YXSV.GetPaidOrder(orderId.ToString(), ref jsonStr);
            return Content(jsonStr);
        }

        /// <summary>
        /// 获取严选物流轨迹信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="packageId"></param>
        /// <param name="expressNo"></param>
        /// <param name="expressCom"></param>
        /// <returns></returns>
        public JsonResult GetYXExpressOrder(Guid orderId, string packageId, string expressNo, string expressCom)
        {
            return Json(YXSV.GetExpressOrder(orderId.ToString(), packageId, expressNo, expressCom), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 网易严选确认收货
        /// </summary>
        /// <param name="orderId"></param>
        public void ConfirmReceivedOrder(Guid orderId)
        {
            YXOrderHelper.ConfirmReceivedOrder(orderId);
        }

        /// <summary>
        /// 网易严选取消订单
        /// </summary>
        /// <param name="orderId"></param>
        public void CancelOrderCallback(Guid orderId, int cancelStatus, string rejectReason)
        {
            YXOrderRefundHelper.CancelOrderCallback(new Deploy.CustomDTO.YX.OrderCancelResultCallBack { orderId = orderId.ToString().ToLower(), cancelStatus = cancelStatus, rejectReason = rejectReason });
        }

        /// <summary>
        /// 退货地址回调（同意售后申请）
        /// </summary>
        /// <param name="orderId"></param>
        public void OrderRefundAddress(Guid orderId, string applyId)
        {
            YXOrderRefundHelper.OrderRefundAddress(new Deploy.CustomDTO.YX.RefundAddress
            {
                applyId = applyId,
                orderId = orderId.ToString().ToLower(),
                returnAddr = new Deploy.CustomDTO.YX.ReturnAddr { fullAddress = "北京金和", mobile = "188888888", name = "测试" },
                type = 1
            });
        }

        /// <summary>
        /// 拒绝退货回调
        /// </summary>
        /// <param name="orderId"></param>
        public void RejectOrderRefund(Guid orderId, string applyId, string rejectReason)
        {
            YXOrderRefundHelper.RejectOrderRefund(new Jinher.AMP.BTP.Deploy.CustomDTO.YX.RejectInfo { applyId = applyId, orderId = orderId.ToString().ToLower(), rejectReason = rejectReason });
        }

        /// <summary>
        /// 取消退货回调
        /// </summary>
        /// <param name="orderId"></param>
        public void SystemCancelOrderRefund(Guid orderId, string applyId, string rejectReason)
        {
            YXOrderRefundHelper.SystemCancelOrderRefund(new Jinher.AMP.BTP.Deploy.CustomDTO.YX.SystemCancel { applyId = applyId, orderId = orderId.ToString().ToLower(), errorMsg = rejectReason });
        }

        /// <summary>
        /// 退款结果回调
        /// </summary>
        /// <param name="orderId"></param>
        public void OrderRefundResult(Guid orderId, string applyId, string skuId, Jinher.AMP.BTP.Deploy.CustomDTO.YX.OrderRefundApplySkuOperateStatusEnum status, string reason)
        {
            YXOrderRefundHelper.OrderRefundResult(new Jinher.AMP.BTP.Deploy.CustomDTO.YX.RefundResult
            {
                applyId = applyId,
                orderId = orderId.ToString().ToLower(),
                refundSkuList = new List<Deploy.CustomDTO.YX.RefundSku>
                {
                    new Deploy.CustomDTO.YX.RefundSku { skuId=skuId, operateSkus = new List<Deploy.CustomDTO.YX.OperateSku>
                    {
                        new Deploy.CustomDTO.YX.OperateSku{ skuId= skuId, status = status, reason = reason }
                    }}
                }
            });
        }

        /// <summary>
        /// 查询售后申请详情
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public JsonResult GetDetailRefundOrder(string applyId)
        {
            return Json(TPS.YXSV.GetDetailRefundOrder(applyId), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 网易严选触发回调-只用于测试环境模拟严选回调

        /// <summary>
        /// 触发异常取消订单回调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult CallbackCancelOrder(Guid orderId)
        {
            return Json(YXSV.CallbackCancelOrder(orderId.ToString()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发包裹物流绑单回调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult CallbackDeliveryOrder(Guid orderId)
        {
            return Json(YXSV.CallbackDeliveryOrder(orderId.ToString()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发审核取消订单回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="cancel">是否同意取消</param>
        /// <returns></returns>
        public JsonResult CallbackAuditCancelOrder(string applyId, bool cancel)
        {
            return Json(YXSV.CallbackAuditCancelOrder(applyId, cancel), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发退货地址回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="type">1：通过，无理由退货，2：通过，质量问题退货</param>
        /// <returns></returns>
        public JsonResult CallbackRefundAddress(string applyId, int type)
        {
            return Json(YXSV.CallbackRefundAddress(applyId, type), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发严选拒绝退货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public JsonResult CallbackRefundReject(string applyId)
        {
            return Json(YXSV.CallbackRefundReject(applyId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发退货包裹确认收货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public JsonResult CallbackRefundExpressConfirm(string applyId)
        {
            return Json(YXSV.CallbackRefundExpressConfirm(applyId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发严选系统取消退货回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public JsonResult CallbackRefundSystemCancel(string applyId)
        {
            return Json(YXSV.CallbackRefundSystemCancel(applyId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发直接退款结果回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <returns></returns>
        public JsonResult CallbackRefundResultDirectly(string applyId)
        {
            return Json(YXSV.CallbackRefundResultDirectly(applyId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 触发退货包裹确认收货后退款结果回调
        /// </summary>      
        /// <param name="applyId">申请单Id</param>
        /// <param name="allApproved">是否该申请单下所有sku都审核同意退款，默认为true</param>
        /// <returns></returns>
        public JsonResult CallbackRefundResultWithExpress(string applyId, bool allApproved)
        {
            return Json(YXSV.CallbackRefundResultWithExpress(applyId, allApproved), JsonRequestBehavior.AllowGet);
        }

        #endregion


        /// <summary>
        /// 确认收货
        /// </summary>
        [HttpGet]
        public JsonResult QuerenShou()
        {
            var facade = new ISV.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.AutoDealOrder();
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 京东物流job执行
        /// </summary>
        [HttpGet]
        public JsonResult OMSJdJob()
        {
            try
            {
                BTP.IBP.Facade.OrderExpressRouteFacade oerFacade = new BTP.IBP.Facade.OrderExpressRouteFacade();
                oerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                oerFacade.GetOrderExpressForJdJob();
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("OTMSJobException:{0}", e.Message), e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 急速物流job执行
        /// </summary>
        [HttpGet]
        public JsonResult OMSJsJob()
        {
            try
            {
                BTP.IBP.Facade.OrderExpressRouteFacade oerFacade = new BTP.IBP.Facade.OrderExpressRouteFacade();
                oerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                oerFacade.GetOrderExpressForJsJob();
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("OTMSJobException:{0}", e.Message), e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 京东已经确认收货，btp未确认交易完成情况
        /// </summary>
        [HttpGet]
        public JsonResult ModifyOrderState()
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
                        var commodityOrderService = CommodityOrderService.ObjectSet().Where(p => p.Code == item.Code).ToList();
                        if (!commodityOrderService.Any())
                        {
                            NewAutoJdConfirmOrder(CommodityOrderId);
                        }
                        else
                        {
                            item.State = 3;
                            item.ConfirmTime = DateTime.Now;
                            item.EntityState = EntityState.Modified;
                        }

                    }
                    contextSession.SaveChanges();
                }

            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("ModifyOrderState异常信息:{0}", e.Message), e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }

        public void NewAutoJdConfirmOrder(string CommodityOrderId)
        {
            try
            {
                var commodityorderfacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
                commodityorderfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                #region 更新订单状态
                JdOrderItemDTO model = new JdOrderItemDTO() { CommodityOrderId = CommodityOrderId };
                var jdorderitemList = jdorderitemfacade.GetJdOrderItemList(model).OrderByDescending(p => p.SubTime).Select(p => new { p.CommodityOrderId, p.State, p.ModifiedOn }).ToList();
                //获取订单的集合
                var array = jdorderitemList.GroupBy(p => p.CommodityOrderId).ToArray();
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        int count = 0;
                        int JScount = 0;
                        int total = 0;
                        var jdorderitems = jdorderitemList.Where(p => p.CommodityOrderId == item.Key.ToString()).OrderByDescending(p => p.ModifiedOn).ToList();
                        //确认收货时间
                        DateTime ConfirmTime = new DateTime();
                        total = jdorderitems.Count();
                        if (total > 0)
                        {
                            ConfirmTime = jdorderitems.FirstOrDefault().ModifiedOn;
                            foreach (var _item in jdorderitems)
                            {
                                if (_item.State == Convert.ToInt32(JdEnum.QRSH))
                                {
                                    count++;
                                }
                                if (_item.State == Convert.ToInt32(JdEnum.JS))
                                {
                                    JScount++;
                                }
                            }
                        }

                        if (count == total || total == JScount || total == count + JScount)
                        {
                            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                            List<Guid> ids = new List<Guid>();
                            ids.Add(Guid.Parse(item.Key.ToString()));
                            var commodityorder = facade.GetCommodityOrder(ids).FirstOrDefault();
                            if (commodityorder != null && (commodityorder.State == 1 || commodityorder.State == 2))
                            {
                                //待发货和已发货的确认交易成功,更新订单状态
                                var result = facade.UpdateJobCommodityOrder(ConfirmTime, Guid.Parse(item.Key.ToString()), Guid.Empty, Guid.Parse("8b4d3317-6562-4d51-bef1-0c05694ac3a6"), commodityorder.Payment, "000000", "");
                            }
                        }


                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdJobHelper.AutoJdConfirmOrder异常:{0}", ex.Message), ex);
            }
        }


        /// <summary>
        /// 京东已经确认收货，单独处理
        /// </summary>
        [HttpGet]
        public JsonResult ModifyOrderStates(string CommodityOrderId)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facade = new ISV.Facade.CommodityOrderFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                List<Guid> ids = new List<Guid>();
                ids.Add(Guid.Parse(CommodityOrderId));
                var commodityorder = facade.GetCommodityOrder(ids).FirstOrDefault();
                if (commodityorder != null)
                {
                    var jdorderItem = JdOrderItem.ObjectSet().Where(p => p.CommodityOrderId == CommodityOrderId).OrderByDescending(p => p.ModifiedOn).FirstOrDefault();
                    //已发货状态更新订单状态其他的不跟新
                    var result = facade.UpdateJobCommodityOrder(jdorderItem.ModifiedOn, Guid.Parse(CommodityOrderId), Guid.Empty, Guid.Parse("8b4d3317-6562-4d51-bef1-0c05694ac3a6"), commodityorder.Payment, "000000", "");

                }

            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("ModifyOrderStates异常信息:{0}", e.Message), e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 创建京东订单(之前没走京东的现在走京东流程)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SaveJdCommodityOrder(string Code, String ProvinceCode, String CityCode, String DistrictCode, String StreetCode)
        {
            try
            {
                ResultDTO result = null;
                Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                string orderPriceSnap = null;
                string sku = null;
                var commodityOrder = CommodityOrder.ObjectSet().Where(p => p.Code == Code).FirstOrDefault();
                var orderItemlist = OrderItem.ObjectSet().Where(p => p.Code == Code).ToList();
                if (orderItemlist != null && orderItemlist.Count() > 0)
                {
                    foreach (var item in orderItemlist)
                    {
                        var commodity = Commodity.ObjectSet().Where(p => p.Id == item.CommodityId).FirstOrDefault();
                        orderPriceSnap += "{'price':" + commodity.CostPrice + ",'skuId':" + commodity.JDCode + "},";
                        sku += "{'skuId':" + commodity.JDCode + ", 'num':" + item.Number + ",'bNeedAnnex':true, 'bNeedGift':false},";

                    }
                }
                if (!string.IsNullOrEmpty(orderPriceSnap) && !string.IsNullOrEmpty(sku))
                {
                    orderPriceSnap = orderPriceSnap.Remove(orderPriceSnap.Length - 1, 1);
                    sku = sku.Remove(sku.Length - 1, 1);
                    orderPriceSnap = "[" + orderPriceSnap + "]";
                    sku = "[" + sku + "]";
                    string thirdOrder = Guid.NewGuid().ToString();
                    string jdporderId = JdHelper.GetJDOrder(thirdOrder, orderPriceSnap, sku, commodityOrder.ReceiptUserName, commodityOrder.ReceiptAddress, commodityOrder.ReceiptPhone, "yijieds@126.com", ProvinceCode, CityCode, DistrictCode, StreetCode);
                    if (!string.IsNullOrWhiteSpace(jdporderId))
                    {

                        JdOrderItemDTO jdorderitemdto = new JdOrderItemDTO()
                        {
                            Id = Guid.NewGuid(),
                            JdPorderId = jdporderId,
                            TempId = Guid.Parse(thirdOrder),
                            JdOrderId = Guid.Empty.ToString(),
                            MainOrderId = Guid.Empty.ToString(),
                            CommodityOrderId = Guid.Empty.ToString(),
                            State = Convert.ToInt32(JdEnum.YZ),
                            StateContent = new EnumHelper().GetDescription(JdEnum.YZ),
                            SubTime = DateTime.Now,
                            ModifiedOn = DateTime.Now
                        };

                        result = jdorderitemfacade.SaveJdOrderItem(jdorderitemdto);
                    }

                    if (result.isSuccess == true)
                    {
                        JdOrderHelper.UpdateJdorder(commodityOrder.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultCode = 0, Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { ResultCode = 0, Message = "执行成功" }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 网易严选确认收货
        /// </summary>
        [HttpGet]
        public JsonResult ConfirmReceivedOrder(string orderId)
        {
            try
            {
                YXOrderHelper.ConfirmReceivedOrder(Guid.Parse(orderId));
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("OTMSJobException:{0}", e.Message), e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 拒收自动退款
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult AutoRefund(string orderId)
        {
            List<string> orderIds = new List<string>();
            try
            {
                LogHelper.Info("京东商品拒收自动退款，Begin....................");
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade commodityOrderFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
                BTP.ISV.Facade.CommodityOrderAfterSalesFacade orderSV = new BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
                Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
                orderSV.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdOrders = JdOrderItem.ObjectSet().Where(_ => _.State == 3 && (_.IsRefund == null || !_.IsRefund.Value) && _.CommodityOrderId == orderId).ToList();
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
                    // check 是否已退款
                    // TODO

                    var orderItem = OrderItem.ObjectSet().Where(_ => _.Id == order.CommodityOrderItemId.Value).FirstOrDefault();

                    if (orderItem == null)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，未找到订单Item, 提交申请：CommodityOrderItemId=" + order.CommodityOrderItemId.Value);
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

                        throw new Exception("退款失败");
                    }

                    // 同意退款
                    CancelTheOrderDTO model = new CancelTheOrderDTO();
                    model.OrderId = modelParam.Id;
                    model.OrderItemId = modelParam.OrderItemId;
                    model.State = 21;
                    model.Message = "";
                    model.UserId = Guid.Empty;
                    var cancelResult = cf.CancelTheOrderAfterSales(model);
                    if (cancelResult.ResultCode == 1)
                    {
                        LogHelper.Error("京东商品拒收自动退款失败，同意退款：CommodityOrderItemId=" + modelParam.OrderItemId + ", Message=" + result.Message);
                        throw new Exception("退款失败");
                    }
                    order.IsRefund = true;
                    contextSession.SaveObject(order);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("京东商品拒收自动退款异常", ex);
                return Json(new { Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 同步退款状态
        /// </summary>
        [HttpGet]
        public JsonResult SyncRefundStatus(string orderId)
        {
            LogHelper.Info("JdOrderHelper.SyncRefundStatus 开始");
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var afterSalesBpFacade = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
                afterSalesBpFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var afterSalesSvFacade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade();
                afterSalesSvFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.AfsServiceStep != 50 && _.AfsServiceStep != 60 && _.OrderId == Guid.Parse(orderId)).ToList();
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
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///查询jdoritem中State=3在京东接口中是1的情况
        /// </summary>
        [HttpGet]
        public JsonResult SearchJs()
        {

            try
            {
                var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                StringBuilder sb = new StringBuilder();
                DateTime starttime = DateTime.Now.AddDays(-30);
                DateTime endtime = DateTime.Now.AddDays(1);
                //获取jdorderitem表中所有的拆单数据
                JdOrderItemDTO entity = new JdOrderItemDTO();
                entity.State = Convert.ToInt32(JdEnum.JS);
                sb.Append("(");
                //获取jdorderitem表中所有的拆单数据
                var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(entity).OrderByDescending(p => p.SubTime).ToList();
                if (jdorderitemlist.Any())
                {
                    foreach (var item in jdorderitemlist)
                    {
                        var selectJdOrder1 = JdHelper.selectJdOrder1(item.JdOrderId);
                        if (!string.IsNullOrWhiteSpace(selectJdOrder1))
                        {
                            JObject objs = JObject.Parse(selectJdOrder1);
                            if (objs["state"] != null)
                            {
                                if (objs["state"].ToString() == "1")
                                {
                                    sb.Append("'" + item.JdOrderId + "',");

                                }

                            }
                        }
                    }
                }
                sb.Append(")");
                LogHelper.Info(string.Format("异常拒收信息:{0}", sb.ToString()));

            }
            catch (Exception e)
            {
                LogHelper.Error("SearchJs异常信息:" + e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        ///查询jdoritem中State=3在京东接口中是1的情况
        /// </summary>
        [HttpGet]
        public JsonResult SearchJss()
        {
            try
            {
                var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                StringBuilder sb = new StringBuilder();
                //获取jdorderitem表中所有的拆单数据
                JdOrderItemDTO entity = new JdOrderItemDTO();
                entity.State = Convert.ToInt32(JdEnum.JS);

                //获取jdorderitem表中所有的拆单数据
                var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(entity).OrderByDescending(p => p.SubTime).ToList();
                if (jdorderitemlist.Any())
                {
                    foreach (var item in jdorderitemlist)
                    {
                        var selectJdOrder1 = JdHelper.selectJdOrder1(item.JdOrderId);
                        if (!string.IsNullOrWhiteSpace(selectJdOrder1))
                        {
                            JObject objs = JObject.Parse(selectJdOrder1);
                            if (objs["state"] != null)
                            {
                                if (objs["state"].ToString() == "1")
                                {
                                    item.State = Convert.ToInt32(JdEnum.QRSH);
                                    item.StateContent = new EnumHelper().GetDescription(JdEnum.QRSH);
                                }
                            }
                            jdorderitemfacade.UpdateJdOrderItem(item);
                        }
                    }
                }


            }
            catch (Exception e)
            {
                LogHelper.Error("SearchJs异常信息:" + e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        ///
        /// </summary>
        [HttpGet]
        public JsonResult _SearchJs()
        {

            try
            {
                var jdorderitemfacade = new Jinher.AMP.BTP.IBP.Facade.JdOrderItemFacade();
                jdorderitemfacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                StringBuilder sb = new StringBuilder();
                DateTime starttime = DateTime.Now.AddDays(-30);
                DateTime endtime = DateTime.Now.AddDays(1);
                //获取jdorderitem表中所有的拆单数据
                JdOrderItemDTO entity = new JdOrderItemDTO();
                entity.State = Convert.ToInt32(JdEnum.QRSH);
                sb.Append("(");
                //获取jdorderitem表中所有的拆单数据
                var jdorderitemlist = jdorderitemfacade.GetJdOrderItemList(entity).Where(p => p.SubTime > starttime && p.SubTime < endtime).OrderByDescending(p => p.SubTime).ToList();
                if (jdorderitemlist.Any())
                {
                    foreach (var item in jdorderitemlist)
                    {
                        Guid CommodityOrderId = Guid.Parse(item.CommodityOrderId);
                        var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == CommodityOrderId);
                        if (commodityOrder != null)
                        {
                            if (commodityOrder.State == 7)
                            {
                                sb.Append("'" + commodityOrder.Code + "',");
                            }
                        }

                    }
                }
                sb.Append(")");
                LogHelper.Info(string.Format("异常拒收信息:{0}", sb.ToString()));

            }
            catch (Exception e)
            {
                LogHelper.Error("SearchJs异常信息:" + e);
            }
            return Json(new { Msg = "执行成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 同步苏宁商品信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="jdCommodityIds"></param>
        /// <returns></returns>
        public JsonResult SysSuNingCommodity(Guid appId, List<Guid> jdCommodityIds)
        {
            var facade = new SNCommodityFacade();
            return Json(facade.AutoSyncSNCommodityInfo(appId, jdCommodityIds), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据APPID获取拼团设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public JsonResult GetDiyGroupConfig(Guid appId)
        {
            return Json(TPS.ZPHSV.Instance.GetDiyGroupConfig(appId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据苏宁物流状态更改订单状态
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="OrderId"></param>
        /// <param name="OrderItemId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeOrderStatus(int Status, Guid OrderId, Guid? OrderItemId)
        {
            string Info = string.Empty;
            SuningSV.suning_govbus_rejection_handmode(Status, OrderId, (OrderItemId == null ? Guid.Empty : (Guid)OrderItemId), ref Info);
            return Content("OK" + (string.IsNullOrWhiteSpace(Info) ? "" : "，异常信息：" + Info));
        }
        /// <summary>
        /// 根据苏宁物流状态更改订单状态
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeOrderStatusBatch(string beginDate, string endDate)
        {
            DateTime beginTime, endTime;
            DateTime.TryParse(beginDate, out beginTime);
            DateTime.TryParse(endDate, out endTime);
            SuningSV.suning_govbus_rejection_batch(beginTime, endTime);
            return Content("OK");
        }

        [HttpGet]
        public ActionResult TestFangZheng(Guid OrderId)
        {
            FangZhengSV.FangZheng_Order_ForJobs();
            //ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //FangZhengSV.FangZheng_Order_Submit(contextSession, new BE.OrderItem
            //{
            //    Id=Guid.NewGuid(),
            //    CommodityOrderId=OrderId,
            //});
            //contextSession.SaveChange();
            //FangZhengSV.FangZheng_Order_Confirm(OrderId, false);
            return Content("TestFangZheng");
        }

        /// <summary>
        /// 苏宁订单手动补偿接口！
        /// </summary>
        /// <param name="OrderId">易捷订单ID</param>
        /// <param name="provinceId_cityId_countyId">直辖市许特殊处理</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SuningOrderHandAdd(Guid OrderId)
        {
            string Info = string.Empty;
            try
            {
                SuningSV.suning_govbus_order_add(OrderId);
            }
            catch (Exception ex)
            {
                Info = ex.Message;
            }
            if (string.IsNullOrWhiteSpace(Info))
                return Content("OK[" + OrderId + "]");
            else
                return Content("ERROR[" + OrderId + "]，异常信息：" + Info);
        }

        [HttpGet]
        public ActionResult FangZhengOrderSumit(Guid OrderId)
        {
            FangZhengSV.FangZheng_Order_Confirm(OrderId, false);
            return Content("OK");
        }

        /// <summary>
        /// 苏宁 订单退款
        /// </summary>
        /// <returns></returns>
        public ActionResult SnFefundOrder(string orderId)
        {
            ResultDTO result = new ResultDTO { ResultCode = 1, isSuccess = false, Message = "操作失败" };
            try
            {
                var order = new ISV.Facade.CommodityOrderFacade().GetOrderItems(Guid.Parse(orderId), Guid.Empty, Guid.Empty);
                if (order != null)
                {
                    var refundMoney = order.Price + (order.YJBPrice ?? 0) + order.ScorePrice + order.YJCouponPrice + order.Freight;
                    BTP.ISV.Facade.CommodityOrderFacade orderFacade = new ISV.Facade.CommodityOrderFacade();
                    result = orderFacade.SubmitOrderRefund(new SubmitOrderRefundDTO
                    {
                        commodityorderId = Guid.Parse(orderId),
                        Id = Guid.Parse(orderId),
                        RefundDesc = "拍错了",
                        RefundExpOrderNo = string.Empty,
                        RefundMoney = refundMoney,
                        RefundCouponPirce = order.YJCouponPrice,
                        State = 2,
                        RefundReason = "拍错了",
                        RefundType = 0,
                        OrderRefundImgs = string.Empty
                    });

                    LogHelper.Info("传入参数===" + JsonHelper.JsonSerializer(new SubmitOrderRefundDTO
                    {
                        commodityorderId = Guid.Parse(orderId),
                        Id = Guid.Parse(orderId),
                        RefundDesc = "拍错了",
                        RefundExpOrderNo = string.Empty,
                        RefundMoney = refundMoney,
                        RefundCouponPirce = order.YJCouponPrice,
                        State = 2,
                        RefundReason = "拍错了",
                        RefundType = 0,
                        OrderRefundImgs = string.Empty
                    }));
                    if (result.ResultCode != 0)
                    {
                        LogHelper.Error("进销存-拒收后自动申请退款失败:" + result.Message + ",入参:" + orderId);
                        result = new ResultDTO { ResultCode = 1, isSuccess = false, Message = "操作失败" };
                    }
                    else
                    {
                        result = new ResultDTO { ResultCode = 0, isSuccess = true, Message = "操作成功" };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultDTO { ResultCode = 1, isSuccess = false, Message = ex.Message.ToString() };
            }
            //ResultDTO ty = ApplyRefund(Guid.Parse(orderId));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  京东 7.7查询子订单信息接口
        /// </summary>
        /// <returns></returns>
        public ActionResult selectJdOrder1(string jdOrderId)
        {
            return Json(JdHelper.selectJdOrder1(jdOrderId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  京东 7.4发起支付接口
        /// </summary>
        /// <returns></returns>
        public ActionResult doPay(string jdOrderId)
        {
            return Json(JdHelper.doPay(jdOrderId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RedisTest()
        {
            string hashId = RedisKeyConst.UserOrder_JdPOrderIdList + "_Test";
            string key = Guid.NewGuid().ToString();
            string str = "[{\"JdPorderId\":\"111\",\"AppId\":\"111\",\"DateTime\":\""+DateTime.Now.ToString()+ "\"},{\"JdPorderId\":\"222\",\"AppId\":\"222\",\"DateTime\":\"" + DateTime.Now.ToString() + "\"}]";
            string json1 = RedisHelperNew.RegionGet<string>(hashId, key, "BTPCache");

            bool ty = string.IsNullOrEmpty(json1);

            RedisHelperNew.RegionSet(hashId, key, str, 1, "BTPCache");
            string json = RedisHelperNew.RegionGet<string>(hashId, key, "BTPCache");
            RedisHelperNew.Remove(hashId, key, "BTPCache");
             json1 = RedisHelperNew.RegionGet<string>(hashId, key, "BTPCache");
            return Content("写入后读取【"+json+"】,删除后读取【"+json1+"】");
        }

        /// <summary>
        ///  补发分享佣金接口
        /// </summary>
        /// <returns></returns>
        public ActionResult PushShareMoney(Guid orderId)
        {
            var facade = new Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade();
            return Json(facade.PushShareMoney(orderId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 苏宁对账1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SynSNBill()
        {
            LogHelper.Info(string.Format("苏宁对账开始1"));
            var facade = new Jinher.AMP.BTP.IBP.Facade.SNCommodityFacade();
            LogHelper.Info(string.Format("苏宁对账结束1"));
            return Json(facade.SynSNBill(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 苏宁对账2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SynSNBill2()
        {
            LogHelper.Info(string.Format("苏宁对账开始2"));
            var facade = new Jinher.AMP.BTP.IBP.Facade.SNCommodityFacade();
            LogHelper.Info(string.Format("苏宁对账结束2"));
            return Json(facade.SynSNBill2(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestExpress100()
        {
            //var result = Express100SV.GetExpressFromKD100DTO("801932173443236512", "圆通速递");
            // return Content("ok");

            var result = new Jinher.AMP.BTP.IBP.Facade.OrderExpressRouteFacade();
            result.GetOrderExpressForJsJob();
            return Content("ok");
        }
        [HttpGet,HttpPost]
        public ActionResult TestOrderExpressForJsJob()
        {
            var result = new Jinher.AMP.BTP.IBP.Facade.OrderExpressRouteFacade();
            result.GetOrderExpressForJsJob();
            return Content("ok");
        }
    }
}
