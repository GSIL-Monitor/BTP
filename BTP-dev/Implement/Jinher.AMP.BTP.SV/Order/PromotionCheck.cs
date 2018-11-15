using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV.Order
{
    public class PromotionCheck
    {
        /// <summary>
        /// 校验活动资源
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="promotions"></param>
        /// <returns>用户是否可以购买</returns>
        public static bool CheckResource(Guid userId, List<Tuple<TodayPromotionDTO, int>> promotions)
        {
            if (promotions == null || !promotions.Any())
                return true;

            foreach (var tuple in promotions)
            {
                TodayPromotionDTO todaypromotion = tuple.Item1;
                int comNumber = tuple.Item2;

                if (todaypromotion.LimitBuyEach != -1)
                {
                    int sumLi = RedisHelper.GetHashValue<int>(RedisKeyConst.UserLimitPrefix + todaypromotion.PromotionId + ":" + todaypromotion.CommodityId, userId.ToString());
                    if (sumLi + comNumber > todaypromotion.LimitBuyEach)
                    {
                        return false;
                    }
                }
                if (todaypromotion.LimitBuyTotal != -1)
                {
                    if (todaypromotion.SurplusLimitBuyTotal + comNumber > todaypromotion.LimitBuyTotal)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 校验活动资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="promotion">商品活动</param>
        /// <param name="comNumber">用户购买数量</param>
        /// <returns>用户是否可以购买</returns>
        public static bool CheckResource(Guid userId, TodayPromotionDTO promotion, int comNumber)
        {
            return CheckResource(userId, new List<Tuple<TodayPromotionDTO, int>>()
                {
                    new Tuple<TodayPromotionDTO, int>(promotion, comNumber)
                });
        }

    }


}
