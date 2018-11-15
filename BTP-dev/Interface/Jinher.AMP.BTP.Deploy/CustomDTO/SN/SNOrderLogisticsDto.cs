using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    public class SNOrderLogisticsDto
    {
        public string IsPackage { get; set; }
        public string OrderId { get; set; }
        public List<OrderItemIds> OrderItemIds { get; set; }
        public List<OrderLogistics> OrderLogistics { get; set; }
        public string PackageId { get; set; }
        public string ReceiveTime { get; set; }
        public string ShippingTime { get; set; }
    }

    public class OrderItemIds
    {
        public string OrderItemId { get; set; }
        public string SkuId { get; set; }
    }

    public class OrderLogistics
    {
        public string OperateState { get; set; }
        public string OperateTime { get; set; }
    }
}
