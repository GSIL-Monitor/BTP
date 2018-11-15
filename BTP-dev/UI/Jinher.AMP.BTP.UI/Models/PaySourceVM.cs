using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.UI.Models
{
    public class PaySourceVM
    {
        /// <summary>
        /// 获取支付方式描述信息
        /// </summary>
        /// <param name="payment">支付方式编号</param>
        /// <returns></returns>
        public static string GetPaymentName(int payment)
        {
            PaySourceFacade psFacade = new PaySourceFacade();
            string paymentName = psFacade.GetPaymentName(payment);
            return paymentName;
        }

        /// <summary>
        /// 获取担保交易类支付方式
        /// </summary>
        /// <returns></returns>
        public static List<int> GetSecuriedTransactionPayment()
        {
            ISV.Facade.PaySourceFacade psFacade = new ISV.Facade.PaySourceFacade();
            List<int> paymentList = psFacade.GetSecuriedTransactionPayment();
            return paymentList;
        }
        /// <summary>
        /// 获取担保交易类支付方式,排除金币。
        /// </summary>
        /// <returns></returns>
        public static List<int> GetSecTransWithoutGoldPayment()
        {
            ISV.Facade.PaySourceFacade psFacade = new ISV.Facade.PaySourceFacade();
            List<int> paymentList = psFacade.GetSecTransWithoutGoldPayment();
            return paymentList;
        }
        /// <summary>
        /// 获取直接到账类支付方式
        /// </summary>
        /// <returns></returns>
        public static List<int> GetDirectArrivalPayment()
        {
            ISV.Facade.PaySourceFacade psFacade = new ISV.Facade.PaySourceFacade();
            List<int> paymentList = psFacade.GetDirectArrivalPayment();
            return paymentList;
        }
    }
}