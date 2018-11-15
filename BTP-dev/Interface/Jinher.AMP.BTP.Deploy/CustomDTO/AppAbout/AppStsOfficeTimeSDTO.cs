using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// App自提点接待时间 BP
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppStsOfficeTimeSDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
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
        /// <summary>
        /// SelfTakeStationId
        /// </summary>
        [DataMember]
        public Guid SelfTakeStationId { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
	}
}