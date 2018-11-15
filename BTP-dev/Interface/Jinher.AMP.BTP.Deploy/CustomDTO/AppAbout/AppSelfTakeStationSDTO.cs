using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// App自提点 BP
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSelfTakeStationSDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 省编码
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 城市编码
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 区县编码
        /// </summary>
        [DataMember]
        public string District { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 自提点联系电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
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
        /// IsDel
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// 自提点负责人列表
        /// </summary>
        [DataMember]
        public List<AppStsManagerSDTO> AppStsManagerList { get; set; }

        /// <summary>
        /// 自提点接待时间列表
        /// </summary>
        [DataMember]
        public List<AppStsOfficeTimeSDTO> AppStsOfficeTimeList { get; set; }
    }
}
