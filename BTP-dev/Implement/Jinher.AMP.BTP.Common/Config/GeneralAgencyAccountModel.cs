using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class GeneralAgencyAccountModel : ConfigurationSection
    {

        [ConfigurationProperty("generalAgencyPercent", IsRequired = false)]
        public decimal GeneralAgencyPercent
        {
            get
            {
                decimal result = 0.0m;
                string generalAgencyPercent = this["generalAgencyPercent"].ToString();
                decimal.TryParse(generalAgencyPercent, out result);
                return result;
            }
        }

        [ConfigurationProperty("jhPercent", IsRequired = false)]
        public decimal JhPercent
        {
            get
            {
                decimal result = 0.0m;
                string jhPercent = this["jhPercent"].ToString();
                decimal.TryParse(jhPercent, out result);
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
