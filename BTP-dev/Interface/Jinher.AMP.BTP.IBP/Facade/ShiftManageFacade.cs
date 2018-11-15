
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/1/10 13:42:00
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
    public class ShiftManageFacade : BaseFacade<IShiftManage>
    {

        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShiftExchange(Jinher.AMP.BTP.Deploy.CustomDTO.ShiftLogDTO dto)
        {
            base.Do();
            return this.Command.ShiftExchange(dto);
        }
        /// <summary>
        /// 获取交班信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FShiftLogCDTO GetLastShiftInfo(System.Guid appId)
        {
            base.Do();
            return this.Command.GetLastShiftInfo(appId);
        }
    }
}