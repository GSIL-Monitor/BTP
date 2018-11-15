using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class InsuranceCompanyController : Controller
    {
        //
        // GET: /InsuranceCompany/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetInsuranceInfo()
        {

            return Json(new object());
        }
    }
}
