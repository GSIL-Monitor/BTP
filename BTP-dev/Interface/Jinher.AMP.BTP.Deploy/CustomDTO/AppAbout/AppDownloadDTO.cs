using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 应用相关接口搜索dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppDownloadDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 推广下载引导语
        /// </summary>
        [DataMember]
        public string PromotionDownGuide { get; set; }
        /// <summary>
        /// logo
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

    }
}
