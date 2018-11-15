
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/6/6 11:44:07
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
    public class OrderPayDetailFacade : BaseFacade<IOrderPayDetail>
    {

        /// <summary>
        /// 查询OrderPayDetail信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> GetOrderPayDetailList(Guid objectid)
        {
            base.Do();
            return this.Command.GetOrderPayDetailList(objectid);
        }
    }
}