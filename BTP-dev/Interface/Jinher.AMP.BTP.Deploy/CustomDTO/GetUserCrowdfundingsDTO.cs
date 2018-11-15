using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class GetUserCrowdfundingsDTO
    {

        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserCrowdfundingDTO> List { get; set; }
        [DataMember]
        public int Total { get; set; }
    }
}
