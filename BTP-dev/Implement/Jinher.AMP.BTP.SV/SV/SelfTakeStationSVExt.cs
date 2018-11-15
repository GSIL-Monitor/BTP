using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.SV
{
    public partial class SelfTakeStationSV : BaseSv, ISelfTakeStation
    {
        /// <summary>
        /// 查询自提点
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public SelfTakeStationResultDTO GetSelfTakeStationExt(SelfTakeStationSearchDTO search)
        {

            SelfTakeStationResultDTO result = new SelfTakeStationResultDTO();
            if (search == null || search.pageIndex < 1 || search.pageSize < 1 || search.appId == Guid.Empty)
            {
                return result;
            }

            var query = from s in AppSelfTakeStation.ObjectSet()
                        where !s.IsDel && s.AppId == search.appId
                        select s;

            if (!string.IsNullOrEmpty(search.searchContent))
            {
                query = query.Where(n => n.Name.Contains(search.searchContent) || n.Phone.Contains(search.searchContent) || n.Address.Contains(search.searchContent));
            }

            query = query.Distinct();
            result.Count = query.Count();

            query = query.OrderByDescending(n => n.SubTime).Skip((search.pageIndex - 1) * search.pageSize).Take(search.pageSize);

            var tmpResult = query.ToList();

            if (tmpResult.Count == 0)
            {
                result.Count = 0;
                return result;
            }
            try
            {
                foreach (var item in tmpResult)
                {
                    SelfTakeStationSearchResultDTO data = new SelfTakeStationSearchResultDTO
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Province = ProvinceCityHelper.GetAreaNameByCode(item.Province),
                            City = ProvinceCityHelper.GetAreaNameByCode(item.City),
                            District = ProvinceCityHelper.GetAreaNameByCode(item.District),
                            Address = item.Address,
                            SubTime = item.SubTime,
                            Phone = item.Phone == null ? "" : item.Phone,
                            ModifiedOn = item.ModifiedOn
                        };
                    result.SelfTakeStationList.Add(data);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询自提点SV服务异常。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                return result;
            }
        }
        /// <summary>
        /// 得到自提地址
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetSelfTakAddress(SelfTakeStation entity)
        {
            if (entity != null)
            {
                //if (ProvinceCityHelper.GetDirectlyCityList().Select(c => c.AreaCode).ToList().Contains(entity.Province))
                //{
                //    return string.Format("{0} {1}", ProvinceCityHelper.GetAreaNameByCode(entity.Province), entity.Address);
                //}
                return string.Format("{0} {1} {2}", ProvinceCityHelper.GetAreaNameByCode(entity.Province), ProvinceCityHelper.GetAreaNameByCode(entity.City), entity.Address);
            }
            return string.Empty;
        }
        /// <summary>
        /// 删除总代关联删除
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO DeleteCityOwnerExt(SelfTakeStationSearchDTO search)
        {
            if (search == null || search.CityOwnerId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var selfTakeStationList = SelfTakeStation.ObjectSet().Where(c => c.CityOwnerId == search.CityOwnerId && !c.IsDel).ToList();
                if (selfTakeStationList.Any())
                {
                    foreach (var selfTakeStation in selfTakeStationList)
                    {
                        selfTakeStation.IsDel = true;
                        selfTakeStation.EntityState = EntityState.Modified;
                    }
                    var selfTakeStationIdList = selfTakeStationList.Select(c => c.Id).ToList();
                    var selfTakeStationManagerList = SelfTakeStationManager.ObjectSet().Where(c => !c.IsDel && selfTakeStationIdList.Contains(c.SelfTakeStationId)).ToList();
                    if (selfTakeStationManagerList.Any())
                    {
                        foreach (var selfTakeStationManager in selfTakeStationManagerList)
                        {
                            selfTakeStationManager.IsDel = true;
                            selfTakeStationManager.EntityState = EntityState.Modified;
                        }
                    }
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除总代异常：search:{0}", JsonHelper.JsonSerializer(search)), ex);
                return new ResultDTO { ResultCode = 1, Message = "删除异常" };
            }
            return new ResultDTO { Message = "删除成功" };
        }
    }
}
