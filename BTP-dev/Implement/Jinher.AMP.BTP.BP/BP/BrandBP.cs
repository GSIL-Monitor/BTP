
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/12 14:33:03
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
    public partial class BrandBP : BaseBP, IBrand
    {

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            base.Do(false);
            return this.AddBrandExt(brandWallDto);
        }
        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            base.Do(false);
            return this.UpdateBrandExt(brandWallDto);
        }
        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandList(string brandName, int status, Guid appId)
        {
            base.Do(false);
            return this.GetBrandListExt(brandName, status, appId);
        }
        /// <summary>
        /// 分页查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandPageList(string brandName, int status, int pageSize, int pageIndex, Guid appId)
        {
            base.Do(false);
            return this.GetBrandPageListExt(brandName, status, pageSize, pageIndex, appId);
        }
        /// <summary>
        /// 是否存在同名品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public bool CheckBrand(string brandName, out int rowCount, Guid appId)
        {
            base.Do(false);
            return this.CheckBrandExt(brandName, out rowCount, appId);
        }
        /// <summary>
        /// 更新品牌状态
        /// </summary>
        /// <param name="id">品牌ID</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrandStatus(Guid id, int status, Guid appId)
        {
            base.Do(false);
            return this.UpdateBrandStatusExt(id, status, appId);
        }
        /// <summary>
        /// 品牌详情
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.BrandwallDTO GetBrand(Guid id, Guid appId)
        {
            base.Do(false);
            return this.GetBrandExt(id, appId);
        }
    }
}