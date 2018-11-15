using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    public class SNQueryParamsDto
    {
        public string OrderId { get; set; }
        public List<SNQueryItemParamsDto> OrderItemIds { get; set; }
    }

    public class SNQueryItemParamsDto
    {
        public string OrderItemId { get; set; }
        public string SkuId { get; set; }
    }

    public class SNConfirmParamsDto
    {
        public string OrderId { get; set; }
        public List<SNConfirmItemParamsDto> SkuConfirmList { get; set; }
    }

    public class SNConfirmItemParamsDto
    {
        public string ConfirmTime { get; set; }
        public string SkuId { get; set; }
    }
}
