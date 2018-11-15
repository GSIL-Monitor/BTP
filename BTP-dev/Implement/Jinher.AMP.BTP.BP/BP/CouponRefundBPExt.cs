
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/8/16 10:00:21
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
    public partial class CouponRefundBP : BaseBP, ICouponRefund
    {

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CouponRefundDetailDTO>> GetCouponRefundListExt(RefundCouponSearchDTO search)
        {
            try
            {
                var query = CouponRefundDetail.ObjectSet().Where(x => x.Id != Guid.Empty);
                if (!string.IsNullOrEmpty(search.OrderNo))
                {
                    query = query.Where(x => x.OrderNo == search.OrderNo);
                }
                if (!string.IsNullOrEmpty(search.ReceiveAccount))
                {
                    query = query.Where(x => x.ReceiveAccount == search.ReceiveAccount);
                }
                if (!string.IsNullOrEmpty(search.ReceiveName))
                {
                    query = query.Where(x => x.ReceiveName == search.ReceiveName);
                }
                if (!string.IsNullOrEmpty(search.RefundStartTime.ToString()) && search.RefundStartTime != DateTime.MinValue)
                {
                    query = query.Where(x => x.RefundTime >= search.RefundStartTime);
                }
                if (!string.IsNullOrEmpty(search.RefundEndTime.ToString()) && search.RefundEndTime != DateTime.MinValue)
                {
                    var endtime = search.RefundEndTime.AddDays(1);
                    query = query.Where(x => x.RefundTime < endtime);
                }
                var count = query.Count();
                var querydata = query.OrderByDescending(q => q.SubTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                var data = new BE.CouponRefundDetail().ToEntityDataList(querydata);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.CouponRefundDetailDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<Jinher.AMP.BTP.Deploy.CouponRefundDetailDTO> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CouponRefundBP.GetCouponRefundListExt 异常", ex);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.CouponRefundDetailDTO>> { isSuccess = false, Data = null };
            }
        }

        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRemarkExt(System.Guid Id, string Remark)
        {
            var result = new ResultDTO
            {
                isSuccess = true
            };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var couponRefundDetail = CouponRefundDetail.ObjectSet().Where(x => x.Id == Id).FirstOrDefault();
                if (couponRefundDetail != null)
                {
                    couponRefundDetail.Remark = Remark;
                }

                var count = contextSession.SaveChanges();
                if (count <= 0)
                {
                    result.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
            }
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<KeyValuePair<Guid, string>>> GetOrderInfoByCouponIdExt(Guid couponId)
        {
            var returnDto = new ResultDTO<List<KeyValuePair<Guid, string>>>() {
                isSuccess = true,
                Data = new List<KeyValuePair<Guid, string>>()
            };

            try
            {
                var listOrder = OrderPayDetail.ObjectSet().Where(o => o.ObjectId == couponId && o.ObjectType == 1).Select(o => o.OrderId); //获取OrderId
                var orderInfo = CommodityOrder.ObjectSet().Where(o => listOrder.Contains(o.Id)).Select(o => new {o.Id,o.Code});
                foreach (var item in orderInfo)
                {
                    returnDto.Data.Add(new KeyValuePair<Guid, string>(item.Id, item.Code));                        
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("CouponRefundBP.GetOrderInfoByCouponIdExt 异常", ex);
                returnDto.isSuccess = false;
            }
            
            return returnDto;
        }

    }
}