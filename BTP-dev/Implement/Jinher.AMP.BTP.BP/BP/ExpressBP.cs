
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017-08-31 15:25:00
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
    public partial class ExpressBP : BaseBP, IExpress
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetSystem()
        {
            base.Do();
            return this.GetSystemExt();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressCodeDTO>> GetAll(System.Guid appId)
        {
            base.Do();
            return this.GetAllExt(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressCodeDTO> Save(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            base.Do();
            return this.SaveExt(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(Jinher.AMP.BTP.Deploy.ExpressCodeDTO dto)
        {
            base.Do();
            return this.RemoveExt(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(System.Guid appId, System.Collections.Generic.List<string> expressCodeList)
        {
            base.Do();
            return this.SaveUsedExt(appId, expressCodeList);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<string>> GetUsed(System.Guid appId)
        {
            base.Do();
            return this.GetUsedExt(appId);
        }
    }
}