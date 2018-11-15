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
    public class WeChatQRCodeShowDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 公众号名称
        /// </summary>
        [DataMember]
        public string WeChatPublicCode { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public System.Guid AppId { get; set; }
        /// <summary>
        /// 二维码序号
        /// </summary>
        [DataMember]
        public int QRNo { get; set; }
        /// <summary>
        /// 获取到的Ticket
        /// </summary>
        [DataMember]
        public string WeChatTicket { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public System.Guid StoreId { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public int IsDel { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public bool IsUse { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public Guid? SpreadInfoId { get; set; }
        /// <summary>
        /// 二维码类型
        /// </summary>  
        [DataMember]
        public int QRType { get; set; }

        /// <summary>
        /// 二维码类型描述
        /// </summary>
        [DataMember]
        public string QrTypeDesc { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 二维码名称(下载二维码图片时使用)
        /// </summary>
        [DataMember]
        public string Name { get; set; }

    }

}
