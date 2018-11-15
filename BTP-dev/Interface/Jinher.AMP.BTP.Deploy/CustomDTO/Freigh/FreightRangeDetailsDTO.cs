using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public partial class FreightRangeDefaultDetailsDTO
    {
        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public decimal Cost { get; set; }
    }

    public partial class FreightRangeSpecificDetailsDTO
    {
        public FreightRangeSpecificDetailsDTO()
        {
            ProvinceNames = new List<string>();
            Details = new List<FreightRangeDefaultDetailsDTO>();
        }

        public string ProvinceCodes { get; set; }

        public IList<string> ProvinceNames { get; set; }

        public IList<FreightRangeDefaultDetailsDTO> Details { get; set; }
    }
}
