using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class DistributorUserRelationDTO
    {
        /// <summary>
        /// 分销商Id(Distributor表的主键id,非用户Id)
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }

        /// <summary>
        /// 当前用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }


        /// <summary>
        /// 电商馆Id
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 当前用户登录账号
        /// </summary>
        [DataMember]
        public string LoginAccount { get; set; }
    }
}
