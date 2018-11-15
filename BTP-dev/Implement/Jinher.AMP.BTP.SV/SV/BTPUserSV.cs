
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/4/18 14:07:57
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
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
    public partial class BTPUserSV : BaseSv, IBTPUser
    {

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDTO">用户信息DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateUser(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO userDTO)
        {
            base.Do();
            return this.UpdateUserExt(userDTO);

        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO GetUser(System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            return this.GetUserExt(userId, appId);

        }
        /// <summary>
        /// 待自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>待自提订单数量</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> GetSelfTakeManager(Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetSelfTakeManagerExt(userId);
            timer.Stop();
            LogHelper.Debug(string.Format("BTPUserSV.GetSelfTakeManager：耗时：{0}。入参：userId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, userId, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> GetVipInfo(Guid userId, Guid appId)
        {
            base.Do(false);
            return this.GetVipInfoExt(userId, appId);
        }
    }
}
