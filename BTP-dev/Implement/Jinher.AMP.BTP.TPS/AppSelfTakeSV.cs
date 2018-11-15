using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 处理时间段
    /// </summary>
    public class AppSelfTakeSV
    {
        /// <summary>
        /// 下单所需要时间
        /// </summary>
        /// <param name="list">时间列表</param>
        /// <param name="delayDay">下单后预约自提天数（目前固定为1天）</param>
        /// <param name="maxBookDay">预约时间范围最大值（天），目前固定14天</param>
        /// <returns></returns>
        public static AppSelfTakeTimeForOrder DealScrollTime(List<AppStationOfficeTime> list, int delayDay, int maxBookDay)
        {
            //没有自提点时，不管
            if (maxBookDay == 0)
            {
                return null;
            }
            var result =new AppSelfTakeTimeForOrder();
            var _now = DateTime.Now;
            var _nowZero = DateTime.Parse(_now.ToString("yyyy-MM-dd"));
            var startDate = _nowZero.AddDays(delayDay);
            var endDate = _nowZero.AddDays(delayDay + maxBookDay);
            result.ScrollDayList=new List<AppSelfTakeTimeForOrderDay>();
            result.ScrollWeekTimeList = new List<AppSelfTakeTimeForOrderWeek>();
           
            if (list == null || list.Count == 0)
            {
                result.IsDefault = true;
                for (int i = 0; i < maxBookDay; i++)
                {
                    var dateModel = new AppSelfTakeTimeForOrderDay();
                    dateModel.ScrollDateValue = startDate.ToString("yyyy年M月d日");
                    dateModel.ScrollDateText = string.Format("{0}[{1}]", startDate.ToString("M月d日"), startDate.ToString("dddd", new System.Globalization.CultureInfo("zh-CN")));
                    dateModel.ScrollWeekDay = ConvertToStationWeek(startDate.DayOfWeek);
                    dateModel.ScrollDate = startDate;
                    result.ScrollDayList.Add(dateModel);
                    startDate = startDate.AddDays(1);
                }
                AppSelfTakeTimeForOrderWeek weekModel = new AppSelfTakeTimeForOrderWeek();
                weekModel.ScrollWeekDay = WeekDayEnum.None;
                weekModel.ScrollTimeList = new List<AppSelfTakeTimeForOrderTime>();
                for (var j = 0; j < 12; j++)
                {
                    var timeModel = new AppSelfTakeTimeForOrderTime();
                    var timeStart = j * 2;
                    var timeEnd = (j + 1) * 2;
                    var timeStartText = string.Empty;
                    var timeEndText = string.Empty;
                    if (timeStart < 10)
                    {
                        timeStartText = "0" + timeStart;
                    }
                    else
                    {
                        timeStartText = timeStart.ToString();
                    }
                    if (timeEnd < 10)
                    {
                        timeEndText = "0" + timeEnd;
                    }
                    else
                    {
                        timeEndText = timeEnd.ToString();
                    }
                    timeModel.ScrollTimeText = string.Format("{0}:00～{1}:00", timeStartText, timeEndText);
                    timeModel.ScrollTimeValue = string.Format("{0}:00～{1}:00", timeStartText, timeEndText);
                    timeModel.ScrollTime = string.Format("{0}:00|{1}:00", timeStartText, timeEndText);
                    timeModel.ScrollWeekDay = WeekDayEnum.None;
                    weekModel.ScrollTimeList.Add(timeModel);
                }
                result.ScrollWeekTimeList.Add(weekModel);
            }
            else
            {
                int weekDays = 0;
                list.ForEach(delegate(AppStationOfficeTime item) { weekDays = (weekDays | item.WeekDays); });
                for (int i = 0; i < maxBookDay; i++)
                {
                    var dayOfWeek =(int) ConvertToStationWeek(startDate.DayOfWeek);
                    if ((dayOfWeek & weekDays) == dayOfWeek)
                    {
                        var dateModel = new AppSelfTakeTimeForOrderDay();
                        dateModel.ScrollDateValue = startDate.ToString("yyyy年M月d日");
                        dateModel.ScrollDateText = string.Format("{0}[{1}]", startDate.ToString("M月d日"), startDate.ToString("dddd", new System.Globalization.CultureInfo("zh-CN")));
                        dateModel.ScrollWeekDay = ConvertToStationWeek(startDate.DayOfWeek);
                        dateModel.ScrollDate = startDate;
                        result.ScrollDayList.Add(dateModel);
                    }
                    startDate = startDate.AddDays(1);
                }
                foreach (WeekDayEnum weekDayEnum in Enum.GetValues(typeof(WeekDayEnum)))
                {
                    if (weekDayEnum == WeekDayEnum.None)
                    {
                        continue;
                    }
                    if ((weekDays & (int)weekDayEnum) == (int)weekDayEnum)
                    {
                        AppSelfTakeTimeForOrderWeek weekModel = new AppSelfTakeTimeForOrderWeek();
                        weekModel.ScrollWeekDay = weekDayEnum;
                        weekModel.ScrollTimeList = new List<AppSelfTakeTimeForOrderTime>();
                        var listTmp =
                            list.Where(t => (t.WeekDays & (int)weekDayEnum) == (int)weekDayEnum)
                                .OrderBy(t => t.StartTime)
                                .ToList();
                        foreach (var item in listTmp)
                        {
                            var timeModel = new AppSelfTakeTimeForOrderTime();

                            var timeStartText = item.StartTime.ToString(@"hh\:mm");
                            var timeEndText = item.EndTime.ToString(@"hh\:mm"); ;
                            timeModel.ScrollTimeText = string.Format("{0}～{1}", timeStartText, timeEndText);
                            timeModel.ScrollTimeValue = string.Format("{0}～{1}", timeStartText, timeEndText);
                            timeModel.ScrollTime = string.Format("{0}|{1}", timeStartText.Replace("：", ":"), timeEndText.Replace("：", ":"));
                            timeModel.ScrollWeekDay = weekDayEnum;
                            weekModel.ScrollTimeList.Add(timeModel);
                        }
                        result.ScrollWeekTimeList.Add(weekModel);
                    }
                }  
            }

            return result;
        }

        public static WeekDayEnum ConvertToStationWeek(DayOfWeek dayOfWeek)
        {
            WeekDayEnum result = WeekDayEnum.None;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = WeekDayEnum.Sunday;
                    break;
                case DayOfWeek.Monday:
                    result = WeekDayEnum.Monday;
                    break;
                case DayOfWeek.Tuesday:
                    result = WeekDayEnum.Tuesday;
                    break;
                case DayOfWeek.Wednesday:
                    result = WeekDayEnum.Wednesday;
                    break;
                case DayOfWeek.Thursday:
                    result = WeekDayEnum.Thursday;
                    break;
                case DayOfWeek.Friday:
                    result = WeekDayEnum.Friday;
                    break;
                case DayOfWeek.Saturday:
                    result = WeekDayEnum.Saturday;
                    break;
                default:
                    result = WeekDayEnum.None;
                    break;
            }
            return result;
        }
    }
}
