using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    /// <summary>
    /// 苏宁图片
    /// </summary>
    public class SNComPicturesDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 访问路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 是否为主图。1：是 0：否
        /// </summary>
        public string primary { get; set; }
    }
}
