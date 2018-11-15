using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    ///自提点负责人信息类
    /// </summary>
    [Serializable]
    [DataContract]
    public class SelfTakeStationManagerSDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }      
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间   
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 负责人IU平台登录账号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 负责人IU平台用户ID
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 自提点ID
        /// </summary>
        [DataMember]
        public Guid SelfTakeStationId { get; set; }
    }
}
