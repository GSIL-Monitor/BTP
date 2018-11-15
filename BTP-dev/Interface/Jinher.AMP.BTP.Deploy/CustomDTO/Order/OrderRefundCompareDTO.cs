using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class OrderRefundCompareDTO
    {
        public DateTime SubTime { get; set; }
        public decimal? OrderRefundMoneyAndCoupun { get; set; }
        public decimal? RefundYJCouponMoney { get; set; }
        public decimal? RefundFreightPrice { get; set; }
        public decimal RefundYJBMoney { get; set; }
        public decimal RefundMoney { get; set; }
        public Guid OrderId{ get; set; }
    }
}
