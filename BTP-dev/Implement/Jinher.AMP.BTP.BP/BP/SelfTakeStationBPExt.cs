
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/8/7 14:47:16
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 自提点
    /// </summary>
    public partial class SelfTakeStationBP : BaseBP, ISelfTakeStation
    {
        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体(District、Remark字段暂时无用)</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSelfTakeStationExt(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            if (selfTakeStationDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点实体不能为空" };
            }

            if (selfTakeStationDTO.CityOwnerId == Guid.Empty || string.IsNullOrWhiteSpace(selfTakeStationDTO.Name) || string.IsNullOrWhiteSpace(selfTakeStationDTO.Province) || string.IsNullOrWhiteSpace(selfTakeStationDTO.Address))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (string.IsNullOrWhiteSpace(selfTakeStationDTO.City))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (string.IsNullOrWhiteSpace(selfTakeStationDTO.QRCodeUrl) || selfTakeStationDTO.SpreadCode == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }

            if (selfTakeStationDTO.SelfTakeStationType == 1 && (!selfTakeStationDTO.AppId.HasValue || selfTakeStationDTO.AppId == Guid.Empty))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }

            //加入数量限制
            try
            {

                Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyCabinetParam prarm = new ZPH.Deploy.CustomDTO.ProxyCabinetParam();
                prarm.ChangeOrg = selfTakeStationDTO.CityOwnerId;
                prarm.BelongTo = selfTakeStationDTO.AppId.HasValue ? selfTakeStationDTO.AppId.Value : Guid.Empty;

                if (selfTakeStationDTO.SelfTakeStationType == 0)
                {
                    prarm.IsSelfPavilion = false;
                }
                else if (selfTakeStationDTO.SelfTakeStationType == 1)
                {
                    prarm.IsSelfPavilion = true;
                }
                LogHelper.Debug(string.Format("核查代理能否创建体验柜。prarm：{0}", JsonHelper.JsonSerializer(prarm)));
                var resultProxy = Jinher.AMP.BTP.TPS.ZPHSV.Instance.ChecksCanBeCreated(prarm);
                if (!resultProxy.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = "已超过最大数量限制" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("核查代理能否创建体验柜服务异常。selfTakeStationDTO：{0}", JsonHelper.JsonSerializer(selfTakeStationDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "核查代理能否创建体验柜服务异常" };
            }

            if (selfTakeStationDTO.selfTakeStationManager != null && selfTakeStationDTO.selfTakeStationManager.Any())
            {
                selfTakeStationDTO.selfTakeStationManager.RemoveAll(
                    checkItem =>
                    checkItem == null || checkItem.UserId == Guid.Empty || string.IsNullOrWhiteSpace(checkItem.UserCode));

                var selfManagerId = selfTakeStationDTO.selfTakeStationManager.Select(t => t.UserId).ToList();
                int count = SelfTakeStationManager.ObjectSet().Count(t => selfManagerId.Contains(t.UserId) && !t.IsDel);

                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "负责人已存在" };
                }
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                Guid selfTakeStationId = Guid.NewGuid();
                SelfTakeStation selfTakeStation = new SelfTakeStation()
                {
                    Id = selfTakeStationId,
                    CityOwnerId = selfTakeStationDTO.CityOwnerId,
                    Name = selfTakeStationDTO.Name,
                    Province = selfTakeStationDTO.Province,
                    City = selfTakeStationDTO.City,
                    District = "",
                    Address = selfTakeStationDTO.Address,
                    SpreadUrl = selfTakeStationDTO.SpreadUrl,
                    Remark = "",
                    QRCodeUrl = selfTakeStationDTO.QRCodeUrl,
                    SpreadCode = selfTakeStationDTO.SpreadCode,
                    SelfTakeStationType = selfTakeStationDTO.SelfTakeStationType,
                    AppId = selfTakeStationDTO.AppId
                };

                selfTakeStation.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(selfTakeStation);

                if (selfTakeStationDTO.selfTakeStationManager != null && selfTakeStationDTO.selfTakeStationManager.Count > 0)
                {
                    foreach (var selfManager in selfTakeStationDTO.selfTakeStationManager)
                    {
                        SelfTakeStationManager selfTakeStationManager = new SelfTakeStationManager()
                        {
                            Id = Guid.NewGuid(),
                            UserCode = selfManager.UserCode,
                            UserId = selfManager.UserId,
                            SelfTakeStationId = selfTakeStationId
                        };
                        selfTakeStationManager.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(selfTakeStationManager);
                    }
                }

                //添加到SpreadInfo
                SpreadInfoDTO spreadInfo = new SpreadInfoDTO();
                spreadInfo.Id = Guid.NewGuid();
                spreadInfo.SpreadId = selfTakeStationDTO.CityOwnerId;
                spreadInfo.SpreadUrl = selfTakeStationDTO.SpreadUrl;
                spreadInfo.SpreadCode = selfTakeStationDTO.SpreadCode;
                //spreadInfo.SpreadDesc
                //SpreadType 推广类型 0：推广主，1：电商馆，2：总代，3企业
                if (selfTakeStationDTO.SelfTakeStationType == 0)
                {
                    spreadInfo.SpreadType = 2;
                }
                else if (selfTakeStationDTO.SelfTakeStationType == 1)
                {
                    spreadInfo.SpreadType = 1;
                }
                spreadInfo.IsDel = 0;
                spreadInfo.AppId = selfTakeStationDTO.AppId.HasValue ? selfTakeStationDTO.AppId.Value : Guid.Empty;

                SpreadSV.Instance.BuildSaveSpreadInfo(spreadInfo);

                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加自提点服务异常。selfTakeStationDTO：{0}", JsonHelper.JsonSerializer(selfTakeStationDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新已建体验柜数量
            try
            {
                int count = 0;

                Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyCabinetNumParam prarm = new ZPH.Deploy.CustomDTO.ProxyCabinetNumParam();
                prarm.ChangeOrg = selfTakeStationDTO.CityOwnerId;
                prarm.BelongTo = selfTakeStationDTO.AppId.HasValue ? selfTakeStationDTO.AppId.Value : Guid.Empty;
                if (selfTakeStationDTO.SelfTakeStationType == 0)
                {
                    //总数量
                    count = SelfTakeStation.ObjectSet().Where(t => t.CityOwnerId == selfTakeStationDTO.CityOwnerId && t.SelfTakeStationType == selfTakeStationDTO.SelfTakeStationType && !t.IsDel).Count();
                    prarm.IsSelfPavilion = false;
                }
                else if (selfTakeStationDTO.SelfTakeStationType == 1)
                {
                    //总数量
                    count = SelfTakeStation.ObjectSet().Where(t => t.CityOwnerId == selfTakeStationDTO.CityOwnerId && t.SelfTakeStationType == selfTakeStationDTO.SelfTakeStationType && t.AppId == selfTakeStationDTO.AppId && !t.IsDel).Count();
                    prarm.IsSelfPavilion = true;
                }
                prarm.CabinetNum = count;
                LogHelper.Debug(string.Format("添加自提点时更新已建体验柜数量。prarm：{0}", JsonHelper.JsonSerializer(prarm)));
                var resultProxy = Jinher.AMP.BTP.TPS.ZPHSV.Instance.UpdateProxyCabinetNum(prarm);
                if (!resultProxy.isSuccess)
                {
                    LogHelper.Error(string.Format("添加自提点时更新已建体验柜数量服务失败。selfTakeStationDTO：{0}", JsonHelper.JsonSerializer(selfTakeStationDTO)));

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加自提点时更新已建体验柜数量服务异常。selfTakeStationDTO：{0}", JsonHelper.JsonSerializer(selfTakeStationDTO)), ex);
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体(District、Remark字段暂时无用)</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSelfTakeStationExt(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            if (selfTakeStationDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点实体不能为空" };
            }

            if (selfTakeStationDTO.Id == Guid.Empty || selfTakeStationDTO.CityOwnerId == Guid.Empty || string.IsNullOrWhiteSpace(selfTakeStationDTO.Name) || string.IsNullOrWhiteSpace(selfTakeStationDTO.Province) || string.IsNullOrWhiteSpace(selfTakeStationDTO.Address))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (string.IsNullOrWhiteSpace(selfTakeStationDTO.City))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (string.IsNullOrWhiteSpace(selfTakeStationDTO.QRCodeUrl) || selfTakeStationDTO.SpreadCode == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (selfTakeStationDTO.SelfTakeStationType == 1 && (!selfTakeStationDTO.AppId.HasValue || selfTakeStationDTO.AppId == Guid.Empty))
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点参数错误" };
            }
            if (selfTakeStationDTO.selfTakeStationManager == null)
                selfTakeStationDTO.selfTakeStationManager = new List<SelfTakeStationManagerSDTO>();
            try
            {
                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                SelfTakeStation selfTakeStation = SelfTakeStation.ObjectSet().FirstOrDefault(t => t.Id == selfTakeStationDTO.Id);
                if (selfTakeStation == null)
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                selfTakeStation.Name = selfTakeStationDTO.Name;
                selfTakeStation.Province = selfTakeStationDTO.Province;
                selfTakeStation.City = selfTakeStationDTO.City;
                selfTakeStation.District = "";
                selfTakeStation.Address = selfTakeStationDTO.Address;
                selfTakeStation.SpreadUrl = selfTakeStationDTO.SpreadUrl;
                selfTakeStation.Remark = "";
                selfTakeStation.QRCodeUrl = selfTakeStationDTO.QRCodeUrl;
                selfTakeStation.SpreadCode = selfTakeStationDTO.SpreadCode;
                selfTakeStation.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(selfTakeStation);
                var selfManagerIdsExitsModels = SelfTakeStationManager.ObjectSet().Where(t => t.SelfTakeStationId == selfTakeStationDTO.Id && !t.IsDel).ToList();
                var exitsUserIds = selfManagerIdsExitsModels.Select(t => t.UserId).ToList();
                var updateUserIds = selfTakeStationDTO.selfTakeStationManager.Select(t => t.UserId).ToList();

                var insertUserIds = updateUserIds.Except(exitsUserIds).ToList();
                var deleteUserIds = exitsUserIds.Except(updateUserIds).ToList();

                //删除
                foreach (var deleteUserId in deleteUserIds)
                {
                    SelfTakeStationManager selfTakeStationManager = selfManagerIdsExitsModels.First(t => t.UserId == deleteUserId);
                    selfTakeStationManager.IsDel = true;
                    selfTakeStationManager.EntityState = System.Data.EntityState.Modified;
                    selfTakeStationManager.ModifiedOn = now;
                }

                foreach (var selfManager in selfTakeStationDTO.selfTakeStationManager)
                {
                    //插入
                    if (insertUserIds.Contains(selfManager.UserId))
                    {
                        SelfTakeStationManager selfTakeStationManager = new SelfTakeStationManager()
                        {
                            Id = Guid.NewGuid(),
                            UserCode = selfManager.UserCode,
                            UserId = selfManager.UserId,
                            SelfTakeStationId = selfTakeStationDTO.Id
                        };
                        selfTakeStationManager.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(selfTakeStationManager);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改自提点服务异常。selfTakeStationDTO：{0}", JsonHelper.JsonSerializer(selfTakeStationDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStationsExt(System.Collections.Generic.List<System.Guid> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点实体不能为空" };
            }
            Guid cityOwnerId = Guid.Empty;
            Guid appId = Guid.Empty;
            int selfTakeStationType = 0;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var models = SelfTakeStation.ObjectSet().Where(s => ids.Contains(s.Id) && !s.IsDel).ToList();
                if (models != null && models.Count > 0)
                {
                    var first = models.FirstOrDefault();
                    cityOwnerId = first.CityOwnerId;
                    appId = first.AppId.HasValue ? first.AppId.Value : Guid.Empty;
                    selfTakeStationType = first.SelfTakeStationType;
                    //删除自提点
                    foreach (var item in models)
                    {
                        item.IsDel = true;
                        item.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(item);
                    }

                    var selfTakeStationManager = SelfTakeStationManager.ObjectSet().Where(s => ids.Contains(s.SelfTakeStationId) && !s.IsDel).ToList();
                    if (selfTakeStationManager != null && selfTakeStationManager.Count > 0)
                    {
                        //删除负责人
                        foreach (var manager in selfTakeStationManager)
                        {
                            manager.IsDel = true;
                            manager.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(manager);
                        }
                    }

                    //删除推广表信息
                    List<Guid> spreadCodeList = models.Select(t => t.SpreadCode).ToList();
                    var spreadInfo = SpreadInfo.ObjectSet().Where(s => spreadCodeList.Contains(s.SpreadCode) && s.IsDel != 1).ToList();
                    foreach (var item in spreadInfo)
                    {
                        item.IsDel = 1;
                        item.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(item);
                    }

                    contextSession.SaveObject(spreadInfo);

                }
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除自提点服务异常。ids：{0}", JsonHelper.JsonSerializer(ids)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新已建体验柜数量
            try
            {
                int count = 0;
                Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyCabinetNumParam prarm = new ZPH.Deploy.CustomDTO.ProxyCabinetNumParam();
                prarm.ChangeOrg = cityOwnerId;
                prarm.BelongTo = appId;

                if (selfTakeStationType == 0)
                {
                    //总数量
                    count = SelfTakeStation.ObjectSet().Where(t => t.CityOwnerId == cityOwnerId && t.SelfTakeStationType == selfTakeStationType && !t.IsDel).Count();
                    prarm.IsSelfPavilion = false;
                }
                else if (selfTakeStationType == 1)
                {
                    //总数量
                    count = SelfTakeStation.ObjectSet().Where(t => t.CityOwnerId == cityOwnerId && t.SelfTakeStationType == selfTakeStationType && t.AppId == appId && !t.IsDel).Count();
                    prarm.IsSelfPavilion = true;
                }
                prarm.CabinetNum = count;
                LogHelper.Debug(string.Format("删除自提点时更新已建体验柜数量。prarm：{0}", JsonHelper.JsonSerializer(prarm)));
                var resultProxy = Jinher.AMP.BTP.TPS.ZPHSV.Instance.UpdateProxyCabinetNum(prarm);
                if (!resultProxy.isSuccess)
                {
                    LogHelper.Error(string.Format("删除自提点成功，但更新已建体验柜数量服务失败。ids：{0}", JsonHelper.JsonSerializer(ids)));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除自提点时更新已建体验柜数量服务异常。ids：{0}", JsonHelper.JsonSerializer(ids)), ex);
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 查询自提点信息
        /// </summary>
        /// <param name="selfTakeStationSearch">查询类</param>
        /// <param name="rowCount">总记录数</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult> GetAllSelfTakeStationExt(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchSDTO selfTakeStationSearch, out int rowCount)
        {
            List<SelfTakeStationResult> result = new List<SelfTakeStationResult>();
            //总代名称
            string cityOwnerName = string.Empty;
            if (selfTakeStationSearch == null || selfTakeStationSearch.pageIndex < 1 || selfTakeStationSearch.pageSize < 1)
            {
                rowCount = 0;
                return result;
            }

            var query = SelfTakeStation.ObjectSet().Where(t => !t.IsDel);

            if (selfTakeStationSearch.SelfTakeStationType != null)
            {
                query = query.Where(t => t.SelfTakeStationType == selfTakeStationSearch.SelfTakeStationType);
            }

            if (selfTakeStationSearch.CityOwnerId != Guid.Empty)
            {
                query = query.Where(t => t.CityOwnerId == selfTakeStationSearch.CityOwnerId);
            }

            if (!string.IsNullOrWhiteSpace(selfTakeStationSearch.Name))
            {
                query = query.Where(t => t.Name.Contains(selfTakeStationSearch.Name));
            }
            if (selfTakeStationSearch.AppId != Guid.Empty)
            {
                query = query.Where(t => t.AppId == selfTakeStationSearch.AppId);
            }

            query = query.Distinct();
            rowCount = query.Count();

            query = query.OrderByDescending(n => n.SubTime).Skip((selfTakeStationSearch.pageIndex - 1) * selfTakeStationSearch.pageSize).Take(selfTakeStationSearch.pageSize);

            var tmpResult = query.ToList();

            if (tmpResult.Count == 0)
            {
                rowCount = 0;
                return result;
            }
            //总代名称服
            //try
            //{
            //    Jinher.AMP.ZPH.ISV.Facade.ProxyFacade proxyFac = new ZPH.ISV.Facade.ProxyFacade();
            //    Jinher.AMP.ZPH.Deploy.CustomDTO.ReturnInfo<List<Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyContentCDTO>> resultProxy = proxyFac.GetProxyById4BTP(selfTakeStationSearch.CityOwnerId);
            //    cityOwnerName = resultProxy.Data[0].proxyName;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error(string.Format("获取总代名称服务异常。id：{0}", selfTakeStationSearch.CityOwnerId), ex);
            //    rowCount = 0;
            //    return result;
            //}

            //总代名称服
            if (selfTakeStationSearch.SelfTakeStationType == 0)
            {
                try
                {
                    Jinher.AMP.ZPH.Deploy.CustomDTO.QueryProxyPrarm prarm = new ZPH.Deploy.CustomDTO.QueryProxyPrarm();
                    prarm.changeOrg = selfTakeStationSearch.CityOwnerId;

                    Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyContentCDTO proxyContent = new ZPH.Deploy.CustomDTO.ProxyContentCDTO();
                    Jinher.AMP.ZPH.Deploy.CustomDTO.ReturnInfo<List<Jinher.AMP.ZPH.Deploy.CustomDTO.ProxyContentCDTO>> resultProxy = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAllProxyList4BTP(prarm);
                    cityOwnerName = resultProxy.Data[0].proxyName;

                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("获取总代名称服务异常。id：{0}", selfTakeStationSearch.CityOwnerId), ex);
                    rowCount = 0;
                    return result;
                }
            }

            //取负责人信息
            var ids = tmpResult.Select(t => t.Id).ToList();
            var selfTakeStationManager = SelfTakeStationManager.ObjectSet().Where(t => ids.Contains(t.SelfTakeStationId) && !t.IsDel).ToList();


            try
            {

                foreach (var item in tmpResult)
                {
                    SelfTakeStationResult data = new SelfTakeStationResult();
                    data.Id = item.Id;
                    data.CityOwnerId = item.CityOwnerId;
                    data.Name = item.Name;
                    data.Province = item.Province;
                    data.City = item.City;
                    data.District = item.District;
                    data.Address = item.Address;
                    data.SpreadUrl = item.SpreadUrl;
                    data.Remark = item.Remark;
                    data.SubTime = item.SubTime;
                    data.ModifiedOn = item.ModifiedOn;
                    data.QRCodeUrl = item.QRCodeUrl;
                    data.SpreadCode = item.SpreadCode;

                    data.CityOwnerName = cityOwnerName;

                    string province = Jinher.AMP.BTP.Common.ProvinceCityHelper.GetAreaNameByCode(data.Province);
                    string city = string.Empty;
                    ////北京110000 天津120000 上海310000 重庆500000
                    //List<string> specialCityList = new List<string>() { "110000", "120000", "310000", "500000" };
                    //if (specialCityList.Contains(data.Province))
                    //{
                    //    city = "";
                    //}
                    //else
                    //{
                    //    city = Jinher.AMP.BTP.Common.ProvinceCityHelper.GetAreaNameByCode(data.City);
                    //}
                    city = Jinher.AMP.BTP.Common.ProvinceCityHelper.GetAreaNameByCode(data.City);
                    data.AddressDetail = province + city + data.Address;

                    //附上负责人信息
                    var selfManager = (from manager in selfTakeStationManager
                                       where manager.SelfTakeStationId == item.Id && !manager.IsDel
                                       select new Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationManagerSDTO
                                       {
                                           Id = manager.Id,
                                           SubTime = manager.SubTime,
                                           ModifiedOn = manager.ModifiedOn,
                                           UserCode = manager.UserCode,
                                           UserId = manager.UserId,
                                           SelfTakeStationId = manager.SelfTakeStationId
                                       }).ToList();
                    data.SelfTakeStationManageList = selfManager;

                    result.Add(data);
                }
                return result;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询自提点BP服务异常。selfTakeStationSearch：{0}", JsonHelper.JsonSerializer(selfTakeStationSearch)), ex);
                rowCount = 0;
                return result;
            }
        }

        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult GetSelfTakeStationByIdExt(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var query = SelfTakeStation.ObjectSet().Where(t => t.Id == id && !t.IsDel).FirstOrDefault();

            if (query == null)
            {
                return null;
            }

            Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult result = new Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult()
            {
                Id = query.Id,
                CityOwnerId = query.CityOwnerId,
                Name = query.Name,
                Province = query.Province,
                City = query.City,
                District = query.District,
                Address = query.Address,
                SpreadUrl = query.SpreadUrl,
                Remark = query.Remark,
                SubTime = query.SubTime,
                ModifiedOn = query.ModifiedOn,
                QRCodeUrl = query.QRCodeUrl,
                SpreadCode = query.SpreadCode
            };

            //附上负责人信息
            var selfManager = (from manager in SelfTakeStationManager.ObjectSet()
                               where manager.SelfTakeStationId == id && !manager.IsDel
                               select new Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationManagerSDTO
                               {
                                   Id = manager.Id,
                                   SubTime = manager.SubTime,
                                   ModifiedOn = manager.ModifiedOn,
                                   UserCode = manager.UserCode,
                                   UserId = manager.UserId,
                                   SelfTakeStationId = manager.SelfTakeStationId
                               }).ToList();
            result.SelfTakeStationManageList = selfManager;
            return result;
        }

        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSelfTakeStationManagerByUserIdExt(System.Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点实体不能为空" };
            }
            var result = SelfTakeStationManager.ObjectSet().Where(t => t.UserId == userId && !t.IsDel).Count();
            if (result == 0)
            {
                return new ResultDTO { ResultCode = 0, Message = "该账号尚未注册，请及时注册，以免影响正常使用！" };
            }
            else
            {
                return new ResultDTO { ResultCode = 2, Message = "该负责人已存在" };
            }
        }
        /// <summary>
        /// 删除负责人信息
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStationManagerByIdExt(System.Collections.Generic.List<System.Guid> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return new ResultDTO { ResultCode = 1, Message = "自提点实体不能为空" };
            }

            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var models = SelfTakeStationManager.ObjectSet().Where(s => ids.Contains(s.Id) && !s.IsDel).ToList();
                if (models != null && models.Count > 0)
                {
                    foreach (var item in models)
                    {
                        item.IsDel = true;
                        item.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(item);
                    }
                }
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除负责人信息服务异常。ids：{0}", JsonHelper.JsonSerializer(ids)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        /// <summary>
        /// 获取所有自提点
        /// </summary>
        /// <param name="appId">卖家id</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<AppSelfTakeStationResultDTO> GetAllAppSelfTakeStationExt(Guid appId, int pageSize, int pageIndex, out int rowCount)
        {
            List<AppSelfTakeStationResultDTO> result = new List<AppSelfTakeStationResultDTO>();
            if (appId == Guid.Empty)
            {
                rowCount = 0;
                return result;
            }
            var query = (from c in AppSelfTakeStation.ObjectSet()
                         where c.AppId == appId && c.IsDel == false
                         orderby c.SubTime descending
                         select new AppSelfTakeStationResultDTO
                             {
                                 Id = c.Id,
                                 AppId = c.AppId,
                                 Name = c.Name,
                                 Address = c.Address,
                                 Phone = c.Phone
                             }).ToList();
            rowCount = query.Count;
            result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); 
            return result;
        }
        /// <summary>
        /// 删除自提点
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStationExt(Guid id)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var selfStation = AppSelfTakeStation.ObjectSet().FirstOrDefault(n => n.Id == id);
                if (selfStation == null)
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                selfStation.ModifiedOn = DateTime.Now;
                selfStation.EntityState = System.Data.EntityState.Modified;
                selfStation.IsDel = true;
                contextSession.SaveChange();

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("删除自提点服务异常。id：{0}", id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }
        /// <summary>
        /// 根据条件查询所有自提点
        /// </summary>
        /// <param name="AppId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="Name"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> GetAllAppSelfTakeStationByWhereExt(Guid AppId, int pageSize, int pageIndex, out int rowCount, string Name, string province, string city, string district)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> result = new List<AppSelfTakeStationResultDTO>();
            if (AppId == Guid.Empty)
            {
                rowCount = 0;
                return result;
            }
            var query = (from c in AppSelfTakeStation.ObjectSet()
                         where c.AppId == AppId && c.IsDel == false
                         orderby c.SubTime descending
                         select new AppSelfTakeStationResultDTO
                         {
                             Id = c.Id,
                             AppId = c.AppId,
                             Name = c.Name,
                             Address = c.Address,
                             Phone = c.Phone,
                             Province = c.Province,
                             City = c.City,
                             District = c.District
                         }).ToList();
            if (!String.IsNullOrEmpty(Name))
            {
                query = query.Where(c => c.Name.Contains(Name)).ToList();
            }
            if (!String.IsNullOrEmpty(province) && province != "000000")
            {
                query = query.Where(c => c.Province == province).ToList();
            }
            if (!String.IsNullOrEmpty(city))
            {
                query = query.Where(c => c.City == city).ToList();
            }
            if (!String.IsNullOrEmpty(district))
            {
                query = query.Where(c => c.District == district).ToList();
            }
            
            rowCount = query.Count;
            result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); 
            return result;
        }
    }
}
