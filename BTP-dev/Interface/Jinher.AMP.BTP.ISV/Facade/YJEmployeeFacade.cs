
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/7/26 14:46:18
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
    public class YJEmployeeFacade : BaseFacade<IYJEmployee>
    {

        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfo()
        {
            base.Do();
            return this.Command.UpdataYJEmployeeInfo();
        }
        /// <summary>
        /// 获取员工code
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeCodeDTO> GetUserCodeByAcccount(System.Guid AppId, System.Guid UserId, string UserAccount)
        {
            base.Do();
            return this.Command.GetUserCodeByAcccount(AppId, UserId, UserAccount);
        }
    }
}