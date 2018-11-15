using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorInfoDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 用户帐号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 成为分销商的时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 分销应用Id
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 上级分销商Id
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }
        /// <summary>
        /// 分销商等级
        /// </summary>
        [DataMember]
        public Int32 Level { get; set; }
        /// <summary>
        /// 分销关系Id，（层级关系从上到下分销商Id以.分割组成的字符串）
        /// </summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>
        /// 分销商用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        [DataMember]
        public DateTime UserSubTime { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        [DataMember]
        public string PicturePath { get; set; }
    }
}
