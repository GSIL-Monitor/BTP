
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/15 17:13:40
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
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CategoryAdvertiseSV : BaseSv, ICategoryAdvertise
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> getBrandWallSpecialByCateID(System.Guid CategoryID)
        {
            base.Do();
            return this.getBrandWallSpecialByCateIDExt(CategoryID);
        }
    }
}