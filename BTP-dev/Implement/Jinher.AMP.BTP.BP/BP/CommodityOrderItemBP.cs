
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/6 13:45:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderItemBP : BaseBP, ICommodityOrderItem
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByUserId(string UserId, int PageSize, int PageIndex)
        {
            base.Do(false);
            return this.GetCommodityOrderItemByUserIdExt(UserId, PageSize, PageIndex);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.CommodityAndOrderItemDTO>> GetCommodityOrderItemByOrderId(Guid orderId)
        {
            base.Do(false);
            return this.GetCommodityOrderItemByOrderIdExt(orderId);
        }
    }


}