using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    [Serializable]
    [DataContract]
    public class SNComponentExport
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
