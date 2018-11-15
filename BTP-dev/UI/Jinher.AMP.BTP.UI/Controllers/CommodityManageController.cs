using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using AppSetCategoryDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO;
using AppSetCommodityDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityDTO;
using AppSetCommodityGridDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGridDTO;
using SetCommodityOrderDTO = Jinher.AMP.BTP.Deploy.CustomDTO.SetCommodityOrderDTO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CommodityManageController : BaseController
    {
        private AppSetFacade appSetFacade = new AppSetFacade();

        public ActionResult CommodityManage()
        {
            return View();
        }
        [CheckAppId]
        public ActionResult TLevelCommodityManage()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            ViewBag.IsPavilion = ZPHSV.Instance.IsAppPavilion(appId);
            return View();
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="isCascade"></param>
        /// <returns></returns>
        public ActionResult GetSetCategory()
        {
            List<AppSetCategoryDTO> categrotList = appSetFacade.GetCategoryListForTree();
            return Json(new { Success = true, data = categrotList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="isCascade"></param>
        /// <returns></returns>
        public ActionResult GetSetTLevelCategory()
        {
            Guid appId = Guid.Parse(Request["appId"]);
            List<AppSetCategoryDTO> categrotList = appSetFacade.GetCategoryListForTree(appId);
            return Json(new { Success = true, data = categrotList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取分类下商品列表
        /// </summary>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetCommodityInCategory()
        {
            string appName = Request["appName"];
            string commodityName = Request["commodityName"];
            Guid categoryId = Guid.Parse(Request["categoryId"]);
            int PageNo = Request["page"] == null ? 0 : int.Parse(Request["page"]);
            int PageSize = Request["rows"] == null ? 0 : int.Parse(Request["rows"]);
            AppSetCommodityGridDTO gridDto = appSetFacade.GetCommodityInCategory(categoryId, PageNo, PageSize, appName, commodityName);
            IList<string> show = new List<string>();
            show.Add("CommodityId");
            show.Add("AppId");
            show.Add("AppIcon");
            show.Add("AppName");
            show.Add("CommodityPic");
            show.Add("CommodityName");
            show.Add("CommodityPrice");
            show.Add("CommodityStock");
            show.Add("Operation");
            return View(new GridModel<AppSetCommodityDTO>(show, gridDto.CommodityList, gridDto.TotalCommodityCount, PageNo, PageSize, string.Empty));
        }

        /// <summary>
        /// 获取分类下商品列表
        /// </summary>
        /// <returns></returns>
        [GridAction]
        [CheckAppId]
        public ActionResult GetCommodityInTLevelCategory()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            string appName = Request["appName"];
            string commodityName = Request["commodityName"];
            Guid categoryId = Guid.Parse(Request["categoryId"]);
            int pageNo = Request["page"] == null ? 0 : int.Parse(Request["page"]);
            int pageSize = Request["rows"] == null ? 0 : int.Parse(Request["rows"]);
            AppSetCommodityGrid2DTO gridDto = appSetFacade.GetCommodityInCategory2(appId, categoryId, pageNo, pageSize, appName, commodityName);
            List<AppSetCommodity2DTO> commodityList = new List<AppSetCommodity2DTO>();
            if (gridDto.TotalCommodityCount > 0)
            {
                commodityList = gridDto.CommodityList.OrderByDescending(p => p.SetCategorySort).ToList();
            }
            IList<string> show = new List<string>();
            show.Add("CommodityId");
            show.Add("AppId");
            show.Add("AppIcon");
            show.Add("AppName");
            show.Add("CommodityPic");
            show.Add("CommodityName");
            show.Add("CommodityPrice");
            show.Add("CommodityCategory");
            show.Add("CommodityStock");
            show.Add("SetCategorySort");
            return View(new GridModel<AppSetCommodity2DTO>(show, commodityList, gridDto.TotalCommodityCount, pageNo, pageSize, string.Empty));
        }


        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCommodityToCategory()
        {
            List<Guid> categoryIdList = new List<Guid>();
            List<Guid> commodityIdList = new List<Guid>();
            string categoryIds = this.Request["categoryIds"];
            string commodityIds = this.Request["commodityIds"];
            if (!string.IsNullOrEmpty(categoryIds))
            {
                categoryIdList = categoryIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            if (!string.IsNullOrEmpty(commodityIds))
            {
                commodityIdList = commodityIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            ResultDTO dto = appSetFacade.AddCommodityToCategory(commodityIdList, categoryIdList);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }


        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCommodityToCategory2()
        {
            List<Guid> categoryIdList = new List<Guid>();
            List<Guid> commodityIdList = new List<Guid>();
            string categoryIds = this.Request["categoryIds"];
            string commodityIds = this.Request["commodityIds"];
            if (!string.IsNullOrEmpty(categoryIds))
            {
                categoryIdList = categoryIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            if (!string.IsNullOrEmpty(commodityIds))
            {
                commodityIdList = commodityIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            ResultDTO dto = appSetFacade.AddCommodityToCategory2(commodityIdList, categoryIdList);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <returns></returns>
        public ActionResult DelCommodityFromCategory()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            List<Guid> commodityIdList = new List<Guid>();
            string commodityIds = this.Request["commodityIds"];
            if (!string.IsNullOrEmpty(commodityIds))
            {
                commodityIdList = commodityIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            ResultDTO dto = appSetFacade.DelCommodityFromCategory(commodityIdList, categoryId);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <returns></returns>
        public ActionResult DelCommodityFromCategory2()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            List<Guid> commodityIdList = new List<Guid>();
            string commodityIds = this.Request["commodityIds"];
            if (!string.IsNullOrEmpty(commodityIds))
            {
                commodityIdList = commodityIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            ResultDTO dto = appSetFacade.DelCommodityFromCategory2(commodityIdList, categoryId);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 分类下商品排序
        /// </summary>
        /// <returns></returns>
        public ActionResult ReOrderCommodityInCategory()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            List<Guid> commodityIdList = new List<Guid>();
            List<int> commoditySortList = new List<int>();
            string commodityIds = this.Request["commodityIds"];
            string commoditySorts = this.Request["commoditySorts"];
            if (!string.IsNullOrEmpty(commodityIds))
            {
                commodityIdList = commodityIds.TrimEnd(',').Split(',').Select(p => Guid.Parse(p)).ToList();
            }
            if (!string.IsNullOrEmpty(commoditySorts))
            {
                commoditySortList = commoditySorts.TrimEnd(',').Split(',').Select(p => int.Parse(p)).ToList();
            }
            ResultDTO dto = appSetFacade.ReOrderCommodityInCategory(categoryId, commodityIdList, commoditySortList);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 调整分类中商品排序(上移\下移)
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeCommodityOrderInCategory()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            Guid commodityId = Guid.Parse(this.Request["commodityId"]);
            string dirctionStr = this.Request["direction"];
            int direction = 0;
            if (dirctionStr == "up")
            {
                direction = -1;
            }
            else if (dirctionStr == "down")
            {
                direction = 1;
            }
            ResultDTO dto = appSetFacade.ChangeCommodityOrderInCategory(categoryId, commodityId, direction);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCommodityCountInCategory()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            int count = appSetFacade.GetCommodityCountInCategory(categoryId);
            return Json(new { Count = count });
        }

        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCommodityCountInCategory2()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            int count = appSetFacade.GetCommodityCountInCategory2(categoryId);
            return Json(new { Count = count });
        }

        /// <summary>
        /// 分类移动
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeCategorySort()
        {
            Guid categoryId = Guid.Parse(Request["categoryId"]);
            Guid targetCategoryId = Guid.Parse(Request["targetCategoryId"]);
            ResultDTO dto = appSetFacade.ChangeCategorySort(categoryId, targetCategoryId);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCategory()
        {
            string categoryName = Request["categoryName"];
            string parentIdStr = Request["parentId"];
            string picturesPath = Request["picturesPath"];
            if (string.IsNullOrEmpty(categoryName))
            {
                return Json(new { Success = false, Message = "分类名称不能为空" });
            }
            if (categoryName.Length > 10)
            {
                return Json(new { Success = false, Message = "分类名称不能超过10个字符" });
            }
            Guid parentId = Guid.Empty;
            if (string.IsNullOrWhiteSpace(parentIdStr) || !Guid.TryParse(parentIdStr, out parentId))
            {
                return Json(new { Success = false, Message = "父分类不明确" });
            }
            var dto = appSetFacade.AddCategory(categoryName, parentId, picturesPath);
            return Json(new { Success = dto.Item2 == 0, Message = dto.Item3, CategoryId = dto.Item1 });
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCategory()
        {
            Guid categoryId = Guid.Parse(Request["categoryId"]);
            string categoryName = Request["categoryName"];
            string picturesPath = Request["picturesPath"];
            if (string.IsNullOrEmpty(categoryName))
            {
                return Json(new { Success = false, Message = "分类名称不能为空" });
            }
            if (categoryName.Length > 10)
            {
                return Json(new { Success = false, Message = "分类名称不能超过10个字符" });
            }
            ResultDTO dto = appSetFacade.UpdateCategory(categoryId, categoryName, picturesPath);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <returns></returns>
        public ActionResult DelCategory()
        {
            Guid categoryId = Guid.Parse(Request["categoryId"]);
            ResultDTO dto = appSetFacade.DelCategory(categoryId);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }
        /// <summary>
        /// 商品在分类中置顶
        /// </summary>
        /// <returns></returns>
        public ActionResult TopCommodityOrderInCategory()
        {
            Guid categoryId = Guid.Parse(this.Request["categoryId"]);
            Guid commodityId = Guid.Parse(this.Request["commodityId"]);
            ResultDTO dto = appSetFacade.TopCommodityOrderInCategory(categoryId, commodityId);
            return Json(new { Success = dto.ResultCode == 0, Message = dto.Message });
        }

        /// <summary>
        /// 批量排序
        /// </summary>
        /// <param name="idSort"></param>
        /// <param name="cryId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetRankNoMultiTLevel(string idSort, string cryId)
        {
            try
            {
                if (string.IsNullOrEmpty(idSort) || string.IsNullOrEmpty(cryId))
                {
                    return Json(new { Success = 1, Message = "参数为空" });
                }

                List<SetCommodityOrderDTO> dtoList = new List<SetCommodityOrderDTO>();
                string[] parasArr = idSort.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, len = parasArr.Length; i < len; i++)
                {
                    string[] parasI = parasArr[i].Split(',');
                    if (parasI.Length != 2)
                    {
                        continue;
                    }

                    Guid pId;
                    Guid.TryParse(parasI[0], out pId);
                    var rankNo = Convert.ToDouble(parasI[1]);
                    if (pId != Guid.Empty)
                    {
                        dtoList.Add(new SetCommodityOrderDTO() { Id = pId, RankNo = rankNo });
                    }
                }

                AppSetFacade appFacade = new AppSetFacade();
                var appSetSortDto = new Deploy.CustomDTO.AppSetSortDTO
                {
                    DtoList = dtoList,
                    CategoryId = Guid.Parse(cryId)
                };
                ResultDTO retInfo = appFacade.SetSetCommodityOrder2(appSetSortDto);
                return Json(new { Success = retInfo.ResultCode, retInfo.Message });
            }
            catch (Exception)
            {
                return Json(new { Success = 1, Message = "出现异常" });
            }
        }
    }
}