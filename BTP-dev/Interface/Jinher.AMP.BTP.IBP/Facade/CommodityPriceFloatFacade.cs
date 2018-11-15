
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/13 17:29:52
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
    public class CommodityPriceFloatFacade : BaseFacade<ICommodityPriceFloat>
    {

        /// <summary>
        /// 获取自动调价设置数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPriceFloatList<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPriceFloatListDto>> GetDataList(System.Guid appId)
        {
            base.Do();
            return this.Command.GetDataList(appId);
        }
        /// <summary>
        /// 添加自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.CommodityPriceFloatDTO dto)
        {
            base.Do();
            return this.Command.Add(dto);
        }
        /// <summary>
        /// 修改自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Update(Jinher.AMP.BTP.Deploy.CommodityPriceFloatDTO dto)
        {
            base.Do();
            return this.Command.Update(dto);
        }
        /// <summary>
        /// 删除自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Delete(System.Guid id)
        {
            base.Do();
            return this.Command.Delete(id);
        }
        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetApps(System.Guid esAppId)
        {
            base.Do();
            return this.Command.GetApps(esAppId);
        }
    }
}