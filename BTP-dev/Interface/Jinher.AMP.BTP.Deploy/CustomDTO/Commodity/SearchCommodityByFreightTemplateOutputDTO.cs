using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public partial class SearchCommodityByFreightTemplateOutputDTO
    {
        public SearchCommodityByFreightTemplateOutputDTO()
        {
            Commodities = new List<ComdtyList4SelCDTO>();
        }

        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public IList<ComdtyList4SelCDTO> Commodities { get; set; }
    }
}
