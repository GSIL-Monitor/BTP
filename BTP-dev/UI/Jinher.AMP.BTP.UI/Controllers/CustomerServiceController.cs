using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CustomerServiceController : BaseController
    {
        //
        // GET: /CustomerService/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 我的订单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult MyOrderInfo()
        {
            var appId = new Guid();
            int pageIndex = 1;
            int pageSize = 20;
            if (Request["UserId"] != null)
            {
                ViewBag.UserId = Request["UserId"];
            }
            if (Request["AppId"] != null)
            {
                ViewBag.AppId = Request["AppId"];
                appId = Guid.Parse(Request["AppId"]);
            }
            else
            {
                return View();
            }

            CommodityOrderFacade cf = new CommodityOrderFacade();
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                UserId = !String.IsNullOrEmpty(Request["UserId"]) ? Guid.Parse(Request["UserId"]) : Guid.Empty
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.EsOrderIds = searchResult.Message;
            CommodityOrderMoneyToSearch moneyToSearch = new CommodityOrderMoneyToSearch()
            {
                AppId = appId
            };
            var moneyToResult = cf.GetCommodityOrderMoneyTo(moneyToSearch);
            if (moneyToResult != null && moneyToResult.Count > 0 && moneyToResult.Data != null)
            {
                ViewBag.MoneyToList = moneyToResult.Data;
            }
            return View();
        }
        [HttpPost]
        [GridAction]
        public ActionResult GetCommodityItemList(string UserId, int PageSize, int page)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderItemFacade fade = new IBP.Facade.CommodityOrderItemFacade();
            var list = fade.GetCommodityOrderItemByUserId(UserId, PageSize, page);

            int rowCount = list.ResultCode;
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Name");
            show.Add("Number");
            show.Add("Price");
            show.Add("SubTime");
            show.Add("OrderState");
            show.Add("OrderId");
            var gridobj = new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>(show, list.Data, rowCount, page, PageSize, string.Empty);
            return View(gridobj);
        }
        /// <summary>
        /// 订单查询页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchOrderInfo()
        {

            var appId = ViewBag.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;

            //ViewBag.AppId = appId;

            Guid UserId = Guid.Empty;

            if (Request["UserId"] != null)
            {
                UserId = Guid.Parse(Request["UserId"].ToString());
            }
            ViewBag.UserId = UserId;

            CommodityOrderFacade cf = new CommodityOrderFacade();
            int pageIndex = 1;
            int pageSize = 20;
            string strPageSize = Request["pageSize"] as string;

            if (!string.IsNullOrEmpty(strPageSize))
            {
                if (!int.TryParse(strPageSize, out pageSize))
                {
                    pageSize = 20;
                }
                else
                {
                    if (pageSize > 200)
                    {
                        pageSize = 200;//最大200条每次
                    }
                }
            }
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                UserId = Guid.Empty //订单查询页面，需要所有的非当前用户的
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.EsOrderIds = searchResult.Message;
            CommodityOrderMoneyToSearch moneyToSearch = new CommodityOrderMoneyToSearch()
            {
                AppId = appId
            };
            var moneyToResult = cf.GetCommodityOrderMoneyTo(moneyToSearch);
            if (moneyToResult != null && moneyToResult.Count > 0 && moneyToResult.Data != null)
            {
                ViewBag.MoneyToList = moneyToResult.Data;
            }

            var orderSoucre = cf.GetOrderSource(moneyToSearch);
            if (orderSoucre != null && orderSoucre.Count > 0 && orderSoucre.Data != null)
            {
                ViewBag.OrderSoucre = orderSoucre.Data;
            }

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPOrderStateUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderStateUpd);
                bool BTPOrderPriceUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderPriceUpd);
                ViewBag.BTPOrderStateUpd = BTPOrderStateUpd;
                ViewBag.BTPOrderPriceUpd = BTPOrderPriceUpd;

                ViewBag.IsOrg = true;
            }

            try
            {
                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<List<Jinher.AMP.SNS.Deploy.CustomDTO.AppSceneUserApiDTO>> sceneUserList = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetSceneUserInfo(appId, UserId);
                if (sceneUserList != null && sceneUserList.Content != null && sceneUserList.Content.Count > 0)
                {
                    ViewBag.SceneId = sceneUserList.Content[0].SceneId;
                }
                else
                {
                    ViewBag.SceneId = Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, UserId), ex);
                ViewBag.SceneId = Guid.Empty;
            }
            return View();
        }

        /// <summary>
        /// 订单列表布局页
        /// </summary>
        /// <param name="priceLow"></param>
        /// <param name="priceHight"></param>
        /// <param name="seacrhContent"></param>
        /// <param name="SearchPhone"></param>
        /// <param name="dayCount"></param>
        /// <param name="state"></param>
        /// <param name="payment"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="esAppId"></param>
        /// <param name="orderSourceId"></param>
        /// <param name="marketing"></param>
        /// <returns></returns>
        public PartialViewResult OrderInfoList(string priceLow, string priceHight, string seacrhContent, string SearchPhone, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId, Guid? orderSourceId, int marketing)
        {
            ViewBag.OrderState = state;

            Guid appId = Guid.Empty;

            if (Request["isLowerPage"] != null)
            {
                appId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;                
            }
            else
            {
                appId = Guid.Parse(Request["APPID"].ToString());
            }
            CommodityOrderFacade cf = new CommodityOrderFacade();


            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request["pageSize"]))
            {
                if (!int.TryParse(Request["pageSize"], out pageSize))
                {
                    pageSize = 20;
                }
                else
                {
                    if (pageSize > 200)
                    {
                        pageSize = 200;//最大200条每次
                    }
                }
            }
            DateTime time = new DateTime();
            DateTime? startOrderTime = null;
            DateTime? endOrderTime = null;
            if (!string.IsNullOrWhiteSpace(startTime) && DateTime.TryParse(startTime, out time))
            {
                dayCount = "0";
                startOrderTime = time;
                if (!string.IsNullOrWhiteSpace(endTime) && DateTime.TryParse(endTime, out time))
                {
                    endOrderTime = time;
                }
            }
            SearchPhone = SearchPhone == "下单人手机号" ? "" : SearchPhone;
            seacrhContent = seacrhContent == "可输入订单编号、收货人姓名、收货人手机号、厂商名称查询" ? "" : seacrhContent;
            //var UserId = string.IsNullOrEmpty(SearchPhone) ? Guid.Empty : TPS.CBCSV.GetUserIdByAccount(SearchPhone);

            //var UserId = Guid.Parse(SearchPhone);
            //var list = new List<Guid>();
            //list.Add(UserId);
            //var phone = new TPS.CBCSVFacade().GetUserNameAccountsByIds(list);
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                PriceLow = priceLow,
                PriceHight = priceHight,
                SeacrhContent = seacrhContent,
                RegisterPhone = SearchPhone,
                UserId = String.IsNullOrEmpty(Request["isLowerPage"]) ? Guid.Parse(Request["UserId"]) : Guid.Empty,
                DayCount = dayCount,
                State = state,
                Payment = payment,
                StartOrderTime = startOrderTime,
                EndOrderTime = endOrderTime,
                EsAppId = esAppId,
                OrderSourceId = orderSourceId,
                Marketing = marketing
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);

            //Jinher.JAP.Common.Loging.LogHelper.Debug(String.Format("OrderInfoList获取数据量:{0}，参数{1}", searchResult.Data.Count, JsonHelper.JsSerializer(search)));


            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.EsOrderIds = searchResult.Message;
            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPOrderStateUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderStateUpd);
                bool BTPOrderPriceUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderPriceUpd);
                ViewBag.BTPOrderStateUpd = BTPOrderStateUpd;
                ViewBag.BTPOrderPriceUpd = BTPOrderPriceUpd;

                ViewBag.IsOrg = true;
            }
            CommodityOrderMoneyToSearch moneyToSearch = new CommodityOrderMoneyToSearch()
            {
                AppId = appId
            };
            var moneyToResult = cf.GetCommodityOrderMoneyTo(moneyToSearch);
            if (moneyToResult != null && moneyToResult.Count > 0 && moneyToResult.Data != null)
            {
                ViewBag.MoneyToList = moneyToResult.Data;
            }
            return PartialView();
        }


        #region 订单详情
        //[CheckAppId]
        public ActionResult CommodityOrderDetail(string commodityOrderId)
        {
            try
            {
                CommodityOrderFacade cf = new CommodityOrderFacade();
                CommodityOrderVM CommodityOrder = cf.GetCommodityOrder(new Guid(commodityOrderId), Guid.Empty);
                if (CommodityOrder != null)
                {
                    if (CommodityOrder.OrderType == 3 && CommodityOrder.State == 3)
                    {
                        var yjbjCardList = new YJBJCardFacade().Get(new Guid(commodityOrderId));
                        CommodityOrder.OrderItems.ForEach(p =>
                        {
                            p.YJBJCardList = yjbjCardList.Where(x => x.CommodityId == p.CommodityId && x.Status == 2).ToList();
                            if (p.YJBJCardList.Count == 0)
                            {
                                p.YJBJCardList = yjbjCardList.Where(x => x.CommodityId == p.CommodityId && x.Status == 1).OrderByDescending(x => x.SubTime).Take(1).ToList();
                            }
                            else
                            {
                                p.YJBJCardList.ForEach(x =>
                                {
                                    x.CheckCode = !string.IsNullOrEmpty(x.CheckCode) ? "******" : string.Empty;
                                });
                            }
                        });

                    }
                }
                ViewBag.BTPOrderStateUpd = Request["BTPOrderStateUpd"];
                ViewBag.IsShowChangeExp = Request["isShowChangeExp"];
                ViewBag.CommodityOrder = CommodityOrder;
                return View();
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("查看商品详情异常信息"), ex);
                return HttpNotFound();
            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetRefundInfo(string OrderItemId)
        {
            Guid itemid = Guid.Parse(OrderItemId);
            OrderRefundInfoManageFacade of = new OrderRefundInfoManageFacade();
            var RefundInfo = of.GetOrderRefundInfoByItemId(itemid);
            return Json(RefundInfo);
        }

    }
}
