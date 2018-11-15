
/***************
功能描述: ZPHBP
作    者: 
创建时间: 2015/4/16 11:08:57
***************/
using System.Collections.Generic;
using Jinher.JAP.BF.BP.Base;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SelectCommodityBP : BaseBP, ISelectCommodity
    {

        /// <summary>
        /// 获取APP
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.AppSetAppGridDTO GetAppList(Deploy.CustomDTO.AppSetSearch2DTO search)
        {
            base.Do();
            return this.GetAppListExt(search);
        }
        /// <summary>
        /// 查询商品 电商馆
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            base.Do(false);
            return this.SearchCommodityExt(search);
        }

        /// <summary>
        /// 查询商品 非电商馆
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity2(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            base.Do();
            return this.SearchCommodity2Ext(search);
        }

        /// <summary>
        /// 按运费模板查询商品
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<SearchCommodityByFreightTemplateOutputDTO> GetCommodityByFreightTemplate(SearchCommodityByFreightTemplateInputDTO inputDTO)
        {
            base.Do();
            return this.GetCommodityByFreightTemplateExt(inputDTO);
        }
    }
}
