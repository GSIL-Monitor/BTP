using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    /// <summary>
    /// 商品详情
    /// </summary>
    public class SNComDetailDto
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 主图地址
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// Html描述
        /// </summary>
        public string introduction { get; set; }
        /// <summary>
        /// 商品清单
        /// </summary>
        public string packlisting { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string productArea { get; set; }
        /// <summary>
        /// 销售单位
        /// </summary>
        public string saleUnit { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 上下架状态【1：在售0：下架】
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string upc { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public string weight { get; set; }
        /// <summary>
        /// 参数信息
        /// </summary>
        public List<ProdParams> prodParams { get; set; }

    }

    public class ProdParams
    {
        /// <summary>
        /// 参数主体描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public List<Param> param { get; set; }
    }
    public class Param
    {
        /// <summary>
        /// 核心参数
        /// </summary>
        public string coreFlag { get; set; }
        /// <summary>
        /// 参数代码
        /// </summary>
        public string snparameterCode { get; set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        public string snparameterdesc { get; set; }
        /// <summary>
        /// 参数组代码
        /// </summary>
        public string snparametersCode { get; set; }
        /// <summary>
        /// 参数组名称
        /// </summary>
        public string snparametersDesc { get; set; }
        /// <summary>
        /// 参数排序码
        /// </summary>
        public string snparameterSequence { get; set; }
        /// <summary>
        /// 参数描述值
        /// </summary>
        public string snparameterVal { get; set; }
        /// <summary>
        /// 参数组排序
        /// </summary>
        public string snsequence { get; set; }
    }
    /// <summary>
    /// 商品扩展信息
    /// </summary>
    public class SNComExtendDto
    {
        /// <summary>
        /// 是否支持开增票(01-支持；02-不支持)
        /// </summary>
        public string increaseTicket { get; set; }
        /// <summary>
        /// 支持无理由退货的天数
        /// </summary>
        public string noReasonLimit { get; set; }
        /// <summary>
        /// 退货描述
        /// </summary>
        public string noReasonTip { get; set; }
        /// <summary>
        /// 是否支持无理由退货(01-7天无理由退货；02-不支持退货)
        /// </summary>
        public string returnGoods { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }

    }

    public class SNSkuStateDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 上下架状态1：在售0：下架
        /// </summary>
        public string state { get; set; }
    }

    public class SNInventory
    {
        /// <summary>
        /// 购买数量
        /// </summary>
        public string num { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }
    }
}
