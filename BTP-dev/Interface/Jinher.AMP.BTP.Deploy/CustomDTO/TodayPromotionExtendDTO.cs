using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    //[KnownType(typeof(TodayPromotionDTO))]
    public class TodayPromotionExtendDTO : TodayPromotionDTO
    {
        /// <summary>
        /// 活动状态 0：没有活动或已失效 ,1:预约预售进行中，2：等待抢购：3：活动进行中，4：活动已结束
        /// </summary>
        [DataMember]
        public int PromotionState { get; set; } 

    }
}
