
using System.Collections.Generic;
namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    public class YXComDetailDTO
    {
        /// <summary>
        /// 商品（SPU）的标识
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 列表页图片的URL
        /// </summary>
        public string listPicUrl { get; set; }

        /// <summary>
        /// 商品下的sku列表
        /// </summary>
        public List<skuList> skuList { get; set; }

        /// <summary>
        /// 商品类目路径列表
        /// </summary>
        public List<List<categoryPathList>> categoryPathList { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string simpleDesc { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品详情
        /// </summary>
        public itemDetail itemDetail { get; set; }
        /// <summary>
        /// 商品属性列表
        /// </summary>
        public List<attrList> attrList { get; set; }
        /// <summary>
        /// 已弃用，请使用渠道售价
        /// </summary>
        public decimal yanxuanPrice { get; set; }
        /// <summary>
        /// 弃用 请使用skuList下的id
        /// </summary>
        public int primarySkuId { get; set; }
    }

    /// <summary>
    /// 商品下的sku列表
    /// </summary>
    public class skuList
    {
        /// <summary>
        /// 已弃用，请使用渠道售价
        /// </summary>
        public decimal yanxuanPrice { get; set; }
        /// <summary>
        /// sku规格图片的URL
        /// </summary>
        public string picUrl { get; set; }
        /// <summary>
        /// 渠道售价
        /// </summary>
        public decimal channelPrice { get; set; }
        /// <summary>
        /// sku规格文字信息
        /// </summary>
        public string displayString { get; set; }
        /// <summary>
        /// Sku的标识（skuId）
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// sku规格信息
        /// </summary>
        public List<itemSkuSpecValueList> itemSkuSpecValueList { get; set; }

    }
    /// <summary>
    /// 商品类目路径列表
    /// </summary>
    public class categoryPathList
    {
        /// <summary>
        ///  类目名称
        /// </summary>
        public string name { get; set; }
    }

    public class itemDetail
    {
        /// <summary>
        /// 详情页html
        /// </summary>
        public string detailHtml { get; set; }
        public string picUrl1 { get; set; }
        public string picUrl2 { get; set; }
        public string picUrl3 { get; set; }
        public string picUrl4 { get; set; }
    }

    public class attrList
    {
        public string attrValue { get; set; }
        public string attrName { get; set; }
    }
    /// <summary>
    /// sku规格信息
    /// </summary>
    public class itemSkuSpecValueList
    {
        /// <summary>
        ///  规格名
        /// </summary>
        public skuSpec skuSpec { get; set; }
        /// <summary>
        /// 规格值
        /// </summary>
        public skuSpecValue skuSpecValue { get; set; }

    }
    /// <summary>
    /// 规格名
    /// </summary>
    public class skuSpec
    {
        public string name { get; set; }
    }
    /// <summary>
    /// 规格值
    /// </summary>
    public class skuSpecValue
    {
        /// <summary>
        /// 规格图片
        /// </summary>
        public string picUrl { get; set; }
        /// <summary>
        /// 规格值
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// 一级商品属性
    /// </summary>
    public class ComAttribute
    {
        /// <summary>
        /// 一级属性名称
        /// </summary>
        public string AttrName { get; set; }
        /// <summary>
        /// 二级属性值
        /// </summary>
        public string Attrvalue { get; set; }
    }
    /// <summary>
    /// 严选库存信息
    /// </summary>
    public class StockDTO
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 库存量
        /// </summary>
        public int inventory { get; set; }

    }
    /// <summary>
    /// 严选SKUID
    /// </summary>
    public class SkuDTO
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string skuId { get; set; }
    }
    /// <summary>
    /// 库存校准记录
    /// </summary>
    public class skuCheck
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Sku校准记录
        /// </summary>
        public List<SkuCheck> skuChecks { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string operateTime { get; set; }
    }
    /// <summary>
    /// Sku校准记录
    /// </summary>
    public class SkuCheck
    {
        /// <summary>
        /// Sku Id
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        public int count { get; set; }
    }
    /// <summary>
    /// 停售报警信息
    /// </summary>
    public class SkuCloseAlarmVO
    {
        /// <summary>
        /// Sku Id
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 当前剩余库存
        /// </summary>
        public int inventory { get; set; }
    }
    /// <summary>
    /// 再次开售信息
    /// </summary>
    public class SkuReopenVO
    {
        /// <summary>
        /// Sku Id
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 当前剩余库存
        /// </summary>
        public int inventory { get; set; }
    }

}
