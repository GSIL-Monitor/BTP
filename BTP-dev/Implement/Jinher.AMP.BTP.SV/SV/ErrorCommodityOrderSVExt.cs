
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/2/18 15:42:38
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ErrorCommodityOrderSV : BaseSv, IErrorCommodityOrder
    {
        /// <summary>
        /// Job自动处理取消订单时回退积分
        /// </summary>
        public void AutoDealOrderCancelSroreExt()
        {
            LogHelper.Info(string.Format("Job处理取消订单时回退积分的服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                while (true)
                {
                   List<int> state =new List<int>(){4,5,6};
                    //DateTime lastday = now;
                   var errorCommodityOrderList = ErrorCommodityOrder.ObjectSet().Where(t => t.State == 0 && state.Contains(t.Source) && t.ResourceType == 1).Take(pageSize).ToList();

                   if (!errorCommodityOrderList.Any())
                        break;

                   LogHelper.Info(string.Format("Job处理取消订单时回退积分的服务处理订单数:{0}", errorCommodityOrderList.Count));

                   foreach (ErrorCommodityOrder errorCommodityOrder in errorCommodityOrderList)
                   {
                       bool result = SignSV.Instance.RefundScore(errorCommodityOrder.UserId, errorCommodityOrder.AppId, errorCommodityOrder.Score, errorCommodityOrder.ErrorOrderId, errorCommodityOrder.OrderCode, (ScoreTypeEnum)errorCommodityOrder.ScoreType);
                        if (!result)
                        {
                            continue;
                        }

                        errorCommodityOrder.State = 1;
                        errorCommodityOrder.ModifiedOn = DateTime.Now;
                        errorCommodityOrder.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveChanges();
                        
                   }
                   if (errorCommodityOrderList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("Job处理取消订单时回退积分的服务成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("Job处理取消订单时回退积分的服务异常。", ex);
            }
        }
        /// <summary>
        ///  Job自动处理售中退款时回退积分
        /// </summary>
        public void AutoDealOrderRefundScoreExt()
        {
            LogHelper.Info(string.Format("Job处理售中退款时回退积分的服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                while (true)
                {
                    //DateTime lastday = now;
                    var errorCommodityOrderList = ErrorCommodityOrder.ObjectSet().Where(t => t.State == 0 && t.Source == 7 && t.ResourceType == 1).Take(pageSize).ToList();

                    if (!errorCommodityOrderList.Any())
                        break;

                    LogHelper.Info(string.Format("Job处理售中退款时回退积分的服务处理订单数:{0}", errorCommodityOrderList.Count));

                    foreach (ErrorCommodityOrder errorCommodityOrder in errorCommodityOrderList)
                    {
                        bool result = SignSV.Instance.RefundScore(errorCommodityOrder.UserId, errorCommodityOrder.AppId, errorCommodityOrder.Score, errorCommodityOrder.ErrorOrderId, errorCommodityOrder.OrderCode, (ScoreTypeEnum)errorCommodityOrder.ScoreType);
                        if (!result)
                        {
                            continue;
                        }

                        errorCommodityOrder.State = 1;
                        errorCommodityOrder.ModifiedOn = DateTime.Now;
                        errorCommodityOrder.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveChanges();

                    }
                    if (errorCommodityOrderList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("Job处理售中退款时回退积分的服务成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("Job处理售中退款时回退积分的服务异常。", ex);
            }
        }
        /// <summary>
        ///  Job自动处理售后退款时回退积分
        /// </summary>
        public void AutoDealOrderAfterSalesRefundScoreExt()
        {
            LogHelper.Info(string.Format("Job处理售后退款时回退积分的服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                while (true)
                {
                    //DateTime lastday = now;
                    var errorCommodityOrderList = ErrorCommodityOrder.ObjectSet().Where(t => t.State == 0 && t.Source == 107 && t.ResourceType == 1).Take(pageSize).ToList();

                    if (!errorCommodityOrderList.Any())
                        break;

                    LogHelper.Info(string.Format("Job处理售后退款时回退积分的服务处理订单数:{0}", errorCommodityOrderList.Count));

                    foreach (ErrorCommodityOrder errorCommodityOrder in errorCommodityOrderList)
                    {
                        bool result = SignSV.Instance.RefundScore(errorCommodityOrder.UserId, errorCommodityOrder.AppId, errorCommodityOrder.Score, errorCommodityOrder.ErrorOrderId, errorCommodityOrder.OrderCode, (ScoreTypeEnum)errorCommodityOrder.ScoreType);
                        if (!result)
                        {
                            continue;
                        }

                        errorCommodityOrder.State = 1;
                        errorCommodityOrder.ModifiedOn = DateTime.Now;
                        errorCommodityOrder.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveChanges();

                    }
                    if (errorCommodityOrderList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("Job处理售后退款时回退积分的服务成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("Job处理售后退款时回退积分的服务异常。", ex);
            }
        }
        /// <summary>
        /// Job自动处理回退积分
        /// </summary>
        public void AutoRefundScoreExt()
        {
            LogHelper.Info(string.Format("Job处理回退积分的服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                while (true)
                {
                    //DateTime lastday = now;
                    var errorCommodityOrderList = ErrorCommodityOrder.ObjectSet().Where(t => t.State == 0 && t.ResourceType == 1).Take(pageSize).ToList();

                    if (!errorCommodityOrderList.Any())
                        break;

                    LogHelper.Info(string.Format("Job处理回退积分的服务处理订单数:{0}", errorCommodityOrderList.Count));

                    foreach (ErrorCommodityOrder errorCommodityOrder in errorCommodityOrderList)
                    {
                        bool result = SignSV.Instance.RefundScore(errorCommodityOrder.UserId, errorCommodityOrder.AppId, errorCommodityOrder.Score, errorCommodityOrder.ErrorOrderId, errorCommodityOrder.OrderCode, (ScoreTypeEnum)errorCommodityOrder.ScoreType);
                        if (!result)
                        {
                            continue;
                        }
                        errorCommodityOrder.State = 1;
                        errorCommodityOrder.ModifiedOn = DateTime.Now;
                        errorCommodityOrder.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveChanges();

                    }
                    if (errorCommodityOrderList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("Job处理回退积分的服务成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("Job处理回退积分的服务异常。", ex);
            }
        }
    }
}