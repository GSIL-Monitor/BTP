
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/5/14 18:30:27
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using System.Data;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DiyGroupSV : BaseSv, IDiyGroup
    {

        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> GetDiyGroupDetailExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search)
        {
            ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> result = new ResultDTO<DiyGroupDetailDTO>();

            if (search == null || search.DiyGoupId == Guid.Empty)
            {
                result.ResultCode = 1;
                result.Message = "参数错误";
                return result;
            }
            var query = (from dg in DiyGroup.ObjectSet()
                         join c in Commodity.ObjectSet() on dg.CommodityId equals c.Id
                         join p in Promotion.ObjectSet() on dg.PromotionId equals p.Id
                         where dg.Id == search.DiyGoupId
                         select new
                             {
                                 DiyGroup = dg,
                                 Commodity = c,
                                 Promotion = p
                             }).FirstOrDefault();
            if (query == null || query.Commodity == null || query.DiyGroup == null || query.Promotion == null)
            {
                result.ResultCode = 2;
                result.Message = "没有获取到相应的拼团详情";
                return result;
            }
            var commodity = query.Commodity;
            var diyGroup = query.DiyGroup;
            var promotion = query.Promotion;
            var promotionItem = PromotionItems.ObjectSet().Where(t => t.PromotionId == diyGroup.PromotionId).FirstOrDefault();

            if (promotionItem == null)
            {
                result.ResultCode = 2;
                result.Message = "没有获取到相应的拼团详情";
                return result;
            }
            var productDetailPicture = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodity.Id).OrderBy(n => n.Sort).Select(c => c.PicturesPath).FirstOrDefault();
            result.Data = new DiyGroupDetailDTO()
                {
                    Id = diyGroup.Id,
                    Name = diyGroup.Name,
                    Code = diyGroup.Code,
                    SubTime = diyGroup.SubTime,
                    SubId = diyGroup.SubId,
                    AppId = diyGroup.AppId,
                    CommodityId = diyGroup.CommodityId,
                    PromotionId = diyGroup.PromotionId,
                    ModifiedOn = diyGroup.ModifiedOn,
                    ExpireTime = diyGroup.ExpireTime,
                    State = diyGroup.State,
                    JoinNumber = diyGroup.JoinNumber,
                    SuccessProcessorId = diyGroup.SuccessProcessorId,
                    SuccessTime = diyGroup.SuccessTime,
                    FailProcessorId = diyGroup.FailProcessorId,
                    FailTime = diyGroup.FailTime,
                    EsAppId = diyGroup.EsAppId,
                    //commodity
                    PicturesPath = commodity.PicturesPath,
                    CommodityName = commodity.Name,
                    ProductDetailPicture = productDetailPicture,
                    //promotion
                    StartTime = promotion.StartTime,
                    EndTime = promotion.EndTime,
                    GroupMinVolume = promotion.GroupMinVolume ?? -1,
                    ExpireSecond = promotion.ExpireSecond ?? -1,
                    Description = promotion.Description,
                    OutsideId = promotion.OutsideId.Value,
                    //promotionitem
                    LimitBuyEach = promotionItem.LimitBuyEach ?? -1,
                    LimitBuyTotal = promotionItem.LimitBuyTotal ?? -1,
                    DiscountPrice = promotionItem.DiscountPrice,
                    SurplusLimitBuyTotal = promotionItem.SurplusLimitBuyTotal ?? -1
                };
            result.Data.DiyGroupOrderList = new List<DiyGroupOrderDetailDTO>();
            var diyGroupOrder = (from dgOrder in DiyGroupOrder.ObjectSet()
                                 join co in CommodityOrder.ObjectSet() on dgOrder.OrderId equals co.Id
                                 where dgOrder.DiyGroupId == search.DiyGoupId && (dgOrder.Role == 0 || dgOrder.Role == 1 && dgOrder.State == 1)
                                 orderby dgOrder.SubTime ascending
                                 select new DiyGroupOrderDetailDTO
                                 {
                                     Id = dgOrder.Id,
                                     SubTime = dgOrder.SubTime,
                                     SubId = dgOrder.SubId,
                                     AppId = dgOrder.AppId,
                                     OrderId = dgOrder.OrderId,
                                     OrderCode = dgOrder.OrderCode,
                                     Role = dgOrder.Role,
                                     DiyGroupId = dgOrder.DiyGroupId,
                                     SubCode = dgOrder.SubCode,
                                     ModifiedOn = dgOrder.ModifiedOn,
                                     DiyGroupPrice = (decimal)co.RealPrice
                                 }).ToList();
            result.Data.DiyGroupOrderList.AddRange(diyGroupOrder);

            //提取用户信息
            var userIdList = result.Data.DiyGroupOrderList.Select(t => t.SubId).ToList();
            var userInfolist = CBCSV.Instance.GetUserInfoWithAccountList(userIdList);
            if (userInfolist != null && userInfolist.Count > 0)
            {
                foreach (var item in result.Data.DiyGroupOrderList)
                {
                    var tmpUserInfo = userInfolist.Where(t => t.UserId == item.SubId).FirstOrDefault();
                    if (tmpUserInfo != null)
                    {
                        item.UserCode = CBCSV.EncodeUserCode(tmpUserInfo.Account);
                        item.UserPicture = tmpUserInfo.HeadIcon;
                    }
                }
            }

            //var diyHeadOrder = diyGroupOrder.Where(t => t.Role == 0).FirstOrDefault();
            //if (diyHeadOrder != null && diyHeadOrder.State == 0)
            //{
            //    result.Data.JoinNumber = result.Data.JoinNumber + 1;
            //}

            //系统当前时间，倒计时用
            result.Data.DateTimeNow = DateTime.Now;
            return result;
        }


        /// <summary>
        /// 处理超时未成团
        /// </summary>
        public void DealUnDiyGroupTimeoutExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;

                //while (true)
                {
                    DateTime now = DateTime.Now;

                    var diygQuery = (from diyg in DiyGroup.ObjectSet()
                                     where diyg.ExpireTime < now && diyg.State == 1
                                     select diyg).Take(pageSize).ToList();
                    foreach (var diy in diygQuery)
                    {
                        diy.State = 4;
                        diy.ModifiedOn = now;
                        diy.EntityState = System.Data.EntityState.Modified;
                    }
                    contextSession.SaveChanges();

                    //if (diygQuery.Count < pageSize)
                    //{
                    //    break;
                    //}
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("DealUnDiyGroupTimeoutExt异常：", ex);
            }
        }
        /// <summary>
        /// 处理 未成团退款
        /// </summary>
        public void DealUnDiyGroupRefundExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var diyGroupOrders = (from diyg in DiyGroup.ObjectSet()
                                      join diyGroupOrder in DiyGroupOrder.ObjectSet() on diyg.Id equals diyGroupOrder.DiyGroupId
                                      where diyg.State == 5 && diyGroupOrder.State == 1
                                      select diyGroupOrder).ToList();

                LogHelper.Debug("开始进入DealUnDiyGroupRefundExt，diyOrderDictionary：" + JsonHelper.JsSerializer(diyGroupOrders));

                foreach (var diyGroupOrder in diyGroupOrders)
                {
                    bool isOk = OrderSV.RefundOrderDiyGroup(diyGroupOrder.OrderId);
                    if (isOk)
                    {
                        var diy = DiyGroup.FindByID(diyGroupOrder.DiyGroupId);
                        diy.State = 6;
                        diy.FailTime = DateTime.Now;
                        diy.ModifiedOn = DateTime.Now;
                        diy.EntityState = EntityState.Modified;
                        contextSession.SaveObject(diy);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("DealUnDiyGroupRefundExt异常：", ex);
            }
        }
        /// <summary>
        /// 我的拼团订单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> GetDiyGroupListExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> resultlist = new List<DiyGroupOrderListDTO>();
            if (search == null)
            {
                return resultlist;
            }
            search.PageIndex = search.PageIndex == 0 ? 1 : search.PageIndex;
            search.PageSize = search.PageSize == 0 ? 20 : search.PageSize;

            var diyList = (from diy in DiyGroup.ObjectSet()
                           join pro in PromotionItems.ObjectSet() on diy.PromotionId equals pro.PromotionId
                           join p in Promotion.ObjectSet() on pro.PromotionId equals p.Id
                           join diyo in DiyGroupOrder.ObjectSet() on diy.Id equals diyo.DiyGroupId
                           join com in CommodityOrder.ObjectSet() on diyo.OrderId equals com.Id                         
                           where diy.EsAppId == search.EsAppId && com.UserId == search.UserId && diyo.State == 1 && diy.State != 0
                           orderby diyo.SubTime descending
                           select new DiyGroupOrderListDTO
                               {
                                   Price = com.RealPrice,
                                   DiyGroupPrice = pro.DiscountPrice,
                                   SubTime = diyo.SubTime,
                                   DiyGroupState = diy.State,
                                   GroupMinVolume=p.GroupMinVolume ?? -1,
                                   JoinNumber=diy.JoinNumber,
                                   DiyGroupId = diy.Id,
                                   EsAppId = diy.EsAppId,
                                   OrderId = diyo.OrderId,
                                   DiyOrderSubTime = diyo.SubTime,
                                   ModifiedOn = diyo.ModifiedOn,
                                   EndTime=p.EndTime
                               }).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

            var diyorderIds = diyList.Select(n => n.OrderId).ToList();
            if (diyList.Any())
            {
                var diyorderList = (from ord in OrderItem.ObjectSet()
                                    where diyorderIds.Contains(ord.CommodityOrderId)
                                    select new DiyGroupManageMM
                                        {
                                            Pic = ord.PicturesPath,
                                            Name = ord.Name,
                                            DiyNumber = ord.Number,
                                            DiyGroupOrderId = ord.CommodityOrderId,
                                            attributes = ord.CommodityAttributes
                                        }).ToList();                
                Dictionary<Guid, List<DiyGroupManageMM>> csdtoList = diyorderList.GroupBy(c => c.DiyGroupOrderId, (key, group) =>
                    new { DiyGroupOrderId = key, CommodityList = group })
                      .ToDictionary(c => c.DiyGroupOrderId, c => c.CommodityList.ToList());
                var listAppIds = (from co in diyList select co.EsAppId).Distinct().ToList();
                Dictionary<Guid, string> dictAppName = APPSV.GetAppNameListByIds(listAppIds);

                foreach (var diyGroupOrder in diyList)
                {
                    if (csdtoList.ContainsKey(diyGroupOrder.OrderId))
                    {
                        var commodityDtoList = csdtoList[diyGroupOrder.OrderId];
                        diyGroupOrder.OrderDataList = commodityDtoList;
                    }
                    if (dictAppName != null && dictAppName.Count > 0 && dictAppName.ContainsKey(diyGroupOrder.EsAppId))
                    {
                        var appNameDto = dictAppName[diyGroupOrder.EsAppId];
                        diyGroupOrder.AppName = appNameDto;
                    }
                    resultlist.Add(diyGroupOrder);
                }

            }
            return resultlist;
        }

        /// <summary>
        /// 自动确认成团 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyConfirmDiyGroupExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //获取所有组团成功，但是没有手动成团的拼团订单
                var diyQuery = (from diy in DiyGroup.ObjectSet()
                                join c in Commodity.ObjectSet() on diy.CommodityId equals c.Id
                                join ps in PromotionItems.ObjectSet() on c.Id equals ps.CommodityId
                                join p in Promotion.ObjectSet() on ps.PromotionId equals p.Id
                                where diy.JoinNumber != 0 && diy.PromotionId == p.Id && diy.State == 2
                                select new DiyGroupManageVM
                                {
                                    DiyId = diy.Id,
                                    AppId = diy.EsAppId
                                });
                diyQuery = diyQuery.Distinct();

                List<DiyGroupConfig> list = new List<DiyGroupConfig>();
                foreach (var diyGroupManageVm in diyQuery)
                {
                    DiyGroupConfigCDTO diyGroupConfig;
                    if (list.Count > 0 && list.Any(t => t.EsAppId == diyGroupManageVm.AppId))
                    {
                        diyGroupConfig = list.FirstOrDefault(t => t.EsAppId == diyGroupManageVm.AppId).DiyGroupConfigCdto;
                    }
                    else
                    {
                        diyGroupConfig = TPS.ZPHSV.Instance.GetDiyGroupConfig(diyGroupManageVm.AppId);
                        DiyGroupConfig diyGroupConfigBp = new DiyGroupConfig
                        {
                            EsAppId = diyGroupManageVm.AppId,
                            DiyGroupConfigCdto = diyGroupConfig
                        };
                        list.Add(diyGroupConfigBp);
                    }

                    if (diyGroupConfig.IsClustering)
                    {
                        var query = (from diyGroup in DiyGroup.ObjectSet()
                                     join diyGroupOrder in DiyGroupOrder.ObjectSet() on diyGroup.Id equals diyGroupOrder.DiyGroupId
                                     join order in CommodityOrder.ObjectSet() on diyGroupOrder.OrderId equals order.Id
                                     where diyGroup.Id == diyGroupManageVm.DiyId && diyGroupOrder.State == 1 && diyGroup.State == 2
                                     select order
                            ).ToList();

                        if (query.Count > 0)
                        {
                            foreach (var diyorder in query)
                            {
                                diyorder.State = 1;
                                diyorder.ModifiedOn = DateTime.Now;
                                diyorder.EntityState = System.Data.EntityState.Modified;
                                contextSession.SaveObject(diyorder);
                                Jinher.AMP.BTP.BE.BELogic.AddMessage addmassage = new Jinher.AMP.BTP.BE.BELogic.AddMessage();
                                addmassage.AddMessages(diyorder.Id.ToString(), diyorder.UserId.ToString(), diyorder.AppId, diyorder.Code, diyorder.State, "", "order");
                                // 触发订单成功事件
                                OrderEventHelper.OnOrderPaySuccess(diyorder);
                            }
                        }
                        var diyquery = DiyGroup.ObjectSet().FirstOrDefault(n => n.Id == diyGroupManageVm.DiyId && n.EsAppId == diyGroupManageVm.AppId && n.State == 2);
                        if (diyquery != null)
                        {
                            diyquery.State = 3;
                            diyquery.ModifiedOn = DateTime.Now;
                            diyquery.EntityState = EntityState.Modified;
                            contextSession.SaveObject(diyquery);
                        }
                    }
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("自动确认成团 -- JOB调用异常"), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 成团自动退款 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyRefundDiyGroupExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //获取所有组团未成功，但是没有手动退款的拼团订单
                var diyQuery = (from diy in DiyGroup.ObjectSet()
                                join c in Commodity.ObjectSet() on diy.CommodityId equals c.Id
                                join ps in PromotionItems.ObjectSet() on c.Id equals ps.CommodityId
                                join p in Promotion.ObjectSet() on ps.PromotionId equals p.Id
                                where diy.JoinNumber != 0 && diy.PromotionId == p.Id && diy.State == 4
                                select new DiyGroupManageVM
                                {
                                    DiyId = diy.Id,
                                    AppId = diy.EsAppId
                                });
                diyQuery = diyQuery.Distinct();

                List<DiyGroupConfig> list = new List<DiyGroupConfig>();
                foreach (var diyGroupManageVm in diyQuery)
                {
                    DiyGroupConfigCDTO diyGroupConfig;
                    if (list.Count > 0 && list.Any(t => t.EsAppId == diyGroupManageVm.AppId))
                    {
                        diyGroupConfig = list.FirstOrDefault(t => t.EsAppId == diyGroupManageVm.AppId).DiyGroupConfigCdto;
                    }
                    else
                    {
                        diyGroupConfig = TPS.ZPHSV.Instance.GetDiyGroupConfig(diyGroupManageVm.AppId);
                        DiyGroupConfig diyGroupConfigBp = new DiyGroupConfig
                        {
                            EsAppId = diyGroupManageVm.AppId,
                            DiyGroupConfigCdto = diyGroupConfig
                        };
                        list.Add(diyGroupConfigBp);
                    }

                    if (diyGroupConfig.IsRefund)
                    {
                        var query = DiyGroup.ObjectSet().FirstOrDefault(n => n.Id == diyGroupManageVm.DiyId && n.EsAppId == diyGroupManageVm.AppId);
                        if (query != null)
                        {
                            query.State = 5;
                            query.ModifiedOn = DateTime.Now;
                            query.EntityState = EntityState.Modified;
                        }
                    }
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("成团自动退款 -- JOB调用"), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 检查拼团状态
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public ResultDTO<CheckDiyGroupOutputDTO> CheckDiyGroupExt(CheckDiyGroupInputDTO inputDTO)
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException();
            }

            var isCompleted = DiyGroup.ObjectSet().Any(any => any.Id == inputDTO.DiyGroupId && any.State > 1);

            var outputDTO = new ResultDTO<CheckDiyGroupOutputDTO>
            {
                isSuccess = true,
                ResultCode = 0,
                Data = new CheckDiyGroupOutputDTO
                {
                    IsCompleted = isCompleted
                }
            };

            return outputDTO;
        }
    }

    [Serializable()]
    [DataContract]
    public class DiyGroupConfig
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 拼团设置
        /// </summary>
        [DataMemberAttribute()]
        public ZPH.Deploy.CustomDTO.DiyGroupConfigCDTO DiyGroupConfigCdto { get; set; }
    }
}