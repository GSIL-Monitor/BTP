
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/3/26 10:14:50
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SelfTakeStationSV : BaseSv, ISelfTakeStation
    {
        /// <summary>
        /// 查询自提点
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public SelfTakeStationResultDTO GetSelfTakeStation(SelfTakeStationSearchDTO search)
        {
            base.Do(false);
            return this.GetSelfTakeStationExt(search);
        }
        /// <summary>
        /// 删除总代关联删除
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO DeleteCityOwner(SelfTakeStationSearchDTO search)
        {
            base.Do();
            return this.DeleteCityOwnerExt(search);
        }
    }
}
