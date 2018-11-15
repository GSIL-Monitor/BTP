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
    public class SupplierController : Jinher.JAP.MVC.Controller.BaseController
    {
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            SupplierFacade Supfacade = new SupplierFacade();
            var supplier = Supfacade.GetSupplierApps(appId);
            List<Guid> supplierdata = supplier.Select(p => p.Id).ToList();

            MallApplyFacade facade = new MallApplyFacade();
            var apps = facade.GetMallApps(appId);
            var appData = apps.Select(_ => new OptionModel { value = _.Id, label = _.Name, disabled = false }).ToList();
            foreach (var item in appData)
            {
                if (supplierdata.Contains(item.value))
                {
                    item.disabled = true;
                }
            }

            if (appId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !appData.Any(_ => _.value == appId))
            {
                appData.Insert(0, new OptionModel { value = appId, label = "易捷北京", disabled = false });
            }
            ViewBag.Apps = Newtonsoft.Json.JsonConvert.SerializeObject(appData);
            return View();
        }

        [HttpPost]
        public ActionResult GetData(SupplierSearchDTO dto)
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
            dto.EsAppId = appId;
            dto.UserId = ContextDTO.LoginUserID;
            SupplierFacade facade = new SupplierFacade();
            return Json(facade.GetSuppliers(dto));
        }


        [HttpPost]
        public ActionResult Add(SupplierUpdateDTO dto)
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
            dto.EsAppId = appId;
            dto.SubId = ContextDTO.LoginUserID;
            SupplierFacade facade = new SupplierFacade();
            return Json(facade.AddSupplier(dto));
        }

        [HttpPost]
        public ActionResult Update(SupplierUpdateDTO dto)
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
            dto.EsAppId = appId;
            dto.SubId = ContextDTO.LoginUserID;
            SupplierFacade facade = new SupplierFacade();
            return Json(facade.UpdateSupplier(dto));
        }

        [HttpPost]
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
            SupplierFacade facade = new SupplierFacade();
            return Json(facade.DeleteSupplier(id));
        }

        public ActionResult CheckSupplerCode(string code)
        {
            SupplierFacade facade = new SupplierFacade();
            return Json(facade.CheckSupplerCode(code), JsonRequestBehavior.AllowGet);
        }
    }
}
