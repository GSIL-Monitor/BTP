
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/18 15:32:59
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AppSelfTakeStationSV : BaseSv, IAppSelfTakeStation
    {

        /// <summary>
        /// 下订单页获取自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationDefaultInfoDTO GetAppSelfTakeStationDefault(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchDTO search)
        {
            base.Do();
            return this.GetAppSelfTakeStationDefaultExt(search);

        }
    }
}