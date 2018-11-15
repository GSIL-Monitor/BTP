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
using System.Web.Script.Serialization;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class StoreController : Controller
    {

        #region 门店列表
        public ActionResult Index()
        {
            StoreFacade sf = new StoreFacade();
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            //string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            if (appId != null)
            {
                System.Web.HttpContext.Current.Session["APPID"] = appId;
            }

            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int rowCount = 0;
            List<StoreDTO> list = sf.GetAllStore(appId, pageSize, pageIndex, out rowCount);

            list = list.ToList();
            ViewBag.StoreList = list;
            ViewBag.Count = rowCount;
            return View();
        }

        [HttpPost]
        public PartialViewResult PartialIndex(string storeName, string province, string city, string district)
        {
            StoreFacade sf = new StoreFacade();
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int rowCount = 0;
            List<StoreDTO> list = sf.GetAllStoreByWhere(appId, pageSize, pageIndex, out rowCount, storeName, province, city, district);

            ViewBag.StoreList = list;
            ViewBag.Count = rowCount;
            ViewBag.lastIndex = (pageIndex - 1) * pageSize;
            return PartialView();
        }


        #endregion
        #region 添加门店
        public ActionResult AddStore()
        {
            ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();
            return View();
        }

        [HttpPost]
        public ActionResult AddStore(StoreDTO storeDTO)
        {

            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }

            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["picture"], false);
            StoreFacade sf = new StoreFacade();
            storeDTO.Id = Guid.NewGuid();
            storeDTO.AppId = appId;
            storeDTO.picture = PictureUrl;

            ResultDTO res = sf.AddStore(storeDTO);

            if (res.ResultCode == 0)
            {

                return Json(new { Result = true, Messages = "添加成功" });
            }
            return Json(new { Result = false, Messages = "添加失败" });
        }
        #endregion



        #region 修改门店
        public ActionResult UpdateStore(Guid storeId)
        {
            ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();
            StoreFacade sf = new StoreFacade();

            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            StoreDTO storeDTO = sf.GetStoreDTO(storeId, appId);
            ViewBag.StoreDTO = storeDTO;


            JavaScriptSerializer js = new JavaScriptSerializer();
            string storeJsonString = js.Serialize(storeDTO);
            storeJsonString = HttpUtility.UrlEncode(storeJsonString);
            ViewBag.StoreDTOJson = storeJsonString;

            return View();
        }

        [HttpPost]
        public ActionResult UpdateStore(StoreDTO storeDTO)
        {
            StoreFacade sf = new StoreFacade();
            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["picture"], false);
            storeDTO.picture = PictureUrl;

            ResultDTO res = sf.UpdateStore(storeDTO);
            if (res.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "修改成功" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }
        #endregion

        #region 删除门店
        public ActionResult DelStore(string storeId)
        {
            StoreFacade sf = new StoreFacade();
            ResultDTO res = sf.DelStore(new Guid(storeId));
            if (res.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "删除成功" });
            }
            return Json(new { Result = false, Messages = "删除失败" });
        }
        #endregion


        [HttpPost]
        [Obsolete("已废弃", false)]
        public PartialViewResult PartialProvince(string provinceCode, string selectedCityCode)
        {
            List<Area> cityList = new List<Area>();
            if (!string.IsNullOrWhiteSpace(provinceCode))
            {
                cityList = CBCBP.Instance.GetCityByProvinceCode(provinceCode);
            }
            cityList.Add(new Area() { Code = "", Name = "请选择" });
            ViewBag.tempList = cityList;
            ViewBag.tempCode = selectedCityCode;
            return PartialView();
        }

        [HttpPost]
        [Obsolete("已废弃", false)]
        public PartialViewResult PartialCity(string cityCode, string selectCountyCode)
        {
            List<Area> countyDtos = new List<Area>();
            if (!string.IsNullOrWhiteSpace(cityCode))
            {
                countyDtos = CBCBP.Instance.GetCountyByCityCode(cityCode);

            }
            countyDtos.Add(new Area() { Code = "", Name = "请选择" });
            ViewBag.tempList = countyDtos;
            ViewBag.tempCode = selectCountyCode;
            return PartialView();
        }

        /// <summary>
        /// 在地图上选择(标记)门店位置
        /// </summary>
        /// <returns></returns>
        public ActionResult SignPositionMap()
        {
            ViewBag.MapAddress = this.Request["address"];
            return View();
        }
    }
}
