
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/7/28 11:43:15
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
    public class CommodityOrderExceptionFacade : BaseFacade<ICommodityOrderException>
    {

        /// <summary>
        ///  按条件获取订单异常日志
        ///  </summary>
        /// <param name="dto">参数实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO>> GetOrderExceptionByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderExceptionParamDTO dto)
        {
            base.Do();
            return this.Command.GetOrderExceptionByAppId(dto);
        }
        /// <summary>
        ///  更新订单异常日志状态
        ///  </summary>
        /// <param name="dto">参数实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderException(Jinher.AMP.BTP.Deploy.CommodityOrderExceptionDTO dto)
        {
            base.Do();
            return this.Command.UpdateOrderException(dto);
        }
    }
}