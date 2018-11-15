
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/1/7 10:39:27
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class AppExtensionFacade : BaseFacade<IAppExtension>
    {

        /// <summary>
        /// 更新应用扩展（店铺扩展）
        /// </summary>
        /// <param name="appExtDTO">应用扩展信息实体</param>
        /// <returns>操作结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppExtension(Jinher.AMP.BTP.Deploy.AppExtensionDTO appExtDTO)
        {
            base.Do();
            return this.Command.UpdateAppExtension(appExtDTO);
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
        /// 获取app的积分抵现设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppScoreSettingDTO GetScoreSetting(System.Guid appId)
        {
            base.Do();
            return this.Command.GetScoreSetting(appId);
        }
        /// <summary>
        /// 设置渠道佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaultChannelAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            base.Do();
            return this.Command.SetDefaultChannelAccount(appExtension);
        }
        /// <summary>
        /// 获取渠道默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulChannelAccount(System.Guid appId)
        {
            base.Do();
            return this.Command.GetDefaulChannelAccount(appId);
        }
        /// <summary>
        /// 获取应用统计信息
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang.AppStatisticsDTO GetAppStatistics(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAppStatistics(appId);
        }
    }
}