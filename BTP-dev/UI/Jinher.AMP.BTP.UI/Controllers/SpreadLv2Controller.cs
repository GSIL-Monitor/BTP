using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 二级推广主管理
    /// </summary>
    public class SpreadLv2Controller : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 二级推广主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Guid iwId = new Guid("03FE56FD-1B83-4930-954F-72756DB95753");
            Guid iwId = ContextDTO.LoginOrg;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            ViewBag.Apps = facade.GetLv1SpreadApps(iwId).Data;
            ViewBag.IWCode = EBCSV.GetOrgInfoById(iwId).IWECode;
            return View();
        }

        /// <summary>
        /// 获取二级推广主
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(SpreadSearchDTO dto, int page)
        {
            dto.IWId = ContextDTO.LoginOrg;
            dto.SpreadType = 6;
            dto.PageIndex = page;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            var result = facade.GetSpreadInfoList(dto);
            if (result.isSuccess)
            {
                return Json(new
                {
                    data = result.Data.List,
                    records = result.Data.Count,
                    page = dto.PageIndex
                });
            }
            return Json(result);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult ExportExcel(SpreadSearchDTO dto)
        {
            dto.IWId = ContextDTO.LoginOrg;
            dto.SpreadType = 6;
            dto.PageIndex = 1;
            dto.PageSize = int.MaxValue;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            var spreadInfoes = facade.GetSpreadInfoList(dto);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("推广主姓名", typeof(string));
            dt.Columns.Add("IU账号", typeof(string));
            dt.Columns.Add("推广组织IW号", typeof(string));
            dt.Columns.Add("推广主类型", typeof(string));
            dt.Columns.Add("推广App名称", typeof(string));
            dt.Columns.Add("推广AppID", typeof(string));
            dt.Columns.Add("旺铺商家名称", typeof(string));
            dt.Columns.Add("旺铺AppID", typeof(string));
            dt.Columns.Add("推广码链接", typeof(string));
            dt.Columns.Add("是否启用", typeof(string));
            dt.Columns.Add("备注", typeof(string));
            foreach (var model in spreadInfoes.Data.List)
            {
                dt.Rows.Add(model.Name, model.Account, model.IWCode, model.SpreadTypeDesc, model.SpreadAppName, model.SpreadAppId, model.HotshopName, model.HotshopId, model.QrCodeUrl, model.IsDel == 0 ? "是" : "否", model.SpreadDesc);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("代理推广码_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 创建二级推广码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult Create(SpreadSaveDTO dto)
        {
            // 二级代理
            dto.SpreadType = 6;
            dto.IWId = ContextDTO.LoginOrg;
            dto.IWCode = EBCSV.GetOrgInfoById(ContextDTO.LoginOrg).IWECode;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.SaveSpreadInfo(dto));
        }

        /// <summary>
        /// 修改总代分佣比例
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult UpdateDividendPercent(SpreadUpdateDividendPercentDTO dto)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.UpdateDividendPercent(dto));
        }

        /// <summary>
        ///  根据应用ID获取一级推广下的热门店铺
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetHotShop(Guid appId)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.GetLv1SpreadHotshops(ContextDTO.LoginOrg, appId), JsonRequestBehavior.AllowGet);
        }
    }
}
