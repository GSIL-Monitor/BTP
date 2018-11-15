using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 交班时间
    /// </summary>
    [DataContract]
    [Serializable]
    public class FCateringShiftTimeCDTO
    {
        /// <summary>
        /// 交班时间
        /// </summary>
        [DataMember]
        public DateTime shiftTime { get; set; }
    }
}
