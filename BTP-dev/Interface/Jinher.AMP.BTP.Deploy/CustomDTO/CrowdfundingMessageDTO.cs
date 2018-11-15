using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CrowdfundingMessageDTO
    {
        /// <summary>
        /// 当前时间
        /// </summary>
            [DataMember]
        public DateTime Now { get; set; }

        /// <summary>
        /// 众筹开始时间
        /// </summary>
           [DataMember]
        public DateTime StartTime { get; set; }


        /// <summary>
        /// 众筹状态
        /// </summary>
            [DataMember]
        public int State { get; set; }

    }
}
