using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class CostScoreInfoDTO
    {
        /// <summary>
        /// 是否通用积分
        /// </summary>
        [DataMember]
        public ScoreTypeEnum ScoreType { get; set; }
        /// <summary>
        /// 积分汇率
        /// </summary>
        [DataMember]
        public int Cost { get; set; }



    }

}
