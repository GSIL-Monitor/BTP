using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    [Serializable]
    public class GoldPayDTO
    {
        public Guid orderId { get; set; }
        public Guid userId { get; set; }
        public Guid appId { get; set; }
        public string goldpwd { get; set; }
        public decimal realprice { get; set; }
        public string comName { get; set; }
        public string sessionId { get; set; }
        public decimal couponCount { get; set; }
        public string couponCodes { get; set; }
        public decimal gold { get; set; }
    }
}