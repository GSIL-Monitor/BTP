
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/2/17 11:01:17
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ScoreSettingFacade : BaseFacade<IScoreSetting>
    {

        /// <summary>
        /// 获取特定app在电商中的当前生效的扩展信息。
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ScoreSettingDTO> GetScoreSettingByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetScoreSettingByAppId(appId);
        }
        /// <summary>
        /// 获取特定app在电商中的当前生效的积分扩展信息。
        /// </summary>
        /// <param name="userId">当前用户id</param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.UserScoreDTO> GetUserScoreInApp(Jinher.AMP.BTP.Deploy.CustomDTO.Param2DTO paramDto)
        {
            base.Do();
            return this.Command.GetUserScoreInApp(paramDto);
        }
        /// <summary>
        /// 校验下单积分
        /// </summary>
        /// <param name="paramDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderScoreCheckResultDTO> OrderScoreCheck(Jinher.AMP.BTP.Deploy.CustomDTO.OrderScoreCheckDTO paramDto)
        {
            base.Do();
            return this.Command.OrderScoreCheck(paramDto);
        }
    }
}