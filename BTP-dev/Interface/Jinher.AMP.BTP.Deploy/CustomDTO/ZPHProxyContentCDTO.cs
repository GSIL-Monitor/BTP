using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 总代信息
    /// </summary>
    public class ZPHProxyContentCDTO
    {
        /// <summary>
        /// 所在区域
        /// </summary>
        public Guid changeOrg { get; set; }
        /// <summary>
        /// 所在市
        /// </summary>
        public string cityCode { get; set; }
        /// <summary>
        /// 标识Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// IW号
        /// </summary>
        public string iwAccount { get; set; }
        /// <summary>
        /// 所在省
        /// </summary>
        public string provinceCode { get; set; }
        /// <summary>
        /// 总代名称
        /// </summary>
        public string proxyName { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        public Guid AppId { get; set; }
    }
}
