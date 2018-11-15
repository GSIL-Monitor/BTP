using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class ShareGoldAccoutModel : ConfigurationSection
    {
        /// <summary>
        /// 电商众销账号
        /// </summary>
        [ConfigurationProperty("btpShareGoldAccount", IsRequired = false)]
        public Guid BTPShareGoldAccount
        {
            get
            {
                var str = this["btpShareGoldAccount"].ToString();
                return Guid.Parse(str);
            }

        }

        [ConfigurationProperty("btpGlodUsageId", IsRequired = false)]
        public Guid BTPGlodUsageId
        {
            get
            {
                var str = this["btpGlodUsageId"].ToString();
                return Guid.Parse(str);
            }

        }
        /// <summary>
        /// 金和众销账号
        /// </summary>
        [ConfigurationProperty("jhShareGoldAccount", IsRequired = false)]
        public Guid JHShareGoldAccount
        {
            get
            {
                var str = this["jhShareGoldAccount"].ToString();
                return Guid.Parse(str);
            }

        }

        [ConfigurationProperty("btpShareAccountPwd", IsRequired = false)]
        public string BTPShareAccountPwd
        {
            get { return this["btpShareAccountPwd"].ToString(); }
        }
        /// <summary>
        /// 电商众销帐号是否需要代金券购买金币
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
        /// <summary>
        /// 金和众销帐号是否需要代金券购买金币
        /// </summary>
        [ConfigurationProperty("isJhVoucherBuyGold", IsRequired = false)]
        public bool IsJhVoucherBuyGold
        {
            get
            {
                var str = this["isJhVoucherBuyGold"].ToString();
                return bool.Parse(str);
            }
        }

    }
}
