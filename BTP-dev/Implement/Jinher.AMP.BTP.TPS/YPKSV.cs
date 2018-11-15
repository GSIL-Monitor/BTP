using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO.YPK;

namespace Jinher.AMP.BTP.TPS
{
    public static class YPKSV
    {
        static readonly string UrlBase = CustomConfig.YPK_UrlBase;
        static readonly string client_id = CustomConfig.client_id;
        static readonly string client_secret = CustomConfig.client_secret;
        static readonly string grant_type = CustomConfig.grant_type;
        static readonly string companyid = CustomConfig.companyid;
        static readonly string username = CustomConfig.username;
        static readonly string password = CustomConfig.password;
        /// <summary>
        /// 获取易派客token
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            var token = (string)HttpRuntime.Cache.Get("YPK_Token");
            if (string.IsNullOrEmpty(token))
            {
                string returnObj = string.Empty;
                try
                {
                    string url = UrlBase + "oauth/token";
                    string data = "client_id=" + client_id +
                    "&client_secret=" + client_secret +
                    "&grant_type=" + grant_type +
                    "&companyid=" + companyid +
                    "&username=" + username +
                    "&password=" + password;
                    returnObj = WebRequestHelper.SendPostInfo(url, data);
                    JObject objwlgs = JObject.Parse(returnObj);
                    if (objwlgs["success"].ToString().ToLower() == "true")
                    {
                        JObject objson = JObject.Parse(objwlgs["data"].ToString());
                        token = objson["access_token"].ToString();
                        HttpRuntime.Cache.Insert("YPK_Token", token, null, DateTime.Now.AddSeconds(7000), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("YPKSV.GetToken 异常，returnObj: " + returnObj, ex);
                }
            }
            return token;
        }
        /// <summary>
        /// 根据skuId获取易派客商品信息
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<YPKComDetailDto> GetYPKComDetail(List<string> skuIds)
        {
            List<YPKComDetailDto> list = new List<YPKComDetailDto>();
            string token = GetToken();
            foreach (var item in skuIds)
            {
                string postData = "access_token=" + token + "&data={productSkuId:" + item + ",externalCode: 1}";
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "v1/product/getProductInfo", postData);
                LogHelper.Info(string.Format("YPKSV.GetYPKComDetail 获取易派客商品详情 url:{0},返回数据:{1}", UrlBase + "v1/product/getProductInfo?" + postData, SerializationHelper.JsonSerialize(jsonstr)));
                JObject objwlgs = JObject.Parse(jsonstr);
                if (objwlgs["success"].ToString().ToLower() == "true")
                {
                    try
                    {
                        JObject objson = JObject.Parse(objwlgs["data"].ToString());
                        YPKComDetailDto com = new YPKComDetailDto();
                        com.skuId = objson["skuId"].ToString();
                        com.brandName = objson["brandName"].ToString();
                        if (string.IsNullOrWhiteSpace(objson["name"].ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            com.name = objson["name"].ToString();
                        }
                        if (string.IsNullOrWhiteSpace(objson["price"].ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            com.price = objson["price"].ToString();
                        }
                        if (string.IsNullOrWhiteSpace(objson["description"].ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            com.description = objson["description"].ToString();
                        }
                        com.weight = objson["weight"].ToString();
                        com.unit = objson["unit"].ToString();
                        com.barCode = objson["barCode"].ToString();
                        com.taxRate = objson["taxRate"].ToString();
                        com.erQiCode = objson["erQiCode"].ToString();
                        JArray array = JArray.Parse(objson["picturesPath"].ToString());
                        if (array.Count > 0)
                        {
                            var picutres = array.ToObject<List<string>>();
                            com.picturesPath = picutres.ToArray();
                        }
                        else
                        {
                            continue;
                        }
                        list.Add(com);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("YPKSV.GetYPKComDetail 获取易派客商品详情异常", ex);
                    }
                }
            }

            return list;
        }
    }
}
