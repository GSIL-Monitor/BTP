
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/4/8 19:46:32
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
    public partial class JdEclpOrderSV : BaseSv, IJdEclpOrder
    {

        /// <summary>
        /// 进销存-同步京东订单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDOrderState(Jinher.AMP.BTP.Deploy.JDEclpOrderJournalDTO arg)
        {
            base.Do();
            return this.SynchronizeJDOrderStateExt(arg);

        }
        /// <summary>
        /// 进销存-同步京东服务单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDServiceState(Jinher.AMP.BTP.Deploy.CustomDTO.SynchronizeJDServiceStateDTO arg)
        {
            base.Do();
            return this.SynchronizeJDServiceStateExt(arg);

        }
    }
}