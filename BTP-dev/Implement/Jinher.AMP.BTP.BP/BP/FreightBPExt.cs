using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.AMP.BTP.IBP.IService;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.PL;
using Jinher.AMP.CBC.IBP.Facade;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    public partial class FreightBP : BaseBP, IFreight
    {
        /// <summary>
        /// 获取运费模板列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<FreightDTO> GetFreightListByAppIdExt(Guid appId)
        {
            try
            {
                var query = (from data in FreightTemplate.ObjectSet()
                             where data.AppId == appId
                             orderby data.SubTime descending
                             select new FreightDTO()
                             {
                                 Id = data.Id,
                                 Name = data.Name,
                                 FreightMethod = data.FreightMethod,
                                 FreightTo = data.FreightTo,
                                 FirstCount = data.FirstCount,
                                 FirstCountPrice = data.FirstCountPrice,
                                 NextCount = data.NextCount,
                                 NextCountPrice = data.NextCountPrice,
                                 IsFreeExp = data.IsFreeExp,
                                 SubTime = data.SubTime,
                                 ModifiedOn = data.ModifiedOn,
                                 PricingMethod = data.PricingMethod,
                                 ExpressType = data.ExpressType
                             }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取运费模板列表。appId：{0}", appId), ex);
                return null;
            }
        }


        /// <summary>
        /// 获取运费模板列表
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页查询几条数据</param>
        /// <returns></returns>
        public List<FreightDTO> GetFreightTemplateListByAppIdExt(Guid appId, int pageIndex, int pageSize, out int rowCount)
        {
            try
            {
                var query = from data in FreightTemplate.ObjectSet()
                            where data.AppId == appId
                            orderby data.SubTime descending
                            select data;

                rowCount = query != null ? query.Count() : 0;

                var fTemplateDTOList = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(data => new FreightDTO
                {
                    Id = data.Id,
                    Name = data.Name,
                    FreightMethod = data.FreightMethod,
                    FreightTo = data.FreightTo,
                    FirstCount = data.FirstCount,
                    FirstCountPrice = data.FirstCountPrice,
                    NextCount = data.NextCount,
                    NextCountPrice = data.NextCountPrice,
                    IsFreeExp = data.IsFreeExp,
                    SubTime = data.SubTime,
                    ModifiedOn = data.ModifiedOn,
                    AppId = data.AppId,
                    PricingMethod = data.PricingMethod,
                    ExpressType = data.ExpressType
                }).ToList();
                var list = (from data in fTemplateDTOList
                            select new FreightDTO()
                            {
                                Id = data.Id,
                                Name = data.Name,
                                FreightMethod = data.FreightMethod,
                                FreightTo = data.FreightTo,
                                FirstCount = data.FirstCount,
                                FirstCountPrice = data.FirstCountPrice,
                                NextCount = data.NextCount,
                                NextCountPrice = data.NextCountPrice,
                                IsFreeExp = data.IsFreeExp,
                                SubTime = data.SubTime,
                                ModifiedOn = data.ModifiedOn,
                                AppId = data.AppId,
                                PricingMethod = data.PricingMethod,
                                ExpressType = data.ExpressType,
                                FreightDetailList = GetFreightTemplateDetailListByTemId(data.Id)
                            }).ToList();

                ComputeFreightRangeDTOs(list);

                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取运费模板列表。appId：{0}，pageIndex：{1}，pageSize：{2}", appId, pageIndex, pageSize), ex);
                rowCount = 0;
                return null;
            }
        }

        private void ComputeFreightRangeDTOs(IList<FreightDTO> templates)
        {
            var templateIds = templates.Where(predicate => predicate.PricingMethod == 3).Select(selector => selector.Id).ToList();

            var freights = FreightRangeDetails.ObjectSet()
                                    .Where(predicate => templateIds.Contains(predicate.TemplateId))
                                    .GroupBy(selector => new { selector.TemplateId, selector.ProvinceCodes })
                                    .ToList();

            foreach (var freight in freights)
            {
                var templateId = freight.Key.TemplateId;

                var template = templates.FirstOrDefault(predicate => predicate.Id == templateId);

                var provinceNames = ComputeProvincesName(freight.Key.ProvinceCodes);

                var costText = string.Join("；", freight.OrderBy(selector => selector.Min).Select(selector => ComputeFreightCostText(selector)));

                template.RangeFreights.Add(new FreightRangeDTO
                {
                    DefaultFreight = string.IsNullOrEmpty(freight.Key.ProvinceCodes) ? "默认运费" : "",
                    ExpressProvinces = provinceNames,
                    FreightCosts = costText
                });
            }
        }

        /// <summary>
        /// 计算多个省份的名称
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        private string ComputeProvincesName(string provinceCodes)
        {
            if (string.IsNullOrEmpty(provinceCodes))
            {
                return string.Empty;
            }

            CBCBPFacade facade = new CBCBPFacade();

            return string.Join("、", facade.GetProvincesNameByCode(provinceCodes));
        }

        /// <summary>
        /// 计算运费显示信息
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        private string ComputeFreightCostText(FreightRangeDetails details)
        {
            if (details == null)
            {
                return string.Empty;
            }

            if (details.Max > 0)
            {
                return string.Format("{0}~{1}元，运费{2}元", details.Min, details.Max, details.Cost);
            }
            return string.Format("{0}元以上，运费{1}元", details.Min, details.Cost);
        }

        /// <summary>
        /// 获取运费模板详细数据
        /// </summary>
        /// <param name="freightTemplateId">运费模板编号</param>
        /// <returns></returns>
        public List<FreightTemplateDetailDTO> GetFreightTemplateDetailListByTemIdExt(Guid freightTemplateId)
        {
            try
            {
                var query = (from data in FreightTemplateDetail.ObjectSet()
                             where data.FreightTemplateId == freightTemplateId
                             select new FreightTemplateDetailDTO()
                             {
                                 Id = data.Id,
                                 FreightTo = data.FreightTo,
                                 FirstCount = data.FirstCount,
                                 FirstCountPrice = data.FirstCountPrice,
                                 NextCount = data.NextCount,
                                 NextCountPrice = data.NextCountPrice,
                                 FreightTemplateId = data.FreightTemplateId,
                                 DestinationCodes = data.DestinationCodes
                             }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取运费模板详细数据。freightTemplateId：{0}", freightTemplateId), ex);
                return null;
            }
        }
        /// <summary>
        /// 增加一条运费明细
        /// </summary>
        /// <param name="freightDetail"></param>
        /// <returns></returns>
        public ResultDTO AddFreightDetailExt(FreightTemplateDetailDTO freightDetail)
        {
            try
            {
                if (freightDetail != null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    FreightTemplateDetail detail = new FreightTemplateDetail();
                    Guid id = Guid.NewGuid();
                    detail.Id = id;
                    detail.FreightTo = freightDetail.FreightTo;
                    detail.FirstCount = freightDetail.FirstCount;
                    detail.FirstCountPrice = freightDetail.FirstCountPrice;
                    detail.NextCount = freightDetail.NextCount;
                    detail.NextCountPrice = freightDetail.NextCountPrice;
                    detail.FreightTemplateId = freightDetail.FreightTemplateId;

                    detail.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(detail);
                    contextSession.SaveChanges();
                    return new ResultDTO { ResultCode = 0, Message = id.ToString() };
                }
                return new ResultDTO { ResultCode = 2, Message = "穿入的实体为null" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("服务异常。freightDetail：{0}", freightDetail), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }
        /// <summary>
        /// 删除一条模板明细
        /// </summary>
        /// <param name="freightDetailId"></param>
        /// <returns></returns>
        public ResultDTO DeleteFreightDetailExt(Guid freightDetailId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                FreightTemplateDetail detail = FreightTemplateDetail.ObjectSet().Where(s => s.Id == freightDetailId).FirstOrDefault();
                if (detail != null)
                {
                    detail.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(detail);
                }
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除一条模板明细服务异常。freightDetailId：{0}", freightDetailId), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }
        /// <summary>
        /// 删除运费模板
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultDTO DeleteFreightExt(Guid Id)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var query = FreightTemplateDetail.ObjectSet().Where(s => s.FreightTemplateId == Id).ToList();
                if (query != null && query.Count > 0)
                {
                    foreach (FreightTemplateDetail detail in query)
                    {
                        detail.EntityState = System.Data.EntityState.Deleted;
                        contextSession.Delete(detail);
                    }
                }
                var fpfQuery = FreightPartialFree.ObjectSet().Where(fpf => fpf.FreightTemplateId == Id);
                if (fpfQuery.Any())
                {
                    foreach (FreightPartialFree partialFree in fpfQuery)
                    {
                        partialFree.EntityState = System.Data.EntityState.Deleted;
                        contextSession.Delete(partialFree);
                    }
                }
                FreightTemplate tem = FreightTemplate.ObjectSet().Where(s => s.Id == Id).FirstOrDefault();
                if (tem != null)
                {
                    tem.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(tem);
                }
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除运费模板服务异常。Id：{0}", Id), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }
        /// <summary>
        /// 保存运费及其明细
        /// </summary>
        /// <param name="freight">运费模板</param>
        /// <param name="freightDetailList">运费明细集合</param>
        /// <returns></returns>
        public ResultDTO AddFreightAndFreightDetailExt(FreightTemplateDTO freight, List<FreightTemplateDetailDTO> freightDetailList)
        {
            try
            {
                if (freight == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "运费模板为null" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                FreightTemplate tem = new FreightTemplate();
                Guid ID = Guid.NewGuid();
                tem.Id = ID;
                tem.Name = freight.Name;
                tem.FreightMethod = freight.FreightMethod;
                tem.FreightTo = freight.FreightTo;
                tem.FirstCount = freight.FirstCount;
                tem.FirstCountPrice = freight.FirstCountPrice;
                tem.NextCount = freight.NextCount;
                tem.NextCountPrice = freight.NextCountPrice;
                tem.IsFreeExp = freight.IsFreeExp;
                tem.AppId = freight.AppId;
                tem.SubTime = DateTime.Now;
                tem.ModifiedOn = DateTime.Now;
                tem.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(tem);
                if (freightDetailList != null && freightDetailList.Count > 0)
                {
                    FreightTemplateDetail ftDetail;
                    foreach (FreightTemplateDetailDTO detail in freightDetailList)
                    {
                        ftDetail = new FreightTemplateDetail();
                        ftDetail.Id = Guid.NewGuid();
                        ftDetail.FreightTemplateId = ID;
                        ftDetail.FreightTo = detail.FreightTo;
                        ftDetail.FirstCount = detail.FirstCount;
                        ftDetail.FirstCountPrice = detail.FirstCountPrice;
                        ftDetail.NextCount = detail.NextCount;
                        ftDetail.NextCountPrice = detail.NextCountPrice;
                        ftDetail.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(ftDetail);
                    }
                }
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = ID.ToString() };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存运费及其明细服务异常。freight：{0}，freightDetailList：{1}", JsonHelper.JsonSerializer(freight), JsonHelper.JsonSerializer(freightDetailList)), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }
        /// <summary>
        /// 更新运费模板和运费详细信息列表
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        public ResultDTO UpdateFreightAndFreightDetailExt(FreightTemplateDTO freight, List<FreightTemplateDetailDTO> freightDetailList)
        {
            try
            {
                if (freight == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "运费模板为null" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                FreightTemplate fTemplate = FreightTemplate.ObjectSet().Where(s => s.Id == freight.Id).FirstOrDefault();
                fTemplate.Name = freight.Name;
                fTemplate.IsFreeExp = freight.IsFreeExp;
                fTemplate.FreightMethod = freight.FreightMethod;
                fTemplate.FirstCount = freight.FirstCount;
                fTemplate.FirstCountPrice = freight.FirstCountPrice;
                fTemplate.NextCount = freight.NextCount;
                fTemplate.NextCountPrice = freight.NextCountPrice;
                fTemplate.ModifiedOn = DateTime.Now;
                fTemplate.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(fTemplate);
                if (freightDetailList != null && freightDetailList.Count > 0)
                {
                    FreightTemplateDetail detail;
                    foreach (FreightTemplateDetailDTO ftDetail in freightDetailList)
                    {
                        if (ftDetail.Id == Guid.Empty)
                        {
                            detail = new FreightTemplateDetail();
                            detail.FreightTo = ftDetail.FreightTo;
                            detail.FirstCount = ftDetail.FirstCount;
                            detail.FirstCountPrice = ftDetail.FirstCountPrice;
                            detail.NextCount = ftDetail.NextCount;
                            detail.NextCountPrice = ftDetail.NextCountPrice;
                            detail.Id = Guid.NewGuid();
                            detail.FreightTemplateId = fTemplate.Id;
                            detail.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(detail);
                        }
                        else
                        {
                            detail = FreightTemplateDetail.ObjectSet().Where(s => s.Id == ftDetail.Id).FirstOrDefault();
                            if (detail != null)
                            {
                                detail.FreightTo = ftDetail.FreightTo;
                                detail.FirstCount = ftDetail.FirstCount;
                                detail.FirstCountPrice = ftDetail.FirstCountPrice;
                                detail.NextCount = ftDetail.NextCount;
                                detail.NextCountPrice = ftDetail.NextCountPrice;
                                detail.EntityState = System.Data.EntityState.Modified;
                                contextSession.SaveObject(detail);
                            }
                        }
                    }
                }
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("更新运费及其明细服务异常。freight：{0}，freightDetailList：{1}", JsonHelper.JsonSerializer(freight), JsonHelper.JsonSerializer(freightDetailList)), ex);
                return new ResultDTO { ResultCode = 1, Message = ex.Message };
            }
        }
        /// <summary>
        /// 获取一条运费模板记录
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public FreightDTO GetOneFreightExt(Guid freightTemplateId)
        {
            try
            {
                FreightDTO freight = null;
                FreightTemplate ftDTO = FreightTemplate.ObjectSet().Where(s => s.Id == freightTemplateId).FirstOrDefault();
                if (ftDTO == null)
                {
                    return freight;
                }
                freight = new FreightDTO();
                freight.Id = ftDTO.Id;
                freight.AppId = ftDTO.AppId;
                freight.Name = ftDTO.Name;
                freight.IsFreeExp = ftDTO.IsFreeExp;
                freight.FreightMethod = ftDTO.FreightMethod;
                freight.FreightTo = ftDTO.FreightTo;
                freight.FirstCount = ftDTO.FirstCount;
                freight.FirstCountPrice = ftDTO.FirstCountPrice;
                freight.NextCount = ftDTO.NextCount;
                freight.NextCountPrice = ftDTO.NextCountPrice;
                freight.PricingMethod = ftDTO.PricingMethod;
                freight.ExpressType = ftDTO.ExpressType;

                //运费详情。
                freight.FreightDetailList = GetFreightTemplateDetailListByTemId(freightTemplateId);

                //部分包邮
                var pfQuery = from fpf in FreightPartialFree.ObjectSet()
                              where fpf.FreightTemplateId == ftDTO.Id
                              select fpf;
                if (pfQuery.Any())
                {
                    var pfList = pfQuery.ToList().ConvertAll(ConvertFreightPartialFree2ExtDTO);
                    try
                    {
                        var provList = CBCBP.Instance.GeProvinceByCountryCode();
                        if (provList != null && provList.Any())
                        {
                            foreach (var pf in pfList)
                            {
                                var provNamesQ = from p in provList
                                                 where ("," + pf.DestinationCodes + ",").Contains("," + p.Code + ",")
                                                 select p.Name;
                                pf.FreightTo = string.Join(",", provNamesQ);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("GetOneFreightExt中调用Jinher.AMP.CBC.IBP.Facade.GeProvinceByCountryCode异常", ex);
                    }
                    freight.PartialFreeList = pfList;
                }

                #region 价格区间运费
                var details = FreightRangeDetails.ObjectSet().Where(predicate => predicate.TemplateId == freightTemplateId);
                TPS.CBCBPFacade facade = new CBCBPFacade();
                if (details.Any())
                {
                    foreach (var group in details.GroupBy(selector => new { selector.ProvinceCodes, selector.IsSpecific }))
                    {
                        var sort = group.OrderBy(selector => selector.Min);

                        if (!group.Key.IsSpecific)
                        {
                            foreach (var detail in sort)
                            {
                                freight.DefaultRangeFreightDetails.Add(new Deploy.CustomDTO.FreightRangeDefaultDetailsDTO
                                {
                                    Min = detail.Min,
                                    Max = detail.Max,
                                    Cost = detail.Cost
                                });
                            }
                            continue;
                        }

                        freight.SpecificRangeFreightDetails.Add(new FreightRangeSpecificDetailsDTO
                        {
                            ProvinceNames = facade.GetProvincesNameByCode(group.Key.ProvinceCodes),
                            ProvinceCodes = group.Key.ProvinceCodes,
                            Details = sort.Select(selector => new Jinher.AMP.BTP.Deploy.CustomDTO.FreightRangeDefaultDetailsDTO
                            {
                                Min = selector.Min,
                                Max = selector.Max,
                                Cost = selector.Cost
                            }).ToList()
                        });
                    }
                }
                #endregion

                return freight;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取一条运费记录服务异常。freightTemplateId：{0}", freightTemplateId), ex);
                return null;
            }
        }
        /// <summary>
        /// 运费模板是否关联了商品
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public ResultDTO IsContactCommodityExt(Guid freightTemplateId)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                var query = Commodity.ObjectSet().Where(c => c.FreightTemplateId == freightTemplateId && c.CommodityType == 0).FirstOrDefault();
                if (query != null)
                {
                    result.ResultCode = 0;
                    result.Message = "Success";
                }
                else
                {
                    result.ResultCode = 1;
                    result.Message = "没有查询到关联的商品";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("运费模板是否关联商品服务异常。freightTemplateId：{0}", freightTemplateId), ex);
                result.ResultCode = 3;
                result.Message = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freightDTO">一个运费模板相关的所有信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveFreightTemplateFullExt(Jinher.AMP.BTP.Deploy.CustomDTO.FreightTempFullDTO freightDTO)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                #region 参数验证

                if (freightDTO == null)
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }



                List<FreightTemplateDetailDTO> fdetailList = freightDTO.DetailList;
                List<FreightPartialFreeDTO> pfList = freightDTO.PartialFreeList;

                if (fdetailList != null && fdetailList.Any())
                {
                    foreach (FreightTemplateDetailDTO ftd in fdetailList)
                    {
                        if (string.IsNullOrWhiteSpace(ftd.DestinationCodes))
                        {
                            result.ResultCode = 4;
                            result.Message = "参数错误，请指定区域！";
                            return result;
                        }
                    }
                }

                if (pfList != null && pfList.Any())
                {
                    foreach (FreightPartialFreeDTO fpf in pfList)
                    {
                        if (string.IsNullOrWhiteSpace(fpf.DestinationCodes))
                        {
                            result.ResultCode = 5;
                            result.Message = "参数错误，请指定包邮区域！";
                            return result;
                        }
                    }
                }

                #endregion






                ContextSession contextSession = ContextFactory.CurrentThreadContext;


                FreightTemplate tem = null;
                bool isNewTemp = false;

                Guid tempId = freightDTO.Freight.Id;

                if (tempId != Guid.Empty)
                {
                    tem = FreightTemplate.ObjectSet().Where(s => s.Id == freightDTO.Freight.Id).FirstOrDefault();
                }
                else
                {
                    tempId = Guid.NewGuid();
                    isNewTemp = true;

                    int existNameCount = (from ft in FreightTemplate.ObjectSet()
                                          where ft.Name == freightDTO.Freight.Name
                                          && ft.AppId == freightDTO.Freight.AppId
                                          select ft.Id).Count();
                    if (existNameCount > 0)
                    {
                        result.ResultCode = 3;
                        result.Message = "已存在相同名称的运费模板！";
                        return result;
                    }
                }

                if (tem == null)
                {
                    tem = new FreightTemplate();
                    isNewTemp = true;
                }


                //运费模板
                tem.FillWith(freightDTO.Freight);
                tem.Id = tempId;
                tem.FreightTo = string.IsNullOrWhiteSpace(freightDTO.Freight.FreightTo) ? "" : freightDTO.Freight.FreightTo;
                tem.ModifiedOn = DateTime.Now;

                if (isNewTemp)
                {
                    tem.SubTime = DateTime.Now;
                    tem.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(tem);
                }

                IEnumerable<FreightTemplateDetail> detailListOri = null;
                IEnumerable<FreightPartialFree> partialFreeListOri = null;
                if (!isNewTemp)
                {
                    detailListOri = from ftd in FreightTemplateDetail.ObjectSet()
                                    where ftd.FreightTemplateId == tempId
                                    select ftd;
                    partialFreeListOri = from pf in FreightPartialFree.ObjectSet()
                                         where pf.FreightTemplateId == tempId
                                         select pf;
                }
                if (detailListOri == null)
                {
                    detailListOri = new List<FreightTemplateDetail>();
                }
                if (partialFreeListOri == null)
                {
                    partialFreeListOri = new List<FreightPartialFree>();
                }

                //模板详情。
                IEnumerable<Guid> detailIdsNew = new List<Guid>();

                if (fdetailList != null && fdetailList.Count > 0)
                {
                    detailIdsNew = from ftd in fdetailList select ftd.Id;
                    foreach (FreightTemplateDetailDTO detail in fdetailList)
                    {
                        bool isdetailNew = false;
                        FreightTemplateDetail ftDetail = detailListOri.Where(detOri => detOri.Id == detail.Id).FirstOrDefault();
                        if (ftDetail == null)
                        {
                            ftDetail = new FreightTemplateDetail();
                            isdetailNew = true;
                        }
                        ftDetail.FillWith(detail);
                        ftDetail.FreightTemplateId = tempId;
                        if (isdetailNew)
                        {
                            ftDetail.Id = Guid.NewGuid();
                            ftDetail.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(ftDetail);
                        }
                    }
                }

                //删除db中有，更新列表中没有的FreightTemplateDetail
                if (detailListOri.Any())
                {

                    var delDetails = from ftd in detailListOri
                                     where (!detailIdsNew.Contains(ftd.Id))
                                     select ftd;
                    if (delDetails.Any())
                    {
                        foreach (var det in delDetails)
                        {
                            det.EntityState = System.Data.EntityState.Deleted;
                        }
                    }
                }

                //部分包邮信息。
                IEnumerable<Guid> pfIdsNew = new List<Guid>();

                if (pfList != null && pfList.Count > 0)
                {
                    pfIdsNew = from pf in pfList select pf.Id;
                    foreach (FreightPartialFreeDTO fpfDto in pfList)
                    {
                        bool isFpfNew = false;
                        FreightPartialFree fpf = partialFreeListOri.Where(detOri => detOri.Id == fpfDto.Id).FirstOrDefault();
                        if (fpf == null)
                        {
                            fpf = new FreightPartialFree();
                            isFpfNew = true;
                        }
                        fpf.FillWith(fpfDto);
                        fpf.FreightTemplateId = tempId;

                        if (isFpfNew)
                        {
                            fpf.Id = Guid.NewGuid();
                            fpf.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(fpf);
                        }
                    }
                }
                //删除db中有，更新列表中没有的FreightTemplateDetail
                if (partialFreeListOri.Any())
                {
                    var delpfs = from pf in partialFreeListOri
                                 where (!pfIdsNew.Contains(pf.Id))
                                 select pf;
                    if (delpfs.Any())
                    {
                        foreach (var det in delpfs)
                        {
                            det.EntityState = System.Data.EntityState.Deleted;
                        }
                    }
                }


                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = tempId.ToString() };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SaveFreightTemplateFullExt服务异常。freightDTO：{0}", JsonHelper.JsonSerializer(freightDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "保存运费模板及其明细服务异常!" };
            }
        }

        private FreightPartialFreeExtDTO ConvertFreightPartialFree2ExtDTO(FreightPartialFree fpf)
        {
            FreightPartialFreeExtDTO fpfDto = new FreightPartialFreeExtDTO();
            fpfDto.FillWith(fpf);
            return fpfDto;
        }

        /// <summary>
        /// 保存区间运费模板及其明细
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO SaveRangeFreightTemplateExt(Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            var output = new ResultDTO();

            if (inputDTO == null)
            {
                output.ResultCode = 2;
                output.Message = "参数错误, inputDTO 不能为空";
            }

            try
            {
                if (inputDTO.TemplateId != Guid.Empty)
                {
                    OnlyUpdateRangeFreightTemplate(inputDTO);
                }
                else
                {
                    OnlyCreateRangeFreightTemplate(inputDTO);
                }

                output.isSuccess = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存区间运费时出现异常.{0} input:{1}", ex.Message, JsonHelper.JsonSerializer(inputDTO)), ex);
                output.ResultCode = 3;
                output.Message = ex.Message;
            }
            return output;
        }

        /// <summary>
        /// 仅创建区间运费模板
        /// </summary>
        /// <param name="inputDTO"></param>
        private void OnlyCreateRangeFreightTemplate(Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            var context = ContextFactory.CurrentThreadContext;

            var template = FreightTemplate.CreateFreightTemplate();
            template.AppId = inputDTO.AppId;
            template.SubId = base.ContextDTO.LoginUserID;
            template.Name = inputDTO.Name;
            template.IsFreeExp = inputDTO.FreeExpress;
            template.PricingMethod = 3; //按区间计价
            template.SubTime = DateTime.Now;
            template.ExpressType = 0;

            template.FreightMethod = 0;
            template.FreightTo = "";
            template.FirstCount = 0M;
            template.FirstCountPrice = 0M;
            template.NextCount = 0M;
            template.NextCountPrice = 0M;
            template.ModifiedOn = DateTime.Now;
            template.IsDefault = 0;

            context.SaveObject(template);

            foreach (var freight in inputDTO.Freights)
            {
                var detail = FreightRangeDetails.CreateFreightRangeDetails();
                detail.TemplateId = template.Id;
                detail.ProvinceCodes = freight.ProvinceCodes;
                detail.IsSpecific = !string.IsNullOrEmpty(freight.ProvinceCodes);
                detail.Min = freight.Min;
                detail.Max = freight.Max;
                detail.Cost = freight.Cost;

                context.SaveObject(detail);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// 仅更新区间运费模板
        /// </summary>
        /// <param name="inputDTO"></param>
        private void OnlyUpdateRangeFreightTemplate(Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException("inputDTO");
            }

            if (inputDTO.TemplateId == Guid.Empty)
            {
                throw new ArgumentException("inputDTO.TemplateId");
            }

            var templateId = inputDTO.TemplateId;

            var template = FreightTemplate.ObjectSet().FirstOrDefault(predicate => predicate.Id == templateId);
            if (template == null)
            {
                throw new Exception(string.Format("未找到运费模板对象 FreightTemplateId: {0}", templateId));
            }

            template.Name = inputDTO.Name;
            template.IsFreeExp = inputDTO.FreeExpress;
            template.ModifiedId = base.ContextDTO.LoginUserID;

            FreightRangeDetails.ObjectSet()
                .Where(predicate => predicate.TemplateId == templateId)
                .ToList()
                .ForEach(freight =>
                        freight.EntityState = System.Data.EntityState.Deleted);

            var context = ContextFactory.CurrentThreadContext;

            foreach (var group in inputDTO.Freights.GroupBy(selector => selector.IsSpecific))
            {
                foreach (var freight in group)
                {
                    var detail = FreightRangeDetails.CreateFreightRangeDetails();
                    detail.TemplateId = template.Id;
                    detail.ProvinceCodes = freight.ProvinceCodes;
                    detail.IsSpecific = freight.IsSpecific;
                    detail.Min = freight.Min;
                    detail.Max = freight.Max;
                    detail.Cost = freight.Cost;

                    context.SaveObject(detail);
                }
            }

            context.SaveObject(template);
            context.SaveChange();
        }

        /// <summary>
        /// 建立运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO JoinCommodityExt(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException();
            }

            var templateId = inputDTO.TemplateId;
            var commodityIds = inputDTO.CommodityIds;
            var context = ContextFactory.CurrentThreadContext;
            var outputDTO = new Deploy.CustomDTO.ResultDTO();
            var defaultTemplateId = Guid.Empty;

            try
            {
                Commodity.ObjectSet()
                    .Where(predicate => commodityIds.Contains(predicate.Id)
                                        && (predicate.FreightTemplateId == null || predicate.FreightTemplateId == defaultTemplateId))
                                        .ToList().ForEach(commodity =>
                                        {
                                            if (commodity.FreightTemplateId == null || commodity.FreightTemplateId == Guid.Empty)
                                            {
                                                commodity.FreightTemplateId = templateId;
                                                commodity.EntityState = System.Data.EntityState.Modified;
                                                context.Update(commodity);
                                            }
                                        });
                context.SaveChange();

                outputDTO.ResultCode = 0;
                outputDTO.Message = "成功的建立了关联";
            }
            catch (Exception ex)
            {
                outputDTO.ResultCode = 1;
                outputDTO.Message = "建立关联失败. 可能的原因是：" + ex.Message;
            }
            return outputDTO;
        }

        /// <summary>
        /// 解除运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO UnjoinCommodityExt(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException();
            }

            var templateId = inputDTO.TemplateId;
            var commodityIds = inputDTO.CommodityIds;
            var context = ContextFactory.CurrentThreadContext;
            var outputDTO = new Deploy.CustomDTO.ResultDTO();

            try
            {
                Commodity.ObjectSet().Where(predicate => commodityIds.Contains(predicate.Id) && predicate.FreightTemplateId == templateId).ToList().ForEach(commodity =>
                {
                    commodity.FreightTemplateId = null;
                    commodity.EntityState = System.Data.EntityState.Modified;
                    context.Update(commodity);
                });
                context.SaveChange();

                outputDTO.isSuccess = true;
                outputDTO.ResultCode = 0;
                outputDTO.Message = "成功的解除了关联";
            }
            catch (Exception ex)
            {
                outputDTO.ResultCode = 1;
                outputDTO.Message = "解除关联失败. 可能的原因是：" + ex.Message;
            }
            return outputDTO;
        }
    }
}
