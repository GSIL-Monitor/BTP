using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 电子发票导出类
    /// </summary>
    [Serializable]
    [DataContract]
    public class ElectronicInvoiceDTO
    {
        /// <summary>
        /// AppId号码
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 发票流水号
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 购货方邮箱
        /// </summary>
        [DataMember]
        public string ReceiptEmail { get; set; }

        /// <summary>
        /// 购货方手机
        /// </summary>
        [DataMember]
        public string ReceiptPhone { get; set; }

        /// <summary>
        /// 购货方名称
        /// </summary>
        [DataMember]
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 购货方识别号
        /// </summary>
        [DataMember]
        public string BuyerCode { get; set; }

        /// <summary>
        /// 购货方地址
        /// </summary>
        [DataMember]
        public string ReceiptAddress { get; set; }

        /// <summary>
        /// 购货方固定电话
        /// </summary>
        [DataMember]
        public string BuyerPhone { get; set; }

        /// <summary>
        /// 购货方银行账号
        /// </summary>
        [DataMember]
        public string BuyerBankNumber { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        //[DataMember]
        //public string Name { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [DataMember]
        public string Specifications { get; set; }

        /// <summary>
        /// 项目单位
        /// </summary>
        [DataMember]
        public string ProjectUnit { get; set; }

        /// <summary>
        /// 项目数量
        /// </summary>
        //[DataMember]
        //public int Number { get; set; }

        /// <summary>
        /// 含税标志
        /// </summary>
        [DataMember]
        public int TallageMark { get; set; }

        /// <summary>
        /// 项目单价
        /// </summary>
        //[DataMember]
        //public decimal? Price { get; set; }

        /// <summary>
        /// 项目金额
        /// </summary>
        [DataMember]
        public decimal? RealPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public double  TaxRate { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 发票行性质
        /// </summary>
        [DataMember]
        public int InvoicelineProperty { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        //[DataMember]
        //public string TaxClassCode { get; set; }

        /// <summary>
        /// 自行编码
        /// </summary>
        //[DataMember]
        //public string No_Code { get; set; }

        /// <summary>
        /// 优惠政策表示
        /// </summary>
        [DataMember]
        public int PolicyMark { get; set; }

        /// <summary>
        /// 零税率标识
        /// </summary>
        [DataMember]
        public int ZeroTaxRateMark { get; set; }

        /// <summary>
        /// 增值税特殊管理
        /// </summary>
        [DataMember]
        public string SpecialParticular { get; set; }


        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        public decimal Freight { get; set; }


        /// <summary>
        /// 电子发票小类
        /// </summary>
        [DataMember]
        public List<SmallInvoiceDTO> SmallInvoice { get; set; }


    }


    /// <summary>
    /// 电子发票小类
    /// </summary>
    [Serializable]
    [DataContract]
    public class SmallInvoiceDTO
    {

        /// <summary>
        /// 项目数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 项目单价
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }


        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string TaxClassCode { get; set; }

        /// <summary>
        /// 自行编码
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }


        /// <summary>
        /// 商品成本价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal? TaxRate { get; set; }


    }
}
