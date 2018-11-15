
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/18 16:54:43
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
    public class ShareQueryFacade : BaseFacade<IShareQuery>
    {

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodity(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodity(commodityId);
        }
        /// <summary>
        /// 获取订单中的商品列表
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderForShareDTO GetOrderCommoditys(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetOrderCommoditys(orderId);
        }
    }
}