using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class RefundOrderListDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// AppName
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }
        [DataMemberAttribute()]
        public int Payment { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<OrderListItemCDTO> ShoppingCartItemSDTO { get; set; }
        /// <summary>
        /// 退款物流公司（售后时存OrderRefundAfterSales.RefundExpCo）
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpCo { get; set; }
        /// <summary>
        /// 退款物流单号（售后时存OrderRefundAfterSales.RefundExpOrderNo）
        /// </summary>
        [DataMemberAttribute()]
        public string RefundExpOrderNo { get; set; }
        /// <summary>
        /// 页面显示的退款状态：0-退款中，1-已完成
        /// </summary>
        public string RefundShowState { get; set; }
                /// <summary>
        /// 页面显示的退款状态：0-退款中，1-已完成
        /// </summary>
        [DataMemberAttribute()]
        public int RefundState { get; set; }
        [DataMemberAttribute()]
        public string ItemName { get; set; }
        [DataMemberAttribute()]
        public string ItemPic { get; set; }
        [DataMemberAttribute()]
        public string ItemSize { get; set; }
        [DataMemberAttribute()]
        public decimal ItemPrice { get; set; }
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }
        [DataMemberAttribute()]
        public int? ItemState { get; set; }
        [DataMemberAttribute()]
        public Guid ItemID { get; set; }
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }
        [DataMemberAttribute()]
        public int OrderState { get; set; }
        [DataMemberAttribute()]
        public int? OrderItemState { get; set; }
    }
}
