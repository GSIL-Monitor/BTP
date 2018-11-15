
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/14 16:33:42
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 拼团
    /// </summary>
    public partial class DiyGroupBP : BaseBP, IDiyGroup
    {
        /// <summary>
        /// 获取拼团信息（必传参数AppId、PageIndex、PageSize、State，可选参数ComNameSub）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> GetDiyGroupsExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> result = new ResultDTO<DiyGroupManageDTO>();
            if (search == null)
            {
                result.Message = "参数错误，appId不能为空！";
                result.ResultCode = 1;
                return result;
            }
            var resultData = new Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO();

            var diyQuery = (from diy in DiyGroup.ObjectSet()
                            join c in Commodity.ObjectSet()
                            on diy.CommodityId equals c.Id
                            join ps in PromotionItems.ObjectSet() on c.Id equals ps.CommodityId
                            join p in Promotion.ObjectSet() on ps.PromotionId equals p.Id
                            where c.AppId == search.AppId && diy.State != 0 && diy.JoinNumber != 0
                            && diy.PromotionId == p.Id
                            select new DiyGroupManageVM
                                {
                                    DiyGroupNumber = diy.Code,
                                    DiyGroupName = c.Name,
                                    DiyGroupPrice = ps.DiscountPrice,
                                    DiyGroupCount = p.GroupMinVolume,
                                    DiyId = diy.Id,
                                    DiyGroupSubTime = diy.SubTime,
                                    DiyGroupState = diy.State
                                });
            if (!String.IsNullOrEmpty(search.ComNameSub))
            {
                diyQuery = diyQuery.Where(c => c.DiyGroupName.Contains(search.ComNameSub));
            }
            if (!String.IsNullOrEmpty(search.State))
            {
                if (search.State.Contains(","))
                {
                    int[] arrystate = Array.ConvertAll<string, int>(search.State.Split(','), s => int.Parse(s)).ToArray();
                    diyQuery = diyQuery.Where(c => arrystate.Contains(c.DiyGroupState));
                }
                else
                {
                    diyQuery = diyQuery.Where(c => c.DiyGroupState == 1);
                }
            }
            else if (search.State == "")
            {

            }

            diyQuery = diyQuery.Distinct();
            resultData.Count = diyQuery.Count();

            var searchResult = diyQuery.OrderByDescending(c => c.DiyGroupNumber).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
            var diygroupIdList = searchResult.Select(c => c.DiyId).ToList();
            //构建拼团订单id数组
            List<Guid> diyOrderIdList = new List<Guid>();
            for (int i = 0; i < searchResult.Count; i++)
            {
                diyOrderIdList.Add(searchResult[i].DiyId);
            }
            List<DiyGroupManageMM> diyGroupManageMmList = (from diyo in DiyGroupOrder.ObjectSet()
                                                           join co in CommodityOrder.ObjectSet() on diyo.OrderId equals co.Id
                                                           where diyo.AppId == search.AppId && diygroupIdList.Contains(diyo.DiyGroupId) && diyo.State == 1
                                                           select new DiyGroupManageMM
                                                           {
                                                               DiyGroupOrderCode = diyo.OrderCode,
                                                               DiyGroupPersonCode = diyo.SubCode,
                                                               DiyGroupPersonRole = diyo.Role,
                                                               DiyGroupOrderId = diyo.OrderId,
                                                               DiyGroupId = diyo.DiyGroupId,
                                                               DiyGroupPrice = (decimal)co.RealPrice
                                                           }
                        ).OrderBy(c => c.DiyGroupOrderCode).ToList();

            foreach (DiyGroupManageVM vm in searchResult)
            {
                List<DiyGroupManageMM> diyorderItemslist = new List<DiyGroupManageMM>();
                foreach (DiyGroupManageMM model in diyGroupManageMmList)
                {
                    if (model.DiyGroupId == vm.DiyId)
                    {
                        diyorderItemslist.Add(model);
                    }
                }
                vm.OrderDataList = diyorderItemslist;
            }
            resultData.Data = searchResult;
            result.Data = resultData;
            return result;

        }
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmDiyGroupExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var query = (from diyGroup in DiyGroup.ObjectSet()
                             join diyGroupOrder in DiyGroupOrder.ObjectSet() on diyGroup.Id equals diyGroupOrder.DiyGroupId
                             join order in CommodityOrder.ObjectSet() on diyGroupOrder.OrderId equals order.Id
                             where diyGroup.Id == search.DiyGoupId && diyGroupOrder.State == 1
                             select order
                ).ToList();
                if (query.Count > 0)
                {
                    foreach (var diyorder in query)
                    {
                        diyorder.State = 1;
                        diyorder.ModifiedOn = DateTime.Now;
                        diyorder.EntityState = System.Data.EntityState.Modified;
                        Jinher.AMP.BTP.BE.BELogic.AddMessage addmassage = new Jinher.AMP.BTP.BE.BELogic.AddMessage();
                        addmassage.AddMessages(diyorder.Id.ToString(), diyorder.UserId.ToString(), diyorder.AppId, diyorder.Code, diyorder.State, "", "order");

                        // 触发订单成功事件
                        OrderEventHelper.OnOrderPaySuccess(diyorder);
                    }
                }
                var diyquery = DiyGroup.ObjectSet().FirstOrDefault(n => n.Id == search.DiyGoupId && n.AppId == search.AppId);
                if (diyquery != null)
                {
                    diyquery.State = 3;
                    diyquery.ModifiedOn = DateTime.Now;
                    diyquery.EntityState = EntityState.Modified;
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("异常。search：{0}", search), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 退款(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefundExt(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var query = DiyGroup.ObjectSet().FirstOrDefault(n => n.Id == search.DiyGoupId && n.AppId == search.AppId);
                if (query != null)
                {
                    query.State = 5;
                    query.ModifiedOn = DateTime.Now;
                    query.EntityState = EntityState.Modified;
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("退款异常。search：{0}", search), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<UnfinishedDiyGroupOutputDTO>> UnfinishedDiyGrouplistExt(UnfinishedDiyGroupInputDTO inputDTO)
        {
            var appId = inputDTO.AppId;
            var outsidePromotionId = inputDTO.OutsidePromoId;
            var maxTakeCount = 10;

            var appinfos = ZPHSV.Instance.GetAppIdlist(new List<Guid>() {appId});
            var appids = appinfos.Select(t => t.AppId).ToList();
            appids.Add(appId);

            try
            {
                LogHelper.Debug(string.Format("查询到的未完成的拼团列表. Input:{0}", JsonHelper.JsonSerializer(inputDTO)));

                //计算未完成的拼团列表
                var query = from g in DiyGroup.ObjectSet()
                            join p in Promotion.ObjectSet() on g.PromotionId equals p.Id
                            join go in
                                (from o in DiyGroupOrder.ObjectSet()
                                 where o.State == 1
                                 group o by o.DiyGroupId into og
                                 select new
                                 {
                                     Key = og.Key,
                                     MemberCount = og.Count()
                                 }) on g.Id equals go.Key
                            where
                                p.PromotionType == 3
                                && g.State == 1
                                && g.ExpireTime >= DateTime.Now
                                && appids.Contains(g.AppId)
                                && p.OutsideId == outsidePromotionId
                            select
                               new
                               {
                                   GroupId = g.Id,
                                   OwnerId = g.SubId,
                                   LackMember = (p.GroupMinVolume ?? 0) - go.MemberCount,
                                   ExpireTime = g.ExpireTime
                               };

                var list = query
                                .OrderByDescending(selector => selector.LackMember)
                                .ThenBy(selector => selector.ExpireTime)
                                .Take(maxTakeCount)
                                .ToList();

                //获取团长信息
                var cbcFacade = new TPS.CBCSVFacade();
                var userNameFunc = new Func<Guid, string>(userId =>
                {
                    var user = cbcFacade.GetUserNameIconDTO(userId);
                    if (user != null)
                    {
                        return user.Name;
                    }
                    return string.Empty;
                });

                var userIconFunc = new Func<Guid, string>(userId =>
                {
                    var user = cbcFacade.GetUserNameIconDTO(userId);
                    if (user != null)
                    {
                        return user.HeadIcon;
                    }
                    return string.Empty;
                });

                var data = list.Select(selector => new UnfinishedDiyGroupOutputDTO()
                  {
                      GroupId = selector.GroupId,
                      LackMember = selector.LackMember,
                      ExpireTime = selector.ExpireTime,
                      OwnerIcon = userIconFunc(selector.OwnerId),
                      OwnerName = userNameFunc(selector.OwnerId)
                  }).ToList();

                var result = new ResultDTO<List<UnfinishedDiyGroupOutputDTO>>()
                {
                    isSuccess = true,
                    ResultCode = 0,
                    Data = data
                };

                LogHelper.Debug(string.Format("查询到的未完成的拼团列表. Output:{0}", JsonHelper.JsonSerializer(result)));

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询未完成的拼团列表发生异常. {0}", ex.Message), ex);
                return new ResultDTO<List<UnfinishedDiyGroupOutputDTO>>
                {
                    Data = new List<UnfinishedDiyGroupOutputDTO>()
                };
            }
        }
    }
}
