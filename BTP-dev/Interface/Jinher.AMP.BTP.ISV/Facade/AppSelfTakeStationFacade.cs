
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/9/18 15:32:57
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
    public class AppSelfTakeStationFacade : BaseFacade<IAppSelfTakeStation>
    {

        /// <summary>
        /// 下订单页获取自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationDefaultInfoDTO GetAppSelfTakeStationDefault(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchDTO search)
        {
            base.Do();
            return this.Command.GetAppSelfTakeStationDefault(search);
        }
    }
}