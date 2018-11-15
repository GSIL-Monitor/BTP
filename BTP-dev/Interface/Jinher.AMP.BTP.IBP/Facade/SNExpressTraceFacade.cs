using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class SNExpressTraceFacade : BaseFacade<ISNExpressTrace>
    {
        public List<Deploy.SNExpressTraceDTO> GetExpressTrace(string orderId, string orderItemId)
        {
            base.Do();
            return this.Command.GetExpressTrace(orderId, orderItemId);
        }

        public bool ChangeLogistStatusForJob()
        {
            base.Do();
            return this.Command.ChangeLogistStatusForJob();
        }
      
     
    }
}
