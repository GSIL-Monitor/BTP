using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 协议价价格
    /// </summary>
    public class JdComDetailDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string sku { get; set; }       
        /// <summary>
        /// 单位
        /// </summary>
        public string saleUnit { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string productArea { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal weight { get; set; }
        /// <summary>
        /// 商品配件
        /// </summary>
        public string wareQD { get; set; }
        /// <summary>
        /// 主图片地址
        /// </summary>
        public string imagePath { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        public string param { get; set; }
        /// <summary>
        /// 上架下架状态
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 售后
        /// </summary>
        public string shouhou { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string brandName { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string upc { get; set; }
        /// <summary>
        /// 手机端商品详情
        /// </summary>
        public string appintroduce { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }       
        /// <summary>
        /// PC端商品详情
        /// </summary>
        public string introduction { get; set; }
    }
}
