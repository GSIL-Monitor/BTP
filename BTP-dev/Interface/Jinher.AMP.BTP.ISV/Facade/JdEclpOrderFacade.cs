
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/4/8 19:46:31
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
    public class JdEclpOrderFacade : BaseFacade<IJdEclpOrder>
    {

        /// <summary>
        /// 进销存-同步京东订单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDOrderState(Jinher.AMP.BTP.Deploy.JDEclpOrderJournalDTO arg)
        {
            base.Do();
            return this.Command.SynchronizeJDOrderState(arg);
        }
        /// <summary>
        /// 进销存-同步京东服务单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDServiceState(Jinher.AMP.BTP.Deploy.CustomDTO.SynchronizeJDServiceStateDTO arg)
        {
            base.Do();
            return this.Command.SynchronizeJDServiceState(arg);
        }
    }
}