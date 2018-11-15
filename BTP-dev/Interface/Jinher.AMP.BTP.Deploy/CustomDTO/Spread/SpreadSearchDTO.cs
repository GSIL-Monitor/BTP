using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class SpreadSearchDTO : SearchBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 微信二维码AppId
        /// </summary>
        [DataMember]
        public Guid WeChatQrCodeAppId { get; set; }
        /// <summary>
        /// 微信二维码类型
        /// </summary>
        [DataMember]
        public int WeChatQrCodeType { get; set; }

        /// <summary>
        /// 推广主类型
        /// </summary>
        [DataMember]
        public int? SpreadType { get; set; }
        /// <summary>
        /// iu帐号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 推广app
        /// </summary>
        [DataMember]
        public Guid? SpreadAppId { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? IWId { get; set; }
    }
}
