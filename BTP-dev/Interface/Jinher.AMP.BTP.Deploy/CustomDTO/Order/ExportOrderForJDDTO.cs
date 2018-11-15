using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class ExportOrderForJDDTO
    {
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        [DataMemberAttribute()]
        public string OrderCode { get; set; }
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }
        [DataMemberAttribute()]
        public string JdCodes { get; set; }
        [DataMemberAttribute()]
        public string ErQiCodes { get; set; }
        [DataMemberAttribute()]
        public string Prices { get; set; }
        [DataMemberAttribute()]
        public string Numbers { get; set; }
    }
}
