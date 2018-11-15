
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/29 17:00:31
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
    public partial class CategoryAdvertiseBP : BaseBP, ICategoryAdvertise
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            base.Do(false);
            return this.CreateCategoryAdvertiseExt(entity);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>> CateGoryAdvertiseList(string advertiseName, int state, System.Guid CategoryId, int pageIndex, int pageSize, out int rowCount)
        {
            base.Do(false);
            return this.CateGoryAdvertiseListExt(advertiseName, state, CategoryId, pageIndex, pageSize, out rowCount);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategoryAdvertise(System.Guid advertiseId)
        {
            base.Do(false);
            return this.DeleteCategoryAdvertiseExt(advertiseId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            base.Do(false);
            return this.EditCategoryAdvertiseExt(entity);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> GetCategoryAdvertise(System.Guid advertiseId)
        {
            base.Do(false);
            return this.GetCategoryAdvertiseExt(advertiseId);
        }
    }
}