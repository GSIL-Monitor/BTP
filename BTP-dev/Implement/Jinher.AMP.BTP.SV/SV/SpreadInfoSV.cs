
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/8/27 15:47:07
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
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
    public partial class SpreadInfoSV : BaseSv, ISpreadInfo
    {

        /// <summary>
        /// 保存推广主信息
        /// </summary>
        /// <param name="spreaderDto">推广主信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveToSpreadInfo(Jinher.AMP.BTP.Deploy.SpreadInfoDTO spreadInfo)
        {
            base.Do();
            LogHelper.Info(string.Format("保存推广主信息:SpreadInfoSV.SaveToSpreadInfo , spreadInfo:{0}", JsonHelper.JsonSerializer(spreadInfo)));
            return this.SaveToSpreadInfoExt(spreadInfo);

        }
        /// <summary>
        /// 查询推广主信息
        /// </summary>
        /// <param name="spreadInfoSearchDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO GetSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoSearchDTO spreadInfoSearchDTO)
        {
            base.Do(false);
            return this.GetSpreadInfoExt(spreadInfoSearchDTO);
        }
    }
}