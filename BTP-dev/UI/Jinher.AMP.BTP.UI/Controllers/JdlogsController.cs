using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class JdlogsController : BaseController
    {
        public ActionResult Index()
        {
            string appId = Request["appId"];
            ViewBag.appId = appId;
            if (appId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId.ToString())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Empty", "Jdlogs");
            }
        }

        public ActionResult Empty()
        {
            return View();
        }

        /// <summary>
        /// 京东大客户异常日志
        /// </summary>
        /// <returns></returns>
        public ActionResult JdIndex()
        {
            return View();
        }

        /// <summary>
        /// 网易严选异常日志
        /// </summary>
        /// <returns></returns>
        public ActionResult YxIndex()
        {
            return View();
        }

        /// <summary>
        /// 方正电商异常日志
        /// </summary>
        /// <returns></returns>
        public ActionResult FzIndex()
        {
            return View();
        }

        /// <summary>
        /// 苏宁异常日志
        /// </summary>
        /// <returns></returns>
        public ActionResult SnIndex()
        {
            return View();
        }

        /// <summary>
        /// 查询京东日志列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllJdlogs(Jinher.AMP.BTP.Deploy.CustomDTO.JdlogsDTO search)
        {
            try
            {
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                JdlogsFacade facade = new JdlogsFacade();
                var result = facade.GetALLJdlogsList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询实体内容
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetJdlogs(Guid id)
        {
            try
            {
                JdlogsFacade facade = new JdlogsFacade();
                var result = facade.GetJdlogs(id);
                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改京东日志信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            ResultDTO result = null;
            try
            {
                JdlogsFacade facade = new JdlogsFacade();
                result = facade.UpdateJdlogs(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }

        /// <summary>
        /// 删除京东日志信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteJdlogs(Guid id)
        {
            ResultDTO result = null;
            try
            {
                JdlogsFacade facade = new JdlogsFacade();
                result = facade.DeleteJdlogs(id);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }
    }
}
