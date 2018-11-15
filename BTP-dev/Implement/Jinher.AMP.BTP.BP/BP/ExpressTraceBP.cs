/***************
功能描述: BTP-OPTBP
作    者: 
创建时间: 2015/7/30 17:59:54
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{  

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ExpressTraceBP : BaseBP, IExpressTrace
    {
        /// <summary>
        /// 查询物流详细信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<ExpressTraceDTO> GetExpressTraceList(ExpressTraceDTO search)
        {
            base.Do();
            return this.GetExpressTraceListExt(search);
        }
        /// <summary>
        /// 保存物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveExpressTraceList(List<ExpressTraceDTO> list)
        {
            base.Do();
            return this.SaveExpressTraceListExt(list);
        }


        /// <summary>
        /// 根据主键id删除物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO DelExpressTrace(Guid ExpRouteId)
        {
            base.Do();
            return this.DelExpressTraceExt(ExpRouteId);
        }
    }
}
