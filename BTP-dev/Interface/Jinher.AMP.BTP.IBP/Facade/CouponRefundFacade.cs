
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/11/13 19:46:26
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
    public class CouponRefundFacade : BaseFacade<ICouponRefund>
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CouponRefundDetailDTO>> GetCouponRefundList(Jinher.AMP.BTP.Deploy.CustomDTO.RefundCouponSearchDTO search)
        {
            base.Do();
            return this.Command.GetCouponRefundList(search);
        }
        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRemark(System.Guid Id, string Remark)
        {
            base.Do();
            return this.Command.UpdateRemark(Id, Remark);
        }
        /// <summary>
        /// 根据优惠券ID获取使用优惠券的订单信息
        /// </summary>
        /// <param name="CouponId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Guid, string>>> GetOrderInfoByCouponId(System.Guid CouponId)
        {
            base.Do();
            return this.Command.GetOrderInfoByCouponId(CouponId);
        }
    }
}