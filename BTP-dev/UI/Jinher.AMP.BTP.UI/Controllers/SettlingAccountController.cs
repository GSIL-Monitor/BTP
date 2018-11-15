using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.PL;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;


namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SettlingAccountController : BaseController
    {

        #region 显示
        /// <summary>
        /// 结算页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Guid appId = Guid.Empty;

            Jinher.AMP.BTP.ISV.Facade.AppSetFacade appSetFa = new Jinher.AMP.BTP.ISV.Facade.AppSetFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO appSearch = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO()
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var applist = appSetFa.GetAppSet(appSearch);
            ViewBag.AppList = applist.AppList;


            Jinher.AMP.BTP.IBP.Facade.CategoryFacade cateFa = new Jinher.AMP.BTP.IBP.Facade.CategoryFacade();
            var catelist = cateFa.GetCategories(appId);
            ViewBag.CategoryList = catelist;

            ViewBag.IsShowCategoryTree = true;

            return View();
        }

        /// <summary>
        /// 结算页中的列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="commodityName"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="sPrice"></param>
        /// <param name="ePrice"></param>
        /// <returns></returns>
        public PartialViewResult PartialIndex(string appId, string commodityName, string commodityCategory)
        {
            Guid appIdTmp = Guid.Empty;
            int rowCount = 0;
            int pageIndex = 1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            Guid.TryParse(appId, out appIdTmp);

            #region 列表
            SettlingAccountFacade comfa = new SettlingAccountFacade();

            SettlingAccountSearchDTO search = new SettlingAccountSearchDTO();
            search.appId = appIdTmp;
            search.pageIndex = pageIndex;
            search.pageSize = pageSize;
            search.commodityName = commodityName;
            search.commodityCategory = commodityCategory;

            List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> comcalist = comfa.GetNowSettlingAccount(search, out rowCount).ToList();

            #endregion
            //var catelist = catefa.GetCategories(appId);
            ViewBag.CommodityList = comcalist;
            ViewBag.Count = rowCount;
            //ViewBag.CategoryList = catelist;
            return PartialView();
        }

        /// <summary>
        /// 商品类别列表
        /// </summary>
        /// <param name="selectappId"></param>
        /// <returns></returns>
        public PartialViewResult showGetCategories(string selectappId)
        {

            Guid appId = new Guid();
            Guid.TryParse(selectappId, out appId);
            CategoryFacade catefa = new CategoryFacade();
            var catelist = catefa.GetCategories(appId);
            ViewBag.CategoryList = catelist;

            if (appId == Guid.Empty)
            {
                ViewBag.IsShowCategoryTree = true;
            }
            else
            {
                ViewBag.IsShowCategoryTree = Jinher.AMP.BTP.UI.Models.APPManageVM.GetIsShowCategoryTree(appId);
            }

            return PartialView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityName"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="selectappId"></param>
        /// <returns></returns>
        public ActionResult SettingAccountHistoryRecord(string commodityId)
        {
            Guid commodityIdTmp = Guid.Empty;
            int rowCount = 0;
            int pageIndex = 1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            Guid.TryParse(commodityId, out commodityIdTmp);

            #region 列表
            SettlingAccountFacade safa = new SettlingAccountFacade();

            Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO search = new Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO();
            search.CommodityId = commodityIdTmp;
            search.PageIndex = pageIndex;
            search.PageSize = pageSize;

            List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> salist = safa.GetHistorySettlingAccount(search, out rowCount).ToList();

            #endregion

            ViewBag.CommodityHistoryList = salist;
            ViewBag.PageCurrent = pageIndex;
            ViewBag.Count = rowCount;

            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="commodityName"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="selectappId"></param>
        /// <returns></returns>
        public PartialViewResult SettingAccountHistoryPartialIndex(string commodityId)
        {
            Guid commodityIdTmp = Guid.Empty;
            int rowCount = 0;
            int pageIndex = 1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["currentPage"]))
            {
                pageIndex = int.Parse(Request.QueryString["currentPage"]);
            }
            int pageSize = 20;
            Guid.TryParse(commodityId, out commodityIdTmp);

            #region 列表
            SettlingAccountFacade safa = new SettlingAccountFacade();

            Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO search = new Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO();
            search.CommodityId = commodityIdTmp;
            search.PageIndex = pageIndex;
            search.PageSize = pageSize;

            List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> salist = safa.GetHistorySettlingAccount(search, out rowCount).ToList();

            #endregion

            ViewBag.CommodityHistoryList = salist;
            ViewBag.PageCurrent2 = pageIndex;
            ViewBag.Count2 = rowCount;

            return PartialView();
        }

        #endregion

        #region 操作

        public ActionResult AddSettlingAccount(string commodityId, string appId, string manufacturerClearingPrice, string effectiveTime)
        {
            if (string.IsNullOrWhiteSpace(commodityId) || string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(manufacturerClearingPrice) || string.IsNullOrWhiteSpace(effectiveTime))
            {
                return Json(new { Success = true, Messages = "参数不能为空" });
            }

            Guid UserId = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current.LoginUserID;

            SettlingAccountFacade comfa = new SettlingAccountFacade();

            try
            {
                Guid commodityIdTmp = Guid.Parse(commodityId);
                Guid appIdTmp = Guid.Parse(appId);
                Decimal manufacturerClearingPriceTmp = decimal.Parse(manufacturerClearingPrice);
                DateTime effectiveTimeTmp = DateTime.Parse(effectiveTime);

                //获取用户信息
                var jsonr = GetUserNameAndCode(UserId);

                Jinher.AMP.BTP.Deploy.SettlingAccountDTO settlingAccountDTO = new SettlingAccountDTO()
                {
                    Id = Guid.NewGuid(),
                    CommodityId = commodityIdTmp,
                    ManufacturerClearingPrice = manufacturerClearingPriceTmp,
                    AppId = appIdTmp,
                    Effectable = 1,
                    EffectiveTime = effectiveTimeTmp,
                    SubId = UserId,
                    SubName = jsonr.Item1,
                    UserCode = jsonr.Item2
                };

                //添加
                ResultDTO result = comfa.SaveSettlingAccount(settlingAccountDTO);

                if (result.ResultCode == 0)
                {
                    return Json(new { Success = true, Messages = result.Message });
                }
                else
                {
                    return Json(new { Success = false, Messages = result.Message });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Messages = ex.Message });
            }

        }

        public ActionResult DeleteSettlingAccount(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { Success = true, Messages = "参数不能为空" });
            }
            Guid ids = new Guid();
            Guid.TryParse(id, out ids);
            List<Guid> idList = new List<Guid>();
            idList.Add(ids);
            SettlingAccountFacade comfa = new SettlingAccountFacade();
            try
            {
                ResultDTO result = comfa.DeleteSettlingAccountById(idList);
                if (result.ResultCode == 0)
                {
                    return Json(new { Success = true, Messages = result.Message });
                }
                else
                {
                    return Json(new { Success = false, Messages = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Messages = ex.Message });
            }
        }

        #endregion

        #region 用户信息
        /// <summary>
        /// 获取用户信息中的用户名和昵称。
        /// </summary>
        private Tuple<string, string> GetUserNameAndCode(Guid userId)
        {
            string uName = "--";
            string uCode = "--";
            Tuple<string, string> tuple = new Tuple<string, string>(uName, uCode);

            var userNamelist = CBCSV.Instance.GetUserNameAccountsByIds(new List<Guid> { userId });
            if (userNamelist == null)
            {
                return tuple;
            }
            if (!userNamelist.Any())
            {
                return tuple;
            }

            var user = userNamelist.First();
            uName = user.userName;
            if (user.Accounts == null || (!user.Accounts.Any()))
            {
                return tuple;
            }
            //取手机号，如果手机号为空取 邮箱， 还为空，随便取
            var acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && !c.Account.Contains('@'));
            if (acc == null)
            {
                acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && c.Account.Contains('@'));
                if (acc == null)
                {
                    acc = user.Accounts.FirstOrDefault(c => !string.IsNullOrEmpty(c.Account));
                }
            }

            if (acc != null)
            {
                uCode = acc.Account;
            }
            tuple = new Tuple<string, string>(uName, uCode);
            return tuple;

        }
        #endregion
    }
}
