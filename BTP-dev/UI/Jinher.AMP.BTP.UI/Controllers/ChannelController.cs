using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class ChannelController : Controller
    {
        //
        // GET: /Channel/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChannelDefaultDividend()
        {
            Guid appId = new Guid(Request["appId"].ToString());
            Jinher.AMP.BTP.IBP.Facade.AppExtensionFacade appFacade = new AppExtensionFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO result = appFacade.GetDefaulChannelAccount(appId);
            ViewBag.AppExtensionDTO = result;
            return View();
        }

        public ActionResult SetDefaultChannelAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            Jinher.AMP.BTP.IBP.Facade.AppExtensionFacade appFacade = new AppExtensionFacade();
            var result = appFacade.SetDefaultChannelAccount(appExtension);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
