
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/15 17:13:37
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
    public class CategoryAdvertiseFacade : BaseFacade<ICategoryAdvertise>
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> getBrandWallSpecialByCateID(System.Guid CategoryID)
        {
            base.Do();
            return this.Command.getBrandWallSpecialByCateID(CategoryID);
        }
    }
}