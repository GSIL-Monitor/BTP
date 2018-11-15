using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 保存推广主dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpreadSaveDTO
    {
        /// <summary>
        /// 推广主帐号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 推广主姓名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 推广类型 具体参见SpreadCategory.[SpreadType]
        /// </summary>
        [DataMember]
        public int SpreadType { get; set; }
        /// <summary>
        /// 推广app
        /// </summary>
        [DataMember]
        public Guid SpreadAppId { get; set; }
        /// <summary>
        /// 旺铺Id
        /// </summary>
        [DataMember]
        public Guid HotshopId { get; set; }
        /// <summary>
        /// 二维码图标
        /// </summary>
        [DataMember]
        public string QrCodeFileImg { get; set; }
        /// <summary>
        /// 推广主描述（目前存储用户姓名）
        /// </summary>
        [DataMember]
        public string SpreadDesc { get; set; }

        /// <summary>
        /// 推广组织IWId
        /// </summary>
        public Guid IWId { get; set; }

        /// <summary>
        /// IWCode
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
