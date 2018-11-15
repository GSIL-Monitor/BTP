/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 14:27:30
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using System.Timers;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PromotionBP : BaseBP, IPromotion
    {

        //private PromotionDTO promotionDTO { get; set; }
        /// <summary>
        /// 添加折扣
        /// </summary>
        /// <param name="discountsDTO">自定义折扣属性</param>
        public ResultDTO AddPromotionExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            //今日促销表队列
            List<TodayPromotion> todayPro = new List<TodayPromotion>();

            string commodityNames = string.Empty;
            Commodity comm = new Commodity();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
            try
            {
                Promotion promotion = Promotion.CreatePromotion();
                promotion.Name = discountsDTO.Name;
                promotion.PicturesPath = discountsDTO.PicturesPath;
                promotion.StartTime = discountsDTO.StartTime;
                promotion.EndTime = discountsDTO.EndTime;
                promotion.Intensity = discountsDTO.Intensity;
                promotion.DiscountPrice = discountsDTO.DiscountPrice;
                promotion.AppId = discountsDTO.SellerId;
                promotion.IsAll = discountsDTO.IsAll;
                promotion.PromotionType = discountsDTO.PromotionType;
                if (discountsDTO.No_Codes != null && discountsDTO.No_Codes.Count > 0)
                {
                    var names = Commodity.ObjectSet().
                        Where(n => n.AppId == discountsDTO.SellerId && n.IsDel == false && n.State == 0 && n.Stock > 0
                        && discountsDTO.No_Codes.Contains(n.No_Code) && n.CommodityType == 0).Select(n => n.Name).ToList();
                    if (names.Count > 0)
                    {
                        commodityNames = string.Join(",", names);
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 2, Message = "添加失败，请检查商品是否有库存" };
                    }
                    promotion.CommodityNames = commodityNames;
                }
                promotion.IsEnable = true;
                //promotion.Add(promotionDTO);


                promotion.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(promotion);

                DateTime now = DateTime.Now.Date;
                DateTime tomorrow = now.AddDays(1);

                //判断是否今日促销
                bool isToday = ((promotion.StartTime < tomorrow || promotion.PresellStartTime < tomorrow) && promotion.EndTime > now);
                //全部商品
                if (discountsDTO.IsAll == true)
                {

                    //循环处理商品
                    foreach (var item in discountsDTO.CommodityIdList)
                    {
                        PromotionItems promotionItems = new PromotionItems();
                        promotionItems.Id = Guid.NewGuid();
                        promotionItems.Name = "促销商品";
                        promotionItems.PromotionId = promotion.Id;
                        promotionItems.CommodityId = item;
                        promotionItems.AppId = discountsDTO.SellerId;
                        promotionItems.SubId = discountsDTO.SellerId;
                        promotionItems.Intensity = discountsDTO.Intensity;
                        promotionItems.LimitBuyEach = -1;
                        promotionItems.LimitBuyTotal = -1;
                        promotionItems.DiscountPrice = -1;
                        promotionItems.SurplusLimitBuyTotal = 0;
                        promotionItems.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(promotionItems);

                        if (isToday)
                        {
                            TodayPromotion pro = new TodayPromotion();
                            pro.Id = Guid.NewGuid();
                            pro.Intensity = discountsDTO.Intensity;
                            pro.CommodityId = promotionItems.CommodityId;
                            pro.StartTime = promotion.StartTime;
                            pro.EndTime = promotion.EndTime;
                            pro.PromotionId = promotion.Id;
                            pro.DiscountPrice = promotionItems.DiscountPrice;
                            pro.LimitBuyEach = promotionItems.LimitBuyEach;
                            pro.LimitBuyTotal = promotionItems.LimitBuyTotal;
                            pro.SurplusLimitBuyTotal = 0;
                            pro.AppId = promotion.AppId;
                            pro.PromotionType = discountsDTO.PromotionType;
                            pro.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(pro);
                            needRefreshCacheTodayPromotions.Add(pro);
                        }
                    }
                }
                //非全部商品
                else
                {

                    var commodityIdList = Commodity.ObjectSet().
                         Where(n => discountsDTO.No_Codes.Contains(n.No_Code) && n.AppId == discountsDTO.SellerId && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                         .Select(n => new
                         {
                             ComId = n.Id,
                             Code = n.No_Code
                         }).Distinct().ToDictionary(p => p.Code, p => p.ComId);

                    if (commodityIdList != null && commodityIdList.Count > 0)
                    {
                        //将商品id列表回传
                        //discountsDTO.CommodityIdList = commodityIdList;
                        for (int i = 0; i < discountsDTO.ComPro.Length; i++)
                        {

                            PromotionItems promotionItemsDTO = new PromotionItems();
                            promotionItemsDTO.Id = Guid.NewGuid();
                            promotionItemsDTO.Name = "促销商品";
                            promotionItemsDTO.PromotionId = promotion.Id;
                            promotionItemsDTO.AppId = discountsDTO.SellerId;
                            promotionItemsDTO.SubId = discountsDTO.SellerId;
                            promotionItemsDTO.Intensity = discountsDTO.Intensity;
                            promotionItemsDTO.CommodityId = commodityIdList[discountsDTO.ComPro[i].Split('|')[0]];
                            promotionItemsDTO.DiscountPrice = Convert.ToDecimal(discountsDTO.ComPro[i].Split('|')[1]);
                            promotionItemsDTO.LimitBuyEach = Convert.ToInt32(discountsDTO.ComPro[i].Split('|')[2]);
                            promotionItemsDTO.LimitBuyTotal = Convert.ToInt32(discountsDTO.ComPro[i].Split('|')[3]);
                            promotionItemsDTO.SurplusLimitBuyTotal = 0;
                            promotionItemsDTO.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(promotionItemsDTO);

                            if (isToday)
                            {
                                TodayPromotion pro = new TodayPromotion();
                                pro.Id = Guid.NewGuid();
                                pro.Intensity = discountsDTO.Intensity;
                                pro.CommodityId = promotionItemsDTO.CommodityId;
                                pro.StartTime = promotion.StartTime;
                                pro.EndTime = promotion.EndTime;
                                pro.PromotionId = promotion.Id;
                                pro.DiscountPrice = promotionItemsDTO.DiscountPrice;
                                pro.LimitBuyEach = promotionItemsDTO.LimitBuyEach;
                                pro.LimitBuyTotal = promotionItemsDTO.LimitBuyTotal;
                                pro.SurplusLimitBuyTotal = 0;
                                pro.AppId = discountsDTO.SellerId;
                                pro.PromotionType = discountsDTO.PromotionType;
                                pro.EntityState = System.Data.EntityState.Added;
                                contextSession.SaveObject(pro);
                                needRefreshCacheTodayPromotions.Add(pro);
                            }
                        }
                    }
                }
                contextSession.SaveChanges();

                if (needRefreshCacheTodayPromotions.Any())
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Added));

                //发广场促销消息
                SendPromotionIUS(promotion);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加折扣服务异常。discountsDTO：{0}", JsonHelper.JsonSerializer(discountsDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }



            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        private void SendPromotionIUS(Promotion promotionadd)
        {
            if (promotionadd.StartTime <= DateTime.Now.AddDays(3))
            {
                try
                {

                    IUS.Deploy.CustomDTO.PicFromUrlCDTO addDataDTO = new IUS.Deploy.CustomDTO.PicFromUrlCDTO();

                    addDataDTO.AppId = promotionadd.AppId;
                    addDataDTO.Content = APPSV.GetAppName(promotionadd.AppId) + "有新促销活动了，快去参加吧~";
                    addDataDTO.PhotoUrl = "";
                    addDataDTO.ShareUrl = string.Format("{0}Mobile/PromotionList?AppId={1}&type=tuwen", Jinher.AMP.BTP.Common.CustomConfig.BtpDomain, promotionadd.AppId);
                    addDataDTO.Source = Jinher.AMP.IUS.Deploy.Enum.SourceEnum.EBusinessInfo;
                    addDataDTO.Title = addDataDTO.Content;
                    addDataDTO.UserId = promotionadd.SubId;
                    addDataDTO.UserName = (ContextDTO != null && ContextDTO.LoginUserName != null) ? ContextDTO.LoginUserName : "btp";
                    var result = Jinher.AMP.BTP.TPS.IUSSV.Instance.AddPicFromUrl(addDataDTO);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("促销发布广场消息异常。promotionadd：{0}", JsonHelper.JsonSerializer(promotionadd)), ex);
                }
            }
        }

        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="id">促销ID</param>
        public ResultDTO DelPromotionExt(Guid id)
        {
            try
            {

                Promotion promotion = Promotion.ObjectSet().Where(n => n.Id == id).FirstOrDefault();
                if (promotion != null)
                {
                    //查询数据
                    List<Guid> proCache = PromotionItems.ObjectSet().
                        Where(n => n.PromotionId.Equals(promotion.Id))
                        .Select(n => n.CommodityId).ToList();

                    var todayPromotions =
                        TodayPromotion.ObjectSet()
                        .Where(c => c.PromotionId == id)
                        .Select(c => new TodayPromotionDTO { Id = c.Id, CommodityId = c.CommodityId, AppId = c.AppId })
                        .ToList();

                    //删除缓存表数据
                    TodayPromotion.ObjectSet().Context.ExecuteStoreCommand("delete from TodayPromotion where PromotionId ='" + id + "'");

                    ////删除促销商品表数据
                    //PromotionItems.ObjectSet().Context.ExecuteStoreCommand(
                    //    "delete from PromotionItems where PromotionId='" + promotion.Id + "'");

                    //删除促销信息
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    promotion.IsDel = true;
                    promotion.EntityState = EntityState.Modified;
                    contextSession.SaveChange();
                    TodayPromotion.RemoveCaches(promotion.AppId, todayPromotions);
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除折扣服务异常。id：{0}", id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 修改折扣
        /// </summary>
        /// <param name="discountsDTO">自定义属性</param>
        public ResultDTO UpdatePromotionExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            List<TodayPromotion> needAddCacheTodayPromotions = new List<TodayPromotion>();
            List<TodayPromotion> needRemoveCacheTodayPromotions = new List<TodayPromotion>();

            //今日促销表队列
            List<TodayPromotion> todayPro = new List<TodayPromotion>();
            //记录原有的促销是否是今天的
            bool isPreToday = false;

            DateTime now = DateTime.Now.Date;
            DateTime tomorrow = now.AddDays(1);

            string commodityNames = string.Empty;
            Commodity comm = new Commodity();
            try
            {

                Promotion promotion = Promotion.ObjectSet().Where(n => n.Id == discountsDTO.PromotionId).FirstOrDefault();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                promotion.EntityState = System.Data.EntityState.Modified;
                promotion.Name = discountsDTO.Name;
                promotion.PicturesPath = discountsDTO.PicturesPath;

                //原有的促销是今日的
                if ((promotion.StartTime < tomorrow & promotion.EndTime > now))
                {
                    isPreToday = true;
                    needRemoveCacheTodayPromotions =
                        TodayPromotion.ObjectSet().Where(c => c.PromotionId == promotion.Id).ToList();
                }
                promotion.StartTime = discountsDTO.StartTime;
                promotion.EndTime = discountsDTO.EndTime;
                promotion.Intensity = discountsDTO.Intensity;
                promotion.IsAll = discountsDTO.IsAll;
                promotion.DiscountPrice = discountsDTO.DiscountPrice;
                promotion.PromotionType = discountsDTO.PromotionType;
                if (discountsDTO.No_Codes != null && discountsDTO.No_Codes.Count > 0)
                {
                    var names = Commodity.ObjectSet().
                        Where(n => n.AppId == discountsDTO.SellerId && n.IsDel == false && n.State == 0
                            && n.Stock > 0 && discountsDTO.No_Codes.Contains(n.No_Code) && n.CommodityType == 0).Select(n => n.Name).ToList();
                    if (names.Count > 0)
                    {
                        commodityNames = string.Join(",", names);
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 2, Message = "添加失败，请检查商品是否有库存" };
                    }
                    promotion.CommodityNames = commodityNames;
                }
                contextSession.SaveObject(promotion);


                bool isToday = ((promotion.StartTime < tomorrow || promotion.PresellStartTime < tomorrow) && promotion.EndTime > now);

                if (discountsDTO.IsAll == true)
                {
                    discountsDTO.CommodityIdList = GetCommodityIdsCan(promotion.StartTime, discountsDTO.EndTime, promotion.AppId, promotion.Id);
                    foreach (var item in discountsDTO.CommodityIdList)
                    {
                        PromotionItemsDTO promotionItemsDTO = new PromotionItemsDTO();
                        promotionItemsDTO.Id = Guid.NewGuid();
                        promotionItemsDTO.Name = "促销商品";
                        promotionItemsDTO.PromotionId = discountsDTO.PromotionId;
                        promotionItemsDTO.CommodityId = item;
                        promotionItemsDTO.AppId = discountsDTO.SellerId;
                        promotionItemsDTO.SubId = discountsDTO.SellerId;
                        promotionItemsDTO.Intensity = discountsDTO.Intensity;
                        promotionItemsDTO.LimitBuyEach = -1;
                        promotionItemsDTO.LimitBuyTotal = -1;
                        promotionItemsDTO.DiscountPrice = -1;
                        PromotionItems promotionItems = new PromotionItems().FromEntityData(promotionItemsDTO);
                        promotionItems.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(promotionItems);

                        //今日的促销
                        if (isToday)
                        {
                            TodayPromotion pro = new TodayPromotion();
                            pro.Id = Guid.NewGuid();
                            pro.Intensity = promotion.Intensity;
                            pro.CommodityId = promotionItemsDTO.CommodityId;
                            pro.StartTime = promotion.StartTime;
                            pro.EndTime = promotion.EndTime;
                            pro.PromotionId = promotion.Id;
                            pro.DiscountPrice = promotionItemsDTO.DiscountPrice;
                            pro.LimitBuyEach = promotionItemsDTO.LimitBuyEach;
                            pro.LimitBuyTotal = promotionItemsDTO.LimitBuyTotal;
                            pro.AppId = discountsDTO.SellerId;
                            pro.PromotionType = discountsDTO.PromotionType;
                            pro.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(pro);
                            needAddCacheTodayPromotions.Add(pro);
                        }
                    }
                }
                else
                {
                    #region 删除原有促销商品
                    //List<PromotionItems> olditems = PromotionItems.ObjectSet().
                    //    Where(n => n.PromotionId.Equals(discountsDTO.PromotionId)).ToList();
                    //foreach (PromotionItems item in olditems)
                    //{
                    //    item.EntityState = System.Data.EntityState.Deleted;
                    //    contextSession.Delete(item);
                    //}



                    #endregion

                    var commodityIdList = Commodity.ObjectSet().
                        Where(n => discountsDTO.No_Codes.Contains(n.No_Code) && n.State == 0 && n.IsDel == false
                          && n.AppId == discountsDTO.SellerId && n.Stock > 0 && n.CommodityType == 0).Select(n => new
                          {
                              ComId = n.Id,
                              Code = n.No_Code
                          }).Distinct().ToDictionary(p => p.Code, p => p.ComId);



                    if (commodityIdList != null && commodityIdList.Count > 0)
                    {
                        ////将商品id列表回传
                        //discountsDTO.CommodityIdList = commodityIdList;

                        for (int i = 0; i < discountsDTO.ComPro.Length; i++)
                        {
                            PromotionItemsDTO promotionItemsDTO = new PromotionItemsDTO();
                            promotionItemsDTO.Id = Guid.NewGuid();
                            promotionItemsDTO.Name = "促销商品";
                            promotionItemsDTO.Intensity = discountsDTO.Intensity;
                            promotionItemsDTO.PromotionId = discountsDTO.PromotionId;
                            if (commodityIdList.ContainsKey(discountsDTO.ComPro[i].Split('|')[0]))
                            {
                                promotionItemsDTO.CommodityId = commodityIdList[discountsDTO.ComPro[i].Split('|')[0]];
                            }
                            promotionItemsDTO.DiscountPrice = Convert.ToDecimal(discountsDTO.ComPro[i].Split('|')[1]);
                            promotionItemsDTO.LimitBuyEach = Convert.ToInt32(discountsDTO.ComPro[i].Split('|')[2]);
                            promotionItemsDTO.LimitBuyTotal = Convert.ToInt32(discountsDTO.ComPro[i].Split('|')[3]);
                            var sur = PromotionItems.ObjectSet().Where(n => n.PromotionId == discountsDTO.PromotionId && n.CommodityId == promotionItemsDTO.CommodityId).Select(z => z.SurplusLimitBuyTotal).FirstOrDefault();
                            if (promotionItemsDTO.LimitBuyTotal < sur && promotionItemsDTO.LimitBuyTotal != -1)
                            {
                                return new ResultDTO { ResultCode = 2, Message = "限购数量不能小于商品限购期间内的销量" };
                            }
                            promotionItemsDTO.SurplusLimitBuyTotal = sur == null ? 0 : sur;
                            promotionItemsDTO.AppId = discountsDTO.SellerId;
                            promotionItemsDTO.SubId = discountsDTO.SellerId;

                            PromotionItems promotionItems = new PromotionItems().FromEntityData(promotionItemsDTO);
                            promotionItems.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(promotionItems);

                            //今日的促销
                            if (isToday)
                            {
                                TodayPromotion pro = new TodayPromotion();
                                pro.Id = Guid.NewGuid();
                                pro.Intensity = promotion.Intensity;
                                pro.CommodityId = promotionItems.CommodityId;
                                pro.StartTime = promotion.StartTime;
                                pro.EndTime = promotion.EndTime;
                                pro.PromotionId = promotion.Id;
                                pro.DiscountPrice = promotionItemsDTO.DiscountPrice;
                                pro.LimitBuyEach = promotionItemsDTO.LimitBuyEach;
                                pro.LimitBuyTotal = promotionItemsDTO.LimitBuyTotal;
                                pro.SurplusLimitBuyTotal = promotionItemsDTO.SurplusLimitBuyTotal;
                                pro.AppId = discountsDTO.SellerId;
                                pro.PromotionType = discountsDTO.PromotionType;
                                pro.EntityState = System.Data.EntityState.Added;

                                contextSession.SaveObject(pro);
                                needAddCacheTodayPromotions.Add(pro);
                            }
                        }
                    }
                }

                PromotionItems.ObjectSet().Context.ExecuteStoreCommand("delete from PromotionItems where PromotionId='" + promotion.Id + "'");

                //促销今日表有数据，需要先删除
                if (isPreToday)
                {
                    TodayPromotion.ObjectSet().Context.
                                ExecuteStoreCommand("delete from todaypromotion where promotionid='" + promotion.Id + "'");
                }

                //统一提交到数据库
                contextSession.SaveChanges();
                if (needRemoveCacheTodayPromotions.Any())
                {
                    needRemoveCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                if (needAddCacheTodayPromotions.Any())
                {
                    needAddCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Added));
                }

                //发广场促销消息
                SendPromotionIUS(promotion);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改折扣服务异常。discountsDTO：{0}", JsonHelper.JsonSerializer(discountsDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 查询所有折扣
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<DiscountsVM> GetAllPromotionExt(System.Guid sellerID, int pageSize, int pageIndex, string startTime, string endTime, string sintensity, string eintensity, string commodityName, string state, out int rowCount)
        {
            PromotionItems p = new PromotionItems();
            var query = from data in Promotion.ObjectSet()
                        where data.AppId == sellerID && !data.IsDel && data.IsEnable && data.PromotionType == 0
                        select new DiscountsVM
                        {
                            PromotionId = data.Id,
                            Name = data.Name,
                            StartTime = data.StartTime,
                            EndTime = data.EndTime,
                            Intensity = data.Intensity,
                            PicturesPath = data.PicturesPath,
                            IsEnable = data.IsEnable,
                            CommodityNames = data.CommodityNames,
                            SubTime = data.SubTime,
                            PromotionType = data.PromotionType
                        };

            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime sstartTime = DateTime.Parse(startTime);
                query = query.Where(n => n.StartTime >= sstartTime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime eeTime = DateTime.Parse(endTime);
                query = query.Where(n => n.StartTime <= eeTime);
            }
            if (!string.IsNullOrEmpty(sintensity))
            {
                decimal intensity = decimal.Parse(sintensity);
                query = query.Where(n => n.Intensity >= intensity);
            }
            if (!string.IsNullOrEmpty(eintensity))
            {
                decimal eeintensity = decimal.Parse(eintensity);
                query = query.Where(n => n.Intensity <= eeintensity);
            }

            if (!string.IsNullOrEmpty(commodityName))
            {
                query = query.Where(n => n.CommodityNames.Contains(commodityName));
            }
            if (!string.IsNullOrEmpty(state))
            {
                //bool result = state == "1" ? true : false;
                //query = query.Where(n => n.IsEnable == result);
                if (state == "0")
                {
                    query = query.Where(n => n.StartTime < DateTime.Now && n.EndTime > DateTime.Now);
                }
                if (state == "1")
                {
                    query = query.Where(n => n.EndTime < DateTime.Now);
                }
                if (state == "2")
                {
                    query = query.Where(n => n.StartTime > DateTime.Now);
                }
            }
            rowCount = query.Count();
            var list = query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //获取所有促销id
            List<Guid> promotionIdList = list.Select(a => a.PromotionId).Distinct().ToList();

            //获取所有促销商品
            var commodityQuery = from n in PromotionItems.ObjectSet()
                                 join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                                 where promotionIdList.Contains(n.PromotionId) && b.CommodityType == 0
                                 select new PromotionItemsVM
                                 {
                                     PromotionId = n.PromotionId,
                                     CommodityName = b.Name,
                                     PromotionSubTime = b.SubTime
                                 };

            var commodityList = commodityQuery.OrderByDescending(n => n.PromotionSubTime).ToList();

            //并行查询
            list.AsParallel().ForAll(
                item =>
                {
                    item.Commoditys = commodityList.Where(a => a.PromotionId == item.PromotionId).Take(4).Select(a => a.CommodityName).ToList();
                });

            return list;
        }


        /// <summary>
        /// 根据促销ID查询促销商品
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM> GetPromotionItemsByPromotionIDExt(System.Guid promotionID, CommoditySearchDTO search, int pageSize, int pageIndex, out int rownum)
        {
            var quary = from n in PromotionItems.ObjectSet()
                        join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                        where n.PromotionId == promotionID && b.CommodityType == 0
                        select new PromotionItemsVM
                        {
                            PromotionId = promotionID,
                            CommodityId = b.Id,
                            CommodityName = b.Name,
                            Price = b.Price,
                            Stock = b.Stock,
                            PicturesPath = b.PicturesPath,
                            Salesvolume = b.Salesvolume,
                            State = b.State,
                            No_Codes = b.No_Code,
                            PromotionSubTime = b.SubTime,
                            DiscountPrice = n.DiscountPrice,
                            LimitBuyEach = n.LimitBuyEach,
                            LimitBuyTotal = n.LimitBuyTotal,
                            SurplusLimitBuyTotal = n.SurplusLimitBuyTotal,
                        };

            #region 条件查询
            if (!string.IsNullOrEmpty(search.commodityName))
            {
                quary = quary.Where(n => n.CommodityName.Contains(search.commodityName));
            }
            if (!string.IsNullOrEmpty(search.commodityCode))
            {
                quary = quary.Where(n => n.No_Codes.Contains(search.commodityCode));
            }
            if (!string.IsNullOrEmpty(search.commodityCategory))
            {
                string[] commodityCategoryID = search.commodityCategory.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string commodityCategoryid in commodityCategoryID)
                {
                    if (!string.IsNullOrEmpty(commodityCategoryid))
                    {
                        idlist.Add(new Guid(commodityCategoryid));
                    }
                }
                quary = (from n in quary
                         join m in CommodityCategory.ObjectSet() on n.CommodityId equals m.CommodityId
                         where idlist.Contains(m.CategoryId)
                         select n).Distinct();
            }
            #endregion
            rownum = quary.Count();
            var query1 = quary.OrderByDescending(n => n.PromotionSubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            List<Guid> commodityIdList = query1.Select(a => a.CommodityId).Distinct().ToList();

            Dictionary<Guid, List<string>> categories = (from cc in CommodityCategory.ObjectSet()
                                                         join cate in Category.ObjectSet() on cc.CategoryId equals cate.Id
                                                         where commodityIdList.Contains(cc.CommodityId) && cc.AppId == search.appId
                                                         select new
                                                         {
                                                             key = cc.CommodityId,
                                                             value = cate.Name
                                                         }).GroupBy(a => a.key, a => a.value, (key, value) => new { Key = key, Value = value }).ToDictionary(a => a.Key, a => a.Value.ToList());


            //zgx-modify
            var comAttributeList = (from a in CommodityStock.ObjectSet()
                                    where commodityIdList.Contains(a.CommodityId)
                                    group a by a.CommodityId into g
                                    select new
                                    {
                                        minPrice = g.Min(a => a.Price),
                                        maxPrice = g.Max(a => a.Price),
                                        CommodityId = g.Key
                                    }).ToList();

            foreach (var c in query1)
            {
                if (categories.ContainsKey(c.CommodityId))
                {
                    c.CommodityCategorys = categories[c.CommodityId];
                }

                //zgx-modify
                if (comAttributeList != null && comAttributeList.Count > 0)
                {
                    var comAttribute = comAttributeList.Find(r => r.CommodityId == c.CommodityId);
                    if (comAttribute != null)
                    {
                        c.MaxPrice = comAttribute.maxPrice;
                        c.MinPrice = comAttribute.minPrice;
                    }
                }
            }

            return query1;
        }

        /// <summary>
        /// 根据促销ID查询促销详情
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM GetPromotionByPromotionIDExt(System.Guid promotionID)
        {
            int pageSize = int.MaxValue;

            PromotionItems promotionItems = new PromotionItems();
            var p = Promotion.ObjectSet().Where(n => n.Id == promotionID);
            IEnumerable<DiscountsVM> query = from data in p
                                             select new DiscountsVM
                                             {
                                                 PromotionId = data.Id,
                                                 StartTime = data.StartTime,
                                                 EndTime = data.EndTime,
                                                 PicturesPath = data.PicturesPath,
                                                 Intensity = data.Intensity,
                                                 Name = data.Name,
                                                 IsAll = data.IsAll,
                                                 DiscountPrice = data.DiscountPrice,
                                                 PromotionType = data.PromotionType
                                                 //No_Codes = promotionItems.GetPromotionItemsByPromotionID(promotionID, null, null).Select(n => n.No_Codes).ToList()
                                             };
            query = query.OrderByDescending(n => n.SubTime).ToList();
            var isAll = query.Select(n => n.IsAll).FirstOrDefault();

            DiscountsVM result = query.FirstOrDefault();

            if (isAll == false)
            {
                //item.No_Codes = promotionItems.GetPromotionItemsByPromotionID(promotionID, pageSize, 1).Select(n => n.No_Codes).ToList();
                result.No_Codes = (from n in PromotionItems.ObjectSet()
                                   join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                                   where n.PromotionId == promotionID && b.CommodityType == 0
                                   orderby b.SubTime descending
                                   select b.No_Code).ToList();

                var pclist = (from n in PromotionItems.ObjectSet()
                              join b in Commodity.ObjectSet() on n.CommodityId equals b.Id
                              where n.PromotionId == promotionID && b.CommodityType == 0
                              orderby b.SubTime descending
                              select new { b.No_Code, n.DiscountPrice, n.LimitBuyEach, n.LimitBuyTotal }).ToList();
                result.ComPro = (from b in pclist
                                 select
                                  b.No_Code + "|" + (b.DiscountPrice == null ? -1 : b.DiscountPrice).ToString()
                                 + "|" + (b.LimitBuyEach == null ? -1 : b.LimitBuyEach).ToString()
                                 + "|" + (b.LimitBuyTotal == null ? -1 : b.LimitBuyTotal).ToString()).ToArray();





            }
            return result;
        }

        /// <summary>
        /// 查询所有能打折所有商品ID
        /// </summary>
        /// <param name="appID">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetCommodityIDExt(System.Guid appID, DateTime startTime, DateTime endTime)
        {
            var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appID && n.IsDel == false && n.State == 0 && n.CommodityType == 0);
            var promotionItemsList = from data in Promotion.ObjectSet().Where(n => n.AppId == appID && n.StartTime < endTime && n.EndTime > startTime && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                                     join data1 in PromotionItems.ObjectSet() on data.Id equals data1.PromotionId
                                     select data1;
            var resultList = Enumerable.Except(commodityDTO.Select(n => n.Id), promotionItemsList.Select(n => n.CommodityId));
            return resultList.ToList();
        }


        /// <summary>
        /// 查询所有折扣商品的编号
        /// </summary>
        /// <param name="appid">appID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<string> GetCommodityCodeByPromotionExt(System.Guid appid, DateTime startTime, DateTime endTime)
        {
            //获取该app的促销商品列表
            var promotionItemlist =
                PromotionItems.ObjectSet().Where(n => n.AppId == appid)
                .Select(n => new
                {
                    PromotionId = n.PromotionId,
                    CommodityId = n.CommodityId
                });

            //获得尚未结束的促销
            var promotion = Promotion.ObjectSet().
                Where(n => n.AppId == appid && n.EndTime > DateTime.Now && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                .Select(n => new
                {
                    StartTime = n.StartTime,
                    EndTime = n.EndTime,
                    Id = n.Id
                });

            //这段时间内有促销的商品
            var commoditiesOnSale = from data in promotionItemlist
                                    join data1 in promotion.Where(n => n.StartTime <= endTime && n.EndTime >= startTime && n.StartTime <= n.EndTime)
                                    on data.PromotionId equals data1.Id
                                    select data.CommodityId;

            var resultList = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appid && n.IsDel == false && n.State == 0 && n.CommodityType == 0)
                .Where(n => commoditiesOnSale.Contains(n.Id)).Select(n => n.No_Code).ToList();
            return resultList;
        }


        /// <summary>
        /// 得到促销下所有的促销商品
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PromotionItemsDTO> GetAllPromotionItemsExt(System.Guid promotionId)
        {
            List<PromotionItems> romotionItems = PromotionItems.ObjectSet().Where(n => n.PromotionId == promotionId).ToList();
            return new PromotionItems().ToEntityDataList(romotionItems);
        }
        /// <summary>
        /// 删除促销商品
        /// </summary>
        /// <param name="promotionId"></param>
        public void DeletePromotionItemsExt(System.Guid promotionId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<PromotionItems> promotionItems = PromotionItems.ObjectSet().Where(n => n.PromotionId.Equals(promotionId)).ToList();
            foreach (PromotionItems item in promotionItems)
            {
                item.EntityState = System.Data.EntityState.Deleted;
                contextSession.Delete(item);
            }
            contextSession.SaveChange();
        }


        public bool IsAddPromotionExt(System.DateTime starTime, System.DateTime endTime, Guid appId)
        {
            starTime = starTime.Date;
            endTime = endTime.Date;
            List<Promotion> promotion = Promotion.ObjectSet().Where(n => n.AppId == appId && n.EndTime > DateTime.Now && n.IsEnable && !n.IsDel && n.PromotionType == 0).ToList(); //筛选一下已经过期的不要选择
            if (promotion == null || promotion.Count() == 0)
            {
                return true;
            }
            else
            {
                DateTime max = promotion.Select(n => n.EndTime).Max();
                DateTime min = promotion.Select(n => n.StartTime).Min();
                DateTime tempDateTime = min;
                List<DateTime> ThedayNotCan = new List<DateTime>();
                System.TimeSpan aa = max - min;
                int bb = Convert.ToInt32(aa.TotalDays);
                for (int i = 0; i < bb; i++)
                {
                    var query = promotion.Where(n => n.StartTime <= tempDateTime && n.EndTime >= tempDateTime);
                    if (query.Count() == 5)
                    {
                        ThedayNotCan.Add(tempDateTime);
                    }
                    tempDateTime = tempDateTime.AddDays(1);
                }
                foreach (var item in ThedayNotCan)
                {
                    if (starTime <= item && item <= endTime)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Guid> IsCommodityCanExt(System.DateTime starTime, System.DateTime endTime, Guid appId)
        {
            #region 优化前
            ////获取该app的促销商品列表
            //var promotionItemlist =
            //    PromotionItems.ObjectSet().Where(n => n.AppId == appId)
            //    .Select(n => new
            //    {
            //        PromotionId = n.PromotionId,
            //        CommodityId = n.CommodityId
            //    }).ToList();

            ////获得尚未结束的促销
            //var promotion = Promotion.ObjectSet().
            //    Where(n => n.AppId == appId && n.EndTime > DateTime.Now)
            //    .Select(n => new
            //    {
            //        StartTime = n.StartTime,
            //        EndTime = n.EndTime,
            //        Id = n.Id
            //    }).ToList();

            //DateTime max = endTime;
            //DateTime min = starTime;
            //System.TimeSpan aa = max - min;
            //List<Guid> TheComNotCan = new List<Guid>();
            //int bb = Convert.ToInt32(aa.TotalDays);
            //for (int i = 0; i < bb; i++)
            //{
            //    var query = from promitem in promotionItemlist
            //                join prom in promotion.Where(n => n.StartTime <= min && min <= n.EndTime)
            //                on promitem.PromotionId equals prom.Id
            //                select promitem;
            //    List<Guid> comlist = query.Select(n => n.CommodityId).ToList();
            //    TheComNotCan = TheComNotCan.Union(comlist).ToList();
            //    min = min.AddDays(1);
            //}

            ////查询所有不在促销中的商品            
            //TheComNotCan = TheComNotCan.Distinct().ToList();
            //var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appId && n.IsDel == false && n.State == 0);
            //var resultList = commodityDTO.Where(i => !TheComNotCan.Contains(i.Id)).Select(i => i.Id).ToList(); //排除正在促销的商品
            //return resultList;
            #endregion

            //获取该app的促销商品列表
            var promotionItemlist =
                PromotionItems.ObjectSet().Where(n => n.AppId == appId)
                .Select(n => new
                {
                    PromotionId = n.PromotionId,
                    CommodityId = n.CommodityId
                });

            //获得尚未结束的促销
            var promotion = Promotion.ObjectSet().
                Where(n => n.AppId == appId && n.EndTime > DateTime.Now && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                .Select(n => new
                {
                    StartTime = n.StartTime,
                    EndTime = n.EndTime,
                    Id = n.Id
                });

            //这段时间内有促销的商品
            var commoditiesOnSale = from data in promotionItemlist
                                    join data1 in promotion.
                                    Where(n => n.StartTime <= endTime && n.EndTime >= starTime && n.StartTime <= n.EndTime)
                                    on data.PromotionId equals data1.Id
                                    select data.CommodityId;

            var resultList = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appId && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                .Where(n => !commoditiesOnSale.Contains(n.Id)).Select(n => n.Id).ToList();
            return resultList;
        }

        /// <summary>
        /// 根据promotionID获得自己所有的商品信息
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        public List<CommodityDTO> GetCommodityByPromotionIDExt(Guid promotionID)
        {
            //根据promotionId从promotionItem表中获得commodityID
            var comIds = from b in PromotionItems.ObjectSet() where b.PromotionId.Equals(promotionID) select b.CommodityId;
            var comList = from a in Commodity.ObjectSet() where comIds.Contains(a.Id) && a.CommodityType == 0 select a;
            return new Commodity().ToEntityDataList(comList.ToList());
        }
        public int GetAllCommodityNumExt(System.DateTime starTime, System.DateTime endTime, Guid appId, Guid? promotionid)
        {
            int num = 0;
            Guid id = promotionid.HasValue ? promotionid.Value : new Guid();

            //获取该app的促销商品列表
            var promotionItemlist =
                PromotionItems.ObjectSet().Where(n => n.AppId == appId && n.PromotionId != id)
                .Select(n => new
                {
                    PromotionId = n.PromotionId,
                    CommodityId = n.CommodityId
                });

            //获得尚未结束的促销
            var promotion = Promotion.ObjectSet().
                Where(n => n.AppId == appId && n.EndTime > DateTime.Now && n.Id != id && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                .Select(n => new
                {
                    StartTime = n.StartTime,
                    EndTime = n.EndTime,
                    Id = n.Id
                });

            //这段时间内有促销的商品
            var commoditiesOnSale = from data in promotionItemlist
                                    join data1 in promotion.
                                    Where(n => n.StartTime <= endTime && n.EndTime >= starTime && n.StartTime <= n.EndTime)
                                    on data.PromotionId equals data1.Id
                                    select data.CommodityId;

            num = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appId && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                .Where(n => !commoditiesOnSale.Contains(n.Id)).Count();

            return num;
        }

        private List<Guid> GetCommodityIdsCan(System.DateTime starTime, System.DateTime endTime, Guid appId, Guid? promotionid)
        {
            Guid id = promotionid.HasValue ? promotionid.Value : new Guid();

            //获取该app的促销商品列表
            var promotionItemlist =
                PromotionItems.ObjectSet().Where(n => n.AppId == appId && n.PromotionId != id)
                .Select(n => new
                {
                    PromotionId = n.PromotionId,
                    CommodityId = n.CommodityId
                });

            //获得尚未结束的促销
            var promotion = Promotion.ObjectSet().
                Where(n => n.AppId == appId && n.EndTime > DateTime.Now && n.Id != id && !n.IsDel && n.IsEnable && n.PromotionType == 0)
                .Select(n => new
                {
                    StartTime = n.StartTime,
                    EndTime = n.EndTime,
                    Id = n.Id
                });

            //这段时间内有促销的商品
            var commoditiesOnSale = from data in promotionItemlist
                                    join data1 in promotion.
                                    Where(n => n.StartTime <= endTime && n.EndTime >= starTime && n.StartTime <= n.EndTime)
                                    on data.PromotionId equals data1.Id
                                    select data.CommodityId;

            return Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId == appId && n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0).Where(n => !commoditiesOnSale.Contains(n.Id)).Select(n => n.Id).ToList();

        }

        public List<string> GetCodesExt(List<string> commCodes, Guid appId)
        {
            return Jinher.AMP.BTP.BE.Commodity.ObjectSet().
                Where(n => n.IsDel.Equals(false) && n.AppId.Equals(appId) && n.State == 0 && n.CommodityType == 0)
               .Where(n => commCodes.Contains(n.No_Code))
                .Select(n => n.No_Code).ToList();
        }

        public List<Guid> GetCommodityIdsExt(Guid promotionId)
        {
            return (from b in Jinher.AMP.BTP.BE.PromotionItems.ObjectSet()
                    where b.PromotionId.Equals(promotionId)
                    select b.CommodityId).ToList();
        }

        public int GetCommodityNumExt(Guid promotionId)
        {
            return (from b in Jinher.AMP.BTP.BE.PromotionItems.ObjectSet()
                    where b.PromotionId.Equals(promotionId)
                    select b).Count();
        }
    }
}

