using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YingKe
{
    /// <summary>
    /// 盈科获取电子券
    /// </summary>
    [Serializable]
    [DataContract]
    public class YingKeCouponDTO
    {
        [DataMember]
        public string rechargePwd { get; set; }

        [DataMember]
        public string preDetailPath { get; set; }

        [DataMember]
        public string presentName { get; set; }

        [DataMember]
        public string endTime { get; set; }
    }
}
