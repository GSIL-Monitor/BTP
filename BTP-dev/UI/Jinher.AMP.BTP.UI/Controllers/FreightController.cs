using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;


namespace Jinher.AMP.BTP.UI.Controllers
{
    public class FreightController : BaseController
    {
        private readonly FreightFacade _freightFacade;

        public FreightController()
        {
            _freightFacade = new FreightFacade();
        }

        //
        // GET: /Freight/

        public ActionResult Index()
        {
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
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

            ViewBag.AppId = appId;

            FreightFacade fFacade = new FreightFacade();
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                int.TryParse(Request.QueryString["currentPage"], out pageIndex);
            }
            int rowCount = 0;
            List<FreightDTO> list = fFacade.GetFreightTemplateListByAppId(appId, pageIndex, pageSize, out rowCount);
            ViewBag.Count = rowCount;
            ViewBag.FreightList = list;
            return View();
        }
        [HttpPost]
        public PartialViewResult PartialIndex()
        {
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

            FreightFacade fFacade = new FreightFacade();
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                int.TryParse(Request.QueryString["currentPage"], out pageIndex);
            }
            int rowCount = 0;
            List<FreightDTO> list = fFacade.GetFreightTemplateListByAppId(appId, pageIndex, pageSize, out rowCount);
            ViewBag.Count = rowCount;
            ViewBag.FreightList = list;
            return PartialView();
        }
        public ActionResult AddFreight()
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;

            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();
            ViewBag.AppId = appId;
            return View();
        }
        public ActionResult UpdateFreight()
        {
            ViewBag.ProvinceList = CBCBP.Instance.GeProvinceByCountryCode();

            Guid id = Guid.Parse(Request["Id"]);
            FreightFacade fFacade = new FreightFacade();
            FreightDTO freight = fFacade.GetOneFreight(id);
            ViewBag.Freight = freight;
            return View();
        }

        /// <summary>
        /// 已废弃。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectProvinces()
        {
            List<string> selectedProvincesList = new List<string>();
            string selectedProvinces = Request.Form["SelectedProvince"];
            if (!string.IsNullOrEmpty(selectedProvinces))
            {
                string[] selPro = selectedProvinces.Substring(1).Split(',');
                if (selPro != null && selPro.Length > 0)
                {
                    selectedProvincesList = selPro.ToList<string>();
                }
            }
            //选择区域
            List<string> selectProvinceList = new List<string>();
            string selectProvinces = Request.Form["SelectProvince"];
            if (!string.IsNullOrEmpty(selectProvinces))
            {
                selectProvinceList = selectProvinces.Substring(1).Split(',').ToList<string>();
            }

            string returnStr = "";
            if (selectProvinceList != null && selectProvinceList.Count > 0)
            {
                foreach (string province in selectProvinceList)
                {
                    if (!selectedProvincesList.Contains(province))
                    {
                        returnStr = returnStr + "," + province;
                    }
                }
            }
            if (string.IsNullOrEmpty(returnStr))
            {
                return Json(new { Result = false, Messages = "筛选失败" });
            }
            else
            {
                return Json(new { Result = true, Messages = returnStr.Substring(1) });
            }
        }

        [HttpPost]
        public ActionResult AddFreightTemplate()
        {
            try
            {
                Guid appId = Guid.Parse(Request.Form["AppId"]);
                string name = Request.Form["Name"];
                bool isFreeExp = int.Parse(Request.Form["IsFreeExp"]) == 1 ? true : false;
                int freightMethod = int.Parse(Request.Form["FreightMethod"]);
                int firstCount = int.Parse(Request.Form["FirstCount"]);
                decimal firstCountPrice = decimal.Parse(Request.Form["FirstCountPrice"]);
                int nextCount = int.Parse(Request.Form["NextCount"]);
                decimal nextCountPrice = decimal.Parse(Request.Form["NextCountPrice"]);
                FreightFacade fFacade = new FreightFacade();
                FreightTemplateDTO ftem = new FreightTemplateDTO();
                ftem.AppId = appId;
                ftem.Name = name;
                ftem.IsFreeExp = isFreeExp;
                ftem.FreightMethod = freightMethod;
                ftem.FreightTo = "";
                ftem.FirstCount = firstCount;
                ftem.FirstCountPrice = firstCountPrice;
                ftem.NextCount = nextCount;
                ftem.NextCountPrice = nextCountPrice;

                string freightDetail = Request.Form["FreightDetailList"];
                List<FreightTemplateDetailDTO> detailList = new List<FreightTemplateDetailDTO>();
                if (!string.IsNullOrEmpty(freightDetail))
                {
                    detailList = JsonHelper.JsonDeserialize<List<FreightTemplateDetailDTO>>(freightDetail);
                }
                ResultDTO result = fFacade.AddFreightAndFreightDetail(ftem, detailList);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddFreightTemplateNew(Jinher.AMP.BTP.Deploy.CustomDTO.FreightTempFullDTO ftDto)
        {
            try
            {
                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.SaveFreightTemplateFull(ftDto);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        /// <summary>
        /// 保存范围运费模板
        /// </summary>
        /// <param name="ftDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RangeFreightTemplate(Jinher.AMP.BTP.Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            try
            {
                FreightFacade facade = new FreightFacade();
                ResultDTO result = facade.SaveRangeFreightTemplate(inputDTO);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult DelFreight()
        {
            try
            {
                Guid id = Guid.Parse(Request.Form["Id"]);
                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.DeleteFreight(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult DelFreightDetail()
        {
            try
            {
                Guid id = Guid.Parse(Request.Form["Id"]);
                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.DeleteFreightDetail(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddFreightDetail()
        {
            try
            {
                Guid freightTemplateId = Guid.Parse(Request.Form["FreightTemplateId"]);
                FreightFacade fFacade = new FreightFacade();
                FreightTemplateDetailDTO detail = new FreightTemplateDetailDTO();
                detail.FreightTo = "";
                detail.FirstCount = 1;
                detail.FirstCountPrice = 0;
                detail.NextCount = 1;
                detail.NextCountPrice = 0;
                detail.FreightTemplateId = freightTemplateId;
                ResultDTO result = fFacade.AddFreightDetail(detail);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult UpdateFreightAndDetail(Jinher.AMP.BTP.Deploy.CustomDTO.FreightTempFullDTO ftDto)
        {
            try
            {

                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.SaveFreightTemplateFull(ftDto);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult IsContactCommodity()
        {
            try
            {
                Guid id = Guid.Parse(Request.Form["Id"]);
                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.IsContactCommodity(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }
        #region 测试代码
        public ActionResult CallFreight()
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade cf = new ISV.Facade.CommodityFacade();
            List<TemplateCountDTO> list = new List<TemplateCountDTO>();
            list.Add(new TemplateCountDTO() { CommodityId = Guid.Parse("de6ff826-ab40-47ba-8c2d-47b5940e8062"), Count = 1 });
            return Json(cf.CalFreight("浙江省", list), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFreightDetail(string id)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityFacade cf = new ISV.Facade.CommodityFacade();

            return Json(cf.GetFreightDetails(Guid.Parse(id)), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConfirmPayPriceExt(string commodityOrderId, string userId)
        {
            Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade cf = new ISV.Facade.CommodityOrderFacade();

            return Json(cf.ConfirmPayPrice(Guid.Parse(commodityOrderId), Guid.Parse(userId)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelFreightTest(string Id)
        {
            try
            {
                Guid id = Guid.Parse(Id);
                FreightFacade fFacade = new FreightFacade();
                ResultDTO result = fFacade.DeleteFreight(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultDTO result = new ResultDTO();
                result.ResultCode = 11;
                result.Message = ex.ToString();
                return Json(result);
            }
        }
        #endregion

        /// <summary>
        /// 运费模板与选定的商品建立关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            var output = _freightFacade.JoinCommodity(inputDTO);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 运费模板与选定的商品解除关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnjoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            var output = _freightFacade.UnjoinCommodity(inputDTO);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }
}
