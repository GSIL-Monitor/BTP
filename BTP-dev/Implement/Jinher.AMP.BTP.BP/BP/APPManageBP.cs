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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class APPManageBP : BaseBP, IAPPManage
    {

        public ResultDTO AddAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            base.Do();
            return this.AddAPPManageExt(AppManageDTO);
        }

        public ResultDTO UpdateAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            base.Do();
            return this.UpdateAPPManageExt(AppManageDTO);
        }
        public ResultDTO DelAPPManage(Guid Id)
        {
            base.Do();
            return this.DelAPPManageExt(Id);
        }
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO> GetAPPManageList()
        {
            base.Do();
            return this.GetAPPManageListExt();
        }
        public ResultDTO ForbitApp()
        {
            //base.Do();
            return this.ForbitAppExt();
        }
    }
}
