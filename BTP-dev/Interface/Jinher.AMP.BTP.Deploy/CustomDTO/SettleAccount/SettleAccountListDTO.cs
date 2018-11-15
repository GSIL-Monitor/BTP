using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算管理搜索结果DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountListDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// 商户Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        ///// <summary>
        ///// 商城ID
        ///// </summary>
        //[DataMember]
        //public Guid EsAppId { get; set; }

        /// <summary>
        /// 商城名称
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMember]
        public short SellerType { get; set; }

        /// <summary>
        /// 订单总额
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 订单真实价格
        /// </summary>
        [DataMember]
        public decimal OrderRealAmount { get; set; }

        /// <summary>
        /// 商家结算金额 
        /// </summary>
        [DataMember]
        public decimal SellerAmount { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        [DataMember]
        public DateTime AmountDate { get; set; }

        /// <summary>
        /// 结算状态(0、待结算 1、等待商家确认 2、待打款 3、已结算)
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 结算结果
        /// </summary>
        [DataMember]
        public bool SettleStatue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
    }
}
