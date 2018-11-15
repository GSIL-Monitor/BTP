using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class JinherBtpAccountModel : ConfigurationSection
    {
        /// <summary>
        /// 电商分润帐号
        /// </summary>   
        [ConfigurationProperty("btpGoldAccount", IsRequired = false)]
        public Guid BtpGoldAccount
        {
            get
            {
                Guid result = Guid.Empty;
                string str = this["btpGoldAccount"].ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    result = Guid.Parse(str);
                }
                return result;
            }
        }
        /// <summary>
        /// 是否需要代金券购买金币
        /// </summary>
        [ConfigurationProperty("isVoucherBuyGold", IsRequired = false)]
        public bool IsVoucherBuyGold
        {
            get
            {
                var str = this["isVoucherBuyGold"].ToString();
                return bool.Parse(str);
            }
        }

    }
}
