
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/18 14:07:54
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
    public class BTPUserFacade : BaseFacade<IBTPUser>
    {
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDTO">用户信息DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateUser(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO userDTO)
        {
            base.Do();
            return this.Command.UpdateUser(userDTO);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO GetUser(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetUser(userId, appId);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO GetSelfTakeManager(Guid userId)
        {
            base.Do();
            return this.Command.GetSelfTakeManager(userId);
        }
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> GetVipInfo(Guid userId, Guid appId)
        {
            base.Do();
            return this.Command.GetVipInfo(userId, appId);
        }
    }
}
