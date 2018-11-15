using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class SignUrlDTO
    {
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public string payeeId { get; set; }
        [DataMember]
        public string outTradeId { get; set; }
        [DataMember]
        public string money { get; set; }
        [DataMember]
        public string gold { get; set; }
        [DataMember]
        public string couponCount { get; set; }
        [DataMember]
        public string couponCodes { get; set; }
    }
}
