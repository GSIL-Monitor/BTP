using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.Facade;
using System.Text.RegularExpressions;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CommodityDetailController : Controller
    {
        public static Regex regex = new Regex(@"<img\s[^>]*>(?:\s*?</img>)?", RegexOptions.Compiled);
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="needImage">是否需要图片 0:不需要 1:需要</param>
        /// <returns>视图</returns>
        //[OutputCache(Duration = 36000, VaryByParam = "id;needImage")]
        public ActionResult Index(string id, string needImage = "1")
        {
            Guid commodityId = Guid.Empty;
            if (Guid.TryParse(id, out commodityId))
            {
                ShareQueryFacade comf = new ShareQueryFacade();
                CommodityDTO commdityDTO = comf.GetCommodity(commodityId);

                if (commdityDTO != null && !string.IsNullOrEmpty(commdityDTO.Description))
                {
                    if (needImage == "1")
                    {
                        this.ViewBag.Content = commdityDTO.Description;
                        this.ViewBag.TechSpecs = commdityDTO.TechSpecs;
                        this.ViewBag.SaleService = commdityDTO.SaleService;
                    }
                    else if (needImage == "0")
                    {
                        this.ViewBag.Content = regex.Replace(commdityDTO.Description, "");
                        this.ViewBag.TechSpecs = regex.Replace(commdityDTO.TechSpecs, "");
                        this.ViewBag.SaleService = regex.Replace(commdityDTO.SaleService, "");
                    }
                    return View();
                }
                else
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }
        }

        /// <summary>
        /// 商品详情(新页面)
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="needImage">是否需要图片 0:不需要 1:需要</param>
        /// <returns>视图</returns>
        //[OutputCache(Duration = 36000, VaryByParam = "id;needImage")]
        public ActionResult Detail(string id, string needImage = "1")
        {
            Guid commodityId = Guid.Empty;
            if (Guid.TryParse(id, out commodityId))
            {
                ShareQueryFacade comf = new ShareQueryFacade();
                CommodityDTO commdityDTO = comf.GetCommodity(commodityId);

                if (commdityDTO != null && !string.IsNullOrEmpty(commdityDTO.Description))
                {
                    if (needImage == "1")
                    {
                        this.ViewBag.Name = commdityDTO.Name;
                        this.ViewBag.Content = commdityDTO.Description;
                        this.ViewBag.TechSpecs = commdityDTO.TechSpecs;
                        this.ViewBag.SaleService = commdityDTO.SaleService;
                    }
                    else if (needImage == "0")
                    {
                        this.ViewBag.Name = commdityDTO.Name;
                        this.ViewBag.Content = regex.Replace(commdityDTO.Description, "");
                        this.ViewBag.TechSpecs = regex.Replace(commdityDTO.TechSpecs, "");
                        this.ViewBag.SaleService = regex.Replace(commdityDTO.SaleService, "");
                    }
                    return View();
                }
                else
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }
        }


    }
}
