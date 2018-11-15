using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 二维码显示dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class QrCodeCreateDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public System.Guid AppId { get; set; }
        /// <summary>
        /// 公众号名称
        /// </summary>
        [DataMember]
        public string WeChatPublicCode { get; set; }
        /// <summary>
        /// 生成数量
        /// </summary>
        [DataMember]
        public int CreateNo { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 二维码类型
        /// </summary>
        [DataMember]
        public int QrType { get; set; }
    }
}
