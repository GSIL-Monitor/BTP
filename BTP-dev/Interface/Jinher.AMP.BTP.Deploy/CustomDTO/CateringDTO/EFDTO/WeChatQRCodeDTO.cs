using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.CateringDTO
{
    public class WeChatQRCodeDTO : DBBase
    {
        /// <summary>
        /// 表键值
        /// </summary>
        [DataMember]
        public Guid id { get; set; }
        /// <summary>
        /// 公众号
        /// </summary>
        [DataMember]
        public string weChatPublicCode { get; set; }
        /// <summary>
        /// 公众号AppId
        /// </summary>
        [DataMember]
        public string WeChatAppId { get; set; }
        /// <summary>
        /// 公众号Secret
        /// </summary>
        [DataMember]
        public string weChatSecret { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid appId { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public Int32 qrNo { get; set; }
        /// <summary>
        /// 类型：门店、桌号、服务员
        /// </summary>
        [DataMember]
        public Deploy.Enum.QrType QrType { get; set; }
        /// <summary>
        /// 微信Ticket
        /// </summary>
        [DataMember]
        public string WeChatTicket { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        [DataMember]
        public Guid StoreId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsUse { get; set; }

        /// <summary>
        /// 推广主id
        /// </summary>
        [DataMember]
        public Guid SpreadInfoId { get; set; }
        /// <summary>
        /// 是否自定义微信设置
        /// </summary>
        [DataMember]
        public bool IsAppWeChatSetting { get; set; }
    }
}
