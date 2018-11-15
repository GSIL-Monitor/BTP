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
    public class TempQrcodeDTO : AccessCertifDTO
    {
        /// <summary>
        /// 场景参数
        /// </summary>
        [DataMember]
        public int SceneId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GetPostJson
        {
            get
            {
                return "{\"expire_seconds\": 86400, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " +
                SceneId + "}}}";
            }
        }
    }
}
