

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
namespace Jinher.AMP.BTP.BE
{
    public partial class Promotion
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
        /// <summary>
        /// 查询所有折扣
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PromotionDTO> GetAllPromotion(System.Guid sellerID, int pageSize, int pageIndex)
        {
            var promotionDTO = Promotion.ObjectSet().Where(n => n.AppId == sellerID && n.EndTime > DateTime.Now).OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            if (promotionDTO.Count == 0)
            {
                return null;
            }
            else
            {
                return new Promotion().ToEntityDataList(promotionDTO);
            }
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        public void Add(PromotionDTO promotionDTO)
        {
            Promotion promotion = new Promotion().FromEntityData(promotionDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            promotion.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(promotion);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        public void Updates(PromotionDTO promotionDTO)
        {
            Promotion promotion = new Promotion().FromEntityData(promotionDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            promotion.EntityState = System.Data.EntityState.Modified;
            contextSession.SaveObject(promotion);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(Promotion promotion)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            promotion.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(promotion);
            contextSession.SaveChange();
        }
        /// <summary>
        /// 查询某个商品的折扣
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Decimal SelectIntensity(Guid commodityId, Guid appid)
        {
            DateTime now = DateTime.Now;
            var promotion = (from data in Promotion.ObjectSet()
                             join data1 in PromotionItems.ObjectSet() on data.Id equals data1.PromotionId
                             where data1.CommodityId == commodityId && data.AppId == appid && data.EndTime >= now && data.StartTime <= now
                             select data).FirstOrDefault();

            if (promotion == null)
            {
                return (decimal)10;
            }
            else
            {
                return promotion.Intensity;
            }
        }

        /// <summary>
        /// 数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        /// <returns>1：成功 0：失败</returns>
        public static int CommodityDataAndRedisDataSynchronization()
        {
            try
            {
                var promotionList = Promotion.ObjectSet().Where(p => p.IsEnable == true && p.IsDel == false && p.StartTime <= DateTime.Now && p.EndTime >= DateTime.Now).Select(p => p.Id);
                if (promotionList.Any())
                {
                    foreach (Guid guid in promotionList)
                    {
                        PromotionRedis(guid);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Promotion.CommodityDataAndRedisDataSynchronization异常：Exception={0}", ex));
                return 0;
            }

            return 1;
        }
        /// <summary>
        /// Redis重置活动资源数据
        /// </summary>
        /// <param name="promotionId">活动ID</param>
        public static int PromotionRedis(Guid promotionId)
        {
            try
            {
                var userLimits = (from ul in UserLimited.ObjectSet()
                                  where ul.PromotionId == promotionId
                                  select ul).ToList();

                var promotionItems = (from p in PromotionItems.ObjectSet()
                                      where p.PromotionId == promotionId
                                      select new
                                          {
                                              Id = p.Id,
                                              CommodityId = p.CommodityId,
                                              SurplusLimitBuyTotal = p.SurplusLimitBuyTotal
                                          }).ToList();
                if (promotionItems.Any())
                {
                    string hashProSaleCountId = RedisKeyConst.ProSaleCountPrefix + promotionId.ToString();
                    foreach (var promotion in promotionItems)
                    {
                        var surplusLimitBuyTotal = promotion.SurplusLimitBuyTotal.HasValue
                                                       ? promotion.SurplusLimitBuyTotal.Value
                                                       : 0;
                        RedisHelper.AddHash(hashProSaleCountId, promotion.CommodityId.ToString(), surplusLimitBuyTotal);
                        string hashGulId = RedisKeyConst.UserLimitPrefix + promotionId.ToString() + ":" + promotion.CommodityId;
                        var userLimitedList = from p in userLimits
                                               where p.PromotionId == promotionId && p.CommodityId == promotion.CommodityId
                                               group p by p.UserId
                                                   into g
                                                   select new
                                                       {
                                                           UserId = g.Key,
                                                           userCount = g.Sum(c => c.Count)
                                                       };
                        if (userLimitedList.Any())
                        {
                            foreach (var userLimited in userLimitedList)
                            {
                                RedisHelper.AddHash(hashGulId, userLimited.UserId.ToString(), userLimited.userCount);
                            }
                        }
                    }
                }

                if (userLimits.Any())
                {
                    var uids = (from ul in userLimits select ul.UserId).Distinct();
                    foreach (Guid uid in uids)
                    {
                        //活动全场限购，用户在当前活动已购买数量
                        var ubc = (from ul in userLimits
                                   where ul.UserId == uid
                                   select ul.Count).Sum();
                        RedisHelper.AddHash(RedisKeyConst.UserPromotionLimitPrefix + promotionId.ToString(), uid.ToString(), ubc);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("Promotion.PromotionRedis异常：Exception={0}", ex));
                return 0;
            }

            return 1;
        }
        /// <summary>
        /// 返回活动类型描述
        /// </summary>
        /// <returns></returns>
        public static string GetPromotionTypeDesc(int promotionType)
        {
            //TODO 活动最好改成枚举，把表中枚举类型化
            switch (promotionType)
            {
                case 0:
                    return "限时打折";
                case 1:
                    return "秒杀";
                case 2:
                    return "预约";
                case 3:
                    return "拼团";
                default:
                    return null;

            }

        }
    }
}



