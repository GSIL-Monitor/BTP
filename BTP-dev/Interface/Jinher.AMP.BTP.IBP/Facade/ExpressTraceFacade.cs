
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/12/31 18:23:43
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
namespace Jinher.AMP.BTP.IBP.Facade
{
    public class ExpressTraceFacade : BaseFacade<IExpressTrace>
    {
        /// <summary>
        /// 查询物流详细信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<ExpressTraceDTO> GetExpressTraceList(ExpressTraceDTO search)
        {
            base.Do();
            return this.Command.GetExpressTraceList(search);
        }
        /// <summary>
        /// 保存物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveExpressTraceList(List<ExpressTraceDTO> list)
        {
            base.Do();
            return this.Command.SaveExpressTraceList(list);
        }


        /// <summary>
        /// 根据ExpRouteId删除物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO DelExpressTrace(Guid ExpRouteId)
        {
            base.Do();
            return this.Command.DelExpressTrace(ExpRouteId);
        }
    }
}
