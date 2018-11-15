using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.JAP.MVC.UIJquery.DataGrid;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CrowdfundingController : BaseController
    {


        /// <summary>
        /// 众筹后台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        ///  获取众筹股东列表
        /// </summary>
        /// <returns></returns>
        [GridAction]
        public ActionResult CrowdfundingList(string appName, int state, int PageSize, int PageNumber)
        {
            CrowdfundingFacade facade = new CrowdfundingFacade();

            PageNumber = Request["page"] == null ? 0 : Convert.ToInt32(Request["page"]);
            PageSize = Request["rows"] == null ? 0 : Convert.ToInt32(Request["rows"]);

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCrowdfundingsDTO GetCrowdfundingsDTOlist = facade.GetCrowdfundings(appName, state, PageNumber, PageSize);

            foreach (var item in GetCrowdfundingsDTOlist.List)
            {
                item.SurplusShareCount = item.ShareCount - item.CurrentShareCount;
                item.strState = item.State == 0 ? "进行中" : "众筹成功";
                item.strCurrentShareCount = item.CurrentShareCount + "/" + item.CurrentShareCount * item.PerShareMoney;
                item.strSurplusShareCount = item.SurplusShareCount + "/" + item.SurplusShareCount * item.PerShareMoney;
                item.DividendPercent = item.DividendPercent * 100;
                item.strTotalDividend = str(item.TotalDividend.ToString());
                item.strDividendPercent = str(item.DividendPercent.ToString()) + "%";

            }
            List<string> showList = new List<string>();
            showList.Add("Id");
            showList.Add("AppName");
            showList.Add("PerShareMoney");
            showList.Add("strDividendPercent");
            showList.Add("ShareCount");
            showList.Add("strCurrentShareCount");
            showList.Add("strSurplusShareCount");
            showList.Add("StartTime");
            showList.Add("strTotalDividend");
            showList.Add("strState");
            showList.Add("AppId");
            showList.Add("Description");
            showList.Add("Slogan");
            showList.Add("SubTime");
            return View(new GridModel<CrowdfundingFullDTO>(showList, GetCrowdfundingsDTOlist.List, GetCrowdfundingsDTOlist.Total, PageNumber, string.Empty));

        }

        /// <summary>
        /// 获取股东信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult GetUserCrowdfundingsList(Guid crowdfundingId, string userName, string userCode)
        {
            CrowdfundingFacade facade = new CrowdfundingFacade();

            var PageNumber = Request["page"] == null ? 0 : Convert.ToInt32(Request["page"]);
            var PageSize = Request["rows"] == null ? 0 : Convert.ToInt32(Request["rows"]);

            GetUserCrowdfundingsDTO GetCrowdfundingsDTOlist = facade.GetUserCrowdfundings(crowdfundingId, userName, userCode, PageNumber, PageSize);


            foreach (var item in GetCrowdfundingsDTOlist.List)
            {
                item.strTotalDividend = ((decimal)item.TotalDividend / 1000).ToString();
                item.strRealGetDividend = ((decimal)item.RealGetDividend / 1000).ToString();
            }

            List<string> showList = new List<string>();
            showList.Add("CrowdfundingId");
            showList.Add("UserName");
            showList.Add("UserCode");
            showList.Add("CurrentShareCount");
            showList.Add("Money");
            showList.Add("OrderCount");
            showList.Add("strTotalDividend");
            showList.Add("strRealGetDividend");
            showList.Add("UserId");
            return View(new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.UserCrowdfundingDTO>(showList, GetCrowdfundingsDTOlist.List, GetCrowdfundingsDTOlist.Total, PageNumber, string.Empty));

        }

        //字符串处理
        private string str(string num)
        {
            string arraylist = num;
            int temp = 0;
            string str1 = "";
            for (int i = arraylist.Length - 1; i >= 0; i--)
            {
                if (arraylist[i].ToString() != "0")
                {
                    temp = i;
                    break;
                }
            }
            for (int i = 0; i <= temp; i++)
            {
                str1 += arraylist[i].ToString();
            }

            string a = str1.Split('.')[0];
            string b = str1.Split('.')[1];

            int length = str1.Split('.').Length;
            if (a != "" && b == "")
            {
                str1 = a;
            }

            return str1;
        }

        /// <summary>
        /// 显示新增众筹
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCrowdfunding()
        {
            return View();
        }


        /// <summary>
        /// 保存新增众筹
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            CrowdfundingFacade facade = new CrowdfundingFacade();
            if (crowdfundingDTO != null)
            {
                ResultDTO result = facade.AddCrowdfunding(crowdfundingDTO);
                return Json(new { Result = result.ResultCode, Messages = result.Message });
            }
            else
            {
                return Json(new { Result = 1, Messages = "新增众筹为null" });
            }

        }


        /// <summary>
        /// 根据Id获取众筹
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetCrowdfundingById(Guid Id)
        {

            CrowdfundingFacade facade = new CrowdfundingFacade();
            Jinher.AMP.BTP.Deploy.CrowdfundingDTO cfDTO = facade.GetCrowdfunding(Id);
            ViewBag.Crowdfunding = cfDTO;
            return View();

        }


        /// <summary>
        /// 修改众筹
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCrowdfunding(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            CrowdfundingFacade facade = new CrowdfundingFacade();

            if (crowdfundingDTO != null)
            {
                ResultDTO result = facade.UpdateCrowdfunding(crowdfundingDTO);
                return Json(new { Result = result.ResultCode, Messages = result.Message });
            }
            else
            {
                return Json(new { Result = 1, Messages = "修改众筹为null" });
            }

        }




        /// <summary>
        /// 众筹股东订单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PartialIndex(Guid crowdfundingId, Guid userId, int pageIndex, int pageSize)
        {
            CrowdfundingFacade facade = new CrowdfundingFacade();


            pageIndex = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            pageSize = 20;
            if (!string.IsNullOrEmpty(Request["pageSize"]))
            {
                if (!int.TryParse(Request["pageSize"], out pageSize))
                {
                    pageSize = 20;
                }
                else
                {
                    if (pageSize > 200)
                    {
                        pageSize = 200;//最大200条每次
                    }
                }

            }
            CommodityOrderVMDTO CommodityOrderVMlist = facade.GetUserCrowdfundingOrders(crowdfundingId, userId, pageIndex, pageSize);
            ViewBag.UserCrowdfundingOrders = CommodityOrderVMlist.List;
            ViewBag.Count = CommodityOrderVMlist.Total;
            return PartialView();

        }


        /// <summary>
        /// 根据appId找appName
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAppNameByAppId(Guid appId)
        {
            try
            {

                AppNameDTO appNameDto = new AppNameDTO();
                List<Guid> applist = new List<Guid>();
                applist.Add(appId);
                Dictionary<Guid, string> list = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(applist);
                if (list != null && list.Count > 0 && list.ContainsKey(appId))
                {
                    appNameDto.AppName = list[appId];
                    return Json(new { Result = 1, Messages = appNameDto.AppName });
                }
                else
                {
                    return Json(new { Result = 2, Messages = "输入的appid不存在,或没有发布到市场" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据appId找appName异常。appId：{0}", appId), ex);
                return Json(new { Result = 3, Messages = "Error" });
            }

        }
        public ActionResult Slogan(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingSlogan(appId);
            ViewBag.Slogan = result;
            return View();
        }

        public ActionResult CrowdDesc(Guid appId)
        {
            Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade srefacade = new ISV.Facade.CrowdfundingFacade();

            var result = srefacade.GetCrowdfundingDesc(appId);
            ViewBag.CrowdDesc = result;
            return View();
        }
    }
}
