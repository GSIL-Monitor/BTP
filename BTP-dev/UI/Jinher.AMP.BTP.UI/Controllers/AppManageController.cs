using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.UI.Models;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class AppManageController : BaseController
    {
        private AppSetFacade appSetFacade = new AppSetFacade();

        public ActionResult AppManage()
        {
            return View();
        }

        /// <summary>
        /// 获取APP列表
        /// </summary>
        /// <param name="SeachComDTO"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetAppManageList()
        {
            string appName = Request["appName"];
            int addToAppSetStatus = int.Parse(Request["addToAppSetStatus"]);
            int PageNo = Request["page"] == null ? 0 : int.Parse(Request["page"]);
            int PageSize = Request["rows"] == null ? 0 : int.Parse(Request["rows"]);
            AppSetAppGridDTO gridDto = appSetFacade.GetAllCommodityApp(PageNo, PageSize, appName, addToAppSetStatus);
            List<AppMangerModel> models = new List<AppMangerModel>();
            foreach (AppSetAppDTO dto in gridDto.AppList)
            {
                AppMangerModel model = new AppMangerModel();
                model.AppId = dto.AppId;
                model.AppIcon = dto.AppIcon;
                model.AppName = dto.AppName;
                model.AppCreateOn = dto.AppCreateOn.ToString("yyyy-MM-dd");
                model.IsAddToAppSet = dto.IsAddToAppSet.ToString();
                models.Add(model);
            }
            IList<string> show = new List<string>();
            show.Add("AppId");
            show.Add("AppIcon");
            show.Add("AppName");
            show.Add("AppCreateOn");
            show.Add("IsAddToAppSet");
            return View(new GridModel<AppMangerModel>(show, models, gridDto.TotalAppCount, PageNo, PageSize, string.Empty));
        }

        /// <summary>
        /// 添加应用到应用组
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAppToAppSet()
        {
            Regex regex = new Regex("\\$\\$\\$\\$");
            string AppID = this.Request["AppID"];
            string AppName = this.Request["AppName"];
            string AppLocn = this.Request["AppLocn"];
            string AppCreateOn = this.Request["AppCreateOn"];
            if (string.IsNullOrEmpty(AppID) || string.IsNullOrEmpty(AppName) || string.IsNullOrEmpty(AppLocn) || string.IsNullOrEmpty(AppCreateOn))
            {
                return Json(new { Success = false, actMessage = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            string[] appIdArr = regex.Split(AppID);
            string[] appNameArr = regex.Split(AppName);
            string[] appIconArr = regex.Split(AppLocn);
            string[] appCreateOnArr = regex.Split(AppCreateOn);
            if (appIdArr == null || appIdArr.Length == 0 ||
                appNameArr == null || appNameArr.Length == 0 ||
                appIconArr == null || appIconArr.Length == 0 ||
                appCreateOnArr == null || appCreateOnArr.Length == 0)
            {
                return Json(new { Success = false, actMessage = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            List<Tuple<Guid, string, string, DateTime>> appInfoList = new List<Tuple<Guid, string, string, DateTime>>();
            for (int i = 0; i < appIdArr.Length; i++)
            {
                appInfoList.Add(new Tuple<Guid, string, string, DateTime>(Guid.Parse(appIdArr[i]), appNameArr[i], appIconArr[i], DateTime.Parse(appCreateOnArr[i])));
            }
            ResultDTO resultDTO = appSetFacade.AddAppToAppSet(appInfoList, Guid.Empty, 1);
            if (resultDTO != null && resultDTO.ResultCode == 0)
                return Json(new { Success = true, actMessage = resultDTO.Message }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = false, actMessage = "添加失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 从应用组移除应用
        /// </summary>
        /// <returns></returns>
        public ActionResult DelAppFromAppSet()
        {
            Regex regex = new Regex("\\$\\$\\$\\$");
            string AppID = this.Request["AppID"];
            if (string.IsNullOrEmpty(AppID))
            {
                return Json(new { Success = false, actMessage = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            List<Guid> appIdList = regex.Split(AppID).Select(l => Guid.Parse(l)).ToList();
            ResultDTO resultDTO = appSetFacade.DelAppFromAppSet(appIdList, Guid.Empty, 1);
            if (resultDTO != null && resultDTO.ResultCode == 0)
                return Json(new { Success = true, actMessage = resultDTO.Message }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Success = false, actMessage = "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 过滤非法应用
        /// </summary>
        /// <returns></returns>
        public ActionResult ForbitApp()
        {
            try
            {
                var ret = new APPManageFacade().ForbitApp();
                return this.Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("AppManageController-ForbitApp", ex);
                var errRet = new ResultDTO { ResultCode = -1, Message = "过滤非法应用失败" };
                return this.Json(errRet, JsonRequestBehavior.AllowGet);
            }
        }
    }
}