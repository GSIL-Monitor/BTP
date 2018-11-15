using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee
{
    /// <summary>
    /// 易捷员工搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class JdCommoditySearchDTO : SearchBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 京东编码
        /// </summary>
        [DataMember]
        public string JDCode { get; set; }
        /// <summary>
        ///信息是否补全状态  0 未补全 1 补全
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 商品中已存在的备注编码
        /// </summary>
        [DataMember]
        public List<string> RepeatData { get; set; }
        /// <summary>
        /// 京东商品表存在的备注编码
        /// </summary>
        [DataMember]
        public List<string> JdRepeatData { get; set; }
        /// <summary>
        /// 无效数据
        /// </summary>
        [DataMember]
        public List<string> InvalidData { get; set; }
        /// <summary>
        /// 苏宁易购价格为空的数据
        /// </summary>
        [DataMember]
        public List<string> NullPriceData { get; set; }
        /// <summary>
        /// 苏宁易购税率为空的数据
        /// </summary>
        [DataMember]
        public List<string> NullTaxData { get; set; }
        /// <summary>
        ///商品或服务名称
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 增值税税率
        /// </summary>
        [DataMember]
        public double taxrate { get; set; }
        /// <summary>
        /// 类别中没有挂上的
        /// </summary>
        [DataMember]
        public List<string> NoCategoryData { get; set; }
        /// <summary>
        /// 商城品类没有挂上的
        /// </summary>
        [DataMember]
        public List<string> NoPinLeiData { get; set; }
        /// <summary>
        /// 异常数据(易派客)
        /// </summary>
        [DataMember]
        public List<YPKException> ExceptionData { get; set; }
        /// <summary>
        /// 失败数(易派客)
        /// </summary>
        [DataMember]
        public int FaildCount { get; set; }
        /// <summary>
        /// 成功数(易派客)
        /// </summary>
        [DataMember]
        public int SuccessCount { get; set; }
        /// <summary>
        /// 异常数据文件地址(易派客)
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }
    }
    public class YPKException
    {
        /// <summary>
        /// 备注编码
        /// </summary>
        [DataMember]
        public string SkuId { get; set; }
        /// <summary>
        /// 商品类目
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }
        /// <summary>
        /// 商城品类
        /// </summary>
        [DataMember]
        public string VideoName { get; set; }
        /// <summary>
        /// 税收编码
        /// </summary>
        [DataMember]
        public string TaxClassCode { get; set; }
        /// <summary>
        /// 销项税
        /// </summary>
        [DataMember]
        public decimal? TaxRate { get; set; }
        /// <summary>
        /// 进项税
        /// </summary>
        [DataMember]
        public decimal? InputRax { get; set; }
        /// <summary>
        /// 商品售价
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
    }
}
