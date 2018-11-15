using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Deploy;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;


namespace Jinher.AMP.BTP.UI.Controllers
{
    public class PromotionController : BaseController
    {

        #region 促销列表
        public ActionResult Index()
        {
            PromotionFacade pf = new PromotionFacade();
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            //string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
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

            int pageIndex = 1;
            int pageSize = 10;
            int rowCount = 0;
            List<DiscountsVM> list = pf.GetAllPromotion(appId, pageSize, pageIndex, null, null, null, null, null, null, out rowCount);
            ViewBag.PromotionList = list;
            ViewBag.Count = rowCount;

            return View();
        }

        [HttpPost]
        public PartialViewResult PartialIndex(string startTime, string endTime, string sintensity, string eintensity, string commodityName, string state)
        {
            PromotionFacade pf = new PromotionFacade();
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
                if (!int.TryParse(Request.QueryString["currentPage"], out pageIndex))
                {
                    pageIndex = 1;
                }
            }
            int pageSize = 10;
            int rowCount = 0;
            List<DiscountsVM> list = pf.GetAllPromotion(appId, pageSize, pageIndex, startTime, endTime, sintensity, eintensity, commodityName, state, out rowCount);
            ViewBag.PromotionList = list;
            ViewBag.Count = rowCount;
            return PartialView();
        }

        #endregion

        #region 添加促销
        public ActionResult AddPromotion()
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            PromotionCookieDTO pcDto = new PromotionCookieDTO();
            HttpCookie cookie = Request.Cookies["SavePromotionCookie"];
            if (cookie != null)
            {
                if (cookie["AppId"] != null)
                {
                    //cookie中的appid相同，展示cookie数据给用户
                    if (cookie["AppId"].ToString() == strAppId)
                    {
                        pcDto.Picture = HttpUtility.UrlDecode(cookie["Picture"]);
                        pcDto.PicName = HttpUtility.UrlDecode(cookie["PicName"]);
                        pcDto.picID = HttpUtility.UrlDecode(cookie["PicId"]);
                        pcDto.PromotionName = HttpUtility.UrlDecode(cookie["PromotionName"]);
                        pcDto.StartTime = HttpUtility.UrlDecode(cookie["PStrartTime"]);
                        pcDto.EndTime = HttpUtility.UrlDecode(cookie["PEndTime"]);
                        pcDto.IntenSity = HttpUtility.UrlDecode(cookie["IntenSity"]);
                        pcDto.CommodityIds = HttpUtility.UrlDecode(cookie["CommodityIds"]);
                        pcDto.PromotionType = "0";  //HttpUtility.UrlDecode(cookie["PromotionType"]);
                    }
                    else
                    {
                        pcDto = new PromotionCookieDTO();
                    }
                }
                else
                {
                    pcDto = new PromotionCookieDTO();
                }
                ViewBag.ProCookie = pcDto;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddPromotion(FormCollection collection)
        {
            CommodityFacade cf = new CommodityFacade();
            PromotionFacade pf = new PromotionFacade();
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["picture"], false);
            DiscountsVM discountsVM = new DiscountsVM();
            discountsVM.Name = collection["PromotionName"];
            discountsVM.PicturesPath = PictureUrl;
            discountsVM.StartTime = Convert.ToDateTime(collection["StartTime"]);
            discountsVM.EndTime = Convert.ToDateTime(collection["EndTime"]);
            discountsVM.Intensity = Convert.ToDecimal(collection["Intensity"]);
            discountsVM.DiscountPrice = Convert.ToDecimal(collection["DiscountPrice"]);
            discountsVM.PromotionType = 0;
            discountsVM.SellerId = appId;
            var aa = collection["IsAll"];

            //判断是否可添加促销
            bool res = pf.IsAddPromotion(Convert.ToDateTime(collection["StartTime"]), Convert.ToDateTime(collection["EndTime"]), appId);
            if (!res)
            {
                return Json(new { Result = false, Messages = "不能添加促销，同一时段超过5个促销！" });
            }
            if (collection["IsAll"] != null && collection["IsAll"] != "" && Convert.ToInt32(collection["IsAll"]) == 1)
            {
                discountsVM.IsAll = true;
                int commodityCount = (new CommodityFacade()).GetCommodityNum(appId, false, 0);
                if (commodityCount <= 0)
                {
                    return Json(new { Result = false, Messages = "您无上架商品！" });
                }
                //可促销商品列表
                List<Guid> commodityIdList = pf.IsCommodityCan(Convert.ToDateTime(collection["StartTime"]),
                    Convert.ToDateTime(collection["EndTime"]), appId);
                if (commodityIdList.Count() <= 0)
                {
                    return Json(new { Result = false, Messages = "所有商品均在打折期内,请更换时段" });
                }
                else
                {

                    discountsVM.CommodityIdList = commodityIdList;
                }

            }
            else
            {
                discountsVM.IsAll = false;
                List<string> commodityNo_Code = pf.GetCommodityCodeByPromotion(appId, Convert.ToDateTime(collection["StartTime"]),
                    Convert.ToDateTime(collection["EndTime"]));
                string codes = collection["CommodityIds"].Replace(';', ',').TrimEnd(',');
                string[] commodityCodes = codes.Split(',');
                List<string> code = new List<string>();
                foreach (string s in commodityCodes)
                {
                    if (!string.IsNullOrEmpty(s.Split('|')[0]))
                    {
                        code.Add(s.Split('|')[0]);
                    }
                }
                List<string> list = new List<string>();
                foreach (string s in code)
                {
                    if (commodityNo_Code.Contains(s))
                    {
                        list.Add(s);
                    }
                }
                string resultCode = string.Empty;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        resultCode = resultCode + item + ",";
                    }
                    resultCode = resultCode.TrimEnd(',');
                    return Json(new { Result = false, Messages = "" + resultCode + "商品还在促销中，请更换时间段" });
                }
                else
                {
                    discountsVM.No_Codes = code;
                    discountsVM.ComPro = commodityCodes;
                }
            }
            //全选不验证code是否存在
            if (Convert.ToInt32(collection["IsAll"]) == 1)
            {
                ResultDTO result = pf.AddPromotion(discountsVM);
                if (result.ResultCode == 0)
                {
                    //清除cookie
                    HttpCookie cookie = Request.Cookies["SavePromotionCookie"];
                    if (cookie != null)
                    {
                        Request.Cookies.Remove("SavePromotionCookie");
                        cookie.Expires = DateTime.Now.AddYears(-10);
                        Response.Cookies.Add(cookie);
                    }

                    return Json(new { Result = true, Messages = "添加成功" });
                }
                else if (result.ResultCode == 2)
                {
                    return Json(new { Result = false, Messages = result.Message });
                }
                return Json(new { Result = false, Messages = "添加失败" });
            }
            else
            {
                //判断用户手动输入的商品code是否存在
                string rest = IsInCommodityList(discountsVM.StartTime.ToString(), discountsVM.EndTime.ToString(), discountsVM.No_Codes, null);

                if (rest != "您输入的商品不在促销商品范围内，请重新输入！")
                {
                    ResultDTO result = pf.AddPromotion(discountsVM);
                    if (result.ResultCode == 0)
                    {
                        //清除cookie
                        HttpCookie cookie = Request.Cookies["SavePromotionCookie"];
                        if (cookie != null)
                        {
                            Request.Cookies.Remove("SavePromotionCookie");
                            cookie.Expires = DateTime.Now.AddYears(-10);
                            Response.Cookies.Add(cookie);
                        }

                        return Json(new { Result = true, Messages = "添加成功" });
                    }
                    else if (result.ResultCode == 2)
                    {
                        return Json(new { Result = false, Messages = result.Message });
                    }
                    return Json(new { Result = false, Messages = "添加失败" });
                }
                else
                {
                    return Json(new { Result = false, Messages = "您输入的商品不在促销商品范围内，请重新输入！" });
                }
            }
        }

        #endregion

        #region 修改促销
        public ActionResult UpdatePromotion(string promotionId)
        {
            PromotionFacade pf = new PromotionFacade();


            DiscountsVM vm = pf.GetPromotionByPromotionID(new Guid(promotionId));
            vm.Intensity = decimal.Round(vm.Intensity, 1);
            ViewBag.Promotion = vm;
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePromotion(FormCollection collection)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            PromotionFacade pf = new PromotionFacade();

            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["picture"], false);
            DiscountsVM discountsVM = new DiscountsVM();
            discountsVM.SellerId = appId;
            discountsVM.Name = collection["PromotionName"];
            discountsVM.PicturesPath = PictureUrl;
            discountsVM.StartTime = Convert.ToDateTime(collection["StartTime"]);
            discountsVM.EndTime = Convert.ToDateTime(collection["EndTime"]);
            discountsVM.Intensity = Convert.ToDecimal(collection["Intensity"]);
            discountsVM.PromotionId = new Guid(collection["PromotionId"]);
            discountsVM.DiscountPrice = Convert.ToDecimal(collection["DiscountPrice"]);
            discountsVM.PromotionType = 0; // Convert.ToInt32(collection["PromotionType"]);
            //bool res = pf.IsAddPromotion(Convert.ToDateTime(collection["StartTime"]), Convert.ToDateTime(collection["EndTime"]), appId);
            //if (!res)
            //{
            //    return Json(new { Result = false, Messages = "同一时期内不允许有5个以上促销！" });
            //}

            //全选
            if (collection["IsAll"] != null && collection["IsAll"] != "" && Convert.ToInt32(collection["IsAll"]) == 1)
            {
                DiscountsVM vm = pf.GetPromotionByPromotionID(discountsVM.PromotionId);
                if (vm != null)
                {
                    //之前不是全部商品
                    if (!(vm.IsAll.HasValue && vm.IsAll.Value))
                    {
                        List<Guid> commodityIdList = pf.IsCommodityCan(Convert.ToDateTime(collection["StartTime"]),
                            Convert.ToDateTime(collection["EndTime"]), appId);
                        if (commodityIdList.Count() <= 0)
                        {
                            return Json(new { Result = false, Messages = "所有商品均在打折期内,请更换时段！" });
                        }
                        else
                        {
                            discountsVM.CommodityIdList = commodityIdList;
                        }
                    }
                    else
                    {
                        discountsVM.CommodityIdList = new List<Guid>();
                    }
                }
                else
                {
                    return Json(new { Result = false, Messages = "修改失败" });
                }
                discountsVM.IsAll = true;


                ResultDTO result = pf.UpdatePromotion(discountsVM);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "修改成功" });
                }
                else if (result.ResultCode == 2)
                {
                    return Json(new { Result = false, Messages = result.Message });
                }
                return Json(new { Result = false, Messages = "修改失败" });
            }
            else
            {
                discountsVM.IsAll = false;
                CommodityFacade cf = new CommodityFacade();

                #region 验证前台传来的商品code中是否有正在促销的

                //获得正在促销商品的code
                List<string> commodityNo_Code = pf.GetCommodityCodeByPromotion(appId,
                    Convert.ToDateTime(collection["StartTime"]), Convert.ToDateTime(collection["EndTime"]));

                //获取前台传过来的商品code列表
                string codes = collection["CommodityIds"].Replace(';', ',').TrimEnd(',');
                string[] commodityCodes = codes.Split(',');
                List<string> code = new List<string>();
                foreach (string s in commodityCodes)
                {
                    if (!string.IsNullOrEmpty(s.Split('|')[0]))
                    {
                        code.Add(s.Split('|')[0]);
                    }
                }

                //获取前台传过来的商品中正在促销的code
                List<string> list = new List<string>();
                foreach (string s in code)
                {
                    if (commodityNo_Code.Contains(s))
                    {
                        list.Add(s);
                    }
                }

                //获得没有点击保存之前，当前promotion正在促销中的商品code
                var commodityIds = (new PromotionFacade()).GetCommodityIds(discountsVM.PromotionId);
                var comList = (new CommodityFacade()).GetCommodityCodes(appId, commodityIds);

                var selfcode = comList.ToList();
                string resultCode = string.Empty;
                //如果正在促销的商品code列表中含有本身，则去掉对其的验证

                if (Convert.ToDateTime(collection["oldEndTime"]) > DateTime.Now)
                {
                    if (selfcode != null && selfcode.Count() > 0)
                    {
                        foreach (string c in selfcode)
                        {
                            if (list.Contains(c))
                            {
                                list.Remove(c);
                            }
                        }
                    }
                }
                //前台传来的商品code中仍有正在促销的，操作失败
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        resultCode = resultCode + "," + item.TrimEnd(',');
                    }
                    return Json(new { Result = false, Messages = "" + resultCode + "商品还在促销中，请更换时间段！" });
                }
                else
                {
                    discountsVM.No_Codes = code;
                    discountsVM.ComPro = commodityCodes;
                }
                #endregion

                //判断用户手动输入的商品code是否存在，
                //编辑时，加上已有促销商品,根据promotionID去promotionItem查commodityID（）
                //var selfComID = pf.GetCommodityByPromotionID(discountsVM.PromotionId).Select(a => a.Id).ToList();
                string rest = IsInCommodityList(discountsVM.StartTime.ToString(), discountsVM.EndTime.ToString(), discountsVM.No_Codes, commodityIds);
                if (rest != "您输入的商品不在促销商品范围内，请重新输入！")
                {
                    //删除原有促销商品(修改为在更新方法里面执行)
                    //pf.DeletePromotionItems(new Guid(collection["PromotionId"]));
                    string[] r = rest.Split(',');
                    discountsVM.No_Codes = r.ToList();
                    ResultDTO result = pf.UpdatePromotion(discountsVM);
                    if (result.ResultCode == 0)
                    {
                        return Json(new { Result = true, Messages = "修改成功" });
                    }
                    else if (result.ResultCode == 2)
                    {
                        return Json(new { Result = false, Messages = result.Message });
                    }
                    return Json(new { Result = false, Messages = "修改失败" });
                }
                else
                {
                    return Json(new { Result = false, Messages = "您输入的商品不在促销商品范围内，请重新输入！" });
                }
            }
        }

        /// <summary>
        /// 更新促销到缓存
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="discount">折扣</param>
        [NonAction()]
        public void UpdateCache(Guid id, decimal discount)
        {
            //Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", id.ToString(), "BTPCache");
            //Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_DiscountInfo", id.ToString(), discount, "BTPCache");

            //LogHelper.Error("促销缓存读取：" + Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_DiscountInfo", id.ToByteArray().ToString()));
        }
        #endregion


        #region 删除促销
        public ActionResult DelPromotion(string promotionId)
        {
            PromotionFacade pf = new PromotionFacade();

            //删除缓存
            //var a = pf.GetPromotionItemsByPromotionID(new Guid(promotionId), int.MaxValue, 1);
            //foreach (Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM item in a)
            //{
            //    DeleteCache(item.CommodityId);
            //}

            ResultDTO result = pf.DelPromotion(new Guid(promotionId));
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "删除成功" });
            }
            return Json(new { Result = false, Messages = "删除失败" });

        }

        /// <summary>
        /// 删除促销缓存
        /// </summary>
        /// <param name="id">商品id</param>
        [NonAction()]
        public void DeleteCache(Guid id)
        {
            Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", id.ToString(), "BTPCache");
        }
        #endregion


        public ActionResult CommodityList(string starTime, string endTime, string promotionId)
        {
            if (starTime != null && starTime != "")
            {
                System.Web.HttpContext.Current.Session["starTime"] = starTime;
            }
            if (endTime != null && endTime != "")
            {
                System.Web.HttpContext.Current.Session["endTime"] = endTime;
            }
            if (promotionId != null && promotionId != "")
            {
                System.Web.HttpContext.Current.Session["promotionId"] = promotionId;
            }
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

            PromotionFacade pf = new PromotionFacade();
            List<Guid> commodityIdList = new List<Guid>();
            if (promotionId == null || promotionId == "" || promotionId == "00000000-0000-0000-0000-000000000000")
            {
                commodityIdList = pf.IsCommodityCan(DateTime.Parse(starTime), DateTime.Parse(endTime), appId);
            }

            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageSize = pageSize;
            search.pageIndex = 1;
            search.commodityIdList = commodityIdList;

            List<CommodityPromVM> list = comfa.GetCommodityVM(search, new Guid(promotionId));

            //获取商品分类列表信息，性能太差，弃之不用
            //foreach (var item in list)
            //{
            //    item.Categorys = comfa.GetCategoryBycommodityId(item.Id);
            //}

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
            ViewBag.StartTime = starTime;
            ViewBag.EndTime = endTime;

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            return View();
        }

        /// <summary>
        /// 获取促销对应的商品数量
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public JsonResult GetCommodityNum(Guid promotionId)
        {
            Jinher.AMP.BTP.IBP.Facade.PromotionFacade facade = new PromotionFacade();

            var result = facade.GetCommodityNum(promotionId);

            return Json(new { count = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 当全选时，返回总条数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public JsonResult GetAllCommodityListCount(string starTime, string endTime, Guid? promotionid)
        {
            if (starTime != null && starTime != "")
            {
                System.Web.HttpContext.Current.Session["starTime"] = starTime;
            }
            if (endTime != null && endTime != "")
            {
                System.Web.HttpContext.Current.Session["endTime"] = endTime;
            }

            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            CommodityFacade comfa = new CommodityFacade();
            CategoryFacade catefa = new CategoryFacade();

            PromotionFacade pf = new PromotionFacade();
            //List<Guid> commodityIdList = pf.IsCommodityCan(DateTime.Parse(starTime), DateTime.Parse(endTime), appId).Distinct().ToList();
            //int totalcount = Jinher.AMP.BTP.BE.Commodity.ObjectSet().
            //    Where(n => n.IsDel.Equals(false) && n.AppId.Equals(appId) && n.State == 0)
            //    .Select(n => n.Id).ToList().Where(n => commodityIdList.Contains(n)).Count();

            int num = pf.GetAllCommodityNum(DateTime.Parse(starTime), DateTime.Parse(endTime), appId, promotionid);

            return Json(new { count = num }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 当修改促销产品时，手动输入促销产品，判断是否在CommodityList中存在
        /// </summary>
        /// <param name="strartTime"></param>
        /// <param name="endTime"></param>
        /// <param name="commIds"></param>
        /// <returns></returns>
        public string IsInCommodityList(string starTime, string endTime, List<string> commCodes, List<Guid> comIds)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            string result = string.Empty;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            #region 原有逻辑(既然是判断输入的商品code在数据库中存不存在，为何要绕这么多圈子呢？？？？？)
            //int rowCount = 0;
            //CommodityFacade comfa = new CommodityFacade();
            //CategoryFacade catefa = new CategoryFacade();

            //PromotionFacade pf = new PromotionFacade();
            ////获取同一促销时期可以添加的商品的id
            //List<Guid> commodityIdList = pf.IsCommodityCan(DateTime.Parse(starTime), DateTime.Parse(endTime), appId).Distinct().ToList();
            //if (comIds != null && comIds.Count() > 0)
            //{
            //    foreach (Guid g in comIds)
            //    {
            //        commodityIdList.Add(g);
            //    }
            //}


            ////获取同一促销时期可以添加的商品的编号
            //List<string> codes = Jinher.AMP.BTP.BE.Commodity.ObjectSet().
            //    Where(n => n.IsDel.Equals(false) && n.AppId.Equals(appId) && n.State == 0).
            //    Select(n => new { Code = n.No_Code, Id = n.Id }).Distinct().ToList().
            //    Where(n => commodityIdList.Contains(n.Id)).Select(n => n.Code).ToList();

            //var code = string.Empty;
            //foreach (string co in commCodes)
            //{
            //    if (codes.Contains(co))
            //    {
            //        code += co + ",";
            //    }
            //}
            //if (code.Length > 0)
            //{
            //    code = code.Substring(0, code.Length - 1);
            //    result = code;
            //}
            //else
            //    result = "您输入的商品不在促销商品范围内，请重新输入！";
            #endregion

            PromotionFacade facade = new PromotionFacade();

            List<string> codes = facade.GetCodes(commCodes, appId);

            if (codes.Count > 0)
            {
                string code = string.Join(",", codes);
                return code;
            }
            else
            {
                result = "您输入的商品不在促销商品范围内，请重新输入！";
            }

            return result;
        }

        public PartialViewResult PartialCommodity(string commodityName, string category, string commodityCode, string promotionId)
        {
            CommodityCategoryFacade cf = new CommodityCategoryFacade();
            CategoryFacade catefa = new CategoryFacade();
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            promotionId = System.Web.HttpContext.Current.Session["promotionId"].ToString();
            Guid appId;
            Guid promotionID;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            int pageIndex = 1;

            if (!Guid.TryParse(promotionId, out promotionID))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            string startTime = System.Web.HttpContext.Current.Session["starTime"] != null ? System.Web.HttpContext.Current.Session["starTime"].ToString() : DateTime.Now.ToString();
            string endTime = System.Web.HttpContext.Current.Session["endTime"] != null ? System.Web.HttpContext.Current.Session["endTime"].ToString() : DateTime.Now.ToString();

            CommodityFacade comfa = new CommodityFacade();
            PromotionFacade pf = new PromotionFacade();
            List<Guid> commodityIdList = new List<Guid>();
            if (promotionId == null || promotionId == "" || promotionId == "00000000-0000-0000-0000-000000000000")
            {
                commodityIdList = pf.IsCommodityCan(DateTime.Parse(startTime), DateTime.Parse(endTime), appId);
            }

            CommoditySearchDTO search = new CommoditySearchDTO();
            search.appId = appId;
            search.pageSize = pageSize;
            search.pageIndex = pageIndex;
            search.commodityIdList = commodityIdList;
            search.commodityName = commodityName;
            search.commodityCategory = category;
            search.commodityCode = commodityCode;

            List<CommodityPromVM> list = comfa.GetCommodityVM(search, new Guid(promotionId));

            //获取商品分类列表信息，性能太差，弃之不用
            //foreach (var item in list)
            //{
            //    item.Categorys = comfa.GetCategoryBycommodityId(item.Id);
            //}

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


        public ActionResult PromotionCommodityDetail(string promotionId, string commodityName, string category, string commodityCode)
        {
            int pageSize = 20;
            int pageIndex = 1;
            if (Request["currentPage"] != null && Request["currentPage"] != "")
            {
                if (!int.TryParse(Request["currentPage"], out pageIndex))
                {
                    pageIndex = 1;
                }
            }
            if (!string.IsNullOrEmpty(Request["promotionId"]))
            {
                promotionId = Request["promotionId"];
            }
            if (!string.IsNullOrEmpty(Request["commodityName"]))
            {
                commodityName = Request["commodityName"];
            }
            if (!string.IsNullOrEmpty(Request["category"]))
            {
                category = Request["category"];
            }
            if (!string.IsNullOrEmpty(Request["commodityCode"]))
            {
                commodityCode = Request["commodityCode"];
            }
            Guid id = new Guid(promotionId);
            PromotionFacade pf = new PromotionFacade();
            int rownum = 0;
            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search = new CommoditySearchDTO();
            search.commodityName = commodityName;
            search.commodityCode = commodityCode;
            search.commodityCategory = category;
            List<PromotionItemsVM> promotionCommodityList = pf.GetPromotionItemsByPromotionID(id, search, pageSize, pageIndex, out rownum);

            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            CategoryFacade catefa = new CategoryFacade();
            ViewBag.CategoryList = catefa.GetCategories(appId);
            ViewBag.CommodityList = promotionCommodityList;
            ViewBag.promotionId = promotionId;
            ViewBag.currentPage = pageIndex;
            ViewBag.Count = rownum;

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            return View();
        }

        /// <summary>
        /// 添加cookie缓存
        /// </summary>
        /// <param name="collection"></param>
        [HttpPost]
        public void SaveCookie(FormCollection collection)
        {
            HttpCookie cookie = new HttpCookie("SavePromotionCookie");
            cookie.Expires = DateTime.Now.AddDays(7);
            cookie["Picture"] = collection["Pic"];
            cookie["PicName"] = collection["PicName"];
            cookie["PicID"] = collection["PicId"];
            cookie["PromotionName"] = collection["ProName"];
            cookie["PStrartTime"] = collection["sTime"];
            cookie["PEndTime"] = collection["eTime"];
            cookie["IntenSity"] = collection["Insity"];
            cookie["CommodityIds"] = collection["commIds"];
            Response.Cookies.Add(cookie);
        }

    }
}


