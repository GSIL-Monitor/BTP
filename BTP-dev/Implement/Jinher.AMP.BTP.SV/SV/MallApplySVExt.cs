
/***************
功能描述: BTPSV
作    者: 
创建时间: 2017/8/19 13:49:48
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.LBP.Deploy.Enum;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{
    public partial class MallApplySV : BaseSv, IMallApply
    {
        /// <summary>
        /// 获取易捷北京商家信息
        /// </summary>
        /// <param name="appIds">商家Id列表</param>
        /// <returns>商家信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO> GetYJSellerInfoesExt(System.Collections.Generic.List<System.Guid> appIds)
        {
            if (appIds.Count == 0)
            {
                return new System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO>();
            }
            var ids = appIds.Distinct();
            var mallApplies = MallApply.ObjectSet().Where(m => m.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && ids.Contains(m.AppId)).ToList()
                .Select(m => new Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO { AppId = m.AppId, AppName = m.AppName, EsAppId = m.EsAppId, EsAppName = m.EsAppName, Type = m.GetTypeString() }).ToList();
            return mallApplies;
        }

        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoListExt(Guid appId)
        {
            var mallapplylist = MallApply.ObjectSet().ToList();
            if (!string.IsNullOrWhiteSpace(appId.ToString()) && (!appId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                mallapplylist = mallapplylist.Where(p => p.EsAppId == appId).ToList();
            }
            var baseCommissionlist = BaseCommission.ObjectSet().ToList();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> searchlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO>();
            if (mallapplylist.Count() > 0)
            {
                foreach (var item in mallapplylist)
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO();
                    model.Id = item.Id;
                    model.SubTime = item.SubTime;
                    model.ModifiedOn = item.ModifiedOn;
                    model.AppId = item.AppId;
                    model.EsAppId = item.EsAppId;
                    model.AppName = item.AppName;
                    model.EsAppName = item.EsAppName;
                    model.ApplyContent = item.ApplyContent;
                    int State = Convert.ToInt32(item.State.Value);
                    #region 获取基础佣金比例
                    var baseCommission = baseCommissionlist.Where(p => p.MallApplyId == item.Id).OrderByDescending(p => p.SubTime).FirstOrDefault();
                    if (baseCommission != null)
                    {
                        model.Commission = baseCommission.Commission;
                    }
                    #endregion
                    model.State = State;
                    model.StateShow = (State == 0
                         ? new EnumHelper().GetDescription(MallApplyEnum.RZSQ)
                            : State == 1
                                ? new EnumHelper().GetDescription(MallApplyEnum.QXRZ)
                                  : State == 2
                                     ? new EnumHelper().GetDescription(MallApplyEnum.TG)
                                       : State == 3
                                         ? new EnumHelper().GetDescription(MallApplyEnum.BTG)
                                           : State == 4
                                             ? new EnumHelper().GetDescription(MallApplyEnum.GQ)
                                               : State == 5
                                                  ? new EnumHelper().GetDescription(MallApplyEnum.QXRZQR) : "");
                    model.Type = item.Type;
                    searchlist.Add(model);
                }
                //当State的状态为5时不显示
                searchlist = searchlist.Where(p => p.State < 5).ToList();
            }
            return searchlist;
        }

        /// <summary>
        /// 获取商品列表 轮播图片 直播列表      
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV3Ext(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListSearchDTO search)
        {
            //商品列表参数
            search.FieldSort = 0;
            search.IsHasStock = false;
            search.OrderState = 1;
            search.areaCode = "";
            //店铺直播参数
            Jinher.AMP.ZPH.Deploy.CustomDTO.QueryLiveActivityParam param = new ZPH.Deploy.CustomDTO.QueryLiveActivityParam()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                LiveAppId = search.AppId ?? new Guid("00000000-0000-0000-0000-000000000000"),
                PageOpt = 0,
                pageSize = 20
            };
            //轮播图参数
            Jinher.AMP.LBP.Deploy.CustomDTO.LBListGetDTO arg = new LBP.Deploy.CustomDTO.LBListGetDTO()
            {
                AppId = search.AppId ?? new Guid("00000000-0000-0000-0000-000000000000"),
                ClientType = ClientTypeEnum.IPhone,
                Count = 5,
                IsAnonymousUser = false,
                PosBizId = new Guid("00000000-0000-0000-0000-000000000000"),
                PosType = 0,
                UserId = search.UserId
            };
            CommoditySV Com = new CommoditySV();
            ComdtyListResultCDTO HomePageInfo = new ComdtyListResultCDTO();
            if (search.AppId != Guid.Empty && search.CouponTemplateId != Guid.Empty && search.UserId != Guid.Empty)
            {
                HomePageInfo = Com.GetCommodityByCouponId_New(search);//获取首页商品列表信息
            }
            else
            {
                HomePageInfo = Com.GetCommodityListV2Ext(search);//获取首页商品列表信息
            }
            if (search.PageIndex <= 1)
            {
                var ActivityResult = ZPHSV.GetLiveActivityList(param);//获取店铺直播列表数据
                if (ActivityResult.isSuccess)
                {
                    HomePageInfo.LiveActivity = ActivityResult.Data.FirstOrDefault();
                }
                var LBlistResult = LBPSV.GetLBList(arg);//获取店铺轮播图片
                if (LBlistResult.IsSuccess)
                {
                    HomePageInfo.LBList = LBlistResult.Data;
                }
            }
            return HomePageInfo;
        }

        /// <summary>
        /// 获取App入驻类型
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public List<MallTypeDTO> GetMallTypeListByEsAppIdExt(Guid esAppId)
        {
            try
            {
                using (StopwatchLogHelper.BeginScope("MallApplySV.GetMallTypeListByEsAppId"))
                {
                    return CacheHelper.MallApply.GetMallTypeListByEsAppId(esAppId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("MallApplySV.GetMallTypeListByEsAppId 获取App入驻类型异常。", ex);
                return null;
            }
        }

        

    }
}