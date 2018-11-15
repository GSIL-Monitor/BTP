
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/10/28 18:04:18
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
    public class CreateOrderFacade : BaseFacade<ICreateOrder>
    {

        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderNeedDTO> GetCreateOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.CreateOrderSearchDTO search)
        {
            base.Do();
            return this.Command.GetCreateOrderInfo(search);
        }
    }
}