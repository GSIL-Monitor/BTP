using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class JhAccountModel: ConfigurationSection
    {
        /// <summary>
        /// 金和收益帐号
        /// </summary>   
        [ConfigurationProperty("jhGoldAccount", IsRequired = false)]
        public Guid JhGoldAccount
        {
            get
            {
                Guid result = Guid.Empty;
                string str = this["jhGoldAccount"].ToString();
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
