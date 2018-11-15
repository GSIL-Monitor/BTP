using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class AppTradeInfoController : Controller
    {
        //
        // GET: /AppTradeInfo/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  获取应用交易列表
        /// </summary>
        /// <returns></returns>
        [GridAction]
        public ActionResult AppTradeInfoList(string appName)
        {
            CommodityOrderFacade facade = new CommodityOrderFacade();

            List<Deploy.CustomDTO.QryOrderTradeMoneyDTO> resultDTOs = facade.QryAppOrderTradeInfo(appName);

            
            List<string> showList = new List<string>();
            showList.Add("AppId");
            showList.Add("UserName");
            showList.Add("AppName");
            showList.Add("TradeMoney");
            showList.Add("PayTradeMoney");
            showList.Add("UserId");
            
            return View(new GridModel<QryOrderTradeMoneyDTO>(showList, resultDTOs, resultDTOs.Count,1, 20, string.Empty));

        }
    }
}
