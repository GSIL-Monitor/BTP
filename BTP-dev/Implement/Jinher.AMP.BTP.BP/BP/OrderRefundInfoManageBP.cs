
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/11 16:12:21
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
    public partial class OrderRefundInfoManageBP : BaseBP, IOrderRefundInfoManage
    {

        /// <summary>
        /// 添加部分退单商品信息
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundComdtyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.BOrderRefundInfoCDTO cdto)
        {
            base.Do();
            return this.AddRefundComdtyInfoExt(cdto);
        }
        /// <summary>
        /// 获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FOrderRefundInfoCDTO GetOrderRefundInfo(System.Guid orderId)
        {
            base.Do();
            return this.GetOrderRefundInfoExt(orderId);
        }
        /// <summary>
        /// 根据订单商品ID获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO GetOrderRefundInfoByItemId(System.Guid orderItemId)
        {
            base.Do(false);
            return this.GetOrderRefundInfoByItemIdExt(orderItemId);
        }
    }
}