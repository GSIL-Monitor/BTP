using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
namespace Jinher.AMP.BTP.Deploy.CustomDTO
{  

     /// <summary>
    /// 京东日志实体
    /// </summary>
    [DataContract]
    [Serializable]
    public class JdlogsDTO : SearchBase
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public bool Isdisable { get; set; }

        [DataMember]
        public ThirdECommerceTypeEnum ThirdECommerceType { get; set; }
    }

    [DataContract]
    [Serializable]
    public class JourneyDTO : SearchBase
    {
        /// <summary>
        /// 当前第几页
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数据行数
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public DateTime dtBegTime { get; set; }

        [DataMember]
        public DateTime dtEndTime { get; set; }

        [DataMember]
        public Guid OrderID { get; set; }

        [DataMember]
        public String OrderCode { get; set; }

        [DataMember]
        public String EclpOrderNo { get; set; }
    }   
}
