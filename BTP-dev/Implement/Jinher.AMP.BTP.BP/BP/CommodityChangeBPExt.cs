
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/1/24 11:24:42
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using System.Data;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityChangeBP : BaseBP, ICommodityChange
    {

        /// <summary>
        /// 获取商品变更表信息列表
        /// </summary>
        /// <param name="Search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> GetCommodityChangeListExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search, int pageIndex, int pageSize)
        {
            LogHelper.Debug("CommodityChangeBP.GetCommodityChangeListExt,入参：" + JsonHelper.JsonSerializer(Search));
            try
            {
                var query = CommodityChange.ObjectSet().Where(p => Search.Appids.Contains(p.AppId)).AsQueryable();
                #region  查询条件
                //根据供应商
                if (Search.AppidsList.Count > 0)
                {
                    query = query.Where(p => Search.AppidsList.Contains(p.AppId));
                }
                //商铺名称
                if (Search.AppId != Guid.Empty)
                {
                    query = query.Where(p => p.AppId == Search.AppId);
                }
                //商品名称
                if (!string.IsNullOrEmpty(Search.Name))
                {
                    query = query.Where(p => Search.Name.Contains(p.Name) || p.Name.Contains(Search.Name));
                }
                //备注编码
                if (!string.IsNullOrEmpty(Search.JDCode))
                {
                    query = query.Where(p => p.JDCode == Search.JDCode);
                }
                //商品条形码
                if (!string.IsNullOrEmpty(Search.Barcode))
                {
                    query = query.Where(p => p.Barcode == Search.Barcode);
                }
                //发布人
                if (Search.SubId != Guid.Empty)
                {
                    query = query.Where(p => p.SubId == Search.SubId);
                }
                if (!string.IsNullOrEmpty(Search.SubStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.SubStarTime);
                    query = query.Where(p => p.SubOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.SubEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.SubEndTime).AddDays(1);
                    query = query.Where(p => p.SubOn <= EndTime);
                }
                if (Search.ModifiedId != Guid.Empty)
                {
                    query = query.Where(p => p.ModifiedId == Search.ModifiedId);
                }
                if (!string.IsNullOrEmpty(Search.ModStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.ModStarTime);
                    query = query.Where(p => p.ModifiedOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.ModEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.ModEndTime).AddDays(1);
                    query = query.Where(p => p.ModifiedOn <= EndTime);
                }
                if (Search.State != -1)
                {
                    if (Search.State == 3)
                    {   //已删除的数据
                        query = query.Where(p => p.IsDel == true);
                    }
                    else
                    {
                        query = query.Where(p => p.State == Search.State && p.IsDel == false);
                    }
                }
                #endregion
                //string AppName = APPSV.GetAppName(appId);
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> result = (from n in query
                                                                                   select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                   {
                                                                                       Id = n.Id,
                                                                                       CommodityId = n.CommodityId,
                                                                                       AppId = n.AppId,
                                                                                       Barcode = n.Barcode,
                                                                                       Type = n.Type,
                                                                                       YJCouponActivityId = n.YJCouponActivityId,
                                                                                       YJCouponType = n.YJCouponType,
                                                                                       No_Code = n.No_Code,
                                                                                       JDCode = n.JDCode,     //备注编码
                                                                                       Name = n.Name,
                                                                                       MarketPrice = n.MarketPrice,
                                                                                       Price = n.Price,
                                                                                       CostPrice = n.CostPrice,
                                                                                       TaxClassCode = n.TaxClassCode,
                                                                                       InputRax = n.InputRax,
                                                                                       TaxRate = n.TaxRate,
                                                                                       ComAttribute = n.ComAttribute,
                                                                                       Unit = n.Unit,
                                                                                       SubId = n.SubId,
                                                                                       SubOn = n.SubOn,
                                                                                       ModifiedId = n.ModifiedId,
                                                                                       ModifiedOn = n.ModifiedOn,
                                                                                       State = n.State,
                                                                                       IsDel = n.IsDel
                                                                                   }).OrderByDescending(p => p.ModifiedOn).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (result.Count() > 0)
                {
                    //获取商铺名称
                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds); //获取商铺名称

                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName, s.SupplierType }).Distinct().ToList();

                    //获取提交人名称
                    List<Guid> SubId = (from n in result where n.SubId != Guid.Empty select n.SubId).Distinct().ToList();
                    List<Guid> ModId = (from n in result where n.ModifiedId.HasValue select n.ModifiedId.Value).Distinct().ToList();
                    List<Guid> userid = SubId.Union(ModId).Distinct().ToList();
                    var Userinfo = CBCSV.GetUserNameAndCodes(userid);

                    foreach (var item in result)
                    {
                        //获取提交人名称
                        var NameAndCodes = Userinfo[item.SubId];
                        item.SubName = NameAndCodes.Item1;
                        item.SubCode = NameAndCodes.Item2;
                        //获取供应商名称
                        var SupplierName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        if (!string.IsNullOrEmpty(SupplierName))
                        {
                            item.SupplierName = SupplierName;
                        }
                        //获取供应商类型
                        var SupplierType = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierType).FirstOrDefault();
                        if (SupplierType != null)
                        {
                            //（0-自营他配；1-第三方；2-自营自配）
                            if (SupplierType == 0)
                                item.SupplierTypeName = "自营他配";
                            else if (SupplierType == 1)
                                item.SupplierTypeName = "第三方";
                            else if (SupplierType == 2)
                                item.SupplierTypeName = "自营自配";
                        }
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //获取修改人名称
                        if (item.ModifiedId != new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9") && item.ModifiedId != null)
                        {
                            Guid MId = item.ModifiedId ?? new Guid("00000000-0000-0000-0000-000000000000");
                            NameAndCodes = Userinfo[MId];
                            item.ModifiedName = NameAndCodes.Item1;
                            item.ModifiedCode = NameAndCodes.Item2;
                        }
                        else if (item.ModifiedId == new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9"))
                        {
                            item.ModifiedName = "系统自动";
                            item.ModifiedCode = "京东同步";
                        }
                        else
                        {
                            item.ModifiedName = "";
                            item.ModifiedCode = "";
                        }
                    }
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取变动商品表异常。"), ex);
                return null;
            }
        }
        /// <summary>
        /// 导出商品信息变更信息
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GetCommodityChangeExportExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            try
            {

                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> result = GetCommodityChangeList(Search);
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取变动商品表异常。"), ex);
                return null;
            }
        }
        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.totalNum> GetTotalListExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            LogHelper.Debug("CommodityChangeBP.GetTotalListExt,入参：" + JsonHelper.JsonSerializer(Search));
            try
            {
                var query = from m in Commodity.ObjectSet()
                            join n in CommodityStock.ObjectSet() on m.Id equals n.CommodityId into CommodityList
                            from com in CommodityList.DefaultIfEmpty()
                            where Search.Appids.Contains(m.AppId)
                            select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                            {
                                Id = com.Id,
                                CommodityId = com.CommodityId,
                                AppId = m.AppId,
                                Barcode = com.Barcode,
                                No_Code = com.No_Code,
                                JDCode = com.JDCode,     //备注编码
                                Name = m.Name,
                                SubId = m.SubId,
                                SubOn = com.SubTime,
                                ModifiedId = m.ModifieId,
                                ModifiedOn = com.ModifiedOn,
                                State = m.State,
                                IsDel = m.IsDel
                            };
                #region  查询条件
                //根据供应商
                if (Search.AppidsList.Count > 0)
                {
                    query = query.Where(p => Search.AppidsList.Contains(p.AppId));
                }
                //商铺名称
                if (Search.AppId != Guid.Empty)
                {
                    query = query.Where(p => p.AppId == Search.AppId);
                }
                //商品名称
                if (!string.IsNullOrEmpty(Search.Name))
                {
                    query = query.Where(p => Search.Name.Contains(p.Name) || p.Name.Contains(Search.Name));
                }
                //备注编码
                if (!string.IsNullOrEmpty(Search.JDCode))
                {
                    query = query.Where(p => p.JDCode == Search.JDCode);
                }
                //商品条形码
                if (!string.IsNullOrEmpty(Search.Barcode))
                {
                    query = query.Where(p => p.Barcode == Search.Barcode);
                }
                //发布人
                if (Search.SubId != Guid.Empty)
                {
                    query = query.Where(p => p.SubId == Search.SubId);
                }
                if (!string.IsNullOrEmpty(Search.SubStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.SubStarTime);
                    query = query.Where(p => p.SubOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.SubEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.SubEndTime).AddDays(1);
                    query = query.Where(p => p.SubOn <= EndTime);
                }
                if (Search.ModifiedId != Guid.Empty)
                {
                    query = query.Where(p => p.ModifiedId == Search.ModifiedId);
                }
                if (!string.IsNullOrEmpty(Search.ModStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.ModStarTime);
                    query = query.Where(p => p.ModifiedOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.ModEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.ModEndTime).AddDays(1);
                    query = query.Where(p => p.ModifiedOn <= EndTime);
                }
                if (Search.State != -1)
                {
                    if (Search.State == 3)
                    {   //已删除的数据
                        query = query.Where(p => p.IsDel == true);
                    }
                    else
                    {
                        query = query.Where(p => p.State == Search.State && p.IsDel == false);
                    }
                }
                #endregion
                totalNum num = new totalNum();
                num.totalGoods = query.Count();
                num.selling = query.Where(p => p.State == 0 && p.IsDel == false).Count();
                num.soldout = query.Where(p => p.State == 1 && p.IsDel == false).Count();
                num.isdel = query.Where(p => p.IsDel == true).Count();
                List<totalNum> list = new List<totalNum>();
                list.Add(num);
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityChangeBP.GetTotalListExt异常", ex);
            }
            return new List<totalNum>();
        }
        /// <summary>
        /// 获取商品信息变更信息
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GetCommodityChangeList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            try
            {
                var query = CommodityChange.ObjectSet().Where(p => Search.Appids.Contains(p.AppId)).AsQueryable();
                #region  查询条件
                //根据供应商
                if (Search.AppidsList.Count > 0)
                {
                    query = query.Where(p => Search.AppidsList.Contains(p.AppId));
                }
                //商铺名称
                if (Search.AppId != Guid.Empty)
                {
                    query = query.Where(p => p.AppId == Search.AppId);
                }
                //商品名称
                if (!string.IsNullOrEmpty(Search.Name))
                {
                    query = query.Where(p => Search.Name.Contains(p.Name) || p.Name.Contains(Search.Name));
                }
                //备注编码
                if (!string.IsNullOrEmpty(Search.JDCode))
                {
                    query = query.Where(p => p.JDCode == Search.JDCode);
                }
                //商品条形码
                if (!string.IsNullOrEmpty(Search.Barcode))
                {
                    query = query.Where(p => p.Barcode == Search.Barcode);
                }
                //发布人
                if (Search.SubId != Guid.Empty)
                {
                    query = query.Where(p => p.SubId == Search.SubId);
                }
                if (!string.IsNullOrEmpty(Search.SubStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.SubStarTime);
                    query = query.Where(p => p.SubOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.SubEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.SubEndTime).AddDays(1);
                    query = query.Where(p => p.SubOn <= EndTime);
                }
                if (Search.ModifiedId != Guid.Empty)
                {
                    query = query.Where(p => p.ModifiedId == Search.ModifiedId);
                }
                if (!string.IsNullOrEmpty(Search.ModStarTime))
                {
                    var StartTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    StartTime = DateTime.Parse(Search.ModStarTime);
                    query = query.Where(p => p.ModifiedOn >= StartTime);
                }
                if (!string.IsNullOrEmpty(Search.ModEndTime))
                {
                    var EndTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    EndTime = DateTime.Parse(Search.ModEndTime).AddDays(1);
                    query = query.Where(p => p.ModifiedOn <= EndTime);
                }
                if (Search.State != -1)
                {
                    if (Search.State == 3)
                    {   //已删除的数据
                        query = query.Where(p => p.IsDel == true);
                    }
                    else
                    {
                        query = query.Where(p => p.State == Search.State && p.IsDel == false);
                    }
                }
                #endregion
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> result = (from n in query
                                                                                   select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                   {
                                                                                       Id = n.Id,
                                                                                       CommodityId = n.CommodityId,
                                                                                       AppId = n.AppId,
                                                                                       Barcode = n.Barcode,
                                                                                       Type = n.Type,
                                                                                       YJCouponActivityId = n.YJCouponActivityId,
                                                                                       YJCouponType = n.YJCouponType,
                                                                                       No_Code = n.No_Code,
                                                                                       JDCode = n.JDCode,     //备注编码
                                                                                       Name = n.Name,
                                                                                       MarketPrice = n.MarketPrice,
                                                                                       Price = n.Price,
                                                                                       CostPrice = n.CostPrice,
                                                                                       TaxClassCode = n.TaxClassCode,
                                                                                       InputRax = n.InputRax,
                                                                                       TaxRate = n.TaxRate,
                                                                                       ComAttribute = n.ComAttribute,
                                                                                       Unit = n.Unit,
                                                                                       SubId = n.SubId,
                                                                                       SubOn = n.SubOn,
                                                                                       ModifiedId = n.ModifiedId,
                                                                                       ModifiedOn = n.ModifiedOn,
                                                                                       State = n.State,
                                                                                       IsDel = n.IsDel
                                                                                   }).OrderByDescending(p => p.ModifiedOn).ToList();
                if (result.Count() > 0)
                {
                    if (result.Count() > 0)
                    {
                        //获取商铺名称
                        List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                        Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds); //获取商铺名称

                        //获取供应商名称
                        var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();

                        //获取提交人名称
                        List<Guid> SubId = (from n in query where n.SubId != null select n.SubId).Distinct().ToList();
                        List<Guid> ModId = (from n in query where n.ModifiedId != null select n.ModifiedId).Distinct().ToList();
                        List<Guid> userid = SubId.Union(ModId).Distinct().ToList();
                        var Userinfo = CBCSV.GetUserNameAndCodes(userid);

                        foreach (var item in result)
                        {
                            //获取提交人名称
                            var NameAndCodes = Userinfo[item.SubId];
                            item.SubName = NameAndCodes.Item1;
                            item.SubCode = NameAndCodes.Item2;
                            //获取供应商名称
                            var SupplierName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                            if (!string.IsNullOrEmpty(SupplierName))
                            {
                                item.SupplierName = SupplierName;
                            }
                            //获取商铺名称
                            if (listApps.ContainsKey(item.AppId))
                            {
                                var listAppName = listApps[item.AppId];
                                if (!String.IsNullOrEmpty(listAppName))
                                {
                                    item.AppName = listAppName;
                                }
                            }
                            //获取修改人名称
                            if (item.ModifiedId != new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9") && item.ModifiedId != null)
                            {
                                Guid MId = item.ModifiedId ?? new Guid("00000000-0000-0000-0000-000000000000");
                                NameAndCodes = Userinfo[MId];
                                item.ModifiedName = NameAndCodes.Item1;
                                item.ModifiedCode = NameAndCodes.Item2;
                            }
                            else if (item.ModifiedId == new Guid("72EF9DC4-2615-4649-92A9-9A4C71D1AFF9"))
                            {
                                item.ModifiedName = "系统自动";
                                item.ModifiedCode = "京东同步";
                            }
                            else
                            {
                                item.ModifiedName = "";
                                item.ModifiedCode = "";
                            }
                            //格式化状态
                            if (!item.IsDel)
                            {
                                if (item.State == 0)
                                {
                                    item.StateName = "在售中";
                                }
                                else
                                {
                                    item.StateName = "已下架";
                                }
                            }
                            else
                            {
                                item.StateName = "已删除";
                            }

                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取变动商品表异常。"), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyListExt(System.Guid EsAppid)
        {
            try
            {
                var mallapplylist = MallApply.ObjectSet().AsQueryable();
                if (!string.IsNullOrWhiteSpace(EsAppid.ToString()) && (!EsAppid.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    mallapplylist = mallapplylist.Where(p => p.EsAppId == EsAppid && (p.State.Value == 2 || p.State.Value == 4)).AsQueryable();
                }
                List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> result = (from c in mallapplylist
                                                                                       select new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO
                                                                                       {
                                                                                           AppId = c.AppId,
                                                                                           AppName = c.AppName
                                                                                       }).ToList();
                //易捷北京特殊处理
                if (EsAppid == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"))
                {
                    Deploy.CustomDTO.MallApply.MallApplyDTO YiJie = new Deploy.CustomDTO.MallApply.MallApplyDTO() { AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"), AppName = "易捷北京" };
                    result.Add(YiJie);
                }
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("查询商城信息GetMallApplyListExt。异常信息"), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询供应商信息
        /// </sum.Add()mary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierListExt(System.Guid EsAppid)
        {
            try
            {
                var SupplierList = Supplier.ObjectSet().AsQueryable();
                if (EsAppid != Guid.Empty)
                {
                    SupplierList = SupplierList.Where(p => p.EsAppId == EsAppid).AsQueryable();
                }
                List<SupplierDTO> result = (from a in SupplierList
                                            select new SupplierDTO
                                            {
                                                AppId = a.AppId,
                                                EsAppId = a.EsAppId,
                                                SupplierName = a.SupplierName,
                                                SupplierCode = a.SupplierCode
                                            }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("查询供应城信息GetSupplierListExt。异常信息"), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询供应商对应的所有商铺ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetAppIdsBySupplierExt(System.Guid EsAppId)
        {
            try
            {
                var SupplierList = Supplier.ObjectSet().Where(p => p.EsAppId == EsAppId).AsQueryable();
                List<Jinher.AMP.BTP.Deploy.SupplierDTO> result = (from a in SupplierList
                                                                  select new Jinher.AMP.BTP.Deploy.SupplierDTO
                                                                  {
                                                                      AppId = a.AppId,
                                                                      EsAppId = a.EsAppId,
                                                                      SupplierName = a.SupplierName
                                                                  }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("查询供应城信息GetAppIdsBySupplierExt。异常信息"), ex);
                return null;
            }
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> GetUserListExt()
        {
            List<Guid> SubId = (from n in CommodityChange.ObjectSet() where n.SubId != null select n.SubId).Distinct().ToList();
            List<Guid> ModId = (from n in CommodityChange.ObjectSet() where n.ModifiedId != null select n.ModifiedId).Distinct().ToList();
            List<Guid> userid = SubId.Union(ModId).Distinct().ToList();
            CBCSVFacade CBCSV = new CBCSVFacade();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> userinfolist = CBCSV.GetUserNameByIds(userid).Select(s => new UserNameDTO { UserId = s.UserId, UserName = s.UserName }).ToList();
            //京东自动改价修改人信息
            userinfolist.Add(new UserNameDTO { UserId = new Guid("9AF7F46A-EA52-4AA3-B8C3-9FD484C2AF12"), UserName = "系统自动" });
            return userinfolist;
        }
        /// <summary>
        /// 插入改动商品信息
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChangeExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> commodity)
        {
            try
            {
                if (commodity == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var ComIds = commodity.Select(s => s.CommodityId).ToList();
                var ComList = CommodityChange.ObjectSet().Where(p => ComIds.Contains(p.CommodityId)).GroupBy(p => p.CommodityId).Select(p => p.OrderByDescending(s => s.ModifiedOn).FirstOrDefault()).ToList();
                
                var LoginUserID =this.ContextDTO.LoginUserID;                
                foreach (var item in commodity.Distinct())
                {
                    var CommodityList = ComList.FirstOrDefault(p => p.CommodityId == item.CommodityId);
                    System.TimeSpan t3 = TimeSpan.Parse("0");
                    if (CommodityList != null)
                    {
                        var ModifyTime = CommodityList.ModifiedOn;
                        t3 = item.ModifiedOn - ModifyTime;
                    }
                    if (t3.TotalSeconds >= 5 || CommodityList == null)//时间间隔超过五秒  避免数据重复
                    {
                        CommodityChange ChangedCommodity = CommodityChange.CreateCommodityChange();
                        ChangedCommodity.Id = Guid.NewGuid();
                        ChangedCommodity.CommodityId = item.CommodityId;
                        ChangedCommodity.Name = item.Name;
                        ChangedCommodity.Code = item.Code;
                        ChangedCommodity.No_Number = item.No_Number;
                        ChangedCommodity.SubId = item.SubId;
                        ChangedCommodity.Price = item.Price;
                        ChangedCommodity.Stock = item.Stock;
                        ChangedCommodity.PicturesPath = item.PicturesPath;
                        ChangedCommodity.State = item.State;
                        ChangedCommodity.IsDel = item.IsDel;
                        ChangedCommodity.Description = "";
                        ChangedCommodity.AppId = item.AppId;
                        ChangedCommodity.No_Code = item.No_Code;
                        ChangedCommodity.TotalCollection = item.TotalCollection;
                        ChangedCommodity.TotalReview = item.TotalReview;
                        ChangedCommodity.Salesvolume = item.Salesvolume;
                        ChangedCommodity.ModifiedOn = DateTime.Now;
                        ChangedCommodity.GroundTime = item.GroundTime;
                        ChangedCommodity.ComAttribute = item.ComAttribute;
                        ChangedCommodity.CategoryName = item.CategoryName;
                        ChangedCommodity.SortValue = item.SortValue;
                        ChangedCommodity.FreightTemplateId = item.FreightTemplateId;
                        ChangedCommodity.MarketPrice = item.MarketPrice;
                        ChangedCommodity.IsEnableSelfTake = item.IsEnableSelfTake;
                        ChangedCommodity.Weight = item.Weight;
                        ChangedCommodity.PricingMethod = item.PricingMethod;
                        ChangedCommodity.SaleAreas = item.SaleAreas;
                        ChangedCommodity.SharePercent = item.SharePercent;
                        ChangedCommodity.CommodityType = item.CommodityType;
                        ChangedCommodity.HtmlVideoPath = item.HtmlVideoPath;
                        ChangedCommodity.MobileVideoPath = item.MobileVideoPath;
                        ChangedCommodity.VideoPic = item.VideoPic;
                        ChangedCommodity.VideoName = item.VideoName;
                        ChangedCommodity.ScorePercent = item.ScorePercent;
                        ChangedCommodity.Duty = item.Duty;
                        ChangedCommodity.SpreadPercent = item.SpreadPercent;
                        ChangedCommodity.ScoreScale = item.ScoreScale;
                        ChangedCommodity.TaxRate = item.TaxRate;
                        ChangedCommodity.TaxClassCode = item.TaxClassCode;
                        ChangedCommodity.Unit = item.Unit;
                        ChangedCommodity.InputRax = item.InputRax;
                        ChangedCommodity.Barcode = item.Barcode;
                        ChangedCommodity.JDCode = item.JDCode;
                        ChangedCommodity.CostPrice = item.CostPrice;
                        ChangedCommodity.IsAssurance = item.IsAssurance;
                        ChangedCommodity.IsReturns = item.IsReturns;
                        ChangedCommodity.ServiceSettingId = item.ServiceSettingId;
                        ChangedCommodity.Type = item.Type;
                        ChangedCommodity.YJCouponActivityId = item.YJCouponActivityId;
                        ChangedCommodity.YJCouponType = item.YJCouponType;
                        ChangedCommodity.SubOn = item.SubOn;
                        ChangedCommodity.ModifiedId = LoginUserID;
                        ChangedCommodity.AuditState = 0;
                        ChangedCommodity.SubTime = DateTime.Now;
                        contextSession.SaveObject(ChangedCommodity);
                    }
                }
                int count = contextSession.SaveChanges();
                LogHelper.Info("数据同步到商品变动明细报表保存条数: " + count + "，DateTime: " + DateTime.Now);
                return new ResultDTO { ResultCode = 1, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存商品变动数据异常"), ex);
                return new ResultDTO { ResultCode = 0, Message = "保存失败", isSuccess = false };
            }
        }

        /// <summary>
        /// 根据商品id获取活动类型
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public int JudgeActivityTypeExt(Guid commodityId)
        {
            try
            {
                DateTime now = DateTime.Now;
                
                List<TodayPromotionDTO> s  = (from p in PromotionItems.ObjectSet()
                    join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                    where p.CommodityId == commodityId && !pro.IsDel && pro.IsEnable &&
                    pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                    orderby pro.PromotionType descending
                    select new TodayPromotionDTO()
                    {
                        PromotionType = pro.PromotionType,
                    }).ToList();
                int proList = 0;                
                if (s != null)
                {
                    foreach (var item in s)
                    {
                        proList = item.PromotionType;
                    }
                }
                else
                    proList = 9;

                return proList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品活动类型异常{0}",commodityId), ex);
                throw;
            }
        }


    }
}
