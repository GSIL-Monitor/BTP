using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
     public class CateNameDTO
    {
         [DataMemberAttribute()]
         public string name { get; set; }
         [DataMemberAttribute()]
         public Guid CommodityId { get; set; }
    }
}
