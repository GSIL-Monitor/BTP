using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品税率编码选择列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommodityTaxRateCDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 税收编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        
        /// <summary>
        /// 商品和服务名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary> 
        [DataMember]
        public string Content { get; set; }
          
        /// <summary>
        /// 增值税税率
        /// </summary>
        [DataMember]
        public double TaxRate { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [DataMember]
        public string KeyWord { get; set; }

        /// <summary>
        /// 是否可归并上一级
        /// </summary>
        [DataMember]
        public string IsCombine { get; set; }        

        /// <summary>
        /// 版本号
        /// </summary>
        [DataMember]
        public string VersionCode { get; set; }

        /// <summary>
        /// 可用状态
        /// </summary>
        [DataMember]
        public string IsUse { get; set; }

        /// <summary>
        /// 增值税特殊管理
        /// </summary>
        [DataMember]
        public string DutyState { get; set; }

        /// <summary>
        /// 增值税政策依据
        /// </summary>
        [DataMember]
        public string Policy { get; set; }

        /// <summary>
        /// 增值税特殊内容代码
        /// </summary>
        [DataMember]
        public string PolicyCode { get; set; }

        /// <summary>
        /// 消费税管理
        /// </summary>
        [DataMember]
        public string ConsumeState { get; set; }

        /// <summary>
        /// 消费税政策依据
        /// </summary>
        [DataMember]
        public string ConsumePolicy { get; set; }

        /// <summary>
        /// 消费税特殊内容代码
        /// </summary>
        [DataMember]
        public string ConsumePolicyCode { get; set; }

        /// <summary>
        /// 消费税特殊内容代码
        /// </summary>
        [DataMember]
        public double TradeCode { get; set; }

        /// <summary>
        /// 海关进出口商品品目
        /// </summary>
        [DataMember]
        public string CIQCategory { get; set; }

        /// <summary>
        /// 启用时间
        /// </summary>
        [DataMember]
        public string FiringTime { get; set; }

        /// <summary>
        /// 过渡期截止时间
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        public string UpdateTime { get; set; }
    }



    /// <summary>
    /// 商品税率编码选择列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommodityTaxRateZphDto
    {

        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 京东地址信息列表
        /// </summary>
        [DataMember]
        public List<CommodityTaxRateZphDTO> Data { get; set; }


    }


    /// <summary>
    /// 商品税率编码选择列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class CommodityTaxRateZphDTO
    {
        
        /// <summary>
        /// 税收编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 商品和服务名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }


        /// <summary>
        /// 增值税税率
        /// </summary>
        [DataMember]
        public double TaxRate { get; set; }

      
    }

}
