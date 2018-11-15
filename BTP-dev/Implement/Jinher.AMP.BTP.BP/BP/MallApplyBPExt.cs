
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:09
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using NPOI.SS.Formula.Functions;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Common;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class MallApplyBP : BaseBP, IMallApply
    {
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoListExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO search)
        {
            var searchlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO>();
            if (search.EsAppId == Guid.Empty && search.AppId == Guid.Empty)
            {
                return searchlist;
            }
            var mallapplylistQuery = MallApply.ObjectSet().Where(m => m.State.Value < 5);
            if (!string.IsNullOrWhiteSpace(search.AppName))
            {
                mallapplylistQuery = mallapplylistQuery.Where(p => p.AppName.Contains(search.AppName));
            }
            if (search.UserId != Guid.Empty)
            {
                mallapplylistQuery = mallapplylistQuery.Where(p => p.UserId == search.UserId);
            }
            if (!string.IsNullOrWhiteSpace(search.AppId.ToString()) && (!search.AppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                mallapplylistQuery = mallapplylistQuery.Where(p => p.AppId == search.AppId);
            }
            if (!string.IsNullOrWhiteSpace(search.EsAppId.ToString()) && (!search.EsAppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                mallapplylistQuery = mallapplylistQuery.Where(p => p.EsAppId == search.EsAppId);
            }
            if (search.Type.HasValue)
            {
                mallapplylistQuery = mallapplylistQuery.Where(p => p.Type == search.Type);
            }
            var baseCommissionlist = BaseCommission.ObjectSet().ToList();
            var commoditySettleAmountQuery = CommoditySettleAmount.ObjectSet().AsQueryable();
            foreach (var item in mallapplylistQuery.ToList())
            {
                var model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO();
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
                var baseCommission = baseCommissionlist
                    .Where(p => p.MallApplyId == item.Id)
                    .OrderByDescending(p => p.SubTime).FirstOrDefault();
                if (baseCommission != null)
                {
                    model.Commission = baseCommission.Commission;
                }
                #endregion
                model.State = State;
                model.StateShow = new EnumHelper().GetDescription((MallApplyEnum)State);

                //model.StateShow = (State == 0
                //     ? new EnumHelper().GetDescription(MallApplyEnum.RZSQ)
                //        : State == 1
                //            ? new EnumHelper().GetDescription(MallApplyEnum.QXRZ)
                //              : State == 2
                //                 ? new EnumHelper().GetDescription(MallApplyEnum.TG)
                //                   : State == 3
                //                     ? new EnumHelper().GetDescription(MallApplyEnum.BTG)
                //                       : State == 4
                //                         ? new EnumHelper().GetDescription(MallApplyEnum.GQ)
                //                           : State == 5
                //                              ? new EnumHelper().GetDescription(MallApplyEnum.QXRZQR) : "");

                model.Type = item.Type;
                model.TypeString = item.GetTypeString();
                model.IsAllSetSettlePrice = true;
                if (model.Type == 0)
                {
                    // 查询是否有商品未设置结算价
                    if (Commodity.ObjectSet().Where(a => a.AppId == item.AppId && !a.IsDel).Count()
                        > commoditySettleAmountQuery.Where(b => b.AppId == item.AppId).GroupBy(c => c.CommodityId).Select(g => g.Key).Count())
                    {
                        model.IsAllSetSettlePrice = false;
                    }
                }
                searchlist.Add(model);
            }
            return searchlist;
        }


        /// <summary>
        /// 保存商城信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveMallApplyExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            MallApply entity = new MallApply();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                entity.Id = model.Id;
                entity.SubTime = model.SubTime;
                entity.ModifiedOn = model.ModifiedOn;
                entity.AppId = model.AppId;
                entity.CrcAppId =Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                entity.AppName = model.AppName;
                entity.EsAppId = model.EsAppId;
                entity.EsAppName = model.EsAppName;
                entity.UserId = model.UserId;
                entity.ApplyContent = model.ApplyContent;
                entity.State = new ApplyStateVO { Value = model.State };
                entity.Type = model.Type ?? 1;
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();

                // 同步Supplier表
                var supplier = Supplier.ObjectSet().Where(_ => _.EsAppId == entity.EsAppId && _.AppId == entity.AppId && !_.IsDel).FirstOrDefault();
                if (supplier != null)
                {
                    if (supplier.SupplierType != entity.Type)
                    {
                        supplier.SupplierType = entity.Type;
                        supplier.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(supplier);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商家信息保存异常。MallApply：{0}", ex.Message));
                return new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return new ResultDTO { ResultCode = 0, Message = "提交成功", isSuccess = true };

        }


        /// <summary>
        /// 修改商城信息状态
        /// </summary>
        /// <returns></returns>
        public ResultDTO UpdateMallApplyExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var mallapply = MallApply.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (mallapply != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.AppId.ToString()) && (!model.AppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        mallapply.AppId = model.AppId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.EsAppId.ToString()) && (!model.EsAppId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        mallapply.EsAppId = model.EsAppId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.UserId.ToString()) && (!model.UserId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        mallapply.UserId = model.UserId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.AppName))
                    {
                        mallapply.AppName = model.AppName;
                    }
                    if (!string.IsNullOrWhiteSpace(model.EsAppName))
                    {
                        mallapply.EsAppName = model.EsAppName;
                    }
                    mallapply.ModifiedOn = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(model.State.ToString()))
                    {
                        mallapply.State = new ApplyStateVO { Value = model.State };
                    }
                    if (!string.IsNullOrWhiteSpace(model.ApplyContent))
                    {
                        mallapply.ApplyContent = model.ApplyContent;
                    }
                    if (Convert.ToBoolean(model.CrcAppId))
                    {
                        mallapply.CrcAppId = model.CrcAppId;
                    }
                    if (model.Type.HasValue)
                    {
                        mallapply.Type = model.Type.Value;
                    }
                    mallapply.EntityState = EntityState.Modified;

                    

                    // 同步Supplier表
                    if (model.Type.HasValue)
                    {
                        var supplier = Supplier.ObjectSet().Where(_ => _.EsAppId == mallapply.EsAppId && _.AppId == mallapply.AppId && !_.IsDel).FirstOrDefault();
                        if (supplier != null)
                        {
                            if (supplier.SupplierType != mallapply.Type)
                            {
                                supplier.SupplierType = mallapply.Type;
                                supplier.EntityState = System.Data.EntityState.Modified;
                                contextSession.SaveObject(supplier);
                            }
                        }
                    }

                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "该信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商家信息保存异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据id获取商城实体信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO GetMallApplyExt(Guid id)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO();
            var mallApply = MallApply.ObjectSet().FirstOrDefault(p => p.Id == id);
            if (mallApply != null)
            {
                model.Id = mallApply.Id;
                model.SubTime = mallApply.SubTime;
                model.ModifiedOn = mallApply.ModifiedOn;
                model.AppId = mallApply.AppId;
                model.EsAppId = mallApply.EsAppId;
                model.AppName = mallApply.AppName;
                model.EsAppName = mallApply.EsAppName;
                model.ApplyContent = mallApply.ApplyContent;
                int State = Convert.ToInt32(mallApply.State.Value);
                #region 获取基础佣金比例
                var baseCommission = BaseCommission.ObjectSet().FirstOrDefault(p => p.MallApplyId == mallApply.Id);
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
                model.Type = mallApply.Type;
                model.TypeString = mallApply.GetTypeString();
            }
            return model;
        }

        /// <summary>
        /// 验证是否存在入驻商家
        /// </summary>
        /// <returns></returns>
        public ResultDTO IsHaveMallApplyExt(Guid esAppId, Guid appId)
        {
            ResultDTO resultDto = new ResultDTO() { isSuccess = false };
            try
            {
                var mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == esAppId && t.AppId == appId);
                if (mallApply != null)
                {
                    resultDto.isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取入驻商家信息异常。MallApply.IsHaveMallApplyExt：{0}", ex.Message));
            }
            return resultDto;
        }


        /// <summary>
        /// 获取商城下入住的APP
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<AppInfoDTO> GetMallAppsExt(Guid esAppId)
        {
            return MallApply.ObjectSet()
                .Where(_ => _.EsAppId == esAppId && _.State.Value == (int)MallApplyEnum.TG)
                .Select(_ => new AppInfoDTO { Id = _.AppId, Name = _.AppName })
                .ToList();
        }

        /// <summary>
        /// 给盈科同步指定商城数据
        /// </summary>
        /// <returns></returns>
        public void GetMallAppsForJobExt(Guid esAppId)
        {
            LogHelper.Info(string.Format("MallApplyBP.GetMallAppsForJobExt入参:esAppId={0}", esAppId));
            if (esAppId == Guid.Empty) return;
            Task.Factory.StartNew(() =>
            {
                MallApply.ObjectSet().Where(_ => _.EsAppId == esAppId)
               .ToList().ForEach(Item =>
               {
                   try
                   {
                       var mqjson = JsonConvert.SerializeObject(Item.ToEntityData());
                       if (TPS.YJBJMQSV.SendToMq("bj_bw_mallapply", JsonConvert.ToString(mqjson)))
                       {
                           LogHelper.Info("MallApplyBP.GetMallAppsForJobExt成功!");
                       }
                       else
                       {
                           LogHelper.Info("MallApplyBP.GetMallAppsForJobExt失败!");
                       }
                   }
                   catch(Exception ex)
                   {
                       LogHelper.Info(string.Format("MallApplyBP.GetMallAppsForJobExt失败:{0}", ex));
                       LogHelper.Info(string.Format("MallApplyBP.GetMallAppsForJobExt失败:{0}", ex.Message));
                   }
               });
            });            
        }

    }
}