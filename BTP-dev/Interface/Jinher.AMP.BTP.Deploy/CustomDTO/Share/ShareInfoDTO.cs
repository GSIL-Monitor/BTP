using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单app分享信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class OrderShareInfoDTO
    {
        /// <summary>
        /// 是否app分享
        /// </summary>
        [DataMember]
        public bool IsShare { get; set; }
        /// <summary>
        /// 分享Id
        /// </summary>
        [DataMember]
        public string ShareId { get; set; }
        /// <summary>
        /// 分享人
        /// </summary>
        [DataMember]
        public Guid ShareUser { get; set; }
        /// <summary>
        /// 分享应用
        /// </summary>
        [DataMember]
        public Guid ShareAppId { get; set; }
        /// <summary>
        /// 订单app分享信息
        /// </summary>
        public OrderShareInfoDTO()
        {
            IsShare = false;
        }
    }
}
