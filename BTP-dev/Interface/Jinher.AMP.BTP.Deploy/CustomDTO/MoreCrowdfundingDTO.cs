using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 更多众筹DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class MoreCrowdfundingDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 应用logo的Url
        /// </summary>
        [DataMember]
        public string AppLogoUrl { get; set; }

        /// <summary>
        /// 剩余股数
        /// </summary>
        [DataMember]
        public long ShareCountRemain { get; set; }

        /// <summary>
        /// 每股金额
        /// </summary>
        [DataMember]
        public decimal PerShareMoney { get; set; }
        /// <summary>
        /// 用户拥有股数
        /// </summary>
        [DataMember]
        public long UserShareCount { get; set; }
        /// <summary>
        /// 用户累计分红
        /// </summary>
        [DataMember]
        public decimal UserTotalDividend { get; set; }

        /// <summary>
        /// 达到下一股所需要金额
        /// </summary>
        [DataMember]
        public decimal NextMoney { get; set; }

        /// <summary>
        /// 下一股数
        /// </summary>
        [DataMember]
        public long NextShare
        {
            get { return UserShareCount + 1; }
        }

        ///// <summary>
        ///// 本活动总购买金额
        ///// </summary>
        // [DataMember]
        //public decimal Money { get; set; }
        /// <summary>
        ///用户购买标示
        /// </summary>
        [DataMember]
        public bool UserBuyFlag { get; set; }
    }
}
