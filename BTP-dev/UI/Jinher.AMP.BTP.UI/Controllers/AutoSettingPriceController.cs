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
    public class AutoSettingPriceController : Jinher.JAP.MVC.Controller.BaseController
    {
        public ActionResult Index(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }

            ViewBag.AppId = appId;
            //ViewBag.Apps = Newtonsoft.Json.JsonConvert.SerializeObject(appData);
            return View();
        }

        [HttpPost]
        public ActionResult GetData(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new CommodityPriceFloatFacade();
            var appIds = facade.GetApps(appId);
            MallApplyFacade mallFacade = new MallApplyFacade();
            var apps = mallFacade.GetMallApps(appId);
            var appData = apps.Select(_ => new OptionModel { value = _.Id, label = _.Name, disabled = false }).Distinct(OptionModelComparer.Instance).OrderBy(_=>_.label).ToList();
            foreach (var item in appData)
            {
                if (appIds.Contains(item.value))
                {
                    item.disabled = true;
                }
            }
            if (appId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !appData.Any(_ => _.value == appId))
            {
                appData.Insert(0, new OptionModel { value = appId, label = "易捷北京", disabled = false });
            }
            var data = facade.GetDataList(appId);
            data.Data.Apps = appData;
            return Json(data);
        }

        [HttpPost]
        public ActionResult Add(CommodityPriceFloatDTO dto)
        {
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.SubId = ContextDTO.LoginUserID;
            CommodityPriceFloatFacade facade = new CommodityPriceFloatFacade();
            return Json(facade.Add(dto));
        }

        [HttpPost]
        public ActionResult Update(CommodityPriceFloatDTO dto)
        {
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.SubId = ContextDTO.LoginUserID;
            CommodityPriceFloatFacade facade = new CommodityPriceFloatFacade();
            return Json(facade.Update(dto));
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            CommodityPriceFloatFacade facade = new CommodityPriceFloatFacade();
            return Json(facade.Delete(id));
        }
    }
}
