using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        [CheckAppId]
        public ActionResult AddPayment()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            PaymentFacade paymentFacade = new PaymentFacade();
            List<PaymentsForEditDTO> allPaymentDTOList = paymentFacade.GetAllPayment(appId);
            ViewBag.allPaymentDTOList = allPaymentDTOList;
            ViewBag.IsMyIntegral = false;
            ViewBag.AppScoreSetting = null;
            if (BACBP.CheckMyIntegral(appId))
            {
                ViewBag.IsMyIntegral = true;
                AppExtensionFacade appExtensionFacade = new AppExtensionFacade();
                var appScoreSetting = appExtensionFacade.GetScoreSetting(appId);
                ViewBag.AppScoreSetting = appScoreSetting;
                var  isEnableCancelScore= paymentFacade.IsEnableCancelScore(appId);
                ViewBag.IsEnableCancelScore = isEnableCancelScore;
            }

            return View();
        }

        [HttpPost]
        [CheckAppId]
        public ActionResult AddPayment(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            DateTime dt = DateTime.Now;
            PaymentFacade paymentFacade = new PaymentFacade();

            string[] paymentIds = collection.GetValues("Checkbox");
            PaymentsVM paymentsVM = new PaymentsVM();
            paymentsVM.Code = dt.ToFileTime().ToString();
            paymentsVM.AppId = appId;
            paymentsVM.IsOnuse = true;
            paymentsVM.PaymentIds = paymentIds;
            paymentsVM.AliPayPartnerId = collection["AliPayPartnerId"];
            paymentsVM.AliPaySeller = collection["AliPaySeller"];
            paymentsVM.AliPayPrivateKey = collection["AliPayPrivateKey"];
            paymentsVM.AliPayPublicKey = collection["AliPayPublicKey"];
            paymentsVM.AliPayVerifyCode = collection["AliPayVerifyCode"];

            bool isMyIntegral;
            bool.TryParse(collection["IsMyIntegral"], out isMyIntegral);
            ViewBag.IsMyIntegral = isMyIntegral;
            ViewBag.AppScoreSetting = null;

            if (isMyIntegral)
            {
                paymentsVM.AppScoreSetting = new AppScoreSettingDTO();
                var isCashForScore = collection.GetValues("IsCashForScore");
                if (isCashForScore != null && isCashForScore.Any())
                {
                    paymentsVM.AppScoreSetting.IsCashForScore = true;
                    int scoreCost;
                    int.TryParse(collection["ScoreCost"], out scoreCost);
                    paymentsVM.AppScoreSetting.ScoreCost = scoreCost;
                    if (paymentsVM.AppScoreSetting.IsCashForScore && CustomConfig.ScoreCostList.All(c => c != paymentsVM.AppScoreSetting.ScoreCost))
                    {
                        ViewBag.Result = 2;
                        var isEnableCancelScore = paymentFacade.IsEnableCancelScore(appId);
                        ViewBag.IsEnableCancelScore = isEnableCancelScore;
                        return View();
                    }
                }


            }

            ViewBag.IsMyIntegral = isMyIntegral;
            ViewBag.AppScoreSetting = null;
            ResultDTO result = paymentFacade.UpdatePayment(paymentsVM);
            ViewBag.Result = result.ResultCode;

            List<PaymentsForEditDTO> allPaymentDTOList = paymentFacade.GetAllPayment(appId);
            ViewBag.allPaymentDTOList = allPaymentDTOList;
            if (isMyIntegral)
            {
                AppExtensionFacade appExtensionFacade = new AppExtensionFacade();
                var appScoreSetting = appExtensionFacade.GetScoreSetting(appId);
                ViewBag.AppScoreSetting = appScoreSetting;
                var isEnableCancelScore = paymentFacade.IsEnableCancelScore(appId);
                ViewBag.IsEnableCancelScore = isEnableCancelScore;
            }
            return View();
        }

    }
}
