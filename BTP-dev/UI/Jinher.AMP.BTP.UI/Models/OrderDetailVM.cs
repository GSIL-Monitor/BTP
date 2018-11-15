using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.UI.Models
{
    public class OrderDetailVM
    {
        public CommodityOrderSDTO data { get; set; }
        public string Msg { get; set; }
        public string AppName { get; set; }
    }
}