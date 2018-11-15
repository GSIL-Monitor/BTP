
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/25 17:24:09
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
    public class BrandFacade : BaseFacade<IBrand>
    {

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            base.Do();
            return this.Command.AddBrand(brandWallDto);
        }
        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            base.Do();
            return this.Command.UpdateBrand(brandWallDto);
        }
        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandList(string brandName, int status, System.Guid appId)
        {
            base.Do();
            return this.Command.GetBrandList(brandName, status, appId);
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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandPageList(string brandName, int status, int pageSize, int pageIndex, System.Guid appId)
        {
            base.Do();
            return this.Command.GetBrandPageList(brandName, status, pageSize, pageIndex, appId);
        }
        /// <summary>
        /// 是否存在同名品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public bool CheckBrand(string brandName, out int rowCount, System.Guid appId)
        {
            base.Do();
            return this.Command.CheckBrand(brandName, out rowCount, appId);
        }
        /// <summary>
        /// 更新品牌状态
        /// </summary>
        /// <param name="id">品牌ID</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrandStatus(System.Guid id, int status, System.Guid appId)
        {
            base.Do();
            return this.Command.UpdateBrandStatus(id, status, appId);
        }
        /// <summary>
        /// 品牌详情
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.BrandwallDTO GetBrand(System.Guid id, System.Guid appId)
        {
            base.Do();
            return this.Command.GetBrand(id, appId);
        }
    }
}