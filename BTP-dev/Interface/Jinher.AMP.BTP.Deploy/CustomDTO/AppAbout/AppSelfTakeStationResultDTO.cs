using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class AppSelfTakeStationResultDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMember]
        public string District { get; set; }
        /// <summary>
        /// 自提点详细地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 自提点电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 提交人ID
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
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
    }
}
