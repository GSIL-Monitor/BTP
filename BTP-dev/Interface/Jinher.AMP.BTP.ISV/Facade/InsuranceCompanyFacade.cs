
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/11/5 20:19:26
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class InsuranceCompanyFacade : BaseFacade<IInsuranceCompany>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompany()
        {
            base.Do();
            return this.Command.GetInsuranceCompany();
        }
    }
}