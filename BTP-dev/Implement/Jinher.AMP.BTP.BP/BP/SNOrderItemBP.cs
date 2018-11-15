using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.BP
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SNOrderItemBP : BaseBP, ISNOrderItem
    {
        public bool AddSNOrderItem(List<Deploy.SNOrderItemDTO> snOrderItem)
        {
            base.Do();
            return this.AddSNOrderItemExt(snOrderItem);
        }

        public bool UpdSNOrderItem(Deploy.SNOrderItemDTO snOrderItem)
        {
            base.Do();
            return this.UpdSNOrderItemExt(snOrderItem);
        }

        public bool ChangeOrderStatusForJob()
        {
            base.Do(false);
            return this.ChangeOrderStatusForJobExt();
        }

        public bool OrderConfirmReceived(Guid OrderId)
        {
            base.Do();
            return this.OrderConfirmReceivedExt(OrderId);
        }
    }
}
