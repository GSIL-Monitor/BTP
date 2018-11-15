using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee
{
    /// <summary>
    /// 易捷员工搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class YJEmployeeSearchDTO : SearchBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 员工编码
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        [DataMember]
        public List<Guid> Ids { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMember]
        public string UserAccount { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [DataMember]
        public string IdentityNum { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 所在区域
        /// </summary>
        [DataMember]
        public string Area { get; set; }

        /// <summary>
        ///加油站编码
        /// </summary>
        [DataMember]
        public string StationCode { get; set; }
        /// <summary>
        /// 加油站名称
        /// </summary>
        [DataMember]
        public string StationName { get; set; }
        /// <summary>
        /// 是否是管理者  0 不是  1 是
        /// </summary>
        [DataMember]
        public int IsManager { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        [DataMember]
        public string Department { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [DataMember]
        public string Station { get; set; }
        /// <summary>
        /// 重复数据
        /// </summary>
        [DataMember]
        public List<YJEmployeeDTO> RepeatData { get; set; }
        /// <summary>
        /// 身份证号重复数据
        /// </summary>
        [DataMember]
        public List<YJEmployeeDTO> RepeatIcardData { get; set; }
        /// <summary>
        /// 无效数据
        /// </summary>
        [DataMember]
        public List<string> InvalidData { get; set; }
        /// <summary>
        /// 重复的行数据
        /// </summary>
        [DataMember]
        public List<string> RepeatList { get; set; }
    }   
}
