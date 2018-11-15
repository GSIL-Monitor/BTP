
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/18 15:33:02
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AppSelfTakeStationSV : BaseSv, IAppSelfTakeStation
    {

        /// <summary>
        /// 下订单页获取自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationDefaultInfoDTO GetAppSelfTakeStationDefaultExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchDTO search)
        {
            var result = new AppSelfTakeStationDefaultInfoDTO();

            if (search == null)
            {
                return null;
            }
            if (search.SearchType == 1)
            {
                if (search.Id == Guid.Empty)
                {
                    return null;
                }
                var station =
                   AppSelfTakeStation.ObjectSet()
                                     .Where(t => t.Id == search.Id && !t.IsDel)
                                     .FirstOrDefault();

                if (station == null)
                {
                    return null;
                }
                result.StationId = station.Id;
                result.StationName = station.Name;
                result.DelayDay = station.DelayDay;
                result.MaxBookDay = station.MaxBookDay;
                result.StationPhone = station.Phone;

                if (!string.IsNullOrWhiteSpace(station.Province))
                {
                    result.StationAddressDetails = ProvinceCityHelper.GetAreaNameByCode(station.Province) +
                                                   ProvinceCityHelper.GetAreaNameByCode(station.City) +
                                                   ProvinceCityHelper.GetAreaNameByCode(station.District) +
                                                   station.Address;
                }
                var officeTime =
                     AppStsOfficeTime.ObjectSet().Where(t => t.SelfTakeStationId == station.Id).ToList();
                if (officeTime.Any())
                {
                    result.StationTimeList=new List<AppStationOfficeTime>();
                    foreach (var appStsOfficeTime in officeTime)
                    {
                        var officeTimeModel = new AppStationOfficeTime();
                        officeTimeModel.StartTime = appStsOfficeTime.StartTime;
                        officeTimeModel.EndTime = appStsOfficeTime.EndTime;
                        officeTimeModel.WeekDays = appStsOfficeTime.WeekDays;
                        result.StationTimeList.Add(officeTimeModel);
                    }
                }
            }
            else if (search.SearchType == 2)
            {
                if (search.EsAppId == Guid.Empty)
                {
                    return null;
                }
                var _userId = this.ContextDTO.LoginUserID;
                var pickUpOrder =
                    AppOrderPickUp.ObjectSet()
                                  .Where(t => t.AppId == search.EsAppId && t.UserId == _userId).OrderByDescending(t => t.SubTime)
                                  .FirstOrDefault();

                if (pickUpOrder == null)
                {
                    return null;
                }
                var station =
                    AppSelfTakeStation.ObjectSet()
                                      .Where(t => t.Id == pickUpOrder.SelfTakeStationId && t.AppId == pickUpOrder.AppId && !t.IsDel)
                                      .FirstOrDefault();

                if (station == null)
                {
                    return null;
                }

                result.StationId = station.Id;
                result.StationName = station.Name;
                result.DelayDay = station.DelayDay;
                result.MaxBookDay = station.MaxBookDay;
                result.StationPhone = station.Phone;
                result.PickUpName = pickUpOrder.Name;
                result.PickUpPhone = pickUpOrder.Phone;

                if (!string.IsNullOrWhiteSpace(station.Province))
                {
                    result.StationAddressDetails = ProvinceCityHelper.GetAreaNameByCode(station.Province) +
                                                   ProvinceCityHelper.GetAreaNameByCode(station.City) +
                                                   ProvinceCityHelper.GetAreaNameByCode(station.District) +
                                                   station.Address;
                }
                var officeTime =
                      AppStsOfficeTime.ObjectSet().Where(t => t.SelfTakeStationId == station.Id).ToList();
                if (officeTime.Any())
                {
                    result.StationTimeList = new List<AppStationOfficeTime>();
                    foreach (var appStsOfficeTime in officeTime)
                    {
                        var officeTimeModel = new AppStationOfficeTime();
                        officeTimeModel.StartTime = appStsOfficeTime.StartTime;
                        officeTimeModel.EndTime = appStsOfficeTime.EndTime;
                        officeTimeModel.WeekDays = appStsOfficeTime.WeekDays;
                        result.StationTimeList.Add(officeTimeModel);
                    }
                }
            }
            else
            {
                return null;
            }
            result.StationTimeShowList = AppSelfTakeSV.DealScrollTime(result.StationTimeList, result.DelayDay, result.MaxBookDay);
            return result;
        }
    }
}