using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用户在{appId}应用中的积分信息。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UserScoreDTO
    { 
        /// <summary>
        /// 积分
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// 兑换比例:积分/ScoreCost=人民币
        /// </summary>
        [DataMember]
        public int ScoreCost { get; set; }

        /// <summary>
        /// 积分可兑换的钱数（单位：元）
        /// </summary>
        [DataMember]
        public decimal Money { get; set; }

        /// <summary>
        /// 是否启用了积分抵现
        /// </summary>
        [DataMember]
        public bool IsCashForScore { get; set; }
    }
}
