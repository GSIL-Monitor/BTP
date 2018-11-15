using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Models
{
    public class PayModel
    {
        /// <summary>
        /// 获取当前用户金币余额。
        /// </summary>
        /// <param name="userId">当前用户id</param>
        /// <param name="sessionId">sessionId</param>
        /// <param name="invoker">调用方</param>
        /// <returns></returns>
        public static ulong GetBalance(System.Guid userId, string sessionId, string invoker)
        {
            return FSPSV.Instance.GetBalance(userId); 
        }



        /// <summary>
        /// 获取当前用户代金券张数。
        /// </summary>
        /// <param name="userId">当前用户id</param>
        /// <param name="invoker">调用方</param>
        /// <returns></returns>
        public static int GetGoldCouponCount(System.Guid userId, string invoker)
        {
            try
            {
                var result = Jinher.AMP.BTP.TPS.PromotionSV.Instance.GetUsersVoucherCount(userId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用Jinher.AMP.Promotion.ISV.Facade.VoucherFacade.GetGoldCouponCount接口异常。userId：{1}", invoker, userId), ex);
            }
            return 0;
        }
    }
}