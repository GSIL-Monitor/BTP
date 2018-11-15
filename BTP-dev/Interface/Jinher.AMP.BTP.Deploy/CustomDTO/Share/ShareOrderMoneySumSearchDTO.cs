using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 众销佣金统计查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShareOrderMoneySumSearchDTO
    {
        /// <summary>
        /// 分享Id
        /// </summary>
        [DataMember]
        public string ShareId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UseId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 分成类型（同收款人角色）,1:商家，2：金和众销（给金和分的钱），3：商贸众销（给分享者分的钱），4：商贸众筹，5：推广主分成，6：应用主分成，7金和分润，8买家,9一级分销,10二级分销,11三级分销,12渠道推广
        /// </summary>
        [DataMember]
        public Int32 PayeeType { get; set; }
    }
}
