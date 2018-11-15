
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/22 10:15:40
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
    public partial class CategoryInnerBrandBP : BaseBP, ICategoryInnerBrand
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO model)
        {
            base.Do();
            return this.AddExt(model);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO>> GetBrandWallList(System.Guid CategoryId)
        {
            base.Do(false);
            return this.GetBrandWallListExt(CategoryId);
        }
        /// <summary>
        /// 删除多个分类品牌
        /// </summary>
        /// <param name="Id">分类Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCateBrandsByCategoryId(System.Guid CategoryId)
        {
            base.Do(false);
            return this.DeleteCateBrandsByCategoryIdExt(CategoryId);
        }
        /// <summary>
        /// 添加多个分类频偏
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddList(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO> models)
        {
            base.Do(false);
            return this.AddListExt(models);
        }
    }
}