using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public partial class CommodityController : BaseController
    {
        //
        // GET: /CommodityController_CY/

        /// <summary>
        /// 舌尖在线
        /// </summary>
        /// <returns></returns>
        public ActionResult CYSalingComdtyManage()
        {
            return this.Index();
        }

        public PartialViewResult CYPartialIndex(string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice)
        {
            return this.PartialIndex(commodityName, commodityCategory, sSalesvolume, eSalesvolume, sPrice, ePrice);
        }

        [CheckAppId]
        public ActionResult CYStoreIndex()
        {
            return this.StoreIndex();
        }

        [CheckAppId]
        public PartialViewResult CYPartialStoreIndex(string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice)
        {
            return this.PartialStoreIndex(commodityName, commodityCategory, sSalesvolume, eSalesvolume, sPrice, ePrice);
        }

        public ActionResult CYAddCommodity()
        {
            return this.AddCommodity();
        }

        public ActionResult CYUpdateCommodity(Guid commodityId)
        {
            var cbox = new Jinher.AMP.BTP.IBP.Facade.CommodityFacade().GetCommodityBoxInfo(commodityId);
            if (cbox != null)
            {
                ViewBag.CommodityBoxPrice = cbox.MealBoxAmount;
                ViewBag.CommodityBoxCount = cbox.MealBoxNum;
            }
            else
            {
                ViewBag.CommodityBoxCount = ViewBag.CommodityBoxPrice = "";
            }
            return this.UpdateCommodity(commodityId);
        }


        /// <summary>
        /// 出售商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CYSaleIndex()
        {
            return this.Index();
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public PartialViewResult CYSetStoreIndex(string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice)
        {
            return this.PartialIndex(commodityName, commodityCategory, sSalesvolume, eSalesvolume, sPrice, ePrice);
        }


    }
}
