using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UserSDTO
    {
        /// <summary>
        /// APPID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMemberAttribute()]
        public string UserName { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        [DataMemberAttribute()]
        public string PicUrl { get; set; }
        /// <summary>
        /// 性别:男=0，女=1
        /// </summary>
        [DataMemberAttribute()]
        public int Sex { get; set; }
        /// <summary>
        /// 用户简介
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [DataMemberAttribute()]
        public string LoginAccount { get; set; }
    }
}
