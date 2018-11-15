
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/8/7 13:57:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    /// <summary>
    /// 自提点
    /// </summary>
    public class SelfTakeStationFacade : BaseFacade<ISelfTakeStation>
    {
        /// <summary>
        /// 查询自提点
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResultDTO GetSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchDTO search)
        {
            base.Do();
            return this.Command.GetSelfTakeStation(search);
        }
        /// <summary>
        /// 删除总代关联删除
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCityOwner(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchDTO search)
        {
            base.Do();
            return this.Command.DeleteCityOwner(search);
        }
    }
}