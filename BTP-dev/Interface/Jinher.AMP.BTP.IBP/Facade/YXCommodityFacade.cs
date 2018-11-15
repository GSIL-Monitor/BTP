
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/5/19 12:55:49
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
    public class YXCommodityFacade : BaseFacade<IYXCommodity>
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYXCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            base.Do();
            return this.Command.AddYXCommodity(input);
        }
        /// <summary>
        /// 导入严选商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYXCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            base.Do();
            return this.Command.ImportYXCommodityData(JdComList, AppId);
        }
        /// <summary>
        /// 自动同步严选商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYXCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            base.Do();
            return this.Command.AutoSyncYXCommodityInfo(AppId, Ids);
        }
    }
}