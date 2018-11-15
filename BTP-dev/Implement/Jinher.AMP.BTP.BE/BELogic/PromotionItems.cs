

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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
    public partial class PromotionItems
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
        /// 根据促销ID查询促销商品
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM> GetPromotionItemsByPromotionID(System.Guid promotionID, int pageSize, int pageIndex)
        {
            var quary = from n in PromotionItems.ObjectSet()
                        join m in Promotion.ObjectSet() on n.PromotionId equals m.Id
                        join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                        where (n.PromotionId == promotionID && b.CommodityType == 0)
                        select new PromotionItemsVM
                        {
                            AppId = n.AppId,
                            PromotionId = m.Id,
                            CommodityId = b.Id,
                            CommodityName = b.Name,
                            Price = b.Price,
                            Stock = b.Stock,
                            PicturesPath = b.PicturesPath,
                            TotalCollection = b.TotalCollection,
                            TotalReview = b.TotalReview,
                            Salesvolume = b.Salesvolume,
                            State = b.State,
                            Intensity = m.Intensity,
                            No_Codes = b.No_Code,
                            PromotionSubTime = b.SubTime
                        };

            

            var query1 = quary.OrderByDescending(n => n.PromotionSubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            foreach (var c in query1)
            {
                c.CommodityCategorys = from cc in CommodityCategory.ObjectSet()
                                       join cate in Category.ObjectSet() on cc.CategoryId equals cate.Id
                                       where cc.CommodityId == c.CommodityId && cc.AppId == c.AppId
                                       select cate.Name;
            }
            return query1;
        }

        /// <summary>
        /// 查询商品的折扣
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetIntensity(List<Guid> commodityIds)
        {
            DateTime now = DateTime.Now;
            var promotionDic = (from p in PromotionItems.ObjectSet()
                                join pro in Promotion.ObjectSet() on p.PromotionId  equals pro.Id
                                where commodityIds.Contains(p.CommodityId) && pro.EndTime > now && pro.StartTime < now
                                select new { ComId = p.CommodityId, Intensity = pro.Intensity }).ToDictionary(p => p.ComId, p => p.Intensity);
            return promotionDic;
        }


        /// <summary>
        /// 添加操作
        /// </summary>
        public void Add(PromotionItemsDTO promotionItemsDTO)
        {
            PromotionItems promotionItems = new PromotionItems().FromEntityData(promotionItemsDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            promotionItems.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(promotionItems);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(PromotionItems promotionItems)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            promotionItems.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(promotionItems);
            contextSession.SaveChange();
        }
    }
}



