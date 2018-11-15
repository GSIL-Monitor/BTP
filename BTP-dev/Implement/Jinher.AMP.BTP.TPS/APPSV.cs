using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.App.ISV.Facade;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.App.Deploy.Enum;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Cache;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */
    public class APPSV : OutSideServiceBase<APPSVFacade>
    {
        /// <summary>
        /// 获取应用名称
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetAppName(Guid appId)
        {
            string result = "";
            try
            {
                Dictionary<Guid, string> dictApp = GetAppNameListByIds(new List<Guid> { appId });

                if (dictApp != null && dictApp.ContainsKey(appId))
                {
                    result = dictApp[appId];
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppName服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取应用图标
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetAppIcon(Guid appId)
        {
            string result = string.Empty;
            var dict = Instance.GetAppDictByIds(new List<Guid> { appId });
            if (dict != null && dict.ContainsKey(appId) && dict[appId] != null)
            {
                result = dict[appId].AppIcon;
            }
            return result;
        }


        /// <summary>
        ///根据AppID判断Redis中是否存在App名称，若存在，则从Redis中取出，否则调用接口取出.
        /// </summary>
        /// <param name="listAppIds">AppId列表</param>
        /// <returns>App列表名称信息</returns>
        public static Dictionary<Guid, string> GetAppNameListByIds(List<Guid> listAppIds)
        {
            Dictionary<Guid, string> result = new Dictionary<Guid, string>();
            var dict = Instance.GetAppDictByIds(listAppIds, null);
            if (!dict.Any())
                return result;
            foreach (var item in dict)
            {
                if (!result.ContainsKey(item.Key) && item.Value != null && !string.IsNullOrEmpty(item.Value.AppName))
                    result.Add(item.Key, item.Value.AppName);
            }
            return result;
        }
        /// <summary>
        /// 获取多个应用数据
        /// </summary>
        /// <param name="appIds"></param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        public static List<AppIdNameIconDTO> GetAppListByIds(List<Guid> appIds, ContextDTO contextDTO = null)
        {
            List<AppIdNameIconDTO> result = new List<AppIdNameIconDTO>();

            var dictApp = Instance.GetAppDictByIds(appIds, contextDTO);
            if (dictApp == null || !dictApp.Any())
                return result;
            return dictApp.Values.ToList();
        }
        /// <summary>
        /// 获取应用名称、图标
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static AppIdNameIconDTO GetAppNameIcon(Guid appId)
        {
            AppIdNameIconDTO result = new AppIdNameIconDTO();
            var dictApp = Instance.GetAppDictByIds(new List<Guid>() { appId });
            if (dictApp != null && dictApp.Any())
                result = dictApp.First().Value;
            return result;
        }
        /// <summary>
        /// 获取皮肤
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static int GetSkinType(Guid appId)
        {
            int result = 0;
            var appPackFaceStartImgDTO = Instance.GetAppPackFaceStartImg(appId);
            if (appPackFaceStartImgDTO != null && appPackFaceStartImgDTO.IsSuccess)
            {
                result = appPackFaceStartImgDTO.FaceStyle;
            }

            return result;
        }
    }

    public class APPSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取应用主信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppIdOwnerIdTypeDTO GetAppOwnerInfo(System.Guid appId, ContextDTO contextDTO = null)
        {
            AppIdOwnerIdTypeDTO applicationDTO = null;
            contextDTO = contextDTO ?? AuthorizeHelper.CoinInitAuthorizeInfo();
            try
            {
                applicationDTO = RedisHelper.GetHashValue<AppIdOwnerIdTypeDTO>(RedisKeyConst.AppOwnerType, appId.ToString());
                if (applicationDTO != null && applicationDTO.OwnerId != Guid.Empty)
                {
                    return applicationDTO;
                }

                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppOwnerType(appId);
                if (applicationDTO != null)
                    RedisHelper.AddHash(RedisKeyConst.AppOwnerType, appId.ToString(), applicationDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppOwnerInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }

        /// <summary>
        /// 获取应用主信息
        /// </summary>
        ///<param name="appIds">appid列表</param>
        /// <param name="contextDTO">用户登录上下文</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<AppOwnerTypeDTO> GetAppOwnerTypeList(List<Guid> appIds, ContextDTO contextDTO = null)
        {
            List<AppOwnerTypeDTO> appOwnerList = new List<AppOwnerTypeDTO>();

            contextDTO = contextDTO ?? AuthorizeHelper.CoinInitAuthorizeInfo();
            try
            { 
                ShareInfoFacade appManagerFacade = new ShareInfoFacade();
                appManagerFacade.ContextDTO = contextDTO;
                appOwnerList = appManagerFacade.GetAppOwnerTypeList(appIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppOwnerTypeList服务异常。 appIds：{0}",JsonConvert.SerializeObject(appIds)), ex);
            }
            return appOwnerList;
        }

        ///// <summary>
        ///// 获取多个应用数据
        ///// </summary>
        ///// <param name="appIds"></param>
        ///// <param name="contextDTO"></param>
        ///// <returns></returns>
        //[BTPAopLogMethod]
        //public Dictionary<Guid, AppIdNameIconDTO> GetAppDictByIds(List<Guid> appIds, ContextDTO contextDTO = null)
        //{
        //    Dictionary<Guid, AppIdNameIconDTO> result = new Dictionary<Guid, AppIdNameIconDTO>();
        //    if (appIds == null || !appIds.Any())
        //        return result;
        //    appIds = appIds.Distinct().ToList();
        //    try
        //    {

        //        List<Guid> redisNotExistappIdsList = new List<Guid>();
        //        //var dict = RedisHelper.GetHashInfoList<AppIdNameIconDTO>(RedisKeyConst.AppNameIcon, appIds.Select(c => c.ToString()).ToList());
        //        foreach (Guid appId in appIds)
        //        {
        //            //    if (dict.ContainsKey(appId.ToString()) && dict[appId.ToString()] != null)
        //            //        result.Add(appId, dict[appId.ToString()]);
        //            //    else
        //            redisNotExistappIdsList.Add(appId);
        //        }
        //        //if (redisNotExistappIdsList.Any())
        //        //{
        //        //LogHelper.Info("GetAppDictByIds中调用接口GetAppListByIds方法的AppId为:" + string.Join(",", redisNotExistappIdsList));
        //        //List<KeyValuePair<string, AppIdNameIconDTO>> keyValuePairs = new List<KeyValuePair<string, AppIdNameIconDTO>>();
        //        App.ISV.Facade.AppManagerFacade appMFacade = new App.ISV.Facade.AppManagerFacade();
        //        appMFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
        //        List<AppIdNameIconDTO> appExistAppNamelist = appMFacade.GetOldAppListByIds(redisNotExistappIdsList);
        //        if (appExistAppNamelist.Any())
        //        {
        //            foreach (AppIdNameIconDTO appIdNameIconDTO in appExistAppNamelist)
        //            {
        //                //KeyValuePair<string, AppIdNameIconDTO> keyValuePair = new KeyValuePair<string, AppIdNameIconDTO>(appIdNameIconDTO.AppId.ToString(), appIdNameIconDTO);
        //                result.Add(appIdNameIconDTO.AppId, appIdNameIconDTO);
        //                //keyValuePairs.Add(keyValuePair);
        //            }
        //            //RedisHelper.SetRangeInHash(RedisKeyConst.AppNameIcon, keyValuePairs);
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(string.Format("APPSV.GetAppDictByIds服务异常:获取应用信息异常。 appIds：{0}", appIds), ex);
        //    }
        //    return result;
        //}

        /// <summary>
        /// 获取多个应用数据
        /// </summary>
        /// <param name="appIds"></param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Dictionary<Guid, AppIdNameIconDTO> GetAppDictByIds(List<Guid> appIds, ContextDTO contextDTO = null)
        {
            Dictionary<Guid, AppIdNameIconDTO> result = new Dictionary<Guid, AppIdNameIconDTO>();
            if (appIds == null || !appIds.Any())
                return result;
            appIds = appIds.Distinct().ToList();
            try
            {

                List<Guid> redisNotExistappIdsList = new List<Guid>();
                var dict = RedisHelper.GetHashInfoList<AppIdNameIconDTO>(RedisKeyConst.AppNameIcon, appIds.Select(c => c.ToString()).ToList());
                foreach (Guid appId in appIds)
                {
                    if (dict.ContainsKey(appId.ToString()) && dict[appId.ToString()] != null)
                        result.Add(appId, dict[appId.ToString()]);
                    else
                        redisNotExistappIdsList.Add(appId);
                }
                if (redisNotExistappIdsList.Any())
                {
                    LogHelper.Info("GetAppDictByIds中调用接口GetAppListByIds方法的AppId为:" + string.Join(",", redisNotExistappIdsList));
                    List<KeyValuePair<string, AppIdNameIconDTO>> keyValuePairs = new List<KeyValuePair<string, AppIdNameIconDTO>>();
                    App.ISV.Facade.AppManagerFacade appMFacade = new App.ISV.Facade.AppManagerFacade();
                    appMFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                    List<AppIdNameIconDTO> appExistAppNamelist = appMFacade.GetOldAppListByIds(redisNotExistappIdsList);
                    if (appExistAppNamelist.Any())
                    {
                        foreach (AppIdNameIconDTO appIdNameIconDTO in appExistAppNamelist)
                        {
                            KeyValuePair<string, AppIdNameIconDTO> keyValuePair = new KeyValuePair<string, AppIdNameIconDTO>(appIdNameIconDTO.AppId.ToString(), appIdNameIconDTO);
                            result.Add(appIdNameIconDTO.AppId, appIdNameIconDTO);
                            keyValuePairs.Add(keyValuePair);
                        }
                        RedisHelper.SetRangeInHash(RedisKeyConst.AppNameIcon, keyValuePairs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppDictByIds服务异常:获取应用信息异常。 appIds：{0}", appIds), ex);
            }
            return result;
        }

        /// <summary>
        /// 获取应用分享信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppShareDTO GetAppShareContent(Guid appId)
        {
            AppShareDTO result = new AppShareDTO();

            if (appId == Guid.Empty)
                return result;
            try
            {
                Jinher.AMP.App.ISV.Facade.ShareInfoFacade facade = new ShareInfoFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var shareInfo = facade.GetAppShareContent(appId);
                return convertToAppShareDTO(shareInfo);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取应用名称异常,AppId:{0}", appId), ex);
            }
            return result;

        }
        private AppShareDTO convertToAppShareDTO(AppShareInfoDTO dto)
        {
            AppShareDTO result = new AppShareDTO();
            if (dto != null && dto.IsSuccess)
            {
                result.Icon = dto.Icon;
                result.ShareContent = dto.ShareContent;
                result.ShareDesc = dto.ShareDesc;
                result.ShareGotoUrl = dto.ShareGotoUrl;
                result.ShareMessSrc = dto.ShareMessSrc;
                result.ShareTopic = dto.ShareTopic;
            }
            return result;
        }
        /// <summary>
        /// 获取最新应用详情
        /// </summary>
        /// <param name="appId">应用ID(以逗号隔开)</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO GetNewAppById(System.Guid appId)
        {
            Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO applicationDTO = new App.Deploy.NewCustomDTO.ApplicationDTO();

            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetNewAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetNewAppById服务异常:获取应用信息异常。 appId:{0}", appId), ex);
            }
            return applicationDTO;
        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppIdNameIconDTO GetAppDetailById(Guid appId)
        {
            AppIdNameIconDTO applicationDTO = null;
            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppDetailById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppDetailById服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }

        /// <summary>
        /// 获取App的级别信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppLevelDTO GetAppLevelInfo(string appId)
        {
            AppLevelDTO applicationDTO = null;
            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppLevelInfo(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppLevelInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }

        [BTPAopLogMethod]
        public List<AppIdNameIconDTO> GetAppListByIdsInfo(List<Guid> appIds)
        {
            List<AppIdNameIconDTO> applicationDTO = null;
            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetOldAppListByIds(appIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppListByIdsInfo服务异常:获取应用信息异常。 appIds：{0}", appIds), ex);
            }
            return applicationDTO;
        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppDetailAndPackageDTO GetAppDetailAndPackage(System.Guid appId, ContextDTO contextDTO)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.AppDetailAndPackageDTO result = null;

            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade appManagerFacade = new Jinher.AMP.App.ISV.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                AppDetailAndPackage applicationDTO = appManagerFacade.GetAppDetailAndPackage(appId);

                if (applicationDTO != null)
                {
                    result = new AppDetailAndPackageDTO();
                    result.DownLoad = applicationDTO.DownLoad;
                    result.Icon = applicationDTO.Icon;
                    result.Id = applicationDTO.Id;
                    result.Illegal = applicationDTO.Illegal;
                    result.Name = applicationDTO.Name;
                    result.OffShelves = applicationDTO.OffShelves;
                    result.QRCodeUrl = applicationDTO.QRCodeUrl;

                    if (applicationDTO.PackageUrl != null)
                    {
                        //苹果的下载地址 只取上架的
                        if (applicationDTO.PackageUrl.ContainsKey("ios"))
                        {
                            var iosDownLoadUrl = applicationDTO.PackageUrl["ios"].Trim();
                            if (!iosDownLoadUrl.Contains(";") || iosDownLoadUrl.Contains(";") && iosDownLoadUrl.Contains(".plist") || iosDownLoadUrl.Contains(";") && iosDownLoadUrl.Contains("itunes.apple.com"))
                            {
                                if (iosDownLoadUrl.Contains(";"))
                                {
                                    result.IosUrl = iosDownLoadUrl.Split(';')[1];
                                }
                                else
                                {
                                    result.IosUrl = iosDownLoadUrl;
                                }
                            }
                        }
                        //Android的下载地址
                        if (applicationDTO.PackageUrl.ContainsKey("android"))
                        {
                            result.AndroidUrl = applicationDTO.PackageUrl["android"].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppDetailAndPackage服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return result;
        }


        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contextDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.App.Deploy.ApplicationDTO GetAppByIdInfo(System.Guid appId, ContextDTO contextDTO)
        {
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = null;
            try
            {
                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppByIdInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }
        [BTPAopLogMethod]
        public AppPackageDetailListDTO GetAppInfo(Guid appId, ContextDTO contextDTO)
        {
            AppPackageDetailListDTO applicationDTO = null;
            try
            {
                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppPackageDetailsWithHostTypeByAppId(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }
        [BTPAopLogMethod]
        public AppIdNameIconDTO GetAppDetailByIdInfo(Guid appId, ContextDTO contextDTO)
        {
            AppIdNameIconDTO applicationDTO = null;
            try
            {
                AppManagerFacade appManagerFacade = new AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppDetailById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetAppDetailByIdInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }
        /// <summary>
        /// 根据应用主Id获取应用信息
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Jinher.AMP.App.Deploy.ApplicationDTO> GetApplicationByOwnId(Guid ownerId)
        {
            List<Jinher.AMP.App.Deploy.ApplicationDTO> appsList = new List<App.Deploy.ApplicationDTO>();
            try
            {
                Jinher.AMP.App.ISV.Facade.AppManagerFacade facade = new Jinher.AMP.App.ISV.Facade.AppManagerFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                appsList = facade.GetApplicationByOwnId(ownerId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPSV.GetApplicationByOwnId服务异常:根据应用主获取应用信息。 ownerId：{0}", ownerId), ex);
            }
            return appsList;
        }
        /// <summary>
        /// 皮肤接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.App.Deploy.CustomDTO.AppPackFaceStartImgDTO GetAppPackFaceStartImg(System.Guid appId)
        {
            Jinher.AMP.App.Deploy.CustomDTO.AppPackFaceStartImgDTO appPackFaceStartImgDTO = null;
            try
            {
                Jinher.AMP.App.ISV.Facade.StartImgFacade startImgFacade = new StartImgFacade();
                startImgFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                appPackFaceStartImgDTO = startImgFacade.GetAppFaceAndStartImgByAppId(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPBP.GetAppPackFaceStartImg服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return appPackFaceStartImgDTO;
        }
    }

}
