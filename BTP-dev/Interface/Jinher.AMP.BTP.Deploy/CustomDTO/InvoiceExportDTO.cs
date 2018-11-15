using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 导出发票DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InvoiceExportDTO
    {
        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public int InvoiceState { get; set; }

        [DataMember]
        public int InvoiceType { get; set; }

        /// <summary>
        /// 年费
        /// </summary>
        [DataMember]
        public int Year { get; set; }

         /// <summary>
         ///月份
         /// </summary>
        [DataMember]
        public int Month { get; set; }


        [DataMember]
        public Guid AppId { get; set; }

         /// <summary>
         /// 开票日期
         /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }


        /// <summary>
        /// 单位名称
        /// </summary>
        [DataMember]
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 订单内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }


        /// <summary>
        /// 订单进金额
        /// </summary>
        [DataMember]
        public decimal? RealPrice { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string Code { get; set; }
    }
}
