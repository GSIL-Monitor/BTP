
/***************
功能描述: ZPHFacade
作    者: 
创建时间: 2015/4/16 11:08:55
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class SelectCommodityFacade : BaseFacade<ISelectCommodity>
    {

        /// <summary>
        /// 获取APP
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.AppSetAppGridDTO GetAppList(Deploy.CustomDTO.AppSetSearch2DTO search)
        {
            base.Do();
            return this.Command.GetAppList(search);
        }
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            base.Do();
            return this.Command.SearchCommodity(search);
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity2(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            base.Do();
            return this.Command.SearchCommodity2(search);
        }

        /// <summary>
        /// 按运费模板查询商品
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<SearchCommodityByFreightTemplateOutputDTO> SearchCommodity3(SearchCommodityByFreightTemplateInputDTO inputDTO)
        {
            base.Do();
            return this.Command.GetCommodityByFreightTemplate(inputDTO);
        }
    }
}
