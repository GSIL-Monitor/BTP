using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.UIJquery.DataGrid;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        #region 查询类目
        [CheckAppId]
        public ActionResult Index()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            CategoryFacade catef = new CategoryFacade();
            List<CategorySDTO> catelist = catef.GetCategories(appId);
            if (!catelist.Any())
            {
                catef.CreatCategory2(appId);
                catelist = catef.GetCategories(appId);
            }

            SecondAttributeFacade com = new SecondAttributeFacade();
            //获取属性列表 
            List<ColorAndSizeAttributeVM> attributeList = com.GetAttributeByAppID(appId);
            ViewBag.AttributeList = attributeList;

            ViewBag.catelist = catelist;

            //是否显示搜索菜单
            //bool isShowSearchMenu = false;
            //var searchResult = catef.CheckIsShowSearchMenu(new CategorySearchDTO { AppId = appId });
            //if (searchResult != null && searchResult.ResultCode == 0)
            //{
            //    isShowSearchMenu = searchResult.Data;
            //}
            ViewBag.isShowSearchMenu = true;  //isShowSearchMenu;
            return View();
        }
        [CheckAppId]
        public ActionResult EsNetIndex()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            CategoryFacade catef = new CategoryFacade();
            List<CategoryS2DTO> catelist = catef.GetCategories2(appId);
            if (!catelist.Any())
            {
                catef.CreatCategory2(appId);
                catelist = catef.GetCategories2(appId);
            }

            SecondAttributeFacade com = new SecondAttributeFacade();
            //获取属性列表 
            List<ColorAndSizeAttributeVM> attributeList = com.GetAttributeByAppID(appId);
            ViewBag.AttributeList = attributeList;

            ViewBag.catelist = catelist;

            //查询是否是定制电商
            bool isCustomEC = false;
            isCustomEC = APPBP.IsFittedApp(appId);

            //是否显示搜索菜单
            bool isShowSearchMenu = false;
            //是否后后设置了
            var searchResult = catef.CheckIsShowSearchMenu(new CategorySearchDTO { AppId = appId });
            if (searchResult != null && searchResult.ResultCode == 0)
            {
                isShowSearchMenu = searchResult.Data;
            }
            //校验是否支持点餐列表功能，点餐app只支持一级分类
            bool isFoodApp = false;
            if (isCustomEC)
            {
                isFoodApp = BACBP.CheckFoodOderList(appId);
            }

            ViewBag.isCustomEC = isCustomEC;
            ViewBag.isShowSearchMenu = isShowSearchMenu;
            ViewBag.isFoodApp = isFoodApp;

            return View();
        }
        public PartialViewResult PartialIndex(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            CategoryFacade catef = new CategoryFacade();
            List<CategorySDTO> catelist = catef.GetCategories(appId);
            ViewBag.catelist = catelist;
            return PartialView();
        }

        /// <summary>
        /// 专题活动
        /// </summary>
        /// 
        /// <returns></returns>
        public ActionResult ThematicActivities()
        {
            return PartialView();
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetThematicList(string ssName, string ActName, DateTime StartTime, DateTime EndTime, int Pagesize, int page)
        {
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("ActName");
            show.Add("SsName");
            show.Add("StartTime");
            show.Add("EndTime");

            try
            {
                Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
                //Guid appId = new Guid("1375ad99-de3b-4e93-80d5-5b96e1588967");
                Jinher.AMP.ZPH.ISV.Facade.SpecialSubjectFacade fade = new ZPH.ISV.Facade.SpecialSubjectFacade();
                var list = fade.GetActivityByPage(appId, ssName, ActName, StartTime, EndTime, Pagesize, page);
                int rowCount = list.Code;
                var gridobj = new GridModel<Jinher.AMP.ZPH.Deploy.CustomDTO.GetActivityByPage>(show, list.Data, rowCount, page, Pagesize, string.Empty);
                return View(gridobj);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取专题活动GetThematicList：{0}出错", System.Web.HttpContext.Current.Session["APPID"].ToString()), ex);
            }

            var gridobjNew = new GridModel<Jinher.AMP.ZPH.Deploy.CustomDTO.GetActivityByPage>(show, null, 0, page, Pagesize, string.Empty);
            return View(gridobjNew);

        }

        /// <summary>
        /// 品牌墙
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult BrandWall()
        {
            if (!String.IsNullOrEmpty(Request["AppId"]))
            {
                ViewBag.AppId = Request["AppId"];
            }
            else
            {
                ViewBag.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId; //取易捷的APPID
            }
            return View();
        }

        [GridAction]
        public ActionResult GetBrandList(String categoryName)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            CategoryFacade catef = new CategoryFacade();
            var catelist = catef.GetCategories(appId);
            // catelist.Where(o => o.CurrentLevel == 1);  //获取一级分类的类目
            var facade = new CategoryInnerBrandFacade();

            if (!String.IsNullOrEmpty(categoryName))
            {
                catelist = catelist.Where(o => o.Name.Contains(categoryName)).ToList();
            }

            foreach (var item in catelist)
            {
                var brand = facade.GetBrandWallList(item.Id);

                foreach (var tm in brand.Data)
                {
                    if (!String.IsNullOrEmpty(tm.Brandname))
                    {
                        item.PicturesPath += tm.Brandname + ",";
                    }
                }
                item.PicturesPath = !String.IsNullOrEmpty(item.PicturesPath) ? item.PicturesPath.Trim(',') : "";  //暂时用PicturesPath存储品牌名称
            }

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Name");
            showList.Add("PicturesPath");

            return View(new GridModel<Deploy.CustomDTO.CategorySDTO>(showList, catelist, catelist.Count(), 1, 20, string.Empty));

            //var listDto = from n in catelist
            //              select new
            //              {
            //                  Id = n.Id,
            //                  CategoryName = n.Name,
            //                  Brand = facade.GetBrandWallList(n.Id).Data,//品牌墙
            //              };


        }

        /// <summary>
        /// 添加分类品牌
        /// </summary>
        /// <param name="brandDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCategoryInnerBrand(FormCollection brandDTO)
        {
            //string strAppId = Request.QueryString["appId"];
            //if (string.IsNullOrEmpty(strAppId))
            //{
            //    strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            //}
            //Guid appId;
            //if (!Guid.TryParse(strAppId, out appId))
            //{
            //    Response.StatusCode = 404;
            //    return null;
            //}
            //if (appId != null)
            //{
            //    System.Web.HttpContext.Current.Session["APPID"] = appId;
            //}
            CategoryInnerBrandFacade cf = new CategoryInnerBrandFacade();

            ResultDTO delRes = cf.DeleteCateBrandsByCategoryId(Guid.Parse(brandDTO["CategoryId"]));
            if (!delRes.isSuccess)
            {
                return Json(new { Result = false, Messages = "删除原有分类品牌失败" });
            }
            CategoryInnerBrandDTO cd = new CategoryInnerBrandDTO();
            List<CategoryInnerBrandDTO> list = new List<CategoryInnerBrandDTO>();
            string Ids = brandDTO["BrandIdList"];
            string Names = brandDTO["BrandNameList"];
            Guid CategoryId = Guid.Parse(brandDTO["CategoryId"]);
            string CategoryName = brandDTO["CategoryName"];
            string[] IdList = Ids.Split(',');
            string[] NameList = Names.Split(',');
            for (int i = 0; i < IdList.Length - 1; i++)
            {
                cd = new CategoryInnerBrandDTO();
                cd.CategoryId = CategoryId;
                cd.CategoryName = CategoryName;
                cd.BrandId = Guid.Parse(IdList[i]);
                cd.Brandname = NameList[i].ToString();
                cd.SubTime = DateTime.Now;
                cd.ModifiedOn = cd.SubTime;
                list.Add(cd);
            }
            ResultDTO res = cf.AddList(list);
            if (res.isSuccess)
            {
                return Json(new { Result = true, Messages = "保存成功" });
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }

        [GridAction]
        public ActionResult GetCategoryAdvertise(String AdvertiseName, int state, Guid CategoryId)
        {
            var facade = new CategoryAdvertiseFacade();

            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");
            int rowCount = 0;
            var list = facade.CateGoryAdvertiseList(AdvertiseName, state, CategoryId, pageIndex, pageSize, out rowCount);

            foreach (var item in list.Data)
            {
                if (item.PutTime < DateTime.Now && item.PushTime > DateTime.Now)
                {
                    item.State = 1;
                }
                else if (item.PushTime < DateTime.Now)
                {
                    item.State = 2;
                }
                else if (item.PutTime > DateTime.Now)
                {
                    item.State = 0;
                }
            }

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("AdvertiseName");
            showList.Add("PutTime");
            showList.Add("PushTime");
            showList.Add("State");
            showList.Add("spreadEnum");

            return View(new GridModel<CategoryAdvertiseDTO>(showList, list.Data, rowCount, pageIndex, pageSize, string.Empty));
        }


        #endregion
        #region 添加类目
        [HttpPost]
        public String AddCategory(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.AddCategory(categoryname, appId, targetId);
            return result.Message.ToString();
        }
        [HttpPost]
        public String AddChildCategory(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.AddChildCategory(categoryname, targetId, appId);

            return result.Message.ToString();
        }
        [HttpPost]
        public ActionResult AddChildCategory2(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            string icon = collection["icon"];
            string isuse = collection["isuse"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.AddChildCategory2(categoryname, targetId, appId, icon, Convert.ToInt32(isuse));
            return Json(result);

        }
        #endregion
        [HttpPost]
        public ActionResult DeleteCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryId = collection["categoryId"];
            CategoryFacade category = new CategoryFacade();
            try
            {
                ResultDTO result = category.DeleteCategory(appId, new Guid(categoryId));
                if (result.ResultCode == 0)
                {

                    return Json(new { Result = true, Messages = "删除成功" });
                }
                else
                {
                    return Json(new { Result = false, Messages = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Messages = "删除失败" });
            }
        }

        [HttpPost]
        public ActionResult UpCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            CategoryFacade category = new CategoryFacade();
            try
            {
                category.UpCategory(appId, new Guid(targetId), new Guid(myId));
                return Json(new { Result = true, Messages = "升序成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Messages = "升序失败" });
            }
        }
        [HttpPost]
        public ActionResult UpdateCategory2(FormCollection collection)
        {
            Guid myId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            string icon = collection["icon"];
            string isuse = collection["isuse"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.UpdateCategory2(appId, categoryname, myId, icon, Convert.ToInt32(isuse));
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "更新成功！" });
            }
            else
            {
                return Json(new { Result = false, Messages = result.Message });
            }
        }
        [HttpPost]
        public ActionResult DownCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            CategoryFacade category = new CategoryFacade();
            try
            {
                category.DownCategory(appId, new Guid(targetId), new Guid(myId));
                return Json(new { Result = true, Messages = "降序成功" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Messages = "降序失败" });
            }
        }
        [HttpPost]
        public ActionResult UpdateCategory(FormCollection collection)
        {
            Guid myId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.UpdateCategory(appId, categoryname, myId);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "更新成功" });
            }
            else
            {
                return Json(new { Result = false, Messages = "更新失败" });
            }
        }
        [HttpPost]
        public ActionResult UpCategoryLevel(FormCollection collection)
        {

            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.LevelUpCategory(appId, new Guid(targetId), new Guid(myId));
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "升级成功" });
            }
            else
            {
                return Json(new { Result = false, Messages = "升级失败" });
            }
        }
        [HttpPost]
        public ActionResult DownCategoryLevel(FormCollection collection)
        {

            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.LevelDownCategory(appId, new Guid(targetId), new Guid(myId));
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "降级成功" });
            }
            else
            {
                return Json(new { Result = false, Messages = "降级失败" });
            }
        }
        [HttpPost]
        public ActionResult DragCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            string type = collection["movetype"];
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.DragCategory(appId, new Guid(targetId), new Guid(myId), type);
            if (result.ResultCode == 1)
            {
                return Json(new { Result = false, Messages = "参数为空" });
            }
            if (result.ResultCode == 2)
            {
                return Json(new { Result = false, Messages = "拖动失败，类别不存在，请重新操作" });
            }
            if (result.ResultCode == 3)
            {
                return Json(new { Result = false, Messages = "拖动失败，被拖拽类目的子节点不存在，请重新操作" });
            }
            if (result.ResultCode == 4)
            {
                return Json(new { Result = false, Messages = "拖动失败，操作会导致节点丢失，请重新操作" });
            }
            if (result.ResultCode == 5)
            {
                return Json(new { Result = false, Messages = "拖动失败，不能超过三级节点，请重新操作" });
            }
            else
            {
                return Json(new { Result = true, Messages = "拖动成功" });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateIsShowSearchMenu(bool isChecked)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            CategoryFacade com = new CategoryFacade();
            ResultDTO result = com.UpdateIsShowSearchMenu(new CategorySearchDTO { AppId = appId, IsShowSearchMenu = isChecked });
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "更新成功" });
            }
            else
            {
                return Json(new { Result = false, Messages = "更新失败" });
            }

        }

        public ActionResult FileUpload()
        {
            if (!String.IsNullOrEmpty(Request["AppId"]))
            {
                ViewBag.AppId = Request["AppId"];
            }
            else
            {
                ViewBag.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId; //取易捷的APPID
            }

            if (!String.IsNullOrEmpty(Request["CategoryID"]))
            {
                ViewBag.CategoryID = Request["CategoryID"];
            }
            else
            {
                ViewBag.CategoryID = "";
            }

            if (!String.IsNullOrEmpty(Request["AdvertiseID"]))
            {
                var bp = new CategoryAdvertiseFacade();
                var advertise = bp.GetCategoryAdvertise(Guid.Parse(Request["AdvertiseID"]));
                if (advertise.isSuccess)
                {
                    return View(advertise.Data);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddAdvertiseMent(String AdvertiseID, String CategoryId, String adName, String adImg, String adVideo, String bgDate, String edDate, String LinkId, String AdverID, String UserServiceName, String LinkUrl)
        {
            CategoryAdvertiseDTO cat;
            var bp = new CategoryAdvertiseFacade();
            if (AdvertiseID != "")
            {
                cat = bp.GetCategoryAdvertise(Guid.Parse(AdvertiseID)).Data;

                cat.AdvertiseImg = adImg;
                cat.AdvertiseMedia = adVideo;
                cat.AdvertiseName = adName;
                cat.CategoryId = Guid.Parse(CategoryId);
                cat.FreeUrl = LinkUrl;
                cat.PutTime = DateTime.Parse(bgDate);
                cat.PushTime = DateTime.Parse(edDate);
                cat.spreadEnum = int.Parse(LinkId);
                cat.UserService = UserServiceName;
                //cat.AdverID = AdverID;

                if (cat.AdvertiseType == 0 && adVideo != "")
                {
                    cat.AdvertiseType = 1;
                }
                else
                {
                    cat.AdvertiseType = 0;
                }
                //编辑
            }
            else
            {   //添加
                cat = new CategoryAdvertiseDTO
                {
                    AdvertiseImg = adImg,
                    AdvertiseMedia = adVideo,
                    AdvertiseName = adName,
                    //AdvertiseType = int.Parse(LinkId),
                    CategoryId = Guid.Parse(CategoryId),
                    FreeUrl = LinkUrl,
                    PutTime = DateTime.Parse(bgDate),
                    PushTime = DateTime.Parse(edDate),
                    spreadEnum = int.Parse(LinkId),
                    UserService = UserServiceName
                };
                if (adImg != "")
                {
                    cat.AdvertiseType = 0;
                }
                else
                {
                    cat.AdvertiseType = 1;
                }
            }

            if (AdverID != "-1")
            {
                cat.AdverID = Guid.Parse(AdverID);
            }
            else
            {
                cat.AdverID = Guid.Empty;
            }

            ResultDTO dto;

            if (AdvertiseID != "")
            {
                dto = bp.EditCategoryAdvertise(cat);
            }
            else
            {
                dto = bp.CreateCategoryAdvertise(cat);
            }


            if (dto.isSuccess)
            {
                return Json(new { Result = true, Messages = "保存/编辑成功" });
            }
            else
            {
                return Json(new { Result = false, Messages = dto.Message });
            }

        }

        [HttpPost]
        public JsonResult DeleteAdvertise(Guid AdvertiseID)
        {
            if (AdvertiseID != null && AdvertiseID != Guid.Empty)
            {
                var bp = new CategoryAdvertiseFacade();
                var rto = bp.DeleteCategoryAdvertise(AdvertiseID);
                if (rto.isSuccess)
                {
                    return Json(new { Result = true, Messages = "删除成功" });
                }
                else
                {
                    return Json(new { Result = false, Messages = "删除失败" });
                }
            }
            else
            {
                return Json(new { Result = false, Messages = "参数不正确" });
            }
        }

        public JsonResult EditAdvertise(Guid AdvertiseID, String adName, String adImg, String adVideo, String bgDate, String edDate, String LinkId, String AdverID, String UserServiceName, String LinkUrl)
        {
            return Json(new { Result = false, Messages = "参数不正确" });
        }

        /// <summary>
        /// 同步类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InitCateogry(Guid appId)
        {
            var fasade = new CategoryFacade();
            var result = fasade.InitAppCategory(appId); //同步分类
            return Json(result);
        }
    }
}
