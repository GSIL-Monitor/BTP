using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class APPManageFacade : BaseFacade<IAPPManage>
    {

        public ResultDTO AddAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            base.Do();
            return this.Command.AddAPPManage(AppManageDTO);
        }

        public ResultDTO UpdateAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            base.Do();
            return this.Command.UpdateAPPManage(AppManageDTO);
        }
        public ResultDTO DelAPPManage(Guid Id)
        {
            base.Do();
            return this.Command.DelAPPManage(Id);
        }
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO> GetAPPManageList()
        {
            base.Do();
            return this.Command.GetAPPManageList();
        }
        public ResultDTO ForbitApp()
        {
            base.Do();
            return this.Command.ForbitApp();
        }
    }
}
