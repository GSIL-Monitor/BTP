using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算管理搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountSearchDTO : SearchBase
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }


        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }



        /// <summary>
        /// App名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }


        /// <summary>
        /// 商城
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 商城名称
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMember]
        public short? SellerType { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        [DataMember]
        public DateTime AmountDate { get; set; }
    }
}
