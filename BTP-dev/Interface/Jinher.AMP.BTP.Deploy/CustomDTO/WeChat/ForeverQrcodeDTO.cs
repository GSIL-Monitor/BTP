using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.WeChat
{
    /// <summary>
    /// 发送消息
    /// </summary>
    [DataContract]
    [Serializable]
    public class ForeverQrcodeDTO : AccessCertifDTO
    {
        /// <summary>
        /// 场景参数
        /// </summary>
        [DataMember]
        public string SceneStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GetPostJson
        {
            get
            {
                return "{\"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" +
                           SceneStr + "\"}}}";
            }
        }

    }
}
