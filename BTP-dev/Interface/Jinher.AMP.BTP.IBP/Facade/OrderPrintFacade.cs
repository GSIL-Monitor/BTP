
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/2/16 16:21:15
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
    public class OrderPrintFacade : BaseFacade<IOrderPrint>
    {

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> GetPrintOrder(System.Collections.Generic.List<System.Guid> orderIds)
        {
            base.Do();
            return this.Command.GetPrintOrder(orderIds);
        }

        /// <summary>
        /// 打印快递单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            base.Do();
            return this.Command.SavePrintOrders(orders);
        }


        /// <summary>
        /// 打印发货单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintInvoiceOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            base.Do();
            return this.Command.SavePrintInvoiceOrders(orders);
        }

    }
}