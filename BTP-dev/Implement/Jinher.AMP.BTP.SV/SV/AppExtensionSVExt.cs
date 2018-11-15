
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/11/18 19:46:35
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Cache;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AppExtensionSV : BaseSv, IAppExtension
    {

        /// <summary>
        /// 查询电商应用级信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtDTO GetBTPAppInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSearchDTO search)
        {

            AppExtDTO appDTO = new AppExtDTO();
            appDTO.AppId = search.AppId;
            if (search.AppId != Guid.Empty)
            {
                appDTO.IsZphApp = ZPHSV.Instance.CheckIsAppInZPH(search.AppId);

                var m = (from s in AppExtension.ObjectSet()
                         where search.AppId == s.Id
                         select s.IsShowAddCart).ToList();
                if (m.Any())
                {
                    appDTO.IsShowAddCart = m.FirstOrDefault();
                }
            }

            return appDTO;
        }

        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppIdExt(System.Guid appId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> resultAppExt = new ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO>();

            try
            {
                if (appId == Guid.Empty)
                {
                    resultAppExt.Message = "参数错误，appId不能为空！";
                    resultAppExt.ResultCode = 1;
                }
                var aeList = (from ae in AppExtension.ObjectSet()
                              where ae.Id == appId
                              select ae).ToList();
                if (aeList.Any())
                {
                    var aeFirst = aeList.FirstOrDefault();
                    Jinher.AMP.BTP.Deploy.AppExtensionDTO aeDto = aeFirst.ToEntityData();
                    resultAppExt.Data = aeDto;
                }
            }
            catch (Exception ex)
            {
                string str = string.Format("AppExtensionSV.GetAppExtensionByAppIdExt中发生异常，参数AppId：{0},异常信息：{1}", appId, ex);
                LogHelper.Error(str);

                resultAppExt.Message = "服务异常！";
                resultAppExt.ResultCode = 2;
            }
            return resultAppExt;
        }
        /// <summary>
        /// 获取特定app下载所需数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO<AppDownloadDTO> GetAppDownLoadInfoExt(Guid appId)
        {
            ResultDTO<AppDownloadDTO> result = new ResultDTO<AppDownloadDTO>() { };
            AppDownloadDTO data = new AppDownloadDTO();
            data.AppId = appId;
            data.Icon = APPSV.GetAppIcon(appId);
            data.PromotionDownGuide = BACSV.GetPromotionDownGuide(appId);
            result.Data = data;
            return result;
        }
    }
}
