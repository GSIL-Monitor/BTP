using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Jinher.JAP.Common.Loging;
using System.Web;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.Common
{
    public static class JdHelper
    {
        private static readonly string UrlBase = "https://bizapi.jd.com/api/";
        /// <summary>
        /// 获取京东token
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            var token = (string)HttpRuntime.Cache.Get("JD_Token");
            if (string.IsNullOrEmpty(token))
            {
                string returnObj = string.Empty;
                try
                {
                    string username = "北京中石化APP";
                    string password = GetMd5("123456");
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string clientSecret = "8MkhJOGQtFJ6586h2l8j";
                    string clientId = "beijingzhongshihua";
                    string sign = clientSecret + timestamp + clientId + username + password + "access_token" + clientSecret;
                    sign = GetMd5(sign).ToUpper();
                    string url = "https://bizapi.jd.com/oauth2/access_token";
                    string data =
                    "grant_type=access_token" +
                    "&client_id=" + clientId +
                    "&username=" + Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(username)) +
                    "&password=" + password +
                    "&timestamp=" + timestamp +
                    "&sign=" + sign;
                    returnObj = WebRequestHelper.SendPostInfo(url, data);
                    JObject objwlgs = JObject.Parse(returnObj);
                    if (objwlgs["success"].ToString() == "True")
                    {
                        JObject objson = JObject.Parse(objwlgs["result"].ToString());
                        token = objson["access_token"].ToString();
                        HttpRuntime.Cache.Insert("JD_Token", token, null, DateTime.Now.AddHours(10), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("JdHelper.GetToken 异常，returnObj: " + returnObj, ex);
                    throw ex;
                }
            }
            return token;
        }


        /// <summary>
        /// 京东确认下订单
        /// </summary>
        /// <param name="thirdOrder">第三方编号</param>
        /// <param name="JdCode">商品编号</param>
        /// <returns></returns>
        public static string GetJDOrder(string thirdOrder, string orderPriceSnap, string sku, string name, string address, string mobile, string email, string province, string city, string county, string town)
        {
            string jdOrderId = null;
            string token = GetToken();
            string url = UrlBase + "order/submitOrder";
            string data = "token=" + token + "&thirdOrder=" + thirdOrder + "&sku=" + sku + "&name=" + name + "&province=" + province + "&city=" + city + "&county=" + county + "&town=" + town + "&address=" + address;
            data += "&zip=1&phone=&mobile=" + mobile + "&email=" + email + "&invoiceState=2&invoiceType=2&selctedInvoiceTitlee=5&companyName=中国石化销售有限公司北京石油分公司&invoiceContent=1&paymentType=4&isUseBalance=0";
            data += "&submitState=0&doOrderPriceMode=1&orderPriceSnap=" + orderPriceSnap;
            string obj = WebRequestHelper.SendPostInfo(url, data);
            //发送数据到mq
            var json = SerializationHelper.JsonSerialize(new { JdOrderItem_TempId = thirdOrder, Request = data, Response = obj });
            RabbitMqHelper.Send(RabbitMqRoutingKey.JdOrderPreCreateEnd, RabbitMqExchange.Order, json);
            LogHelper.Info(string.Format("京东确认下单参数日志:{0}", data.ToString()));
            LogHelper.Info(string.Format("京东确认下单日志:{0}", obj.ToString()));
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                JObject objjson = JObject.Parse(objwlgs["result"].ToString());
                jdOrderId = objjson["jdOrderId"].ToString();
            }
            else
            {
                SessionHelper.Del("Jdlogs");
                //记录现存的缓存数据
                SessionHelper.Add("Jdlogs", objwlgs["resultMessage"].ToString());
                SessionHelper.Del("resultCode");
                //记录现存的缓存数据
                SessionHelper.Add("resultCode", objwlgs["resultCode"].ToString());
            }
            return jdOrderId;

        }


        /// <summary>
        /// 京东确认下订单
        /// </summary>
        /// <param name="thirdOrder">第三方编号</param>
        /// <param name="JdCode">商品编号</param>
        /// <returns></returns>
        public static ResultDTO GetJDOrderNew(string thirdOrder, string orderPriceSnap, string sku, string name, string address, string mobile, string email, string province, string city, string county, string town)
        {
            ResultDTO result = new ResultDTO();
            string token = GetToken();
            string url = UrlBase + "order/submitOrder";
            string data = "token=" + token + "&thirdOrder=" + thirdOrder + "&sku=" + sku + "&name=" + name + "&province=" + province + "&city=" + city + "&county=" + county + "&town=" + town + "&address=" + address;
            data += "&zip=1&phone=&mobile=" + mobile + "&email=" + email + "&invoiceState=2&invoiceType=2&selctedInvoiceTitlee=5&companyName=中国石化销售有限公司北京石油分公司&invoiceContent=1&paymentType=4&isUseBalance=0";
            data += "&submitState=0&doOrderPriceMode=1&orderPriceSnap=" + orderPriceSnap;
            string obj = WebRequestHelper.SendPostInfo(url, data);
            LogHelper.Info(string.Format("京东确认下单参数日志:{0}", data.ToString()));
            LogHelper.Info(string.Format("京东确认下单日志:{0}", obj.ToString()));
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                //JObject objjson = JObject.Parse(objwlgs["result"].ToString());
                //jdOrderId = objjson["jdOrderId"].ToString();
                //todo 返回成功后订单项的列表。
            }
            else
            {
                int rcode = 0;
                int.TryParse(objwlgs["resultCode"].ToString(), out rcode);

                result.ResultCode = rcode;
                result.Message = objwlgs["resultMessage"].ToString();
            }
            return result;
        }



        /// <summary>
        /// 京东取消未确认订单接口
        /// </summary>
        /// <returns></returns>
        public static bool OrderCancel(string jdOrderId)
        {
            bool flag = false;
            string token = GetToken();
            string url = UrlBase + "order/cancel";
            string data = "token=" + token + "&jdOrderId=" + jdOrderId;
            string obj = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["result"].ToString() == "True")
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 确认预占库存接口
        /// </summary>
        /// <returns></returns>
        public static bool confirmOrder(string jdOrderId)
        {
            bool flag = false;
            string token = GetToken();
            string url = UrlBase + "order/confirmOrder";
            string data = "token=" + token + "&jdOrderId=" + jdOrderId;
            string jsonstr = WebRequestHelper.SendPostInfo(url, data);
            LogHelper.Info("京东预占接口数据:" + jsonstr.ToString() + "");
            JObject objwlgs = JObject.Parse(jsonstr);
            if (objwlgs["result"].ToString() == "True")
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        ///  查询子订单信息接口
        /// </summary>
        /// <returns></returns>
        public static string selectJdOrder1(string jdOrderId)
        {
            string str = null;
            try
            {
                string token = GetToken();
                string data = "token=" + token + "&jdOrderId=" + jdOrderId;
                string url = UrlBase + "order/selectJdOrder";
                if (!string.IsNullOrWhiteSpace(jdOrderId))
                {
                    string obj = WebRequestHelper.SendPostInfo(url, data);
                    if (!string.IsNullOrEmpty(obj))
                    {
                        JObject objwlgs = JObject.Parse(obj);
                        if (objwlgs["success"].ToString() == "True")
                        {
                            str = objwlgs["result"].ToString();
                        }
                    }
                    else
                    {
                        str = "未查询到子订单信息";
                        LogHelper.Error("未查询到京东子订单信息,jdOrderId：" + jdOrderId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("查询囧东selectJdOrder1:{0}", ex.Message));
            }
            return str;
        }

        /// <summary>
        ///  7.4 发起支付接口
        /// </summary>
        /// <returns></returns>
        public static string doPay(string jdOrderId)
        {
            string str = null;
            try
            {
                string token = GetToken();
                string data = "token=" + token + "&jdOrderId=" + jdOrderId;
                string url = UrlBase + "order/doPay";
                if (!string.IsNullOrWhiteSpace(jdOrderId))
                {
                    string obj = WebRequestHelper.SendPostInfo(url, data);
                    if (!string.IsNullOrEmpty(obj))
                    {
                        JObject objwlgs = JObject.Parse(obj);
                        if (objwlgs["success"].ToString() == "True")
                        {
                            str = objwlgs["result"].ToString();
                        }
                    }
                    else
                    {
                        str = "发起支付接口失败";
                        LogHelper.Error("发起支付接口失败,jdOrderId：" + jdOrderId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询囧东doPay失败:{0}", ex.Message));
            }
            return str;
        }


        /// <summary>
        ///  查询父订单拆分京东订单信息接口
        /// </summary>
        /// <returns></returns>
        public static string selectJdOrder2(string jdOrderId)
        {
            string str = null;
            string token = GetToken();
            string url = UrlBase + "order/selectJdOrder";
            string data = "token=" + token + "&jdOrderId=" + jdOrderId;
            if (!string.IsNullOrWhiteSpace(jdOrderId))
            {
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    str = objwlgs["result"]["cOrder"].ToString();
                }

            }
            return str;
        }


        /// <summary>
        ///  查询子订单信息
        /// </summary>
        /// <returns></returns>
        public static Suborders SearchJdOrder(string jdOrderId)
        {
            Suborders suborders = null;
            try
            {
                string token = GetToken();
                string data = "token=" + token + "&jdOrderId=" + jdOrderId;
                string url = UrlBase + "order/selectJdOrder";
                if (!string.IsNullOrWhiteSpace(jdOrderId))
                {
                    string obj = WebRequestHelper.SendPostInfo(url, data);
                    if (!string.IsNullOrEmpty(obj))
                    {
                        JObject objwlgs = JObject.Parse(obj);
                        if (objwlgs["success"].ToString() == "True")
                        {
                            string jsonstr = objwlgs["result"].ToString();
                            if (!string.IsNullOrEmpty(jsonstr))
                            {
                                JObject firstObject = JObject.Parse(jsonstr);
                                suborders = new Suborders();
                                suborders.pOrder = firstObject["pOrder"].ToString();
                                suborders.orderState = int.Parse(firstObject["orderState"].ToString());
                                suborders.jdOrderId = firstObject["jdOrderId"].ToString();
                                suborders.freight = decimal.Parse(firstObject["freight"].ToString());
                                suborders.state = int.Parse(firstObject["state"].ToString());
                                suborders.submitState = int.Parse(firstObject["submitState"].ToString());
                                suborders.orderPrice = decimal.Parse(firstObject["orderPrice"].ToString());
                                suborders.orderNakedPrice = decimal.Parse(firstObject["orderNakedPrice"].ToString());
                                suborders.type = int.Parse(firstObject["type"].ToString());
                                suborders.orderTaxPrice = decimal.Parse(firstObject["orderTaxPrice"].ToString());
                                JArray secondObject = JArray.Parse(firstObject["sku"].ToString());
                                if (secondObject.Count > 0)
                                {
                                    List<goodssku> goodsskulist = new List<goodssku>();
                                    foreach (var item in secondObject)
                                    {
                                        goodssku model = new goodssku()
                                        {
                                            category = item["category"].ToString(),
                                            num = int.Parse(item["num"].ToString()),
                                            price = decimal.Parse(item["price"].ToString()),
                                            tax = decimal.Parse(item["tax"].ToString()),
                                            oid = int.Parse(item["oid"].ToString()),
                                            taxPrice = decimal.Parse(item["taxPrice"].ToString()),
                                            skuId = item["skuId"].ToString(),
                                            nakedPrice = decimal.Parse(item["nakedPrice"].ToString()),
                                            type = int.Parse(item["type"].ToString())
                                        };
                                        goodsskulist.Add(model);
                                    }
                                    suborders.sku = goodsskulist;
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                suborders = null;
                LogHelper.Error(string.Format("查询京东SearchJdOrder异常信息:{0}", ex.Message), ex);
            }
            return suborders;
        }


        /// <summary>
        ///  查询子订单信息
        /// </summary>
        /// <returns></returns>
        public static List<Suborders> SearchJdOrderlist(string jdOrderId)
        {
            List<Suborders> objlist = new List<Suborders>();
            try
            {
                string token = GetToken();
                string data = "token=" + token + "&jdOrderId=" + jdOrderId;
                string url = UrlBase + "order/selectJdOrder";
                if (!string.IsNullOrWhiteSpace(jdOrderId))
                {
                    string obj = WebRequestHelper.SendPostInfo(url, data);
                    if (!string.IsNullOrEmpty(obj))
                    {
                        JObject objwlgs = JObject.Parse(obj);
                        if (objwlgs["success"].ToString() == "True")
                        {
                            string jsonstr = objwlgs["result"]["cOrder"].ToString();
                            Suborders suborders = new Suborders();
                            if (!string.IsNullOrEmpty(jsonstr))
                            {
                                JArray array = JArray.Parse(jsonstr);
                                if (array.Count() > 0)
                                {
                                    foreach (var arr in array)
                                    {
                                        JObject firstObject = JObject.Parse(arr.ToString());
                                        suborders = new Suborders();
                                        suborders.pOrder = firstObject["pOrder"].ToString();
                                        suborders.orderState = int.Parse(firstObject["orderState"].ToString());
                                        suborders.jdOrderId = firstObject["jdOrderId"].ToString();
                                        suborders.freight = decimal.Parse(firstObject["freight"].ToString());
                                        suborders.state = int.Parse(firstObject["state"].ToString());
                                        suborders.submitState = int.Parse(firstObject["submitState"].ToString());
                                        suborders.orderPrice = decimal.Parse(firstObject["orderPrice"].ToString());
                                        suborders.orderNakedPrice = decimal.Parse(firstObject["orderNakedPrice"].ToString());
                                        suborders.type = int.Parse(firstObject["type"].ToString());
                                        suborders.orderTaxPrice = decimal.Parse(firstObject["orderTaxPrice"].ToString());
                                        JArray secondObject = JArray.Parse(firstObject["sku"].ToString());
                                        if (secondObject.Count > 0)
                                        {
                                            List<goodssku> goodsskulist = new List<goodssku>();
                                            foreach (var item in secondObject)
                                            {
                                                goodssku model = new goodssku()
                                                {
                                                    category = item["category"].ToString(),
                                                    num = int.Parse(item["num"].ToString()),
                                                    price = decimal.Parse(item["price"].ToString()),
                                                    tax = decimal.Parse(item["tax"].ToString()),
                                                    oid = int.Parse(item["oid"].ToString()),
                                                    taxPrice = decimal.Parse(item["taxPrice"].ToString()),
                                                    skuId = item["skuId"].ToString(),
                                                    nakedPrice = decimal.Parse(item["nakedPrice"].ToString()),
                                                    type = int.Parse(item["type"].ToString())
                                                };
                                                goodsskulist.Add(model);
                                            }
                                            suborders.sku = goodsskulist;
                                        }
                                        objlist.Add(suborders);

                                    }

                                }
                            }


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询京东SearchJdOrder异常信息:{0}", ex.Message), ex);
            }
            return objlist;
        }



        /// <summary>
        /// 判断是否是父订单还是子订单
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <returns></returns>
        public static bool IsjdOrder(string jdOrderId)
        {
            bool flag = false;
            string token = GetToken();
            string data = "token=" + token + "&jdOrderId=" + jdOrderId;
            string url = UrlBase + "order/selectJdOrder";
            if (!string.IsNullOrWhiteSpace(jdOrderId))
            {
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    JObject objson = JObject.Parse(objwlgs["result"].ToString());
                    if (objson["type"].ToString() == "1")
                    {
                        //父订单
                        flag = true;
                    }
                    else
                    {
                        //子订单
                        flag = false;
                    }
                }

            }
            return flag;
        }

        /// <summary>
        /// 查询京东配送信息接口
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <returns></returns>
        public static string orderTrack(string jdOrderId)
        {
            string str = null;
            string token = GetToken();
            string data = "token=" + token + "&jdOrderId=" + jdOrderId;
            string url = UrlBase + "order/orderTrack";
            if (!string.IsNullOrWhiteSpace(jdOrderId))
            {
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    str = objwlgs["result"]["orderTrack"].ToString();
                }
            }
            return str;
        }

        /// <summary>
        /// 确人是否拆单获取订单编号
        /// </summary>
        /// <returns></returns>
        public static string getMessage()
        {
            string jdOrderId = null;
            string token = GetToken();
            string data = "token=" + token + "&type=1";
            string url = UrlBase + "message/get";
            string obj = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                JArray objson = JArray.Parse(objwlgs["result"].ToString());
                foreach (var item in objson)
                {
                    if (item["type"].ToString() == "1")
                    {
                        JObject objjson = JObject.Parse(item["result"].ToString());
                        jdOrderId = objjson["pOrder"].ToString();
                    }
                }


            }
            return jdOrderId;

        }

        /// <summary>
        /// 获取拆单的提示信息
        /// </summary>
        /// <returns></returns>
        public static string chidanMessage()
        {
            string objson = null;
            string token = GetToken();
            string data = "token=" + token + "&type=1";
            string url = UrlBase + "message/get";
            string obj = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                objson = objwlgs["result"].ToString();
            }
            return objson;

        }

        /// <summary>
        /// 获取拆单后物流的推送信息
        /// </summary>
        /// <returns></returns>
        public static JdMessageDto JdMessage(int type)
        {
            string token = GetToken();
            string data = "token=" + token + "&type=" + type;
            string url = UrlBase + "message/get";
            string objson = WebRequestHelper.SendPostInfo(url, data);
            JObject firstjson = JObject.Parse(objson);
            JdMessageDto exEntity = new JdMessageDto();
            exEntity.success = bool.Parse(firstjson["success"].ToString());
            exEntity.resultMessage = firstjson["resultMessage"].ToString();
            exEntity.resultCode = firstjson["resultCode"].ToString();
            List<JdMessagefirst> result = new List<JdMessagefirst>();
            if (firstjson["success"].ToString() == "True")
            {
                JArray jarray = JArray.Parse(firstjson["result"].ToString());
                if (jarray.Count() > 0)
                {
                    foreach (var item in jarray)
                    {
                        JdMessagefirst jdmessagefirst = new JdMessagefirst();
                        jdmessagefirst.id = item["id"].ToString();
                        JdMessageSecond jdmessagesecond = new JdMessageSecond();
                        JObject secondjson = JObject.Parse(item["result"].ToString());
                        jdmessagesecond.orderId = secondjson["orderId"].ToString();
                        jdmessagesecond.state = int.Parse(secondjson["state"].ToString());
                        jdmessagefirst.result = jdmessagesecond;
                        jdmessagefirst.time = DateTime.Parse(item["time"].ToString());
                        jdmessagefirst.type = int.Parse(item["type"].ToString());
                        result.Add(jdmessagefirst);
                    }
                }
            }
            exEntity.result = result;
            return exEntity;

        }

        /// <summary>
        /// 删除推送信息
        /// </summary>
        /// <returns></returns>
        public static bool DelMessage(string id)
        {
            bool flag = false;
            string token = GetToken();
            string data = "token=" + token + "&id=" + id;
            string url = UrlBase + "message/del";
            string obj = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 根据第三方编号获取京东编号
        /// </summary>
        /// <returns></returns>
        public static string getJdDanhao(string thirdOrder)
        {
            string jdPOrderId = null;
            string token = GetToken();
            string data = "token=" + token + "&thirdOrder=" + thirdOrder;
            string url = UrlBase + "order/selectJdOrderIdByThirdOrder";
            string jsonstr = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(jsonstr);
            if (objwlgs["success"].ToString() == "True")
            {
                jdPOrderId = objwlgs["result"].ToString();
            }
            return jdPOrderId;
        }


        /// <summary>
        /// 根据第三方编号判断是否是京东编号
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckJdDanhao(string thirdOrder)
        {
            bool flag = false;
            string token = GetToken();
            string data = "token=" + token + "&thirdOrder=" + thirdOrder;
            string url = UrlBase + "order/selectJdOrderIdByThirdOrder";
            string jsonstr = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(jsonstr);
            if (objwlgs["success"].ToString() == "True")
            {
                flag = true;
            }
            return flag;
        }


        /// <summary>
        /// 判断京东商品sku是否支持7天无理由退货
        /// </summary>
        /// <returns></returns>
        public static bool checkProduct(string skuId)
        {
            bool is7ToReturn = false;
            string token = GetToken();
            string data = "token=" + token + "&skuIds=" + skuId;
            string url = UrlBase + "product/check";
            string obj = WebRequestHelper.SendPostInfo(url, data);
            JObject objwlgs = JObject.Parse(obj);
            if (objwlgs["success"].ToString() == "True")
            {
                if (!string.IsNullOrEmpty(objwlgs["result"].ToString()))
                {
                    JArray jarray = JArray.Parse(objwlgs["result"].ToString());
                    if (jarray.Count() > 0)
                    {
                        foreach (var item in jarray)
                        {
                            if (item["is7ToReturn"].ToString() == "1")
                            {
                                is7ToReturn = true;
                            }
                            else
                            {
                                is7ToReturn = false;
                            }
                        }
                    }

                }
            }
            return is7ToReturn;
        }



        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        public static string GetProvince()
        {
            string objson = null;
            try
            {
                string token = GetToken();
                string url = "https://bizapi.jd.com/api/area/getProvince";
                string data = "token=" + token;
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    objson = objwlgs["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                objson = null;
            }
            return objson;
        }


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        public static string GetCity(string Code)
        {
            string objson = null;
            try
            {
                string token = GetToken();
                string url = "https://bizapi.jd.com/api/area/getCity";
                string data = "token=" + token + "&id=" + Code;
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    objson = objwlgs["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                objson = null;
            }
            return objson;
        }



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        public static string GetCounty(string Code)
        {
            string objson = null;
            try
            {
                string token = GetToken();
                string url = "https://bizapi.jd.com/api/area/getCounty";
                string data = "token=" + token + "&id=" + Code;
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    objson = objwlgs["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                objson = null;
            }
            return objson;
        }


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        public static string GetTown(string Code)
        {
            string objson = null;
            try
            {
                string token = GetToken();
                string url = "https://bizapi.jd.com/api/area/getTown";
                string data = "token=" + token + "&id=" + Code;
                string obj = WebRequestHelper.SendPostInfo(url, data);
                JObject objwlgs = JObject.Parse(obj);
                if (objwlgs["success"].ToString() == "True")
                {
                    objson = objwlgs["result"].ToString();
                }
            }
            catch (Exception ex)
            {
                objson = null;
            }
            return objson;

        }


        /// <summary>
        /// 获取商品价格
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static string GetPrice(string skuId)
        {
            string objson = null;
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + skuId;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "price/getPrice", postData);
                JObject objwlgs = JObject.Parse(jsonstr);
                if (objwlgs["success"].ToString() == "True")
                {

                }
            }
            catch (Exception)
            {
                objson = null;
            }
            return objson;
        }


        /// <summary>
        /// 判断是否存在skuId
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static bool IscheckskuId(string skuId)
        {
            bool flag = false;
            try
            {
                string token = JdHelper.GetToken();
                string postData = "token=" + token + "&sku=" + skuId;
                string jsonstr = WebRequestHelper.SendPostInfo(UrlBase + "price/getPrice", postData);
                JObject objwlgs = JObject.Parse(jsonstr);
                if (objwlgs["success"].ToString() == "True")
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }







        /// <summary>
        /// md5加密算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetMd5(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            MD5 md5 = MD5.Create();
            byte[] retVal = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }


    }
}
