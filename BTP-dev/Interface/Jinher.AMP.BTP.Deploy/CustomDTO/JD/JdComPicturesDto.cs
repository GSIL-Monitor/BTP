using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 获取京东图片
    /// </summary>
    public class JdComPicturesDto
    {
        /// <summary>
        /// 图片id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime created { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modified { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int yn { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 是否是主图 1为主图，0为附图
        /// </summary>
        public string isPrimary { get; set; }
        /// <summary>
        /// 排序图片路径 
        /// </summary>
        public string orderSort { get; set; }
        public string position { get; set; }
        public int type { get; set; }
        public string features { get; set; }
    }
}
