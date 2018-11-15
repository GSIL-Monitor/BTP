
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/1/10 13:42:02
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
    public partial class ShiftManageBP : BaseBP, IShiftManage
    {

        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShiftExchange(Jinher.AMP.BTP.Deploy.CustomDTO.ShiftLogDTO dto)
        {
            base.Do();
            return this.ShiftExchangeExt(dto);
        }
        /// <summary>
        /// 获取交班信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FShiftLogCDTO GetLastShiftInfo(System.Guid appId)
        {
            base.Do();
            return this.GetLastShiftInfoExt(appId);
        }
    }
}