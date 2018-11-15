
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/18 16:54:45
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
    public partial class ShareQueryBP : BaseBP, IShareQuery
    {

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodity(System.Guid commodityId)
        {
            base.Do(false);
            return this.GetCommodityExt(commodityId);
        }
        /// <summary>
        /// 获取订单中的商品列表
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderForShareDTO GetOrderCommoditys(System.Guid orderId)
        {
            base.Do(false);
            return this.GetOrderCommoditysExt(orderId);
        }
    }
}