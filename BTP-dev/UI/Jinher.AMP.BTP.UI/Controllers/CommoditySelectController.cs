using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.UI.Models;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.App.Deploy.Enum;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CommoditySelectController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            Guid ownerid = Getownerid();
            //自己的APP
            List<AppModel> myAPPList = GetMyAppList(ownerid, true);
            ViewBag.CallBack = Request["callBack"];
            ViewBag.MyAppList = myAPPList;
            ViewBag.CommodityUrl = CustomConfig.AommodityUrl;

            return View();
        }

        [GridAction]
        public ActionResult SearchApp(string appName, Guid? categoryId, string plateCode, int rows, int page, Guid? appId)
        {
            int totalCount = 0;
            string usercode = null;
            List<Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO> appList;
            List<AppModel> modelList = new List<AppModel>();
            string appDownloadUrl = CustomConfig.AppDownloadUrl;
            if (!string.IsNullOrEmpty(appName))
            {

                AppTemplateEnum? template;
                switch (plateCode)
                {
                    case "1": template = AppTemplateEnum.ImageText;
                        break;
                    case "3": template = AppTemplateEnum.Audio;
                        break;
                    case "4": template = AppTemplateEnum.Trade;
                        break;
                    case "5": template = AppTemplateEnum.Social;
                        break;
                    default: template = null;
                        break;
                }

                if (string.IsNullOrEmpty(appName))
                {
                    appName = null;
                }

                appList = APPBP.Instance.GetNewAppByNameOrCategoryOrTemplate(appName, categoryId, template, usercode, rows, page, out totalCount);
                if (appList != null && appList.Any())
                {
                    foreach (var item in appList)
                    {
                        modelList.Add(new AppModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Category = item.CategoryName,
                            Descript = item.Description,
                            CreatedDate = item.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            ShareUrl = appDownloadUrl + item.Id,
                            AdProduct = string.Empty,
                        });
                    }
                }

            }
            else if (appId != null)
            {
                try
                {
                    Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO app = APPSV.Instance.GetNewAppById((Guid)appId);
                    if (app != null)
                    {
                        modelList.Add(new AppModel()
                        {
                            Id = app.Id,
                            Name = app.Name,
                            Category = app.CategoryName,
                            Descript = app.Description,
                            CreatedDate = app.SubTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            ShareUrl = appDownloadUrl + app.Id,
                            AdProduct = string.Empty,
                        });

                        totalCount = 1;
                    }
                }
                catch (Exception ex)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取App异常 。appName：{0}，categoryId：{1}，plateCode：{2}，rows：{3}，page：{4}，appId：{5}", appName, categoryId, plateCode, rows, page, appId), ex);
                    JAP.Common.Loging.LogHelper.Info("Error_LogKey:CommoditySelectController.SearchApp:App.ISV.Facade.AppManagerFacade.GetNewAppById");
                }
            }

            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Name");
            show.Add("Category");
            show.Add("Descript");
            show.Add("CreatedDate");
            show.Add("ShareUrl");

            return View(new GridModel<AppModel>(show, modelList, totalCount, page, rows, string.Empty));
        }

        [GridAction]
        public ActionResult SearchGoods(string name, string category, int rows, int page, string appIds)
        {
            int totalCount = 0;
            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchForAppsResultDTO result = null;
            List<CommodityModel> modelList = new List<CommodityModel>();
            if (appIds != null)
            {
                Guid tempId;
                List<Guid> appIdList = new List<Guid>();
                string[] appIdStrList = appIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in appIdStrList)
                {
                    if (Guid.TryParse(item, out tempId))
                    {
                        appIdList.Add(tempId);
                    }
                }
                if (appIdList.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Empty;
                        }
                        if (string.IsNullOrEmpty(category))
                        {
                            category = string.Empty;
                        }
                        Jinher.JAP.Common.Loging.LogHelper.Info("获取商品列表参数 appIds:" + appIds + "，category：" + category + ",name:" + name + ",page:" + page.ToString() + ",rows:" + rows.ToString());

                        Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new BTP.ISV.Facade.CommodityFacade();
                        result = facade.CommoditySearchFromApps(appIdList, category, name, page, rows);
                    }
                    catch (Exception ex)
                    {
                        Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取商品列表异常。appIdList：{0}，category：{1}，name：{2}，page：{3}，rows：{4}", JsonHelper.JsonSerializer(appIdList), category, name, page, rows), ex);
                    }
                }
            }
            if (result != null && result.CommodityList != null)
            {
                totalCount = result.TotalCount;
                foreach (var item in result.CommodityList)
                {
                    modelList.Add(new CommodityModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Category = item.CommodityCategory,
                        Pic = item.Pic,
                        Price = item.Price,
                        Intensity = item.Intensity.ToString("#.#"),
                        AppId = item.AppId
                    });
                }
            }

            #region TestData
            /*
            if (modelList.Count == 0) {
                modelList.Add(new GoodsModel()
                {
                    Id = Guid.NewGuid(),
                    Name = "测试数据111",
                    Category = "类别1",
                    Pic = "http://testfileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f9650/TempDirectory/791ce063-b895-4dba-8c63-9bacdd1ec2c7_c531afdf-97a8-44e2-b273-aafa50ac57b8.jpg"
                });
                modelList.Add(new GoodsModel()
                {
                    Id = Guid.NewGuid(),
                    Name = "测试数据222",
                    Category = "类别2",
                    Pic = "http://testfileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f9650/TempDirectory/b011f2eb-72d5-4653-87ab-5166aa74e201_3a7348d8-04e8-4830-85c2-295ca60fba9e.jpg"
                });
                modelList.Add(new GoodsModel()
                {
                    Id = Guid.NewGuid(),
                    Name = "测试数据333",
                    Category = "类别3",
                    Pic = "http://testfileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f9650/TempDirectory/1e23865f-12f1-4013-a096-4c4fb960d9d4_c83c468c-da4a-4fba-8b86-8d1259298e34.jpg"
                });
            }
            */
            #endregion

            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("Name");
            show.Add("Category");
            show.Add("Pic");
            show.Add("Price");
            show.Add("Intensity");
            show.Add("AppId");
            var gridobj = new GridModel<CommodityModel>(show, modelList, totalCount, page, rows, string.Empty);
            return View(gridobj);
        }

        /// <summary>
        /// 获取用户级别
        /// </summary>
        /// <returns></returns>
        private Guid Getownerid()
        {
            Guid ownerid;
            if (ContextDTO.LoginOrg != Guid.Empty)
            {
                ownerid = ContextDTO.LoginOrg;
            }
            else
            {
                ownerid = ContextDTO.LoginUserID;
            }

            return ownerid;
        }

        private List<AppModel> GetMyAppList(Guid ownerid, bool onlyTrade)
        {
            AppModel adApp;
            List<AppModel> myAPPList = new List<AppModel>();
            List<Jinher.AMP.App.Deploy.ApplicationDTO> appsList = APPSV.Instance.GetApplicationByOwnId(ownerid);
            if (appsList != null && appsList.Count > 0)
            {
                foreach (Jinher.AMP.App.Deploy.ApplicationDTO item in appsList)
                {
                    //4 免费电商；8定制应用；
                    //免费电商和定制应用（且选择了电商基础功能）。
                    if (onlyTrade == false || item.TemplateId == 4 || (item.TemplateId == 8 && Jinher.AMP.BTP.TPS.BACBP.CheckTradeBasic(item.Id)))
                    {
                        adApp = new AppModel()
                        {
                            Id = item.Id,
                            Name = item.Name.Length > 8 ? item.Name.Substring(0, 6) + "..." : item.Name,
                        };
                        myAPPList.Add(adApp);
                    }
                }
            }

            return myAPPList;
        }

        public ActionResult TestCommoditySelect()
        {
            return View();
        }
    }
}
