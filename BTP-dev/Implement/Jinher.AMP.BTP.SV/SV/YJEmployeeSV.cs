
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/26 14:46:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class YJEmployeeSV : BaseSv, IYJEmployee
    {

        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfo()
        {
            base.Do(false);
            return this.UpdataYJEmployeeInfoExt();

        }
        /// <summary>
        /// 获取员工code
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> GetUserCodeByAcccount(System.Guid AppId, System.Guid UserId, string UserAccount)
        {
            base.Do(false);
            return this.GetUserCodeByAcccountExt(AppId, UserId, UserAccount);

        }
    }
}