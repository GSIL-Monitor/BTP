
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/2/18 15:42:35
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using System.Diagnostics;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ErrorCommodityOrderSV : BaseSv, IErrorCommodityOrder
    {
        /// <summary>
        /// Job自动处理取消订单时回退积分
        /// </summary>
        public void AutoDealOrderCancelSrore()
        {
            base.Do(false);
            this.AutoDealOrderCancelSroreExt();
        }
        /// <summary>
        ///  Job自动处理售中退款时回退积分
        /// </summary>
        public void AutoDealOrderRefundScore()
        {
            base.Do(false);
            this.AutoDealOrderRefundScoreExt();
        }
        /// <summary>
        ///  Job自动处理售后退款时回退积分
        /// </summary>
        public void AutoDealOrderAfterSalesRefundScore()
        {
            base.Do(false);
            this.AutoDealOrderAfterSalesRefundScoreExt();
        }
        /// <summary>
        /// Job自动处理回退积分
        /// </summary>
        public void AutoRefundScore()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoRefundScoreExt();
            timer.Stop();
            LogHelper.Info(string.Format("ErrorCommodityOrderSV.AutoRefundScore：耗时：{0}。", timer.ElapsedMilliseconds));
        }
    }
}
