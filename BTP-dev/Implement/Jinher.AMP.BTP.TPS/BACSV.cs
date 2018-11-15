using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BAC.Deploy.BEDTO;
using Jinher.AMP.BAC.Deploy.CustomDTO;
using Jinher.AMP.BAC.IBP.Facade;
using Jinher.AMP.BAC.ISV.Facade;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    /// <summary>
    /// bac外部调用类
    /// </summary>
    public class BACSV : OutSideServiceBase<BACSVFacade>
    {
        /// <summary>
        /// 河套版式
        /// </summary>
        public const string HetaoLayoutKey = "22";
        /// <summary>
        /// 获取定制应用底部菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<BacMenuForMobileDTO> GetAppBottomMenus(Guid appId, Guid userId)
        {
            const string naviCode = "BOTTOM_MENU";
            return Instance.GetAppMenuByNaviCode(appId, userId, naviCode);
        }

        /// <summary>
        /// 返回定制应用是否在店铺商品列表中显示加入购物车按钮
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        public static bool IsAddShopCartInComList(Guid appId)
        {
            bool result = false;
            const string functionCode = "GoodsListFormatSet";
            const string shopGoodsList = "ShopGoodsList";
            const int valueType = 0;
            var dict = Instance.GetFunctionPeropty(appId, functionCode, valueType);
            if (dict != null && dict.ContainsKey(shopGoodsList) && dict[shopGoodsList] == "2")
                result = true;
            return result;
        }
        /// <summary>
        /// 获取店铺列表相关全局配置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static ComListSettingDTO GetComListSetting(Guid appId)
        {
            ComListSettingDTO result = new ComListSettingDTO();
            const string functionCode = "GoodsListFormatSet";
            const int valueType = 0;
            var dict = Instance.GetFunctionPeropty(appId, functionCode, valueType);
            if (dict == null || !dict.Any())
                return result;
            const string shopGoodsList = "ShopGoodsList";
            const string goodsDefaultFormat = "GoodsDefaultFormat";
            if (dict.ContainsKey(shopGoodsList) && dict[shopGoodsList] == "2")
                result.IsAddShopCartInComList = true;
            if (dict.ContainsKey(goodsDefaultFormat) && dict[goodsDefaultFormat] == "1")
                result.GoodsDefaultFormat = 1;
            return result;
        }


        /// <summary>
        /// 获得app推广下载引导语
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetPromotionDownGuide(Guid appId)
        {
            string result = string.Empty;
            var dict = Instance.GetAppFunctionPeropty(appId, "share");
            if (dict != null && dict.ContainsKey("PromotionDownGuide"))
            {
                result = dict["PromotionDownGuide"];
            }
            return result;
        }
        /// <summary>
        /// 获取应用菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> GetFunctionUsed(Guid appId, List<string> functionCodes)
        {

            Dictionary<string, bool> result = new Dictionary<string, bool>();
            if (functionCodes == null || !functionCodes.Any())
                return result;
            var list = Instance.GetFunctionUsedInfo(appId, functionCodes);
            bool isUsed;
            foreach (var functionCode in functionCodes)
            {
                if (!result.ContainsKey(functionCode))
                {
                    isUsed = false;
                    var function = list.FirstOrDefault(c => c.Code == functionCode);
                    if (function != null && function.IsUsedByApp)
                        isUsed = true;
                    result.Add(functionCode, isUsed);
                }
            }
            return result;
        }
        /// <summary>
        /// 河套版式
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsHeTaoFormat(Guid appId)
        {
            if (Instance.GetAppLayoutCode(appId) == HetaoLayoutKey)
                return true;
            return false;
        }
    }

    public class BACSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取应用菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<AppMenuChangeInfoDTO> GetAppMenuChangeInfo(Guid appId)
        {
            List<AppMenuChangeInfoDTO> result = new List<AppMenuChangeInfoDTO>();
            if (appId == Guid.Empty)
                return result;
            Jinher.AMP.BAC.IBP.Facade.CreatAppFacade facade = new CreatAppFacade();
            try
            {
                result = facade.GetAppMenuChangeInfo(appId);


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetAppMenuChangeInfo服务异常。appId:{0} ", appId), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取应用菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<BacFunctionMenuDTO> GetFunctionUsedInfo(Guid appId, List<string> functionCodes)
        {
            var result = new List<BacFunctionMenuDTO>();
            if (appId == Guid.Empty || functionCodes == null || !functionCodes.Any())
                return result;
            Jinher.AMP.BAC.ISV.Facade.AppDataServiceFacade facade = new AppDataServiceFacade();
            try
            {

                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                result = facade.GetFunctionUsedInfo(appId, functionCodes);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetFunctionUsedInfo服务异常。appId:{0},functionCodes:{1} ", appId, functionCodes), ex);
            }
            return result ?? new List<BacFunctionMenuDTO>();
        }
        /// <summary>
        /// 获取区域菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="naviCode"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<BacMenuForMobileDTO> GetAppMenuByNaviCode(Guid appId, Guid userId, string naviCode)
        {
            List<BacMenuForMobileDTO> result = new List<BacMenuForMobileDTO>();
            try
            {
                AppDataServiceFacade facade = new AppDataServiceFacade();
                //facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                result = facade.GetAppMenuByNaviCodeWithUserId(appId, naviCode, userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("BACSV.GetAppMenuByNaviCode异常，异常信息", ex);
            }
            return result ?? new List<BacMenuForMobileDTO>();
        }


        /// <summary>
        /// 获取单个菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="menuCode"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public BacMenuForMobileDTO GetAppSingleMenuInfo(Guid appId, string menuCode)
        {
            var result = new BacMenuForMobileDTO();
            if (appId == Guid.Empty || string.IsNullOrEmpty(menuCode))
                return result;
            Jinher.AMP.BAC.ISV.Facade.AppDataServiceFacade facade = new AppDataServiceFacade();
            try
            {

                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                result = facade.GetAppSingleMenuInfo(appId, menuCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetAppSingleMenuInfo服务异常。appId:{0},menuCode:{1} ", appId, menuCode), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取商品列表模板
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="functionCode">"GoodsListFormatSet" 商品列表</param>
        /// <param name="valueType">0</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Dictionary<string, string> GetFunctionPeropty(Guid appId, string functionCode, int valueType)
        {
            var result = new Dictionary<string, string>();
            if (appId == Guid.Empty || string.IsNullOrWhiteSpace(functionCode))
                return result;
            Jinher.AMP.BAC.ISV.Facade.AppDataServiceFacade facade = new AppDataServiceFacade();
            try
            {
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                result = facade.GetFunctionPeropty(appId, functionCode, valueType);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetFunctionPeropty服务异常。appId:{0},functionCode:{1},valutType:{2} ", appId, functionCode, valueType), ex);
            }
            return result;
        }
        [BTPAopLogMethod]
        public Dictionary<string, string> GetAppFunctionPeropty(Guid appId, string functionCode)
        {
            var result = new Dictionary<string, string>();
            if (appId == Guid.Empty || string.IsNullOrWhiteSpace(functionCode))
                return result;
            Jinher.AMP.BAC.ISV.Facade.AppDataServiceFacade facade = new AppDataServiceFacade();
            try
            {
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                result = facade.GetAppFunctionPeropty(appId, functionCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetAppFunctionPeropty服务异常。appId:{0},functionCode:{1} ", appId, functionCode), ex);
            }
            return result;
        }
        /// <summary>
        /// 获得app推广下载引导语
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GetAppLayoutCode(Guid appId)
        {
            string result = "0";
            try
            {
                AppDataServiceFacade facade = new AppDataServiceFacade();
                result = facade.GetAppLayoutCode(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BACSV.GetAppLayoutCode服务异常。appId:{0}  ", appId), ex);
            }
            return result;
        }
    }



}

