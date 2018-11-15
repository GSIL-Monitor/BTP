using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class DiyGroupManageVM
    {
        /// <summary>
        /// 拼团Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid DiyId { get; set; }
        /// <summary>
        /// 拼团提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime DiyGroupSubTime { get; set; }
        /// <summary>
        /// 拼团编号
        /// </summary>
        [DataMemberAttribute()]
        public string DiyGroupNumber { get; set; }
        /// <summary>
        /// 拼团商品(名称)
        /// </summary>
        [DataMemberAttribute()]
        public string DiyGroupName { get; set; }
        /// <summary>
        /// 拼团价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiyGroupPrice { get; set; }
        /// <summary>
        /// 成团人数
        /// </summary>
        [DataMemberAttribute()]
        public int? DiyGroupCount { get; set; }
        /// <summary>
        /// 拼团状态
        /// </summary>
        [DataMemberAttribute()]
        public int DiyGroupState { get; set; }
        /// <summary>
        /// 拼团订单信息列表
        /// </summary>
        [DataMember]
        public List<DiyGroupManageMM> OrderDataList { get; set; }
        /// <summary>
        /// 所属AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
    }
}
