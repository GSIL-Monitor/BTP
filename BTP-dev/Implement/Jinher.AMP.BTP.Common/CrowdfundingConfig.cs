using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace Jinher.AMP.BTP.Common
{

    public class CrowdfundingConfig : ConfigurationSection
    {


        [ConfigurationProperty("dividentContent", IsRequired = false)]
        public string DividentContent
        {
            get { return this["dividentContent"].ToString(); }

        }


        [ConfigurationProperty("dividentDue", IsRequired = false)]
        public double DividentDue
        {
            get
            {
                double result;
                if (double.TryParse(this["dividentDue"].ToString(), out result))
                {
                    return result;
                }
                return 24;
            }

        }

        [ConfigurationProperty("dividentMessageDue", IsRequired = false)]
        public double DividentMessageDue
        {
            get
            {
                double result;
                if (double.TryParse(this["dividentMessageDue"].ToString(), out result))
                {
                    return result;
                }
                return 48;
            }

        }
        [ConfigurationProperty("warnDividend", IsRequired = false)]
        public decimal WarnDividend
        {
            get
            {
                decimal result;
                if (decimal.TryParse(this["warnDividend"].ToString(), out result))
                {
                    return result;
                }
                return 0.5m;
            }
        }
        [ConfigurationProperty("maxDividend", IsRequired = false)]
        public decimal MaxDividend
        {
            get
            {
                decimal result;
                if (decimal.TryParse(this["maxDividend"].ToString(), out result))
                {
                    return result;
                }
                return 0.99m;
            }
        }


    }
}
