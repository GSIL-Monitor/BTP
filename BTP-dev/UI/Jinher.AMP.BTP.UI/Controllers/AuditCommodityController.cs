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


namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class AuditCommodityController : BaseController
    {
        /// <summary>
        /// 商品审核首页
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Index()
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid AppId = new Guid(strAppId);
            if (AppId == Guid.Empty)
            {
                return HttpNotFound();
            }
            //判断是否显示首页
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            if (AuditCom.IsNeedAudit(AppId))
            {
                ViewBag.IsEsAppId = true;
                ViewBag.appId = AppId;
                ViewBag.Name = "商品审核";
                return View("Index");
            }
            else
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityForYJBFacade YJBFacade = new ISV.Facade.CommodityForYJBFacade();
                var list = YJBFacade.GetMallApplyInfoList(AppId).ToList();
                if (list.Any())
                {
                    ViewBag.IsEsAppId = true;
                }
                else
                {
                    ViewBag.IsEsAppId = false;
                }
                ViewBag.appId = AppId;
                return View("ApplyCommodity");
            }
        }
        /// <summary>
        /// 商品修改记录
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult ApplyCommodity(Guid AppId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityForYJBFacade YJBFacade = new ISV.Facade.CommodityForYJBFacade();
            var list= YJBFacade.GetMallApplyInfoList(AppId).ToList();            
            if (list.Any())
            {
                ViewBag.IsEsAppId =true;
            }
            else
            {
                ViewBag.IsEsAppId =false;
            }
            ViewBag.appId = AppId;
            return View();
        }
        /// <summary>
        /// 获取商品修改记录
        /// </summary>       
        [CheckAppId]
        [GridAction]
        public ActionResult GetApplyCommodityList(Guid Appid, string Name, string CateNames, int AuditState)
        {
            try
            {
                AuditCommodityFacade AuditCom = new AuditCommodityFacade();
                CommodityAndCategoryDTO Search = new CommodityAndCategoryDTO();
                List<Guid> Appids = new List<Guid>();
                Appids.Add(Appid);
                int pageIndex = 1;
                int pageSize = 20;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                //获取数据
                ResultDTO<List<CommodityAndCategoryDTO>> retInfo = AuditCom.GetApplyCommodityList(Appids, Name, CateNames, AuditState, pageIndex, pageSize);

                List<string> ChangePriceList = new List<string>
                {
                    "AuditId",
                    "CateNames",
                    "BarCode",
                    "PicturesPath",
                    "Name",
                    "Price",
                    "CostPrice",
                    "Stock",
                    "Unit",
                    "ApplyTime",
                    "ActionName",
                    "AuditState",
                    "AuditRemark",
                    "CommodityId",
                    "AppId"
                };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityAndCategoryDTO>();
                return View(new GridModel<CommodityAndCategoryDTO>(ChangePriceList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品修改记录异常ApplyCommodity"), ex);
                return null;
            }
        }
        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [CheckAppId]
        [HttpPost]
        public ActionResult BackOut(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return null;
                }
                AuditCommodityFacade AuditCom = new AuditCommodityFacade();
                List<Guid> ids = new List<Guid>();
                ids.Add(Id);
                //调用批量审核接口
                ResultDTO result = AuditCom.AuditApply(ids, 4, "");//撤销状态为4
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "撤销成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "请稍候重试" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Messages = ex.Message });
            }

        }
        /// <summary>
        /// 审核页面
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult AuditCommodity(Guid AppId)
        {
            //只有馆可以审核
            //if (AppId == Guid.Empty || AppId != new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6"))
            //{
            //    return HttpNotFound();
            //}
            //根据商铺名称获取审核设置方式
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            bool flag = AuditCom.IsAutoModeStatus(AppId);
            ViewBag.IsAuto = flag.ToString().ToLower();
            ViewBag.EsAppId = AppId;
            ViewBag.Name = "商品审核";
            return View();
        }
        /// <summary>
        /// 获取商品审核列表
        /// </summary>       
        [CheckAppId]
        [GridAction]
        public ActionResult GetAuditCommodityList(Guid EsAppId, Guid Appid, string SupplyCode, string Name)
        {
            try
            {
                AuditCommodityFacade AuditCom = new AuditCommodityFacade();

                //根据供应商code获取店铺列表
                CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
                List<Guid> AppidByCode = ChangeFacade.GetSupplierList(EsAppId).Where(p => p.SupplierCode == SupplyCode).Select(p => p.AppId).ToList();
                if (Appid != Guid.Empty)
                {
                    AppidByCode.Add(Appid);
                }
                int pageIndex = 1;
                int pageSize = 20;
                if (!string.IsNullOrEmpty(Request["page"]))
                {
                    pageIndex = int.Parse(Request["page"]);
                }
                //获取数据
                ResultDTO<List<CommodityAndCategoryDTO>> retInfo = AuditCom.GetAuditCommodityList(EsAppId, AppidByCode, Name, 0, pageIndex, pageSize);

                List<string> ChangePriceList = new List<string>
                {
                    "AuditId",
                    "SupplyName",
                    "AppName",
                    "CateNames",
                    "BarCode",
                    "PicturesPath",
                    "Name",
                    "Price",
                    "CostPrice",
                    "Stock",
                    "Unit",
                    "ApplyTime",
                    "ActionName",
                    "AuditState",
                    "CommodityId",
                    "AppId"                     
                };
                int rowCount = retInfo != null ? retInfo.ResultCode : 0;
                var list = retInfo != null ? retInfo.Data : new List<CommodityAndCategoryDTO>();
                return View(new GridModel<CommodityAndCategoryDTO>(ChangePriceList, list, rowCount, pageIndex, pageSize, string.Empty));
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品修改记录异常ApplyCommodity"), ex);
                return null;
            }
        }
        
        /// <summary>
        ///模糊匹配商家名称
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public JsonResult GetMallAppInfo(Guid esAppId)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            var list = ChangeFacade.GetMallApplyList(esAppId).Select(p => new BigAutocomplete
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
        /// <summary>
        /// 模糊匹配供应商Code
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public JsonResult GetSupplierCode(Guid esAppId)
        {
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            var list = ChangeFacade.GetSupplierList(esAppId).Select(p => new BigAutocomplete
            {
                title = p.SupplierName,
                result = p.SupplierCode
            }).ToList();
            var keyword = Request["keyword"];
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.title.Contains(keyword)).ToList();
            }
            return Json(new { data = list });
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditApply()
        {
            try
            {
                string idlist = Request["Ids"] ?? "";
                if (string.IsNullOrEmpty(idlist))
                {
                    return null;
                }
                JArray objson = JArray.Parse(idlist);
                List<Guid> ids = new List<Guid>();
                foreach (var item in objson)
                {
                    Guid id = Guid.Parse(item["Id"].ToString());
                    ids.Add(id);
                }
                int state = Convert.ToInt32(Request["state"]);
                string AuditRemark = Request["AuditRemark"];
                if (state == 2 && string.IsNullOrEmpty(AuditRemark))
                {
                    return Json(new { Result = false, code = 1, Messages = "审核不通过时审核意见必填" });
                }
                AuditCommodityFacade AuditCom = new AuditCommodityFacade();
                //调用批量审核接口
                ResultDTO result = AuditCom.AuditApply(ids, state, AuditRemark);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "审核成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "审核失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("审核异常AuditApply"), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }

        }
        /// <summary>
        /// 设置审核方式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetAuditMode(Guid AppId, int ModeStatus)
        {
            try
            {                
                AuditCommodityFacade AuditCom = new AuditCommodityFacade();
                //调用批量审核接口
                ResultDTO result = AuditCom.SetModeStatus(AppId,ModeStatus);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, code = 0, Messages = "设置成功" });
                }
                else
                {
                    return Json(new { Result = false, code = 1, Messages = "设置失败" });
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format(" 设置审核方式SetAuditMode,AppId{0},ModeStatus{1}",AppId,ModeStatus), ex);
                return Json(new { Success = false, Messages = ex.Message });
            }

        }
        /// <summary>
        /// 申请页商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <returns></returns>        
        public ActionResult GetApplyComDetail(Guid Id, Guid commodityId, Guid appId)
        {            
            Guid ColorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Guid SizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            CategoryFacade catefa = new CategoryFacade();
            ComAttibuteFacade comaf = new ComAttibuteFacade();
            var catelist = catefa.GetCategories(appId);
            var Attributelist = comaf.GetSecondAttribute(appId);
            List<SecondAttributeDTO> size = Attributelist.Where(n => n.AttributeId == SizeId).ToList();
            List<SecondAttributeDTO> color = Attributelist.Where(n => n.AttributeId == ColorId).ToList();
            CommodityAndCategoryDTO commodity = AuditCom.GetAuditCommodity(Id,commodityId, appId);
            if (commodity != null && !string.IsNullOrEmpty(commodity.Description))
                commodity.Description = commodity.Description.Replace("\n", "").Replace("\r", "");

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
        /// <summary>
        /// 审核页商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <returns></returns>        
        public ActionResult GetAuditComDetail(Guid Id, Guid commodityId, Guid appId)
        {
            Guid ColorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Guid SizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            CategoryFacade catefa = new CategoryFacade();
            ComAttibuteFacade comaf = new ComAttibuteFacade();
            var catelist = catefa.GetCategories(appId);
            var Attributelist = comaf.GetSecondAttribute(appId);
            List<SecondAttributeDTO> size = Attributelist.Where(n => n.AttributeId == SizeId).ToList();
            List<SecondAttributeDTO> color = Attributelist.Where(n => n.AttributeId == ColorId).ToList();
            CommodityAndCategoryDTO commodity = AuditCom.GetAuditCommodity(Id, commodityId, appId);
            if (commodity != null && !string.IsNullOrEmpty(commodity.Description))
                commodity.Description = commodity.Description.Replace("\n", "").Replace("\r", "");

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
            ViewBag.AuditId = Id;//审核id           
            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowJDCode = showJDCode;
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
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <returns></returns>        
        public ActionResult UpdateAuditCom(Guid Id, Guid commodityId, Guid appId)
        {
            Guid ColorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Guid SizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            AuditCommodityFacade AuditCom = new AuditCommodityFacade();
            CategoryFacade catefa = new CategoryFacade();
            ComAttibuteFacade comaf = new ComAttibuteFacade();
            var catelist = catefa.GetCategories(appId);
            var Attributelist = comaf.GetSecondAttribute(appId);
            List<SecondAttributeDTO> size = Attributelist.Where(n => n.AttributeId == SizeId).ToList();
            List<SecondAttributeDTO> color = Attributelist.Where(n => n.AttributeId == ColorId).ToList();
            CommodityAndCategoryDTO commodity = AuditCom.GetAuditCommodity(Id, commodityId, appId);
            if (commodity != null && !string.IsNullOrEmpty(commodity.Description))
                commodity.Description = commodity.Description.Replace("\n", "").Replace("\r", "");

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
            ViewBag.AuditId = Id;//审核id  
            ViewBag.IsYJBJ = isYJBJ;
            ViewBag.ShowJDCode = showJDCode;
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

    }
}
