using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class PrintOrderDTO
    {
        /// <summary>
        /// 订单数据
        /// </summary>
        [DataMember]
        public ImportOrderDTO Orders { get; set; }

        /// <summary>
        /// 订单商品数据
        /// </summary>
        [DataMember]
        public List<ImportOrderItemDTO> OrderItems { get; set; }
    }

    /// <summary>
    /// 打印订单更新数据信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UpdatePrintDTO
    {
        /// <summary>
        ///应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        ///用户
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        ///打印类型
        /// </summary>
        [DataMember]
        public int PrintType { get; set; } 

        /// <summary>
        /// 是否自动发货
        /// </summary>
        [DataMember]
        public bool AutoSend { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [DataMember]
        public string ShipName { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        [DataMember]
        public List<UpdatePrintOrderDTO> Orders { get; set; }
    }

    /// <summary>
    /// 已经打印的订单数据
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UpdatePrintOrderDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 打印的订单快递号
        /// </summary>
        [DataMember]
        public string ExpressOrder { get; set; }
    }

}
