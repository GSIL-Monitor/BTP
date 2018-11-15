using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class ShareOwnerModel : ConfigurationSection
    {
        /// <summary>
        /// 分享出去的链接购买是否给应用主分成
        /// </summary>
        [ConfigurationProperty("isSharedShare", IsRequired = false)]
        public bool IsSharedShare
        {
            get
            {
                bool result;
                var str = this["isSharedShare"].ToString();
                bool.TryParse(str, out result);
                return result;
            }
        }
        /// <summary>
        /// 是否通过前端url给应用主分成|否则使用UserOwner表作为分成依据
        /// </summary>
        [ConfigurationProperty("isShareFromUrl", IsRequired = false)]
        public bool IsShareFromUrl
        {
            get
            {
                bool result;
                var str = this["isShareFromUrl"].ToString();
                bool.TryParse(str, out result);
                return result;
            }
        }
        [ConfigurationProperty("ownerPercent", IsRequired = false)]
        public decimal OwnerPercent
        {
            get
            {
                decimal result = 0.0m;
                string ownerPercent = this["ownerPercent"].ToString();
                decimal.TryParse(ownerPercent, out result);
                return result;
            }
        }
        [ConfigurationProperty("appMinCalcScore", IsRequired = false)]
        public int AppMinCalcScore
        {
            get
            {
                int result = 0;
                string ownerPercent = this["appMinCalcScore"].ToString();
                int.TryParse(ownerPercent, out result);
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
