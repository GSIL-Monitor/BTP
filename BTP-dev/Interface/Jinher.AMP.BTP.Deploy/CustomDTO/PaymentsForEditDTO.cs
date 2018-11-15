using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class PaymentsForEditDTO
    {
        /// <summary>
        /// 支付方式Id
        /// </summary>
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [DataMember]
        public string PaymentName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否使用
        /// </summary>
        [DataMember]
        public bool IsOnuse
        {
            get;
            set;
        }

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

        /// <summary>
        /// 是否初始化加载
        /// </summary>
        [DataMember]
        public bool isInitLoad
        {
            get;
            set;
        }
    }
}
