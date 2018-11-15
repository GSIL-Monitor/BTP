using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.JAP.MVC.Controller;
using System.Web.Mvc;
using Jinher.AMP.BTP.UI.Util;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 商品推广佣金设置
    /// </summary>
    public class CommoditySpreadDividendController : BaseController
    {
        /// <summary>
        /// 商品佣金设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Guid appId = WebUtil.AppId;
            bool? isDividendAll = null;
            decimal spreadPercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommoditySpreadPercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isDividendAll = result.Data.IsDividendAll;
            spreadPercent = result.Data.SharePercent;

            ViewBag.IsDividendAll = isDividendAll == null ? -1 : isDividendAll.Value == true ? 1 : 0;
            ViewBag.SpreadPercent = spreadPercent;
            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            ViewBag.AppExtensionDTO = appExtensionDTO;
            if (isDividendAll == null)
            {
                var url = Request.Url.ToString();
                url = url.TrimEnd('/');
                string param = url.Substring(url.IndexOf("?"));
                return Redirect(Url.Action("Edit") + param);
            }
            return View();
        }

        /// <summary>
        /// 商品佣金编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            Guid appId = WebUtil.AppId;
            bool? isDividendAll = false;
            decimal sharePercent = 0;
            CommodityFacade comfa = new CommodityFacade();
            var result = comfa.GetCommoditySpreadPercentByAppId(appId);
            if (result.ResultCode != 0)
            {

            }
            isDividendAll = result.Data.IsDividendAll;
            sharePercent = result.Data.SharePercent;

            ViewBag.IsDividendAll = isDividendAll == null ? -1 : isDividendAll.Value == true ? 1 : 0;

            //获取全局配置信息
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtensionDTO = comfa.GetDefaulDistributionAccount(appId);
            if (appExtensionDTO == null)
            {
                appExtensionDTO = new Deploy.CustomDTO.AppExtensionDTO();
                appExtensionDTO.Id = appId;
            }
            ViewBag.AppExtensionDTO = appExtensionDTO;

            if (isDividendAll == null)
            {
                ViewBag.SharePercent = -1;
            }
            else if (result.Data.CShareList == null && result.Data.SharePercent == 0)
            {
                ViewBag.SharePercent = 0;
            }
            else
            {
                ViewBag.SharePercent = sharePercent;
            }
            return View();
        }

        /// <summary>
        /// 获取商品佣金Grid数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetGridData(Guid appId)
        {
            int pNum = 0;
            int.TryParse(Request["page"], out pNum);

            int pSize = 0;
            int.TryParse(Request["rows"], out pSize);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam getCommodityByNameParam = new Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam();

            getCommodityByNameParam.AppId = appId;
            getCommodityByNameParam.OnlySpreadMoney = true;
            getCommodityByNameParam.PageIndex = pNum;
            getCommodityByNameParam.PageSize = pSize;

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();

            var result = comFacade.GetCommodityByName(getCommodityByNameParam);

            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("Pic");
            showList.Add("Name");
            showList.Add("Price");
            showList.Add("SpreadPercent");
            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>(showList, result.Data.CommodityList, result.Data.Count, getCommodityByNameParam.PageIndex, string.Empty));
        }

        /// <summary>
        /// 获取商品佣金编辑时展示JSON
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGridDataJson(Guid appId)
        {
            //int pNum = 0;
            //int.TryParse(Request["page"], out pNum);

            //int pSize = 0;
            //int.TryParse(Request["rows"], out pSize);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam getCommodityByNameParam = new Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam();

            getCommodityByNameParam.AppId = appId;
            getCommodityByNameParam.OnlySpreadMoney = true;
            getCommodityByNameParam.PageIndex = 1;
            getCommodityByNameParam.PageSize = int.MaxValue;

            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();

            var result = comFacade.GetCommodityByName(getCommodityByNameParam);

            return Json(result.Data.CommodityList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 选择商品
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult SelectCommodity(GetCommodityByNameParam pdto)
        {
            Guid appId = WebUtil.AppId;
            CategoryFacade catefa = new CategoryFacade();
            ViewBag.CategoryList = catefa.GetCategories(appId);

            ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);

            return View();
        }

        /// <summary>
        /// 获取选择商品Grid数据
        /// </summary>
        /// <param name="pdto"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult SelectCommodityGridData(GetCommodityByNameParam pdto)
        {
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }

            CommodityFacade comfa = new CommodityFacade();

            List<CommodityListCDTO> comCDTO = new List<CommodityListCDTO>();

            ResultDTO<CommodityDividendListDTO> comList = new ResultDTO<CommodityDividendListDTO>();
            comList = comfa.GetCommodityByName(pdto);
            List<CommodityListSDTO> coms = new List<CommodityListSDTO>();
            foreach (CommodityListCDTO dto in comList.Data.CommodityList)
            {
                CommodityListSDTO com = new CommodityListSDTO();
                com.Id = dto.Id;
                com.Pic = dto.Pic;
                com.Name = dto.Name;
                com.Price = dto.Price;
                com.Stock = dto.Stock;
                coms.Add(com);
            }
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Pic");
            show.Add("Name");
            show.Add("Price");
            show.Add("Stock");
            return View(new GridModel<CommodityListSDTO>(show, coms, comList.Data.Count, pageIndex, 66365, string.Empty));
        }

        /// <summary>
        /// 保存佣金比例
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult SaveCommissionPercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            Jinher.AMP.BTP.IBP.Facade.CommodityFacade comFacade = new IBP.Facade.CommodityFacade();
            var result = comFacade.SaveCommoditySpreadPercent(dto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}