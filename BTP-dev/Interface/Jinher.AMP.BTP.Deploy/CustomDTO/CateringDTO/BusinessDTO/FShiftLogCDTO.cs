﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 交班日志
    /// </summary>
    [DataContract]
    [Serializable]
    public class FShiftLogCDTO
    {
        /// <summary>
        /// 交班用户Id
        /// </summary>
        [DataMember]
        public Guid userId { get; set; }
        /// <summary>
        /// 交班时间
        /// </summary>
        [DataMember]
        public DateTime shiftTime { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        [DataMember]
        public Guid storeId { get; set; }
    }
}
