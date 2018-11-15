using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.JAP.MVC.Controller;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class SelectCommodityController : BaseController
    {
        //
        // GET: /SelectCommodity/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAppList()
        {
            string strAppId = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = Convert.ToString(System.Web.HttpContext.Current.Session["APPID"]);
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

            AppSetSearch2DTO search = new AppSetSearch2DTO
            {
                belongTo = appId,
                PageIndex = Convert.ToInt32(Request["apppage"]),
                PageSize = Convert.ToInt32(Request["pagesize"])
            };
            IBP.Facade.SelectCommodityFacade facade = new IBP.Facade.SelectCommodityFacade();
            AppSetAppGridDTO appGridDto = facade.GetAppList(search);
            int totalpage = Convert.ToInt32(Math.Ceiling((double)appGridDto.TotalAppCount / search.PageSize));
            return Json(new { applist = appGridDto.AppList, totalpage });
        }

        [GridAction]
        public ActionResult SearchCommodity(Guid? AppId, string appName = "", string commodityName = "")
        {
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            ResultDTO<List<ComdtyList4SelCDTO>> retInfo = GetCommodityList(AppId, appName, commodityName, pageIndex, pageSize);

            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("AppId");
            show.Add("AppName");
            show.Add("Pic");
            show.Add("Name");
            show.Add("Price");
            show.Add("CommodityCategory");
            show.Add("Stock");
            int count = retInfo != null ? retInfo.ResultCode : 0;
            return View(new GridModel<ComdtyList4SelCDTO>(show, retInfo.Data, count, pageIndex, pageSize, string.Empty));
        }

        /// <summary>
        /// 私有方法 获取选择商品数据
        /// </summary>
        /// <param name="AppId">AppId</param>
        /// <param name="AppNmae">App名称</param>
        /// <param name="commodityName">商品名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        private ResultDTO<List<ComdtyList4SelCDTO>> GetCommodityList(Guid? AppId, string AppNmae = "", string commodityName = "", int pageIndex = 1, int pageSize = 20)
        {
            string strAppId = Request.QueryString["belongTo"];
            if (string.IsNullOrEmpty(strAppId))
            {
                strAppId = Convert.ToString(System.Web.HttpContext.Current.Session["belongTo"]);
            }
            Guid belongTo;
            if (!Guid.TryParse(strAppId, out belongTo))
            {
                Response.StatusCode = 404;
                return null;
            }
            System.Web.HttpContext.Current.Session["belongTo"] = belongTo;

            ComdtySearch4SelCDTO search = new ComdtySearch4SelCDTO
            {
                AppId = AppId,
                AppName = AppNmae,
                CommodityName = commodityName,
                PageIndex = pageIndex,
                PageSize = pageSize,
                belongTo = belongTo
            };

            IBP.Facade.SelectCommodityFacade facade = new IBP.Facade.SelectCommodityFacade();
            return facade.SearchCommodity(search);
        }

        /// <summary>
        /// 选择商品（单选）页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SelSingleComdty()
        {
            return View();
        }

        ///// <summary>
        ///// 获取选择商品（单选）列表数据
        ///// </summary>
        ///// <param name="appId">appId</param>
        ///// <param name="appName">app名称</param>
        ///// <param name="commodityName">商品名称</param>
        ///// <returns></returns>
        //[GridAction]
        //public ActionResult SearchSingleCmdty(Guid? appId, string appName = "", string commodityName = "")
        //{
        //    int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
        //    int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

        //    Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> retInfo = GetCommodityList(appId, appName, commodityName, pageIndex, pageSize);

        //    IList<string> show = new List<string>();
        //    show.Add("Id");
        //    show.Add("AppId");
        //    show.Add("AppName");
        //    show.Add("Pic");
        //    show.Add("Name");
        //    show.Add("Price");
        //    show.Add("Stock");
        //    int count = retInfo.Data != null ? retInfo.ResultCode : 0;
        //    return View(new GridModel<Deploy.CustomDTO.ComdtyList4SelCDTO>(show, retInfo.Data, count, pageIndex, pageSize, string.Empty));
        //}

        /// <summary>
        /// APP馆选择单个商品
        /// </summary>
        /// <returns></returns>
        public ActionResult AoSelComdty()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="belongTo"></param>
        /// <param name="commodityName"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult SearchAoAppCommodity(Guid? belongTo, string commodityName = "")
        {
            int pageIndex = Convert.ToInt32(Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request["rows"] ?? "20");

            ResultDTO<List<ComdtyList4SelCDTO>> retInfo = GetCommodityList2(belongTo, commodityName, pageIndex, pageSize);

            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("AppId");
            show.Add("AppName");
            show.Add("Pic");
            show.Add("Name");
            show.Add("Price");
            show.Add("CommodityCategory");
            show.Add("Stock");
            int count = retInfo != null ? retInfo.ResultCode : 0;
            return View(new GridModel<ComdtyList4SelCDTO>(show, retInfo.Data, count, pageIndex, pageSize, string.Empty));
        }

        /// <summary>
        /// 私有方法 获取选择商品数据
        /// </summary>
        /// <param name="AppId">AppId</param>
        /// <param name="AppNmae">App名称</param>
        /// <param name="commodityName">商品名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        private ResultDTO<List<ComdtyList4SelCDTO>> GetCommodityList2(Guid? AppId, string commodityName = "", int pageIndex = 1, int pageSize = 20)
        {
            ComdtySearch4SelCDTO search = new ComdtySearch4SelCDTO
            {
                AppId = AppId,
                CommodityName = commodityName,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            IBP.Facade.SelectCommodityFacade facade = new IBP.Facade.SelectCommodityFacade();
            return facade.SearchCommodity2(search);
        }

        /// <summary>
        /// 运费模板关联商品
        /// </summary>
        /// <param name="appId">应用编号</param>
        /// <param name="templateId">运费模板编号</param>
        /// <param name="showAssociated">是否要显示已关联的</param>
        /// <param name="callback">js的回调方法</param>
        /// <returns></returns>
        public ActionResult FreightTemplateJoinCommodity(Guid appId, Guid templateId, bool showAssociated, string callback)
        {
            return View();
        }

        /// <summary>
        /// 运费模板商品关系
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="templateId"></param>
        /// <param name="showAssociated"></param>
        /// <param name="commodityName"></param>
        /// <returns></returns>
        [GridAction, HttpGet]
        public ActionResult FreightTemplateCommodityRel(SearchCommodityByFreightTemplateInputDTO inputDTO, int page = 1)
        {
            var displayColumns = new List<string> { "Id", "Pic", "Name", "Price", "Stock" };

            inputDTO.PageIndex = page;
            inputDTO.PageSize = 20;

            var facade = new IBP.Facade.SelectCommodityFacade();

            var outputDTO = facade.SearchCommodity3(inputDTO);

            var data = outputDTO.Data;

            return View(new GridModel<ComdtyList4SelCDTO>(displayColumns, data.Commodities, data.Total, page, 20, string.Empty));
        }

        #region 获取分类
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCategoryLevelOne(string appId)
        {
            //获取分类******

            List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> list = Commons.CategoryHelper.GetCategoryLevelOne(appId);
            return Json(new { data = list });

        }
        #endregion
    }
}
