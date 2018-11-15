using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    public class AppMangerModel
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用生成时间
        /// </summary>
        public string AppCreateOn { get; set; }

        /// <summary>
        /// 应用图标
        /// </summary>
        public string AppIcon { get; set; }

        /// <summary>
        /// 是否已加入应用组
        /// </summary>
        public string IsAddToAppSet { get; set; }
    }
}