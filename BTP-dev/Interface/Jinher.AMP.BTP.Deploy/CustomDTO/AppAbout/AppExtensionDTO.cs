using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商贸app扩展表
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppExtensionDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 是否显示分类菜单
        /// </summary>
        [DataMember]
        public bool IsShowSearchMenu { get; set; }
        /// <summary>
        /// 是否商品列表加购物车 0 否，1是
        /// </summary>
        [DataMember]
        public bool IsShowAddCart { get; set; }
        /// <summary>
        /// 是否全部商品分成
        /// </summary>
        [DataMember]
        public bool? IsDividendAll { get; set; }
        /// <summary>
        /// 众销分成比例
        /// </summary>
        [DataMember]
        public decimal SharePercent { get; set; }
        /// <summary>
        /// 是否启用积分抵现功能
        /// </summary>
        [DataMember]
        public bool IsCashForScore { get; set; }
        /// <summary>
        /// 分销体系中直接上级分成比例
        /// </summary>
        [DataMember]
        public decimal? DistributeL1Percent { get; set; }
        /// <summary>
        /// 分销体系中2级上级分成比例
        /// </summary>
        [DataMember]
        public decimal? DistributeL2Percent { get; set; }
        /// <summary>
        /// 分销体系中3级上级分成比例
        /// </summary>
        [DataMember]
        public decimal? DistributeL3Percent { get; set; }
        /// <summary>
        /// 默认发票类型
        /// </summary>
        [DataMember]
        public Int32 InvoiceDefault { get; set; }
        /// <summary>
        /// 发票设置
        /// </summary>
        [DataMember]
        public Int32 InvoiceValues { get; set; }
        /// <summary>
        /// 渠道分销体系中分成比例
        /// </summary>
        [DataMember]
        public decimal? ChannelDistributePercent { get; set; }
        /// <summary>
        /// 是否统一设置积分抵现比例
        /// </summary>   
        [DataMember]
        public bool? IsScoreAll { get; set; }
        /// <summary>
        /// 积分抵用上限
        /// </summary>   
        [DataMember]
        public decimal ScorePercent { get; set; }
    }
}
