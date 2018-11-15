using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class OptionModel
    {
        public string label { get; set; }
        public Guid value { get; set; }
        public bool disabled { get; set; }
    }

    public class OptionModelComparer : IEqualityComparer<OptionModel>
    {
        public static OptionModelComparer Instance = new OptionModelComparer();

        public bool Equals(OptionModel x, OptionModel y)
        {
            return x.value == y.value;
        }

        public int GetHashCode(OptionModel obj)
        {
            return obj.value.GetHashCode();
        }
    }
}
