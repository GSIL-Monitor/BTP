using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 应用搜素结果数据
    /// </summary>
    public class AppSearchResultDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        public Guid AppId { get; set; }
        /// <summary>
        /// 所属者Id
        /// </summary>
        public Guid OwnerId { get; set; }
        /// <summary>
        /// 所属者类型：0 是个人，1是企业
        /// </summary>
        public string OwnerType { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
    }
}
