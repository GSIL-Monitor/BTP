
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/1/7 10:39:28
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AppExtensionBP : BaseBP, IAppExtension
    {

        /// <summary>
        /// 更新应用扩展（店铺扩展）
        /// </summary>
        /// <param name="appExtDTO">应用扩展信息实体</param>
        /// <returns>操作结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppExtension(Jinher.AMP.BTP.Deploy.AppExtensionDTO appExtDTO)
        {
            base.Do();
            return this.UpdateAppExtensionExt(appExtDTO);
        }
        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetAppExtensionByAppIdExt(appId);
        }
        /// <summary>
        /// 获取app的积分抵现设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppScoreSettingDTO GetScoreSetting(System.Guid appId)
        {
            base.Do();
            return this.GetScoreSettingExt(appId);
        }
        /// <summary>
        /// 设置渠道佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaultChannelAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            base.Do(false);
            return this.SetDefaultChannelAccountExt(appExtension);
        }
        /// <summary>
        /// 获取渠道默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulChannelAccount(System.Guid appId)
        {
            base.Do(false);
            return this.GetDefaulChannelAccountExt(appId);
        }
        /// <summary>
        /// 获取应用统计信息
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang.AppStatisticsDTO GetAppStatistics(System.Guid appId)
        {
            base.Do(false);
            return this.GetAppStatisticsExt(appId);
        }
    }
}