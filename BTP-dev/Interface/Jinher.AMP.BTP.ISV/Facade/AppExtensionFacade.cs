
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/8/2 15:47:25
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class AppExtensionFacade : BaseFacade<IAppExtension>
    {

        /// <summary>
        /// 查询电商应用级信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtDTO GetBTPAppInfo(Jinher.AMP.BTP.Deploy.CustomDTO.AppSearchDTO search)
        {
            base.Do();
            return this.Command.GetBTPAppInfo(search);
        }
        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAppExtensionByAppId(appId);
        }
        /// <summary>
        /// 获取特定app下载所需数据
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppDownloadDTO> GetAppDownLoadInfo(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAppDownLoadInfo(appId);
        }
       
    }
}