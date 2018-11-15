
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/2/16 16:21:17
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
    public partial class OrderPrintBP : BaseBP, IOrderPrint
    {

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> GetPrintOrder(System.Collections.Generic.List<System.Guid> orderIds)
        {
            base.Do();
            return this.GetPrintOrderExt(orderIds);
        }

        /// <summary>
        /// 打印快递单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            base.Do();
            return this.SavePrintOrdersExt(orders);
        }

        /// <summary>
        /// 保存打印发货单
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintInvoiceOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            base.Do();
            return this.SavePrintInvoiceOrdersExt(orders);
        }

    }
}