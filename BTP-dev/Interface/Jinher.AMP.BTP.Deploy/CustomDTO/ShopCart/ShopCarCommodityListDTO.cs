using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class ShopCarCommodityListDTO
    {
        [DataMemberAttribute()]
        public string AppName { get; set; }

        [DataMemberAttribute()]
        public List<CommoditySDTO> CommoditySDTOList { get; set; }
    }
}
