
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/2/18 15:42:34
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ErrorCommodityOrderFacade : BaseFacade<IErrorCommodityOrder>
    {
        /// <summary>
        /// Job自动处理取消订单时回退积分
        /// </summary>
        public void AutoDealOrderCancelSrore()
        {
            base.Do();
            this.Command.AutoDealOrderCancelSrore();
        }
        /// <summary>
        ///  Job自动处理售中退款时回退积分
        /// </summary>
        public void AutoDealOrderRefundScore()
        {
            base.Do();
            this.Command.AutoDealOrderRefundScore();
        }
        /// <summary>
        ///  Job自动处理售后退款时回退积分
        /// </summary>
        public void AutoDealOrderAfterSalesRefundScore()
        {
            base.Do();
            this.Command.AutoDealOrderAfterSalesRefundScore();
        }
        /// <summary>
        /// Job自动处理回退积分
        /// </summary>
        public void AutoRefundScore()
        {
            base.Do();
            this.Command.AutoRefundScore();
        }
    }
}