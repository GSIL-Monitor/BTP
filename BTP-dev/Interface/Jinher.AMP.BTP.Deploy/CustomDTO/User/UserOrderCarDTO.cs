using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    [Serializable]
    [DataContract]
    public class UserOrderCarDTO
    {
        //众筹每股金额
        [DataMember]
        public decimal PerShareMoney { get; set; }
        //用户已购买金额
        [DataMember]
        public decimal Money { get; set; }
        //用户已持有股数
        [DataMember]
        public long CurrentShareCount { get; set; }

        /// <summary>
        /// 是否是进行中的众筹 true为活动中 
        /// </summary>
        [DataMember]
        public bool IsActiveCrowdfunding { get; set; }

        /// <summary>
        /// 众筹可购买股数
        /// </summary>
         [DataMember]
        public long ShareCountRemain { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
         [DataMember]
         public Guid AppId { get; set; }

    }
}
