
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2018/6/13 13:49:52
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 自动调价设置
    /// </summary>
    public partial class CommodityPriceFloatBP : BaseBP, ICommodityPriceFloat
    {
        /// <summary>
        /// 获取自动调价设置数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPriceFloatList<CommodityPriceFloatListDto>> GetDataListExt(System.Guid appId)
        {
            try
            {
                var query = CommodityPriceFloat.ObjectSet().Where(_ => !_.IsDel && _.EsAppId == appId);
                var count = query.Count();
                var data = query.OrderByDescending(q => q.SubTime).Select(_ => new CommodityPriceFloatListDto
                {
                    Id = _.Id,
                    EsAppId = _.EsAppId,
                    AppIds = _.AppIds,
                    FloatPrice = _.FloatPrice
                }).ToList();

                var malls = MallApply.GetTGQuery(appId).Select(_ => new { _.AppId, _.AppName }).ToList();
                data.ForEach(_ =>
                {
                    if (!string.IsNullOrEmpty(_.AppIds))
                    {
                        List<string> appNames = new List<string>();
                        foreach (var dAppId in _.AppIds.Split(','))
                        {
                            var mall = malls.Find(m => m.AppId == new Guid(dAppId));
                            if (mall != null)
                            {
                                appNames.Add(mall.AppName);
                            }
                        }
                        _.AppNames = string.Join("、", appNames);
                    }
                    else
                    {
                        _.AppNames = "未指定店铺";
                    }
                });
                return new ResultDTO<CommodityPriceFloatList<CommodityPriceFloatListDto>>
                {
                    isSuccess = true,
                    Data = new CommodityPriceFloatList<CommodityPriceFloatListDto> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityPriceFloatBP.GetDataListExt 异常", ex);
                return new ResultDTO<CommodityPriceFloatList<CommodityPriceFloatListDto>>
                {
                    isSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 添加自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddExt(Jinher.AMP.BTP.Deploy.CommodityPriceFloatDTO dto)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var entity = CommodityPriceFloat.CreateCommodityPriceFloat();
                entity.EsAppId = dto.EsAppId;
                entity.AppIds = dto.AppIds;
                entity.FloatPrice = dto.FloatPrice;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityPriceFloatBP.AddExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 修改自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateExt(Jinher.AMP.BTP.Deploy.CommodityPriceFloatDTO dto)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var entity = CommodityPriceFloat.FindByID(dto.Id);
                entity.EsAppId = dto.EsAppId;
                entity.AppIds = dto.AppIds;
                entity.FloatPrice = dto.FloatPrice;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityPriceFloatBP.UpdateExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 删除自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteExt(System.Guid id)
        {
            if (id == Guid.Empty)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var supplierMain = CommodityPriceFloat.FindByID(id);
            if (supplierMain != null)
            {
                supplierMain.IsDel = true;
                contextSession.SaveObject(supplierMain);
            }
            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityPriceFloatBP.DeleteExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public List<Guid> GetAppsExt(Guid appId)
        {
            try
            {
                var appIds = CommodityPriceFloat.ObjectSet().Where(p => !p.IsDel && p.EsAppId == appId && !string.IsNullOrEmpty(p.AppIds)).Select(_ => _.AppIds).ToList();
                return appIds.SelectMany(_ => _.Split(',')).Distinct().Select(_ => new Guid(_)).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityPriceFloatBP.GetAppsExt 异常", ex);
                return new List<Guid>();
            }
        }
    }
}