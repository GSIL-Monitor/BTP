
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/6/2 10:03:30
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
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class PaySourceSV : BaseSv, IPaySource
    {

        /// <summary>
        /// 获取所有支付方式和描述信息。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PaySourceDTO> GetAllPaySourcesExt()
        {
            try
            {
                List<PaySource> psList = PaySource.GetAllPaySources();
                if (psList == null || !psList.Any())
                {
                    return null;
                }
                List<Jinher.AMP.BTP.Deploy.PaySourceDTO> psDtoList = psList.ConvertAll(ps => ps.ToEntityData());
                return psDtoList;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAllPaySourcesExt异常,异常信息:", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取所有可用支付方式。
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllPayments()
        {

            List<int> paymentList = new List<int>();
            try
            {
                var pQuery = (from p in PaySource.GetAllPaySources()
                              select p.Payment).ToList();
                if (pQuery != null && pQuery.Any())
                {
                    paymentList.AddRange(pQuery);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAllPayments异常,异常信息:", ex);
            }
            return paymentList;
        }
        /// <summary>
        /// 获取担保交易类支付方式
        /// </summary>
        /// <returns></returns>
        public List<int> GetSecuriedTransactionPaymentExt()
        {
            return GetPaymentByTradeType(0);
        }
        /// <summary>
        /// 获取担保交易类支付方式,排除金币。
        /// </summary>
        /// <returns></returns>
        public List<int> GetSecTransWithoutGoldPaymentExt()
        {
            List<int> paymentList = GetPaymentByTradeType(0);
            if (paymentList.Contains(0))
            {
                paymentList.Remove(0);
            }
            return paymentList;
        }
        /// <summary>
        /// 获取直接到账类支付方式
        /// </summary>
        /// <returns></returns>
        public List<int> GetDirectArrivalPaymentExt()
        {
            return GetPaymentByTradeType(1);
        }
        /// <summary>
        /// 按类型获取支付方式
        /// </summary>
        /// <returns></returns>
        private List<int> GetPaymentByTradeType(int tradeType)
        {
            List<int> paymentList = new List<int>();
            try
            {
                var pQuery = (from p in PaySource.GetAllPaySources()
                              where p.TradeType == tradeType
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
        /// 获取所有支付方式对应的描述信息
        /// </summary>
        /// <param name="payment">支付方式数字编号</param>
        /// <returns></returns>
        public string GetPaymentNameExt(int payment)
        {
            try
            {
                List<PaySource> psList = PaySource.GetAllPaySources();
                if (psList == null || !psList.Any())
                {
                    return "";
                }
                PaySource psModel = psList.Where(ps => ps.Payment == payment).FirstOrDefault();
                if (psModel == null)
                {
                    return "";
                }
                return psModel.Name;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetPaymentNameExt异常,异常信息:", ex);
            }
            return "";
        }

    }
}