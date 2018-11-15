using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 赠品活动
    /// </summary>
    public class PresentPromotionController : BaseController
    {
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.AppId = appId;
            return View();
        }

        [HttpPost]
        public ActionResult GetData(PresentPromotionSearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new PresentPromotionFacade();
            return Json(facade.GetPromotions(dto));
        }

        public ActionResult Details(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.AppId = appId;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GetDeatils(Guid id)
        {
            var facade = new PresentPromotionFacade();
            return Json(facade.GetPromotionDetails(id), JsonRequestBehavior.AllowGet);
        }

        [CheckAppId]
        public ActionResult Create()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            GetCategoryLevelOne(appId.ToString());
            return View();
        }

        [HttpPost]
        public ActionResult Create(PresentPromotionCreateDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new PresentPromotionFacade();
            return Json(facade.CreatePromotion(dto));
        }

        [CheckAppId]
        public ActionResult Update(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.AppId = appId;
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public ActionResult Update(PresentPromotionCreateDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new PresentPromotionFacade();
            return Json(facade.UpdatePromotion(dto));
        }


        public ActionResult Delete(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new PresentPromotionFacade();
            return Json(facade.DeletePromotion(id));
        }

        public ActionResult EndActivity(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new PresentPromotionFacade();
            return Json(facade.EndPromotion(id));
        }

        public ActionResult GetCommodities(PresentPromotionCommoditySearchDTO input)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            //}
            input.AppId = appId;
            var facade = new PresentPromotionFacade();
            return Json(facade.GetCommodities(input));
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

            ViewBag.Apps = Newtonsoft.Json.JsonConvert.SerializeObject(list);

        }
        #endregion
    }
}
