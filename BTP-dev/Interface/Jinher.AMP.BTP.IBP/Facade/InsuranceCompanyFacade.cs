
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/11/6 11:31:01
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
    public class InsuranceCompanyFacade : BaseFacade<IInsuranceCompany>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompany()
        {
            base.Do();
            return this.Command.GetInsuranceCompany();
        }
    }
}