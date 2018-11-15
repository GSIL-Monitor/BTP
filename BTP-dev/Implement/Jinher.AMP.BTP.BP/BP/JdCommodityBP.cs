
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/4/22 10:49:48
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
    public partial class JdCommodityBP : BaseBP, IJdCommodity
    {

        /// <summary>
        /// 查询列表信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> GetJdCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            base.Do(false);
            return this.GetJdCommodityListExt(input);
        }
        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddJdCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do(false);
            return this.AddJdCommodityExt(input);
        }
        /// <summary>
        /// 获取商品详情
        /// </summary>
        public Jinher.AMP.BTP.Deploy.JdCommodityDTO GetJdCommodityInfo(System.Guid Id)
        {
            base.Do(false);
            return this.GetJdCommodityInfoExt(Id);
        }
        /// <summary>
        /// 批量删除商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelJdCommodityAll(System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do(false);
            return this.DelJdCommodityAllExt(Ids);
        }
        /// <summary>
        /// 导出商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> ExportJdCommodityData(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            base.Do(false);
            return this.ExportJdCommodityDataExt(input);
        }
        /// <summary>
        /// 导入京东商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportJdCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do(false);
            return this.ImportJdCommodityDataExt(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do(false);
            return this.AutoSyncCommodityInfoExt(AppId, Ids);
        }
        /// <summary>
        /// 获取商城品类
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.InnerCategoryDTO> GetCategories(System.Guid AppId)
        {
            base.Do(false);
            return this.GetCategoriesExt(AppId);
        }
        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryList(System.Guid AppId)
        {
            base.Do(false);
            return this.GetCategoryListExt(AppId);
        }
    }
}