using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class OrderListAllCDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderListItemCDTO> ShoppingCartItemSDTO { get; set; }
        /// <summary>
        ///  AppID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        ///  UserID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        ///  订单状态
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }

        [DataMemberAttribute()]
        public decimal Freight { get; set; }

    }
}
