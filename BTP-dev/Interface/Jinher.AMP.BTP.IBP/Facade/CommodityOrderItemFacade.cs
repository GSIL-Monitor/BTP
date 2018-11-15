
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/7/6 13:45:02
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
    public class CommodityOrderItemFacade : BaseFacade<ICommodityOrderItem>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByUserId(string UserId, int PageSize, int PageIndex)
        {
            base.Do();
            return this.Command.GetCommodityOrderItemByUserId(UserId, PageSize, PageIndex);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByOrderId(Guid orderId)
        {
            base.Do();
            return this.Command.GetCommodityOrderItemByOrderId(orderId);
        }
    }
}