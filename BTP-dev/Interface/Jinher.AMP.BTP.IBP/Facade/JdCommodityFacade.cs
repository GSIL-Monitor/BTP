
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/4/22 10:49:44
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
    public class JdCommodityFacade : BaseFacade<IJdCommodity>
    {

        /// <summary>
        /// 查询列表信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> GetJdCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            base.Do();
            return this.Command.GetJdCommodityList(input);
        }
        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddJdCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do();
            return this.Command.AddJdCommodity(input);
        }
        /// <summary>
        /// 获取商品详情
        /// </summary>
        public Jinher.AMP.BTP.Deploy.JdCommodityDTO GetJdCommodityInfo(System.Guid Id)
        {
            base.Do();
            return this.Command.GetJdCommodityInfo(Id);
        }
        /// <summary>
        /// 批量删除商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelJdCommodityAll(System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.Command.DelJdCommodityAll(Ids);
        }
        /// <summary>
        /// 导出商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> ExportJdCommodityData(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            base.Do();
            return this.Command.ExportJdCommodityData(input);
        }
        /// <summary>
        /// 导入京东商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportJdCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do();
            return this.Command.ImportJdCommodityData(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.Command.AutoSyncCommodityInfo(AppId, Ids);
        }
        /// <summary>
        /// 获取商城品类
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.InnerCategoryDTO> GetCategories(System.Guid AppId)
        {
            base.Do();
            return this.Command.GetCategories(AppId);
        }
        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryList(System.Guid AppId)
        {
            base.Do();
            return this.Command.GetCategoryList(AppId);
        }
    }
}