
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/10/12 11:09:19
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
    public class OrderRefundFacade : BaseFacade<IOrderRefund>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundCompareDTO>> GetOrderRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundSearchDTO search)
        {
            base.Do();
            return this.Command.GetOrderRefund(search);
        }
    }
}