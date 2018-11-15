using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 注册绑定类
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserSpreaderBindDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 买家Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 推广者Id
        /// </summary>
        [DataMember]
        public Guid SpreaderId { get; set; }
        /// <summary>
        /// 推广码
        /// </summary>
        [DataMember]
        public Guid SpreadCode { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
        /// <summary>
        /// 初始订单
        /// </summary>
        [DataMember]
        public Guid CreateOrderId { get; set; }
        /// <summary>
        /// 微信OpenId
        /// </summary>
        [DataMember]
        public string WxOpenId { get; set; }
    }
}
