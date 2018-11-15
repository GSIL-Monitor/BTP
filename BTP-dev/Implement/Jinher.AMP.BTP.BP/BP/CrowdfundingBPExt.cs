
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/12/29 15:18:45
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Jinher.AMP.App.ISV.Facade;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Runtime.Serialization.Json;
using System.IO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.AMP.BTP.BE.BELogic;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CrowdfundingBP : BaseBP, ICrowdfunding
    {

        /// <summary>
        /// 添加众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        public ResultDTO AddCrowdfundingExt(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            if (crowdfundingDTO == null)
                return new ResultDTO { ResultCode = 2, Message = "入参不能为空" };
            if (crowdfundingDTO.AppId == Guid.Empty)
                return new ResultDTO { ResultCode = 2, Message = "请正确填写应用" };

            var dividendPercent = crowdfundingDTO.DividendPercent * crowdfundingDTO.ShareCount;
            if (dividendPercent > CustomConfig.CrowdfundingConfig.MaxDividend)
                return new ResultDTO { ResultCode = 2, Message = "您的订单成交额已不足抵扣分红，请重新设置！" };

            if (crowdfundingDTO.StartTime < DateTime.Now)
            {
                return new ResultDTO { ResultCode = 2, Message = "众筹开始时间必须大于当前时间！" };
            }

            string message = "Success";

            try
            {
                var tmp = Crowdfunding.ObjectSet().FirstOrDefault(c => c.AppId == crowdfundingDTO.AppId);
                if (tmp != null)
                    return new ResultDTO { ResultCode = 3, Message = "该应用已经参加众筹活动，不能重复添加" };

                Dictionary<Guid, string> list = APPSV.GetAppNameListByIds(new List<Guid> { crowdfundingDTO.AppId });

                if (list == null || !list.Any() || !list.ContainsKey(crowdfundingDTO.AppId) || list[crowdfundingDTO.AppId] != crowdfundingDTO.AppName.Trim())
                    return new ResultDTO { ResultCode = 2, Message = "应用Id与名称不符" };

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //保存众筹
                Crowdfunding entity = Crowdfunding.CreateCrowdfunding();
                entity.AppName = crowdfundingDTO.AppName;
                entity.AppId = crowdfundingDTO.AppId;
                entity.PerShareMoney = crowdfundingDTO.PerShareMoney;
                entity.DividendPercent = crowdfundingDTO.DividendPercent;
                entity.ShareCount = crowdfundingDTO.ShareCount;
                entity.StartTime = crowdfundingDTO.StartTime;
                entity.State = 0;
                entity.Slogan = crowdfundingDTO.Slogan;
                entity.Description = crowdfundingDTO.Description;
                contextSession.SaveObject(entity);

                //保存众筹计数表
                CrowdfundingCount cnt = CrowdfundingCount.CreateCrowdfundingCount();
                cnt.AppId = entity.AppId;
                cnt.CrowdfundingId = entity.Id;
                cnt.ShareCount = entity.ShareCount;
                contextSession.SaveObject(cnt);

                contextSession.SaveChanges();
                if (dividendPercent > CustomConfig.CrowdfundingConfig.WarnDividend)
                    message = string.Format("您的分红支出已超{0:P0}，请注意收支平衡！", CustomConfig.CrowdfundingConfig.WarnDividend);

                CrowdfundingMessageDTO Message = new CrowdfundingMessageDTO();
                Message.Now = DateTime.Now;
                Message.StartTime = entity.StartTime;
                Message.State = -1;
                AddMessage addMessage = new AddMessage();
                addMessage.SendMessage(entity.Id, entity.AppId, entity.StartTime, Message);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加众筹服务异常。crowdfundingDTO：{0}", crowdfundingDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = message };
        }
        /// <summary>
        /// 更新众筹
        /// </summary>
        /// <param name="crowdfundingDTO">众筹实体</param>
        public ResultDTO UpdateCrowdfundingExt(Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO)
        {
            if (crowdfundingDTO == null)
                return new ResultDTO { ResultCode = 2, Message = "DTO is null" };
            Crowdfunding crowdfunding = Crowdfunding.ObjectSet().FirstOrDefault(c => c.Id == crowdfundingDTO.Id);
            if (crowdfunding == null)
                return new ResultDTO { ResultCode = 2, Message = "BE is null" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                crowdfunding.Slogan = crowdfundingDTO.Slogan;
                crowdfunding.Description = crowdfundingDTO.Description;
                crowdfunding.EntityState = EntityState.Modified;
                contextSession.SaveObject(crowdfunding);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改众筹服务异常。crowdfundingDTO：{0}", crowdfundingDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="id">众筹Id</param>
        public Jinher.AMP.BTP.Deploy.CrowdfundingDTO GetCrowdfundingExt(System.Guid id)
        {

            Jinher.AMP.BTP.Deploy.CrowdfundingDTO crowdfundingDTO = new CrowdfundingDTO();
            try
            {

                if (id == null)
                {
                    crowdfundingDTO = null;
                }
                else
                {

                    var query = Crowdfunding.ObjectSet().Where(c => c.Id == id).FirstOrDefault();
                    if (query != null)
                    {
                        crowdfundingDTO = query.ToEntityData();

                    }
                    else
                    {
                        crowdfundingDTO = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取众筹列表服务异常。id：{0}", id), ex);
                return null;

            }


            return crowdfundingDTO;


        }
        /// <summary>
        /// 获取众筹列表
        /// </summary>
        /// <param name="appName">app名称</param>
        /// <param name="cfState">众筹状态(-1,不限)</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public GetCrowdfundingsDTO GetCrowdfundingsExt(string appName, int cfState, int pageIndex, int pageSize)
        {

            GetCrowdfundingsDTO getCrowdfundingsDTO = new GetCrowdfundingsDTO();
            if (pageIndex < 1 || pageSize < 1)
                return null;
            try
            {
                var querys = (from query in Crowdfunding.ObjectSet()
                              join cnt in CrowdfundingCount.ObjectSet() on query.Id equals cnt.CrowdfundingId
                              where (string.IsNullOrEmpty(appName) ? true : (query.AppName.Contains(appName))) &&
                              (cfState == -1 ? true : (cfState == query.State))
                              select new CrowdfundingFullDTO
                              {
                                  Id = query.Id,
                                  AppId = query.AppId,
                                  AppName = query.AppName,
                                  CurrentShareCount = cnt.CurrentShareCount,
                                  Description = query.Description,
                                  DividendPercent = query.DividendPercent,
                                  PerShareMoney = query.PerShareMoney,
                                  ShareCount = query.ShareCount,
                                  Slogan = query.Slogan,
                                  StartTime = query.StartTime,
                                  State = query.State,
                                  SubTime = query.SubTime,
                                  TotalDividend = (decimal)cnt.TotalDividend / 1000
                              }
                            ).OrderByDescending(q => q.SubTime);

                var list = querys.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                if (list != null && list.Count > 0)
                {
                    getCrowdfundingsDTO.List = list;
                    getCrowdfundingsDTO.Total = querys.Count();


                }
                else
                {

                    getCrowdfundingsDTO.Total = 0;
                }

                return getCrowdfundingsDTO;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取众筹列表服务异常。appName：{0}，cfState：{1}，pageIndex：{2}，pageSize：{3}", appName, cfState, pageIndex, pageSize), ex);
                return null;
            }
        }
        /// <summary>
        /// 获取众筹股东列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="userCode">用户账号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public GetUserCrowdfundingsDTO GetUserCrowdfundingsExt(System.Guid crowdfundingId, string userName, string userCode, int pageIndex, int pageSize)
        {

            GetUserCrowdfundingsDTO getUserCrowdfundingsDTO = new GetUserCrowdfundingsDTO();
            try
            {
                var result = (from query in UserCrowdfunding.ObjectSet()
                              where query.CrowdfundingId == crowdfundingId && query.CurrentShareCount > 0 && (string.IsNullOrEmpty(userName) ? true : query.UserName.Contains(userName.Trim()))
                              && (string.IsNullOrEmpty(userCode) ? true : query.UserCode.Contains(userCode.Trim()))
                              select new Jinher.AMP.BTP.Deploy.CustomDTO.UserCrowdfundingDTO
                              {
                                  Id = query.Id,
                                  AppId = query.AppId,
                                  CrowdfundingId = query.CrowdfundingId,
                                  OrderCount = query.OrderCount,
                                  UserId = query.UserId,
                                  UserCode = query.UserCode,
                                  UserName = query.UserName,
                                  Money = query.OrdersMoney,
                                  CurrentShareCount = query.CurrentShareCount,
                                  TotalDividend = query.TotalDividend,
                                  RealGetDividend = query.RealGetDividend,
                                  SubTime = query.SubTime,
                                  ModifiedOn = query.ModifiedOn

                              }

                              ).OrderByDescending(q => q.SubTime);


                var list = result.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                if (list != null && list.Count > 0)
                {
                    getUserCrowdfundingsDTO.List = list;
                    getUserCrowdfundingsDTO.Total = result.Count();

                }
                else
                {

                    getUserCrowdfundingsDTO.Total = 0;
                }

                return getUserCrowdfundingsDTO;


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取众筹列表服务异常。crowdfundingId：{0}，userName：{1}，userCode：{2}，pageIndex：{3}，pageSize：{4}", crowdfundingId, userName, userCode, pageIndex, pageSize), ex); 
                return null;
            }
        }
        /// <summary>
        /// 众筹股东订单列表
        /// </summary>
        /// <param name="crowdfundingId">众筹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public CommodityOrderVMDTO GetUserCrowdfundingOrdersExt(System.Guid crowdfundingId, System.Guid userId, int pageIndex, int pageSize)
        {
            CommodityOrderVMDTO CommodityOrderVMDTO = new CommodityOrderVMDTO();

            try
            {

                var query = from data in CommodityOrder.ObjectSet()
                            join CrowdfundingData in Crowdfunding.ObjectSet()
                                on data.AppId equals CrowdfundingData.AppId
                            join data1 in OrderRefund.ObjectSet() 
                                on data.Id equals data1.OrderId
                                into tempT
                            from tb3 in tempT.DefaultIfEmpty()
                            join cf in CfOrderDividend.ObjectSet()
                                on data.Id equals cf.CommodityOrderId into CfOrderDividends
                            from cfOrderDividend in CfOrderDividends.DefaultIfEmpty()
                            where
                                data.UserId == userId && data.IsCrowdfunding != 0 && data.State != 0 &&
                                CrowdfundingData.Id == crowdfundingId
                                &&
                                (tb3.RefundMoney == null ? 0 : tb3.RefundMoney) <
                                (data.IsModifiedPrice ? data.RealPrice : data.Price)
                                && tb3.State != 2 && tb3.State != 3 && tb3.State != 4 && tb3.State != 13
                            select new CommodityOrderVM
                                {
                                    AppId = data.AppId,
                                    UserId = data.UserId,
                                    CommodityOrderId = data.Id,
                                    CurrentPrice = data.RealPrice,
                                    State = data.State,
                                    CommodityOrderCode = data.Code,
                                    SubTime = data.SubTime,
                                    ReceiptUserName = data.ReceiptUserName,
                                    Payment = data.Payment,
                                    PaymentTime = data.PaymentTime,
                                    ShipmentsTime = data.ShipmentsTime,
                                    ConfirmTime = data.ConfirmTime,
                                    ModifiedOn = data.ModifiedOn,
                                    Price = data.Price,
                                    MessageToBuyer = data.MessageToBuyer,
                                    IsModifiedPrice = data.IsModifiedPrice,
                                    ReceiptPhone = data.ReceiptPhone,
                                    RefundTime = data.RefundTime,
                                    AgreementTime = data.AgreementTime,
                                    Freight = data.Freight,
                                    Commission = data.Commission,
                                    IsCrowdfunding = true,
                                    CfDividend = cfOrderDividend.Gold
                                };

                var result = query.OrderByDescending(q => q.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                #region 查询订单商品信息(优化)

                //构建订单id数组
                List<Guid> commodityOrderIdList = new List<Guid>();
                for (int i = 0; i < result.Count; i++)
                {
                    commodityOrderIdList.Add(result[i].CommodityOrderId);
                }

                CommodityCategory cc = new CommodityCategory();

                //取出所有订单的所有商品
                var orderItems = (from data in OrderItem.ObjectSet()
                                  //join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                                  where commodityOrderIdList.Contains(data.CommodityOrderId)
                                  select new
                                  {
                                      CommodityOrderId = data.CommodityOrderId,
                                      CommodityId = data.CommodityId,
                                      CommodityIdName = data.Name,
                                      PicturesPath = data.PicturesPath,
                                      Price = data.CurrentPrice,//取订单商品列表中的价格
                                      Number = data.Number,
                                      CommodityAttributes = data.CommodityAttributes,
                                      CategoryName = data.CategoryNames,
                                      RealPrice = data.RealPrice

                                  }).ToList();

                List<OrderItemsVM> orderItemsVMList = (from data in orderItems
                                                       select new OrderItemsVM
                                                       {
                                                           CommodityOrderId = data.CommodityOrderId,
                                                           CommodityId = data.CommodityId,
                                                           CommodityIdName = data.CommodityIdName,
                                                           PicturesPath = data.PicturesPath,
                                                           Price = data.Price,//取订单商品列表中的价格
                                                           RealPrice = data.RealPrice,
                                                           Number = data.Number,
                                                           SizeAndColorId = data.CommodityAttributes,
                                                           CommodityCategorys =
                                                           data.CategoryName == null ? new List<string>() : data.CategoryName.Split(',').ToList()

                                                       }).ToList();

                Collection collect = new Collection();

                //遍历订单
                foreach (CommodityOrderVM v in result)
                {
                    List<OrderItemsVM> orderItemslist = new List<OrderItemsVM>();
                    //遍历订单中的商品，获取每个商品对应的颜色、尺寸属性
                    foreach (OrderItemsVM model in orderItemsVMList)
                    {
                        if (model.CommodityOrderId == v.CommodityOrderId)
                        {
                            orderItemslist.Add(model);
                        }
                    }
                    v.OrderItems = orderItemslist;
                }

                #endregion


                if (result != null && result.Count > 0)
                {
                    CommodityOrderVMDTO.List = result;
                    CommodityOrderVMDTO.Total = query.Count();

                }
                else
                {

                    CommodityOrderVMDTO.Total = 0;
                }

                return CommodityOrderVMDTO;

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("获取众筹列表服务异常。crowdfundingId：{0}，userId：{1}，pageIndex：{2}，pageSize：{3}", crowdfundingId, userId, pageIndex, pageSize), ex); 
                return null;
            }


            return CommodityOrderVMDTO;

        }


        /// <summary>
        /// 根据appId找appName
        /// </summary>
        /// <returns></returns>
        public AppNameDTO GetAppNameByAppIdExt(Guid appId)
        {
            AppNameDTO appNameDto = new AppNameDTO();
            List<Guid> applist = new List<Guid>();
            applist.Add(appId);

            Dictionary<Guid, string> list = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(applist);

            if (list != null && list.Count > 0&&list.ContainsKey(appId))
            {
                appNameDto.AppName = list[appId];
            }
            else
            {
                appNameDto = null;
            }

            return appNameDto;

        }
    }
}
