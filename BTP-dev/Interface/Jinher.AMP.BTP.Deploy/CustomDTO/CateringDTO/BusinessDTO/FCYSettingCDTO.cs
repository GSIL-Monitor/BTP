using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class FCYSettingCDTO
    {
        /// <summary>
        /// 门店设置
        /// </summary>
        [DataMember]
        public FCateringSettingCDTO CateringSetting { get; set; }
        /// <summary>
        /// 营业时间
        /// </summary>
        [DataMember]
        public List<FCateringBusinessHoursCDTO> CYBusinessHours { get; set; }
        /// <summary>
        /// 交班时间
        /// </summary>
        [DataMember]
        public List<FCateringShiftTimeCDTO> CYShiftTime { get; set; }
    }
}
