using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class RuleDescriptionDTO
    {
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        [DataMemberAttribute()]
        public string Description { get; set; }
        [DataMemberAttribute()]
        public Guid appId { get; set; }
    }
}
