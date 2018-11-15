/***************
功能描述: BTPBP
作    者:  LSH
创建时间: 2017/9/21 11:00:04
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
    /// 供用商管理
    /// </summary>
    public partial class SupplierBP : BaseBP, ISupplier
    {
        /// <summary>
        /// 获取供用商数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<SupplierListDTO>> GetSuppliersExt(SupplierSearchDTO searchDto)
        {
            var supplierQuery = SupplierMain.ObjectSet().Where(_ => /*_.SubId == searchDto.UserId &&*/ _.EsAppId == searchDto.EsAppId && !_.IsDel);
            if (!string.IsNullOrWhiteSpace(searchDto.Name))
            {
                supplierQuery = supplierQuery.Where(_ => _.SupplierName.Contains(searchDto.Name));
            }
            if (!string.IsNullOrWhiteSpace(searchDto.Code))
            {
                supplierQuery = supplierQuery.Where(_ => _.SupplierCode.Contains(searchDto.Code));
            }
            if (searchDto.Type.HasValue)
            {
                supplierQuery = supplierQuery.Where(_ => _.SupplierType == searchDto.Type);
            }
            var count = supplierQuery.Count();
            var suppliers = supplierQuery.OrderByDescending(q => q.SubTime).Skip((searchDto.PageIndex - 1) * searchDto.PageSize).Take(searchDto.PageSize).Select(_ => new SupplierListDTO
            {
                Id = _.Id,
                _AppIds = _.AppIds,
                AppNames = _.AppNames,
                //AppIds = _.AppIds.Split(','),
                Name = _.SupplierName,
                Code = _.SupplierCode,
                Type = _.SupplierType,
                ShipperType = _.ShipperType
            }).ToList();
            suppliers.ForEach(_ => _.AppIds = _._AppIds.ToLower().Split(','));
            return new ResultDTO<ListResult<SupplierListDTO>>
            {
                isSuccess = true,
                Data = new ListResult<SupplierListDTO> { List = suppliers, Count = count }
            };
        }

        /// <summary>
        /// 添加供用商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSupplierExt(SupplierUpdateDTO dto)
        {
            if (dto.AppIds == null || dto.AppIds.Count == 0)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var supplierMain = SupplierMain.CreateSupplierMain();
            Dictionary<Guid, string> appNames = new Dictionary<Guid, string>();
            foreach (var appId in dto.AppIds)
            {
                var supplier = Supplier.CreateSupplier();
                supplier.EsAppId = dto.EsAppId;
                supplier.AppId = appId;
                var appName = Jinher.AMP.BTP.TPS.APPSV.GetAppName(appId);
                if (appName=="")
                {
                    appName=MallApply.ObjectSet().Where(_ => _.EsAppId == supplier.EsAppId && _.AppId == supplier.AppId).Select(_=>_.AppName).FirstOrDefault();
                }
                appNames.Add(appId, appName);
                supplier.AppName = appName;
                supplier.SubId = dto.SubId;
                supplier.SupplierName = dto.SupplierName;
                supplier.SupplierCode = dto.SupplierCode;
                supplier.SupplierType = dto.SupplierType;
                supplier.ShipperType = dto.ShipperType;
                supplier.IsDel = false;
                supplier.SupplierMainId = supplierMain.Id;
                contextSession.SaveObject(supplier);

                // 同步MallApply表
                var mallApply = MallApply.ObjectSet().Where(_ => _.EsAppId == supplier.EsAppId && _.AppId == supplier.AppId /*&& _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG*/).FirstOrDefault();
                if (mallApply != null)
                {
                    if (mallApply.AppName != supplier.AppName || mallApply.Type != supplier.SupplierType)
                    {
                        mallApply.Type = supplier.SupplierType;
                        mallApply.AppName = supplier.AppName;
                        mallApply.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mallApply);
                    }
                }

            }
            supplierMain.EsAppId = dto.EsAppId;
            supplierMain.AppIds = string.Join(",", dto.AppIds);
            supplierMain.AppNames = string.Join(",", appNames.Values);
            supplierMain.SubId = dto.SubId;
            supplierMain.SupplierName = dto.SupplierName;
            supplierMain.SupplierCode = dto.SupplierCode;
            supplierMain.SupplierType = dto.SupplierType;
            supplierMain.ShipperType = dto.ShipperType;
            supplierMain.IsDel = false;
            contextSession.SaveObject(supplierMain);

            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("Supplier.AddSupplierExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };

        }

        /// <summary>
        /// 修改供用商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSupplierExt(SupplierUpdateDTO dto)
        {
            if (dto.Id == Guid.Empty)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            if (dto.AppIds == null || dto.AppIds.Count == 0)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            var supplierMain = SupplierMain.FindByID(dto.Id);
            if (supplierMain == null)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "供用商不存在" };
            }
            if (supplierMain.IsDel)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "供用商已删除" };
            }
            //if (supplierMain.SubId != dto.SubId)
            //{
            //    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "无权操作" };
            //}

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var delSupplicers = Supplier.ObjectSet().Where(_ => _.SupplierMainId == supplierMain.Id).ToList();
            foreach (var item in delSupplicers)
            {
                item.EntityState = System.Data.EntityState.Deleted;
                contextSession.SaveObject(item);
            }
            Dictionary<Guid, string> appNames = new Dictionary<Guid, string>();
            foreach (var appId in dto.AppIds)
            {
                var supplier = Supplier.CreateSupplier();
                supplier.EsAppId = dto.EsAppId;
                supplier.AppId = appId;
                var appName = Jinher.AMP.BTP.TPS.APPSV.GetAppName(appId);
                if (appName == "")
                {
                    appName = MallApply.ObjectSet().Where(_ => _.EsAppId == supplier.EsAppId && _.AppId == supplier.AppId).Select(_ => _.AppName).FirstOrDefault();
                }
                appNames.Add(appId, appName);
                supplier.AppName = appName;
                supplier.SubId = dto.SubId;
                supplier.SupplierName = dto.SupplierName;
                supplier.SupplierCode = dto.SupplierCode;
                supplier.SupplierType = dto.SupplierType;
                supplier.ShipperType = dto.ShipperType;
                supplier.IsDel = false;
                supplier.SupplierMainId = supplierMain.Id;
                contextSession.SaveObject(supplier);

                // 同步MallApply表
                var mallApply = MallApply.ObjectSet().Where(_ => _.EsAppId == supplier.EsAppId && _.AppId == supplier.AppId /*&& _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG*/).FirstOrDefault();
                if (mallApply != null)
                {
                    if (mallApply.AppName != supplier.AppName || mallApply.Type != supplier.SupplierType)
                    {
                        mallApply.Type = supplier.SupplierType;
                        mallApply.AppName = supplier.AppName;
                        mallApply.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mallApply);
                    }
                }
            }

            //supplierMain.EsAppId = dto.EsAppId;
            supplierMain.AppIds = string.Join(",", dto.AppIds);
            supplierMain.AppNames = string.Join(",", appNames.Values);
            supplierMain.SupplierName = dto.SupplierName;
            supplierMain.SupplierCode = dto.SupplierCode;
            supplierMain.SupplierType = dto.SupplierType;
            supplierMain.ShipperType = dto.ShipperType;
            //supplier.ModifiedOn = DateTime.Now;
            supplierMain.EntityState = System.Data.EntityState.Modified;
            contextSession.SaveObject(supplierMain);

            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("Supplier.UpdateSupplierExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 删除供用商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSupplierExt(System.Guid id)
        {
            if (id == Guid.Empty)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var supplierMain = SupplierMain.FindByID(id);
            if (supplierMain != null)
            {
                supplierMain.IsDel = true;

                var delSupplicers = Supplier.ObjectSet().Where(_ => _.SupplierMainId == supplierMain.Id).ToList();
                foreach (var item in delSupplicers)
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.SaveObject(item);
                }
            }
            contextSession.SaveObject(supplierMain);
            try
            {
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("Supplier.DeleteSupplierExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 检查供应商编码
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSupplerCodeExt(string code)
        {
            if (Supplier.ObjectSet().Any(_ => !_.IsDel && _.SupplierCode == code))
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "供应商编码不能重复" };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }
        /// <summary>
        /// 获取对应商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SupplierListDTO> GetSupplierAppsExt(System.Guid esAppId)
        {
            return Supplier.ObjectSet().Where(p => p.EsAppId == esAppId).Select(p => new SupplierListDTO
            {
                Id = p.AppId,
                AppNames = p.AppName
            }).ToList();
        }

    }
}
