using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 营业时间
    /// </summary>
    [DataContract]
    [Serializable]
    public class FCateringBusinessHoursCDTO
    {
        /// <summary>
        /// 开店时间
        /// </summary>
        [DataMember]
        public DateTime openingTime { get; set; }
        /// <summary>
        /// 打烊时间
        /// </summary>
        [DataMember]
        public DateTime closingTime { get; set; }
    }
}
