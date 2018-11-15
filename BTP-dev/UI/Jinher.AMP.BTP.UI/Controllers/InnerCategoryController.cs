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

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class InnerCategoryController : Controller
    {
        #region 查询类目
        [CheckAppId]
        public ActionResult Index()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            InnerCategoryFacade catef = new InnerCategoryFacade();
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
            bool isShowSearchMenu = false;
            var searchResult = catef.CheckIsShowSearchMenu(new CategorySearchDTO { AppId = appId });
            if (searchResult != null && searchResult.ResultCode == 0)
            {
                isShowSearchMenu = searchResult.Data;
            }
            ViewBag.isShowSearchMenu = isShowSearchMenu;
            return View();
        }
        [CheckAppId]
        public ActionResult EsNetIndex()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            InnerCategoryFacade catef = new InnerCategoryFacade();
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
            //if (isCustomEC)
            //{
            //    isFoodApp = BACBP.CheckFoodOderList(appId);
            //}

            ViewBag.isCustomEC = isCustomEC;
            ViewBag.isShowSearchMenu = isShowSearchMenu;
            ViewBag.isFoodApp = isFoodApp;

            return View();
        }
        public PartialViewResult PartialIndex(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            InnerCategoryFacade catef = new InnerCategoryFacade();
            List<CategorySDTO> catelist = catef.GetCategories(appId);
            ViewBag.catelist = catelist;
            return PartialView();
        }
        #endregion

        #region 添加类目
        [HttpPost]
        public String AddCategory(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            InnerCategoryFacade com = new InnerCategoryFacade();
            ResultDTO result = com.AddCategory(categoryname, appId, targetId);
            return result.Message.ToString();
        }
        [HttpPost]
        public String AddChildCategory(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            InnerCategoryFacade com = new InnerCategoryFacade();
            ResultDTO result = com.AddChildCategory(categoryname, targetId, appId);

            return result.Message.ToString();
        }
        [HttpPost]
        public ActionResult AddChildCategory2(FormCollection collection)
        {
            Guid targetId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            string code = collection["code"];
            InnerCategoryFacade com = new InnerCategoryFacade();
            ResultDTO result = com.AddChildCategory2(categoryname, targetId, appId, code);
            return Json(result);

        }
        #endregion

        #region 更新类目
        [HttpPost]
        public ActionResult UpdateCategory(FormCollection collection)
        {
            Guid myId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            InnerCategoryFacade com = new InnerCategoryFacade();
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
        public ActionResult UpdateCategory2(FormCollection collection)
        {
            Guid myId = new Guid(collection["categoryId"]);
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryname = collection["name"];
            string code = collection["code"];
            InnerCategoryFacade com = new InnerCategoryFacade();
            ResultDTO result = com.UpdateCategory2(appId, categoryname, myId, code);
            if (result.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "更新成功！" });
            }
            else
            {
                return Json(new { Result = false, Messages = result.Message });
            }
        }
        #endregion

        // 删除类目
        [HttpPost]
        public ActionResult DeleteCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string categoryId = collection["categoryId"];
            InnerCategoryFacade category = new InnerCategoryFacade();
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

        #region 移动类目
        [HttpPost]
        public ActionResult UpCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            InnerCategoryFacade category = new InnerCategoryFacade();
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
        public ActionResult UpCategoryLevel(FormCollection collection)
        {

            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            InnerCategoryFacade com = new InnerCategoryFacade();
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
        public ActionResult DownCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            InnerCategoryFacade category = new InnerCategoryFacade();
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
        public ActionResult DownCategoryLevel(FormCollection collection)
        {

            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            InnerCategoryFacade com = new InnerCategoryFacade();
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

        // 拖拽类目
        [HttpPost]
        public ActionResult DragCategory(FormCollection collection)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            string targetId = collection["targetId"];
            string myId = collection["categoryId"];
            string type = collection["movetype"];
            InnerCategoryFacade com = new InnerCategoryFacade();
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
        #endregion

        [HttpPost]
        public ActionResult UpdateIsShowSearchMenu(bool isChecked)
        {
            Guid appId = new Guid(System.Web.HttpContext.Current.Session["APPID"].ToString());
            InnerCategoryFacade com = new InnerCategoryFacade();
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
    }
}
