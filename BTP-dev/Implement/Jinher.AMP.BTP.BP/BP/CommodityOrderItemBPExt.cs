
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/6 11:29:06
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
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 根据UserId获取我的订单商品列表
    /// </summary>
    public partial class CommodityOrderItemBP : BaseBP, ICommodityOrderItem
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<CommodityAndOrderItemDTO>> GetCommodityOrderItemByUserIdExt(string UserId, int PageSize, int PageIndex)
        {
            try
            {
                var userid = Guid.Parse(UserId);
                var query = (from c in CommodityOrder.ObjectSet()
                             join it in OrderItem.ObjectSet()
                             on c.Id equals it.CommodityOrderId
                             orderby c.SubTime
                             where c.UserId.Equals(userid)
                             select new CommodityAndOrderItemDTO
                             {
                                 OrderId = c.Id,
                                 Name = it.Name,
                                 Number = it.Number,
                                 Price = it.CurrentPrice,
                                 SubTime = c.SubTime,
                                 OrderState = c.State
                             });
                var count = query.Count();
                var data = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                return new ResultDTO<List<CommodityAndOrderItemDTO>>
                {
                    isSuccess = true,
                    Data = data,
                    ResultCode = count

                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityOrderItemBP.GetCommodityOrderItemByUserIdExt 异常", ex);
                return new ResultDTO<List<CommodityAndOrderItemDTO>>
                {
                    isSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取子订单相关信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<CommodityAndOrderItemDTO>> GetCommodityOrderItemByOrderIdExt(Guid orderId)
        {
            try
            {
                var query = (from c in CommodityOrder.ObjectSet()
                             join it in OrderItem.ObjectSet()
                             on c.Id equals it.CommodityOrderId
                             orderby c.SubTime
                             where c.Id.Equals(orderId)
                             select new CommodityAndOrderItemDTO
                             {
                                 OrderId = c.Id,
                                 Name = it.Name,
                                 Number = it.Number,
                                 Price = it.CurrentPrice,
                                 SubTime = c.SubTime,
                                 OrderState = c.State,
                                 code = it.Code
                             });

                return new ResultDTO<List<CommodityAndOrderItemDTO>>
                {
                    isSuccess = true,
                    Data = query.ToList(),
                    ResultCode = query.Count()
                };
            }
            catch(Exception ex)
            {
                LogHelper.Error("CommodityOrderItemBP.GetCommodityOrderItemByOrderExt 异常", ex);
                return new ResultDTO<List<CommodityAndOrderItemDTO>>
                {
                    isSuccess = false,
                    Message = ex.Message
                };
            }
            
        }
    }
}