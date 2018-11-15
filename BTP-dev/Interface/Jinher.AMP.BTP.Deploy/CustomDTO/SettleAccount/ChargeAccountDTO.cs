using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 银行账户DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class ChargeAccountDTO
    {
        /// <summary>
        /// 银行账号
        /// </summary>
        [DataMember]
        public string BankAccount { get; set; }

        /// <summary>
        /// 开户名称
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        [DataMember]
        public string BankName { get; set; }
    }
}
