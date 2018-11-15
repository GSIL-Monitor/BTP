
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/8/27 15:47:04
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
    public class SpreadInfoFacade : BaseFacade<ISpreadInfo>
    {

        /// <summary>
        /// 保存推广主信息
        /// </summary>
        /// <param name="spreadInfo">推广主信息</param>
        /// <returns>Result</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveToSpreadInfo(Jinher.AMP.BTP.Deploy.SpreadInfoDTO spreadInfo)
        {
            base.Do();
            return this.Command.SaveToSpreadInfo(spreadInfo);
        }
        /// <summary>
        /// 查询推广主信息
        /// </summary>
        /// <param name="spreadInfoSearchDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO GetSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoSearchDTO spreadInfoSearchDTO)
        {
            base.Do();
            return this.Command.GetSpreadInfo(spreadInfoSearchDTO);
        }
    }
}