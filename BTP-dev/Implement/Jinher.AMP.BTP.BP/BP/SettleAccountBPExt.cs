
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/7/4 10:49:40
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
using System.Data;
using Jinher.JAP.Common.Loging;
using System.Diagnostics;
using Jinher.AMP.BTP.Common.Extensions;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    ///  结算管理
    /// </summary>
    public partial class SettleAccountBP : BaseBP, ISettleAccount
    {
        /// <summary>
        /// 获取商城结算数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountListDTO>> GetMallSettleAccountsExt(SettleAccountSearchDTO searchDto)
        {
            if (searchDto == null || searchDto.EsAppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { ResultCode = 1, Message = "参数为空" };
            }
            searchDto.AmountDate = searchDto.AmountDate.Date;
            // 重新生成临时结算单
            // 1. 删除之前生成的临时结算单
            var sas = SettleAccounts.ObjectSet().Where(s => s.EsAppId == searchDto.EsAppId && s.State.Value == 0).ToList();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            foreach (var sa in sas)
            {
                contextSession.Delete(sa);
            }
            contextSession.SaveChanges();

            // 2. 查询商城下APP
            var mallQuery = MallApply.ObjectSet().Where(m => m.EsAppId == searchDto.EsAppId && (m.State.Value == 2 || m.State.Value == 4));
            if (!string.IsNullOrWhiteSpace(searchDto.AppName))
            {
                mallQuery = mallQuery.Where(a => a.AppName.Contains(searchDto.AppName));
            }
            if (searchDto.SellerType.HasValue)
            {
                mallQuery = mallQuery.Where(a => a.Type == searchDto.SellerType);
            }
            var appIds = mallQuery.Select(m => m.AppId).Distinct().ToList();
            if (appIds.Count > 0)
            {
                // 3. 生成临时结算单
                var result = CreateSettleAccountCore(new SettleAccountCreateDTO { AmountDate = searchDto.AmountDate, AppIds = appIds, EsAppId = searchDto.EsAppId });
                if (!result.isSuccess)
                {
                    return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = result.isSuccess, Message = result.Message };
                }
            }
            // 查询已生成结算单数据
            var date = DateTime.Now.Date.AddDays(1);
            var settleAccountQuery = SettleAccounts.ObjectSet().Where(s => s.EsAppId == searchDto.EsAppId && s.State.Value != 3 && s.AmountDate < date);
            if (!string.IsNullOrWhiteSpace(searchDto.AppName))
            {
                settleAccountQuery = settleAccountQuery.Where(a => a.AppName.Contains(searchDto.AppName));
            }
            if (searchDto.SellerType.HasValue)
            {
                settleAccountQuery = settleAccountQuery.Where(a => a.SellerType == searchDto.SellerType);
            }
            var data = settleAccountQuery.OrderBy(q => q.EsAppName).Select(q => new SettleAccountListDTO
            {
                Id = q.Id,
                AmountDate = q.AmountDate,
                AppId = q.AppId,
                AppName = q.AppName,
                OrderAmount = q.OrderAmount,
                OrderRealAmount = q.OrderRealAmount,
                SellerAmount = q.SellerAmount,
                State = q.State.Value,
                SettleStatue = q.SettleStatue,
                SellerType = q.SellerType,
                SupplierName = null
            }).ToList();

            if (data.Count() > 0)
            {
                foreach (var item in data.ToList())
                {
                    if (item.AppId != Guid.Empty)
                    {
                        string AppIds = item.AppId.ToString();
                        var supplierQuery = SupplierMain.ObjectSet().FirstOrDefault(_p => _p.AppIds.Contains(AppIds));
                        if (supplierQuery != null)
                        {
                            item.SupplierName = supplierQuery.SupplierName.Trim();
                        }
                    }

                }

            }
            if (!string.IsNullOrWhiteSpace(searchDto.SupplierName))
            {
                string Name = searchDto.SupplierName.Trim();
                data = data.Where(p => p.SupplierName != null && p.SupplierName.Contains(searchDto.SupplierName)).ToList();
            }
            var count = data.Count();
            return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, Data = new ListResult<SettleAccountListDTO> { List = data.Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize).ToList(), Count = count } };
        }

        /// <summary>
        /// 获取商家结算数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountListDTO>> GetSellerSettleAccountsExt(SettleAccountSearchDTO searchDto)
        {
            if (searchDto == null || searchDto.AppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { ResultCode = 1, Message = "参数为空" };
            }
            // 只显示 已生成结算单的并且未打款的
            var query = SettleAccounts.ObjectSet().Where(s => s.AppId == searchDto.AppId && (s.State.Value == 1 || s.State.Value == 2));
            if (!string.IsNullOrWhiteSpace(searchDto.EsAppName))
            {
                searchDto.EsAppName = searchDto.EsAppName.Trim();
                query = query.Where(q => q.EsAppName.Contains(searchDto.EsAppName));
            }
            if (searchDto.SellerType.HasValue)
            {
                query = query.Where(a => a.SellerType == searchDto.SellerType);
            }
            var data = query.OrderBy(q => q.EsAppName).Select(q => new SettleAccountListDTO
            {
                Id = q.Id,
                AmountDate = q.AmountDate,
                EsAppName = q.EsAppName,
                OrderAmount = q.OrderAmount,
                OrderRealAmount = q.OrderRealAmount,
                SellerAmount = q.SellerAmount,
                State = q.State.Value,
                SettleStatue = q.SettleStatue,
                SellerType = q.SellerType
            }).ToList();
            return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, Data = new ListResult<SettleAccountListDTO> { List = data, Count = data.Count } };
        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private ResultDTO CreateSettleAccountCore(SettleAccountCreateDTO dto)
        {
            if (dto == null || dto.EsAppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = false, ResultCode = 1, Message = "参数为空" };
            }
            if (dto.AppIds == null || dto.AppIds.Count == 0)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, ResultCode = 0 };
            }
            dto.AmountDate = dto.AmountDate.Date;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            // 查询结算周期
            //var day = GetSettleAccountPeriodNumOfDay(dto.EsAppId);
            var period = SettleAccountsPeriod.FindByID(dto.EsAppId);
            bool useAfterSalesEndTime;
            int day;
            if (period == null)
            {
                day = 7;
                useAfterSalesEndTime = false;
            }
            else
            {
                day = period.NumOfDay;
                useAfterSalesEndTime = period.UseAfterSalesEndTime ?? false;
            }
            var compareDate = dto.AmountDate.Date.AddDays(1);
            var endTime = compareDate.AddDays(-day);
            var nowDate = DateTime.Now.Date.AddDays(1);
            int seq = 0;
            foreach (var appId in dto.AppIds)
            {

                // 查询场馆信息
                var mall = MallApply.ObjectSet().Where(m => m.AppId == appId && m.EsAppId == dto.EsAppId && (m.State.Value == 2 || m.State.Value == 4)).FirstOrDefault();
                if (mall == null)
                {
                    LogHelper.Info("生成结算项失败，商城中未找到该APP，AppID：" + appId + "，EsAppID：" + dto.EsAppId);
                    continue;
                }
                // 查询符合的结算项
                List<SettleAccountsDetails> sads;
                if (useAfterSalesEndTime)
                {
                    sads = SettleAccountsDetails.ObjectSet().Where(s =>
                       s.IsSettled == false
                       && s.EsAppId == dto.EsAppId
                       && s.AppId == appId
                       && s.AfterSalesEndTime.HasValue && s.AfterSalesEndTime < nowDate).ToList();
                }
                else
                {
                    sads = SettleAccountsDetails.ObjectSet().Where(s =>
                       s.IsSettled == false
                       && s.EsAppId == dto.EsAppId
                       && s.AppId == appId
                       && s.OrderConfirmTime.HasValue && s.OrderConfirmTime < endTime).ToList();
                }
                if (sads.Count > 0)
                {
                    // 结算单
                    SettleAccounts sa = SettleAccounts.CreateSettleAccounts();
                    sa.UserId = ContextDTO.LoginUserID;
                    sa.ModifiedOn = sa.SubTime = DateTime.Now;
                    sa.AmountDate = dto.AmountDate;
                    sa.AppId = appId;
                    sa.EsAppId = dto.EsAppId;
                    sa.AppName = APPSV.GetAppName(sa.AppId);
                    sa.EsAppName = ZPHSV.Instance.GetAppPavilionInfo(new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam { id = sa.EsAppId }).pavilionName;
                    sa.OrderAmount = sads.Sum(s => s.OrderAmount);
                    sa.OrderRealAmount = sads.Sum(s => s.OrderRealAmount);
                    sa.CouponAmount = sads.Sum(s => s.OrderCouponAmount);
                    sa.RefundAmount = sads.Sum(s => s.OrderRefundAmount);
                    sa.PromotionCommissionAmount = sads.Sum(s => s.OrderPromotionCommissionAmount);
                    sa.PromotionAmount = sads.Sum(s => s.PromotionAmount);
                    sa.SellerAmount = sads.Sum(s => s.SellerAmount);
                    sa.OrderYJBAmount = sads.Sum(s => s.OrderYJBAmount);
                    sa.AmountStartDate = sads.Min(s => s.AfterSalesEndTime);
                    sa.IsAmount = false;
                    sa.SettleStatue = true;
                    sa.Code = DateTime.Now.ToString("yyyyMMddHHmmss") + (++seq).ToString("D4");
                    sa.State = new Deploy.SettleAccountsVO() { Value = 0 };
                    sa.SellerType = mall.Type;
                    contextSession.SaveObject(sa);
                    foreach (var sad in sads)
                    {
                        sad.SAId = sa.Id;
                        sad.IsSettled = false;
                        sad.EntityState = EntityState.Modified;
                        contextSession.SaveObject(sad);
                    }
                }
            }
            try
            {
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountBP.CreateSettleAccountExt异常: {1}，入参: {0}", JsonHelper.JsonSerializer(dto), ex));
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }

        //public ResultDTO CreateSettleAccountExt(SettleAccountCreateDTO dto)
        //{
        //    if (dto == null || dto.EsAppId == Guid.Empty)
        //    {
        //        return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = false, ResultCode = 1, Message = "参数为空" };
        //    }
        //    if (dto.AppIds == null || dto.AppIds.Count == 0)
        //    {
        //        return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, ResultCode = 0 };
        //    }
        //    dto.AmountDate = dto.AmountDate.Date;
        //    ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //    // 查询结算周期
        //    var day = GetSettleAccountPeriodNumOfDay(dto.EsAppId);
        //    var compareDate = dto.AmountDate.Date.AddDays(1);
        //    var hadAddOrderQuery = SettleAccountsDetails.ObjectSet().AsQueryable();
        //    List<SettleAccountListDTO> data = new List<SettleAccountListDTO>();
        //    var endTime = compareDate.AddDays(-day);
        //    List<int> directArrivalPayments = GetDirectArrivalPayment();
        //    foreach (var appId in dto.AppIds)
        //    {
        //        // 查询历史订单
        //        var orders = CommodityOrderService.ObjectSet().Where(s => (s.State == 3 || s.State == 7 || s.State == 15) && s.SubTime < endTime).
        //            Join(CommodityOrder.ObjectSet().Where(o =>
        //                o.AppId == appId && o.EsAppId == dto.EsAppId && directArrivalPayments.Contains(o.Payment)),
        //                s => s.Id, o => o.Id, (s, o) => new { s.State, o }).
        //            Where(so => !hadAddOrderQuery.Where(h => h.AppId == appId).Any(h => h.Id == so.o.Id)).ToList();
        //        if (orders.Count > 0)
        //        {
        //            // 生成结算单
        //            foreach (var so in orders)
        //            {
        //                so.o.State = so.State;
        //            }
        //            Jinher.AMP.BTP.TPS.OrderSV.CreateSettleAccount(contextSession, orders.Select(so => so.o).ToList(), ContextDTO.LoginUserID, dto.AmountDate, true);
        //        }
        //    }
        //    try
        //    {
        //        contextSession.SaveChanges();
        //        return new ResultDTO { isSuccess = true, Message = "success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(string.Format("SettleAccountBP.CreateSettleAccountExt异常: {1}，入参: {0}", JsonHelper.JsonSerializer(dto), ex));
        //        return new ResultDTO { ResultCode = 1, Message = ex.Message };
        //    }
        //}

        /// <summary>
        /// 导入历史订单，生成结算项
        /// </summary>
        /// <returns></returns>
        public ResultDTO CreateSettleAccountDetailsExt(SettleAccountDetailsCreateDTO dto)
        {
            if (dto == null || dto.EsAppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = false, ResultCode = 1, Message = "参数为空" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var compareDate = dto.EndDate.Date.AddDays(1);
            var hadAddOrderQuery = SettleAccountsDetails.ObjectSet().AsQueryable();
            List<SettleAccountListDTO> data = new List<SettleAccountListDTO>();
            // 2. 查询商城下APP
            var mallQuery = MallApply.ObjectSet().Where(m => m.EsAppId == dto.EsAppId && (m.State.Value == 2 || m.State.Value == 4));
            var appIds = mallQuery.Select(m => m.AppId).Distinct().ToList();
            foreach (var appId in appIds)
            {
                var orderQuery = CommodityOrder.ObjectSet().Where(o =>
                    o.AppId == appId
                    && o.EsAppId == dto.EsAppId
                    && o.PaymentTime < compareDate
                    && o.Payment != 0);
                if (dto.StartDate.HasValue)
                {
                    var startDate = dto.StartDate.Value.Date;
                    orderQuery = orderQuery.Where(s => s.PaymentTime > startDate);
                }
                var orders = orderQuery.Join(CommodityOrderService.ObjectSet().Where(s => (s.State == 3 || s.State == 7 || s.State == 15)),
                    o => o.Id, s => s.Id,
                    (o, s) => new { s.State, o })
                    .Where(so => !hadAddOrderQuery.Where(h => h.EsAppId == dto.EsAppId && h.AppId == appId)
                    .Any(h => h.Id == so.o.Id))
                    .ToList();
                if (orders.Count > 0)
                {
                    // 生成结算项
                    foreach (var so in orders)
                    {
                        SettleAccountHelper.CreateSettleAccountDetails(contextSession, so.o, so.State);
                    }
                }
            }
            try
            {
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountBP.CreateSettleAccountDetailsExt异常: {1}，入参: {0}", JsonHelper.JsonSerializer(dto), ex));
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }

        /// <summary>
        ///  获取结算单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO<SettleAccountDetailsDTO> GetSettleAccountDetailsExt(Guid id)
        {
            var s = SettleAccounts.FindByID(id);
            SettleAccountDetailsDTO date = new SettleAccountDetailsDTO()
            {
                Id = s.Id,
                AmountDate = s.AmountDate,
                AppId = s.AppId,
                AppName = s.AppName,
                CouponAmount = s.CouponAmount,
                EsAppName = s.EsAppName,
                OrderAmount = s.OrderAmount,
                OrderRealAmount = s.OrderRealAmount,
                PromotionAmount = s.PromotionAmount,
                PromotionCommissionAmount = s.PromotionCommissionAmount,
                RefundAmount = s.RefundAmount,
                Remark = s.Remark,
                SellerAmount = s.SellerAmount,
                SettleStatue = s.SettleStatue,
                State = s.State.Value,
                Code = s.Code,
                SellerType = s.SellerType,
                OrderYJBAmount = s.OrderYJBAmount,
                AmountStartDate = s.AmountStartDate
            };
            return new ResultDTO<SettleAccountDetailsDTO>() { isSuccess = true, Data = date };
        }

        /// <summary>
        /// 获取结算单订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountOrderDTO>> GetSettleAccountOrdersExt(SettleAccountOrderSearchDTO searchDto)
        {
            var query = SettleAccountsDetails.ObjectSet().Where(s => s.SAId == searchDto.Id);
            var count = query.Count();
            var data = query.OrderBy(q => q.OrderCode).Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize).Select(s => new SettleAccountOrderDTO()
            {
                Id = s.Id,
                CouponAmount = s.OrderCouponAmount,
                OrderAmount = s.OrderAmount,
                OrderRealAmount = s.OrderRealAmount,
                OrderCode = s.OrderCode,
                OrderSubTime = s.OrderSubTime,
                PromotionAmount = s.PromotionAmount,
                PromotionCommissionAmount = s.OrderPromotionCommissionAmount,
                RefundAmount = s.OrderRefundAmount,
                SellerAmount = s.SellerAmount,
                IsMallCoupon = s.IsMallCoupon,
                OrderYJBAmount = s.OrderYJBAmount,
                OrderFreight = s.OrderFreight,
                SettleAmount = s.SettleAmount
            }).ToList();

            return new ResultDTO<ListResult<SettleAccountOrderDTO>>()
            {
                isSuccess = true,
                Data = new ListResult<SettleAccountOrderDTO> { List = data, Count = count }
            };
        }

        /// <summary>
        ///  获取结算单订单项详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountOrderItemDTO>> GetSettleAccountOrderItemsExt(Guid id)
        {
            var data = SettleAccountsOrderItem.ObjectSet().Where(s => s.OrderId == id).Select(s => new SettleAccountOrderItemDTO()
            {
                BaseCommission = s.BaseCommission,
                CategoryCommission = s.CategoryCommission,
                CommodityCommission = s.CommodityCommission,
                Name = s.OrderItemName,
                Number = s.OrderItemNumber,
                Price = s.OrderItemPrice,
                PromotionAmount = s.PromotionAmount,
                PromotionCommissionAmount = s.OrderItemPromotionCommissionAmount,
                OrderItemYJBAmount = s.OrderItemYJBAmount,
                SettleAmount = s.SettleAmount
            }).ToList();
            return new ResultDTO<ListResult<SettleAccountOrderItemDTO>>()
            {
                isSuccess = true,
                Data = new ListResult<SettleAccountOrderItemDTO> { List = data, Count = data.Count }
            };
        }

        /// <summary>
        /// 获取商家结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountListDTO>> GetMallSettleAccountHistoriesExt(SettleAccountHistorySearchDTO searchDto)
        {
            if (searchDto.EsAppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { ResultCode = 1, Message = "参数为空" };
            }
            var query = SettleAccounts.ObjectSet().Where(s => s.EsAppId == searchDto.EsAppId && s.State.Value == 3);
            if (searchDto.SettleStatue.HasValue)
            {
                query = query.Where(q => q.SettleStatue == searchDto.SettleStatue);
            }
            if (!string.IsNullOrWhiteSpace(searchDto.AppName))
            {
                searchDto.AppName = searchDto.AppName.Trim();
                query = query.Where(q => q.AppName.Contains(searchDto.AppName));
            }
            if (searchDto.SellerType.HasValue)
            {
                query = query.Where(q => q.SellerType == searchDto.SellerType);
            }
            query = query.WhereDate(searchDto.StartDate.HasValue ? searchDto.StartDate.Value.AddDays(-1) : searchDto.StartDate, searchDto.EndDate, "AmountDate");

            var count = query.Count();
            var data = query.OrderByDescending(q => q.AmountDate).
                Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize).Select(q => new SettleAccountListDTO
            {
                Id = q.Id,
                AmountDate = q.AmountDate,
                AppName = q.AppName,
                OrderAmount = q.OrderAmount,
                OrderRealAmount = q.OrderRealAmount,
                SellerAmount = q.SellerAmount,
                State = q.State.Value,
                SettleStatue = q.SettleStatue,
                Remark = q.Remark,
                SellerType = q.SellerType,
            }).ToList();
            return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, Data = new ListResult<SettleAccountListDTO> { List = data, Count = count } };
        }

        /// <summary>
        /// 获取商城结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public ResultDTO<ListResult<SettleAccountListDTO>> GetSellerSettleAccountHistoriesExt(SettleAccountHistorySearchDTO searchDto)
        {
            if (searchDto.AppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<SettleAccountListDTO>> { ResultCode = 1, Message = "参数为空" };
            }
            var query = SettleAccounts.ObjectSet().Where(s => s.AppId == searchDto.AppId && s.State.Value == 3);
            if (searchDto.SettleStatue.HasValue)
            {
                query = query.Where(q => q.SettleStatue == searchDto.SettleStatue);
            }
            if (!string.IsNullOrWhiteSpace(searchDto.EsAppName))
            {
                searchDto.EsAppName = searchDto.EsAppName.Trim();
                query = query.Where(q => q.EsAppName.Contains(searchDto.EsAppName));
            }
            if (searchDto.SellerType.HasValue)
            {
                query = query.Where(q => q.SellerType == searchDto.SellerType);
            }
            query = query.WhereDate(searchDto.StartDate, searchDto.EndDate, "AmountDate");

            var count = query.Count();
            var data = query.OrderByDescending(q => q.AmountDate).
                Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize).Select(q => new SettleAccountListDTO
                {
                    Id = q.Id,
                    AmountDate = q.AmountDate,
                    EsAppName = q.EsAppName,
                    OrderAmount = q.OrderAmount,
                    OrderRealAmount = q.OrderRealAmount,
                    SellerAmount = q.SellerAmount,
                    State = q.State.Value,
                    SettleStatue = q.SettleStatue,
                    Remark = q.Remark,
                    SellerType = q.SellerType
                }).ToList();
            return new ResultDTO<ListResult<SettleAccountListDTO>> { isSuccess = true, Data = new ListResult<SettleAccountListDTO> { List = data, Count = count } };
        }

        /// <summary>
        /// 获取商城的结算周期
        /// </summary>
        /// <param name="esAppId">商城Id</param>
        /// <returns>结算周期</returns>
        public ResultDTO<SettleAccountPeriodDTO> GetSettleAccountPeriodExt(System.Guid esAppId)
        {
            if (esAppId == Guid.Empty) return new ResultDTO<SettleAccountPeriodDTO> { ResultCode = 1, Message = "参数为空" };
            var period = SettleAccountsPeriod.FindByID(esAppId);
            if (period == null)
            {
                return new ResultDTO<SettleAccountPeriodDTO>() { isSuccess = true, Data = new SettleAccountPeriodDTO() { EsAppId = esAppId, NumOfDay = 7, UseAfterSalesEndTime = false } };
            }
            return new ResultDTO<SettleAccountPeriodDTO>() { isSuccess = true, Data = new SettleAccountPeriodDTO() { EsAppId = esAppId, NumOfDay = period.NumOfDay, UseAfterSalesEndTime = period.UseAfterSalesEndTime ?? false } };
        }

        /// <summary>
        /// 修改结算单状态(真正的生成结算单)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateStateExt(SettleAccountUpdateStateDTO dto)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var id in dto.Ids)
                {
                    var sa = SettleAccounts.FindByID(id);
                    if (sa == null) continue;
                    if (sa.State.Value != dto.State)
                    {
                        sa.State.Value = dto.State;
                        sa.ModifiedOn = DateTime.Now;
                        sa.EntityState = EntityState.Modified;
                        // 生成结算单
                        if (dto.State == 1)
                        {
                            var details = SettleAccountsDetails.ObjectSet().Where(s => s.SAId == sa.Id).ToList();
                            foreach (var item in details)
                            {
                                item.IsSettled = true;
                                item.EntityState = EntityState.Modified;
                                contextSession.SaveObject(item);
                            }
                        }
                    }
                }
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountBP.UpdateStatusExt异常，id={0}，status={1}", JsonHelper.JsonSerializer(dto.Ids), dto.State, ex));
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }

        /// <summary>
        /// 修改计算结果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateSettleStatueExt(SettleAccountUpdateSettleStatueDto dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var sa = SettleAccounts.FindByID(dto.Id);
                if (sa == null)
                    return new ResultDTO { ResultCode = 2, Message = "结算单不存在" };
                sa.SettleStatue = dto.SettleStatue;
                sa.Remark = dto.Remark;
                sa.ModifiedOn = DateTime.Now;
                sa.EntityState = EntityState.Modified;
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountBP.UpdateStatusExt异常，入参：", JsonHelper.JsonSerializer(dto), ex));
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }

        /// <summary>
        /// 修改商城的结算周期
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateSettleAccountPeriodExt(SettleAccountPeriodDTO dto)
        {
            if (!dto.UseAfterSalesEndTime)
            {
                if (dto.NumOfDay < 7)
                {
                    return new ResultDTO { ResultCode = 1, Message = "结算周期必须大于等于7天" };
                }
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var period = SettleAccountsPeriod.FindByID(dto.EsAppId);
            if (period == null)
            {
                period = new SettleAccountsPeriod()
                {
                    NumOfDay = dto.NumOfDay,
                    UseAfterSalesEndTime = dto.UseAfterSalesEndTime,
                    UserId = dto.UserId,
                    EntityState = EntityState.Added
                };
                period.Id = period.EsAppId = dto.EsAppId;
                period.ModifiedOn = period.SubTime = DateTime.Now;
            }
            else
            {
                period.ModifiedOn = DateTime.Now;
                period.NumOfDay = dto.NumOfDay;
                period.UseAfterSalesEndTime = dto.UseAfterSalesEndTime;
            }
            contextSession.SaveObject(period);
            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SettleAccountBP.UpdateSettleAccountPeriodExt异常，入参:{0}", JsonHelper.JsSerializer(dto)), ex);
                return new ResultDTO { Message = "异常，请重试！" };
            }
            return new ResultDTO { isSuccess = true };
        }


        private List<int> GetDirectArrivalPayment()
        {
            List<int> paymentList = new List<int>();
            try
            {
                var pQuery = (from p in PaySource.GetAllPaySources()
                              where p.TradeType == 1
                              select p.Payment).ToList();
                if (pQuery != null && pQuery.Any())
                {
                    paymentList.AddRange(pQuery);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetPaymentByTradeType异常,异常信息:", ex);
            }
            return paymentList;
        }

        /// <summary>
        /// 获取商品的结算价
        /// </summary>
        public ResultDTO<ListResult<CommoditySettleAmountListDTO>> GetCommoditySettleAmountExt(CommoditySettleAmountSearchDTO searchDto)
        {
            try
            {
                var result = new List<CommoditySettleAmountListDTO>();
                var commoditesQuery = Commodity.ObjectSet().Where(a => a.AppId == searchDto.AppId && !a.IsDel);
                var commoditySettleAmountQuery = CommoditySettleAmount.ObjectSet().AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchDto.Name))
                {
                    searchDto.Name = searchDto.Name.Trim();
                    commoditesQuery = commoditesQuery.Where(c => c.Name.Contains(searchDto.Name));
                }
                if (searchDto.HasSetted.HasValue)
                {
                    if (searchDto.HasSetted.Value)
                    {
                        commoditesQuery = commoditesQuery.Where(c => commoditySettleAmountQuery.Where(s => s.CommodityId == c.Id).Any());
                    }
                    else
                    {
                        commoditesQuery = commoditesQuery.Where(c => !commoditySettleAmountQuery.Where(s => s.CommodityId == c.Id).Any());
                    }
                }
                var count = commoditesQuery.Count();
                var tempComs = commoditesQuery.OrderByDescending(q => q.SubTime).Select(q => new
                {
                    Id = q.Id,
                    Name = q.Name,
                    Price = q.Price,
                    ComAttribute = q.ComAttribute
                }).Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize);

                List<CommoditySettleAmountListDTO> data = new List<CommoditySettleAmountListDTO>();
                var comIdsd = tempComs.Select(t => t.Id).ToList();
                var comStocks = CommodityStock.ObjectSet().Where(c => comIdsd.Contains(c.CommodityId));
                var comSettleAmounts = CommoditySettleAmount.ObjectSet().Where(c => comIdsd.Contains(c.CommodityId));

                foreach (var com in tempComs)
                {
                    var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute)
                        .Where(a => !string.IsNullOrEmpty(a.Attribute) && !string.IsNullOrEmpty(a.SecondAttribute))
                        .ToList();

                    CommoditySettleAmountListDTO d = new CommoditySettleAmountListDTO();
                    d.CommodityId = com.Id;
                    d.Name = com.Name;

                    var firstCSA = comSettleAmounts.Where(c => c.CommodityId == com.Id).OrderByDescending(c => c.EffectiveTime).ThenByDescending(c => c.SubTime).FirstOrDefault();
                    if (firstCSA != null)
                    {
                        d.EffectiveTime = firstCSA.EffectiveTime;
                    }
                    if (attrs != null && attrs.Count > 1)
                    {
                        //{"Attribute":"颜色","SecondAttribute":"水电费"},
                        //{"Attribute":"颜色","SecondAttribute":"问问"},
                        //[{"Attribute":"尺寸","SecondAttribute":"阿斯顿"},
                        //{"Attribute":"尺寸","SecondAttribute":"的风格都是"}]
                        var attrNames = attrs.GroupBy(a => a.Attribute).Select(g => g.Key).ToList();
                        if (attrNames.Count > 1)
                        {
                            // 双属性
                            d.AttributeName = attrNames[0];
                            d.SecondAttributeName = attrNames[1];
                            d.HasSecondAttribute = d.HasAttributes = true;
                            d.Attributes = new List<CommodityAttribute>();
                            foreach (var firstAttr in attrs.Where(a => a.Attribute == d.AttributeName))
                            {
                                var comAttr = new CommodityAttribute();
                                comAttr.HasAttributes = true;
                                comAttr.AttributeValue = firstAttr.SecondAttribute;
                                comAttr.Attributes = new List<CommodityAttribute>();

                                foreach (var secondAttr in attrs.Where(a => a.Attribute == d.SecondAttributeName))
                                {
                                    // 查找Stock
                                    var stockAttrStr1 = "[{\"Attribute\":\"" + firstAttr.Attribute + "\",\"SecondAttribute\":\"" + firstAttr.SecondAttribute + "\"},{\"Attribute\":\"" + secondAttr.Attribute + "\",\"SecondAttribute\":\"" + secondAttr.SecondAttribute + "\"}]";
                                    var stockAttrStr2 = "[{\"Attribute\":\"" + secondAttr.Attribute + "\",\"SecondAttribute\":\"" + secondAttr.SecondAttribute + "\"},{\"Attribute\":\"" + firstAttr.Attribute + "\",\"SecondAttribute\":\"" + firstAttr.SecondAttribute + "\"}]";
                                    var comStock = comStocks.Where(c => c.CommodityId == com.Id && (c.ComAttribute == stockAttrStr1 || c.ComAttribute == stockAttrStr2)).FirstOrDefault();
                                    var comSettleAmount = comSettleAmounts
                                        .Where(c => c.CommodityId == com.Id)
                                        .OrderByDescending(a => a.SubTime).ThenByDescending(a => a.EffectiveTime)
                                        .FirstOrDefault();

                                    var secondComAttr = new CommodityAttribute();
                                    //secondComAttr.Attribute = stockAttrStr1;
                                    secondComAttr.AttributeValue = secondAttr.SecondAttribute;
                                    if (comStock == null)
                                    {
                                        secondComAttr.Price = com.Price;
                                        LogHelper.Info("商品库存记录不存在，商品ID：" + com.Id + "，商品SKU：" + stockAttrStr1);
                                    }
                                    else
                                    {
                                        secondComAttr.Price = comStock.Price;
                                    }

                                    if (comSettleAmount != null)
                                    {
                                        var currentSettle = JsonHelper.JsonDeserialize<List<CommodityAttributePrice>>(comSettleAmount.CommodityAttrJson)
                                            .Where(_ => _.AttributeValue == firstAttr.SecondAttribute && _.SecAttributeValue == secondAttr.SecondAttribute)
                                            .FirstOrDefault();
                                        if (currentSettle != null)
                                        {
                                            secondComAttr.SettlePrice = currentSettle.SettlePrice;
                                        }
                                    }
                                    comAttr.Attributes.Add(secondComAttr);
                                }
                                d.Attributes.Add(comAttr);
                            }
                        }
                        else
                        {
                            // 单属性
                            d.AttributeName = attrNames[0];
                            d.HasAttributes = true;
                            d.HasSecondAttribute = false;
                            d.Attributes = new List<CommodityAttribute>();
                            foreach (var attr in attrs)
                            {
                                // [{"Attribute":"容积","SecondAttribute":"3L"}]
                                var attrStr = "[" + JsonHelper.JsonSerializer(attr) + "]";
                                var comStock = comStocks.Where(c => c.CommodityId == com.Id && c.ComAttribute == attrStr).FirstOrDefault();
                                var comSettleAmount = comSettleAmounts.Where(c => c.CommodityId == com.Id)
                                    .OrderByDescending(a => a.SubTime).ThenByDescending(a => a.EffectiveTime)
                                    .FirstOrDefault();

                                var comAttr = new CommodityAttribute();
                                comAttr.HasAttributes = false;
                                comAttr.AttributeValue = attr.SecondAttribute;

                                if (comStock == null)
                                {
                                    comAttr.Price = com.Price;
                                    LogHelper.Info("商品库存记录不存在，商品ID：" + com.Id + "，商品SKU：" + attrStr);
                                }
                                else
                                {
                                    comAttr.Price = comStock.Price;
                                }
                                if (comSettleAmount != null)
                                {
                                    var currentSettle = JsonHelper.JsonDeserialize<List<CommodityAttributePrice>>(comSettleAmount.CommodityAttrJson)
                                        .Where(_ => _.AttributeValue == attr.SecondAttribute)
                                        .FirstOrDefault();
                                    if (currentSettle != null)
                                    {
                                        comAttr.SettlePrice = currentSettle.SettlePrice;
                                    }
                                }
                                d.Attributes.Add(comAttr);
                            }
                        }
                    }
                    else
                    {
                        var comSettleAmount = comSettleAmounts
                            .Where(c => c.CommodityId == com.Id)
                             .OrderByDescending(a => a.SubTime).ThenByDescending(a => a.EffectiveTime)
                            .FirstOrDefault();
                        d.HasSecondAttribute = d.HasAttributes = false;
                        d.Price = com.Price;
                        if (comSettleAmount != null)
                        {
                            var currentSettle = JsonHelper.JsonDeserialize<List<CommodityAttributePrice>>(comSettleAmount.CommodityAttrJson)
                                .FirstOrDefault();
                            if (currentSettle != null)
                            {
                                d.SettlePrice = currentSettle.SettlePrice;
                            }
                        }
                    }
                    data.Add(d);
                }

                return new ResultDTO<ListResult<CommoditySettleAmountListDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<CommoditySettleAmountListDTO>
                    {
                        Count = count,
                        List = data
                    }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("SettleAccountBP.GetCommoditySettleAmountExt 异常，输入：" + JsonHelper.JsonSerializer(searchDto), ex);
                return new ResultDTO<ListResult<CommoditySettleAmountListDTO>> { isSuccess = false, Message = ex.Message };
            }
        }


        /// <summary>
        /// 设置商品的结算价
        /// </summary>
        public ResultDTO SetCommoditySettleAmountExt(CommoditySettleAmountInputDTO input)
        {
            if (input.EsAppId == Guid.Empty)
            {
                return new ResultDTO { isSuccess = false, Message = "请输入EsAPPID。" };
            }
            if (input.AppId == Guid.Empty)
            {
                return new ResultDTO { isSuccess = false, Message = "请输入APPID。" };
            }
            if (!input.EffectiveTime.HasValue)
            {
                return new ResultDTO { isSuccess = false, Message = "请输入生效时间。" };
            }
            if (input.Items.Count == 0)
            {
                return new ResultDTO { isSuccess = false, Message = "请输入结算价。" };
            }
            foreach (var item in input.Items)
            {
                if (!item.SettlePrice.HasValue)
                {
                    return new ResultDTO { isSuccess = false, Message = "请输入结算价。" };
                }
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            CommoditySettleAmount commoditySA = CommoditySettleAmount.CreateCommoditySettleAmount();
            var commodity = Commodity.FindByID(input.CommodityId);
            var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodity.ComAttribute)
                .Where(a => !string.IsNullOrEmpty(a.Attribute) && !string.IsNullOrEmpty(a.SecondAttribute))
                .ToList();
            if (attrs != null && attrs.Count > 1)
            {
                var attrNames = attrs.GroupBy(a => a.Attribute).Select(g => g.Key).ToList();
                if (attrNames.Count > 1)
                {
                    commoditySA.CommodityType = 2;
                    commoditySA.CommodityAttrName = attrNames[0];
                    commoditySA.CommoditySecAttrName = attrNames[1];

                }
                else
                {
                    commoditySA.CommodityType = 1;
                    commoditySA.CommodityAttrName = attrNames[0];
                }
            }
            else
            {
                commoditySA.CommodityType = 0;
            }
            commoditySA.AppId = input.AppId;
            commoditySA.UserId = input.UserId;
            commoditySA.UserName = CBCSV.GetUserNameAndCode(commoditySA.UserId).Item1;
            commoditySA.EffectiveTime = input.EffectiveTime.Value;
            commoditySA.CommodityId = input.CommodityId;
            commoditySA.CommodityAttrJson = JsonHelper.JsonSerializer(input.Items);
            contextSession.SaveObject(commoditySA);
            contextSession.SaveChanges();
            return new ResultDTO { isSuccess = true };
        }


        public ResultDTO<ListResult<CommoditySettleAmountHistoryDTO>> GetCommoditySettleAmountHistoriesExt(Guid commodityId, int pageIndex, int pageSize)
        {
            var sasQuery = CommoditySettleAmount.ObjectSet().Where(a => a.CommodityId == commodityId)
                .OrderByDescending(a => a.SubTime);
            var sas = sasQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            List<CommoditySettleAmountHistoryDTO> data = new List<CommoditySettleAmountHistoryDTO>();
            foreach (var sa in sas)
            {
                var saAttrs = JsonHelper.JsonDeserialize<List<CommodityAttributePrice>>(sa.CommodityAttrJson);
                CommoditySettleAmountHistoryDTO item = new CommoditySettleAmountHistoryDTO();
                item.Id = sa.Id;
                item.UserName = sa.UserName;
                item.EffectiveTime = sa.EffectiveTime;
                item.SubTime = sa.SubTime;
                item.AttributeName = sa.CommodityAttrName;
                item.SecondAttributeName = sa.CommoditySecAttrName;
                item.HasAttributes = item.HasSecondAttribute = false;
                if (sa.CommodityType == 0)
                {
                    item.Price = saAttrs[0].Price;
                    item.SettlePrice = saAttrs[0].SettlePrice.Value;
                }
                else if (sa.CommodityType == 1)
                {
                    item.HasAttributes = true;
                    item.Attributes = new List<CommodityAttribute>();
                    foreach (var saAttr in saAttrs)
                    {
                        var tempAttr = new CommodityAttribute();
                        tempAttr.AttributeValue = saAttr.AttributeValue;
                        tempAttr.Price = saAttr.Price;
                        tempAttr.SettlePrice = saAttr.SettlePrice;
                        item.Attributes.Add(tempAttr);
                    }
                }
                else
                {
                    // 双属性
                    item.HasAttributes = true;
                    item.HasSecondAttribute = true;
                    item.Attributes = new List<CommodityAttribute>();
                    foreach (var attrVaue in saAttrs.GroupBy(s => s.AttributeValue).Select(g => g.Key).ToList())
                    {
                        var tempAttr = new CommodityAttribute();
                        tempAttr.AttributeValue = attrVaue;
                        tempAttr.Attributes = new List<CommodityAttribute>();
                        item.Attributes.Add(tempAttr);
                        foreach (var saSecAttr in saAttrs.Where(a => a.AttributeValue == tempAttr.AttributeValue && a.SecAttributeName == item.SecondAttributeName))
                        {
                            var tempSecAttr = new CommodityAttribute();
                            tempSecAttr.AttributeValue = saSecAttr.SecAttributeValue;
                            tempSecAttr.Price = saSecAttr.Price;
                            tempSecAttr.SettlePrice = saSecAttr.SettlePrice;
                            tempAttr.Attributes.Add(tempSecAttr);
                        }
                    }

                    //foreach (var saAttr in saAttrs.Where(a => a.AttributeName == item.AttributeName))
                    //{
                    //    var tempAttr = new CommodityAttribute();
                    //    tempAttr.AttributeValue = saAttr.AttributeValue;
                    //    tempAttr.Attributes = new List<CommodityAttribute>();
                    //    item.Attributes.Add(tempAttr);
                    //    foreach (var saSecAttr in saAttrs.Where(a => a.AttributeValue == tempAttr.AttributeValue && a.SecAttributeName == item.SecondAttributeName))
                    //    {
                    //        var tempSecAttr = new CommodityAttribute();
                    //        tempSecAttr.AttributeValue = saSecAttr.SecAttributeValue;
                    //        tempSecAttr.Price = saSecAttr.Price;
                    //        tempSecAttr.SettlePrice = saSecAttr.SettlePrice;
                    //        tempAttr.Attributes.Add(tempSecAttr);
                    //    }
                    //}
                }
                data.Add(item);
            }

            return new ResultDTO<ListResult<CommoditySettleAmountHistoryDTO>>
            {
                isSuccess = true,
                Data = new ListResult<CommoditySettleAmountHistoryDTO>
                {
                    Count = sasQuery.Count(),
                    List = data
                }
            };
        }

        public ResultDTO DeleteCommoditySettleAmountHistoryExt(Guid id)
        {
            var csa = CommoditySettleAmount.FindByID(id);
            if (csa != null)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.Delete(csa);
                contextSession.SaveChange();
            }
            return new ResultDTO { isSuccess = true };
        }

    }
}
