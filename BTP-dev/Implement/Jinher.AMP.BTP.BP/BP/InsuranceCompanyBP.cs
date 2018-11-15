
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/11/6 11:31:01
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
    public partial class InsuranceCompanyBP : BaseBP, IInsuranceCompany
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompany()
        {
            base.Do(false);
            return this.GetInsuranceCompanyExt();
        }
    }
}