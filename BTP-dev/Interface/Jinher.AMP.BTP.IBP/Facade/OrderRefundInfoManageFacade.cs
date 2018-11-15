
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/7/11 16:12:17
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
    public class OrderRefundInfoManageFacade : BaseFacade<IOrderRefundInfoManage>
    {

        /// <summary>
        /// 添加部分退单商品信息
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundComdtyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.BOrderRefundInfoCDTO cdto)
        {
            base.Do();
            return this.Command.AddRefundComdtyInfo(cdto);
        }
        /// <summary>
        /// 获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FOrderRefundInfoCDTO GetOrderRefundInfo(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetOrderRefundInfo(orderId);
        }
        /// <summary>
        /// 根据订单商品ID获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO GetOrderRefundInfoByItemId(System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderRefundInfoByItemId(orderItemId);
        }
    }
}