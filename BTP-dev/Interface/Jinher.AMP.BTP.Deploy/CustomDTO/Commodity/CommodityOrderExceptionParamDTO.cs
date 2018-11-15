using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// GetAllCommodityOrderExceptionByAppId接口参数。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityOrderExceptionParamDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int State { get; set; }


        /// <summary>
        /// 每页返回记录数     
        /// </summary>
        [DataMember]
        public int PageSize   {get;set;}
        
        /// <summary>
        /// 页码
        /// </summary>
        [DataMember]
        public int PageNumber { get; set; }

        /// <summary>
        /// 调用方
        /// </summary>
        [DataMember]
        public string Invoker { get; set; }
    }
}
