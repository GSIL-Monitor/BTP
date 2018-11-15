using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class SaleShareModel : ConfigurationSection
    {
        [ConfigurationProperty("sharerPercent", IsRequired = false)]
        public string SharerPercent
        {
            get { return this["sharerPercent"].ToString(); }

        }

        [ConfigurationProperty("commission", IsRequired = false)]
        public decimal Commission
        {
            get
            {
                return Convert.ToDecimal(this["commission"].ToString());
            }
        }

        [ConfigurationProperty("description", IsRequired = false)]
        public string Description
        {
            get { return this["description"].ToString(); }
           
        }



        [ConfigurationProperty("dividentContent", IsRequired = false)]
        public string DividentContent
        {
            get { return this["dividentContent"].ToString(); }

        }

        [ConfigurationProperty("dividentDue", IsRequired = false)]
        public string DividentDue
        {
            get { return this["dividentDue"].ToString(); }

        }


        [ConfigurationProperty("dividentMessageDue", IsRequired = false)]
        public string DividentMessageDue
        {
            get { return this["dividentMessageDue"].ToString(); }

        }

        [ConfigurationProperty("DownAppId", IsRequired = false)]
        public string AppId
        {
            get { return this["DownAppId"].ToString(); }

        }
        /// <summary>
        /// 金和众销分成
        /// </summary>
        public decimal JinherPercent
        {
            get
            {
                decimal result = 1.0m;
                if (!string.IsNullOrEmpty(SharerPercent))
                {
                    var arr = SharerPercent.Replace('，', ',').Split(',');
                    if (arr.Length == 2)
                        result = Convert.ToDecimal(arr[1]);
                }
                return result;
            }
        }
        /// <summary>
        /// 买家众销分成
        /// </summary>
        public decimal BuyerPercent
        {
            get
            {
                decimal result = 1.0m;
                if (!string.IsNullOrEmpty(SharerPercent))
                {
                    var arr = SharerPercent.Replace('，', ',').Split(',');
                    if (arr.Length == 2)
                        result = Convert.ToDecimal(arr[0]);
                }
                return result;
            }
        }
    }
}
