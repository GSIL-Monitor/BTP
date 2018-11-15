using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销统计查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorProfitsSearchDTO
    {
        /// <summary>
        /// 分销者Id
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }
        /// <summary>
        /// 分销商用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 分销者Id列表
        /// </summary>
        [DataMember]
        public List<Guid> DistributorIdList { get; set; }
        /// <summary>
        /// 查询种类。0：用分销者Id查；1：分销者Id列表查
        /// </summary>
        [DataMember]
        public int SearchOneOrMore { get; set; }     
        /// <summary>
        /// 查询等级。0：本人；1：一级；2：二级
        /// </summary>
        [DataMember]
        public int SearchType { get; set; }      
        /// <summary>
        /// 当前页
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
    }
}
