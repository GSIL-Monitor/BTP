using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using System.Web.Caching;

namespace Jinher.AMP.BTP.UI.Models
{
    public class CrowdfundingVM
    {
        /// <summary>
        /// 用户众筹信息
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="invoker">调用者</param>
        /// <returns></returns>
        public static Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuy(Guid appId, Guid userId, string invoker)
        {
            var key = "tUserCrowdfundingBuy:" + appId + userId;
            try
            {
                var result = HttpRuntime.Cache.Get(key) as Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO;
                if (result == null)
                {
                    Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade facade = new ISV.Facade.CrowdfundingFacade();
                    result = facade.GetUserCrowdfundingBuy(appId, userId);
                    if (result == null)
                    {
                        result = new Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO();
                    }
                    HttpRuntime.Cache.Add(key, result, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    LogHelper.Debug("CrowdfundingVM.GetUserCrowdfundingBuy from DB......");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("{0}中调用Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade.GetUserCrowdfundingBuy接口异常。appId：{1}，userId：{2}", invoker, appId, userId), ex);
            }
            return null;
        }


    }
}
