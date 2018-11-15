using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.MallApply
{
    /// <summary>
    /// 商城实体
    /// </summary>
    [DataContract]
    [Serializable]
    public class MallApplyDTO : SearchBase
    {

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime SubTime { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public Guid EsAppId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public string EsAppName { get; set; }

        [DataMember]
        public string ApplyContent { get; set; }

        [DataMember]
        public Decimal Commission { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public string StateShow { get; set; }

        /// <summary>
        /// 商家类型（0-自营他配；1-第三方；2-自营自配自采；3-自营自配统采）
        /// </summary>   
        [DataMember]
        public short? Type { get; set; }

        [DataMember]
        public string TypeString { get; set; }

        /// <summary>
        /// 是否全部设置的结算价
        /// </summary>
        [DataMember]
        public bool IsAllSetSettlePrice { get; set; }
       
        [DataMember]
        public long CrcAppId { get; set; }
    }
}
