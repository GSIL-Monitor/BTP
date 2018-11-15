using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 自提点默认值
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppSelfTakeStationDefaultInfoDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid StationId { get; set; }
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMember]
        public string StationName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [DataMember]
        public string StationAddressDetails { get; set; }
        /// <summary>
        /// 自提点联系电话
        /// </summary>
        [DataMember]
        public string StationPhone { get; set; }
        /// <summary>
        /// 提货人姓名
        /// </summary>
        [DataMember]
        public string PickUpName { get; set; }
        /// <summary>
        /// 提货人联系方式
        /// </summary>
        [DataMember]
        public string PickUpPhone { get; set; }
        /// <summary>
        /// 下单后预约自提天数（目前固定为1天）
        /// </summary>
        [DataMember]
        public Int32 DelayDay { get; set; }
        /// <summary>
        /// 预约时间范围最大值（天），目前固定14天
        /// </summary>
        [DataMember]
        public Int32 MaxBookDay { get; set; }

        /// <summary>
        /// 提货时间列表
        /// </summary>
        [DataMember]
        public List<AppStationOfficeTime> StationTimeList { get; set; }
        /// <summary>
        /// 提货时间列表
        /// </summary>
        [DataMember]
        public AppSelfTakeTimeForOrder StationTimeShowList { get; set; }

    }

    /// <summary>
    /// 自提点默认值
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppStationOfficeTime
    {
        /// <summary>
        /// WeekDays
        /// </summary>
        [DataMember]
        public Int32 WeekDays { get; set; }
        /// <summary>
        /// StartTime
        /// </summary>
        [DataMember]
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// EndTime
        /// </summary>
        [DataMember]
        public TimeSpan EndTime { get; set; }

    }
}
