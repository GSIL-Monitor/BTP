using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CommodityPriceFloatListDto : CommodityPriceFloatDTO
    {
        public string AppNames { get; set; }
    }
}
