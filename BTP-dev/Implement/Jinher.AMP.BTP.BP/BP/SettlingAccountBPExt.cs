
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/17 14:51:03
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 结算操作类
    /// </summary>
    public partial class SettlingAccountBP : BaseBP, ISettlingAccount
    {
        #region 查询某个商家所有上架商品

        /// <summary>
        /// 获取当前商品结算列表
        /// </summary>
        /// <param name="id">商品结算价检索类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> GetNowSettlingAccountExt(SettlingAccountSearchDTO search, out int rowCount)
        {    

            List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> comcalist=new List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM>();

            if (search == null || search.appId == Guid.Empty || search.pageIndex<1 || search.pageSize<1)
            {
                rowCount = 0;
                return comcalist;
            }

              DateTime now = DateTime.Now;

            //appid
            var query = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId.Equals(search.appId) && n.State == 0 && n.CommodityType == 0);


            //commodityName
            if (!string.IsNullOrWhiteSpace(search.commodityName))
            {
                query = query.Where(n => n.Name.Contains(search.commodityName));
            }

            //commodityCategory
            if (!string.IsNullOrWhiteSpace(search.commodityCategory))
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
                query = from n in query
                        join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                        where idlist.Contains(m.CategoryId)
                        select n;
            }

            query = query.Distinct();
            rowCount = query.Count();
            query = query.OrderBy(n => n.SortValue).ThenByDescending(n => n.SubTime).Skip((search.pageIndex - 1) * search.pageSize).Take(search.pageSize);

            var commodityIds = query.Select(t => t.Id).ToList();

            //取出厂家结算价
            var settlingAccountQuery = (
                    from s in SettlingAccount.ObjectSet()
                    where commodityIds.Contains(s.CommodityId) && s.EffectiveTime < now
                    select new
                    {
                        Id = s.Id,
                        CommodityId = s.CommodityId,
                        ManufacturerClearingPrice = s.ManufacturerClearingPrice,
                        AppId = s.AppId,
                        Effectable = s.Effectable,
                        EffectiveTime = s.EffectiveTime,
                        SubId = s.SubId,
                        SubName = s.SubName,
                        SubTime = s.SubTime,
                        ModifiedOn = s.ModifiedOn,
                        UserCode = s.UserCode

                    })
                    .GroupBy(t => t.CommodityId).ToDictionary(x => x.Key, y => y.OrderByDescending(c => c.EffectiveTime).First());
                  
            
            //取出商品的销售价
            var compromoList = (from t in TodayPromotion.ObjectSet()
                                where commodityIds.Contains(t.CommodityId) && t.EndTime > now && t.StartTime < now
                                select new
                                {
                                    SurplusLimitBuyTotal = t.SurplusLimitBuyTotal,
                                    ComdityID = t.CommodityId,
                                    LimitBuyEach = t.LimitBuyEach,
                                    PromotionId = t.PromotionId,
                                    ID = t.Id,
                                    LimitBuyTotal = t.LimitBuyTotal,
                                    Intensity = t.Intensity,
                                    DiscountPrice = t.DiscountPrice,
                                    PromotionType = t.PromotionType,
                                    ChannelId = t.ChannelId,
                                    OutsideId = t.OutsideId

                                }).GroupBy(c => c.ComdityID).ToDictionary(x => x.Key, y => y.OrderByDescending(c => c.PromotionType).First());

            var comList = query.ToList();

            int? intNull = null;
            DateTime? dateTimeNull = null;
            Guid? guidNull = null;
            Decimal? decimalNull = null;

            foreach (var com in comList)
            {
                var settlingAccountTmp = settlingAccountQuery.ContainsKey(com.Id) ? settlingAccountQuery[com.Id] : null;

                decimal Intensity = 10;
                decimal? DiscountPrice = -1;
                decimal realPrice = com.Price;
                if (compromoList.ContainsKey(com.Id))
                {
                    if (compromoList[com.Id].DiscountPrice > -1)
                    {
                        Intensity = 10;
                        DiscountPrice = compromoList[com.Id].DiscountPrice==null?-1:compromoList[com.Id].DiscountPrice;
                    }
                    else
                    {
                        Intensity = compromoList[com.Id].Intensity == null ? 10 : compromoList[com.Id].Intensity;
                        DiscountPrice = -1;
                    }

                    realPrice = (DiscountPrice > -1)
                                     ? DiscountPrice.Value
                                     : decimal.Round((com.Price * Intensity / 10), 2, MidpointRounding.AwayFromZero);
                }

                Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM settingAccount = new Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM()
                {
                  AppId= com.AppId,
                  CommodityCode=com.No_Code,
                  CommodityId=com.Id,
                  CommodityName =com.Name,
                  Effectable = settlingAccountTmp == null ? intNull : settlingAccountTmp.Effectable,
                  EffectiveTime = settlingAccountTmp == null ? dateTimeNull : settlingAccountTmp.EffectiveTime,
                  Id = settlingAccountTmp == null ? guidNull : settlingAccountTmp.Id,
                   IsEnableSelfTake = com.IsEnableSelfTake,
                  UserCode = settlingAccountTmp == null ? null : settlingAccountTmp.UserCode,
                  ManufacturerClearingPrice = settlingAccountTmp == null ? decimalNull : settlingAccountTmp.ManufacturerClearingPrice,
                  ModifiedOn = settlingAccountTmp == null ? dateTimeNull : settlingAccountTmp.ModifiedOn,
                   PicturesPath =com.PicturesPath,
                  SalePrice = realPrice,
                  SubId = settlingAccountTmp == null ? guidNull : settlingAccountTmp.SubId,
                  SubName = settlingAccountTmp == null ? null : settlingAccountTmp.SubName,
                  SubTime = settlingAccountTmp == null ? dateTimeNull : settlingAccountTmp.SubTime,                  
           
                };
                comcalist.Add(settingAccount);
                
            }          
            
            return comcalist;
        }

        /// <summary>
        /// 添加商品的厂家结算价
        /// </summary>
        /// <param name="settlingAccountDTO">结算价实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSettlingAccountExt(Jinher.AMP.BTP.Deploy.SettlingAccountDTO settlingAccountDTO)
        {
            //参数判断
            if (settlingAccountDTO.CommodityId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "商品ID不能为空" };
            }

            //生效时间的判断，只判断不能为重的情况
            var tmp = SettlingAccount.ObjectSet().Where(t => t.CommodityId == settlingAccountDTO.CommodityId && t.EffectiveTime == settlingAccountDTO.EffectiveTime).Count();
            if (tmp > 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "已添加了该商品在该生效时间的厂家结算价" };                
            } 

            try
            {
                SettlingAccount commodity = new SettlingAccount()
                {
                    Id = settlingAccountDTO.Id,
                    CommodityId = settlingAccountDTO.CommodityId,
                    ManufacturerClearingPrice = settlingAccountDTO.ManufacturerClearingPrice,
                    AppId = settlingAccountDTO.AppId,
                    Effectable = settlingAccountDTO.Effectable,
                    EffectiveTime = settlingAccountDTO.EffectiveTime,
                    SubId = settlingAccountDTO.SubId,
                    SubName = settlingAccountDTO.SubName,
                    UserCode = settlingAccountDTO.UserCode,

                };
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                commodity.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(commodity);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加商品的厂家结算价服务异常。settlingAccountDTO：{0}", JsonHelper.JsonSerializer(settlingAccountDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取当前商品厂家结算价设置的历史列表
        /// </summary>
        /// <param name="search">商品结算价修改历史检索类</param>
        /// <param name="rowCount">记录数</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> GetHistorySettlingAccountExt(Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO search, out int rowCount)
        {
            List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> result = new List<SettlingAccountDTO>();
            if (search == null || search.CommodityId == Guid.Empty || search.PageIndex < 1 || search.PageSize < 1)
            {
                rowCount = 0;
                return result;
            }

            var query = SettlingAccount.ObjectSet().Where(n => n.CommodityId == search.CommodityId);
            rowCount = query.Count();
            query = query.OrderByDescending(n => n.SubTime).Skip((search.PageIndex - 1) * search.PageIndex).Take(search.PageSize);

            result = (from q in query
                      select new Jinher.AMP.BTP.Deploy.SettlingAccountDTO
                      {
                          Id = q.Id,
                          CommodityId = q.CommodityId,
                          ManufacturerClearingPrice = q.ManufacturerClearingPrice,
                          AppId = q.AppId,
                          Effectable = q.Effectable,
                          EffectiveTime = q.EffectiveTime,
                          SubId = q.SubId,
                          SubName = q.SubName,
                          SubTime = q.SubTime,
                          ModifiedOn = q.ModifiedOn,
                          UserCode = q.UserCode,

                      }).ToList();

            return result;
        }


        /// <summary>
        /// 删除厂家结算记录
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSettlingAccountByIdExt(List<Guid> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (Guid item in ids)
                {
                    var settlingAccount = SettlingAccount.ObjectSet().Where(n => n.Id == item).FirstOrDefault();
                    if (!string.IsNullOrEmpty(settlingAccount.ToString()))
                    {
                        settlingAccount.EntityState = System.Data.EntityState.Deleted;
                        contextSession.SaveChanges();
                    }
                }
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除多个商品服务异常。ids：{0}", JsonHelper.JsonSerializer(ids)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

        }

        #endregion 
    }
}
