using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Deploy;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.UIJquery.DataGrid;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class BrandController : BaseController
    {
        //
        // GET: /Brand/
        public ActionResult Index()
        {
            BrandFacade pf = new BrandFacade();
            string strappid = Request.QueryString["appId"];
            if (string.IsNullOrEmpty(strappid))
            {
                strappid = System.Web.HttpContext.Current.Session["APPID"].ToString();
            }
            Guid appid;
            if (!Guid.TryParse(strappid, out appid))
            {
                Response.StatusCode = 404;
                return null;
            }
            if (appid != null)
            {
                System.Web.HttpContext.Current.Session["APPID"] = appid;
            }
            return View();
        }

        /// <summary>
        /// 根据条件分页查询品牌列表
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="brandStatus"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [GridAction]
        public ActionResult GetBrandPageList(string brandName, int brandStatus, int page, int pageSize)
        {
            BrandFacade pf = new BrandFacade();
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            int rowCount = 0;
            ResultDTO<List<BrandwallDTO>> result = pf.GetBrandPageList(brandName, brandStatus, pageSize, page, appId);
            List<BrandModel> list = new List<BrandModel>();
            if (result.isSuccess)
            {
                rowCount = result.ResultCode;
                foreach (var item in result.Data)
                {
                    list.Add(new BrandModel()
                    {
                        Id = item.Id,
                        BrandName = item.Brandname,
                        BrandLogo = item.BrandLogo,
                        BrandStatu = item.Brandstatu
                    });
                }
            }

            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("BrandName");
            show.Add("BrandLogo");
            show.Add("BrandStatu");
            var gridobj = new GridModel<BrandModel>(show, list, rowCount, page, pageSize, string.Empty);
            return View(gridobj);
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="brandDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBrand(FormCollection brandDTO)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            BrandFacade bf = new BrandFacade();
            int rowCount = 0;
            bool flag = bf.CheckBrand(brandDTO["BrandName"], out rowCount, appId);
            if (flag && rowCount > 0)
                return Json(new { Result = false, Messages = "品牌名称已存在，请重新输入" });
            BrandwallDTO bw = new BrandwallDTO();
            bw.Id = Guid.NewGuid();
            bw.Brandname = brandDTO["BrandName"];
            bw.BrandLogo = brandDTO["BrandLogo"];
            bw.Brandstatu = Convert.ToInt32(brandDTO["BrandStatu"]);
            bw.SubTime = DateTime.Now;
            bw.ModifiedOn = bw.SubTime;
            bw.AppId = appId;
            ResultDTO res = bf.AddBrand(bw);

            if (res.ResultCode == 0)
            {
                return Json(new { Result = true, Messages = "保存成功" });
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="brandDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateBrand(FormCollection brandDTO)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            BrandFacade bf = new BrandFacade();
            int rowCount = 0;
            bool flag = bf.CheckBrand(brandDTO["BrandName"], out rowCount, appId);
            if (flag & rowCount > 1)
                return Json(new { Result = false, Messages = "品牌名称已存在，请重新输入" });
            BrandwallDTO bw = bf.GetBrand(Guid.Parse(brandDTO["ID"]), appId);
            if (bw != null)
            {
                bw.BrandLogo = brandDTO["BrandLogo"];
                bw.Brandname = brandDTO["Brandname"];
                bw.Brandstatu = Convert.ToInt32(brandDTO["Brandstatu"]);
                bw.ModifiedOn = DateTime.Now;
                bw.AppId = appId;
                ResultDTO res = bf.UpdateBrand(bw);
                if (res.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "修改成功" });
                }
            }

            return Json(new { Result = false, Messages = "修改失败" });
        }

        /// <summary>
        /// 更新品牌状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateBrandStatus(FormCollection brandWallDto)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            BrandFacade bf = new BrandFacade();
            BrandwallDTO brandDto = bf.GetBrand(Guid.Parse(brandWallDto["ID"]), appId);
            if (brandDto != null)
            {
                if (Convert.ToInt32(brandWallDto["BrandStatus"]) == 1)
                    brandDto.Brandstatu = 2;
                if (Convert.ToInt32(brandWallDto["BrandStatus"]) == 2)
                    brandDto.Brandstatu = 1;

                ResultDTO res = bf.UpdateBrand(brandDto);
                if (res.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "修改成功" });
                }
                return Json(new { Result = false, Messages = "修改失败" });
            }
            return Json(new { Result = false, Messages = "修改失败" });
        }

        /// <summary>
        /// 获取品牌详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBrandDetail(Guid id)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            BrandFacade bf = new BrandFacade();
            BrandwallDTO brandDto = bf.GetBrand(id, appId);
            if (brandDto != null)
            {
                return Json(new { Result = true, BrandName = brandDto.Brandname, BrandLogo = brandDto.BrandLogo, BrandStatus = brandDto.Brandstatu });
            }
            return Json(new { Result = false });
        }
        /// <summary>
        /// 分类品牌墙查询
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="brandStatus"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBrandList(string brandName, int brandStatus, int pageNo, int pageSize)
        {
            string strAppId = System.Web.HttpContext.Current.Session["APPID"].ToString();
            Guid appId;
            if (!Guid.TryParse(strAppId, out appId))
            {
                Response.StatusCode = 404;
                return null;
            }
            BrandFacade pf = new BrandFacade();
            ResultDTO<List<BrandwallDTO>> result = pf.GetBrandPageList(brandName, brandStatus, pageSize, pageNo, appId);
            List<BrandwallDTO> list = new List<BrandwallDTO>();
            if (result.isSuccess)
            {
                foreach (var item in result.Data)
                {
                    list.Add(new BrandwallDTO()
                    {
                        Id = item.Id,
                        Brandname = item.Brandname
                    });
                }
                return Json(new { Result = true, DataList = list, DataCount = result.ResultCode });
            }
            return Json(new { Result = false });
        }

        /// <summary>
        /// 根据appId获取品牌信息
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="brandStatus"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="brandAppId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBrandListByAppId(string brandName, int brandStatus, int pageNo, int pageSize, string brandAppId)
        {
            Guid appId;
            if (!Guid.TryParse(brandAppId, out appId))
            {
                Response.StatusCode = 404;
                return Json(new { Result = false });
            }
            BrandFacade pf = new BrandFacade();
            ResultDTO<List<BrandwallDTO>> result = pf.GetBrandPageList(brandName, brandStatus, pageSize, pageNo, appId);
            List<BrandwallDTO> list = new List<BrandwallDTO>();
            if (result.isSuccess)
            {
                foreach (var item in result.Data)
                {
                    list.Add(new BrandwallDTO()
                    {
                        Id = item.Id,
                        Brandname = item.Brandname
                    });
                }
                return Json(new { Result = true, DataList = list, DataCount = result.ResultCode });
            }
            return Json(new { Result = false });
        }

        /// <summary>
        /// 根据一级分类ID获取品牌墙信息
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getBrandByCateID(Guid CategoryID)
        {
            Jinher.AMP.BTP.ISV.Facade.BrandFacade pf = new ISV.Facade.BrandFacade();
            ResultDTO<IList<BrandwallDTO>> result = pf.getBrandByCateID(CategoryID);
            IList<BrandwallDTO> list = new List<BrandwallDTO>();
            if (result.isSuccess)
            {
                foreach (var item in result.Data)
                {
                    list.Add(new BrandwallDTO()
                    {
                        Id = item.Id,
                        Brandname = item.Brandname
                    });
                }
                return Json(new { Result = true, DataList = list, DataCount = result.ResultCode });
            }
            return Json(new { Result = false });
        }

    }
}
