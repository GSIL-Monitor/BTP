using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.Common.Loging;
using System.Web.Script.Serialization;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 门店信息 手机端web页
    /// </summary>
    public class StorefrontController : Controller
    {
        /// <summary>
        /// 门店列表
        /// </summary>
        /// <returns></returns>
        [Filters.CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        public ActionResult Index(Guid appId)
        {
            Guid esAppId = appId;
            try
            {
                ISV.Facade.StoreFacade storeFacade = new ISV.Facade.StoreFacade();
                ResultDTO<StoreSDTO> result = storeFacade.GetOnlyStoreInApp(esAppId);
                if (result.ResultCode == 3)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string storeJsonString = js.Serialize(result.Data);
                    storeJsonString = HttpUtility.UrlEncode(storeJsonString);
                    ViewBag.StoreDTOJson = storeJsonString;

                    return View("~/Views/Storefront/Map.cshtml");
                }
                var menu = BACSV.Instance.GetAppSingleMenuInfo(esAppId, "StoreInfo");
                ViewBag.Title = menu != null ? menu.Name : "";
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用ISV.Facade.StoreFacade.GetOnlyStoreInApp异常，异常信息：", ex);
            }
            return View();
        }
        /// <summary>
        /// 获取门店信息列表。
        /// </summary>
        /// <param name="slp"></param>
        /// <returns></returns>
        public JsonResult GetStoreByLocation(StoreLocationParam slp)
        {
            NStoreSDTO stores = null;
            try
            {
                ISV.Facade.StoreFacade storeFacade = new ISV.Facade.StoreFacade();
                stores = storeFacade.GetStoreByLocation(slp);
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用ISV.Facade.StoreFacade.GetStoreByLocation异常，异常信息：", ex);
            }
            if (stores == null)
            {
                stores = new NStoreSDTO();
            }
            return Json(stores);
        }
        [DealMobileUrl]
        public ActionResult Map()
        {
            return View();
        }
    }
}
