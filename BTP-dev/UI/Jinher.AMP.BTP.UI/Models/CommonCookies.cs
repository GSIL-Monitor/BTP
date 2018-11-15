using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Models
{
    public class CommonCookies
    {
        /// <summary>
        /// 当前平台appid
        /// </summary>
        public Guid AppId { get; set; }
        /// <summary>
        /// 皮肤类型
        /// </summary>
        public int SkinType { get; set; }
        /// <summary>
        /// 是否定制应用
        /// </summary>
        public bool IsFitted { get; set; }
        /// <summary>
        /// 应用版式
        /// </summary>
        public string LayoutCode { get; set; }
        /// <summary>
        /// 货币符号
        /// </summary>
        public string Currency { get; set; }
    }
}
