using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 绑定微信二维码
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadBindWeChatQrCodeDTO
    {
        /// <summary>
        /// 公众号名称
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 二维码类型
        /// </summary>  
        [DataMember]
        public int QRType { get; set; }

        /// <summary>
        /// 二维码序号
        /// </summary>
        [DataMember]
        public int QRNo { get; set; }
    }
}
