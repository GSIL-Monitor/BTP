using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.App.ISV.Facade;


namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityController : BaseController
    {
        #region 已上架商品列表
        [CheckAppId]
        public ActionResult Index()
        {
            Guid UserId = this.ContextDTO.LoginUserID;
            Guid SessionId = new Guid(this.ContextDTO.SessionID);
            Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;


            Guid appId = WebUtil.AppId;
            CategoryFacade catefa = new CategoryFacade();
            var catelist = catefa.GetCategories(appId);
            ViewBag.CategoryList = catelist;
            ViewBag.AppId = appId.ToString().ToUpper();

            bool BTPOffShelfCom = false;
            bool BTPDelSaleCom = false;
            bool BTPDistributeCom = false;
            bool BTPDistributeDivi = false;

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {

                //出售商品下架
                BTPOffShelfCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOffShelfCom);
                //出售商品删除
                BTPDelSaleCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDelSaleCom);
                if (Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
                {
                    //三级分销分销商品设置
                    BTPDistributeCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDistributeCom);
                    //三级分销佣金设置
                    BTPDistributeDivi = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDistributeDivi);
                }
                ViewBag.IsOrg = true;
            }

            ViewBag.BTPOffShelfCom = BTPOffShelfCom;
            ViewBag.BTPDelSaleCom = BTPDelSaleCom;
            ViewBag.BTPDistributeCom = BTPDistributeCom;
            ViewBag.BTPDistributeDivi = BTPDistributeDivi;

            ViewBag.ProvinceCityUrl = CustomConfig.ProvinceCityUrl + "?userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg;
            ViewBag.VideoHost = CustomConfig.VideoHost + "?appId=" + appId + "&userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg + "&MediaType =1";
            AppExtensionFacade appExtFacade = new AppExtensionFacade();
            var appExtResult = appExtFacade.GetAppExtensionByAppId(appId);
            if (appExtResult.ResultCode == 0 && appExtResult.Data != null)
            {
                ViewBag.IsShowAddCart = appExtResult.Data.IsShowAddCart;
                ViewBag.IsCashForScore = appExtResult.Data.IsCashForScore;
            }
            else
            {
                ViewBag.IsShowAddCart = false;
                ViewBag.IsCashForScore = false;
            }
            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);
            ViewBag.isCustomES = isCustomES;
            return View();
        }

        [CheckAppId]
        public PartialViewResult PartialIndex(string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice)
        {
            Guid appId = WebUtil.AppId;
            ViewBag.appId = appId;
            ViewBag.userId = this.ContextDTO.LoginUserID;
            if (string.IsNullOrWhiteSpace(commodityName)
                && string.IsNullOrWhiteSpace(commodityCategory)
                && string.IsNullOrWhiteSpace(sSalesvolume)
                && string.IsNullOrWhiteSpace(eSalesvolume)
                && string.IsNullOrWhiteSpace(sPrice)
                && string.IsNullOrWhiteSpace(ePrice))
            {
                //从cookie中将保存的查询条件读取出来。
                HttpCookie hc = Request.Cookies["Commodity.SearchEntityJson." + appId];
                if (hc != null && (!string.IsNullOrWhiteSpace(hc.Value))
                    && (hc.Value != ""))
                {
                    string str = HttpUtility.UrlDecode(hc.Value);
                    JObject sn = JObject.Parse(str);

                    commodityName = sn["commodityName"].ToString();
                    commodityCategory = sn["commodityCategory"].ToString();
                    sSalesvolume = sn["sSalesvolume"].ToString();
                    eSalesvolume = sn["eSalesvolume"].ToString();
                    sPrice = sn["sPrice"].ToString();
                    ePrice = sn["ePrice"].ToString();
                }

            }

            int rowCount = 0;
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            #region 商品列表
            CommodityFacade comfa = new CommodityFacade();

            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageIndex = pageIndex;
            search.pageSize = pageSize;
            search.commodityName = commodityName;
            search.commodityCategory = commodityCategory;
            search.sSalesvolume = sSalesvolume;
            search.eSalesvolume = eSalesvolume;
            search.sPrice = sPrice;
            search.ePrice = ePrice;

            List<CommodityVM> comcalist = comfa.GetAllCommodityBySellerIDBySalesvolume(search, out rowCount).ToList();

            #endregion
            //var catelist = catefa.GetCategories(appId);
            ViewBag.CommodityList = comcalist;
            ViewBag.Count = rowCount;
            //ViewBag.CategoryList = catelist;

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPUpdSaleCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPUpdSaleCom);

                ViewBag.BTPUpdSaleCom = BTPUpdSaleCom;
                ViewBag.IsOrg = true;
            }

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            bool BTPDistributeCom = false;
            bool BTPDistributeDivi = false;

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                if (Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
                {
                    //三级分销分销商品设置
                    BTPDistributeCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDistributeCom);
                    //三级分销佣金设置
                    BTPDistributeDivi = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDistributeDivi);
                }
                ViewBag.IsOrg = true;
            }
            ViewBag.BTPDistributeCom = BTPDistributeCom;
            ViewBag.BTPDistributeDivi = BTPDistributeDivi;
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);
            ViewBag.isCustomES = isCustomES;

            return PartialView();
        }


        /// <summary>
        /// 销量管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SalesManage()
        {

            //Guid appId = WebUtil.AppId;
            APPManageFacade appManageFacade = new APPManageFacade();
            var list = appManageFacade.GetAPPManageList();
            ViewBag.appManageList = list;
            string selectappId = Request["appIds"];
            Guid appId = new Guid();

            if (selectappId == null)
            {
                if (list.Count > 0)
                {
                    appId = list.FirstOrDefault().AppId;
                }
            }
            else
            {
                appId = Guid.Parse(selectappId);
            }

            if (appId != null)
            {

                if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
                {
                    bool BTPOffShelfCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPOffShelfCom);
                    bool BTPDelSaleCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDelSaleCom);
                    ViewBag.BTPOffShelfCom = BTPOffShelfCom;
                    ViewBag.BTPDelSaleCom = BTPDelSaleCom;
                    ViewBag.IsOrg = true;
                }
            }


            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            return View();
        }

        public PartialViewResult showGetCategories(string selectappId)
        {

            Guid appId = new Guid();
            APPManageFacade appManageFacade = new APPManageFacade();

            var list = appManageFacade.GetAPPManageList();
            if (selectappId == null)
            {
                if (list.Count > 0)
                {
                    appId = list.FirstOrDefault().AppId;
                }

            }
            else
            {
                appId = Guid.Parse(selectappId);
            }

            if (appId != null)
            {
                CategoryFacade catefa = new CategoryFacade();
                var catelist = catefa.GetCategories(appId);
                ViewBag.CategoryList = catelist;
            }


            return PartialView();
        }

        public PartialViewResult SalesManagePartialIndex(string commodityName, string commodityCategory, string selectappId)
        {
            APPManageFacade appManageFacade = new APPManageFacade();
            var list = appManageFacade.GetAPPManageList();
            Guid appId = new Guid();
            if (selectappId == null)
            {
                if (list.Count > 0)
                {
                    appId = list.FirstOrDefault().AppId;
                }

            }
            else
            {
                appId = Guid.Parse(selectappId);
            }


            if (appId != null)
            {
                int rowCount = 0;
                int pageIndex = 1;
                if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
                {
                    pageIndex = int.Parse(Request.QueryString["currentPage"]);
                }
                int pageSize = 20;
                #region 商品列表
                CommodityFacade comfa = new CommodityFacade();

                CommoditySearchDTO search = new CommoditySearchDTO();
                search.appId = appId;
                search.pageIndex = pageIndex;
                search.pageSize = pageSize;
                search.commodityName = commodityName;
                search.commodityCategory = commodityCategory;


                List<CommodityVM> comcalist = comfa.GetAllCommodityBySellerIDBySalesvolume(search, out rowCount).ToList();

                #endregion
                //var catelist = catefa.GetCategories(appId);
                ViewBag.CommodityList = comcalist;
                ViewBag.Count = rowCount;
                //ViewBag.CategoryList = catelist;

                if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
                {
                    bool BTPUpdSaleCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPUpdSaleCom);

                    ViewBag.BTPUpdSaleCom = BTPUpdSaleCom;
                    ViewBag.IsOrg = true;
                }
            }

            return PartialView();
        }

        #endregion
        #region 获取库存商品

        [CheckAppId]
        public ActionResult StoreIndex()
        {
            Guid appId = WebUtil.AppId;
            CategoryFacade catefa = new CategoryFacade();
            var catelist = catefa.GetCategories(appId);
            ViewBag.CategoryList = catelist;
            ViewBag.AppId = appId.ToString().ToUpper();

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPCreateStockCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPCreateStockCom);
                bool BTPDelSaleCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDelStockCom);
                bool BTPShelfCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPShelfCom);
                LogHelper.Info(string.Format("仓库商品权限:{0},EmoloyeeId:{1},OrgId:{2},AppId:{3},FeatureCode:{4}", BTPCreateStockCom, this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, "BTPCreateStockCom"));
                LogHelper.Info(string.Format("仓库商品权限:{0},EmoloyeeId:{1},OrgId:{2},AppId:{3},FeatureCode:{4}", BTPDelSaleCom, this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, "BTPDelStockCom"));
                LogHelper.Info(string.Format("仓库商品权限:{0},EmoloyeeId:{1},OrgId:{2},AppId:{3},FeatureCode:{4}", BTPShelfCom, this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, "BTPShelfCom"));
                ViewBag.BTPCreateStockCom = BTPCreateStockCom;
                ViewBag.BTPDelSaleCom = BTPDelSaleCom;
                ViewBag.BTPShelfCom = BTPShelfCom;

                ViewBag.IsOrg = true;
            }
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);
            ViewBag.isCustomES = isCustomES;
            return View();
        }

        [CheckAppId]
        public PartialViewResult PartialStoreIndex(string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice)
        {
            Guid appId = WebUtil.AppId;
            ViewBag.appId = appId;
            ViewBag.userId = this.ContextDTO.LoginUserID;
            //将cookie保存的查询条件读取出来。
            if (string.IsNullOrWhiteSpace(commodityName)
             && string.IsNullOrWhiteSpace(commodityCategory)
             && string.IsNullOrWhiteSpace(sSalesvolume)
             && string.IsNullOrWhiteSpace(eSalesvolume)
             && string.IsNullOrWhiteSpace(sPrice)
             && string.IsNullOrWhiteSpace(ePrice))
            {
                HttpCookie hc = Request.Cookies["CommodityStore.SearchEntityJson." + appId.ToString().ToUpper()];
                if (hc != null && (!string.IsNullOrWhiteSpace(hc.Value))
                    && (hc.Value != ""))
                {
                    string str = HttpUtility.UrlDecode(hc.Value);
                    JObject sn = JObject.Parse(str);

                    commodityName = sn["commodityName"].ToString();
                    commodityCategory = sn["commodityCategory"].ToString();
                    sSalesvolume = sn["sSalesvolume"].ToString();
                    eSalesvolume = sn["eSalesvolume"].ToString();
                    sPrice = sn["sPrice"].ToString();
                    ePrice = sn["ePrice"].ToString();

                }

            }

            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 100;
            int rowCount = 0;
            #region 商品列表
            CommodityFacade comfa = new CommodityFacade();

            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageIndex = pageIndex;
            search.pageSize = pageSize;
            search.commodityName = commodityName;
            search.commodityCategory = commodityCategory;
            search.sSalesvolume = sSalesvolume;
            search.eSalesvolume = eSalesvolume;
            search.sPrice = sPrice;
            search.ePrice = ePrice;

            List<CommodityVM> comcalist = comfa.GetAllNoOnSaleCommodityBySellerIDBySalesvolume(search, out rowCount).ToList();
            #endregion
            //var catelist = catefa.GetCategories(appId);
            //ViewBag.CategoryList = catelist;
            ViewBag.CommodityList = comcalist;
            ViewBag.Count = rowCount;

            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                bool BTPUpdStockCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPUpdStockCom);
                ViewBag.BTPUpdStockCom = BTPUpdStockCom;

                ViewBag.IsOrg = true;
            }
            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);
            ViewBag.isCustomES = isCustomES;
            return PartialView();
        }
        #endregion
        #region 添加商品

        public ActionResult AddCommodity()
        {
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (appId != null)
            {
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }

            Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel = APPSV.Instance.GetAppOwnerInfo(appId);
            if (appModel != null && appModel.OwnerType == 0)
            {
                CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoDTO = CBCSV.Instance.GetOrgInfoNewBySubId(appModel.OwnerId);
                if (orgInfoDTO == null || string.IsNullOrEmpty(orgInfoDTO.CompanyPhone))
                {
                    ViewBag.OrgUrl = "cbc";
                }
            }
            Guid ColorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Guid SizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");

            CommodityFacade comf = new CommodityFacade();
            CommodityCookie cc = new CommodityCookie();
            HttpCookie cookie = Request.Cookies["SaveCommodityCookie"];
            if (cookie != null)
            {
                if (cookie["AppId"] != null)
                {
                    //cookie中的appid相同，展示cookie数据给用户
                    if (cookie["AppId"].ToString() == strAppId)
                    {
                        cc.Picture = HttpUtility.UrlDecode(cookie["Picture"]);
                        cc.CommodityName = HttpUtility.UrlDecode(cookie["CommodityName"]);
                        cc.CommodityStock = HttpUtility.UrlDecode(cookie["CommodityStock"]);
                        cc.State = HttpUtility.UrlDecode(cookie["State"]);
                        cc.CommodityCode = HttpUtility.UrlDecode(cookie["CommodityCode"]);
                        cc.CommodityMarketPrice = HttpUtility.UrlDecode(cookie["CommodityMarketPrice"]);
                        cc.CommodityPrice = HttpUtility.UrlDecode(cookie["CommodityPrice"]);
                        cc.CommodityDuty = HttpUtility.UrlDecode(cookie["CommodityDuty"]);
                        cc.CommodityTaxRate = HttpUtility.UrlDecode(cookie["CommodityTaxRate"]);
                        cc.CommodityInputTax = HttpUtility.UrlDecode(cookie["CommodityInputTax"]);
                        cc.TaxClassCode = HttpUtility.UrlDecode(cookie["TaxClassCode"]);
                        cc.Unit = HttpUtility.UrlDecode(cookie["Unit"]);
                        cc.ImgList = HttpUtility.UrlDecode(cookie["ImgList"]);
                        cc.CommodityDetails = HttpUtility.UrlDecode(cookie["CommodityDetails"]);
                        cc.TechSpecs = HttpUtility.UrlDecode(cookie["TechSpecs"]);
                        cc.SaleService = HttpUtility.UrlDecode(cookie["SaleService"]);
                        cc.CommoditCategory = HttpUtility.UrlDecode(cookie["CommoditCategory"]);
                        cc.CommoditCategoryName = HttpUtility.UrlDecode(cookie["CommoditCategoryName"]);
                        cc.CommoditySizeids = HttpUtility.UrlDecode(cookie["CommoditySize"]);
                        cc.CommodityColorids = HttpUtility.UrlDecode(cookie["CommodityColor"]);
                        cc.CommoditySizeName = HttpUtility.UrlDecode(cookie["CommoditySizeName"]);
                        cc.CommodityColorName = HttpUtility.UrlDecode(cookie["CommodityColorName"]);
                        cc.listImgShowSrcString = HttpUtility.UrlDecode(cookie["listImgShowSrcString"]);
                        cc.Hidpic = HttpUtility.UrlDecode(cookie["hidpic"]);
                        cc.SelectAttr = cookie["SelectAttr"];
                        cc.FristAttrValueList = cookie["FristAttrValueList"];
                        cc.TwoAttrValueList = cookie["TwoAttrValueList"];
                        cc.InputAttrValue = cookie["InputAttrValue"];
                        cc.FreightId = HttpUtility.UrlDecode(cookie["FreightId"]);
                        cc.FreightName = HttpUtility.UrlDecode(cookie["FreightName"]);

                        // 20170918新增
                        cc.CommoditInnerCategory = HttpUtility.UrlDecode(cookie["CommoditInnerCategory"]);
                        cc.CommoditInnerCategoryName = HttpUtility.UrlDecode(cookie["CommoditInnerCategoryName"]);
                        cc.BarCode = HttpUtility.UrlDecode(cookie["BarCode"]);
                        cc.JDCode = HttpUtility.UrlDecode(cookie["JDCode"]);
                        cc.CostPrice = HttpUtility.UrlDecode(cookie["CostPrice"]);

                        cc.EsCategory = HttpUtility.UrlDecode(cookie["EsCategory"]);
                        cc.EsCategoryName = HttpUtility.UrlDecode(cookie["EsCategoryName"]);


                        if (!string.IsNullOrWhiteSpace(cookie["IsAssurance"]))
                        {
                            cc.IsAssurance = (cookie["IsAssurance"].ToString() == "0" ? false : true);
                        }
                        else
                        {
                            cc.IsAssurance = false;
                        }
                        if (!string.IsNullOrWhiteSpace(cookie["IsReturns"]))
                        {
                            cc.IsReturns = (cookie["IsReturns"].ToString() == "0" ? false : true);
                        }
                        else
                        {
                            cc.IsReturns = false;
                        }
                        if (!string.IsNullOrWhiteSpace(cookie["Isnsupport"]))
                        {
                            cc.Isnsupport = (cookie["Isnsupport"].ToString() == "0" ? false : true);
                        }
                        else
                        {
                            cc.Isnsupport = false;
                        }

                        if (!string.IsNullOrWhiteSpace(cookie["ServiceSettingId"]))
                        {
                            cc.ServiceSettingId = HttpUtility.UrlDecode(cookie["ServiceSettingId"]);
                        }
                        else
                        {
                            cc.ServiceSettingId = "";
                        }
                        int IsEnableSelfTake = 0;
                        int.TryParse(HttpUtility.UrlDecode(cookie["IsEnableSelfTake"]), out IsEnableSelfTake);
                        cc.IsEnableSelfTake = IsEnableSelfTake;

                        //运费计费方式
                        cc.PricingMethod = HttpUtility.UrlDecode(cookie["PricingMethod"]);

                        //重量参数
                        cc.Weight = HttpUtility.UrlDecode(cookie["Weight"]);

                        //销售地区
                        //cc.SaleAreas = HttpUtility.UrlDecode(cookie["SaleAreas"]);
                        //cc.SaleAreasText = HttpUtility.UrlDecode(cookie["SaleAreasText"]);

                        cc.CommodityBoxPrice = HttpUtility.UrlDecode(cookie["CommodityBoxPrice"]);
                        cc.CommodityBoxCount = HttpUtility.UrlDecode(cookie["CommodityBoxCount"]);

                        //积分比例
                        cc.ScoreScale = HttpUtility.UrlDecode(cookie["ScoreScale"]);
                        // 20171117新增
                        cc.CommodityType = HttpUtility.UrlDecode(cookie["CommodityType"]);
                        if (cc.CommodityType == "1")
                        {
                            cc.YJCouponActivityId = HttpUtility.UrlDecode(cookie["YJCouponActivityId"]);
                            cc.YJCouponType = HttpUtility.UrlDecode(cookie["YJCouponType"]);
                        }

                    }
                    else
                    {
                        cc = null;
                    }
                }
                else
                {
                    cc = null;
                }

                ViewBag.Cookie = cc;
            }

            var isYJBJ = false;
            var showJDCode = false;
            var showErQiCode = false;
            // 新增 品牌墙需求，判断是否为易捷北京以及入驻到易捷北京的App,
            ViewBag.BrandAppId = appId;
            ViewBag.IsYJApp = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
                showJDCode = true;
                ViewBag.IsYJApp = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    ViewBag.BrandAppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                    isYJBJ = true;
                    if (mall.Type == 0 || mall.Type == 2 || mall.Type == 3)
                    {
                        showJDCode = true;
                    }
                    if (mall.Type == 2 || mall.Type == 3) showErQiCode = true;
                }
            }

            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowErQiCode = showErQiCode;
            ViewBag.ShowJDCode = showJDCode;

            // 查询商品类目
            CategoryFacade catefa = new CategoryFacade();
            var catelist = catefa.GetCategories(appId);
            if (catelist.Count() == 0)//查询不到类目则创建
            {
                catefa.CreatCategory2(appId);
                catelist = catefa.GetCategories(appId);
            }
            ViewBag.CategoryList = catelist;

            // 易捷北京APP查询商城品类
            if (isYJBJ)
            {
                InnerCategoryFacade innerCatefa = new InnerCategoryFacade();
                var innerCatelist = innerCatefa.GetCategories(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                if (innerCatelist.Count() == 0)
                {
                    innerCatefa.CreatCategory2(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                    innerCatelist = innerCatefa.GetCategories(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                }
                ViewBag.InnerCategoryList = innerCatelist;
            }

            var maFacade = new MallApplyFacade();

            var mallDto = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO();
            mallDto.AppId = appId;
            var appInfo = maFacade.GetMallApplyInfoList(mallDto).FirstOrDefault();

            if (appInfo != null && appInfo.EsAppId != null && appInfo.EsAppId != Guid.Empty)
            {
                var esCategoryList = catefa.GetCategories(appInfo.EsAppId);
                if (esCategoryList.Count() == 0)
                {
                    catefa.CreatCategory2(appId);
                    esCategoryList = catefa.GetCategories(appInfo.EsAppId);
                }
                ViewBag.EsCategoryList = esCategoryList;
            }
            else
            {
                LogHelper.Info(string.Format("获取店铺信息查询: 参数{0},结果{1}", appId, appInfo?.EsAppId));
            }


            ComAttibuteFacade comaf = new ComAttibuteFacade();
            var Attributelist = comaf.GetSecondAttribute(appId);
            List<SecondAttributeDTO> size = Attributelist.Where(n => n.AttributeId == SizeId).ToList();
            List<SecondAttributeDTO> color = Attributelist.Where(n => n.AttributeId == ColorId).ToList();

            //zgx-Modify 
            SecondAttributeFacade sf = new SecondAttributeFacade();
            ViewBag.AttributeList = sf.GetAttributeByAppID(appId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            string attribJsonResult = js.Serialize(Attributelist);
            ViewBag.AttributeValueJson = attribJsonResult;

            FreightFacade freight = new FreightFacade();
            ViewBag.FreightList = JsonHelper.JsonSerializer(freight.GetFreightListByAppId(appId));

            ViewBag.Sizelist = size;
            ViewBag.Colorlist = color;

            if (Jinher.AMP.BTP.TPS.ZPHSV.Instance.CheckIsAppInZPH(appId))
            {
                ViewBag.IsAppInZPH = "block";
            }
            else
            {
                ViewBag.IsAppInZPH = "none";
            }
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);

            //匿名账号
            Guid UserId = this.ContextDTO.LoginUserID;
            Guid SessionId = new Guid(this.ContextDTO.SessionID);
            Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;
            ViewBag.ProvinceCityUrl = CustomConfig.ProvinceCityUrl + "?userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg;
            ViewBag.VideoHost = CustomConfig.VideoHost + "?appId=" + appId + "&userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg + "&MediaType =1";
            ;
            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            bool hasVideoFunction = BACBP.CheckCommodityVideo(appId);
            ViewBag.hasVideoFunction = hasVideoFunction;
            ViewBag.isCustomES = isCustomES;

            if (isYJBJ)
            {
                ViewBag.IsHaveMallApply = true;
            }
            else
            {
                //是否是中石化的入驻商家
                Guid zshAppid = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                MallApplyFacade mallApplyFacade = new MallApplyFacade();
                ViewBag.IsHaveMallApply = mallApplyFacade.IsHaveMallApply(zshAppid, appId).isSuccess;
            }
            if (isYJBJ && appId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                ViewBag.IsShowEsCategory = true;
            }
            else
            {
                ViewBag.IsShowEsCategory = false;
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddCommodity(FormCollection collection)
        {
            LogHelper.Info("添加商品开始");
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            var isYJBJ = false;
            var showJDCode = false;
            ViewBag.IsYJApp = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
                showJDCode = true;
                ViewBag.IsYJApp = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    isYJBJ = true;
                    if (mall.Type == 0 || mall.Type == 2)
                    {
                        showJDCode = true;
                    }
                }
            }
            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowJDCode = showJDCode;

            CommodityFacade comf = new CommodityFacade();
            CommodityAndCategoryDTO commodityAndCategoryDTO = new CommodityAndCategoryDTO();
            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["Picture"], false);
            commodityAndCategoryDTO.PicturesPath = PictureUrl;
            commodityAndCategoryDTO.Name = collection["CommodityName"];
            commodityAndCategoryDTO.CateNames = collection["CategoryName"];

            //20171117新增
            int commodityType;
            int.TryParse(collection["CommodityType"], out commodityType);
            commodityAndCategoryDTO.CommodityType = commodityType;
            if (commodityAndCategoryDTO.CommodityType == 1)
            {
                commodityAndCategoryDTO.YJCouponActivityId = collection["YJCouponActivityId"];
                commodityAndCategoryDTO.YJCouponType = collection["YJCouponType"];
            }
            int commodityStock = 0;
            if (!int.TryParse(collection["CommodityStock"], out commodityStock))
            {
                return Json(new { Result = false, Messages = "商品数量数据不合法" });
            }
            commodityAndCategoryDTO.Stock = commodityStock;

            commodityAndCategoryDTO.State = Convert.ToInt32(collection["State"]);
            commodityAndCategoryDTO.No_Code = collection["CommodityCode"];
            commodityAndCategoryDTO.AppId = appId;

            if (!string.IsNullOrEmpty(collection["CommodityMarketPrice"]))
            {
                decimal marketPrice = 0;
                if (!decimal.TryParse(collection["CommodityMarketPrice"], out marketPrice))
                {
                    return Json(new { Result = false, Messages = "市场价数据不合法" });
                }
                commodityAndCategoryDTO.MarketPrice = marketPrice;
            }

            decimal commodityPrice = 0;
            if (!decimal.TryParse(collection["CommodityPrice"], out commodityPrice))
            {
                return Json(new { Result = false, Messages = "商品价格数据不合法" });
            }
            commodityAndCategoryDTO.Price = commodityPrice;

            if (!string.IsNullOrEmpty(collection["CommodityDuty"]))
            {
                decimal duty = 0;
                if (!decimal.TryParse(collection["CommodityDuty"], out duty))
                {
                    return Json(new { Result = false, Messages = "关税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityDuty = duty;
            }

            if (!string.IsNullOrEmpty(collection["CommodityTaxRate"]))
            {
                decimal taxRate = 0;
                if (!decimal.TryParse(collection["CommodityTaxRate"], out taxRate))
                {
                    return Json(new { Result = false, Messages = "商品销项税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityTaxRate = taxRate;
            }

            if (!string.IsNullOrEmpty(collection["CommodityInputTax"]))
            {
                decimal inputTax = 0;
                if (!decimal.TryParse(collection["CommodityInputTax"], out inputTax))
                {
                    return Json(new { Result = false, Messages = "商品进项税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityInputTax = inputTax;
            }

            commodityAndCategoryDTO.TaxClassCode = collection["TaxClassCode"];
            commodityAndCategoryDTO.Unit = collection["Unit"];

            // 20170918新增
            if (isYJBJ)
            {
                commodityAndCategoryDTO.InnerCateNames = collection["InnerCategoryName"] == "undefined" ? null : collection["InnerCategoryName"];
                commodityAndCategoryDTO.JDCode = collection["JDCode"] == "undefined" ? null : collection["JDCode"];
                commodityAndCategoryDTO.BarCode = collection["BarCode"];
                commodityAndCategoryDTO.ErQiCode = collection["ErQiCode"] == "undefined" ? null : collection["ErQiCode"];

                decimal costPrice = 0;
                if (!decimal.TryParse(collection["CostPrice"], out costPrice))
                {
                    return Json(new { Result = false, Messages = "商品进货价数据不合法" });
                }
                commodityAndCategoryDTO.CostPrice = costPrice;
            }

            commodityAndCategoryDTO.EsCategory = collection["EsCategory"];
            commodityAndCategoryDTO.EsCagetoryName = collection["EsCategoryName"];



            if (collection["IsAssurance"] == "undefined" || string.IsNullOrEmpty(collection["IsAssurance"]))
            {
                commodityAndCategoryDTO.IsAssurance = false;
            }
            else
            {
                commodityAndCategoryDTO.IsAssurance = (collection["IsAssurance"].ToString() == "0" ? false : true);
            }
            if (collection["IsReturns"] == "undefined" || string.IsNullOrEmpty(collection["IsReturns"]))
            {
                commodityAndCategoryDTO.IsReturns = false;
            }
            else
            {
                commodityAndCategoryDTO.IsReturns = (collection["IsReturns"].ToString() == "0" ? false : true);
            }
            if (collection["Isnsupport"] == "undefined" || string.IsNullOrEmpty(collection["Isnsupport"]))
            {
                commodityAndCategoryDTO.Isnsupport = false;
            }
            else
            {
                commodityAndCategoryDTO.Isnsupport = (collection["Isnsupport"].ToString() == "0" ? false : true);
            }
            if (collection["ServiceSettingId"] == "undefined" || string.IsNullOrEmpty(collection["ServiceSettingId"]))
            {
                commodityAndCategoryDTO.ServiceSettingId = null;
            }
            else
            {
                commodityAndCategoryDTO.ServiceSettingId = collection["ServiceSettingId"].ToString();
            }
            string picUrl = string.Empty;
            if (!string.IsNullOrEmpty(collection["CommodityImgList"]))
            {
                string imgList = collection["CommodityImgList"];
                commodityAndCategoryDTO.Picturelist = imgList != null ? imgList.Split(',').ToList() : null;
                //helper.UploadADPicList(imgList);
            }
            var desc = collection["CommodityDetails"] == "" ? "此商品无描述信息" : HttpUtility.UrlDecode(collection["CommodityDetails"]);
            if (!string.IsNullOrWhiteSpace(desc))
            {
                desc = desc.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.Description = desc;

            var techSpecs = collection["TechSpecs"] == "" ? "没有更多了~" : HttpUtility.UrlDecode(collection["TechSpecs"]);
            if (!string.IsNullOrWhiteSpace(techSpecs))
            {
                techSpecs = techSpecs.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.TechSpecs = techSpecs;

            var saleService = collection["SaleService"] == "" ? "没有更多了~" : HttpUtility.UrlDecode(collection["SaleService"]);
            if (!string.IsNullOrWhiteSpace(saleService))
            {
                saleService = saleService.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.SaleService = saleService;


            commodityAndCategoryDTO.CategoryPath = collection["CommoditCategory"];
            commodityAndCategoryDTO.InnerCategoryPath = collection["CommoditInnerCategory"];
            commodityAndCategoryDTO.SizeIds = collection["CommoditySize"];
            commodityAndCategoryDTO.ColorIds = collection["CommodityColor"];
            commodityAndCategoryDTO.ColorNames = collection["ColorNames"];
            commodityAndCategoryDTO.SizeNames = collection["SizeNames"];
            commodityAndCategoryDTO.AttributeIds = commodityAndCategoryDTO.SizeIds + ',' + commodityAndCategoryDTO.ColorIds;

            //zgx-modify
            commodityAndCategoryDTO.AttrName = collection["AttrName"];
            commodityAndCategoryDTO.AttrId = collection["AttrId"];
            commodityAndCategoryDTO.AttrValueIds = collection["AttrIds"];
            commodityAndCategoryDTO.AttrValueNames = collection["AttrNames"];
            commodityAndCategoryDTO.FreightId = collection["FreightId"];
            commodityAndCategoryDTO.FreightName = collection["FreightName"];
            commodityAndCategoryDTO.RelaCommodityList = collection["RelaCommodityList"];

            string attrobjstr = collection["AttrObj"];
            if (!string.IsNullOrEmpty(attrobjstr))
            {
                attrobjstr = attrobjstr.Replace("\"undefined\"", "null");
                // LogHelper.Info("attrobjstr：" + attrobjstr);
                commodityAndCategoryDTO.ComAttributes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityStockDTO>>(attrobjstr);
                foreach (var item in commodityAndCategoryDTO.ComAttributes)
                {
                    if (!string.IsNullOrEmpty(item.CarouselImgs))
                    {
                        //将新文件上传。UploadADPicList会过滤掉已上传过的以http开头的文件。
                        List<string> newimgs = helper.UploadADPicList(item.CarouselImgs);
                        //保证顺序，只替换新上传的图片。
                        string[] imgs = item.CarouselImgs.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        int j = 0;
                        for (int i = 0; i < imgs.Length; i++)
                        {
                            if ((!imgs[i].StartsWith("http://")) && (!imgs[i].StartsWith("https://")) && j < newimgs.Count)
                            {
                                imgs[i] = newimgs[j];
                                j++;
                            }
                        }
                        item.CarouselImgs = string.Join(",", imgs);
                    }
                }
                // LogHelper.Info("ComAttributes：" + JsonHelper.JsonSerializer(commodityAndCategoryDTO.ComAttributes));
            }
            //是否支持自提。
            int isEnableSelfTake = 0;
            int.TryParse(collection["IsEnableSelfTake"], out isEnableSelfTake);
            commodityAndCategoryDTO.IsEnableSelfTake = isEnableSelfTake;

            //运费计费方式
            byte pricingMethod;
            byte.TryParse(collection["PricingMethod"], out pricingMethod);
            commodityAndCategoryDTO.PricingMethod = pricingMethod;

            //重量参数
            decimal weight;
            decimal.TryParse(collection["Weight"], out weight);
            commodityAndCategoryDTO.Weight = weight;

            //销售地区
            commodityAndCategoryDTO.SaleAreas = collection["SaleAreas"];
            //视频地址
            commodityAndCategoryDTO.VideoUrl = collection["VideoUrl"];
            //网页视频地址
            commodityAndCategoryDTO.VideoclientUrl = collection["VideoWebUrl"];
            //视频图片地址
            commodityAndCategoryDTO.VideoPicUrl = collection["VideoPicUrl"];
            //视频名称
            commodityAndCategoryDTO.VideoName = collection["VideoName"];
            //包装规格设置
            commodityAndCategoryDTO.Specifications = collection["Specifications"];
            if (!string.IsNullOrEmpty(collection["From"]) && collection["From"] == "1")
            {
                commodityAndCategoryDTO.From = int.Parse(collection["From"]);
                decimal _Price = -1;
                if (!string.IsNullOrEmpty(collection["CommodityBoxPrice"]) && !decimal.TryParse(collection["CommodityBoxPrice"], out _Price))
                {
                    return Json(new { Result = false, Messages = "餐盒价格数据不合法" });
                }
                commodityAndCategoryDTO.CommodityBoxPrice = _Price;

                int _count = -1;
                if (!string.IsNullOrEmpty(collection["CommodityBoxCount"]) && !int.TryParse(collection["CommodityBoxCount"], out _count))
                {
                    return Json(new { Result = false, Messages = "餐盒数量数据不合法" });
                }
                commodityAndCategoryDTO.CommodityBoxCount = _count;
            }

            //积分比例
            decimal scoreScale;
            decimal.TryParse(collection["ScoreScale"], out scoreScale);
            commodityAndCategoryDTO.ScoreScale = scoreScale;

            //添加商品品牌-2018/06/23
            if (!string.IsNullOrEmpty(collection["BrandId"]))
            {
                commodityAndCategoryDTO.BrandId = Guid.Parse(collection["BrandId"]);
            }
            if (!string.IsNullOrEmpty(collection["BrandName"]))
            {
                commodityAndCategoryDTO.BrandName = collection["BrandName"];
            }
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            ResultDTO result;
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            if (AuditCom.IsAuditAppid(appId))
            {
                //需要审核的商品添加
                commodityAndCategoryDTO.Action = Convert.ToInt32(OperateTypeEnum.商品发布);
                result = AuditCom.AddAuditCommodity(commodityAndCategoryDTO);
            }
            else
            {
                result = comf.SaveCommodity(commodityAndCategoryDTO);
            }
            string re = collection["State"];
            //if (result.ResultCode == 2)
            //{
            //    return Json(new { Result = false, Messages = result.Message });
            //}
            if (result.ResultCode == 0)
            {
                //清除cookie
                HttpCookie cookie = Request.Cookies["SaveCommodityCookie"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(-10);
                    Response.Cookies.Add(cookie);
                }
                return Json(new { Result = true, Messages = "发布成功", CommodityUrl = re });
            }
            else
            {
                return Json(new { Result = false, Messages = result.Message });
            }
            //if (re == "0")
            //{
            //    return Json(new { Result = false, Messages = "发布失败" });
            //}
            //else
            //{
            //    return Json(new { Result = false, Messages = "保存失败" });
            //}
        }
        #endregion
        #region 修改商品
        public ActionResult UpdateCommodity(Guid commodityId)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            Guid ColorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Guid SizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            CommodityFacade comf = new CommodityFacade();
            CategoryFacade catefa = new CategoryFacade();
            ComAttibuteFacade comaf = new ComAttibuteFacade();
            var catelist = catefa.GetCategories(appId);
            var Attributelist = comaf.GetSecondAttribute(appId);
            List<SecondAttributeDTO> size = Attributelist.Where(n => n.AttributeId == SizeId).ToList();
            List<SecondAttributeDTO> color = Attributelist.Where(n => n.AttributeId == ColorId).ToList();
            CommodityAndCategoryDTO commodity = comf.GetCommodity(commodityId, appId);
            if (commodity != null && !string.IsNullOrEmpty(commodity.Description))
                commodity.Description = commodity.Description.Replace("\n", "").Replace("\r", "");
            //查询商品品牌信息
            CommodityInnerBrandFacade cibf = new CommodityInnerBrandFacade();
            CommodityInnerBrandDTO comBrand = cibf.GetComInnerBrand(commodityId);
            if (comBrand != null)
            {
                commodity.BrandId = comBrand.Id;
                commodity.BrandName = comBrand.Name;
            }
            var isYJBJ = false;
            var showJDCode = false;
            var showErQiCode = false;
            // 新增 品牌墙需求，判断是否为易捷北京以及入驻到易捷北京的App,
            ViewBag.BrandAppId = appId;
            ViewBag.IsYJApp = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
                showJDCode = true;
                ViewBag.IsYJApp = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    ViewBag.BrandAppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                    isYJBJ = true;
                    if (mall.Type == 0 || mall.Type == 2 || mall.Type == 3)
                    {
                        showJDCode = true;
                    }
                    if (mall.Type == 2 || mall.Type == 3) showErQiCode = true;
                }
            }

            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowJDCode = showJDCode;
            ViewBag.ShowErQiCode = showErQiCode;
            // 易捷北京APP查询商城品类
            if (isYJBJ)
            {
                InnerCategoryFacade innerCatefa = new InnerCategoryFacade();
                var innerCatelist = innerCatefa.GetCategories(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                if (innerCatelist.Count() == 0)
                {
                    innerCatefa.CreatCategory2(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                    innerCatelist = innerCatefa.GetCategories(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                }
                ViewBag.InnerCategoryList = innerCatelist;
            }

            ViewBag.Sizelist = size;
            ViewBag.Colorlist = color;
            ViewBag.CategoryList = catelist;
            ViewBag.Commodity = commodity;
            ViewBag.NoCode = commodity == null ? "" : commodity.No_Code;
            ViewBag.VideoName = commodity == null ? "" : commodity.VideoName;
            ViewBag.VideoUrl = commodity == null ? "" : commodity.VideoUrl;
            ViewBag.VideoWebUrl = commodity == null ? "" : commodity.VideoclientUrl;
            ViewBag.VideoPicUrl = commodity == null ? "" : commodity.VideoPicUrl;
            ViewBag.AppId = appId;

            //zgx-modify
            ViewBag.First = "";
            ViewBag.Two = "";
            ViewBag.SelectAttr = "";
            ViewBag.SelectAttrValue = "";
            ViewBag.FreightId = commodity.FreightId;

            if (commodity.ComAttributes != null && commodity.ComAttributes.Count > 0)
            {
                ViewBag.SelectAttrValue = JsonHelper.JsonSerializer<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityStockDTO>>(commodity.ComAttributes);
                List<AttrNameAndIdOrder> f = new List<AttrNameAndIdOrder>();
                List<AttrNameAndIdOrder> t = new List<AttrNameAndIdOrder>();
                List<AttrNameAndIdOrder> s = new List<AttrNameAndIdOrder>();
                bool first = true;
                commodity.ComAttributes.ForEach(r =>
                {
                    if (r.ComAttributeIds.Count() > 0 && r.ComAttributeIdOrders.Count() > 0)
                    {
                        if (!f.Exists(e => e.Id == r.ComAttributeIds[0].SecondAttributeId && e.Name.ToLower() == r.ComAttributeIds[0].SecondAttribute.ToLower()))
                        {
                            f.Add(new AttrNameAndIdOrder
                            {
                                Id = r.ComAttributeIds[0].SecondAttributeId,
                                Name = r.ComAttributeIds[0].SecondAttribute,
                                OrderTime = r.ComAttributeIdOrders[0].OrderTime

                            });
                        }
                        if (r.ComAttributeIds.Count() == 2 && r.ComAttributeIds.Count() == 2)
                        {
                            if (!t.Exists(e => e.Id == r.ComAttributeIds[1].SecondAttributeId && e.Name.ToLower() == r.ComAttributeIds[1].SecondAttribute.ToLower()))
                            {
                                t.Add(new AttrNameAndIdOrder
                                {
                                    Id = r.ComAttributeIds[1].SecondAttributeId,
                                    Name = r.ComAttributeIds[1].SecondAttribute,
                                    OrderTime = r.ComAttributeIdOrders[1].OrderTime
                                });
                            }
                        }
                        if (first)
                        {
                            if (r.ComAttributeIds.Count() == 1 && r.ComAttributeIds.Count() == 1)
                            {
                                s.Add(new AttrNameAndIdOrder
                                {
                                    Id = r.ComAttributeIds[0].AttributeId,
                                    Name = r.ComAttributeIds[0].Attribute,
                                    OrderTime = r.ComAttributeIdOrders[0].OrderTime
                                });
                            }
                            else
                            {
                                s.Add(new AttrNameAndIdOrder
                                {
                                    Id = r.ComAttributeIds[0].AttributeId,
                                    Name = r.ComAttributeIds[0].Attribute,
                                    OrderTime = r.ComAttributeIdOrders[0].OrderTime
                                });
                                s.Add(new AttrNameAndIdOrder
                                {
                                    Id = r.ComAttributeIds[1].AttributeId,
                                    Name = r.ComAttributeIds[1].Attribute,
                                    OrderTime = r.ComAttributeIdOrders[1].OrderTime
                                });
                            }
                            first = false;
                        }
                    }
                });
                f = f.OrderBy(r => r.OrderTime).ToList();
                t = t.OrderBy(r => r.OrderTime).ToList();
                ViewBag.First = JsonHelper.JsonSerializer<List<AttrNameAndIdOrder>>(f);
                ViewBag.Two = JsonHelper.JsonSerializer<List<AttrNameAndIdOrder>>(t);
                ViewBag.SelectAttr = JsonHelper.JsonSerializer<List<AttrNameAndIdOrder>>(s);
            }
            else if (!string.IsNullOrEmpty(commodity.AttrValueNames))
            {
                List<AttrNameAndId> f = new List<AttrNameAndId>();
                List<AttrNameAndId> s = new List<AttrNameAndId>();
                s.Add(new AttrNameAndId
                {
                    Id = Guid.Parse(commodity.AttrId),
                    Name = commodity.AttrName
                });
                string[] av = commodity.AttrValueNames.Split(',');
                string[] avids = commodity.AttrValueIds.Split(',');
                for (int i = 0; i < av.Length; i++)
                {
                    f.Add(new AttrNameAndId
                    {
                        Id = Guid.Parse(avids[i]),
                        Name = av[i]
                    });
                }

                ViewBag.First = JsonHelper.JsonSerializer<List<AttrNameAndId>>(f);
                ViewBag.SelectAttr = JsonHelper.JsonSerializer<List<AttrNameAndId>>(s);
            }

            //zgx-Modify 
            SecondAttributeFacade sf = new SecondAttributeFacade();
            ViewBag.AttributeList = sf.GetAttributeByAppID(appId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string attribJsonResult = js.Serialize(Attributelist);
            ViewBag.AttributeValueJson = attribJsonResult;
            FreightFacade freight = new FreightFacade();
            ViewBag.FreightList = JsonHelper.JsonSerializer(freight.GetFreightListByAppId(appId));
            if (Jinher.AMP.BTP.TPS.ZPHSV.Instance.CheckIsAppInZPH(appId))
            {
                ViewBag.IsAppInZPH = "block";
            }
            else
            {
                ViewBag.IsAppInZPH = "none";
            }

            Guid UserId = this.ContextDTO.LoginUserID;
            Guid SessionId = new Guid(this.ContextDTO.SessionID);
            Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;
            ViewBag.ProvinceCityUrl = CustomConfig.ProvinceCityUrl + "?userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg;
            ViewBag.VideoHost = CustomConfig.VideoHost + "?appId=" + appId + "&userId=" + UserId + "&sessionId=" + SessionId + "&changeOrg=" + ChangeOrg + "&MediaType =1";
            //TODO 销售地区名称
            string saleAreasText = string.Empty;
            if (commodity.SaleAreas == "000000")
            {
                saleAreasText = "全网销售";
            }
            else if (string.IsNullOrWhiteSpace(commodity.SaleAreas))
            {
                saleAreasText = "未指定销售区域";
            }
            else
            {
                List<string> arearCodes = commodity.SaleAreas.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                saleAreasText = ProvinceCityHelper.GetAreaNamesByCodeList(arearCodes, "、");
            }
            ViewBag.SaleAreasText = saleAreasText;
            //查询是否是定制电商
            bool isCustomES = false;
            isCustomES = APPBP.IsFittedApp(appId);

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);
            bool hasVideoFunction = BACBP.CheckCommodityVideo(appId);
            ViewBag.hasVideoFunction = hasVideoFunction;
            ViewBag.isCustomES = isCustomES;
            if (isYJBJ)
            {
                ViewBag.IsHaveMallApply = true;
            }
            else
            {
                //是否是中石化的入驻商家
                Guid zshAppid = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                MallApplyFacade mallApplyFacade = new MallApplyFacade();
                ViewBag.IsHaveMallApply = mallApplyFacade.IsHaveMallApply(zshAppid, appId).isSuccess;
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateCommodity(FormCollection collection)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            var isYJBJ = false;
            var showJDCode = false;
            ViewBag.IsYJApp = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
                showJDCode = true;
                ViewBag.IsYJApp = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    isYJBJ = true;
                    if (mall.Type == 0 || mall.Type == 2)
                    {
                        showJDCode = true;
                    }
                }
            }
            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowJDCode = showJDCode;

            CommodityFacade comf = new CommodityFacade();
            CommodityAndCategoryDTO commodityAndCategoryDTO = new CommodityAndCategoryDTO();
            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["Picture"], false);
            commodityAndCategoryDTO.CommodityId = new Guid(collection["commodityId"]);
            commodityAndCategoryDTO.PicturesPath = PictureUrl;
            commodityAndCategoryDTO.Name = collection["CommodityName"];
            commodityAndCategoryDTO.ColorNames = collection["ColorNames"];
            commodityAndCategoryDTO.SizeNames = collection["SizeNames"];
            commodityAndCategoryDTO.CateNames = collection["CategoryName"];
            //2017-11-23新增
            int commodityType;
            int.TryParse(collection["CommodityType"], out commodityType);
            commodityAndCategoryDTO.CommodityType = commodityType;//商品类型
            if (commodityAndCategoryDTO.CommodityType == 1)
            {
                commodityAndCategoryDTO.YJCouponActivityId = collection["YJCouponActivityId"];//活动编码
                commodityAndCategoryDTO.YJCouponType = collection["YJCouponType"];//类型编码
            }

            int commodityStock = 0;
            if (!int.TryParse(collection["CommodityStock"], out commodityStock))
            {
                return Json(new { Result = false, Messages = "商品数量数据不合法" });
            }
            commodityAndCategoryDTO.Stock = commodityStock;

            commodityAndCategoryDTO.State = Convert.ToInt32(collection["State"]);
            commodityAndCategoryDTO.No_Code = collection["CommodityCode"];
            string hidden_code = collection["Hid_Code"];
            commodityAndCategoryDTO.AppId = appId;

            if (!string.IsNullOrEmpty(collection["CommodityMarketPrice"]))
            {
                decimal marketPrice;
                if (!decimal.TryParse(collection["CommodityMarketPrice"], out marketPrice))
                {
                    return Json(new { Result = false, Messages = "市场价数据不合法" });
                }
                commodityAndCategoryDTO.MarketPrice = marketPrice;
            }
            else
            {
                commodityAndCategoryDTO.MarketPrice = null;
            }
            if (!string.IsNullOrEmpty(collection["CommodityDuty"]))
            {
                decimal duty;
                if (!decimal.TryParse(collection["CommodityDuty"], out duty))
                {
                    return Json(new { Result = false, Messages = "关税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityDuty = duty;
            }
            else
            {
                commodityAndCategoryDTO.CommodityDuty = null;
            }
            if (!string.IsNullOrEmpty(collection["CommodityTaxRate"]))
            {
                decimal taxRate;
                if (!decimal.TryParse(collection["CommodityTaxRate"], out taxRate))
                {
                    return Json(new { Result = false, Messages = "商品销项税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityTaxRate = taxRate;
            }
            else
            {
                commodityAndCategoryDTO.CommodityTaxRate = null;
            }
            if (!string.IsNullOrEmpty(collection["CommodityInputTax"]))
            {
                decimal inputTax;
                if (!decimal.TryParse(collection["CommodityInputTax"], out inputTax))
                {
                    return Json(new { Result = false, Messages = "商品进项税数据不合法" });
                }
                commodityAndCategoryDTO.CommodityInputTax = inputTax;
            }
            else
            {
                commodityAndCategoryDTO.CommodityInputTax = null;
            }
            commodityAndCategoryDTO.TaxClassCode = collection["TaxClassCode"];
            commodityAndCategoryDTO.Unit = collection["Unit"];
            decimal commodityPrice = 0;
            if (!decimal.TryParse(collection["CommodityPrice"], out commodityPrice))
            {
                return Json(new { Result = false, Messages = "商品价格数据不合法" });
            }
            commodityAndCategoryDTO.Price = commodityPrice;

            // 20170918新增
            if (isYJBJ)
            {
                commodityAndCategoryDTO.InnerCateNames = collection["InnerCategoryName"] == "undefined" ? null : collection["InnerCategoryName"];
                commodityAndCategoryDTO.BarCode = collection["BarCode"];
                commodityAndCategoryDTO.JDCode = collection["JDCode"] == "undefined" ? null : collection["JDCode"];
                commodityAndCategoryDTO.ErQiCode = collection["ErQiCode"] == "undefined" ? null : collection["ErQiCode"];
                decimal costPrice = 0;
                if (!decimal.TryParse(collection["CostPrice"], out costPrice))
                {
                    return Json(new { Result = false, Messages = "商品进货价数据不合法" });
                }
                commodityAndCategoryDTO.CostPrice = costPrice;
            }
            if (collection["IsAssurance"] == "undefined" || string.IsNullOrEmpty(collection["IsAssurance"]))
            {
                commodityAndCategoryDTO.IsAssurance = false;
            }
            else
            {
                commodityAndCategoryDTO.IsAssurance = (collection["IsAssurance"].ToString() == "0" ? false : true);
            }
            if (collection["IsReturns"] == "undefined" || string.IsNullOrEmpty(collection["IsReturns"]))
            {
                commodityAndCategoryDTO.IsReturns = false;
            }
            else
            {
                commodityAndCategoryDTO.IsReturns = (collection["IsReturns"].ToString() == "0" ? false : true);
            }
            if (collection["Isnsupport"] == "undefined" || string.IsNullOrEmpty(collection["Isnsupport"]))
            {
                commodityAndCategoryDTO.Isnsupport = false;
            }
            else
            {
                commodityAndCategoryDTO.Isnsupport = (collection["Isnsupport"].ToString() == "0" ? false : true);
            }


            if (collection["ServiceSettingId"] == "undefined" || string.IsNullOrEmpty(collection["ServiceSettingId"]))
            {
                commodityAndCategoryDTO.ServiceSettingId = null;
            }
            else
            {
                commodityAndCategoryDTO.ServiceSettingId = collection["ServiceSettingId"].ToString();
            }
            string picUrl = string.Empty;
            if (!string.IsNullOrEmpty(collection["CommodityImgList"]))
            {
                string imgList = collection["CommodityImgList"];

                //将新文件上传。UploadADPicList会过滤掉已上传过的以http开头的文件。
                List<string> newimgs = helper.UploadADPicList(imgList);

                //保证顺序，只替换新上传的图片。
                string[] imgs = imgList.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                int j = 0;
                for (int i = 0; i < imgs.Length; i++)
                {
                    if ((!imgs[i].StartsWith("http://")) && (!imgs[i].StartsWith("https://"))
                        && j < newimgs.Count)
                    {
                        imgs[i] = newimgs[j];
                        j++;
                    }
                }
                commodityAndCategoryDTO.Picturelist = imgs.ToList();
            }
            var desc = collection["CommodityDetails"] == "" ? "此商品无描述信息" : HttpUtility.UrlDecode(collection["CommodityDetails"]);
            if (!string.IsNullOrWhiteSpace(desc))
            {
                desc = desc.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.Description = desc;

            var techSpecs = collection["TechSpecs"] == "" ? "没有更多了~" : HttpUtility.UrlDecode(collection["TechSpecs"]);
            if (!string.IsNullOrWhiteSpace(techSpecs))
            {
                techSpecs = techSpecs.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.TechSpecs = techSpecs;

            var saleService = collection["SaleService"] == "" ? "没有更多了~" : HttpUtility.UrlDecode(collection["SaleService"]);
            if (!string.IsNullOrWhiteSpace(saleService))
            {
                saleService = saleService.Replace("\n", "").Replace("\r", "");
            }
            commodityAndCategoryDTO.SaleService = saleService;

            commodityAndCategoryDTO.CategoryPath = collection["CommoditCategory"];
            commodityAndCategoryDTO.InnerCategoryPath = collection["CommoditInnerCategory"];
            commodityAndCategoryDTO.SizeIds = collection["CommoditySize"];
            commodityAndCategoryDTO.ColorIds = collection["CommodityColor"];
            commodityAndCategoryDTO.AttributeIds = commodityAndCategoryDTO.SizeIds + ',' + commodityAndCategoryDTO.ColorIds;

            //zgx-modify
            commodityAndCategoryDTO.AttrName = collection["AttrName"];
            commodityAndCategoryDTO.AttrId = collection["AttrId"];
            commodityAndCategoryDTO.AttrValueIds = collection["AttrIds"];
            commodityAndCategoryDTO.AttrValueNames = collection["AttrNames"];

            commodityAndCategoryDTO.FreightId = collection["FreightId"];
            commodityAndCategoryDTO.FreightName = collection["FreightName"];
            commodityAndCategoryDTO.RelaCommodityList = collection["RelaCommodityList"];

            string attrobjstr = collection["AttrObj"];
            if (!string.IsNullOrEmpty(attrobjstr))
            {
                attrobjstr = attrobjstr.Replace("\"undefined\"", "null");
                //LogHelper.Info("attrobjstr：" + attrobjstr);
                commodityAndCategoryDTO.ComAttributes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityStockDTO>>(attrobjstr);
                foreach (var item in commodityAndCategoryDTO.ComAttributes)
                {
                    if (!string.IsNullOrEmpty(item.CarouselImgs))
                    {
                        //将新文件上传。UploadADPicList会过滤掉已上传过的以http开头的文件。
                        List<string> newimgs = helper.UploadADPicList(item.CarouselImgs);
                        //保证顺序，只替换新上传的图片。
                        string[] imgs = item.CarouselImgs.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        int j = 0;
                        for (int i = 0; i < imgs.Length; i++)
                        {
                            if ((!imgs[i].StartsWith("http://")) && (!imgs[i].StartsWith("https://")) && j < newimgs.Count)
                            {
                                imgs[i] = newimgs[j];
                                j++;
                            }
                        }
                        item.CarouselImgs = string.Join(",", imgs);
                    }
                }
                //LogHelper.Info("ComAttributes：" + Newtonsoft.Json.JsonConvert.SerializeObject(commodityAndCategoryDTO.ComAttributes));
            }

            //是否支持自提。
            int isEnableSelfTake = 0;
            int.TryParse(collection["IsEnableSelfTake"], out isEnableSelfTake);
            commodityAndCategoryDTO.IsEnableSelfTake = isEnableSelfTake;

            //运费计费方式
            byte pricingMethod;
            byte.TryParse(collection["PricingMethod"], out pricingMethod);
            commodityAndCategoryDTO.PricingMethod = pricingMethod;

            //重量参数
            decimal weight;
            decimal.TryParse(collection["Weight"], out weight);
            commodityAndCategoryDTO.Weight = weight;

            //销售地区
            commodityAndCategoryDTO.SaleAreas = collection["SaleAreas"];
            //视频地址
            commodityAndCategoryDTO.VideoUrl = collection["VideoUrl"];
            //网页视频地址
            commodityAndCategoryDTO.VideoclientUrl = collection["VideoWebUrl"];
            //视频图片地址
            commodityAndCategoryDTO.VideoPicUrl = collection["VideoPicUrl"];
            //视频名称
            commodityAndCategoryDTO.VideoName = collection["VideoName"];
            //商品审核页面操作行为 2018年2月24日增加
            commodityAndCategoryDTO.ActionName = collection["ActionName"];
            //积分比例
            decimal scoreScale;
            decimal.TryParse(collection["ScoreScale"], out scoreScale);
            commodityAndCategoryDTO.ScoreScale = scoreScale;

            if (!string.IsNullOrEmpty(collection["From"]) && collection["From"] == "1")
            {
                commodityAndCategoryDTO.From = int.Parse(collection["From"]);
                decimal _Price = -1;
                if (!string.IsNullOrEmpty(collection["CommodityBoxPrice"]) && !decimal.TryParse(collection["CommodityBoxPrice"], out _Price))
                {
                    return Json(new { Result = false, Messages = "餐盒价格数据不合法" });
                }
                commodityAndCategoryDTO.CommodityBoxPrice = _Price;

                int _count = -1;
                if (!string.IsNullOrEmpty(collection["CommodityBoxCount"]) && !int.TryParse(collection["CommodityBoxCount"], out _count))
                {
                    return Json(new { Result = false, Messages = "餐盒数量数据不合法" });
                }
                commodityAndCategoryDTO.CommodityBoxCount = _count;
            }

            //添加商品品牌-2018/06/23
            if (!string.IsNullOrEmpty(collection["BrandId"]))
            {
                commodityAndCategoryDTO.BrandId = Guid.Parse(collection["BrandId"]);
            }
            if (!string.IsNullOrEmpty(collection["BrandName"]))
            {
                commodityAndCategoryDTO.BrandName = collection["BrandName"];
            }

            //包装规格设置
            commodityAndCategoryDTO.Specifications = collection["Specifications"];
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            ResultDTO result;
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            if (AuditCom.IsAuditAppid(appId))
            {
                if (AuditCom.IsExistCom(commodityAndCategoryDTO.CommodityId, appId))
                {
                    commodityAndCategoryDTO.Action = Convert.ToInt32(OperateTypeEnum.商品发布);
                }
                else
                {
                    commodityAndCategoryDTO.Action = Convert.ToInt32(OperateTypeEnum.商品编辑);
                }
                result = AuditCom.AddAuditCommodity(commodityAndCategoryDTO);
            }
            else
            {
                if (commodityAndCategoryDTO.ActionName == "111")
                {
                    result = comf.SaveCommodity(commodityAndCategoryDTO);
                }
                else
                {
                    result = comf.UpdateCommodity(commodityAndCategoryDTO);
                }
            }
            string re = collection["State"];
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功", CommodityUrl = re });
            }
            else
            {
                return Json(new { Result = false, Messages = result.Message });
            }

        }
        #endregion
        #region 修改类别
        public ActionResult UpdateCommodityCategory(Guid appId)
        {
            CategoryFacade categorys = new CategoryFacade();
            var categorylist = categorys.GetCategories(appId);
            ViewBag.Categorys = categorys;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateCommodityCategory(FormCollection collection)
        {
            UCategoryVM vm = new UCategoryVM();
            vm.CommodityId = new Guid(collection["CommodityId"]);
            vm.ComCateIds = collection["CateogryIdList"];
            CommodityFacade comf = new CommodityFacade();
            //comf.UpdateCategoryBycommodityId(vm);

            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());

            ResultDTO result = comf.UpdateCategoryBycommodityId(vm);

            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功", CommodityUrl = collection["State"] });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion
        #region 修改价格
        [HttpPost]
        public ActionResult UpdatePrice(FormCollection collection)
        {
            CommodityFacade comf = new CommodityFacade();
            Guid id = new Guid(collection["Commodityid"]);
            decimal price = 0;
            if (!decimal.TryParse(collection["Price"], out price))
            {
                return Json(new { Result = false, Messages = "修改失败" });
            }
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.Price == price)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.Price = price;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改现价);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdatePrice(id, price);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion
        #region 修改市场价
        /// <summary>
        /// 修改市场价
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateMarketPrice(FormCollection collection)
        {
            CommodityFacade comf = new CommodityFacade();
            Guid id = new Guid(collection["Commodityid"]);
            decimal? marketPrice;
            if (!string.IsNullOrEmpty(collection["MarketPrice"]))
            {
                decimal price = 0;
                if (!decimal.TryParse(collection["MarketPrice"], out price))
                {
                    return Json(new { Result = false, Messages = "修改失败" });
                }
                marketPrice = price;
            }
            else
            {
                marketPrice = null;
            }
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.MarketPrice == marketPrice)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.MarketPrice = marketPrice;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改市场价);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdateMarketPrice(id, marketPrice);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion
        #region 修改市场价
        /// <summary>
        /// 修改进货价
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCostPrice(FormCollection collection)
        {
            CommodityFacade comf = new CommodityFacade();
            Guid id = new Guid(collection["Commodityid"]);
            decimal? costPrice;
            if (!string.IsNullOrEmpty(collection["CostPrice"]))
            {
                decimal price = 0;
                if (!decimal.TryParse(collection["CostPrice"], out price))
                {
                    return Json(new { Result = false, Messages = "修改失败" });
                }
                costPrice = price;
            }
            else
            {
                costPrice = null;
            }
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.CostPrice == costPrice)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.CostPrice = costPrice;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改进货价);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdateCostPrice(id, costPrice);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion
        #region 修改库存
        [HttpPost]
        public ActionResult UpdateStock(FormCollection collection)
        {
            Guid id = new Guid(collection["CommodityId"]);

            int commodityStock = 0;
            if (!int.TryParse(collection["CommodityStock"], out commodityStock))
            {
                return Json(new { Result = false, Messages = "商品数量数据不合法" });
            }
            CommodityFacade comf = new CommodityFacade();
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.Stock == commodityStock)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.Stock = commodityStock;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改库存);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdateStock(id, commodityStock);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion

        #region 修改销量
        [HttpPost]
        public ActionResult UpdateSalesvolume(string CommodityId, string CommoditySalesvolume)
        {
            Guid id = new Guid(CommodityId);

            int commoditySalesvolume = 0;
            if (!int.TryParse(CommoditySalesvolume, out commoditySalesvolume))
            {
                return Json(new { Result = false, Messages = "商品销量数据不合法" });
            }
            CommodityFacade comf = new CommodityFacade();

            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.Salesvolume == commoditySalesvolume)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.Salesvolume = commoditySalesvolume;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改销量);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdateSalesvolume(id, commoditySalesvolume);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion

        #region 修改商品名称
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateName(FormCollection collection)
        {
            Guid id = new Guid(collection["CommodityId"]);
            string name = collection["Commodityname"];
            CommodityFacade comf = new CommodityFacade();
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            ResultDTO result;
            if (AuditCom.IsAuditAppid(appId))
            {
                //判断是否重复提交
                AuditCommodityDTO ComInfo = AuditCom.GetApplyCommodityInfo(id, appId);
                if (ComInfo.Name == name)
                {
                    return Json(new { Result = false, Messages = "已提交等待审核" });
                }
                //需要审核的商品取出全部数据
                CommodityAndCategoryDTO commodity = comf.GetCommodity(id, appId);
                commodity.Name = name;
                commodity.Action = Convert.ToInt32(OperateTypeEnum.修改名称);
                result = AuditCom.EditAuditCommodity(commodity);
            }
            else
            {
                result = comf.UpdateName(id, name);
            }

            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion
        #region 删除商品
        [HttpPost]
        public ActionResult DeleteCommoditys(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            CommodityFacade comf = new CommodityFacade();
            string ids = "";
            ids = collection["CommodityIds"];
            string[] commodityId = ids.Split(',');

            List<Guid> idlist = new List<Guid>();
            foreach (string s in commodityId)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    idlist.Add(new Guid(s));
                }
            }
            ResultDTO result = comf.DeleteCommoditys(idlist);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "删除成功" });
            }
            return Json(new { Result = false, Messages = "删除失败" });
        }
        #endregion
        #region 上架商品
        [HttpPost]
        public ActionResult Shelves(FormCollection collection)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            CommodityFacade comf = new CommodityFacade();
            string ids = "";
            ids = collection["CommodityIds"];
            string[] commodityId = ids.Split(',');
            List<Guid> idlist = new List<Guid>();
            foreach (string s in commodityId)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    idlist.Add(new Guid(s));
                }
            }
            //判断该商铺发布商品是否需要被审核  2018-01-05添加
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            ResultDTO result = new ResultDTO();
            if (AuditCom.IsAuditAppid(appId))
            {
                foreach (var item in idlist)
                {
                    CommodityAndCategoryDTO commodity = comf.GetCommodity(item, appId);
                    commodity.State = 0;
                    commodity.Action = Convert.ToInt32(OperateTypeEnum.商品上架);
                    result = AuditCom.EditAuditCommodity(commodity);
                }
            }
            else
            {
                result = comf.Shelves(idlist);
            }
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "上架成功" });
            }
            return Json(new { Result = false, Messages = "上架失败" });
        }
        #endregion
        #region 下架商品
        [HttpPost]
        public ActionResult OffShelves(FormCollection collection)
        {
            string ids = collection["CommodityIds"];
            string[] commodityId = ids.Split(',');
            List<Guid> idlist = new List<Guid>();
            foreach (string s in commodityId)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    idlist.Add(new Guid(s));
                }
            }
            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.OffShelves(idlist);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "下架成功" });
            }
            return Json(new { Result = false, Messages = "下架失败" });
        }
        #endregion
        /// <summary>
        /// 商品列表拖动排序
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveCommoditySort(FormCollection collection)
        {
            string comIds = collection["comIds"];
            if (string.IsNullOrEmpty(comIds))
                return null;
            List<Guid> importOrderIds = new List<Guid>();
            foreach (string strOrderId in comIds.Split(','))
            {
                Guid guidOrderId = Guid.Empty;
                if (Guid.TryParse(strOrderId, out guidOrderId))
                {
                    importOrderIds.Add(guidOrderId);
                }
            }
            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.SaveCommoditySortValue(importOrderIds);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "保存成功" });
            }
            return Json(new { Result = false, Messages = "保存失败" });

        }
        /// <summary>
        /// 上移一页下移一页功能
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveUpCommoditySort(FormCollection collection)
        {
            Guid appId = WebUtil.AppId;
            Guid id = new Guid(collection["Id"]); ;
            string comIds = collection["comIds"];
            if (string.IsNullOrEmpty(comIds))
                return null;
            List<Guid> importOrderIds = new List<Guid>();
            foreach (string strOrderId in comIds.Split(','))
            {
                Guid guidOrderId = Guid.Empty;
                if (Guid.TryParse(strOrderId, out guidOrderId))
                {
                    importOrderIds.Add(guidOrderId);
                }
            }
            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.SaveUpCommoditySortValue(appId, importOrderIds, id);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "保存成功" });
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }

        [HttpPost]
        public ActionResult SaveDownCommoditySort(FormCollection collection)
        {
            Guid appId = WebUtil.AppId;
            Guid id = new Guid(collection["Id"]); ;
            string comIds = collection["comIds"];
            if (string.IsNullOrEmpty(comIds))
                return null;
            List<Guid> importOrderIds = new List<Guid>();
            foreach (string strOrderId in comIds.Split(','))
            {
                Guid guidOrderId = Guid.Empty;
                if (Guid.TryParse(strOrderId, out guidOrderId))
                {
                    importOrderIds.Add(guidOrderId);
                }
            }
            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.SaveDownCommoditySortValue(appId, importOrderIds, id);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "保存成功" });
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }
        /// <summary>
        /// 商品列表置顶
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetCommodityFirst(FormCollection collection)
        {
            string comId = collection["comId"];
            if (string.IsNullOrEmpty(comId))
                return null;

            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.SetCommodityFirst(new Guid(comId));
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "置顶成功" });
            }
            return Json(new { Result = false, Messages = "置顶失败" });

        }

        /// <summary>
        /// 此段代码完全没有必要存在，改为在客户端用js写cookie 
        /// commented by mw.2014/5/23
        /// </summary>
        /// <param name="collection"></param>
        [HttpPost]
        public void SaveCookie(FormCollection collection)
        {
            HttpCookie cookie = new HttpCookie("SaveCommodityCookie");
            cookie.Expires = DateTime.Now.AddDays(7);
            cookie["Picture"] = collection["Picture"];
            cookie["CommodityName"] = collection["CommodityName"];
            cookie["CommodityStock"] = collection["CommodityStock"];
            cookie["State"] = collection["State"];
            cookie["CommodityCode"] = collection["CommodityCode"];
            cookie["CommodityMarketPrice"] = collection["CommodityMarketPrice"];
            cookie["CommodityPrice"] = collection["CommodityPrice"];
            cookie["ImgList"] = collection["CommodityImgList"];
            cookie["listImgShowSrcString"] = collection["listImgShowSrcString"];
            cookie["CommodityDetails"] = collection["CommodityDetails"];//前端已编码
            cookie["TechSpecs"] = collection["TechSpecs"];
            cookie["SaleService"] = collection["SaleService"];
            cookie["CommoditCategory"] = collection["CommoditCategory"];
            cookie["CommoditySize"] = collection["CommoditySize"];
            cookie["CommodityColor"] = collection["CommodityColor"];
            cookie["CommoditCategoryName"] = collection["CommoditCategoryName"];
            cookie["CommoditySizeName"] = collection["CommoditySizeName"];
            cookie["CommodityColorName"] = collection["CommodityColorName"];
            cookie["hidpic"] = collection["hidpic"];

            // 20170918新增
            cookie["CommoditInnerCategory"] = collection["CommoditInnerCategory"];
            cookie["BarCode"] = collection["BarCode"];
            cookie["JDCode"] = collection["JDCode"] == "undefined" ? null : collection["JDCode"];
            cookie["CostPrice"] = collection["CostPrice"];

            cookie["IsAssurance"] = (collection["IsAssurance"] == "undefined" || string.IsNullOrWhiteSpace(collection["IsAssurance"])) ? false.ToString() : collection["IsAssurance"];
            cookie["IsReturns"] = (collection["IsReturns"] == "undefined" || string.IsNullOrWhiteSpace(collection["IsReturns"])) ? false.ToString() : collection["IsReturns"];
            cookie["Isnsupport"] = (collection["Isnsupport"] == "undefined" || string.IsNullOrWhiteSpace(collection["Isnsupport"])) ? false.ToString() : collection["Isnsupport"];
            cookie["ServiceSettingId"] = collection["ServiceSettingId"];
            if (cookie["AppId"] == null)
            {
                string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
                cookie["AppId"] = strAppId;
            }
            Response.Cookies.Add(cookie);
        }

        #region 相关商品
        [CheckAppId]
        public ActionResult RelationCommodityList(Guid CommodityId)
        {
            System.Web.HttpContext.Current.Session["CommodityId"] = CommodityId;

            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            int rowCount = 0;
            CommodityFacade comfa = new CommodityFacade();
            CategoryFacade catefa = new CategoryFacade();
            List<Guid> commodityIdList = new List<Guid>();
            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageSize = pageSize;
            search.pageIndex = 1;
            search.commodityIdList = commodityIdList;

            List<CommodityPromVM> list = comfa.RelationCommodityList(CommodityId, search);

            #region 获取商品分类列表信息
            //获取商品id数组
            List<Guid> cmdyIdList = new List<Guid>();
            foreach (var item in list)
            {
                cmdyIdList.Add(item.Id);
            }

            #endregion

            ViewBag.CategoryList = catefa.GetCategories(appId);
            ViewBag.CommodityList = list;
            ViewBag.Count = search.rowCount;

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);


            return View();
        }

        public PartialViewResult RelationPartialCommodity(string commodityName, string category, string commodityCode)
        {

            CommodityCategoryFacade cf = new CommodityCategoryFacade();
            CategoryFacade catefa = new CategoryFacade();
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();

            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            int pageIndex = 1;

            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            CommodityFacade comfa = new CommodityFacade();
            PromotionFacade pf = new PromotionFacade();
            List<Guid> commodityIdList = new List<Guid>();

            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageSize = pageSize;
            search.pageIndex = pageIndex;
            search.commodityIdList = commodityIdList;
            search.commodityName = commodityName;
            search.commodityCategory = category;
            search.commodityCode = commodityCode;

            List<CommodityPromVM> list = comfa.RelationCommodityList(new Guid(System.Web.HttpContext.Current.Session["CommodityId"].ToString()), search);

            #region 获取商品分类列表信息

            //获取商品id数组
            List<Guid> cmdyIdList = new List<Guid>();
            foreach (var item in list)
            {
                cmdyIdList.Add(item.Id);
            }

            #endregion

            ViewBag.CategoryList = catefa.GetCategories(appId);
            ViewBag.CommodityList = list;
            ViewBag.Count = search.rowCount;
            return PartialView();
        }
        #endregion

        public ActionResult AppManageIndex()
        {

            APPManageFacade appManageFacade = new APPManageFacade();

            ViewBag.appManageList = appManageFacade.GetAPPManageList();

            return View();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddAppManage(Guid appId, string appName, string remark)
        {

            APPManageFacade appManageFacade = new APPManageFacade();

            try
            {
                Deploy.CustomDTO.APPManageDTO appManageDto = new Deploy.CustomDTO.APPManageDTO();
                appManageDto.Id = Guid.NewGuid();
                appManageDto.AppId = appId;
                appManageDto.AppName = appName;
                appManageDto.Remark = remark;
                appManageDto.SubId = this.ContextDTO.LoginUserID;
                appManageDto.SubTime = DateTime.Now;
                appManageDto.ModifiedId = this.ContextDTO.LoginUserID;
                appManageDto.ModifiedOn = DateTime.Now;

                ResultDTO result = appManageFacade.AddAPPManage(appManageDto);

                if (result.ResultCode == 0)
                {
                    return Json(new { Success = true, Messages = result.Message });
                }
                else
                {
                    return Json(new { Success = false, Messages = result.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, Messages = ex.Message });
            }

        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult UpdateAppManage(Guid Id, Guid appId, string appName, string remark)
        {

            try
            {
                APPManageFacade appManageFacade = new APPManageFacade();

                Deploy.CustomDTO.APPManageDTO appManageDto = new Deploy.CustomDTO.APPManageDTO();
                appManageDto.Id = Id;
                appManageDto.AppId = appId;
                appManageDto.AppName = appName;
                appManageDto.Remark = remark;
                appManageDto.ModifiedId = ContextDTO.LoginUserID;
                appManageDto.ModifiedOn = DateTime.Now;

                ResultDTO result = appManageFacade.UpdateAPPManage(appManageDto);

                if (result.ResultCode == 0)
                {
                    return Json(new { Success = true, Messages = result.Message });
                }
                else
                {
                    return Json(new { Success = false, Messages = result.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, Messages = ex.Message });
            }

        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteAppManage(Guid Id)
        {

            APPManageFacade appManageFacade = new APPManageFacade();

            ResultDTO result = appManageFacade.DelAPPManage(Id);

            if (result.ResultCode == 0)
            {
                return Json(new { Success = true, Messages = result.Message });
            }
            else
            {
                return Json(new { Success = false, Messages = result.Message });
            }
        }
        /// <summary>
        /// 设置销售地区
        /// </summary>
        /// <param name="ids">商品Id列表</param>
        /// <param name="saleAreas">销售地区</param>
        /// <returns>结果</returns>
        public ActionResult UpdateCommodityListSaleAreas(FormCollection collection)
        {
            string ids = "";
            ids = collection["ids"];
            string[] commodityId = ids.Split(',');

            List<Guid> idlist = new List<Guid>();
            foreach (string s in commodityId)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    idlist.Add(new Guid(s));
                }
            }
            string saleAreas = collection["saleAreas"];
            CommodityFacade comf = new CommodityFacade();
            ResultDTO result = comf.UpdateCommodityListSaleAreas(idlist, saleAreas);

            if (result.ResultCode == 0)
            {
                return Json(new { Success = true, Messages = result.Message });
            }
            else
            {
                return Json(new { Success = false, Messages = result.Message });
            }
        }


        public ActionResult UpdateAppExt(Guid appId, bool isShowAddCart)
        {
            try
            {
                Jinher.AMP.BTP.Deploy.AppExtensionDTO appExt = new Jinher.AMP.BTP.Deploy.AppExtensionDTO();
                appExt.Id = appId;
                appExt.IsShowAddCart = isShowAddCart;

                AppExtensionFacade appExtFacade = new AppExtensionFacade();
                ResultDTO result = appExtFacade.UpdateAppExtension(appExt);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = string.Format("CommodityController.UpdateAppExt发生异常，异常信息：{0}", ex);
                LogHelper.Error(msg);

                ResultDTO result = new ResultDTO();
                result.Message = "服务异常，请稍后重试！";
                result.ResultCode = -1;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 推广管理
        /// </summary>
        /// <returns></returns>
        public ActionResult CommodityDividend()
        {
            Guid appId = WebUtil.AppId;
            bool? isDividendAll = null;
            decimal sharePercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommoditySharePercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isDividendAll = result.Data.IsDividendAll;
            sharePercent = result.Data.SharePercent;

            ViewBag.IsDividendAll = isDividendAll == null ? -1 : isDividendAll.Value == true ? 1 : 0;
            ViewBag.SharePercent = sharePercent;
            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            // 判断是否为易捷北京以及入驻到易捷北京的App
            var isYJBJ = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    isYJBJ = true;
                }
            }
            if (isYJBJ)
            {
                appExtensionDTO.IsCashForScore = true;
            }
            ViewBag.AppExtensionDTO = appExtensionDTO;
            if (isDividendAll == null)
            {
                var url = Request.Url.ToString();
                url = url.TrimEnd('/');
                string param = url.Substring(url.IndexOf("?"));
                return Redirect("CommodityDividendSavePage?" + param);
            }
            return View();
        }
        /// <summary>
        /// 推广管理->商品选择
        /// </summary>
        /// <returns></returns>

        [CheckAppId]
        public ActionResult DividendComSelect(GetCommodityByNameParam pdto)
        {
            Guid appId = WebUtil.AppId;
            CategoryFacade catefa = new CategoryFacade();
            ViewBag.CategoryList = catefa.GetCategories(appId);

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            return View();
        }

        [GridAction]
        public ActionResult DividendCommoditySelect(GetCommodityByNameParam pdto)
        {
            int count = 0;
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }

            CommodityFacade comfa = new CommodityFacade();

            List<CommodityListCDTO> comCDTO = new List<CommodityListCDTO>();

            ResultDTO<CommodityDividendListDTO> comList = new ResultDTO<CommodityDividendListDTO>();
            comList = comfa.GetCommodityByName(pdto);
            List<CommodityListSDTO> coms = new List<CommodityListSDTO>();
            foreach (CommodityListCDTO dto in comList.Data.CommodityList)
            {
                CommodityListSDTO com = new CommodityListSDTO();
                com.Id = dto.Id;
                com.Pic = dto.Pic;
                com.Name = dto.Name;
                com.Price = dto.Price;
                com.Stock = dto.Stock;
                coms.Add(com);
            }
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Pic");
            show.Add("Name");
            show.Add("Price");
            show.Add("Stock");
            return View(new GridModel<CommodityListSDTO>(show, coms, comList.Data.Count, pageIndex, 66365, string.Empty));
        }

        /// <summary>
        /// 推广管理编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CommodityDividendSavePage()
        {
            Guid appId = WebUtil.AppId;
            bool? isDividendAll = false;
            decimal sharePercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommoditySharePercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isDividendAll = result.Data.IsDividendAll;
            sharePercent = result.Data.SharePercent;

            ViewBag.IsDividendAll = isDividendAll == null ? -1 : isDividendAll.Value == true ? 1 : 0;

            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            // 判断是否为易捷北京以及入驻到易捷北京的App
            var isYJBJ = false;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                isYJBJ = true;
            }
            else
            {
                var mall = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).FirstOrDefault();
                if (mall != null)
                {
                    isYJBJ = true;
                }
            }
            if (isYJBJ)
            {
                appExtensionDTO.IsCashForScore = true;
            }

            ViewBag.AppExtensionDTO = appExtensionDTO;

            if (isDividendAll == null)
            {
                ViewBag.SharePercent = -1;
            }
            else if (result.Data.CShareList == null && result.Data.SharePercent == 0)
            {
                ViewBag.SharePercent = 0;
            }
            else
            {
                ViewBag.SharePercent = sharePercent;
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="OnlyShareMoney"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetCommodityDividendGrid(Guid appId, bool OnlyShareMoney)
        {
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam getCommodityByNameParam = new Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam();

            getCommodityByNameParam.AppId = appId;
            getCommodityByNameParam.OnlyShareMoney = OnlyShareMoney;
            getCommodityByNameParam.PageIndex = pNum;
            getCommodityByNameParam.PageSize = pSize;

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();

            var result = comFacade.GetCommodityByName(getCommodityByNameParam);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Pic");
            showList.Add("Name");
            showList.Add("Price");
            showList.Add("SharePercent");
            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>(showList, result.Data.CommodityList, result.Data.Count, getCommodityByNameParam.PageIndex, string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="onlyShareMoney">是否只显示设计分成商品</param>
        /// <param name="onlyScoreMoney">是否只显示设置积分抵现商品</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCommodityGridJson(Guid appId, bool onlyShareMoney = false, bool onlyScoreMoney = false)
        {
            //int pNum = 0;
            //int.TryParse(Request["page"], out pNum);

            //int pSize = 0;
            //int.TryParse(Request["rows"], out pSize);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam getCommodityByNameParam = new Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam();

            getCommodityByNameParam.AppId = appId;
            getCommodityByNameParam.OnlyShareMoney = onlyShareMoney;
            getCommodityByNameParam.OnlyScoreMoney = onlyScoreMoney;
            getCommodityByNameParam.PageIndex = 1;
            getCommodityByNameParam.PageSize = int.MaxValue;

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();

            var result = comFacade.GetCommodityByName(getCommodityByNameParam);

            return Json(result.Data.CommodityList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult SaveCommoditySharePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SaveCommoditySharePercent(dto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置商品是否参加分销
        /// </summary>
        /// <param name="commodityIdList">商品Id列表</param>
        /// <param name="isDistribute">是否分销(false：取消分销。1：参加分销)</param>
        /// <returns>结果</returns>
        public ActionResult SetCommodityDistribution(List<Guid> commodityIdList, bool isDistribute)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SetCommodityDistribution(commodityIdList, isDistribute);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置各分销商品佣金比例
        /// </summary>
        /// <param name="commodityDistributionList">佣金比例列表</param>
        /// <returns>结果</returns>
        public ActionResult SetDistributionAccount(List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDistributionDTO> commodityDistributionList)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SetDistributionAccount(commodityDistributionList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置分销商品默认佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public ActionResult SetDefaulDistributionAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SetDefaulDistributionAccount(appExtension);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取分销商品默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public ActionResult GetDefaulDistributionAccount(Guid appId)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.GetDefaulDistributionAccount(appId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 三级分销佣金设置
        /// </summary>
        /// <returns></returns>
        public ActionResult CommodityDefaultDividend()
        {
            Guid appId = new Guid(Request["appId"].ToString());
            if (this.ContextDTO.LoginOrg != Guid.Empty && this.ContextDTO.EmployeeId != Guid.Empty)
            {
                if (Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
                {
                    //三级分销佣金设置
                    bool BTPOffShelfCom = EBCSV.Instance.GetHasFeatureByCode(this.ContextDTO.EmployeeId, this.ContextDTO.LoginOrg, appId, FeatureConstant.BTPDistributeDivi);
                    if (!BTPOffShelfCom)
                    {
                        return Redirect("Error");
                    }
                }
                else
                {
                    return Redirect("Error");
                }
            }
            else
            {
                return Redirect("Error");
            }
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO result = comFacade.GetDefaulDistributionAccount(appId);
            if (result == null)
            {
                result = new Deploy.CustomDTO.AppExtensionDTO();
                result.Id = appId;
                Jinher.AMP.BTP.Deploy.CustomDTO.AppDetailAndPackageDTO appDetail = APPSV.Instance.GetAppDetailAndPackage(appId, this.ContextDTO);
                result.AppName = appDetail.Name;
            }

            ViewBag.AppExtensionDTO = result;
            return View();
        }
        /// <summary>
        /// 错误页
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// 是否存在该商品编号
        /// </summary>
        /// <param name="noCode"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult IsExistsNoCode(string noCode, Guid appId)
        {
            CommodityFacade comf = new CommodityFacade();
            bool isExists = comf.IsExists(noCode, appId);
            ResultDTO result = new ResultDTO();
            if (!isExists)
            {
                result.Message = "存在";
                result.ResultCode = 1;
            }
            else
            {
                result.Message = "不存在";
                result.ResultCode = 0;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #region 积分
        /// <summary>
        /// 积分设置
        /// </summary>
        /// <returns></returns>
        public ActionResult CommodityScore()
        {
            Guid appId = WebUtil.AppId;
            bool? isAll = null;
            decimal scorePercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommodityScorePercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isAll = result.Data.IsAll;
            scorePercent = result.Data.ScorePercent;

            ViewBag.IsAll = isAll == null ? -1 : isAll.Value == true ? 1 : 0;
            ViewBag.ScorePercent = scorePercent;
            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            ViewBag.AppExtensionDTO = appExtensionDTO;
            if (isAll == null)
            {
                var url = Request.Url.ToString();
                url = url.TrimEnd('/');
                string param = url.Substring(url.IndexOf("?"));
                return Redirect("CommodityScoreEdit?" + param);
            }
            return View();
        }
        /// <summary>
        /// 积分设置编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult CommodityScoreEdit()
        {
            Guid appId = WebUtil.AppId;
            bool? isAll = false;
            decimal scorePercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommodityScorePercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isAll = result.Data.IsAll;
            scorePercent = result.Data.ScorePercent;

            ViewBag.IsAll = isAll == null ? -1 : isAll.Value == true ? 1 : 0;

            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            ViewBag.AppExtensionDTO = appExtensionDTO;

            if (isAll == null)
            {
                ViewBag.ScorePercent = -1;
            }
            else if (result.Data.CScoreList == null && result.Data.ScorePercent == 0)
            {
                ViewBag.ScorePercent = 0;
            }
            else
            {
                ViewBag.ScorePercent = scorePercent;
            }
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="onlyScoreMoney"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetCommodityScoreGrid(Guid appId, bool onlyScoreMoney)
        {
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam getCommodityByNameParam = new Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam();

            getCommodityByNameParam.AppId = appId;
            getCommodityByNameParam.OnlyScoreMoney = onlyScoreMoney;
            getCommodityByNameParam.PageIndex = pNum;
            getCommodityByNameParam.PageSize = pSize;

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();

            var result = comFacade.GetCommodityByName(getCommodityByNameParam);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Pic");
            showList.Add("Name");
            showList.Add("Price");
            showList.Add("ScorePercent");
            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>(showList, result.Data.CommodityList, result.Data.Count, getCommodityByNameParam.PageIndex, string.Empty));



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult SaveCommodityScorePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO dto)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SaveCommodityScorePercent(dto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 选择税收编码（单选）页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SelSingleTaxRateCode()
        {
            return View();
        }

        /// <summary>
        /// 获取商品税收编码列表
        /// </summary>
        /// <param name="name">商品和服务名称</param>
        /// <returns></returns>
        [GridAction]
        public ActionResult SearchSingleCode(string name = "", double taxrate = -1.0)
        {
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            IBP.Facade.CommodityFacade facade = new IBP.Facade.CommodityFacade();
            ResultDTO<List<CommodityTaxRateCDTO>> retInfo = facade.GetSingleCommodityCode(name, taxrate, pageIndex, pageSize);

            IList<string> show = new List<string>();
            show.Add("Code");
            show.Add("Name");
            show.Add("Code");
            show.Add("TaxRate");
            int count = retInfo != null ? retInfo.ResultCode : 0;
            return View(new GridModel<CommodityTaxRateCDTO>(show, retInfo.Data, count, pageIndex, pageSize, string.Empty));
        }


        /// <summary>
        /// 获取所有的服务项设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllServiceSettings(Guid AppId)
        {
            BTP.IBP.Facade.ServiceSettingsFacade facade = new BTP.IBP.Facade.ServiceSettingsFacade();
            Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model = new BTP.Deploy.ServiceSettingsDTO();
            model.AppId = AppId;
            var result = facade.GetALLServiceSettingsList(model).OrderBy(p => p.SubTime).ToList();
            return Json(result);
        }


        /// <summary>
        /// 获取所有的服务项设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetServiceSettings(string ids)
        {
            List<Guid> servicesettings = new List<Guid>();
            string[] arr = ids.Split(new char[] { '|' });
            if (arr.Count() > 0 && arr != null)
            {
                foreach (var item in arr)
                {
                    servicesettings.Add(Guid.Parse(item));
                }
            }
            BTP.IBP.Facade.ServiceSettingsFacade facade = new BTP.IBP.Facade.ServiceSettingsFacade();
            List<BTP.Deploy.ServiceSettingsDTO> result = null;
            if (servicesettings != null && servicesettings.Count() > 0)
            {
                result = facade.GetServiceSettingsList(servicesettings).OrderBy(p => p.SubTime).ToList();
            }
            return Json(result);
        }

        /// <summary>
        /// 退货运费管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowReturnCommodityTemplate()
        {
            var facade = new Jinher.AMP.BTP.IBP.Facade.FreightFacade();

            if (!String.IsNullOrEmpty(Request["AppId"]))
            {
                var appID = Guid.Parse(Request["AppId"]);
                var list = facade.GetFreightListByAppId(appID);
                return View(list);
            }
            return View();
        }

        /// <summary>
        /// 获取退货物流信息
        /// </summary>
        /// <param name="goodName"></param>
        /// <param name="goodState"></param>
        /// <param name="ReturnState"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetReturnData(String goodName, String goodState, String ReturnState)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");
            var facade = new CommodityFacade();
            var appId = String.IsNullOrEmpty(Request["AppId"]) ? Guid.Empty : Guid.Parse(Request["AppId"]);

            if (goodState == "-1")
            {
                goodState = "";
            }

            if (ReturnState == "-1")
            {
                ReturnState = "";
            }

            var resultData = facade.GetCommodityFreightTemplate(appId, goodName, goodState, ReturnState, pageIndex, pageSize);

            var list = resultData.Data;

            //comFacade.GetCommodityVM()
            //var json = new JsonResult();

            //json.Data = list;
            //json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return json;

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("No_Code");
            showList.Add("goodName");
            showList.Add("State");
            showList.Add("FirstCountPrice");
            showList.Add("RefundFreightPrice");

            return View(new GridModel<CommodityAndTemplateDTO>(showList, list, resultData.ResultCode, pageIndex, pageSize, string.Empty));
        }

        /// <summary>
        /// 设置运费模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult SetCommodityFreightTemp(String comIds, String freightID)
        {
            var ids = comIds.Split(',');
            bool isSucess = true;
            var facade = new Jinher.AMP.BTP.IBP.Facade.CommodityFacade();

            foreach (var iditem in ids)
            {
                var resultDto = facade.SetCommodityRefoundFreightTemp(Guid.Parse(iditem), Guid.Parse(freightID));
                if (!resultDto.isSuccess)
                {
                    isSucess = false;
                }
            }

            if (isSucess)
            {
                return Content("sucess");
            }
            return Content("faile");
        }

        public ActionResult SelectCommodityList()
        {
            ViewBag.InnerCategoryList = "[]";
            ViewBag.AppId = Request["appId"];
            if (!String.IsNullOrEmpty(Request["appId"]))
            {
                ViewBag.AppId = Request["appId"];
            }

            GetCategoryLevelOne(ViewBag.AppId);
            return View();
        }

        [HttpPost]
        public JsonResult GetApp(int PageIndex, int PageSize, Guid AppId)
        {
            try
            {
                int pageIndex = 1;
                int pageSize = PageSize;
                if (PageIndex != 0)
                {
                    pageIndex = (int)PageIndex;
                }
                IList<object> objlist = new List<object>();
                var result = GetMallApplyInfoList(AppId).OrderByDescending(p => p.SubTime).ToList();
                AppManagerFacade appMFacade = new AppManagerFacade();
                List<Guid> listAppId = new List<Guid>();
                listAppId.Add(AppId);
                var _result = appMFacade.GetOldAppListByIds(listAppId);
                if (result.Count() > 0 || _result.Count() > 0)
                {
                    foreach (var item in _result)
                    {
                        objlist.Add(new
                        {
                            EsAppId = AppId,
                            EsAppName = item.AppName
                        });
                    }
                    foreach (var item in result)
                    {
                        objlist.Add(new
                        {
                            EsAppId = item.AppId,
                            EsAppName = item.AppName
                        });
                    }
                }
                int Count = objlist.Count();
                objlist = objlist.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = objlist, count = Count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(Guid appId)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> result = null;
            try
            {
                // 已审核【通过】
                //int[] arr = { 0, 1, 2, 4 };
                Jinher.AMP.BTP.IBP.Facade.MallApplyFacade userManagerFacade = new Jinher.AMP.BTP.IBP.Facade.MallApplyFacade();
                var searchDto = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO()
                {
                    EsAppId = appId
                };

                result = userManagerFacade.GetMallApplyInfoList(searchDto);
                result = result.Where(p => p.State == 2).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用Jinher.AMP.BTP.ISV.Facade.MallApplyFacade.GetSellerInfoes异常", ex);
            }
            return result;
        }


        /// <summary>
        /// 获取商品内容
        /// </summary>
        /// <param name="model"></param>
        /// <param name="appIds"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommoditys(Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashPercentDTO model, List<Guid> appIds)
        {
            var facade = new Jinher.AMP.YJB.ISV.Facade.CommodityCashPercentFacade();
            return Json(facade.GetCommodityPercentSetting(model, appIds));
            //try
            //{
            //    int pageIndex = 1;
            //    int pageSize = PageSize;
            //    if (PageIndex != 0)
            //    {
            //        pageIndex = (int)PageIndex;
            //    }
            //    CommodityFacade face = new CommodityFacade();
            //    List<CommodityListCDTO> ojblist = new List<CommodityListCDTO>();
            //    foreach (var item in CommodityList)
            //    {
            //        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityDividendListDTO> result = face.GetCommodityByName(item);
            //        if (result.Data.CommodityList.Count() > 0)
            //        {
            //            ojblist.AddRange(result.Data.CommodityList);
            //        }
            //    }
            //    int Count = ojblist.Count();
            //    ojblist = ojblist.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            //    return Json(new { data = ojblist, count = Count });
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }


        #region 获取分类
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <returns></returns>

        public void GetCategoryLevelOne(string appId)
        {
            //获取分类******
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> list = Commons.CategoryHelper.GetCategoryLevelOne(appId);
            ViewBag.CategoryList = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        }
        #endregion

    }
}
