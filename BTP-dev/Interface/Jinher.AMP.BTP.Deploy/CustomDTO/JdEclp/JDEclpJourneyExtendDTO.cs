using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp
{
    [Serializable()]
    [DataContract]
    public class JDEclpJourneyExtendDTO:JDEclpOrderJournalDTO
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        [DataMemberAttribute()]
        public String AppName { get;set;}
        /// <summary>
        /// 供应商类型
        /// </summary>
        [DataMemberAttribute()]
        public short? AppType { get;set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMemberAttribute()]
        public string SupplierName { get; set; }
    }
}
