using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YPK
{
    /// <summary>
    /// 商品详情
    /// </summary>
    public class YPKComDetailDto
    {
        /// <summary>
        /// 易派客单品Id
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string brandName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 单品价格
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 商品描述,后拼接规格属性html
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 重量（单位：kg）
        /// </summary>
        public string weight { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 商品条形码
        /// </summary>
        public string barCode { get; set; }
        /// <summary>
        /// 商品图片路径
        /// </summary>
        public string[] picturesPath { get; set; }
        /// <summary>
        /// 商品税率
        /// </summary>
        public string taxRate { get; set; }
        /// <summary>
        /// 海信物料编号
        /// </summary>
        public string erQiCode { get; set; }
        /// <summary>
        /// 时间戳(精确到毫秒)
        /// </summary>
        public string timeStamp { get; set; } 

    }  
}
