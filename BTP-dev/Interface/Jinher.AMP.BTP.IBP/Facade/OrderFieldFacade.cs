
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/5/29 11:37:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class OrderFieldFacade : BaseFacade<IOrderField>
    {
        /// <summary>
        /// 获取订单设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.OrderFieldDTO GetOrderSet(Guid AppId)
        {
            base.Do();
            return this.Command.GetOrderSet(AppId);
        }
    }
}