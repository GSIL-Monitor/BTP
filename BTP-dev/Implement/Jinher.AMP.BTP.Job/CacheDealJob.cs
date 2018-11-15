using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.Facade;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class CacheDealJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时处理缓存:CacheDealJob...begin");
            try
            {
                //清空缓存
                //Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache("BTPCache");
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new ISV.Facade.PromotionFacade();
                
                #region 促销缓存

                ConsoleLog.WriteLog("定时刷新今日优惠信息开始...");
                var dict = facade.GetAppPromotions();
                ConsoleLog.WriteLog("定时刷新今日优惠信息完成...");
                #endregion

                #region 门店缓存

                ////获取所有门店信息,并根据appid分类
                //List<StoreCacheDTO> storeList = facade.GetAllStores();
                //Dictionary<Guid, List<StoreCacheDTO>> storeDic = storeList.GroupBy(a => a.AppId, (key, value) => new { Id = key, Value = value })
                //    .ToDictionary(a => a.Id, a => a.Value.ToList());

                //foreach (Guid id in storeDic.Keys)
                //{
                //    string key = id.ToString();
                //    if (Jinher.JAP.Cache.GlobalCacheWrapper.Contains("G_StoreInfo", key, "BTPCache"))
                //    {
                //        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_StoreInfo", key, "BTPCache");
                //    }
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_StoreInfo", key, storeDic[id], "BTPCache");
                //}


                #endregion

                #region 商品属性缓存

                //Dictionary<Guid, List<ComAttributeCacheDTO>> attrDic = facade.GetAllCommAttributes()
                //    .GroupBy(a => a.CommodityId, (key, value) => new { Key = key, Value = value }).ToDictionary(a => a.Key, a => a.Value.ToList());

                //foreach (Guid id in attrDic.Keys)
                //{
                //    string key = id.ToString();
                //    if (Jinher.JAP.Cache.GlobalCacheWrapper.Contains("G_CommodityAttrInfo", key, "BTPCache"))
                //    {
                //        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CommodityAttrInfo", key, "BTPCache");
                //    }
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CommodityAttrInfo", key, attrDic[id], "BTPCache");
                //}

                //List<ComAttributeCacheDTO> attrList = facade.GetAllCommAttributes();
                //foreach (ComAttributeCacheDTO dto in attrList)
                //{
                //    string key = dto.Id.ToString();
                //    if (Jinher.JAP.Cache.GlobalCacheWrapper.Contains("G_AttributeInfo", key, "BTPCache"))
                //    {
                //        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_AttributeInfo", key, "BTPCache");
                //    }
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_AttributeInfo", key, dto, "BTPCache");
                //}

                #endregion

                #region 类目缓存

                //Dictionary<Guid, List<CategoryCacheDTO>> categoryDic = facade.GetAllCateGories().
                //    GroupBy(a => a.AppId, (key, value) => new { Key = key, Value = value }).ToDictionary(a => a.Key, a => a.Value.ToList());

                //foreach (Guid id in categoryDic.Keys)
                //{
                //    string key = id.ToString();
                //    if (Jinher.JAP.Cache.GlobalCacheWrapper.Contains("G_CategoryInfo", key, "BTPCache"))
                //    {
                //        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CategoryInfo", key, "BTPCache");
                //    }
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CategoryInfo", key, categoryDic[id], "BTPCache");
                //}

                #endregion
                //DateTime today = DateTime.Now.Date;
                //DateTime last = today.AddDays(-1);

                //string key = today.ToString();
                ////移除前一天的缓存数据
                //Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", last.ToString(), "BTPCache");
                ////缓存商品折扣信息
                //Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_DiscountInfo", today.ToString(), dic, "BTPCache");

                //foreach (Guid id in dic.Keys)
                //{
                //    //缓存商品折扣信息
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_DiscountInfo", id.ToString(), dic[id], "BTPCache");
                //}
                #region App缓存
                //Jinher.AMP.BTP.ISV.Facade.CacheFacade cacheFacade=new CacheFacade();
                //ConsoleLog.WriteLog("定时清理App缓存开始...");
                //cacheFacade.RemoveAppCache();
                //ConsoleLog.WriteLog("定时清理App缓存完成...");
                #endregion
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message,LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时处理缓存:CacheDealJob end");
        }
    }
}
