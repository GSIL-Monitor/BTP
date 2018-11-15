using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.App.Deploy.NewCustomDTO;
using Jinher.AMP.App.IBP.Facade;
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

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */
    public class APPBP : OutSideServiceBase<APPFacade>
    {
        /// <summary>
        /// 校验当前app是否为定制app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsFittedApp(Guid appId)
        {
            var appInfo = Instance.GetAppById(appId);
            return appInfo != null && appInfo.TemplateId == 8;
        }
    }

    public class APPFacade : OutSideFacadeBase
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
            contextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
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
        /// 根据应用ID和应用类型获取应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppPackageDetailListDTO GetAppPackageDetailsWithHostTypeByAppId(System.Guid appId)
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
                LogHelper.Error(string.Format("APPSV.GetAppOwnerInfo服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }

        /// <summary>
        /// 根据应用名称和分类Id和应用模板查询应用
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="categoryId"></param>
        /// <param name="template"></param>
        /// <param name="usercode"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagenum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO> GetNewAppByNameOrCategoryOrTemplate(string appName, Guid? categoryId, AppTemplateEnum? template, string usercode, int pagesize, int pagenum, out int count)
        {
            List<Jinher.AMP.App.Deploy.NewCustomDTO.ApplicationDTO> result = new List<ApplicationDTO>();
            count = 0;
            try
            {
                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = appManagerFacade.GetNewAppByNameOrCategoryOrTemplate(appName, categoryId, template, usercode, pagesize, pagenum, out count);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPBP.GetNewAppByNameOrCategoryOrTemplate服务异常:根据应用名称和分类Id和应用模板查询应用异常。 appName :{0}, categoryId :{1} , template  :{2}, usercode  :{3}, pagesize  :{4}, pagenum  :{5}", appName, categoryId, template, usercode, pagesize, pagenum), ex);
            }
            return result;
        }
        /// <summary>
        /// 通过应用标识获取应用信息
        /// </summary>
        /// <param name="appId">应用标识</param>
        /// <returns>应用信息</returns>
        [BTPAopLogMethod]
        public Jinher.AMP.App.Deploy.ApplicationDTO GetAppById(System.Guid appId)
        {
            Jinher.AMP.App.Deploy.ApplicationDTO applicationDTO = null;
            try
            {
                //***********   暂时注释调试 
                //applicationDTO = GlobalCacheWrapper.GetData(RedisKeyConst.AppInfo, appId.ToString(), CacheTypeEnum.redisSS, "BTPCache") as Jinher.AMP.App.Deploy.ApplicationDTO;
                //if (applicationDTO != null)
                //{
                //    return applicationDTO;
                //}
                //Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                //appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                //applicationDTO = appManagerFacade.GetAppById(appId);
                //if (applicationDTO != null)
                //{
                //    string json = JsonHelper.JsonSerializer<Jinher.AMP.App.Deploy.ApplicationDTO>(applicationDTO);
                //    GlobalCacheWrapper.Add(RedisKeyConst.AppInfo, appId.ToString(), json, CacheTypeEnum.redisSS, "BTPCache");
                //}



                Jinher.AMP.App.IBP.Facade.AppManagerFacade appManagerFacade = new App.IBP.Facade.AppManagerFacade();
                appManagerFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                applicationDTO = appManagerFacade.GetAppById(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("APPBP.GetAppById服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return applicationDTO;
        }
    }


}
