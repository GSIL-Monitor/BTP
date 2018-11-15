using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 红包
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UserRedEnvelopeDTO
    {

        //Id, UserId, SubTime, ModifiedOn, State, DueDate, Content, GoldCount, Description, AppId
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public DateTime SubTime { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public DateTime DueDate { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public long GoldCount { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public int RedEnvelopeType { get; set; }
    }
}
