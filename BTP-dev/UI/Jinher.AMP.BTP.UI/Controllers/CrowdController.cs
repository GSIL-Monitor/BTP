using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CrowdController : BaseController
    {
        public ActionResult Slogan(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingSlogan(appId);
            ViewBag.Slogan = result;
            return View();
        }

        public ActionResult CrowdDesc(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingDesc(appId);
            ViewBag.CrowdDesc = result;
            return View();
        }
    }
}
