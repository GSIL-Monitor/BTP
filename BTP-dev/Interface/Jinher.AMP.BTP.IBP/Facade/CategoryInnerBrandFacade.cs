
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/22 10:15:40
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
    public class CategoryInnerBrandFacade : BaseFacade<ICategoryInnerBrand>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO model)
        {
            base.Do();
            return this.Command.Add(model);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO>> GetBrandWallList(System.Guid CategoryId)
        {
            base.Do();
            return this.Command.GetBrandWallList(CategoryId);
        }
        /// <summary>
        /// 删除多个分类品牌
        /// </summary>
        /// <param name="Id">分类Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCateBrandsByCategoryId(System.Guid CategoryId)
        {
            base.Do();
            return this.Command.DeleteCateBrandsByCategoryId(CategoryId);
        }
        /// <summary>
        /// 添加多个分类频偏
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddList(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO> models)
        {
            base.Do();
            return this.Command.AddList(models);
        }
    }
}