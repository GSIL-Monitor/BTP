using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.EDMX;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityOrderController : BaseController
    {
        //
        // GET: /CommodityOrder/
        #region 订单查询
        [CheckAppId]
        public ActionResult Index()
        {
            Guid appId;

            if (System.Web.HttpContext.Current.Session["APPID"] == null)
            {
                Guid.TryParse(Request["appId"], out appId);
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }
            else
            {
                //appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
                appId = Guid.Parse(System.Web.HttpContext.Current.Session["APPID"].ToString());
            }

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
                PageIndex = pageIndex
            };
            var searchResult = cf.GetAllCommodityOrderByAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data;
            ViewBag.Count = searchResult.Count;
            if (commodityOrderList.Count() > 0)
            {
                foreach (var item in commodityOrderList)
                {
                    var commodityOrderService = CommodityOrderService.ObjectSet().FirstOrDefault(p => p.Code == item.CommodityOrderCode);
                    if (commodityOrderService != null)
                    {
                        item.StateAfterSales = commodityOrderService.State;
                    }
                }
            }
            ViewBag.commodityOrderList = commodityOrderList;
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
                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<List<Jinher.AMP.SNS.Deploy.CustomDTO.AppSceneUserApiDTO>> sceneUserList = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetSceneUserInfo(appId, this.ContextDTO.LoginUserID);
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
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
                ViewBag.SceneId = Guid.Empty;
            }
            return View();
        }

        #endregion

        #region 统计所有订单查询
        [CheckAppId]
        public ActionResult TotalIndex()
        {
            Guid appId;
            if (System.Web.HttpContext.Current.Session["APPID"] == null)
            {
                Guid.TryParse(Request["appId"], out appId);
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }
            else
            {
                appId = Guid.Parse(System.Web.HttpContext.Current.Session["APPID"].ToString());
            }

            #region 获取出库发货权限信息

            Guid UserId = Guid.Parse(Request["userId"]);
            var tuple = CBCSV.GetUserNameAndCode(UserId);
            Guid OrgId = Guid.Parse(Request["curChangeOrg"]);
            Jinher.AMP.EBC.IBP.Facade.EmployeeFacade employeefacade = new Jinher.AMP.EBC.IBP.Facade.EmployeeFacade();
            Jinher.AMP.EBC.IBP.Facade.RoleFeatureFacade rolefeaturefacade = new Jinher.AMP.EBC.IBP.Facade.RoleFeatureFacade();
            var employee = employeefacade.GetEmployeeInfo(OrgId, UserId);
            List<string> objstr = new List<string>();
            if (employee != null)
            {
                var arr = employee.Role.Split(',');
                foreach (var item in arr)
                {
                    //.Where(p => p.FeatureName.Contains("出库内容编辑") || p.FeatureName.Contains("发货内容编辑"))
                    //YunYingPingTai 运营平台下的所有选中的操作权限。
                    List<EBC.Deploy.CustomDTO.FeatureDTO> rolefeature = rolefeaturefacade.GetAllFeatureInfo(OrgId, Guid.Parse(item)).Where(x => x.ParentCode.Contains(CustomConfig.YlptFeatureCode) && x.IsCheck == true).ToList();
                    if (rolefeature != null && rolefeature.Any())
                    {
                        objstr.AddRange(rolefeature.Select(x => x.FeatureName));
                    }
                }
                objstr = objstr.Distinct().ToList();
            }
            ViewBag.objstr = objstr;

            #endregion

            #region 获取阳光餐饮的所有AppId

            Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
            List<Guid> Appids = storefacade.GetAppIdList("1");


            #endregion

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
                AppId = Guid.Empty,
                AppIds = Appids,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            var searchResult = cf.GetAllCommodityOrderByAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data;
            ViewBag.Count = searchResult.Count;
            List<string> ordercodelist = commodityOrderList.Select(p => p.CommodityOrderCode).ToList();

            #region 组建url

            Jinher.AMP.Store.ISV.Facade.OrderIdListFacade orderIdlistfacade = new Jinher.AMP.Store.ISV.Facade.OrderIdListFacade();
            Jinher.AMP.Store.Deploy.CustomDTO.OrderManage.OrderIdListInfo arrlist = orderIdlistfacade.GetOrderIdList(ordercodelist);
            foreach (var item in commodityOrderList)
            {
                arrlist.OrderIdList.ForEach(p =>
                {
                    if (item.CommodityOrderCode == p.OrderNumber)
                    {
                        item.IsExistence = p.IsExistence;
                    }
                });
                string ReciveAddress = item.Province + item.City + item.District + item.Street + item.ReceiptAddress;
                string url = "UserId=" + item.UserId + "&userName=" + item.ReceiptUserName + "&AppId=" + item.AppId + "&TrackNumber=" + item.ExpOrderNo + "&DeliveryUserId=" + item.ReceiptUserId + "&OrderNum=" + item.CommodityOrderCode + "&IsExistence=" + item.IsExistence + "&CommodityOrderId=" + item.CommodityOrderId + "&adminuserName=" + tuple.Item1 + "&receiverName=" + item.ReceiptUserName + "&ReciveAddress=" + ReciveAddress + "&ReceiptPhone=" + item.ReceiptPhone;
                StringBuilder sb = new StringBuilder();
                if (item.OrderItems.Count() > 0)
                {
                    sb.Append("[");
                    foreach (var _item in item.OrderItems)
                    {
                        #region 设置属性
                        string strComAttr = string.Empty;
                        if (!string.IsNullOrEmpty(_item.SizeAndColorId))
                        {
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("颜色", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("尺寸", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace(":", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("：", "");
                            string[] attrs = _item.SizeAndColorId.Split(new char[] { ',', '，' });
                            foreach (string attr in attrs)
                            {
                                if (attr != "null" && attr != "请选择")
                                {
                                    if (strComAttr != string.Empty)
                                    {
                                        strComAttr = string.Format("{0} {1}", strComAttr, attr);
                                    }
                                    else
                                    {
                                        strComAttr = attr;
                                    }

                                }
                            }
                        }
                        #endregion
                        sb.Append("{'GoodsId':'" + _item.CommodityId + "','GoodsName':'" + _item.CommodityIdName + "','GoodDes':'" + strComAttr + "','DeviceCount':'" + _item.Number + "'},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                    url += "&OrderItems=" + GetBase64(sb.ToString());
                }
                else
                {
                    url += "&OrderItems=";
                }
                item.Url = url;
            }
            ViewBag.commodityOrderList = commodityOrderList;


            #endregion

            CommodityOrderMoneyToSearch moneyToSearch = new CommodityOrderMoneyToSearch()
            {
                AppId = Guid.Empty,
                AppIds = Appids
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
                if (objstr != null && objstr.Count() > 0)
                {
                    BTPOrderStateUpd = true;
                }
                ViewBag.BTPOrderStateUpd = BTPOrderStateUpd;
                ViewBag.BTPOrderPriceUpd = BTPOrderPriceUpd;
                ViewBag.IsOrg = true;
            }

            try
            {
                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<List<Jinher.AMP.SNS.Deploy.CustomDTO.AppSceneUserApiDTO>> sceneUserList = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetSceneUserInfo(appId, this.ContextDTO.LoginUserID);
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
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
                ViewBag.SceneId = Guid.Empty;
            }
            return View();
        }

        #endregion

        //base64加密
        public string GetBase64(string str)
        {
            System.Text.Encoding encode = System.Text.Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(str);
            string strPath = Convert.ToBase64String(bytedata, 0, bytedata.Length);
            return strPath;
        }

        #region 统计订单查询

        [HttpPost]
        [CheckAppId]
        public PartialViewResult TotalPartialIndex(string priceLow, string priceHight, string seacrhContent, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId, Guid? orderSourceId)
        {

            #region 获取出库发货权限信息
            Guid UserId = Guid.Parse(Request["userId"]);
            var tuple = CBCSV.GetUserNameAndCode(UserId);
            Guid OrgId = Guid.Parse(Request["curChangeOrg"]);
            Jinher.AMP.EBC.IBP.Facade.EmployeeFacade employeefacade = new Jinher.AMP.EBC.IBP.Facade.EmployeeFacade();
            Jinher.AMP.EBC.IBP.Facade.RoleFeatureFacade rolefeaturefacade = new Jinher.AMP.EBC.IBP.Facade.RoleFeatureFacade();
            var employee = employeefacade.GetEmployeeInfo(OrgId, UserId);
            List<string> objstr = new List<string>();
            if (employee != null)
            {
                var arr = employee.Role.Split(',');
                foreach (var item in arr)
                {
                    //.Where(p => p.FeatureName.Contains("出库内容编辑") || p.FeatureName.Contains("发货内容编辑"))
                    //YunYingPingTai 运营平台下的所有选中的操作权限。
                    List<EBC.Deploy.CustomDTO.FeatureDTO> rolefeature = rolefeaturefacade.GetAllFeatureInfo(OrgId, Guid.Parse(item))
                        .Where(x => x.ParentCode.Contains(CustomConfig.YlptFeatureCode) && x.IsCheck == true).ToList();
                    if (rolefeature != null && rolefeature.Any())
                    {
                        objstr.AddRange(rolefeature.Select(x => x.FeatureName));
                    }
                }
                objstr = objstr.Distinct().ToList();
            }
            ViewBag.objstr = objstr;
            #endregion

            #region 获取阳光餐饮的所有AppId
            Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
            List<Guid> Appids = storefacade.GetAppIdList("1");
            #endregion

            ViewBag.OrderState = state;
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
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
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = Guid.Empty,
                AppIds = Appids,
                PageSize = pageSize,
                PageIndex = pageIndex,
                PriceLow = priceLow,
                PriceHight = priceHight,
                SeacrhContent = seacrhContent,
                DayCount = dayCount,
                State = state,
                Payment = payment,
                StartOrderTime = startOrderTime,
                EndOrderTime = endOrderTime,
                EsAppId = esAppId,
                OrderSourceId = orderSourceId
            };
            var searchResult = cf.GetAllCommodityOrderByAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data;

            #region 组建url
            List<string> ordercodelist = commodityOrderList.Select(p => p.CommodityOrderCode).ToList();
            Jinher.AMP.Store.ISV.Facade.OrderIdListFacade orderIdlistfacade = new Jinher.AMP.Store.ISV.Facade.OrderIdListFacade();
            Jinher.AMP.Store.Deploy.CustomDTO.OrderManage.OrderIdListInfo arrlist = orderIdlistfacade.GetOrderIdList(ordercodelist);
            foreach (var item in commodityOrderList)
            {

                arrlist.OrderIdList.ForEach(p =>
                {
                    if (item.CommodityOrderCode == p.OrderNumber)
                    {
                        item.IsExistence = p.IsExistence;
                    }
                });
                string ReciveAddress = item.Province + item.City + item.District + item.Street + item.ReceiptAddress;
                string url = "UserId=" + item.UserId + "&userName=" + item.ReceiptUserName + "&AppId=" + item.AppId + "&TrackNumber=" + item.ExpOrderNo + "&DeliveryUserId=" + item.ReceiptUserId + "&OrderNum=" + item.CommodityOrderCode + "&IsExistence=" + item.IsExistence + "&CommodityOrderId=" + item.CommodityOrderId + "&adminuserName=" + tuple.Item1 + "&receiverName=" + item.ReceiptUserName + "&ReciveAddress=" + ReciveAddress + "&ReceiptPhone=" + item.ReceiptPhone;
                StringBuilder sb = new StringBuilder();
                if (item.OrderItems.Count() > 0)
                {
                    sb.Append("[");
                    foreach (var _item in item.OrderItems)
                    {
                        #region 设置属性
                        string strComAttr = string.Empty;
                        if (!string.IsNullOrEmpty(_item.SizeAndColorId))
                        {
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("颜色", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("尺寸", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace(":", "");
                            _item.SizeAndColorId = _item.SizeAndColorId.Replace("：", "");
                            string[] attrs = _item.SizeAndColorId.Split(new char[] { ',', '，' });
                            foreach (string attr in attrs)
                            {
                                if (attr != "null" && attr != "请选择")
                                {
                                    if (strComAttr != string.Empty)
                                    {
                                        strComAttr = string.Format("{0} {1}", strComAttr, attr);
                                    }
                                    else
                                    {
                                        strComAttr = attr;
                                    }

                                }
                            }
                        }
                        #endregion
                        sb.Append("{'GoodsId':'" + _item.CommodityId + "','GoodsName':'" + _item.CommodityIdName + "','GoodDes':'" + strComAttr + "','DeviceCount':'" + _item.Number + "'},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                    url += "&OrderItems=" + GetBase64(sb.ToString());
                }
                else
                {
                    url += "&OrderItems=";
                }
                item.Url = url;
            }
            ViewBag.commodityOrderList = commodityOrderList;
            #endregion

            ViewBag.Count = searchResult.Count;
            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPOrderStateUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderStateUpd);
                bool BTPOrderPriceUpd = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOrderPriceUpd);
                if (objstr != null && objstr.Count() > 0)
                {
                    BTPOrderStateUpd = true;
                }
                ViewBag.BTPOrderStateUpd = BTPOrderStateUpd;
                ViewBag.BTPOrderPriceUpd = BTPOrderPriceUpd;

                ViewBag.IsOrg = true;
            }
            CommodityOrderMoneyToSearch moneyToSearch = new CommodityOrderMoneyToSearch()
            {
                AppId = Guid.Empty,
                AppIds = Appids
            };
            var moneyToResult = cf.GetCommodityOrderMoneyTo(moneyToSearch);
            if (moneyToResult != null && moneyToResult.Count > 0 && moneyToResult.Data != null)
            {
                ViewBag.MoneyToList = moneyToResult.Data;
            }
            return PartialView();
        }

        #endregion

        //
        // GET: /CommodityOrder/
        #region 订单查询
        [CheckAppId]
        public ActionResult EsAppIndex()
        {
            Guid appId;

            if (System.Web.HttpContext.Current.Session["APPID"] == null)
            {
                Guid.TryParse(Request["appId"], out appId);
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }
            else
            {
                //appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
                appId = Guid.Parse(System.Web.HttpContext.Current.Session["APPID"].ToString());
            }

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
                PageIndex = pageIndex
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.commodityOrderList = commodityOrderList;
            //ViewBag.EsOrderIds = searchResult.Message;

            SessionHelper.Del("orderids");
            SessionHelper.Add("orderids", searchResult.Message);
            

            
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
                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<List<Jinher.AMP.SNS.Deploy.CustomDTO.AppSceneUserApiDTO>> sceneUserList = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetSceneUserInfo(appId, this.ContextDTO.LoginUserID);
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
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
                ViewBag.SceneId = Guid.Empty;
            }

            return View();
        }

        #endregion
        #region 订单查询

        /// <summary>
        /// 
        /// <para>tips: 2018-04-13 张剑 添加优惠活动筛选项</para>
        /// </summary>
        /// <param name="priceLow"></param>
        /// <param name="priceHight"></param>
        /// <param name="seacrhContent"></param>
        /// <param name="dayCount"></param>
        /// <param name="state"></param>
        /// <param name="payment"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="esAppId"></param>
        /// <param name="orderSourceId"></param>
        /// <param name="marketing">优惠活动筛选项</param>
        /// <returns></returns>
        [HttpPost]
        [CheckAppId]
        public PartialViewResult PartialEsAppIndex(string priceLow, string priceHight, string seacrhContent, string registerPhone, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId, Guid? orderSourceId, int marketing)
        {
            ViewBag.OrderState = state;

            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
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
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                PriceLow = priceLow,
                PriceHight = priceHight,
                SeacrhContent = seacrhContent,
                RegisterPhone = registerPhone,
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
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.EsOrderIds = searchResult.Message;
            SessionHelper.Del("orderids");
            SessionHelper.Add("orderids", searchResult.Message);
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

        #endregion

        #region 订单交易明细查询(暂只用于易捷北京项目)
        public string GetPayTypeName(int payType)
        {
            switch (payType)
            {
                case 0: return "易捷抵用券";
                case 1:
                    return "支付宝";
                case 5:
                    return "银联";
                case 6:
                    return "微信";
                case 2001:
                    return "金采";
            }
            return payType.ToString();
        }
        public string GetAppTypeName(int appType)
        {
            switch (appType)
            {
                case 0:
                    return "自营他配";
                case 1:
                    return "第三方";
                case 2:
                    return "自营自配自采";
                case 3:
                    return "自营自配统采";
            }
            return appType.ToString();
        }

        [CheckAppId]
        public ActionResult OrderTradeList()
        {
            this.ViewBag.ESAppId = Request["AppId"];

            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="esAppId"></param>
        /// <param name="appTypes"></param>
        /// <param name="payTypes"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="betinDate"></param>
        /// <param name="endDate"></param>
        /// <param name="supplierCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private Models.OrderTradeInfos GetOrderTradeListTemp(Guid esAppId, string appTypes, string payTypes, Guid appId, string appName, string betinDate, string endDate, string supplierCode, int pageIndex, int pageSize)
        {
            Models.OrderTradeInfos result = new Models.OrderTradeInfos();
            if (string.IsNullOrEmpty(appTypes) || string.IsNullOrEmpty(payTypes))
            {
                return new Models.OrderTradeInfos();
            }
            var appTypeList = appTypes.TrimEnd(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var payTypeList = payTypes.TrimEnd(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            if (appTypeList.Count == 0 || payTypeList.Count == 0)
            {
                return new Models.OrderTradeInfos();
            }
            var beginTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (!string.IsNullOrEmpty(betinDate))
            {
                beginTime = DateTime.Parse(betinDate);
            }
            var endTime = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
            if (!string.IsNullOrEmpty(endDate))
            {
                endTime = DateTime.Parse(endDate).AddDays(1);
            }
            using (var yjbj = new EDMX.YJBJContainer())
            {
                var queryYJCoupon = payTypeList.Contains(0);

                var temp = yjbj.PayTransaction1.Where(p =>
                                                      p.TradeTime >= beginTime
                                                      && p.TradeTime < endTime
                                                      && appTypeList.Contains(p.AppType)
                                                      && payTypeList.Contains(p.PayType)
                                                      && p.EsAppId == esAppId);

                if (!string.IsNullOrEmpty(appName))
                {
                    temp = temp.Where(p => p.AppName.Contains(appName));
                }
                else if (appId != Guid.Empty)
                {
                    temp = temp.Where(p => p.AppId == appId);
                }
                if (!string.IsNullOrEmpty(supplierCode))
                {
                    temp = temp.Where(p => p.SupplierCode == supplierCode);
                }
                result.Count = temp.Count();
                var list = temp.Select(p => new Models.OrderTradeInfo
                {
                    TradeId = p.TradeId,
                    SupplierCode = p.SupplierCode,
                    SupplierName = p.SupplierName,
                    AppName = p.AppName,
                    AppType = p.AppType,
                    OrderState = p.OrderState,
                    OrderId = p.OrderId,
                    OrderCode = p.OrderCode,
                    TradeTime = p.TradeTime,
                    TradeNum = p.TradeNum,
                    PayMoney = p.TradeResult == 4 ? p.PayMoney : -p.RefundMoney,
                    PayType = p.PayType,
                    FreightMoney = p.TradeResult == 4 ? p.FreightMoney : -p.RefundFreightMoney,
                    YJCouponMoney = p.YJCouponMoney ?? 0
                }).Distinct().OrderByDescending(p => p.TradeTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                var tradeIdList = list.Select(p => p.TradeId).ToList();
                var commodityList = yjbj.OrderItem1
                    .Where(p => tradeIdList.Contains(p.TradeId))
                    .Select(p => new { p.TradeId, p.CommodityName, p.Number, p.CostPrice })
                    .ToList()
                    .Select(p => new OrderItem1 { TradeId = p.TradeId, CommodityName = p.CommodityName, Number = p.Number, CostPrice = p.CostPrice })
                    .ToList();
                list.ForEach(p =>
                {
                    p.AppTypeStr = GetAppTypeName(p.AppType);
                    p.PayTypeStr = GetPayTypeName(p.PayType);
                    p.CommodityMoney = p.PayMoney + (p.YJCouponMoney ?? 0) - p.FreightMoney;
                    p.CommodityList = commodityList.Where(x => x.TradeId == p.TradeId).ToList();
                    p.CommodityInfo = string.Empty;
                    p.CommodityNumber = string.Empty;
                    p.CommodityCostPrice = string.Empty;
                    p.CommodityCostpurchase = string.Empty;
                    if (p.OrderState == "退款成功")
                    {
                        p.CommodityList.ForEach(x =>
                        {
                            p.CommodityInfo += x.CommodityName + "<hr>";
                            p.CommodityNumber += "-" + x.Number + "<hr>";
                            p.CommodityCostPrice += "-" + (x.CostPrice.HasValue ? x.CostPrice.Value.ToString() : "&nbsp;") + "<hr>";
                            p.CommodityCostpurchase += "-" + (x.Number * x.CostPrice) + "<hr>";
                        });
                    }
                    else
                    {
                        p.CommodityList.ForEach(x =>
                        {

                            p.CommodityInfo += x.CommodityName + "<hr>";
                            p.CommodityNumber += x.Number + "<hr>";
                            p.CommodityCostPrice += (x.CostPrice.HasValue ? x.CostPrice.Value.ToString() : "&nbsp;") + "<hr>";
                            p.CommodityCostpurchase += (x.Number * x.CostPrice) + "<hr>";

                        });
                    }

                    if (p.CommodityInfo.EndsWith("<hr>")) p.CommodityInfo = p.CommodityInfo.Substring(0, p.CommodityInfo.Length - 4);
                    if (p.CommodityNumber.EndsWith("<hr>")) p.CommodityNumber = p.CommodityNumber.Substring(0, p.CommodityNumber.Length - 4);
                    if (p.CommodityCostPrice.EndsWith("<hr>")) p.CommodityCostPrice = p.CommodityCostPrice.Substring(0, p.CommodityCostPrice.Length - 4);
                    if (p.CommodityCostpurchase.EndsWith("<hr>")) p.CommodityCostpurchase = p.CommodityCostpurchase.Substring(0, p.CommodityCostpurchase.Length - 4);
                    p.TradeTimeStr = p.TradeTime.ToString("yyyy-MM-dd HH:mm:ss");
                });

                result.Data = list;
                return result;
            }
        }


        [GridAction]
        public ActionResult GetOrderTradeList(Guid esAppId, string appTypes, string payTypes, Guid appId, string appName, string betinDate, string endDate, string supplierCode)
        {
            int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            int pageSize = Request["rows"] == null ? 100 : int.Parse(Request["rows"]);
            Models.OrderTradeInfos list = GetOrderTradeListTemp(esAppId, appTypes, payTypes, appId, appName, betinDate, endDate, supplierCode, pageIndex, pageSize);
            List<string> propertyNameList = new List<string>
            {
                "OrderId",
                "SupplierCode",
                "SupplierName",
                "AppName",
                "AppTypeStr",
                "CommodityInfo",
                "CommodityNumber",
                "CommodityCostPrice",
                "CommodityCostpurchase",
                "OrderState",
                "OrderCode",
                "TradeTimeStr",
                "TradeNum",
                "PayMoney",
                "PayTypeStr",
                "CommodityMoney",
                "FreightMoney",
                "YJCouponMoney"
            };
            return View(new GridModel<Models.OrderTradeInfo>(propertyNameList, list.Data, list.Count, pageIndex, pageSize, string.Empty));
        }


        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="esAppId"></param>
        /// <param name="appTypes"></param>
        /// <param name="payTypes"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="betinDate"></param>
        /// <param name="endDate"></param>
        /// <param name="supplierCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOrderTradeLists(Guid esAppId, string appTypes, string payTypes, Guid appId, string appName, string betinDate, string endDate, string supplierCode)
        {
            Models.OrderData result = new Models.OrderData();
            result.TotalCommodityCostPrice = 0;
            result.TotalCommodityCostpurchase = 0;
            result.TotalCommodityMoney = 0;
            result.TotalCommodityNumber = 0;
            result.TotalFreightMoney = 0;
            result.TotalPayMoney = 0;
            result.TotalYJCouponRefundMoney = 0;
            result.TotalYJCouponUseMoney = 0;
            if (string.IsNullOrEmpty(appTypes) || string.IsNullOrEmpty(payTypes))
            {
                return Json(result);
            }
            var objlist = GetOrderTradeListTemps(esAppId, appTypes, payTypes, appId, appName, betinDate, endDate, supplierCode);
            var tradeListTemp = objlist.OrderBy(p => p.TradeId).ThenByDescending(t => t.TradeTime).ToList();

            var tradeList = new List<Models.OrderTradeInfoData>();
            string tradeId = string.Empty;

            foreach (var item in tradeListTemp)
            {
                if (item.TradeId != tradeId)
                {
                    tradeId = item.TradeId;
                    item.FreightMoney = (item.PayMoney > 0 ? item.FreightMoney : -item.RefundFreightMoney);
                    item.CommodityMoney = item.PayMoney + (item.YJCouponMoney ?? 0) - item.FreightMoney;
                    tradeList.Add(item);
                }
                if (item.OrderState == "退款成功")
                {
                    result.TotalCommodityNumber += (-item.Number);
                    result.TotalCommodityCostPrice += (-Convert.ToDecimal(item.CostPrice ?? 0));
                    result.TotalCommodityCostpurchase += (-Convert.ToDecimal(item.Number * item.CostPrice ?? 0));
                }
                else
                {
                    result.TotalCommodityNumber += item.Number;
                    result.TotalCommodityCostPrice += Convert.ToDecimal(item.CostPrice ?? 0);
                    result.TotalCommodityCostpurchase += Convert.ToDecimal(item.Number * item.CostPrice ?? 0);
                }
            }

            result.TotalPayMoney = tradeList.Sum(p => p.PayMoney);
            result.TotalCommodityMoney = (tradeList.Sum(p => p.CommodityMoney) ?? 0);
            result.TotalFreightMoney = tradeList.Sum(p => p.FreightMoney);
            result.TotalYJCouponUseMoney = tradeList.Where(p => p.YJCouponMoney > 0).Sum(p => p.YJCouponMoney.Value);
            result.TotalYJCouponRefundMoney = tradeList.Where(p => p.YJCouponMoney < 0).Sum(p => p.YJCouponMoney.Value);
            return Json(result);
        }



        /// <summary>
        /// 
        /// <para>tips: 2018-04-26 张剑 添加查询"易捷抵用券"的查询条件.</para>
        /// </summary>
        /// <param name="esAppId"></param>
        /// <param name="appTypes"></param>
        /// <param name="payTypes"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="betinDate"></param>
        /// <param name="endDate"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public ActionResult ExportOrderTradeListExcel(Guid esAppId, string appTypes, string payTypes, Guid appId, string appName, string betinDate, string endDate, string supplierCode)
        {

            JdOrderItemFacade facade = new JdOrderItemFacade();
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "供应商编码", "供应商名称", "商户名称", "商户类型", "商品名称", "数量", "进货价", "进货金额", "交易状态", "订单编号",  "订单时间", "订单交易流水号", "订单总金额", "支付方式", "订单商品金额", "订单运费金额", "易捷抵用券金额" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            var objlist = GetOrderTradeListTemps(esAppId, appTypes, payTypes, appId, appName, betinDate, endDate, supplierCode);
            var tradeListTemp = objlist.OrderBy(p => p.TradeId).ThenByDescending(t => t.TradeTime).ToList();

            var tradeList = new List<Models.OrderTradeInfoData>();
            string tradeId = string.Empty;

            var commodityList = new List<Models.OrderTradeInfoData>();

            foreach (var trade in tradeListTemp)
            {
                if (trade.TradeId != tradeId)
                {
                    tradeId = trade.TradeId;
                    trade.FreightMoney = (trade.PayMoney > 0 ? trade.FreightMoney : -trade.RefundFreightMoney);
                    trade.CommodityMoney = trade.PayMoney + (trade.YJCouponMoney ?? 0) - trade.FreightMoney;
                    tradeList.Add(trade);

                    if (commodityList.Count > 0)
                        GetTradeListHtml(commodityList, sbHtml, commodityList[0]);

                    commodityList.Clear();
                    commodityList.Add(trade);
                }
                else
                {
                    commodityList.Add(trade);
                }
            }

            if (commodityList.Count > 0)
                GetTradeListHtml(commodityList, sbHtml, commodityList[0]);

            return File(Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("订单交易明细_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        public void GetTradeListHtml(List<Models.OrderTradeInfoData> commodityList,StringBuilder sbHtml, Models.OrderTradeInfoData trade)
        {
            var rowspan = commodityList.Count();
            for (int i = 0; i < commodityList.Count(); i++)
            {
                var commodity = commodityList[i];
                sbHtml.Append("<tr>");
                if (i == 0)
                {
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.SupplierCode);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.SupplierName);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.AppName);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, GetAppTypeName(trade.AppType));
                }
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", commodity.CommodityName);
                if (trade.OrderState != "退款成功")
                {
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", commodity.Number);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", commodity.CostPrice);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", (commodity.Number * commodity.CostPrice));
                }
                else
                {
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", -commodity.Number);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", -commodity.CostPrice);
                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", -(commodity.Number * commodity.CostPrice));
                }
                if (i == 0)
                {
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.OrderState);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>~{1}</td>", rowspan, trade.OrderCode);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.TradeTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>~{1}</td>", rowspan, trade.TradeNum);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, (trade.PayMoney > 0 ? trade.PayMoney : -trade.RefundMoney));
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, GetPayTypeName(trade.PayType));
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.CommodityMoney);
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, (trade.PayMoney > 0 ? trade.FreightMoney : -trade.RefundFreightMoney));
                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", rowspan, trade.YJCouponMoney);
                }
                sbHtml.Append("</tr>");
            }
        }


        private List<Models.OrderTradeInfoData> GetOrderTradeListTemps(Guid esAppId, string appTypes, string payTypes, Guid appId, string appName, string betinDate, string endDate, string supplierCode)
        {
            if (string.IsNullOrEmpty(appTypes) || string.IsNullOrEmpty(payTypes))
            {
                return new List<Models.OrderTradeInfoData>();
            }
            var appTypeList = appTypes.TrimEnd(',');
            var payTypeList = payTypes.TrimEnd(',');
            var beginTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (!string.IsNullOrEmpty(betinDate))
            {
                beginTime = DateTime.Parse(betinDate);
            }
            var endTime = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
            if (!string.IsNullOrEmpty(endDate))
            {
                endTime = DateTime.Parse(endDate).AddDays(1);
            }
            using (var yjbj = new EDMX.YJBJContainer())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" select s1.TradeId,s2.SupplierCode,s2.SupplierName,s2.AppName,s2.AppType,");
                sb.Append(" s1.CommodityName,s1.Number,s1.CostPrice,(s1.Number*s1.CostPrice) as TotalPrice,s2.OrderState,s2.OrderCode,s2.TradeTime,s2.TradeNum,(case when s2.TradeResult = 4 then s2.PayMoney  else -s2.RefundMoney end ) as PayMoney,");
                sb.Append(" s2.PayType,");
                sb.Append(" ((case when s2.TradeResult = 4 then s2.PayMoney  else -s2.RefundMoney end )+ISNULL(s2.YJCouponMoney,0) - s2.FreightMoney)  as CommodityMoney,s2.RefundMoney,s2.FreightMoney,s2.RefundFreightMoney,s2.YJCouponMoney");
                sb.Append(" from  [dbo].[OrderItem1] s1 ");
                sb.Append(" left join [dbo].[PayTransaction1] s2 on s1.TradeId=s2.TradeId ");
                sb.Append(" where 1=1 ");
                sb.Append(" and s2.TradeTime>='" + beginTime + "' and  s2.TradeTime<'" + endTime + "' ");
                if (!string.IsNullOrWhiteSpace(appTypeList))
                {
                    sb.Append(" and s2.AppType in (" + appTypeList + ") ");
                }
                if (!string.IsNullOrWhiteSpace(payTypeList))
                {
                    sb.Append(" and s2.PayType in (" + payTypeList + ") ");
                }
                if (esAppId != Guid.Empty)
                {
                    sb.Append(" and s2.EsAppId='" + esAppId + "' ");
                }
                if (!string.IsNullOrEmpty(supplierCode))
                {
                    sb.Append(" and s2.supplierCode='" + supplierCode + "' ");
                }
                if (!string.IsNullOrEmpty(appName))
                {
                    sb.Append(" and s2.AppName='" + appName + "' ");
                }
                sb.Append(" order by s2.TradeTime desc ");
                LogHelper.Debug(string.Format("sql语句:{0}",sb.ToString()));
                var objlist = yjbj.ExecuteStoreQuery<Models.OrderTradeInfoData>(sb.ToString()).ToList();
                return objlist;
            }
        }


        public JsonResult GetSupplierInfo(Guid esAppId, string type)
        {
            var isCode = type == "code";
            var list = new ISV.Facade.CommodityForYJBFacade().GetSupplierInfoList(esAppId)
                .Select(p => new { p.SupplierCode, p.SupplierName })
                .Distinct()
                .Select(p => new Models.BigAutocomplete
                {
                    title = isCode ? p.SupplierCode : p.SupplierName,
                    result = isCode ? p.SupplierName : p.SupplierCode
                }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }

        public JsonResult GetMallAppInfo(Guid esAppId)
        {
            var list = new ISV.Facade.MallApplyFacade().GetMallApplyInfoList(esAppId)
                .Select(p => new Models.BigAutocomplete
                {
                    title = p.AppName,
                    result = p.AppId.ToString()
                }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }



        //用户每日交易排名前5导出excel
        public ActionResult UserDayTradeRankExcel(string date)
        {
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "用户帐号", "交易总额", "商户名称", "交易状态", "订单编号", "交易时间", "支付网关支付流水号", "交易金额" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            using (var yjbj = new EDMX.YJBJContainer())
            {
                var userRankList = yjbj.PayTransaction1
                    .Where(p => p.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && p.TradeDate == date)
                    .GroupBy(p => p.PayorId)
                    .Select(p => new
                    {
                        UserId = p.Key.Value,
                        TradeMoney = p.Sum(x => x.PayMoney - x.RefundMoney)
                    })
                    .Where(p => p.TradeMoney >= 100)
                    .OrderByDescending(p => p.TradeMoney)
                    .Take(5).ToList();
                var userIdList = userRankList.Select(p => p.UserId).ToList();
                var userTradeList = yjbj.PayTransaction1
                    .Where(p => p.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && p.TradeDate == date && userIdList.Contains(p.PayorId.Value))
                    .Select(p => new
                    {
                        UserId = p.PayorId.Value,
                        p.AppName,
                        p.OrderState,
                        p.OrderCode,
                        p.TradeTime,
                        p.TradeNum,
                        TradeMoney = p.PayMoney - p.RefundMoney
                    });
                var userAccountList = CBCSV.Instance.GetUserInfoWithAccountList(userIdList);
                if (userAccountList == null || userAccountList.Count <= 0) return File(Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("用户日交易排名_{0}.xls", date));
                foreach (var rank in userRankList)
                {
                    var tradeList = userTradeList.Where(p => p.UserId == rank.UserId).ToList();
                    for (var i = 0; i < tradeList.Count; i++)
                    {
                        var trade = tradeList[i];
                        sbHtml.Append("<tr>");
                        if (i == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", tradeList.Count, userAccountList.Where(x => x.UserId == rank.UserId).Select(x => x.Account).FirstOrDefault());
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", tradeList.Count, rank.TradeMoney);
                        }
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", trade.AppName);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", trade.OrderState);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", trade.OrderCode);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", trade.TradeTime);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", trade.TradeNum);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", trade.TradeMoney);
                        sbHtml.Append("</tr>");
                    }
                }
            }
            return File(Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/vnd.ms-excel", string.Format("用户日交易排名_{0}.xls", date));
        }
        #endregion

        #region 订单查询

        [HttpPost]
        [CheckAppId]
        public PartialViewResult PartialIndex(string priceLow, string priceHight, string seacrhContent, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId, Guid? orderSourceId)
        {
            ViewBag.OrderState = state;

            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
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
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                PriceLow = priceLow,
                PriceHight = priceHight,
                SeacrhContent = seacrhContent,
                DayCount = dayCount,
                State = state,
                Payment = payment,
                StartOrderTime = startOrderTime,
                EndOrderTime = endOrderTime,
                EsAppId = esAppId,
                OrderSourceId = orderSourceId
            };

            var searchResult = cf.GetAllCommodityOrderByAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data;

            if (commodityOrderList.Count()>0)
            {
                foreach (var item in commodityOrderList)
                {
                    var commodityOrderService = CommodityOrderService.ObjectSet().FirstOrDefault(p => p.Code == item.CommodityOrderCode);
                    if (commodityOrderService != null)
                    {
                        item.StateAfterSales = commodityOrderService.State;
                        item.ModifiedOn = commodityOrderService.ModifiedOn;
                    }
                }
            }
           
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.Count = searchResult.Count;
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

        #endregion

        #region 修改订单
        public ActionResult UpdateCommodityOrder(Guid commodityOrderId, int state, string message, Guid userId, Guid orderItemId)
        {
            decimal rejectFreightMoney;
            decimal.TryParse(Request["rejectFreightMoney"], out rejectFreightMoney);

            CommodityOrderFacade cf = new CommodityOrderFacade();

            UpdateCommodityOrderParamDTO dto = new UpdateCommodityOrderParamDTO();
            dto.orderId = commodityOrderId;
            dto.targetState = state;
            dto.remessage = message;
            dto.userId = userId;
            dto.orderItemId = orderItemId;
            dto.RejectFreightMoney = rejectFreightMoney;

            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.CancelTheOrder(dto);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }
        #endregion

        #region 批量修改订单状态为出库中
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityOrderIds"></param>
        /// <returns></returns>
        public ActionResult BatchUpdateCommodityOrder(string commodityOrderIds)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.BatchUpdateCommodityOrder(commodityOrderIds);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }
        #endregion

        #region 修改订单总价
        public ActionResult UpdateOrderPrice(Guid orderId, decimal price, Guid userId)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.UpdateOrderPrice(orderId, price, userId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion

        #region 订单详情
        [CheckAppId]
        public ActionResult CommodityOrderDetail(string commodityOrderId)
        {
            try
            {
                var CommodityOrderId = new Guid(commodityOrderId);
                CommodityOrderFacade cf = new CommodityOrderFacade();
                CommodityOrderVM CommodityOrder = cf.GetCommodityOrder(CommodityOrderId, Guid.Empty);
                if (CommodityOrder != null)
                {
                    if (CommodityOrder.OrderType == 3 && CommodityOrder.State == 3)
                    {
                        #region 易捷卡密订单
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
                        #endregion
                    }
                    else
                    {
                        #region 第三方电商数据
                        var isThirdECommerceOrder = ThirdECommerceHelper.IsThirdECommerceOrder(CommodityOrder.AppId, CommodityOrder.CommodityOrderId);
                        if (isThirdECommerceOrder)
                        {
                            var skuList = YXOrderHelper.GetYXExpressDetailInfoSkuList(CommodityOrder.CommodityOrderId);
                            var packageList = YXOrderHelper.GetYXOrderPackageList(CommodityOrder.CommodityOrderId);
                            CommodityOrder.OrderItems.ForEach(orderItem =>
                            {
                                var sku = skuList.FirstOrDefault(s => s.OrderItemId == orderItem.Id);
                                if (sku != null)
                                {
                                    var package = packageList.Where(p => p.PackageId == sku.PackageId).FirstOrDefault();
                                    orderItem.JdorderId = sku.ExpressNo;
                                    orderItem.ExpressCompany = sku.ExpressCompany;
                                    if (package != null) orderItem.JdfhTime = package.ExpCreateTime ?? DateTime.MinValue;
                                }
                            });
                        }
                        #endregion
                    }
                }
                ViewBag.BTPOrderStateUpd = Request["BTPOrderStateUpd"];
                ViewBag.IsShowChangeExp = Request["isShowChangeExp"];
                ViewBag.CommodityOrder = CommodityOrder;
                ViewBag.IsSuNingOrder =  false;
                ViewBag.SNLogistics = null;
                ViewBag.SNOrderNo = null;
                if (ThirdECommerceHelper.IsSuNingYiGou(CommodityOrder.AppId))
                {
                    ViewBag.IsSuNingOrder = true;
                    var SuningData = SNOrderItem.ObjectSet().Where(w => w.OrderId == CommodityOrderId).ToList();
                    if (SuningData != null)
                    {
                        if (SuningData.Count > 0)
                        {                            
                            var QueryParmas = SuningData.GroupBy(g => g.CustomOrderId)
                            .Select(s => new SNQueryParamsDto
                            {
                                OrderId = s.Key,
                                OrderItemIds = s.ToList().Select(ss => new SNQueryItemParamsDto
                                {
                                    SkuId = ss.CustomSkuId,
                                    OrderItemId = ss.CustomOrderItemId
                                }).ToList()
                            }).FirstOrDefault();
                            ViewBag.SNOrderNo = QueryParmas.OrderId;
                            ViewBag.SNLogistics = SuningSV.suning_govbus_orderlogistnew_get(QueryParmas);
                        }
                    }                    
                }
                return View();
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("查看商品详情异常信息"), ex);
                return HttpNotFound();
            }
        }
        #endregion

        #region 手动发货
        [CheckAppId]
        public bool ManualDelivery(string commodityOrderId)
        {
            bool flag = false;
            var DManual = new YJBJCardFacade().Create(new Guid(commodityOrderId));
            flag = DManual.isSuccess;
            return flag;
        }
        #endregion

        #region other

        /// <summary>
        /// 导出订单查询结果
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public void ExportOrders(string orderIds)
        {

            try
            {

                if (string.IsNullOrEmpty(orderIds))
                {
                    orderIds = Request["orderIdsWord"];
                    if (string.IsNullOrEmpty(orderIds))
                    {
                        return;
                    }
                }
                List<Guid> importOrderIds = new List<Guid>();
                foreach (string strOrderId in orderIds.Split(','))
                {
                    Guid guidOrderId = Guid.Empty;
                    if (Guid.TryParse(strOrderId, out guidOrderId))
                    {
                        importOrderIds.Add(guidOrderId);
                    }
                }
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = orderBP.ImportOrder(importOrderIds);

                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("\\"))
                {
                    int startIndex = result.Message.LastIndexOf("\\");
                    string fileName = result.Message.Substring(startIndex + 1 + 36 + 6);

                    ////return File(result.Message, "text/plain", fileName);
                    //return File(result.Message, "application/msword", fileName);

                    //设置Http的头信息,编码格式
                    Response.Charset = "UTF-8";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=dingdan" + fileName);
                    Response.ContentType = "application/ms-word";


                    //关闭控件的视图状态           
                    //source.Page.EnableViewState = false;
                    //初始化HtmlWriter              
                    //System.IO.StringWriter writer = new System.IO.StringWriter();
                    //System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(writer);
                    //using (FileStream stream = new FileStream(result.Message, FileMode.Open))
                    //{
                    //    int fileLength = Convert.ToInt32(stream.Length);

                    //    byte[] fileData = new byte[fileLength];
                    //    stream.Read(fileData, 0, fileLength);

                    Response.WriteFile(result.Message);
                    Response.End();
                    //}
                    //source.RenderControl(htmlWriter);
                    //输出      
                }
                //else
                //{
                //    return null;
                //}
                //return null;

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("导出订单异常。orderIds：{0}", orderIds), ex);

            }
        }


        /// <summary>
        /// 获取导出word或者excel数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state">99为代自提</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult GetOrderIds(string startTime, string endTime, string state, string appid)
        {
            try
            {
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                ExportParamDTO param = new ExportParamDTO();
                param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                param.State = state;
                param.AppId = Guid.Parse(appid);
                List<Guid> orderIds = orderBP.GetOrderIds(param);
                string tempOrderIds = string.Empty;
                if (orderIds.Count() > 0)
                {
                    foreach (Guid order in orderIds)
                    {
                        tempOrderIds += order.ToString() + ",";
                    }
                }
                return Json(new { Result = true, Messages = tempOrderIds });
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单导出GetOrderIds出错", ex);
                return Json(new { Result = false, Messages = ex.Message }); ;
            }
        }




        /// <summary>
        /// 获取导出word或者excel的统一订单数据(所有阳光餐饮的数据)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state">99为代自提</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult GetTotalOrderIds(string startTime, string endTime, string state)
        {
            LogHelper.Info(string.Format("订单导出GetTotalOrderIds入参参数:{0},{1},{2}",startTime,endTime,state));
            try
            {  
                Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
                List<Guid> Appids = storefacade.GetAppIdList("1");
                string tempOrderIds = string.Empty;
                List<string> arr = new List<string>();
                if (state.Contains("|"))
                {
                    arr = state.Split('|').ToList();
                }
                else
                {
                    arr.Add(state);
                }
                LogHelper.Info(string.Format("订单导出GetTotalOrderIds入参参数arr:{0}", arr.ToString()));
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                if (arr.Any())
                {
                    arr.ForEach(s =>
                    {
                        ExportParamDTO param = new ExportParamDTO();
                        param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                        param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                        param.State = s.Trim();
                        param.AppIds = Appids;
                        List<Guid> orderIds = orderBP.GetTotalOrderIds(param);
                        LogHelper.Info(string.Format("订单导出GetTotalOrderIds入参参数orderIds中count:{0}", orderIds.Count()));
                        if (orderIds.Any())
                        {
                            orderIds.ForEach(p =>
                            {
                                tempOrderIds += p.ToString() + ",";
                            });
                        }
                    });
                }
                return Json(new { Result = true, Messages = tempOrderIds });
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单导出GetTotalOrderIds出错", ex);
                return Json(new { Result = false, Messages = ex.Message }); ;
            }
        }




        /// <summary>
        /// 获取导出word 数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state">99为代自提</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult GetEsOrderIds(string startTime, string endTime, string state, string appid)
        {
            try
            {
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                ExportParamDTO param = new ExportParamDTO();
                param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                param.State = state;
                param.AppId = Guid.Parse(appid);
                List<Guid> orderIds = orderBP.GetEsOrderIds(param);
                string tempOrderIds = string.Empty;
                if (orderIds.Count() > 0)
                {
                    foreach (Guid order in orderIds)
                    {
                        tempOrderIds += order.ToString() + ",";
                    }
                }
                return Json(new { Result = true, Messages = tempOrderIds });
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单导出GetOrderIds出错", ex);
                return Json(new { Result = false, Messages = ex.Message }); ;
            }
        }


        /// <summary>
        /// 导出word
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public void ExportWordData(string startTime, string endTime, string state, string appid)
        {
            try
            {
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                ExportParamDTO param = new ExportParamDTO();
                param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                param.State = state;
                param.AppId = Guid.Parse(appid);
                List<Guid> orderIds = orderBP.GetOrderIds(param);

                Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = orderBP.ImportOrder(orderIds);

                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("\\"))
                {
                    int startIndex = result.Message.LastIndexOf("\\");
                    string fileName = result.Message.Substring(startIndex + 1 + 36 + 6);

                    //设置Http的头信息,编码格式
                    Response.Charset = "UTF-8";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=dingdan" + fileName);
                    Response.ContentType = "application/ms-word";

                    Response.WriteFile(result.Message);
                    Response.End();

                }
            }
            catch (Exception ex)
            {
                string errStack = ex.Message + ex.StackTrace;
                while (ex.InnerException != null)
                {
                    errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                    ex = ex.InnerException;
                }
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("导出订单异常。startTime：{0}，endTime：{1}，state：{2}，appid：{3}", startTime, endTime, state, appid), ex);

            }
        }

        /// <summary>
        /// 导出订单(excel)
        /// </summary>
        /// <returns></returns>
        public FileResult ExportExcelData()
        {
            string startTime = Request["startTimeExcel"];
            string endTime = Request["endTimeExcel"];
            string state = Request["stateExcel"];
            string appid = Request["appidExcel"];
            string orderids = Request["orderIdsExcel"];
            OrderFieldFacade ordersetfacde = new OrderFieldFacade();
            var filed = ordersetfacde.GetOrderSet(Guid.Parse(appid));
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "ID(序列号)", "订单ID", "订单编号", "苏宁订单编号", "商品名称", "商品单价", "购买数量/件", "进货价", "结算价", "下单时间", "下单账号", "下单昵称", "付款时间", "付款方式", "实付款", "已退款", "收货人", "手机号", "收货地址", "邮编", "物流公司", "快递单号", "买家备注","商家备注","渠道佣金", "分销佣金", "含众筹佣金", "众销佣金", "含运费", "使用代金券金额", "使用优惠券金额", "积分抵现", "使用金币金额", "推广主佣金", "应用主佣金", "订单状态", "是否自提", "自提点", "订单来源" };
            if (filed != null)
            {
                if (!string.IsNullOrEmpty(filed.FirstField))
                {
                    lstTitle.Add(filed.FirstField);
                }
                if (!string.IsNullOrEmpty(filed.SecondField))
                {
                    lstTitle.Add(filed.SecondField);
                }
                if (!string.IsNullOrEmpty(filed.ThirdField))
                {
                    lstTitle.Add(filed.ThirdField);
                }
            }
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            try
            {
                ExportParamDTO param = new ExportParamDTO();
                param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                param.State = state;
                param.AppId = Guid.Parse(appid);
                param.orderIds = orderids;

                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                List<ExportResultDTO> result = orderBP.ExportResult(param);
                int i = 1, orderItemCount = 0, j = 0;
                foreach (ExportResultDTO model in result)
                {
                    orderItemCount = model.Products == null ? 0 : model.Products.Count;
                    j = 0;
                    foreach (ProductList product in model.Products)
                    {
                        if (model.State == 7 || model.StateAfterSales == 7)
                        {
                            model.ChannelShareMoney = 0;
                        }
                        sbHtml.Append("<tr>");
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, i);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.CommodityOrderId);
                            if (model.Code.Contains(","))
                            {
                                string[] codes = model.Code.Split(',');
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, codes[0]);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, codes[1].Replace("苏宁订单编号：",""));
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Code);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, "无");
                            }
                            
                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.CommodityOrderId);
                            if (model.Code.Contains(","))
                            {
                                string[] codes = model.Code.Split(',');
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, codes[0]);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, codes[1].Replace("苏宁订单编号：", ""));
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Code);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, "无");
                            }
                        }
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", product.ProductName);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.ProductPric);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.BuyNumber);
                        if (product.CostPrice == -1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                        }
                        else
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.CostPrice * product.BuyNumber);
                        }
                        if (product.ManufacturerClearingPrice == -1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                        }
                        else
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.ManufacturerClearingPrice * product.BuyNumber);
                        }
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OrdersTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Ucode);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Uname);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentType);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PracticalPayment);
                            if (model.State == 7 && model.RefundMoney + model.RefundScoreMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}（含{2}元积分）</td>", orderItemCount, model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, "");
                            }
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Payer);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Phone);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ShippingAddress);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Postcode);

                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ShipExpCo);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ExpOrderNo);

                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.remark);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SellersRemark);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.ChannelShareMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.DistributeMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.CrowdfundingPrice);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Commission);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Freight);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.GoldCoupon);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.CouponValue);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.SpendScoreMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.GoldPrice);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.SpreadGold / 1000);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OwnerShare);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, GetOrderSelfTakeFlag(model.SelfTakeFlag));
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SelfTakeAddress);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.EsAppName);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.FirstContent);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SecondContent);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ThirdContent);

                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrdersTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Ucode);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Uname);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PaymentTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.PaymentType);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PracticalPayment);
                            if (model.State == 7 && model.RefundMoney >= 0 || model.State == 3 && model.StateAfterSales == 7 && model.RefundMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}（含{1}元积分）</td>", model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                            }
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Payer);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Phone);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ShippingAddress);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Postcode);

                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ShipExpCo);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ExpOrderNo);

                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.remark);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.SellersRemark);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.ChannelShareMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DistributeMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CrowdfundingPrice);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Commission);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Freight);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.GoldCoupon);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CouponValue);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SpendScoreMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.GoldPrice);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SpreadGold / 1000);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OwnerShare);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", GetOrderSelfTakeFlag(model.SelfTakeFlag));
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.SelfTakeAddress);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.EsAppName);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.FirstContent);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SecondContent);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.ThirdContent);
                        }
                        sbHtml.Append("</tr>");

                        j++;
                    }
                  
                    i++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单导出ExportExcelData出错。startTime：{0}，endTime：{1}，state：{2}，appid：{3}，orderids：{4}", startTime, endTime, state, appid, orderids), ex);
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("dingdan{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }


        /// <summary>
        /// 给阳光餐饮用的
        /// </summary>
        /// <returns></returns>
        public FileResult YgcyExportExcelData()
        {
            string startTime = Request["startTimeExcel"];
            string endTime = Request["endTimeExcel"];
            string state = Request["stateExcel"];
            string appid = Request["appidExcel"];
            string orderids = Request["orderIdsExcel"];
            OrderFieldFacade ordersetfacde = new OrderFieldFacade();
            var filed = ordersetfacde.GetOrderSet(Guid.Parse(appid));
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "ID(序列号)", "订单ID", "订单编号", "商品名称", "商品单价", "购买数量/件", "进货价", "结算价", "下单时间", "下单账号", "下单昵称", "付款时间", "付款方式", "实付款", "已退款", "收货人", "手机号", "收货地址", "邮编", "物流公司", "快递单号", "买家备注", "商家备注", "渠道佣金", "分销佣金", "含众筹佣金", "众销佣金", "含运费", "使用代金券金额", "使用优惠券金额", "积分抵现", "使用金币金额", "推广主佣金", "应用主佣金", "订单状态", "是否自提", "自提点", "订单来源" };
            if (filed != null)
            {
                if (!string.IsNullOrEmpty(filed.FirstField))
                {
                    lstTitle.Add(filed.FirstField);
                }
                if (!string.IsNullOrEmpty(filed.SecondField))
                {
                    lstTitle.Add(filed.SecondField);
                }
                if (!string.IsNullOrEmpty(filed.ThirdField))
                {
                    lstTitle.Add(filed.ThirdField);
                }
            }
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            try
            {
                ExportParamDTO param = new ExportParamDTO();
                param.StartDate = Convert.ToDateTime(startTime + " 00:00:00");
                param.EndDate = Convert.ToDateTime(endTime).AddDays(1);
                param.State = state;
                param.AppId = Guid.Parse(appid);
                param.orderIds = orderids;

                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                List<ExportResultDTO> result = orderBP.ExportResult(param);
                int i = 1, orderItemCount = 0, j = 0;
                foreach (ExportResultDTO model in result)
                {
                    orderItemCount = model.Products == null ? 0 : model.Products.Count;
                    j = 0;
                    foreach (ProductList product in model.Products)
                    {
                        if (model.State == 7 || model.StateAfterSales == 7)
                        {
                            model.ChannelShareMoney = 0;
                        }
                        sbHtml.Append("<tr>");
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, i);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.CommodityOrderId);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Code);
                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.CommodityOrderId);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Code);
                        }
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", product.ProductName);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.ProductPric);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.BuyNumber);
                        if (product.CostPrice == -1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                        }
                        else
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.CostPrice * product.BuyNumber);
                        }
                        if (product.ManufacturerClearingPrice == -1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                        }
                        else
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.ManufacturerClearingPrice * product.BuyNumber);
                        }
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OrdersTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Ucode);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Uname);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentType);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PracticalPayment);
                            if (model.State == 7 && model.RefundMoney + model.RefundScoreMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}（含{2}元积分）</td>", orderItemCount, model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, "");
                            }
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Payer);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Phone);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ShippingAddress);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Postcode);

                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ShipExpCo);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ExpOrderNo);

                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.remark);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SellersRemark);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.ChannelShareMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.DistributeMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.CrowdfundingPrice);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Commission);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Freight);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.GoldCoupon);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.CouponValue);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.SpendScoreMoney);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.GoldPrice);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.SpreadGold / 1000);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OwnerShare);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, GetOrderSelfTakeFlag(model.SelfTakeFlag));
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SelfTakeAddress);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.EsAppName);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.FirstContent);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SecondContent);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ThirdContent);

                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrdersTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Ucode);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Uname);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PaymentTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.PaymentType);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PracticalPayment);
                            if (model.State == 7 && model.RefundMoney >= 0 || model.State == 3 && model.StateAfterSales == 7 && model.RefundMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}（含{1}元积分）</td>", model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                            }
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Payer);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Phone);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ShippingAddress);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Postcode);

                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ShipExpCo);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ExpOrderNo);

                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.remark);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.SellersRemark);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.ChannelShareMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DistributeMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CrowdfundingPrice);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Commission);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Freight);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.GoldCoupon);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CouponValue);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SpendScoreMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.GoldPrice);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SpreadGold / 1000);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OwnerShare);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", GetOrderSelfTakeFlag(model.SelfTakeFlag));
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.SelfTakeAddress);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.EsAppName);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.FirstContent);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SecondContent);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.ThirdContent);
                        }
                        sbHtml.Append("</tr>");

                        j++;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单导出YgcyExportExcelData出错。startTime：{0}，endTime：{1}，state：{2}，appid：{3}，orderids：{4}", startTime, endTime, state, appid, orderids), ex);
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("dingdan{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        


        /// <summary>
        /// 导出订单(excel) 订单查询
        /// </summary>
        /// <returns></returns>
        public FileResult ExportEsExcelData()
        {
           
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "ID(序列号)", "订单编号", "厂商名称", "厂商类型", "商品名称", "商品单价", "购买数量/件", "下单时间", "付款时间", "付款方式", "实付款", "订单状态", "已退款", "收货人", "手机号", "收货地址", "含运费", "使用代金券金额", "使用优惠券金额", "回款总金额","易捷币抵现金额" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            try
            {
                ExportParamDTO param = new ExportParamDTO();
                string orderIs = SessionHelper.Get("orderids");
                List<Guid> orderlist = new List<Guid>();
                if (orderIs!=null)
                {
                    string[] arr = orderIs.Split(',').ToArray();
                    foreach (var item in arr)
                    {
                        orderlist.Add(Guid.Parse(item));
                    }
                }
                param._orderIds = orderlist;
                CommodityOrderFacade orderBP = new CommodityOrderFacade();
                List<ExportResultDTO> result = orderBP.ExportResult(param);
                int i = 1, orderItemCount = 0, j = 0;
                foreach (ExportResultDTO model in result)
                {
                    orderItemCount = model.Products == null ? 0 : model.Products.Count;
                    j = 0;
                    foreach (ProductList product in model.Products)
                    {
                        if (model.State == 7 || model.StateAfterSales == 7)
                        {
                            model.ChannelShareMoney = 0;
                        }
                        sbHtml.Append("<tr>");
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, i);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Code);
                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Code);
                        }
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.AppName);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.AppType);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", product.ProductName);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.ProductPric);
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", product.BuyNumber);
                        if (orderItemCount > 1 && j == 0)
                        {
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OrdersTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentTime);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PaymentType);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PracticalPayment);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            if (model.State == 7 && model.RefundMoney + model.RefundScoreMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}（含{2}元积分）</td>", orderItemCount, model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, "");
                            }
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Payer);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Phone);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.ShippingAddress);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Freight);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.GoldCoupon);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.CouponValue);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.JczfAmonut);
                            sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.SpendYJBMoney);

                        }
                        else if (orderItemCount == 1)
                        {
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrdersTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PaymentTime);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.PaymentType);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PracticalPayment);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", GetOrderStateDescription(model.State, model.SelfTakeFlag, model.StateAfterSales));
                            if (model.State == 7 && model.RefundMoney >= 0 || model.State == 3 && model.StateAfterSales == 7 && model.RefundMoney >= 0)
                            {
                                if (model.RefundScoreMoney > 0)
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}（含{1}元积分）</td>", model.RefundMoney.Value + model.RefundScoreMoney, model.RefundScoreMoney);
                                }
                                else
                                {
                                    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RefundMoney.Value);
                                }
                            }
                            else
                            {
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                            }
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Payer);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Phone);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.ShippingAddress);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Freight);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.GoldCoupon);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CouponValue);
                           // sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SpendYJBMoney);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.JczfAmonut);
                           
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.SpendYJBMoney);

                        }
                        sbHtml.Append("</tr>");

                        j++;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单导出ExportEsExcelData出错：{0}",ex.Message),ex);
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("dingdan{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }



        /// <summary>
        /// 导出订单(excel) 订单查询
        /// </summary>
        /// <returns></returns>
        public FileResult ExportEsExcelData1()
        {
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "订单编号", "商家类型", "商品名称", "订单金额", "下单时间", "实付款" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            try
            {
                ExportParamDTO param = new ExportParamDTO();
                string orderIs = SessionHelper.Get("orderids");
                List<Guid> orderlist = new List<Guid>();
                if (orderIs != null)
                {
                    string[] arr = orderIs.Split(',').ToArray();
                    orderlist.AddRange(arr.Select(item => Guid.Parse(item)));
                }
                param._orderIds = orderlist;
                CommodityOrderFacade orderBp = new CommodityOrderFacade();
                List<ExportResultDTO> result = orderBp.ExportResult1(param);
                foreach (ExportResultDTO model in result)
                {
                    var orderItemCount = model.Products == null ? 0 : model.Products.Count;
                    var j = 0;
                    if (model.Products != null)
                        foreach (ProductList product in model.Products)
                        {
                            sbHtml.Append("<tr>");
                            if (orderItemCount > 1 && j == 0)
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{1}</td>", orderItemCount, model.Code);
                            }
                            else if (orderItemCount == 1)
                            {
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.Code);
                            }
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", model.AppType);
                            sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;vnd.ms-excel.numberformat:@'>{0}</td>", product.ProductName);
                            if (orderItemCount > 1 && j == 0)
                            {
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.Price);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.OrdersTime);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, model.PracticalPayment);
                                sbHtml.AppendFormat("<td rowspan='{0}' style='font-size: 12px;height:20px;'>{1}</td>", orderItemCount, "");
                            }
                            else if (orderItemCount == 1)
                            {
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Price);
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrdersTime);
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PracticalPayment);
                                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", "");
                            }
                            sbHtml.Append("</tr>");

                            j++;
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("订单导出ExportEsExcelData1出错：{0}", ex.Message));
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("dingdan{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 查看退款申请
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrderRefund(FormCollection collection)
        {
            CommodityOrderFacade comf = new CommodityOrderFacade();
            Guid id = new Guid(collection["CommodityOrderId"]);
            SubmitOrderRefundDTO sord = new SubmitOrderRefundDTO();
            Guid orderItemId = new Guid();
            if (collection["OrderItemId"] != null)
            {
                orderItemId = new Guid(collection["OrderItemId"]);
            }
            if (orderItemId != Guid.Empty)
            {
                sord = comf.GetOrderItemRefund(id, orderItemId);
            }
            else
            {
                sord = comf.GetOrderRefund(id);
                sord.IsJdEclpOrder = new IBP.Facade.JdEclpOrderFacade().ISEclpOrder(id);
                if (sord.IsJdEclpOrder)
                    sord.RejectFreightMoney = new ISV.Facade.CommodityFacade().CalRefundFreight(id, orderItemId).Freight;
            }
            return Json(sord);
        }

        public ActionResult ShipUpdataOrder(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.ShipUpdataOrder(commodityOrderId, shipExpCo, expOrderNo);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "发货成功" });
            }
            return Json(new { Result = false, Messages = "发货失败" });
        }


        public ActionResult AlipayZhiTui(Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.AlipayZhiTui(commodityorderId, ReceiverAccount, Receiver, RefundMoney);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "退款成功" });
            }
            return Json(new { Result = false, Messages = "退款失败" });
        }

        public ActionResult DelOrder(Guid commodityorderId, int IsDel)
        {
            BTP.ISV.Facade.CommodityOrderFacade cf = new ISV.Facade.CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.DelOrder(commodityorderId, IsDel);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "删除成功" });
            }
            return Json(new { Result = false, Messages = "删除失败" });
        }

        /// <summary>
        /// 获取联系客户相关的信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetContactCustomerData(Guid orderId, Guid appId, Guid userId, Guid sceneId)
        {
            bool result = true;
            string messages = "";
            try
            {
                BTP.ISV.Facade.BTPUserFacade btpUserInfo = new ISV.Facade.BTPUserFacade();
                UserSDTO userInfo = btpUserInfo.GetUser(userId, appId);

                if (userInfo != null && sceneId != Guid.Empty)
                {
                    return Json(new { Result = result, Messages = messages, Data = new { fromUserId = userInfo.UserId, fromUserName = userInfo.UserName, appId = appId, sceneId = sceneId, iconPath = userInfo.PicUrl } });
                }
                else
                {
                    result = false;
                    messages = "未获取到数据";
                }

            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error(string.Format("获取联系客户相关的信息Exception。orderId：{0}，appId：{1}，userId：{2}，sceneId：{3}", orderId, appId, userId, sceneId), ex);
                result = false;
                messages = "加载数据异常";
            }

            return Json(new { Result = result, Messages = messages });
        }


        /// <summary>
        /// 修改卖家备注
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <param name="SellersRemark"></param>
        /// <returns></returns>
        public ActionResult UpdateSellersRemark(Guid commodityOrderId, string SellersRemark, string state)
        {

            if (state == "delete")
            {
                SellersRemark = null;
            }
            BTP.IBP.Facade.CommodityOrderFacade cf = new IBP.Facade.CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Result = cf.UpdateSellersRemark(commodityOrderId, SellersRemark);
            if (Result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        /// <summary>
        /// 修改物流信息（只修改信息，不修改订单状态）
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        public ActionResult ChgOrderShip(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            ResultShipDTO result = cf.ChgOrderShip(commodityOrderId, shipExpCo, expOrderNo);
            return Json(result);
        }

        /// <summary>
        /// 订单状态描述
        /// </summary>
        /// <param name="state">订单状态编码</param>
        /// <param name="selfTakeFlag">自提标识</param>
        /// <param name="stataS">售后订单状态编码</param>
        /// <returns>订单状态描述</returns>
        private string GetOrderStateDescription(int state, int selfTakeFlag, int stataS)
        {
            //待自提
            if ((state == 1 || state == 11) && selfTakeFlag == 1)
            {
                state = 99;
            }
            string result = string.Empty;
            switch (state)
            {
                case 0:
                    result = "待付款";
                    break;
                case 1:
                    result = "待发货";
                    break;
                case 2:
                    result = "已发货";
                    break;
                case 3:
                    if (stataS == 7)
                    {
                        result = "已退款";
                    }
                    else if (stataS == 5 || stataS == 10 || stataS == 12)
                    {
                        result = "退款中";
                    }
                    else if (stataS == 15)
                    {
                        result = "售后完毕";
                    }
                    else if (stataS == 3)
                    {
                        result = "确认收货";
                    }
                    else
                    {
                        result = "确认收货";
                    }
                    break;
                case 4:
                    result = "商家取消订单";
                    break;
                case 5:
                    result = "客户取消订单";
                    break;
                case 6:
                    result = "超时交易关闭";
                    break;
                case 7:
                    result = "已退款";
                    break;
                case 8:
                    result = "退款中";
                    break;
                case 9:
                    result = "退款中";
                    break;
                case 10:
                    result = "退款中";
                    break;
                case 11:
                    result = "待发货";
                    break;
                case 12:
                    result = "退款中";
                    break;
                case 13:
                    result = "出库中";
                    break;
                case 14:
                    result = "退款中";
                    break;
                case 15:
                    result = "售后完毕";
                    break;
                case 99:
                    result = "待自提";
                    break;
                default:
                    result = "未知";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 订单自提描述
        /// </summary>
        /// <param name="selfTakeFlag">订单自提标志</param>
        /// <returns>订单自提描述</returns>
        private string GetOrderSelfTakeFlag(int selfTakeFlag)
        {
            string result = string.Empty;
            switch (selfTakeFlag)
            {
                case 0:
                    result = "非自提";
                    break;
                case 1:
                    result = "自提";
                    break;
                default:
                    result = "未知";
                    break;

            }
            return result;
        }
        /// <summary>
        /// 订单查询（根据订单id列表取订单数据）
        /// </summary>
        /// <returns></returns>
        public ActionResult SeachOrdersPay()
        {
            return View();
        }
        /// <summary>
        /// 订单查询（根据订单id列表取订单数据）
        /// </summary>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetMainOrdersPay(string mainOrderIds)
        {
            BTP.IBP.Facade.CommodityOrderFacade cf = new IBP.Facade.CommodityOrderFacade();
            List<CommodityOrderCheckAccount> result = cf.GetMainOrdersPay(mainOrderIds);

            int PageNo = Request["page"] == null ? 0 : int.Parse(Request["page"]);
            int PageSize = Request["rows"] == null ? 0 : int.Parse(Request["rows"]);
            IList<string> show = new List<string>();
            show.Add("AccountId");
            show.Add("AccountIdString");
            show.Add("O2OId");
            show.Add("AppName");
            show.Add("PaymentTime");
            show.Add("RealPrice");
            show.Add("RefundPrice");
            return View(new GridModel<CommodityOrderCheckAccount>(show, result, result.Count, PageNo, PageSize, string.Empty));
        }

        #endregion

        #region 售后

        /// <summary>
        /// 同意退款/退款申请
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult UpdateCommodityOrderAfterSales(Guid commodityOrderId, int state, string message, Guid userId, Guid orderItemId)
        {
            decimal pickUpFreightMoney;
            decimal sendBackFreightMoney;
            decimal.TryParse(Request["pickUpFreightMoney"], out pickUpFreightMoney);
            decimal.TryParse(Request["sendBackFreightMoney"], out sendBackFreightMoney);

            CommodityOrderAfterSalesFacade cf = new CommodityOrderAfterSalesFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.OrderItemId = orderItemId;
            model.PickUpFreightMoney = pickUpFreightMoney;
            model.SendBackFreightMoney = sendBackFreightMoney;

            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.CancelTheOrderAfterSales(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "售后订单修改成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        /// <summary>
        /// 拒绝退款/退货申请
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        ///  <param name="refuseReason"></param>
        /// <returns></returns>
        public ActionResult RefuseRefundOrderAfterSales(Guid commodityOrderId, Guid orderItemId, int state, string message, Guid userId, string refuseReason = "")
        {
            CommodityOrderAfterSalesFacade cf = new CommodityOrderAfterSalesFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.RefuseReason = refuseReason;
            model.OrderItemId = orderItemId;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.RefuseRefundOrderAfterSales(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "拒绝申请成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        ///// <summary>
        ///// 查看售后退款申请
        ///// </summary>
        ///// <param name="collection"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult GetOrderRefundAfterSales(Guid CommodityOrderId, Guid OrderItemId)
        //{
        //    SubmitOrderRefundDTO sord = new SubmitOrderRefundDTO();
        //    try
        //    {
        //        CommodityOrderAfterSalesFacade comf = new CommodityOrderAfterSalesFacade();
        //        JdEclpOrderFacade  JdEclpOrder=  new IBP.Facade.JdEclpOrderFacade();
        //        Guid id = CommodityOrderId;
        //        Guid orderItemId = OrderItemId;
        //        if (orderItemId != Guid.Empty)
        //        {
        //            LogHelper.Debug(string.Format("查看售后退款申请异常信息1:{0}", orderItemId));
        //            sord = comf.GetOrderItemRefundAfterSales(id, orderItemId);
        //            LogHelper.Debug(string.Format("查看售后退款申请异常信息2"));
        //            var commodityOrder = CommodityOrder.ObjectSet().Where(o => o.Id == id && (o.AppType == 2 || o.AppType == 3)).AsQueryable();
        //            var jdEclpOrder = JDEclpOrder.ObjectSet().Where(o => o.OrderId == id).AsQueryable();
        //            if (commodityOrder != null && jdEclpOrder!=null)
        //            {
        //                LogHelper.Debug(string.Format("查看售后退款申请异常信息3:{0},{1}", commodityOrder.Count(), jdEclpOrder.Count()));
        //                if (commodityOrder.Count()>0&&jdEclpOrder.Count()>0)
        //                {
        //                    sord.PickUpFreightMoney = new ISV.Facade.CommodityFacade().CalRefundFreight(id, orderItemId).Freight;
        //                }
                        
        //            }
                       
        //        }
        //        else
        //        {
        //            sord = comf.GetOrderRefundAfterSales(id);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        LogHelper.Error(string.Format("查看售后退款申请异常信息:{0}",ex.Message),ex);
        //    }
          
        //    return Json(sord);
        //}


        /// <summary>
        /// 查看售后退款申请
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrderRefundAfterSales(FormCollection collection)
        {
            CommodityOrderAfterSalesFacade comf = new CommodityOrderAfterSalesFacade();
            SubmitOrderRefundDTO sord = new SubmitOrderRefundDTO();
            Guid id = new Guid(collection["CommodityOrderId"]);
            Guid orderItemId =new Guid();
            if (collection["OrderItemId"] != null)
            {
                orderItemId = new Guid(collection["OrderItemId"]);
            }
             
            if (orderItemId != Guid.Empty)
            {
                sord = comf.GetOrderItemRefundAfterSales(id, orderItemId);
                sord.IsJdEclpOrder = new IBP.Facade.JdEclpOrderFacade().ISEclpOrder(id);
                if (sord.IsJdEclpOrder)
                    sord.PickUpFreightMoney = new ISV.Facade.CommodityFacade().CalRefundFreight(id, orderItemId).Freight;
            }
            else
            {
                sord = comf.GetOrderRefundAfterSales(id);
            }
            return Json(sord);
        }

        /// <summary>
        /// 查看售后退款历史申请记录
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRefundInfoAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfo)
        {
            CommodityOrderAfterSalesFacade comf = new CommodityOrderAfterSalesFacade();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> sord = comf.GetRefundInfoAfterSales(refundInfo);
            return Json(sord);
        }

        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityOrderId">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelayConfirmTimeAfterSales(Guid commodityOrderId)
        {
            CommodityOrderAfterSalesFacade comf = new CommodityOrderAfterSalesFacade();

            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = comf.DelayConfirmTimeAfterSales(commodityOrderId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "售后延长收货时间成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ActionResult RefuseRefundOrderSellerAfterSales(Guid commodityOrderId, Guid orderItemId, int state, string message, Guid userId, string refuseReason = "")
        {
            CommodityOrderAfterSalesFacade cf = new CommodityOrderAfterSalesFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.RefuseReason = refuseReason;
            model.OrderItemId = orderItemId;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.RefuseRefundOrderSellerAfterSales(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "拒绝收货成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        #endregion


        #region 售中

        /// <summary>
        /// 拒绝退款/退货申请
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ActionResult RefuseRefundOrder(Guid commodityOrderId, Guid orderItemId, int state, string message, Guid userId, string refuseReason = "")
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.RefuseReason = refuseReason;
            model.OrderItemId = orderItemId;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.RefuseRefundOrder(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "拒绝申请成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ActionResult RefuseRefundOrderSeller(Guid commodityOrderId, Guid orderItemId, int state, string message, Guid userId, string refuseReason = "")
        {
            CommodityOrderFacade cf = new CommodityOrderFacade();
            CancelTheOrderDTO model = new CancelTheOrderDTO();
            model.OrderId = commodityOrderId;
            model.State = state;
            model.Message = message;
            model.UserId = userId;
            model.RefuseReason = refuseReason;
            model.OrderItemId = orderItemId;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = cf.RefuseRefundOrderSeller(model);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "拒绝收货成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }

        /// <summary>
        /// 售中延长收货时间
        /// </summary>
        /// <param name="commodityOrderId">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelayConfirmTime(Guid commodityOrderId)
        {
            CommodityOrderFacade comf = new CommodityOrderFacade();

            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = comf.DelayConfirmTime(commodityOrderId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "售中延长收货时间成功" });
            }
            return Json(new { Result = false, Messages = result.Message });
        }


        #endregion


        /// <summary>
        /// 查看退款历史申请记录（含售中与售后的）
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRefundInfo(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfo)
        {
            CommodityOrderFacade comf = new CommodityOrderFacade();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> sord = comf.GetRefundInfo(refundInfo);
            return Json(sord);
        }

        /// <summary>
        /// 分销商订单
        /// </summary>
        /// <returns></returns>
        public ActionResult DistributeOrders()
        {
            //TODO 三级分销
            throw new NotImplementedException();
        }

        /// <summary>
        /// 订单相关所有信息。
        /// </summary>
        /// <param name="orderId">商品订单ID或订单编号</param>
        /// <returns></returns>
        public ActionResult OrderFullInfo()
        {
            return View();
        }

        /// <summary>
        /// 订单相关所有信息。
        /// </summary>
        /// <param name="orderId">商品订单ID或订单编号</param>
        /// <returns></returns>
        public ActionResult GetOrderFullInfo(string orderId)
        {
            CommodityOrderFacade coFacade = new CommodityOrderFacade();
            OrderFullInfo ofi = coFacade.GetFullOrderInfoById(orderId);
            return Json(ofi);
        }

        /// <summary>
        /// 售中标记为已退款
        /// </summary>
        /// <param name="orderRefundDto"></param>
        /// <returns></returns>
        public ActionResult DirectPayRefund(OrderRefundDTO orderRefundDto)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade facade = new CommodityOrderFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = facade.DirectPayRefund(orderRefundDto);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "退款成功" });
            }
            return Json(new { Result = false, Messages = "退款失败" });

        }
        /// <summary>
        /// 售后标记为已退款
        /// </summary>
        /// <param name="orderRefundDto"></param>
        /// <returns></returns>
        public ActionResult DirectPayRefundAfterSales(OrderRefundDTO orderRefundDto)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade facade = new CommodityOrderAfterSalesFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = facade.DirectPayRefundAfterSales(orderRefundDto);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "退款成功" });
            }
            return Json(new { Result = false, Messages = "退款失败" });
        }

        public ActionResult DownLoadFiles(string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            string fileExtention = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();
            string contentType = string.Empty;
            switch (fileExtention)
            {
                case "jpg":
                case "jpeg":
                    contentType = "image/jpeg jpeg jpg jpe";
                    break;
                case "gif":
                    contentType = "image/gif gif";
                    break;
                case "png":
                    contentType = "image/png png";
                    break;
                default:
                    contentType = "image/jpeg jpeg jpg jpe";
                    break;
            }

            WebRequest webRequest = WebRequest.Create(filePath);
            WebResponse webResponse = webRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            MemoryStream mem = new MemoryStream();
            BufferedStream bfs = new BufferedStream(stream);
            int len = 0;
            byte[] buffer = new byte[4096];
            do
            {
                len = bfs.Read(buffer, 0, buffer.Length);
                if (len > 0)
                    mem.Write(buffer, 0, len);
            }
            while (len > 0);
            bfs.Close();
            byte[] picbytes = mem.ToArray();
            mem.Close();

            return File(picbytes, contentType, Url.Encode(fileName));
        }

        /// <summary>
        ///  进销存系统对接临时方案-按京东eclp系统标准导出订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportOrderForJD()
        {
            try
            {
                Guid appId;
                Guid.TryParse(Request["AppId"], out appId);
                if (appId == Guid.Empty) return Content(string.Empty);
                var startTimeStr = RedisHelper.GetHashValue<string>(RedisKeyConst.ExportOrderForJDLastTime, appId.ToString().ToLower());
                if (string.IsNullOrEmpty(startTimeStr)) startTimeStr = "20180313000000";
                var startTime = DateTime.ParseExact(startTimeStr, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
                var endTime = DateTime.Now;
                var orderList = new CommodityOrderFacade().ExportOrderForJD(appId, startTime, endTime);
                var dt = new System.Data.DataTable();
                dt.Columns.Add("isvUUID", typeof(string));
                dt.Columns.Add("isvSource", typeof(string));
                dt.Columns.Add("shopNo", typeof(string));
                dt.Columns.Add("bdOwnerNo", typeof(string));
                dt.Columns.Add("departmentNo", typeof(string));
                dt.Columns.Add("warehouseNo", typeof(string));
                dt.Columns.Add("shipperNo", typeof(string));
                dt.Columns.Add("salesPlatformOrderNo", typeof(string));
                dt.Columns.Add("salePlatformSource", typeof(string));
                dt.Columns.Add("salesPlatformCreateTime", typeof(string));
                dt.Columns.Add("consigneeName", typeof(string));
                dt.Columns.Add("consigneeMobile", typeof(string));
                dt.Columns.Add("consigneePhone", typeof(string));
                dt.Columns.Add("consigneeEmail", typeof(string));
                dt.Columns.Add("expectDate", typeof(string));
                dt.Columns.Add("addressProvince", typeof(string));
                dt.Columns.Add("addressCity", typeof(string));
                dt.Columns.Add("addressCounty", typeof(string));
                dt.Columns.Add("addressTown", typeof(string));
                dt.Columns.Add("consigneeAddress", typeof(string));
                dt.Columns.Add("consigneePostcode", typeof(string));
                dt.Columns.Add("receivable", typeof(string));
                dt.Columns.Add("consigneeRemark", typeof(string));
                dt.Columns.Add("orderMark", typeof(string));
                dt.Columns.Add("thirdWayBill", typeof(string));
                dt.Columns.Add("packageMark", typeof(string));
                dt.Columns.Add("businessType", typeof(string));
                dt.Columns.Add("destinationCode", typeof(string));
                dt.Columns.Add("destinationName", typeof(string));
                dt.Columns.Add("sendWebsiteCode", typeof(string));
                dt.Columns.Add("sendWebsiteName", typeof(string));
                dt.Columns.Add("sendMode", typeof(string));
                dt.Columns.Add("receiveMode", typeof(string));
                dt.Columns.Add("appointDeliveryTime", typeof(string));
                dt.Columns.Add("insuredPriceFlag", typeof(string));
                dt.Columns.Add("insuredValue", typeof(string));
                dt.Columns.Add("insuredFee", typeof(string));
                dt.Columns.Add("thirdPayment", typeof(string));
                dt.Columns.Add("monthlyAccount", typeof(string));
                dt.Columns.Add("shipment", typeof(string));
                dt.Columns.Add("sellerRemark", typeof(string));
                dt.Columns.Add("thirdSite", typeof(string));
                dt.Columns.Add("goodsNos", typeof(string));
                dt.Columns.Add("goodsNosOther", typeof(string));
                dt.Columns.Add("prices", typeof(string));
                dt.Columns.Add("quantities", typeof(string));
                dt.Columns.Add("pinAccount", typeof(string));
                dt.Columns.Add("customerNo", typeof(string));
                dt.Columns.Add("soType", typeof(string));
                dt.Columns.Add("packagingDetails", typeof(string));
                foreach (var model in orderList)
                {
                    dt.Rows.Add(model.OrderCode, "ISV0020000000145", "ESP0020000026240", "010K164734", "EBU4418046541248", "110008714", "CYS0000010"
                        , string.Empty, "6", string.Empty, model.ReceiptUserName, model.ReceiptPhone, string.Empty, string.Empty, string.Empty, string.Empty
                        , string.Empty, string.Empty, string.Empty, model.ReceiptAddress, string.Empty, string.Empty, string.Empty
                        , "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                        , string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty
                        , string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty
                        , model.JdCodes, model.ErQiCodes, model.Prices, model.Numbers, "中国石化易捷北京", string.Empty, 1, string.Empty);
                }
                if (orderList.Count > 0) RedisHelper.AddHash(RedisKeyConst.ExportOrderForJDLastTime, appId.ToString().ToLower(), endTime.ToString("yyyyMMddHHmmss"));
                var fileName = string.Format("jdeclp_{0}_{1}.xls", startTime.ToString("yyyyMMddHHmmss"), endTime.ToString("yyyyMMddHHmmss"));
                LogHelper.Debug("CommodityOrderController.ExportOrderForJD,fileName=" + fileName);
                return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityOrderController.ExportOrderForJD异常", ex);
            }
            return Content(string.Empty);
        }

        [HttpPost]
        /// <summary>
        /// 保存回款项
        /// </summary>
        /// <param name="CommodityOrderId">商品订单id</param> 
        /// <param name="OrderItemId">订单id</param> 
        /// <param name="Type">回款方式</param>
        /// <param name="RefundTime">回款时间</param>
        /// <param name="Refund">回款金额</param>
        /// <param name="RefundRemark">备注</param>
        /// <param name="OrderAmount">订单总金额</param>
        /// <returns></returns>
        public JsonResult SaveRefund(string CommodityOrderId, string OrderItemId, int Type, string RefundTime, string Refund, string RefundRemark, decimal OrderAmount = 0)
        {
            Guid userId;
            Guid.TryParse(Request["userId"], out userId);
            if (CommodityOrderId == null && CommodityOrderId == "")
            {
                return Json(new { status = 1002 });
            }
            DateTime time;
            if (!DateTime.TryParse(RefundTime, out time))
            {
                return Json(new { status = 1003 });
            }
            decimal amount;
            if (!decimal.TryParse(Refund, out amount))
            {
                return Json(new { status = 1004 });
            }
            Guid guidCommodityOrderId;
            Guid.TryParse(CommodityOrderId, out guidCommodityOrderId);

            CommodityOrderRefundFacade fac = new CommodityOrderRefundFacade();
            var RefundAmount = fac.GetListByCommodityOrderId(guidCommodityOrderId).Sum(o => o.RefundAmount) + amount;

            if (RefundAmount > OrderAmount)//判断订单总金额不小于回款总金额
            {
                return Json(new { status = 1006 });
            }

            try
            {
                Guid guidOrderItemId;
                Guid.TryParse(OrderItemId, out guidOrderItemId);
                Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO model = new Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO();
                model.Id = Guid.NewGuid();
                model.SubTime = DateTime.Now;
                model.SubId = userId;
                model.CommodityOrderId = guidCommodityOrderId;
                model.OrderItemId = guidOrderItemId;
                model.RefundType = Type;
                model.RefundDate = time;
                model.RefundAmount = amount;
                model.Remark = RefundRemark;
                if (!fac.Insert(model))
                {
                    return Json(new { status = 0 });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("保存回款项异常：" + ex);
                return Json(new { status = 1005 });
            }
            return Json(new { status = 1 });
        }


        /// <summary>
        /// 获得回款明细
        /// </summary>
        /// <param name="CommodityOrderId">商品订单id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRefundDetail(string CommodityOrderId)
        {
            if (CommodityOrderId == null && CommodityOrderId == "")
            {
                return Json(new { status = 0 });
            }
            Guid guidCommodityOrderId;
            Guid.TryParse(CommodityOrderId, out guidCommodityOrderId);
            CommodityOrderRefundFacade fac = new CommodityOrderRefundFacade();
            var list = fac.GetListByCommodityOrderId(guidCommodityOrderId);
            string resultjson = JsonHelper.JsonSerializer(list);
            return Json(new { status = 1, list = resultjson });
        }


        #region 金彩支付订单查询

        /// <summary>
        /// 金彩支付订单查询
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult EsAppJczfIndex()
        {
            Guid appId;

            if (System.Web.HttpContext.Current.Session["APPID"] == null)
            {
                Guid.TryParse(Request["appId"], out appId);
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }
            else
            {
                //appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
                appId = Guid.Parse(System.Web.HttpContext.Current.Session["APPID"].ToString());
            }

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
                Payment = "2001",
                Isjczf = true
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.EsOrderIds = searchResult.Message;
            SessionHelper.Add("orderids", searchResult.Message);
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
                Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<List<Jinher.AMP.SNS.Deploy.CustomDTO.AppSceneUserApiDTO>> sceneUserList = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetSceneUserInfo(appId, this.ContextDTO.LoginUserID);
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
                JAP.Common.Loging.LogHelper.Error(string.Format("Exception。appId：{0}，userId：{1}", appId, this.ContextDTO.LoginUserID), ex);
                ViewBag.SceneId = Guid.Empty;
            }

            return View();
        }

        #endregion

        #region 金彩支付订单查询

        /// <summary>
        /// 
        /// <para> </para>
        /// </summary>
        /// <param name="priceLow"></param>
        /// <param name="priceHight"></param>
        /// <param name="seacrhContent"></param>
        /// <param name="dayCount"></param>
        /// <param name="state"></param>
        /// <param name="payment"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="esAppId"></param>
        /// <param name="orderSourceId"></param>
        /// <param name="marketing">优惠活动筛选项</param>
        /// <returns></returns>
        [HttpPost]
        [CheckAppId]
        public PartialViewResult PartialJczfEsAppIndex(string priceLow, string priceHight, string seacrhContent, string dayCount, string state, string payment, string startTime, string endTime, Guid? esAppId, Guid? orderSourceId, int marketing)
        {
            payment = "2001";//金彩支付类型数据
            ViewBag.OrderState = state;

            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
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
            CommodityOrderSearchDTO search = new CommodityOrderSearchDTO()
            {
                AppId = appId,
                PageSize = pageSize,
                PageIndex = pageIndex,
                PriceLow = priceLow,
                PriceHight = priceHight,
                SeacrhContent = seacrhContent,
                DayCount = dayCount,
                State = state,
                Payment = payment,
                StartOrderTime = startOrderTime,
                EndOrderTime = endOrderTime,
                EsAppId = esAppId,
                OrderSourceId = orderSourceId,
                Marketing = marketing,
                Isjczf = true
            };
            var searchResult = cf.GetAllCommodityOrderByEsAppId(search);
            List<CommodityOrderVM> commodityOrderList = searchResult.Data.Data;
            ViewBag.commodityOrderList = commodityOrderList;
            ViewBag.Count = searchResult.Data.Count;
            ViewBag.EsOrderIds = searchResult.Message;
            SessionHelper.Add("orderids", searchResult.Message);
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

        #endregion
    }
}
