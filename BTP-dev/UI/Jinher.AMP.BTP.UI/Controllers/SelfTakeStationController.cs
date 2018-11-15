using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Models;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.Common;
using System;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.BTP.Deploy;
using System.IO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class SelfTakeStationController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;
                if (ChangeOrg == Guid.Empty)
                {
                    return Redirect("Error");
                }


                Jinher.AMP.ZPH.Deploy.CustomDTO.QueryProxyPrarm prarm = new ZPH.Deploy.CustomDTO.QueryProxyPrarm();
                prarm.changeOrg = ChangeOrg;
                Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyContentCDTO proxyContent = new ZPH.Deploy.CustomDTO.ProxyContentCDTO();
                Jinher.AMP.ZPH.Deploy.CustomDTO.ReturnInfo<List<Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyContentCDTO>> result = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAllProxyList4BTP(prarm);
                proxyContent = result.Data[0];

                Jinher.AMP.BTP.Deploy.CustomDTO.ZPHProxyContentCDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ZPHProxyContentCDTO();

                model.changeOrg = proxyContent.changeOrg;
                model.cityCode = proxyContent.cityCode;
                model.id = proxyContent.id;
                model.iwAccount = proxyContent.iwAccount;
                model.provinceCode = proxyContent.provinceCode;
                model.proxyName = proxyContent.proxyName;
                model.AppId = proxyContent.belongTo ?? Guid.Empty;

                ViewBag.ProxyContent = model;

                return View();
            }
            catch (Exception ex)
            {
                return Redirect("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        [GridAction]
        public ActionResult GetAllSelfTakeStationGrid(SelfTakeStationSearchSDTO selfTakeStationSearchSDto)
        {
            int rowCount = 0;
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            selfTakeStationSearchSDto.pageIndex = pNum;
            selfTakeStationSearchSDto.pageSize = pSize;

            Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;
            selfTakeStationSearchSDto.CityOwnerId = ChangeOrg;

            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();

            var result = stsFacade.GetAllSelfTakeStation(selfTakeStationSearchSDto, out rowCount);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Name");
            showList.Add("CityOwnerName");
            showList.Add("AddressDetail");
            showList.Add("SpreadUrl");
            showList.Add("QRCodeUrl");

            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult>(showList, result, rowCount, selfTakeStationSearchSDto.pageIndex, string.Empty));
        }

        public ActionResult SaveSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            if (string.IsNullOrWhiteSpace(selfTakeStationDTO.QRCodeUrl))
            {
                string qrCodeurl = Jinher.AMP.BTP.UI.Commons.QRCodeHelper.GenerateImgTwoCode("", selfTakeStationDTO.SpreadUrl, 1);
                selfTakeStationDTO.QRCodeUrl = qrCodeurl;
            }

            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
            var result = stsFacade.SaveSelfTakeStation(selfTakeStationDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
            var result = stsFacade.UpdateSelfTakeStation(selfTakeStationDTO);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSelfTakeStations(Guid id)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
            List<Guid> ids = new List<Guid>() { id };
            var result = stsFacade.DeleteSelfTakeStations(ids);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelfTakeStationById(Guid id)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
            var result = stsFacade.GetSelfTakeStationById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckUserIDByUserCode(string userCode)
        {
            string userId = string.Empty;
            CBC.Deploy.CustomDTO.UserBasicInfoDTO info = CBCSV.Instance.GetUserBasicInfo(userCode);
            if (info != null && info.UserId != Guid.Empty)
            {
                userId = info.UserId.ToString();
                Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
                var checkResult = stsFacade.CheckSelfTakeStationManagerByUserId(info.UserId);
                if (checkResult.ResultCode == 0)
                {
                    //不存在
                    var result = new
                    {
                        UserId = userId,
                        ResultCode = 0,
                        Message = "可以填加"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //不存在或参数错误
                    var result = new
                    {
                        UserId = userId,
                        ResultCode = 1,
                        Message = checkResult.Message
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            //不存在或参数错误
            var dResult = new
            {
                UserId = Guid.Empty,
                ResultCode = 1,
                Message = "该账号尚未注册，请及时注册，以免影响正常使用！"
            };

            return Json(dResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得推广地址
        /// </summary>
        /// <returns></returns>
        public ActionResult GenareteSpreadUrlAndCode(Guid appId)
        {
            Guid speader = Guid.NewGuid();

            //string longUrl = Jinher.AMP.BTP.Common.CustomConfig.ZPHUrl + "zph?source=share&speader=" + speader + "&ProductType=webcjzy&type=tuwen&SrcType=34&isshowsharebenefitbtn=1&cityagent=" + cityagent;
            string longUrl = Jinher.AMP.BTP.Common.CustomConfig.BtpDomain + "Spread/Index?speader=" + speader + "&appId=" + appId;
            var shortUrl = ShortUrlSV.Instance.GenShortUrl(longUrl);
            var result = new
                     {
                         SpreadUrl = shortUrl,
                         SpreadCode = speader
                     };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSelfTakeStationManagerById(Guid id)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();
            var result = stsFacade.DeleteSelfTakeStationManagerById(new List<Guid>() { id });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得二维码图
        /// </summary>
        /// <param name="fileImg">临时图片名称，可以空</param>
        /// <param name="replaceUrl">要构建二维码的推广码的短地址</param>
        /// <returns></returns>
        public ActionResult GenareteQRCode(string fileImg, string replaceUrl)
        {
            string result = Jinher.AMP.BTP.UI.Commons.QRCodeHelper.GenerateImgTwoCode(fileImg, replaceUrl, 2);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProvince()
        {
            var provinceDtos = CBCBP.Instance.GeProvinceByCountryCode();
            provinceDtos.Add(new Area() { Code = "", Name = "请选择" });
            return Json(provinceDtos, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取市列表
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        public ActionResult PartialProvince(string provinceCode)
        {
            List<Area> cityList = new List<Area>();
            if (!string.IsNullOrWhiteSpace(provinceCode))
            {
                cityList = CBCBP.Instance.GetCityByProvinceCode(provinceCode);
            }
            cityList.Add(new Area() { Code = "", Name = "请选择" });
            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取区列表
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public ActionResult PartialCity(string cityCode)
        {
            List<Area> countyDtos = new List<Area>();
            if (string.IsNullOrWhiteSpace(cityCode))
            {
                countyDtos = CBCBP.Instance.GetCountyByCityCode(cityCode);
            }
            countyDtos.Add(new Area() { Code = "", Name = "请选择" });
            return Json(countyDtos, JsonRequestBehavior.AllowGet);
        }

        #region 电商馆

        public ActionResult ECIndex()
        {
            try
            {
                string appId = Request.QueryString["appId"];

                if (!string.IsNullOrEmpty(appId))
                {
                    System.Web.HttpContext.Current.Session["EcAppId"] = appId;
                }

                Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;

                Jinher.AMP.BTP.Deploy.CustomDTO.ZPHECDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ZPHECDTO();

                model.changeOrg = ChangeOrg;
                model.cityCode = "230200";
                model.id = new Guid("E5F0B848-1F7D-4C7B-B8BD-24786E8A0C59");
                model.iwAccount = "Windows2015";
                model.provinceCode = "230000";
                model.proxyName = "无和";

                ViewBag.ProxyContent = model;

                return View();
            }
            catch (Exception ex)
            {
                return Redirect("ECError");
            }
        }

        public ActionResult ECError()
        {
            return View();
        }
        private Guid getECAppId()
        {
            Guid appId;
            Guid.TryParse(System.Web.HttpContext.Current.Session["EcAppId"].ToString(), out appId);
            return appId;
        }
        [GridAction]
        public ActionResult GetAllSelfTakeStationGridEC(SelfTakeStationSearchSDTO selfTakeStationSearchSDto)
        {
            int rowCount = 0;
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            selfTakeStationSearchSDto.pageIndex = pNum;
            selfTakeStationSearchSDto.pageSize = pSize;

            Guid ChangeOrg = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginOrg;
            selfTakeStationSearchSDto.CityOwnerId = ChangeOrg;

            selfTakeStationSearchSDto.AppId = getECAppId();

            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade stsFacade = new IBP.Facade.SelfTakeStationFacade();

            var result = stsFacade.GetAllSelfTakeStation(selfTakeStationSearchSDto, out rowCount);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Name");
            showList.Add("AddressDetail");
            showList.Add("SpreadUrl");
            showList.Add("QRCodeUrl");

            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult>(showList, result, rowCount, selfTakeStationSearchSDto.pageIndex, string.Empty));
        }

        /// <summary>
        /// 获得推广地址 电商馆
        /// </summary>
        /// <returns></returns>
        public ActionResult GenareteSpreadUrlAndCodeEC(string cityagent)
        {
            Guid speader = Guid.NewGuid();
            //string longUrl = Jinher.AMP.BTP.Common.CustomConfig.BacEUrl + "/AppPage/TemplatePage/BusinessActive.html?speader=" + speader + "&apptype=4&esappid=" + getECAppId() + "&srctype=49&linkmall=1&source=share&isshowsharebenefitbtn=1";
            string longUrl = Jinher.AMP.BTP.Common.CustomConfig.BtpDomain + "Spread/Index?speader=" + speader + "&appId=" + getECAppId();
            var shortUrl = ShortUrlSV.Instance.GenShortUrl(longUrl);
            var result = new
            {
                SpreadUrl = shortUrl,
                SpreadCode = speader
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion



        #region App自提点

        public ActionResult AppIndex()
        {
            return View();
        }

        public ActionResult AppError()
        {
            return View();
        }

        [GridAction]
        public ActionResult GetAppSelfTakeStationGrid(AppSelfTakeStationSearchSDTO search)
        {
            int rowCount = 0;
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            search.PageIndex = pNum;
            search.PageSize = pSize;

            Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();

            var result = facade.GetAppSelfTakeStationList(search);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Name");
            showList.Add("AddressDetail");
            showList.Add("Phone");

            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO>(showList, result.Data, result.Count, search.PageIndex, string.Empty));
        }

        public ActionResult SaveAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();
            var result = facade.SaveAppSelfTakeStation(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();
            var result = facade.UpdateAppSelfTakeStation(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAppSelfTakeStations(Guid id)
        {
            Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();
            List<Guid> ids = new List<Guid>() { id };
            var result = facade.DeleteAppSelfTakeStations(ids);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAppSelfTakeStationById(Guid id)
        {
            Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();
            var result = facade.GetAppSelfTakeStationById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckUserIdExists(string userCode, Guid appId)
        {
            string userId = string.Empty;
            CBC.Deploy.CustomDTO.UserBasicInfoDTO info = CBCSV.Instance.GetUserBasicInfo(userCode);
            if (info != null && info.UserId != Guid.Empty)
            {
                userId = info.UserId.ToString();
                Jinher.AMP.BTP.IBP.Facade.AppSelfTakeStationFacade facade = new IBP.Facade.AppSelfTakeStationFacade();
                var checkResult = facade.CheckUserIdExists(info.UserId, appId);
                if (checkResult.ResultCode == 0)
                {
                    //不存在
                    var result = new
                    {
                        UserId = userId,
                        ResultCode = 0,
                        Message = "可以填加"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //不存在或参数错误
                    var result = new
                    {
                        UserId = userId,
                        ResultCode = 1,
                        Message = checkResult.Message
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            //不存在或参数错误
            var dResult = new
            {
                UserId = Guid.Empty,
                ResultCode = 1,
                Message = "该账号尚未注册，请及时注册，以免影响正常使用！"
            };

            return Json(dResult, JsonRequestBehavior.AllowGet);
        }


        #endregion
        [CheckAppId]
        public ActionResult SelfTakeList()
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade sf = new SelfTakeStationFacade();
            Guid appId = WebUtil.AppId;
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int rowCount = 0;
            List<AppSelfTakeStationResultDTO> list = sf.GetAllAppSelfTakeStation(appId, pageSize, pageIndex, out rowCount);
            list = list.ToList();
            ViewBag.AppSelfStationList = list;
            ViewBag.Count = rowCount;
            ViewBag.AppId = WebUtil.AppId;
            return View();
        }

        public PartialViewResult PartialIndex(string Name, string province, string city, string district)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade sf = new SelfTakeStationFacade();
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
            List<AppSelfTakeStationResultDTO> list = sf.GetAllAppSelfTakeStationByWhere(appId, pageSize, pageIndex,
                                                                                      out  rowCount, Name, province, city,
                                                                                        district);
            ViewBag.AppSelfStationList = list;
            ViewBag.Count = rowCount;
            ViewBag.lastIndex = (pageIndex - 1) * pageSize;
            return PartialView();
        }
        public ActionResult DelAppSelfStation(string appStationId)
        {
            Jinher.AMP.BTP.IBP.Facade.SelfTakeStationFacade sf = new SelfTakeStationFacade();
            ResultDTO res = sf.DeleteAppSelfTakeStation(new Guid(appStationId));
            if (res.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "删除成功" });
            }
            return Json(new { Result = false, Messages = "删除失败" });
        }
    }
}
