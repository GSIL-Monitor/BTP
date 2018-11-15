using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.WeChat
{
    /// <summary>
    /// 访问凭证
    /// </summary>
    [DataContract]
    [Serializable]
    public class AccessCertifDTO
    {
        /// <summary>
        /// 使用开发者ID
        /// </summary>
        [DataMember]
        public bool UseDeveloperId { get; set; }

        /// <summary>
        /// 开发者ID-应用ID :: UseDeveloperId为true时必填
        /// </summary>
        [DataMember]
        public string AppId { get; set; }

        /// <summary>
        /// 开发者ID-应用密钥 :: UseDeveloperId为true时必填
        /// </summary>
        [DataMember]
        public string AppSecret { get; set; }

        /// <summary>
        /// AccessToken :: UseDeveloperId为false时必填
        /// </summary>
        [DataMember]
        public string AccessToken { get; set; }

        /// <summary>
        /// 是否自定义微信设置
        /// </summary>
        [DataMember]
        public bool IsAppWeChatSetting { get; set; }
        /// <summary>
        /// 是否自定义微信设置
        /// </summary>
        [DataMember]
        public Guid JhAppId { get; set; }
    }
}
