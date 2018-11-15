

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
namespace Jinher.AMP.BTP.BE
{
    public partial class PaySource
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion


        private volatile static List<PaySource> paySourceList = null;

        private static readonly object lockHelper = new object();
        /// <summary>
        /// 获取推广主分成比例相关所有信息
        /// </summary>
        /// <returns>推广主分成比例信息</returns>
        public static List<PaySource> GetAllPaySources()
        {
            if (paySourceList == null)
            {
                lock (lockHelper)
                {
                    if (paySourceList == null)
                    {
                        paySourceList = PaySource.ObjectSet().ToList();
                        return paySourceList;
                    }

                }
            }
            return paySourceList;
        }


        /// <summary>
        /// 按类型获取支付方式
        /// </summary>
        /// <returns></returns>
        public static List<int> GetPaymentByTradeType(int tradeType)
        {
            return (from p in GetAllPaySources()
                    where p.TradeType == tradeType
                    select p.Payment).ToList();
        }

        /// <summary>
        /// 获取非金币支付
        /// </summary>
        /// <returns></returns>
        public static List<int> GetSecTransWithoutGoldPayment()
        {
            var result = GetPaymentByTradeType(0);
            result.Remove(0);
            return result;
        }
        /// <summary>
        /// 获取支付方式名称
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public static string GetPaymentName(int payment)
        {
            var paysource = GetAllPaySources().FirstOrDefault(c => c.Payment == payment);
            if (paysource != null)
            {
                return paysource.Name;
            }
            return string.Empty;

        }
        /// <summary>
        /// 获取支付方式类型 0担保交易，1直接到账，2货到付款
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public static TradeTypeEnum GetTradeType(int payment)
        {
            var paysource = GetAllPaySources().FirstOrDefault(c => c.Payment == payment);
            if (paysource != null)
            {
                return (TradeTypeEnum)paysource.TradeType;
            }
            return TradeTypeEnum.None;
        }
    }
}



