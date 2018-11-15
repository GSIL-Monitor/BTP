using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 应用组-应用信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSetAppDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }

        /// <summary>
        /// 应用生成时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime AppCreateOn { get; set; }

        /// <summary>
        /// 应用图标
        /// </summary>
        [DataMemberAttribute()]
        public string AppIcon { get; set; }

        /// <summary>
        /// 是否已加入应用组
        /// </summary>
        [DataMemberAttribute()]
        public bool IsAddToAppSet { get; set; }
    }

    /// <summary>
    /// 应用组-应用分页列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppSetAppGridDTO
    {
        /// <summary>
        /// 应用总数
        /// </summary>
        [DataMemberAttribute()]
        public int TotalAppCount { get; set; }

        /// <summary>
        /// 应用列表
        /// </summary>
        [DataMemberAttribute()]
        public List<AppSetAppDTO> AppList { get; set; }
    }
}
