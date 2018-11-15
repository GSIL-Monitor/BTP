using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    [Serializable()]
    [DataContract]
    public class PaymentsVM
    {
        /// <summary>
        /// Code
        /// </summary>
        [DataMemberAttribute()]
        public string Code { get; set; }
        /// <summary>
        /// APPID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [DataMemberAttribute()]
        public bool IsOnuse { get; set; }
        /// <summary>
        /// 支付名称
        /// </summary>
        [DataMemberAttribute()]
        public string PaymentName { get; set; }
        /// <summary>
        /// 支付ID集合
        /// </summary>
        public string[] PaymentIds { get; set; }

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
        /// 积分抵现设置
        /// </summary>
        [DataMember]
        public AppScoreSettingDTO AppScoreSetting { get; set; }
    }
}
