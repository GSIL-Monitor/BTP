using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 二维码查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class WeChatQRCodeSearchDTO : SearchBase
    {
        /// <summary>
        /// 是否显示餐饮分类
        /// </summary>
        [DataMember]
        public bool IsShowCatering { get; set; }

        public Guid AppId { get; set; }

        /// <summary>
        /// 公众号名称
        /// </summary>
        [DataMember]
        public string WeChatPublicCode { get; set; }

        /// <summary>
        /// 二维码类型
        /// </summary>  
        [DataMember]
        public int? QRType { get; set; }

        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public bool? IsUse { get; set; }
    }
}
