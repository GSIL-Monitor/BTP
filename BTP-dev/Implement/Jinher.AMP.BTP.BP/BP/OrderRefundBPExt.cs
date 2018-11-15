
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/10/10 15:34:06
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class OrderRefundBP : BaseBP, IOrderRefund
    {

        public ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundCompareDTO>> GetOrderRefundExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundSearchDTO search)
        {
            var result = new ResultDTO<ListResult<OrderRefundCompareDTO>>();
            try
            {
                var refunds = (from o in OrderRefund.ObjectSet()
                               where o.OrderRefundMoneyAndCoupun != o.RefundYJCouponMoney + o.RefundFreightPrice + o.RefundYJBMoney + o.RefundMoney
                               select new Deploy.CustomDTO.OrderRefundCompareDTO
                               {
                                   SubTime = o.SubTime,
                                   OrderRefundMoneyAndCoupun = o.OrderRefundMoneyAndCoupun,
                                   RefundYJCouponMoney = o.RefundYJCouponMoney,
                                   RefundFreightPrice = o.RefundFreightPrice,
                                   RefundYJBMoney = o.RefundYJBMoney,
                                   RefundMoney = o.RefundMoney,
                                   OrderId = o.OrderId
                               })
                               .Union(from a in OrderRefundAfterSales.ObjectSet()
                                      where a.OrderRefundMoneyAndCoupun != a.RefundYJCouponMoney + a.RefundFreightPrice + a.RefundYJBMoney + a.RefundMoney
                                      select new Deploy.CustomDTO.OrderRefundCompareDTO
                                      {
                                          SubTime = a.SubTime,
                                          OrderRefundMoneyAndCoupun = a.OrderRefundMoneyAndCoupun,
                                          RefundYJCouponMoney = a.RefundYJCouponMoney,
                                          RefundFreightPrice = a.RefundFreightPrice,
                                          RefundYJBMoney = a.RefundYJBMoney,
                                          RefundMoney = a.RefundMoney,
                                          OrderId=a.OrderId
                                      });
                result.isSuccess = true;
                var data = refunds.OrderByDescending(x => x.SubTime).Skip(search.PageSize * (search.PageIndex-1)).Take(search.PageSize).ToList();
                result.Data = new ListResult<OrderRefundCompareDTO> { List = data, Count = refunds.Count() };
                return result;
            }
            catch(Exception ex)
            {
                LogHelper.Error("JdCommodityBP.GetJdCommodityListExt 异常", ex);
                return new ResultDTO<ListResult<OrderRefundCompareDTO>> { isSuccess = false, Data = null };
            }
        }
    }
}