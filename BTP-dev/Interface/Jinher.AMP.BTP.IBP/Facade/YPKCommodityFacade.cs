
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/9/6 16:04:43
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
    public class YPKCommodityFacade : BaseFacade<IYPKCommodity>
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYPKCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do();
            return this.Command.AddYPKCommodity(input);
        }
        /// <summary>
        /// 导入易派客商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYPKCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do();
            return this.Command.ImportYPKCommodityData(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYPKCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.Command.AutoSyncYPKCommodityInfo(AppId, Ids);
        }
        /// <summary>
        /// 全量同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoYPKSyncCommodity(System.Guid AppId)
        {
            base.Do();
            return this.Command.AutoYPKSyncCommodity(AppId);
        }
    }
}