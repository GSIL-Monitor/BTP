using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 高德地图帮助类
    /// </summary>
    public class AampHelper
    {
        /// <summary>
        /// 高德地图获取地址对应的经纬度
        /// </summary>
        public static string GetLongLatitudeByAddress(string address, string cityCode)
        {
            string ll = ",";
            string urlParam = "output=json&address={0}&city={1}&key={2}";
            urlParam = string.Format(urlParam,address, cityCode, CustomConfig.AmapKey);
            string resultJson = WebRequestHelper.SendPostInfo(CustomConfig.AmapGetLongLatitudeByAddress, urlParam);
            LongLatitudeResult llResult = JsonHelper.JsonDeserialize<LongLatitudeResult>(resultJson);
            if (llResult != null && llResult.geocodes != null && llResult.geocodes.Any())
            {
                ll = llResult.geocodes.First().location;
            }
            return ll;
        }
    }

    /// <summary>
    /// 经纬度结果实体。
    /// </summary>
    public class LongLatitudeResult
    {
        public int count { get; set; }
        public List<geocode> geocodes { get; set; }
        public string info { get; set; }
        public string infocode { get; set; }
        public string status { get; set; }
    }

    public class geocode
    {
        public string city { get; set; }
        public string citycode { get; set; }
        public string district { get; set; }
        public string formatted_address { get; set; }
        public string location { get; set; }
        public string number { get; set; }
        public string province { get; set; }
        public string street { get; set; }
        public string township { get; set; }
    }

}
