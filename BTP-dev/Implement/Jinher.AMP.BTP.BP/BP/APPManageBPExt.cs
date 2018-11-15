
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using System.Data;
using Jinher.JAP.Cache;

namespace Jinher.AMP.BTP.BP
{
    public partial class APPManageBP : BaseBP, IAPPManage
    {

        public ResultDTO AddAPPManageExt(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            ResultDTO result = new ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                //检查添加是否存在
                var query = APPManage.ObjectSet().Where(q => q.AppId == AppManageDTO.AppId).FirstOrDefault();

                if (query != null)
                {
                    result.ResultCode = 1;
                    result.Message = "此应用已经存在";
                    return result;
                }
                else
                {


                    APPManage appManage = new APPManage();
                    appManage.Id = AppManageDTO.Id;
                    appManage.AppId = AppManageDTO.AppId;
                    appManage.AppName = AppManageDTO.AppName;
                    appManage.Remark = AppManageDTO.Remark;
                    appManage.SubTime = AppManageDTO.SubTime;
                    appManage.SubId = AppManageDTO.SubId;
                    appManage.ModifiedOn = AppManageDTO.ModifiedOn;
                    appManage.ModifiedId = AppManageDTO.ModifiedId;

                    appManage.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(appManage);
                    int num = contextSession.SaveChanges();

                    if (num > 0)
                    {
                        GlobalCacheWrapper.Remove(RedisKeyConst.AppInZPH, AppManageDTO.AppId.ToString(), CacheTypeEnum.redisSS, "BTPCache");
                        result.ResultCode = 0;
                        result.Message = "添加成功";
                        return result;
                    }
                    else
                    {
                        result.ResultCode = 1;
                        result.Message = "添加失败";
                        return result;
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加AppManage异常。AppManageDTO：{0}", JsonHelper.JsonSerializer(AppManageDTO)), ex);
                result.ResultCode = 1;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultDTO UpdateAPPManageExt(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO)
        {
            ResultDTO result = new ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //原来的appid
            string oldAppId = string.Empty;
            try
            {
                //检查添加是否存在
                var query = APPManage.ObjectSet().Where(q => q.Id != AppManageDTO.Id && q.AppId == AppManageDTO.AppId).FirstOrDefault();

                if (query != null)
                {
                    result.ResultCode = 1;
                    result.Message = "此应用已经存在";
                    return result;
                }


                var appManage = APPManage.ObjectSet().Where(q => q.Id == AppManageDTO.Id).FirstOrDefault();
                oldAppId = appManage.AppId.ToString();


                if (appManage != null)
                {
                    appManage.Id = AppManageDTO.Id;
                    appManage.AppId = AppManageDTO.AppId;
                    appManage.AppName = AppManageDTO.AppName;
                    appManage.Remark = AppManageDTO.Remark;
                    appManage.SubTime = AppManageDTO.SubTime;
                    appManage.SubId = ContextDTO.LoginUserID;
                    appManage.ModifiedOn = AppManageDTO.ModifiedOn;
                    appManage.ModifiedId = AppManageDTO.ModifiedId;

                    appManage.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(appManage);
                    int num = contextSession.SaveChanges();

                    if (num > 0)
                    {
                        //如果没有修改appid
                        if (oldAppId == AppManageDTO.AppId.ToString())
                        {
                            GlobalCacheWrapper.Remove(RedisKeyConst.AppInZPH, AppManageDTO.AppId.ToString(), CacheTypeEnum.redisSS, "BTPCache");
                        }
                        //如果修改了appid,则2个都要删除
                        else
                        {
                            GlobalCacheWrapper.Remove(RedisKeyConst.AppInZPH, AppManageDTO.AppId.ToString(), CacheTypeEnum.redisSS, "BTPCache");
                            GlobalCacheWrapper.Remove(RedisKeyConst.AppInZPH, oldAppId, CacheTypeEnum.redisSS, "BTPCache");
                        }
                        result.ResultCode = 0;
                        result.Message = "修改成功";
                        return result;

                    }
                    else
                    {
                        result.ResultCode = 1;
                        result.Message = "修改失败";
                        return result;
                    }
                }
                else
                {
                    result.ResultCode = 0;
                    result.Message = "Deled";
                    return result;
                }




            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改AppManage异常。AppManageDTO：{0}", JsonHelper.JsonSerializer(AppManageDTO)), ex);
                result.ResultCode = 1;
                result.Message = ex.Message;
                return result;
            }


        }
        public ResultDTO DelAPPManageExt(Guid Id)
        {


            ResultDTO result = new ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                //检查添加是否存在
                var query = APPManage.ObjectSet().Where(q => q.Id == Id).FirstOrDefault();

                if (query == null)
                {
                    result.ResultCode = 1;
                    result.Message = "不存在删除的记录";
                    return result;

                }
                else
                {
                    query.EntityState = System.Data.EntityState.Deleted;
                    contextSession.SaveObject(query);
                    int num = contextSession.SaveChanges();

                    if (num > 0)
                    {
                        GlobalCacheWrapper.Remove(RedisKeyConst.AppInZPH, query.AppId.ToString(), CacheTypeEnum.redisSS, "BTPCache");
                        result.ResultCode = 0;
                        result.Message = "删除成功";
                        return result;

                    }
                    else
                    {
                        result.ResultCode = 1;
                        result.Message = "删除失败";
                        return result;
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除AppManage异常。。Id：{0}", Id), ex);
                result.ResultCode = 1;
                result.Message = ex.Message;
                return result;
            }


        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO> GetAPPManageListExt()
        {

            List<Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO> APPManageDTOs = new List<Deploy.CustomDTO.APPManageDTO>();


            try
            {

                var query = APPManage.ObjectSet().OrderBy(q => q.SubTime).ToList();

                foreach (var item in query)
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO = new Deploy.CustomDTO.APPManageDTO();

                    AppManageDTO.Id = item.Id;
                    AppManageDTO.AppName = item.AppName;
                    AppManageDTO.AppId = item.AppId;
                    AppManageDTO.Remark = item.Remark;
                    AppManageDTO.SubId = item.SubId;
                    AppManageDTO.SubTime = item.SubTime;
                    AppManageDTO.ModifiedOn = item.ModifiedOn;
                    AppManageDTO.ModifiedId = item.ModifiedId;
                    APPManageDTOs.Add(AppManageDTO);
                }

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("获取AppManage异常。"), ex);
                return null;
            }

            return APPManageDTOs;

        }

        /// <summary>
        /// 过滤非法应用
        /// </summary>
        /// <returns></returns>
        public ResultDTO ForbitAppExt()
        {
            try
            {
                var deleteCount = 0;
                var updateCount = 0;
                var appIdList = new List<Guid>();
                foreach (var item in AppSet.ObjectSet())
                {
                    var app = APPSV.Instance.GetNewAppById(item.AppId.Value);
                    if (app == null) continue;
                    if (app.Illegal == 1)
                    {
                        item.EntityState = EntityState.Deleted;
                        ContextFactory.CurrentThreadContext.SaveObject(item);
                        appIdList.Add(item.AppId.Value);
                        deleteCount++;
                    }
                    else if (app.Illegal == 0)
                    {
                        if (item.AppName != app.Name || item.AppIcon != app.Icon)
                        {
                            item.AppName = app.Name;
                            item.AppIcon = app.Icon;
                            item.EntityState = EntityState.Modified;
                            ContextFactory.CurrentThreadContext.SaveObject(item);
                            updateCount++;
                        }
                    }
                }

                var comIds = Commodity.ObjectSet()
                    .Where(e => appIdList.Contains(e.AppId) && e.CommodityType == 0)
                    .Select(e => e.Id)
                    .ToList();
                var query = SetCommodityCategory.ObjectSet().Where(e => comIds.Contains(e.CommodityId));
                foreach (var item in query)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                ContextFactory.CurrentThreadContext.SaveChanges();
                var retMsg = string.Format("共有{0}个App被删除，共有{1}个App被修改", deleteCount, updateCount);
                return new ResultDTO { ResultCode = 0, Message = retMsg };
            }
            catch (Exception ex)
            {
                LogHelper.Error("AppManageBP-ForbitAppExt", ex);
                return new ResultDTO { ResultCode = -1, Message = ex.Message };
            }
        }
    }
}
