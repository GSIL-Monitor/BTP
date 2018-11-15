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
using Jinher.AMP.BTP.UI.Models;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class StoreController : Controller
    {
        #region 门店列表
        public ActionResult CYIndex()
        {
            return this.Index();
        }

        [HttpPost]
        public PartialViewResult CYPartialIndex(string storeName, string province, string city, string district)
        {
            return this.PartialIndex(storeName, province, city, district);
        }


        //public ActionResult SetStore()
        //{
        //    ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();
        //    return View();
        //}

        #endregion
        #region 添加门店
        public ActionResult CYAddStore()
        {
            ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();
            return View();
        }

        [HttpPost]
        public ActionResult CYAddStoreSetting(StoreVM storeDTO)
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

            JavaScriptSerializer seri = new JavaScriptSerializer();
            seri.MaxJsonLength = int.MaxValue;
            var settingvm = seri.Deserialize<StoreSettingVM>(storeDTO.Setting);

            var sdt = DateTime.Now;
            var edt = DateTime.Now;
            FCYSettingCDTO settingDTO = new FCYSettingCDTO()
            {
                CateringSetting = new FCateringSettingCDTO()
                {
                    AppId = appId,
                    DeliveryAmount = settingvm.DeliveryAmount,
                    DeliveryFee = settingvm.DeliveryFee,
                    DeliveryRange = settingvm.DeliveryRange,
                    Id = Guid.NewGuid(),
                    MostCoupon = int.MaxValue,
                    StoreId = storeDTO.Id,
                    Specification = "",
                    Unit = "",
                    DeliveryFeeDiscount = settingvm.DeliveryFeeDiscount,
                    DeliveryFeeEndT = DateTime.TryParse(settingvm.DeliveryFeeEndT, out edt)?edt:default(DateTime?),
                    DeliveryFeeStartT = DateTime.TryParse(settingvm.DeliveryFeeStartT, out sdt) ? sdt : default(DateTime?),
                    FreeAmount = settingvm.FreeAmount
                },
                CYBusinessHours = new List<FCateringBusinessHoursCDTO>()
            };

            DateTime dt = DateTime.Now;
            settingvm.WorkTimes.ForEach(r =>
            {
                settingDTO.CYBusinessHours.Add(new FCateringBusinessHoursCDTO()
                {
                    openingTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", dt.Year, dt.Month, dt.Day, r.stime.hour, r.stime.min)),
                    closingTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", dt.Year, dt.Month, dt.Day, r.etime.hour, r.etime.min)),
                });
            });

            ResultDTO res = sf.AddStore(storeDTO);

            if (res.ResultCode == 0)
            {
                var ressetting = new CateringSettingFacade().AddCateringSetting(settingDTO);
                if (ressetting.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "添加成功" });
                }
            }
            return Json(new { Result = false, Messages = "添加失败" });
        }
        #endregion



        #region 修改门店
        public ActionResult CYUpdateStore(Guid storeId)
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
        public ActionResult CYUpdateStore(StoreVM storeDTO)
        {
            StoreFacade sf = new StoreFacade();
            Helper helper = new Helper();
            string PictureUrl = helper.UploadADPic(Request.Form["picture"], false);
            storeDTO.picture = PictureUrl;

            JavaScriptSerializer seri = new JavaScriptSerializer();
            seri.MaxJsonLength = int.MaxValue;
            var settingvm = seri.Deserialize<StoreSettingVM>(storeDTO.Setting);
            var sdt = DateTime.Now;
            var edt = DateTime.Now;
            FCYSettingCDTO settingDTO = new FCYSettingCDTO()
            {
                CateringSetting = new FCateringSettingCDTO()
                {
                    AppId = Guid.Parse( settingvm.AppId),
                    DeliveryAmount = settingvm.DeliveryAmount,
                    DeliveryFee = settingvm.DeliveryFee,
                    DeliveryRange = settingvm.DeliveryRange,
                    Id =  Guid.Parse(settingvm.Id),
                    MostCoupon = int.MaxValue,
                    StoreId =  Guid.Parse(settingvm.StoreId),
                    Specification="",
                    Unit="",
                    DeliveryFeeDiscount = settingvm.DeliveryFeeDiscount,
                    DeliveryFeeEndT = DateTime.TryParse(settingvm.DeliveryFeeEndT, out edt) ? edt : default(DateTime?),
                    DeliveryFeeStartT = DateTime.TryParse(settingvm.DeliveryFeeStartT, out sdt) ? sdt : default(DateTime?),
                    FreeAmount = settingvm.FreeAmount
                },
                CYBusinessHours = new List<FCateringBusinessHoursCDTO>()
            };

            DateTime dt = DateTime.Now;
            settingvm.WorkTimes.ForEach(r =>
            {
                settingDTO.CYBusinessHours.Add(new FCateringBusinessHoursCDTO()
                {
                    openingTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", dt.Year, dt.Month, dt.Day, r.stime.hour, r.stime.min)),
                    closingTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", dt.Year, dt.Month, dt.Day, r.etime.hour, r.etime.min)),
                });
            });


            ResultDTO res = sf.UpdateStore(storeDTO);
            if (res.ResultCode == 0)
            {
                var ressetting = new CateringSettingFacade().UpdateCateringSetting(settingDTO);
                if (ressetting.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "修改成功" });
                }
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }

        public ActionResult GetStoreSetting(Guid storeId)
        {
            try
            {
                var setting = new CateringSettingFacade().GetCateringSetting(storeId);
                return Json(new { Result = true, setting = setting });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false });
            }

        }


        #endregion

        #region 删除门店
        public ActionResult CYDelStore(string storeId)
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
    }
}
