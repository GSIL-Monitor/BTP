
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017-08-31 15:24:58
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
    public class ExpressFacade : BaseFacade<IExpress>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetSystem()
        {
            base.Do();
            return this.Command.GetSystem();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetAll(System.Guid appId)
        {
            base.Do();
            return this.Command.GetAll(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressCodeDTO> Save(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            base.Do();
            return this.Command.Save(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            base.Do();
            return this.Command.Remove(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(System.Guid appId, System.Collections.Generic.List<string> expressCodeList)
        {
            base.Do();
            return this.Command.SaveUsed(appId, expressCodeList);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<string>> GetUsed(System.Guid appId)
        {
            base.Do();
            return this.Command.GetUsed(appId);
        }
    }
}