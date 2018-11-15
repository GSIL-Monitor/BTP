using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 付款成功配置
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderPayedConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 订单付款成功图片
        /// </summary>
        [DataMember]
        public string OrderPayedPic { get; set; }
        /// <summary>
        /// 订单付款成功描述
        /// </summary>
        [DataMember]
        public string OrderShareDesc { get; set; }


    }
}
