
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/9/12 14:15:04
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
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AppSelfTakeStationBP : BaseBP, IAppSelfTakeStation
    {

        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppSelfTakeStationExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Province) || string.IsNullOrWhiteSpace(model.City) || string.IsNullOrWhiteSpace(model.Address))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不参为空" };
            }
            //if (string.IsNullOrWhiteSpace(model.District))
            //{
            //    return new ResultDTO { ResultCode = 1, Message = "参数不参为空" };
            //}

            try
            {
                var userId = this.ContextDTO.LoginUserID;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                AppSelfTakeStation appSelfTakeStation = new AppSelfTakeStation();
                appSelfTakeStation.Id = Guid.NewGuid();
                appSelfTakeStation.AppId = model.AppId;
                appSelfTakeStation.Name = model.Name;
                appSelfTakeStation.Province = model.Province;
                appSelfTakeStation.City = model.City;
                appSelfTakeStation.District = string.IsNullOrWhiteSpace(model.District) ? "" : model.District;
                appSelfTakeStation.Address = model.Address;
                appSelfTakeStation.Phone = model.Phone;
                appSelfTakeStation.DelayDay = model.DelayDay;
                appSelfTakeStation.MaxBookDay = model.MaxBookDay;
                appSelfTakeStation.SubTime = DateTime.Now;
                appSelfTakeStation.ModifiedOn = DateTime.Now;
                appSelfTakeStation.IsDel = false;
                appSelfTakeStation.SubId = userId;

                appSelfTakeStation.EntityState = EntityState.Added;
                contextSession.SaveObject(appSelfTakeStation);

                if (model.AppStsOfficeTimeList != null && model.AppStsOfficeTimeList.Count > 0)
                {
                    foreach (var item in model.AppStsOfficeTimeList)
                    {
                        AppStsOfficeTime appStsOfficeTime = new AppStsOfficeTime();
                        appStsOfficeTime.Id = Guid.NewGuid();
                        appStsOfficeTime.SubTime = DateTime.Now;
                        appStsOfficeTime.ModifiedOn = DateTime.Now;
                        appStsOfficeTime.WeekDays = item.WeekDays;
                        appStsOfficeTime.StartTime = item.StartTime;
                        appStsOfficeTime.EndTime = item.EndTime;
                        appStsOfficeTime.SelfTakeStationId = appSelfTakeStation.Id;
                        appStsOfficeTime.SubId = userId;

                        appStsOfficeTime.EntityState = EntityState.Added;
                        contextSession.SaveObject(appStsOfficeTime);
                    }
                }
                if (model.AppStsManagerList != null && model.AppStsManagerList.Count > 0)
                {
                    var userIds = model.AppStsManagerList.Select(t => t.UserId).ToList();
                    var userIdsDisCount = userIds.Distinct().Count();
                    if (userIdsDisCount < userIds.Count())
                    {
                        return new ResultDTO { ResultCode = 2, Message = "不能填加重复的负责人" };
                    }
                    var countExits =
                        AppStsManager.ObjectSet().Where(t => t.AppId == model.AppId && userIds.Contains(t.UserId) && !t.IsDel).ToList().Count();
                    if (countExits > 0)
                    {
                        return new ResultDTO { ResultCode = 2, Message = "联系人已存在" };
                    }

                    foreach (var item in model.AppStsManagerList)
                    {
                        AppStsManager appStsManager = new AppStsManager();
                        appStsManager.Id = Guid.NewGuid();
                        appStsManager.SubTime = DateTime.Now;
                        appStsManager.ModifiedOn = DateTime.Now;
                        appStsManager.UserCode = item.UserCode;
                        appStsManager.UserId = item.UserId;
                        appStsManager.SelfTakeStationId = appSelfTakeStation.Id;
                        appStsManager.IsDel = false;
                        appStsManager.AppId = model.AppId;

                        appStsManager.EntityState = EntityState.Added;
                        contextSession.SaveObject(appStsManager);
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SaveAppSelfTakeStationExt接口错误。model:{0}", JsonHelper.JsonSerializer(model)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppSelfTakeStationExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model)
        {
            if (model == null || model.Id == Guid.Empty || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Province) || string.IsNullOrWhiteSpace(model.City) || string.IsNullOrWhiteSpace(model.Address))
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不参为空" };
            }
            //if (string.IsNullOrWhiteSpace(model.District))
            //{
            //    return new ResultDTO { ResultCode = 1, Message = "参数不参为空" };
            //}

            try
            {
                var modelExits = AppSelfTakeStation.ObjectSet().Where(t => t.Id == model.Id).FirstOrDefault();
                if (modelExits == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "自提点不存在" };
                }

                var userId = this.ContextDTO.LoginUserID;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                modelExits.Name = model.Name;
                modelExits.Province = model.Province;
                modelExits.City = model.City;
                modelExits.District = string.IsNullOrWhiteSpace(model.District) ? "" : model.District;
                modelExits.Address = model.Address;
                modelExits.Phone = model.Phone;
                modelExits.DelayDay = model.DelayDay;
                modelExits.MaxBookDay = model.MaxBookDay;
                modelExits.ModifiedOn = DateTime.Now;

                modelExits.EntityState = EntityState.Modified;


                var appStsOfficeTimeList =
                    AppStsOfficeTime.ObjectSet().Where(t => t.SelfTakeStationId == model.Id).ToList();
                if (appStsOfficeTimeList.Any())
                {
                    foreach (var appStsOfficeTime in appStsOfficeTimeList)
                    {
                        appStsOfficeTime.EntityState = EntityState.Deleted;
                    }
                }
                if (model.AppStsOfficeTimeList != null && model.AppStsOfficeTimeList.Count > 0)
                {
                    foreach (var item in model.AppStsOfficeTimeList)
                    {
                        AppStsOfficeTime appStsOfficeTime = new AppStsOfficeTime();
                        appStsOfficeTime.Id = Guid.NewGuid();
                        appStsOfficeTime.SubTime = DateTime.Now;
                        appStsOfficeTime.ModifiedOn = DateTime.Now;
                        appStsOfficeTime.WeekDays = item.WeekDays;
                        appStsOfficeTime.StartTime = item.StartTime;
                        appStsOfficeTime.EndTime = item.EndTime;
                        appStsOfficeTime.SelfTakeStationId = modelExits.Id;
                        appStsOfficeTime.SubId = userId;

                        appStsOfficeTime.EntityState = EntityState.Added;
                        contextSession.SaveObject(appStsOfficeTime);
                    }
                }
                if (model.AppStsManagerList != null && model.AppStsManagerList.Count > 0)
                {
                    var userIds = model.AppStsManagerList.Select(t => t.UserId).ToList();
                    var userIdsDisCount = userIds.Distinct().Count();
                    if (userIdsDisCount < userIds.Count())
                    {
                        return new ResultDTO { ResultCode = 2, Message = "不能填加重复的负责人" };
                    }
                    var appStsManagerListThis = AppStsManager.ObjectSet()
                                     .Where(t => t.SelfTakeStationId == model.Id && !t.IsDel)
                                     .ToList();

                    var addUserIdList = new List<Guid>();
                    var deleteUserIdList = new List<Guid>();
                    var updateUserIdList = new List<Guid>();

                    if (appStsManagerListThis.Any())
                    {
                        var oldUserIdList = appStsManagerListThis.Select(t => t.UserId).ToList();
                        addUserIdList = userIds.Except(oldUserIdList).ToList();
                        deleteUserIdList = oldUserIdList.Except(userIds).ToList();
                        updateUserIdList = userIds.Intersect(oldUserIdList).ToList();
                    }
                    else
                    {
                        addUserIdList = model.AppStsManagerList.Select(t => t.UserId).ToList();
                    }

                    var appStsManagerList =
                        AppStsManager.ObjectSet()
                                     .Where(t => t.AppId == model.AppId && addUserIdList.Contains(t.UserId) && !t.IsDel)
                                     .ToList();
                    if (appStsManagerList.Any())
                    {
                        return new ResultDTO { ResultCode = 2, Message = "联系人已存在" };
                    }

                    if (addUserIdList.Any())
                    {
                        var dealList =
                            model.AppStsManagerList.Where(t => addUserIdList.Contains(t.UserId)).ToList();
                        foreach (var item in dealList)
                        {
                            AppStsManager appStsManager = new AppStsManager();
                            appStsManager.Id = Guid.NewGuid();
                            appStsManager.SubTime = DateTime.Now;
                            appStsManager.ModifiedOn = DateTime.Now;
                            appStsManager.UserCode = item.UserCode;
                            appStsManager.UserId = item.UserId;
                            appStsManager.SelfTakeStationId = model.Id;
                            appStsManager.IsDel = false;
                            appStsManager.AppId = model.AppId;

                            appStsManager.EntityState = EntityState.Added;
                            contextSession.SaveObject(appStsManager);
                        }
                    }
                    if (updateUserIdList.Any())
                    {
                        var dealList =
                            model.AppStsManagerList.Where(t => updateUserIdList.Contains(t.UserId)).ToList();
                        foreach (var item in dealList)
                        {
                            var appStsManager =
                                appStsManagerListThis.Where(t => t.UserId == item.UserId).FirstOrDefault();
                            if (appStsManager.UserCode == item.UserCode)
                            {
                                continue;
                            }
                            appStsManager.ModifiedOn = DateTime.Now;
                            appStsManager.UserCode = item.UserCode;
                            appStsManager.EntityState = EntityState.Modified;
                        }
                    }
                    if (deleteUserIdList.Any())
                    {
                        var dealList =
                            appStsManagerListThis.Where(t => deleteUserIdList.Contains(t.UserId)).ToList();
                        foreach (var item in dealList)
                        {
                            var appStsManager =
                                appStsManagerListThis.Where(t => t.UserId == item.UserId).FirstOrDefault();
                            if (appStsManager == null)
                            {
                                continue;
                            }
                            appStsManager.ModifiedOn = DateTime.Now;
                            appStsManager.IsDel = true;
                            appStsManager.EntityState = EntityState.Modified;
                        }
                    }

                }
                else
                {
                    var appStsManagerList =
                       AppStsManager.ObjectSet()
                                    .Where(t => t.SelfTakeStationId == model.Id && !t.IsDel).ToList();
                    if (appStsManagerList.Any())
                    {
                        foreach (var item in appStsManagerList)
                        {
                            item.ModifiedOn = DateTime.Now;
                            item.IsDel = true;
                            item.EntityState = EntityState.Modified;
                        }
                    }
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SaveAppSelfTakeStationExt接口错误。model:{0}", JsonHelper.JsonSerializer(model)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStationsExt(System.Collections.Generic.List<System.Guid> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 查询自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchResultSDTO GetAppSelfTakeStationListExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchSDTO search)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO GetAppSelfTakeStationByIdExt(System.Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var query = AppSelfTakeStation.ObjectSet().Where(t => t.Id == id && !t.IsDel).FirstOrDefault();

            if (query == null)
            {
                return null;
            }

            Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO();
            result.Id = query.Id;
            result.AppId = query.AppId;
            result.Name = query.Name;
            result.Province = query.Province;
            result.City = query.City;
            result.District = query.District;
            result.Address = query.Address;
            result.Phone = query.Phone;
            result.DelayDay = query.DelayDay;
            result.MaxBookDay = query.MaxBookDay;
            result.SubTime = query.SubTime;
            result.ModifiedOn = query.ModifiedOn;
            result.IsDel = query.IsDel;
            result.SubId = query.SubId;
            var selfManager = (from appStsManager in AppStsManager.ObjectSet()
                               where appStsManager.SelfTakeStationId == id && !appStsManager.IsDel
                               select new Jinher.AMP.BTP.Deploy.CustomDTO.AppStsManagerSDTO
                               {
                                   Id = appStsManager.Id,
                                   SubTime = appStsManager.SubTime,
                                   ModifiedOn = appStsManager.ModifiedOn,
                                   UserCode = appStsManager.UserCode,
                                   UserId = appStsManager.UserId,
                                   SelfTakeStationId = appStsManager.SelfTakeStationId,
                                   AppId = appStsManager.AppId
                               }).ToList();
            result.AppStsManagerList = selfManager;
            var selfTime = (from appStsOfficeTime in AppStsOfficeTime.ObjectSet()
                            where appStsOfficeTime.SelfTakeStationId == id
                            select new Jinher.AMP.BTP.Deploy.CustomDTO.AppStsOfficeTimeSDTO
                               {
                                   Id = appStsOfficeTime.Id,
                                   SubTime = appStsOfficeTime.SubTime,
                                   ModifiedOn = appStsOfficeTime.ModifiedOn,
                                   WeekDays = appStsOfficeTime.WeekDays,
                                   StartTime = appStsOfficeTime.StartTime,
                                   EndTime = appStsOfficeTime.EndTime,
                                   SelfTakeStationId = appStsOfficeTime.SelfTakeStationId,
                                   SubId = appStsOfficeTime.SubId
                               }).ToList();

            result.AppStsOfficeTimeList = selfTime;

            return result;
        }
        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckUserIdExistsExt(System.Guid userId, System.Guid appId)
        {
            if (userId == Guid.Empty || appId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不参为空" };
            }
            var result =
                AppStsManager.ObjectSet().Where(t => t.AppId == appId && t.UserId == userId && !t.IsDel).Count();
            if (result == 0)
            {
                return new ResultDTO { ResultCode = 0, Message = "该账号尚未注册，请及时注册，以免影响正常使用！" };
            }
            else
            {
                return new ResultDTO { ResultCode = 2, Message = "该负责人已存在" };
            }
        }
    }
}