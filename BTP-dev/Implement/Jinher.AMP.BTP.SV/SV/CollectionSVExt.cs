using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 收藏接口类
    /// </summary>
    public partial class CollectionSV : BaseSv, ICollection
    {
        /// <summary> 
        /// 添加收藏
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCollectionExt(System.Guid commodityId, System.Guid userId, System.Guid appId)
        {
            ResultDTO result = new ResultDTO { ResultCode = 0, Message = "Success" };
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                int collections = Collection.ObjectSet()
                    .Where(n => n.CommodityId == commodityId && n.UserId == userId && n.AppId == appId).Count();
                if (collections == 0)
                {


                    Collection collection = Collection.CreateCollection();
                    collection.AppId = appId;
                    collection.Code = "1";
                    collection.CommodityId = commodityId;
                    collection.SubTime = DateTime.Now;
                    collection.Name = "收藏信息";
                    collection.SubId = userId;
                    collection.UserId = userId;
                    contextSession.SaveObject(collection);
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == commodityId).FirstOrDefault();
                    com.EntityState = System.Data.EntityState.Modified;
                    com.TotalCollection += 1;
                    contextSession.SaveObject(com);
                    contextSession.SaveChanges();

                    com.RefreshCache(EntityState.Modified);
                }
                else
                {
                    result = new ResultDTO { ResultCode = 1, Message = "已有收藏" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加收藏服务异常，commodityId{0}，userId{1}，appId{2}。", commodityId,userId,appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return result;
        }
        /// <summary>
        /// 根据用户ID查询收藏商品
        /// </summary>
        /// <param name="userId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetCollectionItemsExt(System.Guid userId, System.Guid appId)
        {          
            var commodityList = (from data in Collection.ObjectSet()
                                 join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                                 where data.UserId == userId && data.AppId == appId && data1.IsDel == false && data1.CommodityType == 0
                                 select new CommoditySDTO
                                  {
                                      Name = data1.Name,
                                      Id = data1.Id,
                                      Price = data1.Price,
                                      Pic = data1.PicturesPath,
                                      Stock = data1.Stock,
                                      State = data1.State,
                                      CollectNum = data1.TotalCollection,
                                      ReviewNum = data1.TotalReview,
                                      Total = data1.Salesvolume
                                  }).ToList();

            DateTime now = DateTime.Now;

            List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();

            //读今日折扣表
            try
            {

                //zgx-modify
                var comAttributeList = (from a in CommodityStock.ObjectSet()
                                        where commodityIds.Contains(a.CommodityId)
                                        group a by a.CommodityId into g
                                        select new
                                        {
                                            minPrice = g.Min(a => a.Price),
                                            maxPrice = g.Max(a => a.Price),
                                            CommodityId = g.Key
                                        }).ToList();

                var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                foreach (var commodity in commodityList)
                {

                    //zgx-modify
                    if (comAttributeList != null && comAttributeList.Count > 0)
                    {
                        var comAttribute = comAttributeList.Find(r => r.CommodityId == commodity.Id);
                        if (comAttribute != null)
                        {
                            commodity.MaxPrice = comAttribute.maxPrice;
                            commodity.MinPrice = comAttribute.minPrice;
                        }
                    }

                    bool isdi = false;
                    foreach (var com in promotionDic)
                    {
                        if (com.CommodityId == commodity.Id)
                        {
                            commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                            commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                            commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
                            if (com.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                commodity.Intensity = 10;
                                isdi = true;
                                break;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = com.Intensity;
                                isdi = true;
                                break;
                            }
                        }
                    }
                    if (!isdi)
                    {
                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                    }
                    //if (promotionDic.ContainsKey(commodity.Id))
                    //{
                    //    commodity.Intensity = promotionDic[commodity.Id];
                    //}
                    //else
                    //{
                    //    commodity.Intensity = 10;
                    //}
                }
            }
            catch (Exception e)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误，userId{111}，appId{111}",userId,appId), e);
            }
            return commodityList;
        }
        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollectionExt(System.Guid commodityId, System.Guid userId, System.Guid appId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var collection = Collection.ObjectSet()
                    .Where(n => n.CommodityId == commodityId && n.UserId == userId && n.AppId == appId)
                    .FirstOrDefault();
                if (collection != null)
                {
                    //删除收藏
                    collection.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(collection);

                    //更新商品收藏数
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == commodityId).FirstOrDefault();
                    com.EntityState = System.Data.EntityState.Modified;
                    com.TotalCollection -= 1;
                    contextSession.SaveObject(com);
                    contextSession.SaveChanges();

                    com.RefreshCache(EntityState.Modified);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除收藏服务异常。commodityId{0}，userId{1}，appId{2}", commodityId,userId,appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
    }
}