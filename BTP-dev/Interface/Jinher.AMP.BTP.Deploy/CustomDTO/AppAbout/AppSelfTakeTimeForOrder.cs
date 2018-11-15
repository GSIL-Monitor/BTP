using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 下订单页时间处理
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeTimeForOrder
    {
        /// <summary>
        /// 是否默认，当没有设置时采用此默认
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }
        /// <summary>
        /// 下拉时间
        /// </summary>
        [DataMember]
        public List<AppSelfTakeTimeForOrderDay> ScrollDayList { get; set; }
        /// <summary>
        /// 下拉时间
        /// </summary>
        [DataMember]
        public List<AppSelfTakeTimeForOrderWeek> ScrollWeekTimeList { get; set; }
    }

    /// <summary>
    /// 下订单页时间处理
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeTimeForOrderDay
    {
        /// <summary>
        /// 下拉日期
        /// </summary>
        [DataMember]
        public string ScrollDateText { get; set; }
        /// <summary>
        /// 下拉日期
        /// </summary>
        [DataMember]
        public DateTime ScrollDate { get; set; }
        /// <summary>
        /// 下拉日期值
        /// </summary>
        [DataMember]
        public string ScrollDateValue { get; set; }
        /// <summary>
        /// 周几
        /// </summary>
        [DataMember]
        public WeekDayEnum ScrollWeekDay { get; set; }
    }


    /// <summary>
    /// 下订单页时间处理
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeTimeForOrderWeek
    {
        /// <summary>
        /// 周几
        /// </summary>
        [DataMember]
        public WeekDayEnum ScrollWeekDay { get; set; }
        /// <summary>
        /// 时间列表
        /// </summary>
        [DataMember]
        public List<AppSelfTakeTimeForOrderTime> ScrollTimeList { get; set; }
        
    }
    /// <summary>
    /// 下订单页时间
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeTimeForOrderTime
    {
        /// <summary>
        /// 周几
        /// </summary>
        [DataMember]
        public WeekDayEnum ScrollWeekDay { get; set; }
        /// <summary>
        /// 下拉时间
        /// </summary>
        [DataMember]
        public string ScrollTimeText { get; set; }
        /// <summary>
        /// 下拉时间值
        /// </summary>
        [DataMember]
        public string ScrollTimeValue { get; set; }
        /// <summary>
        /// 下拉时间
        /// </summary>
        [DataMember]
        public string ScrollTime { get; set; }
    }
}
