using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 省份LIST
    /// </summary>
    [Serializable()]
    [DataContract]
   public class ProvinceSDTO
    {
       /// <summary>
       /// 省份名称
       /// </summary>
       [DataMemberAttribute()]
       public string Province { get; set; }
       /// <summary>
       /// 是否默认---默认=true，非默认=false
       /// </summary>
       [DataMemberAttribute()]
       public bool IsDefault { get; set; }
    }
}
