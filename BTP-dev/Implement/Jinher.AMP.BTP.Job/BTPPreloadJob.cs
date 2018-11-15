using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{
    public class BTPPreloadJob : IPreloadJob
    {
        public void PreloadJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            Console.WriteLine("BTPPreloadJob运行 Start");

            scheduler.JobGroupNames.Contains("Jinher.AMP.BTP.Job");

            // 同步京东价格
            //SyncJdPriceJob(scheduler, param);

            //京东自动下订单Job
            //AddJdOrderJob(scheduler, param);

            //追踪物流信息Job
            //AddOTMSJob(scheduler, param);

            //追踪服务项设置job
            //AddServiceSettingJob(scheduler, param);

            //全量同步严选价格,库存
            //SyncAllYXPriceJob(scheduler, param);



            //处理订单
            //AddOrderDealJobNow(scheduler, param);
            //AddOrderDealJob(scheduler, param);
            //处理售后订单
            //AddOrderDealAfterSalesJobNow(scheduler, param);
            //AddOrderDealAfterSalesJob(scheduler, param);
            //处理缓存
            //AddCacheJobNow(scheduler, param);
            //AddCacheJob(scheduler, param);
            //处理热门商品
            //AddHotCommodityJobNow(scheduler, param);
            //AddHotCommodityJob(scheduler, param);
            //处理促销推送
            //AddPromotionPushJob(scheduler, param);
            //处理评价表用户更新
            //AddUserInfoUpdateJogNow(scheduler, param);
            //AddUserInfoUpdateJog(scheduler, param);
            //更新易捷员工表无效用户信息
            //AddEmployeeUpdateJob(scheduler, param);

            //处理促销活动消息发送广场
            //AddPromotionPushIUSJob(scheduler, param);

            //处理过期红包Job
            //HandleInValidRedJob(scheduler, param);
            //HandleInValidRedJobNow(scheduler, param);

            //结算众销佣金
            //SettleSaleCommissionJob(scheduler, param);
            //SettleSaleCommissionJobNow(scheduler, param);
            //发送红包Job
            //SendShareRedJob(scheduler, param);
            //SendShareRedJobNow(scheduler, param);

            //众筹每日计算job
            //CrowdfundingCalcJob(scheduler, param);
            //CrowdfundingCalcJobNow(scheduler, param);

            //发送众筹红包Job
            //SendCfRedJob(scheduler, param);
            //SendCfRedJobNow(scheduler, param);

            AddOrderExpirePayJob(scheduler, param);

            //补发订单数据到盈科
            AddSendOrderInfoToYKBDMqJob(scheduler, param);

            //处理商品缓存
            //AddCommodityJob(scheduler, param);
            //AddCommodityJobNow(scheduler, param);

            //处理正品会APP缓存
            //AddAppSetJob(scheduler, param);
            //AddAppSetJobNow(scheduler, param);

            //AddOrderRepaireJobJob(scheduler, param);

            //快递路由。
            //AddOrderExpressRouteJob(scheduler, param);

            //处理ErrorCommodityOrder
            //AddErrorCommodityOrderJob(scheduler, param);
            //AddErrorCommodityOrderJobNow(scheduler, param);

            //服务订单
            //AddServiceOrderDealJob(scheduler, param);

            //拼团
            //AddDiyGroupJob(scheduler, param);

            //商品改低价格推送
            //AddCommodityModifyPriceJob(scheduler, param);

            //中石化电子发票 补发错误发票请求以及下载电子发票接口调用
            AddDownloadEInvoiceInfoJob(scheduler, param);

            //易捷币抵现订单 按照商品进行拆分
            //AddUpdateOrderItemYjbPriceJob(scheduler, param);

            //苏宁订单及物流数据同步
            SyncSuningDataJob(scheduler, param);
            Console.WriteLine("BTPPreloadJob运行 END");
        }

        #region 用户信息
        private void AddUserInfoUpdateJog(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("UserInfoUpdateJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("UserInfoUpdateJob", "Jinher.AMP.BTP.Job", typeof(UserInfoUpdateJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("UserInfoUpdateTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "UserInfoUpdateJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddUserInfoUpdateJogNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("UserInfoUpdateJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("UserInfoUpdateJobNow", "Jinher.AMP.BTP.Job", typeof(UserInfoUpdateJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("UserInfoUpdateTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "UserInfoUpdateJob触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 易捷员工信息更新
        private void AddEmployeeUpdateJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("EmployeeUpdateJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("EmployeeUpdateJob", "Jinher.AMP.BTP.Job", typeof(EmployeeUpdateJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("EmployeeUpdateJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "EmployeeUpdateJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion

        #region 促销推送
        private void AddPromotionPushJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("PromotionPushJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("PromotionPushJob", "Jinher.AMP.BTP.Job", typeof(PromotionPushJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("PromotionPushTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(10));
            trigger.Description = "PromotionPushJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion

        #region 处理促销活动消息发送广场
        private void AddPromotionPushIUSJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("PromotionPushIUSJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("PromotionPushIUSJob", "Jinher.AMP.BTP.Job", typeof(PromotionPushIUSJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("PromotionPushIUSTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "PromotionPushIUSJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion

        #region 订单处理
        private void AddOrderDealJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderDealJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderDealJob", "Jinher.AMP.BTP.Job", typeof(OrderDealJob));
            job.JobDataMap.PutAll(param);

            //DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-7);//北京时间领先UTC 8个小时
            DateTime start = DateTime.UtcNow;
            //每天零点运行
            SimpleTrigger trigger = new SimpleTrigger("OrderDealTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(12));
            trigger.Description = "OrderDealJob触发器";

            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddOrderDealJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderDealJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderDealJobNow", "Jinher.AMP.BTP.Job", typeof(OrderDealJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("OrderDealTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "OrderDealJob触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }

        private void AddSendOrderInfoToYKBDMqJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SendOrderInfoToYKBDMqJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SendOrderInfoToYKBDMqJob", "Jinher.AMP.BTP.Job", typeof(SendOrderInfoToYKBDMqJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //每隔12小时执行一次
            SimpleTrigger trigger = new SimpleTrigger("SendOrderInfoToYKBDMqTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(12));
            trigger.Description = "SendOrderInfoToYKBDMqJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        private void AddOrderExpirePayJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderExpirePayJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderExpirePayJob", "Jinher.AMP.BTP.Job", typeof(OrderExpirePayJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔5分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("OrderExpirePayTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(1));
            trigger.Description = "OrderExpirePayJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        private void AddOrderRepaireJobJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderRepaireJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderRepaireJob", "Jinher.AMP.BTP.Job", typeof(OrderRepaireJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔2分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("OrderRepaireJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(2));
            trigger.Description = "OrderRepaireJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        private void AddDownloadEInvoiceInfoJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("DownloadEInvoiceInfoJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("DownloadEInvoiceInfoJob", "Jinher.AMP.BTP.Job", typeof(DownloadEInvoiceInfoJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔1小时执行一次
            SimpleTrigger trigger = new SimpleTrigger("DownloadEInvoiceInfoJob", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(1));
            trigger.Description = "DownloadEInvoiceInfoJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        private void AddUpdateOrderItemYjbPriceJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("UpdateOrderItemYjbPriceJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("UpdateOrderItemYjbPriceJob", "Jinher.AMP.BTP.Job", typeof(UpdateOrderItemYjbPriceJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔1分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("UpdateOrderItemYjbPriceJob", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(1));
            trigger.Description = "UpdateOrderItemYjbPriceJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }
        #endregion

        #region 售后订单处理
        private void AddOrderDealAfterSalesJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderDealAfterSalesJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderDealAfterSalesJob", "Jinher.AMP.BTP.Job", typeof(OrderDealAfterSalesJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-6);//北京时间领先UTC 8个小时
            //每天零点运行
            SimpleTrigger trigger = new SimpleTrigger("OrderDealAfterSalesTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "OrderDealAfterSalesJob触发器";

            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddOrderDealAfterSalesJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderDealAfterSalesJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderDealAfterSalesJobNow", "Jinher.AMP.BTP.Job", typeof(OrderDealAfterSalesJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("OrderDealAfterSalesTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "OrderDealAfterSalesJob触发器";
            scheduler.ScheduleJob(job, triggerNow);

        }

        #endregion

        #region 缓存
        private void AddCacheJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CacheDealJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CacheDealJob", "Jinher.AMP.BTP.Job", typeof(CacheDealJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            //每日零点更新商品促销缓存和每日促销数据表
            SimpleTrigger trigger = new SimpleTrigger("CacheDealTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "CacheDealJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddCacheJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CacheDealJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CacheDealJobNow", "Jinher.AMP.BTP.Job", typeof(CacheDealJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("CacheDealTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "CacheDealJob触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 热门商品
        private void AddHotCommodityJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("HotCommodityJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("HotCommodityJob", "Jinher.AMP.BTP.Job", typeof(HotCommodityJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("HotCommodityTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "HotCommodityJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddHotCommodityJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("HotCommodityJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("HotCommodityJobNow", "Jinher.AMP.BTP.Job", typeof(HotCommodityJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("HotCommodityTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "UserInfoUpdateJob触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 处理过期红包Job
        private void HandleInValidRedJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("HandleInValidRedJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("HandleInValidRedJob", "Jinher.AMP.BTP.Job", typeof(HandleInValidRedJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("HandleInValidRedTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "HandleInValidRed触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void HandleInValidRedJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("HandleInValidRedJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("HandleInValidRedJobNow", "Jinher.AMP.BTP.Job", typeof(HandleInValidRedJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("HandleInValidRedTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "HandleInValidRedNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 发送红包Job
        private void SendShareRedJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SendShareRedJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SendShareRedJob", "Jinher.AMP.BTP.Job", typeof(SendShareRedJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(1);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("SendShareRedTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "SendShareRed触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void SendShareRedJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SendShareRedJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SendShareRedJobNow", "Jinher.AMP.BTP.Job", typeof(SendShareRedJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("SendShareRedTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "SendShareRedNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 结算众销佣金
        private void SettleSaleCommissionJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SettleSaleCommissionJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SettleSaleCommissionJob", "Jinher.AMP.BTP.Job", typeof(SettleSaleCommissionJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("SettleSaleCommissionTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "SettleSaleCommission触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void SettleSaleCommissionJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SettleSaleCommissionJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SettleSaleCommissionJobNow", "Jinher.AMP.BTP.Job", typeof(SettleSaleCommissionJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("SettleSaleCommissionTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "SettleSaleCommissionNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 众筹每日计算
        private void CrowdfundingCalcJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CrowdfundingCalcJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CrowdfundingCalcJob", "Jinher.AMP.BTP.Job", typeof(CrowdfundingCalcJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("CrowdfundingCalcTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "CrowdfundingCalc触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void CrowdfundingCalcJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CrowdfundingCalcJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CrowdfundingCalcJobNow", "Jinher.AMP.BTP.Job", typeof(CrowdfundingCalcJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("CrowdfundingCalcTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "CrowdfundingCalcNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 众筹红包

        private void SendCfRedJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SendCfRedJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SendCfRedJob", "Jinher.AMP.BTP.Job", typeof(SendCfRedJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(1);//北京时间领先UTC 8个小时
            SimpleTrigger trigger = new SimpleTrigger("SendCfRedTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "SendCfRed触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void SendCfRedJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SendCfRedJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SendCfRedJobNow", "Jinher.AMP.BTP.Job", typeof(SendCfRedJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("SendCfRedJobTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "SendCfRedNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }
        #endregion

        #region 处理商品缓存

        private void AddCommodityJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CommodityDealJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CommodityDealJob", "Jinher.AMP.BTP.Job", typeof(CommodityDealJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            //每日零点商品详情的缓存
            SimpleTrigger trigger = new SimpleTrigger("CommodityDealJob", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "CommodityDealJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddCommodityJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CommodityDealJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CommodityDealJobNow", "Jinher.AMP.BTP.Job", typeof(CommodityDealJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("CommodityDealTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "CommodityDealJobNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }

        #endregion

        #region 处理正品会APP缓存

        private void AddAppSetJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("AppSetDealJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("AppSetDealJob", "Jinher.AMP.BTP.Job", typeof(AppSetDealJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-8);//北京时间领先UTC 8个小时
            //每日零点的正品会APP缓存
            SimpleTrigger trigger = new SimpleTrigger("AppSetDealJob", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "AppSetDealJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddAppSetJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("AppSetDealJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("AppSetDealJobNow", "Jinher.AMP.BTP.Job", typeof(AppSetDealJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("AppSetDealTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "AppSetDealJobNow触发器";
            scheduler.ScheduleJob(job, triggerNow);
        }

        #endregion


        #region 使用job重新订阅快递鸟物流信息（对订阅失败的）

        private void AddOrderExpressRouteJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OrderExpressRouteJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OrderExpressRouteJob", "Jinher.AMP.BTP.Job", typeof(OrderExpressRouteJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔5分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("OrderExpressRouteTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(10));
            trigger.Description = "OrderExpressRouteJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        #endregion

        #region 处理ErrorCommodityOrder

        private void AddErrorCommodityOrderJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("ErrorCommodityOrderJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("ErrorCommodityOrderJob", "Jinher.AMP.BTP.Job", typeof(ErrorCommodityOrderJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(-6);//北京时间领先UTC 8个小时
            //每天零点运行
            SimpleTrigger trigger = new SimpleTrigger("ErrorCommodityOrderTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger.Description = "ErrorCommodityOrderJob触发器";

            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        private void AddErrorCommodityOrderJobNow(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("ErrorCommodityOrderJobNow", "Jinher.AMP.BTP.Job");
            job = new JobDetail("ErrorCommodityOrderJobNow", "Jinher.AMP.BTP.Job", typeof(ErrorCommodityOrderJob));
            job.JobDataMap.PutAll(param);

            DateTime now = DateTime.UtcNow;//发布时运行一次
            SimpleTrigger triggerNow = new SimpleTrigger("ErrorCommodityOrderTriggerNow", "Jinher.AMP.BTP.Job", now);
            triggerNow.Description = "ErrorCommodityOrderJob触发器";
            scheduler.ScheduleJob(job, triggerNow);

        }

        #endregion


        #region 服务订单状态变化发出通知

        private void AddServiceOrderDealJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("ServiceOrderDealJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("ServiceOrderDealJob", "Jinher.AMP.BTP.Job", typeof(ServiceOrderDealJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔5分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("ServiceOrderTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(1));
            trigger.Description = "ServiceOrderDealJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        #endregion



        #region 使用job处理拼团（超时未成团 未成团退款）

        private void AddDiyGroupJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("DiyGroupJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("DiyGroupJob", "Jinher.AMP.BTP.Job", typeof(DiyGroupJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            //立即开始执行 每隔5分钟执行一次
            SimpleTrigger trigger = new SimpleTrigger("DiyGroupTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(1));
            trigger.Description = "DiyGroupJob触发器";

            scheduler.ScheduleJob(job, trigger);
        }

        #endregion

        #region 商品改低价格推送
        private void AddCommodityModifyPriceJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("CommodityModifyPriceJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("CommodityModifyPriceJob", "Jinher.AMP.BTP.Job", typeof(CommodityModifyPriceJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("CommodityModifyPriceTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(10));

            trigger.Description = "CommodityModifyPriceJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion


        #region 京东自动下订单自动修改订单
        private void AddJdOrderJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("JdOrderJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("JdOrderJob", "Jinher.AMP.BTP.Job", typeof(JdOrderJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("JdOrderJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(2));

            trigger.Description = "JdOrderJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion



        #region 追踪物流信息Job
        private void AddOTMSJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("OTMSJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("OTMSJob", "Jinher.AMP.BTP.Job", typeof(OTMSJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(1);
            SimpleTrigger trigger = new SimpleTrigger("OTMSJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));

            trigger.Description = "OTMSJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion


        #region 追踪服务项设置Job
        private void AddServiceSettingJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("ServiceSettingJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("ServiceSettingJob", "Jinher.AMP.BTP.Job", typeof(ServiceSettingJob));
            job.JobDataMap.PutAll(param);
            DateTime start = DateTime.UtcNow.Date.AddDays(1).AddHours(4);
            //DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("ServiceSettingJobTrigger", "Jinher.AMP.BTP.Job", start, null, Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));

            trigger.Description = "ServiceSettingJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }
        #endregion




        private void SyncJdPriceJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("JdSyncPriceJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("JdSyncPriceJob", "Jinher.AMP.BTP.Job", typeof(JdSyncPriceJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("JdSyncPriceJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(2));
            trigger.Description = "JdSyncPriceJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 全量同步严选商品价格和库存
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="param"></param>
        private void SyncAllYXPriceJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job1 = scheduler.GetJobDetail("YXSyncAllPriceJob1", "Jinher.AMP.BTP.Job");
            job1 = new JobDetail("YXSyncAllPriceJob1", "Jinher.AMP.BTP.Job", typeof(YXSyncAllPriceJob));
            job1.JobDataMap.PutAll(param);

            //北京时间领先UTC 8个小时
            //每天10点运行
            SimpleTrigger trigger1 = new SimpleTrigger("YXSyncAllPriceJobTrigger1", "Jinher.AMP.BTP.Job", DateTime.UtcNow.Date.AddDays(1).AddHours(2), null,
            Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger1.Description = "YXSyncAllPriceJobTrigger1触发器";
            scheduler.ScheduleJob(job1, trigger1);

            JobDetail job2 = scheduler.GetJobDetail("YXSyncAllPriceJob2", "Jinher.AMP.BTP.Job");
            job2 = new JobDetail("YXSyncAllPriceJob2", "Jinher.AMP.BTP.Job", typeof(YXSyncAllPriceJob));
            job2.JobDataMap.PutAll(param);

            //每天15.30点运行
            SimpleTrigger trigger2 = new SimpleTrigger("YXSyncAllPriceJobTrigger2", "Jinher.AMP.BTP.Job", DateTime.UtcNow.Date.AddDays(1).AddHours(7.5), null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(24));
            trigger2.Description = "YXSyncAllPriceJobTrigger2触发器";
            scheduler.ScheduleJob(job2, trigger2);
        }

        private void SyncSuningDataJob(IScheduler scheduler, System.Collections.IDictionary param)
        {
            JobDetail job = scheduler.GetJobDetail("SNExpressJob", "Jinher.AMP.BTP.Job");
            job = new JobDetail("SNExpressJob", "Jinher.AMP.BTP.Job", typeof(SNExpressJob));
            job.JobDataMap.PutAll(param);

            DateTime start = DateTime.UtcNow;
            SimpleTrigger trigger = new SimpleTrigger("SNExpressJobTrigger", "Jinher.AMP.BTP.Job", start, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromHours(3));
            trigger.Description = "SNExpressJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job, trigger);



            JobDetail job1 = scheduler.GetJobDetail("SNOrderJob", "Jinher.AMP.BTP.Job");
            job1 = new JobDetail("SNOrderJob", "Jinher.AMP.BTP.Job", typeof(SNExpressJob));
            job1.JobDataMap.PutAll(param);

            DateTime start1 = DateTime.UtcNow;
            SimpleTrigger trigger1 = new SimpleTrigger("SNOrderJobTrigger", "Jinher.AMP.BTP.Job", start1, null,
                Jinher.JAP.Job.Engine.SimpleTrigger.RepeatIndefinitely, TimeSpan.FromMinutes(5));
            trigger.Description = "SNOrderJob触发器";
            //把创建的任务和触发器注册到调度器中
            scheduler.ScheduleJob(job1, trigger1);
        }
    }
}
