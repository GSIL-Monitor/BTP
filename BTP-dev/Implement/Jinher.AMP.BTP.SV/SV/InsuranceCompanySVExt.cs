
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/11/5 20:19:28
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class InsuranceCompanySV : BaseSv, IInsuranceCompany
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompanyExt()
        {
            throw new NotImplementedException();
        }
    }
}