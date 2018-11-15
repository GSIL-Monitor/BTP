using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS.Helper
{
    public static class PromotionHelper
    {
        /// <summary>
        /// 预售商品定时上架
        /// </summary>
        public static void Shelve()
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var startDate = DateTime.Now.AddMinutes(1);
            LogHelper.Info("PromotionHelper.Shelve-------------------------------------Begin");
            try
            {
                var promotions = Jinher.AMP.BTP.BE.Promotion.ObjectSet()
            .Where(_ => _.PromotionType == 5 && !_.IsStatis && _.PresellStartTime < startDate && !_.IsSell.Value).ToList();
                foreach (var promotion in promotions)
                {
                    var promotionComs = Jinher.AMP.BTP.BE.PromotionItems.ObjectSet().Where(_ => _.PromotionId == promotion.Id)
                        .Join(Jinher.AMP.BTP.BE.Commodity.ObjectSet()
                            .Where(_ => !_.IsDel && _.State == 1), p => p.CommodityId, c => c.Id, (p, c) => c)
                        .ToList();
                    promotion.IsStatis = true;
                    contextSession.SaveObject(promotion);

                    foreach (var commodity in promotionComs)
                    {
                        commodity.State = 0;
                        contextSession.SaveObject(commodity);
                        commodity.RefreshCache(EntityState.Modified);
                        LogHelper.Info("PromotionHelper.Shelve 商品ID：" + commodity.Id + "，名称：" + commodity.Name + "上架");
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("PromotionHelper.Shelve Exception", ex);
                throw;
            }
            LogHelper.Info("PromotionHelper.Shelve-------------------------------------End");
        }

        /// <summary>
        /// 预售商品下架
        /// </summary>
        public static void CommoditySoldOut(ContextSession contextSession, Jinher.AMP.BTP.BE.Promotion promotion, Guid commodityId)
        {
            if (promotion.IsSell.HasValue)
            {
                if (promotion.IsSell.Value)
                {
                    // 商品上架
                }
                else
                {
                    // 商品下架
                    var commodity = Commodity.FindByID(commodityId);
                    if (commodity == null)
                    {
                        LogHelper.Info("PromotionHelper.CommoditySoldOut 商品下架，商品不存在，商品ID：" + commodityId);
                    }
                    else
                    {
                        commodity.State = 1;
                        contextSession.SaveObject(commodity);
                        commodity.RefreshCache(EntityState.Modified);
                        LogHelper.Info("PromotionHelper.CommoditySoldOut 商品下架，商品ID：" + commodityId);
                    }
                }
            }
        }
    }
}
