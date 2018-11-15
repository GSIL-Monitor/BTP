using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class AlipayDTO
    {
        /// <summary>
        /// 合作者身份Id
        /// </summary>
        [DataMember]
        public string AliPayPartnerId
        {
            get;
            set;
        }

        /// <summary>
        /// 收款支付宝账号
        /// </summary>
        [DataMember]
        public string AliPaySeller
        {
            get;
            set;
        }

        /// <summary>
        /// 商户私钥
        /// </summary>
        [DataMember]
        public string AliPayPrivateKey
        {
            get;
            set;
        }

        /// <summary>
        /// 支付宝公钥
        /// </summary>
        [DataMember]
        public string AliPayPublicKey
        {
            get;
            set;
        }

        /// <summary>
        /// 安全校验码
        /// </summary>
        [DataMember]
        public string AliPayVerifyCode
        {
            get;
            set;
        }
    }
}
