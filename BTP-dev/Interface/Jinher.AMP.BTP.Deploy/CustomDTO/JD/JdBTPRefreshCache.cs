using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JD
{
    public class JdBTPRefreshCache
    {
        /// <summary>
        /// 数据
        /// </summary>
        public List<JdBTPRefreshCacheList> DictList { get; set; }
    }
    /// <summary>
    /// 子集
    /// </summary>
    public class JdBTPRefreshCacheList
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// 类别0 删除  1 修改 2 增加
        /// </summary>
        public int State { get; set; }
    }
}
