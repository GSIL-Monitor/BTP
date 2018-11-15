using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    /// 苏宁售后状态表
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderRefundDto
    {
        //服务单号
        public string ServiceId { get; set; }

        /// <summary>
        /// 是否可取消(0代表否，1代表是)
        /// </summary>
        [DataMember]
        public int Cancel { get; set; }

        /// <summary>
        /// 取件方式(1客户发货 2上门取件)
        /// </summary>
        [DataMember]
        public int PickwareType { get; set; }

        [DataMember]
        public string CustomerContactName { get; set; }

        [DataMember]
        public string CustomerTel { get; set; }

        [DataMember]
        public string PickwareAddress { get; set; }

        [DataMember]
        public string PickwarePovinceCode { get; set; }

        [DataMember]
        public string PickwareCityCode { get; set; }
    }
}
