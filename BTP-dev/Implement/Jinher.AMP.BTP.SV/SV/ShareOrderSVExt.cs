using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.SNS.ISV.Facade;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Runtime.Serialization.Json;
using System.IO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Finance.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using System.Data;
using ReturnInfoDTO = Jinher.AMP.Finance.Deploy.CustomDTO.ReturnInfoDTO;


namespace Jinher.AMP.BTP.SV
{
    public partial class ShareOrderSV : BaseSv, IShareOrder
    {
        /// <summary>
        /// 获取众销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO GetShareOrderMoneySumInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search)
        {
            if (search == null || search.UseId == Guid.Empty || search.AppId == Guid.Empty)
            {
                return null;
            }
            Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO result = new ShareOrderMoneySumDTO();

            //售中待收益
            var orderStates = new List<int>() { 1, 2, 8, 9, 10, 12, 13, 14 };
            //售后待收益
            var orderAfterStates = new List<int>() { 3, 5, 10, 12, 13 };

            //佣金总数（已收益佣金）
            var sumInfo = (from orderShare in OrderShare.ObjectSet()
                           join order in CommodityOrder.ObjectSet()
                               on orderShare.OrderId equals order.Id
                           join dataS in CommodityOrderService.ObjectSet()
                               on orderShare.OrderId equals dataS.Id
                               into tempS
                           from orderService in tempS.DefaultIfEmpty()
                           where
                               orderShare.PayeeId == search.UseId && orderShare.PayeeType == search.PayeeType && orderShare.Commission > 0 &&
                               order.EsAppId == search.AppId && order.State == 3 && orderService.State == 15
                           select orderShare.Commission).ToList().Sum();

            //待收益佣金总数
            var sumUnPayInfo = (from orderShare in OrderShare.ObjectSet()
                                join order in CommodityOrder.ObjectSet()
                                    on orderShare.OrderId equals order.Id
                                join dataS in CommodityOrderService.ObjectSet()
                                    on orderShare.OrderId equals dataS.Id
                                    into tempS
                                from orderService in tempS.DefaultIfEmpty()
                                where
                                    orderShare.PayeeId == search.UseId && orderShare.PayeeType == search.PayeeType && orderShare.Commission > 0 &&
                                    order.EsAppId == search.AppId && (orderStates.Contains(order.State) ||
                                    order.State == 3 && orderService.State != null && orderAfterStates.Contains(orderService.State))
                                select orderShare.Commission).ToList().Sum();

            result.CommissionAmount = sumInfo;
            result.CommmissionUnPay = sumUnPayInfo;

            return result;
        }

        /// <summary>
        /// 获取众销入账信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO GetShareOrderMoneyInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO result = new ShareOrderMoneyResultDTO();

            if (search == null || search.UseId == Guid.Empty || search.AppId == Guid.Empty || search.PageIndex < 1 || search.PageSize < 1)
            {
                return result;
            }

            //售中待收益
            var orderStates = new List<int>() { 1, 2, 8, 9, 10, 12, 13, 14 };
            //售后待收益
            var orderAfterStates = new List<int>() { 3, 5, 10, 12, 13 };
            //已收益
            if (search.SearchType == 1)
            {
                var query = from orderShare in OrderShare.ObjectSet()
                            join order in CommodityOrder.ObjectSet()
                                on orderShare.OrderId equals order.Id
                            join dataS in CommodityOrderService.ObjectSet()
                                on orderShare.OrderId equals dataS.Id
                                into tempS
                            from orderService in tempS.DefaultIfEmpty()
                            where
                                orderShare.PayeeId == search.UseId && orderShare.PayeeType == search.PayeeType && orderShare.Commission > 0 &&
                                order.EsAppId == search.AppId && order.State == 3 && orderService.State == 15
                            select new ShareOrderMoneyDTO
                                {
                                    SortTime = orderService.EndTime.Value,
                                    Money = orderShare.Commission,
                                    State = 0,
                                    SrcType = order.SrcType
                                };
                result.Count = query.Count();



                result.ShareOrderMoneyList = query.OrderByDescending(n => n.SortTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

            }
            //待收益
            else if (search.SearchType == 2)
            {
                var query = from orderShare in OrderShare.ObjectSet()
                            join order in CommodityOrder.ObjectSet()
                                on orderShare.OrderId equals order.Id
                            join dataS in CommodityOrderService.ObjectSet()
                                on orderShare.OrderId equals dataS.Id
                                into tempS
                            from orderService in tempS.DefaultIfEmpty()
                            where
                                orderShare.PayeeId == search.UseId && orderShare.PayeeType == search.PayeeType && orderShare.Commission > 0 &&
                                order.EsAppId == search.AppId && (orderStates.Contains(order.State) ||
                                                                  order.State == 3 && orderService.State != null &&
                                                                  orderAfterStates.Contains(orderService.State))
                            select new ShareOrderMoneyDTO
                                {
                                    SortTime = order.PaymentTime.Value,
                                    Money = orderShare.Commission,
                                    State = 1,
                                    SrcType = order.SrcType
                                };
                result.Count = query.Count();
                result.ShareOrderMoneyList =
                    query.OrderByDescending(n => n.SortTime)
                         .Skip((search.PageIndex - 1) * search.PageSize)
                         .Take(search.PageSize)
                         .ToList();

            }
            else
            {

            }

            return result;
        }

    }
}
