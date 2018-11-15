
/***************
功能描述: BTP-setSV
作    者: 
创建时间: 2015/11/21 15:50:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AppExtensionSV : BaseSv, IAppExtension
    {

        /// <summary>
        /// 查询电商应用级信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtDTO GetBTPAppInfo(Jinher.AMP.BTP.Deploy.CustomDTO.AppSearchDTO search)
        {
            base.Do(false);
            return this.GetBTPAppInfoExt(search);

        }
        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(System.Guid appId)
        {
            base.Do(false);
            return this.GetAppExtensionByAppIdExt(appId);

        }
        /// <summary>
        /// 获取特定app下载所需数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO> GetAppDownLoadInfo(System.Guid appId)
        {
            base.Do(false);
            return this.GetAppDownLoadInfoExt(appId);

        }
      
    }
}