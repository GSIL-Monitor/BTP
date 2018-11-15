using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace Jinher.AMP.BTP.Common
{

    public class CrowdfundingAccountModel : ConfigurationSection
    {

        /// <summary>
        /// 金和众筹帐号
        /// </summary>
        [ConfigurationProperty("btpCrowdfundingAcount", IsRequired = false)]
        public Guid BTPCrowdfundingAcount
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this["btpCrowdfundingAcount"].ToString(), out result))
                {
                    return result;
                }
                return Guid.Empty;
            }
        }


        [ConfigurationProperty("btpCrowdfundingUsageId", IsRequired = false)]
        public Guid BTPCrowdfundingUsageId
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this["btpCrowdfundingUsageId"].ToString(), out result))
                {
                    return result;
                }
                return Guid.Empty;
            }

        }

        [ConfigurationProperty("jhCrowdfundingGoldAccount", IsRequired = false)]
        public Guid JhCrowdfundingGoldAccount
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this["jhCrowdfundingGoldAccount"].ToString(), out result))
                {
                    return result;
                }
                return Guid.Empty;
            }

        }
        [ConfigurationProperty("btpCrowdfundingPwd", IsRequired = false)]
        public string BTPCrowdfundingPwd
        {
            get { return this["btpCrowdfundingPwd"].ToString(); }

        }
        /// <summary>
        /// 电商众筹帐号是否需要代金券购买金币
        /// </summary>
        [ConfigurationProperty("isBtpVoucherBuyGold", IsRequired = false)]
        public bool IsBtpVoucherBuyGold
        {
            get
            {
                var str = this["isBtpVoucherBuyGold"].ToString();
                return bool.Parse(str);
            }
        }

    }
}
