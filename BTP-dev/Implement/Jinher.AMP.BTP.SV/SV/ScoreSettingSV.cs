
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/2/17 11:01:18
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
    public partial class ScoreSettingSV : BaseSv, IScoreSetting
    {

        /// <summary>
        /// 获取特定app在电商中的当前生效的扩展信息。
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> GetScoreSettingByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetScoreSettingByAppIdExt(appId);

        }
        /// <summary>
        /// 获取特定app在电商中的当前生效的积分扩展信息。
        /// </summary>
        /// <param name="userId">当前用户id</param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO> GetUserScoreInApp(Jinher.AMP.BTP.Deploy.CustomDTO.Param2DTO paramDto)
        {
            base.Do(false);
            return this.GetUserScoreInAppExt(paramDto);

        }

        public ResultDTO<OrderScoreCheckResultDTO> OrderScoreCheck(OrderScoreCheckDTO paramDto)
        {
            base.Do(false);
            return this.OrderScoreCheckExt(paramDto);
        }
    }
}