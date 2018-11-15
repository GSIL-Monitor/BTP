using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class SNOrderItemFacade : BaseFacade<ISNOrderItem>
    {
        public bool AddSNOrderItem(List<SNOrderItemDTO> snOrderItem)
        {
            base.Do();
            return this.Command.AddSNOrderItem(snOrderItem);
        }

        public bool UpdSNOrderItem(SNOrderItemDTO snOrderItem)
        {
            base.Do();
            return this.Command.UpdSNOrderItem(snOrderItem);
        }

        public bool ChangeOrderStatusForJob()
        {
            base.Do();
            return this.Command.ChangeOrderStatusForJob();
        }

        public bool OrderConfirmReceived(Guid OrderId)
        {
            base.Do();
            return this.Command.OrderConfirmReceived(OrderId);
        }
    }
}
