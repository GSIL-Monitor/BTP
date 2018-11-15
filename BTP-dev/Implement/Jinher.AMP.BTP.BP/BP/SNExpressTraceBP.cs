using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;

namespace Jinher.AMP.BTP.BP
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SNExpressTraceBP : BaseBP, ISNExpressTrace
    {

        public List<Deploy.SNExpressTraceDTO> GetExpressTrace(string orderId, string orderItemId)
        {
            base.Do(false);
            return this.GetExpressTraceExt(orderId, orderItemId);
        }


        public bool ChangeLogistStatusForJob()
        {
            base.Do(false);
            return this.ChangeLogistStatusForJobExt();
        }
    }
}
