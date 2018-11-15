using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 范围运费模板输入对象
    /// </summary>
    public class RangeFreightTemplateInputDTO
    {
        public RangeFreightTemplateInputDTO()
        {
            Freights = new List<RangeFreightModel>();
        }

        public Guid TemplateId { get; set; }

        public Guid AppId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public bool FreeExpress { get; set; }

        public List<RangeFreightModel> Freights { get; set; }
    }

    public class RangeFreightModel
    {
        public string ProvinceCodes { get; set; }

        public bool IsSpecific
        {
            get
            {
                return !string.IsNullOrEmpty(ProvinceCodes);
            }
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public int Cost { get; set; }
    }
}
