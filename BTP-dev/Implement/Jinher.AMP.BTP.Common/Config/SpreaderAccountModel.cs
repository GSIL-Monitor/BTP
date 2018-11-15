using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class SpreaderAccountModel : ConfigurationSection
    {
        /// <summary>
        /// 推广者分成
        /// </summary>   
        [ConfigurationProperty("spreaderPercent", IsRequired = false)]
        public decimal SpreaderPercent
        {
            get
            {
                decimal result = 1.0m;
                string spreaderPercent = this["spreaderPercent"].ToString();
                if (!string.IsNullOrEmpty(spreaderPercent))
                {
                    result = Convert.ToDecimal(spreaderPercent);
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
