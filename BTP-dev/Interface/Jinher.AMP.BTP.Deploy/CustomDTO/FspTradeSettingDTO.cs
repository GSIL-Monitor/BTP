using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// fsp支付设置
    /// </summary>
    [Serializable]
    [DataContract]
    public class FspTradeSettingDTO
    {
        private const string TradeTypeKey = "TradeType";
        private const string IsSetWeixinPayKey = "IsSetWeixinPay";
        /// <summary>
        /// fsp支付设置
        /// </summary>
        public FspTradeSettingDTO()
        {
            _tradeType = -1;
            _isSetWeixinPay = false;
        }
        /// <summary>
        /// fsp支付设置
        /// </summary>
        /// <param name="settings"></param>
        public FspTradeSettingDTO(Dictionary<string, string> settings)
            : this()
        {
            if (settings == null || !settings.Any())
                return;
            if (settings.ContainsKey(TradeTypeKey))
                int.TryParse(settings[TradeTypeKey], out _tradeType);
            if (settings.ContainsKey(IsSetWeixinPayKey))
            {
                int setWenxinpayFlag = 0;
                int.TryParse(settings[IsSetWeixinPayKey], out setWenxinpayFlag);
                _isSetWeixinPay = setWenxinpayFlag == 1;
            }

        }
        private int _tradeType;
        /// <summary>
        /// 支付类型 0:担保交易;1：非担保交易（直接到账）
        /// </summary>
        [DataMember]
        public int TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }

        private bool _isSetWeixinPay;
        /// <summary>
        /// 是否设置微信支付 1：已设置;0：未设置
        /// </summary>
        [DataMember]
        public bool IsSetWeixinPay
        {
            get { return _isSetWeixinPay; }
            set { _isSetWeixinPay = value; }
        }
    }
}
