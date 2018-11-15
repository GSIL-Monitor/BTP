using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class OrderQueueDTO
    {
        /// <summary>
        /// 订单id
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMemberAttribute()]
        public string Code { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid UserId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid AppId { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary> 
        [DataMemberAttribute()]
        public int Payment { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }
        /// <summary>
        /// 省
        /// </summary> 
        [DataMemberAttribute()]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [DataMemberAttribute()]
        public string Street { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        [DataMemberAttribute()]
        public int SrcType { get; set; }
        /// <summary>
        /// 来源标识Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid SrcTagId { get; set; }

        /// <summary>
        /// CPSId
        /// </summary>
        [DataMemberAttribute()]
        public string CPSId { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }

        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<ShoppingCartItemSDTO> ShoppingCartItemSDTO { get; set; }
    }
}
