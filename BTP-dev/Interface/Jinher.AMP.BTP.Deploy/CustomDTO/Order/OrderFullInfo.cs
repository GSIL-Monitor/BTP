using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单相关信息。
    /// </summary>
    [DataContract]
    public class OrderFullInfo
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        [DataMember]
        public CommodityOrderVM OrderVM { get; set; }

        /// <summary>
        /// 售后服务
        /// </summary>
        [DataMember]
        public CommodityOrderServiceDTO CoServiceDTO { get; set; }

        /// <summary>
        /// 订单商品集合
        /// </summary>
        [DataMember]
        public List<OrderItemsVM> OrderItems { get; set; }

        /// <summary>
        /// 售后退款历史
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.OrderRefundAfterSalesDTO> OrasList { get; set; }

        /// <summary>
        /// 售中退款历史
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.OrderRefundDTO> OrList { get; set; }


        /// <summary>
        /// 分润设置
        /// </summary>
        [DataMember]
        public List<SettlingAccountDTO> SaList { get; set; }

        /// <summary>
        /// 钱款去向
        /// </summary>
        [DataMember]
        public List<OrderPayeeDTO> OrderPayeeList { get; set; }
    }
}
