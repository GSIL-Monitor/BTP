using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 推广主展示dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadInfoShowDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 推广主userid
        /// </summary>
        [DataMember]
        public System.Guid SpreadId { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [DataMember]
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 推广类型 具体参见SpreadCategory.[SpreadType]
        /// </summary>
        [DataMember]
        public int SpreadType { get; set; }
        /// <summary>
        /// 推广类型描述
        /// </summary>
        [DataMember]
        public string SpreadTypeDesc { get; set; }
        /// <summary>
        /// 推广appId
        /// </summary>
        [DataMember]
        public Guid SpreadAppId { get; set; }
        /// <summary>
        /// 推广app名称
        /// </summary>
        [DataMember]
        public string SpreadAppName { get; set; }
        /// <summary>
        /// 旺铺Id
        /// </summary>
        [DataMember]
        public Guid HotshopId { get; set; }
        /// <summary>
        /// 旺铺名称
        /// </summary>
        [DataMember]
        public string HotshopName { get; set; }
        /// <summary>
        /// 二维码地址
        /// </summary>
        [DataMember]
        public string QrCodeUrl { get; set; }
        /// <summary>
        /// 推广主描述（目前存储用户姓名）
        /// </summary>
        [DataMember]
        public string SpreadDesc { get; set; }
        /// <summary>
        /// 推广url
        /// </summary>
        [DataMember]
        public string SpreadUrl { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 是否绑定微信二维码
        /// </summary>
        [DataMember]
        public bool IsBindWeChatQrCode { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        [DataMember]
        public int IsDel { get; set; }

        /// 推广组织IWCode
        /// </summary>
        public string IWCode { get; set; }

        /// <summary>
        /// 子代理数量
        /// </summary>
        public int SubSpreadCount { get; set; }

        /// <summary>
        /// 总代分佣比例
        /// </summary>
        public decimal DividendPercent { get; set; }
    }
}
