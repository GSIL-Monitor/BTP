using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// App自提点负责人 BP
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppStsManagerSDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// UserCode
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// SelfTakeStationId
        /// </summary>
        [DataMember]
        public Guid SelfTakeStationId { get; set; }
        /// <summary>
        /// IsDel
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

	}
}