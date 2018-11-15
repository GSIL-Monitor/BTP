using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    [Serializable]
    [DataContract]
    public class UserCrowdfundingDTO
    {

        [DataMember]
        public Guid Id { get; set; }
        // 摘要:
        //     没有元数据文档可用。
        [DataMember]
        public Guid AppId { get; set; }
        //
        // 摘要:
        //     众筹表Id
        [DataMember]
        public Guid CrowdfundingId { get; set; }
        //
        // 摘要:
        //     已获得股数
        [DataMember]
        public long CurrentShareCount { get; set; }
        //
        // 摘要:
        //     本活动总购买金额
        [DataMember]
        public decimal Money { get; set; }
        //
        // 摘要:
        //     本活动总订单数
        [DataMember]
        public int OrderCount { get; set; }
        //
        // 摘要:
        //     股东累计分红
        [DataMember]
        public long TotalDividend { get; set; }


        [DataMember]
        public string strTotalDividend { get; set; }

        [DataMember]
        public long RealGetDividend { get; set; }

        [DataMember]
        public string strRealGetDividend { get; set; }
        //
        // 摘要:
        //     用户账号
        [DataMember]
        public string UserCode { get; set; }
        //
        // 摘要:
        //     没有元数据文档可用。
        [DataMember]
        public Guid UserId { get; set; }
        //
        // 摘要:
        //     用户姓名
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime SubTime { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }


    }
}
