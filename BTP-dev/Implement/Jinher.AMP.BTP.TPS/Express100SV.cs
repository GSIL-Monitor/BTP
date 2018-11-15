using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Jinher.AMP.BTP.TPS
{
    /// <summary>
    /// 快递100 接口帮助
    /// </summary>
    public class Express100SV
    {
        //private static readonly string url = "https://poll.kuaidi100.com/poll/query.do";
        //private static readonly string key = "PzvmKRTa1982";
        //private static readonly string customer = "8467327D6A4B9745D3A2334D4D836F1C";
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>
        /// 快递100配置数据
        /// </summary>
        public struct Express100Data
        {
            /// <summary>
            /// 请求Url地址
            /// </summary>
            public string UrlBase { get; set; }
            /// <summary>
            /// 方正门店
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 方正密码
            /// </summary>
            public string Customer { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        private static Express100Data GetExpress100Info
        {
            get
            {
                var _UrlBase = CustomConfig.Express100_UrlBase;
                var _Key = CustomConfig.Express100_Key;
                var _Customer = CustomConfig.Express100_Customer;
                return new Express100Data
                {
                    UrlBase = _UrlBase,
                    Key = _Key,
                    Customer = _Customer
                };
            }
        }
        /// <summary>
        /// 快递100接口查询
        /// </summary>
        /// <param name="num">   //查询的快递单号， 单号的最大长度是32个字符   必须</param>
        /// <param name="shipExpCo"> 快递公司名称</param>
        public static ResultDTO<OrderExpressRouteExtendDTO> GetExpressFromKD100DTO(string num, string shipExpCo)
        {
            ResultDTO<OrderExpressRouteExtendDTO> result = new ResultDTO<OrderExpressRouteExtendDTO>();
            try
            {
                string expressJson = GetExpressFromKD100(num, shipExpCo);
              
                if (string.IsNullOrEmpty(expressJson))
                {
                    result.ResultCode = 1;
                    result.Message = "暂无数据";
                    return result;
                }
                else
                {
                   
                    OrderExpressRouteExtendDTO dTO = new OrderExpressRouteExtendDTO();
                    JObject jObject = JObject.Parse(expressJson);
                    if (jObject["message"].ToString().ToLower().Equals("ok"))
                    {
                        List<ExpressTraceDTO> listDto = new List<ExpressTraceDTO>();
                        string data = jObject["data"].ToString();
                        JArray obArr = JArray.Parse(data);
                        foreach (var item in obArr)
                        {
                            string time = item["time"].ToString();
                            string ftime = item["ftime"].ToString();
                            string context = item["context"].ToString();
                            listDto.Add(new ExpressTraceDTO
                            {
                                Id = Guid.NewGuid(),
                                ExpRouteId = Guid.NewGuid(),
                                AcceptStation = context,
                                AcceptTime = DateTime.Parse(ftime)
                            });
                        }
                        dTO.Traces = listDto;
                    }
                    else
                    {
                        result.ResultCode = 1;
                        result.Message = jObject["message"].ToString();
                    }
                    result.Data = dTO;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetExpressFromKD100DTO 错误--->快递单号【"+ num + "】快递公司名称-->【"+ shipExpCo + "】", ex);
            }
            return result;
        }
        /// <summary>
        /// 快递100接口查询
        /// </summary>
        /// <param name="num">   //查询的快递单号， 单号的最大长度是32个字符   必须</param>
        /// <param name="shipExpCo"> //查询的快递公司的编码， 一律用小写字母  必须</param>
        public static string GetExpressFromKD100(string num, string shipExpCo)
        {
            string html = string.Empty;
            try
            {
                string com = GetWuLiuCompanyCode(shipExpCo);
                Encoding encoding = Encoding.GetEncoding("utf-8");
                ////查询的快递单号， 单号的最大长度是32个字符   必须
                //string num = "3378952335790";
                ////查询的快递公司的编码， 一律用小写字母  必须
                //string com = "shentong";
                //广东深圳	出发地城市  非必须
                string from = "";
                //北京朝阳	目的地城市，到达目的地后会加大监控频率   非必须
                string to = "";
                string param = "{\"com\":\"" + com + "\",\"num\":\"" + num + "\",\"from\":\"" + from + "\",\"to\":\"" + to + "\"}";
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] InBytes = Encoding.GetEncoding("UTF-8").GetBytes(param + CustomConfig.Express100_Key + CustomConfig.Express100_Customer);
                byte[] OutBytes = md5.ComputeHash(InBytes);
                string OutString = "";
                for (int i = 0; i < OutBytes.Length; i++)
                {
                    OutString += OutBytes[i].ToString("x2");
                }
                string sign = OutString.ToUpper();
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("param", param);
                parameters.Add("customer", CustomConfig.Express100_Customer);
                parameters.Add("sign", sign);
                HttpWebResponse response = CreatePostHttpResponse(CustomConfig.Express100_UrlBase, parameters, encoding);
                //打印返回值
                Stream stream = response.GetResponseStream();   //获取响应的字符串流
                StreamReader sr = new StreamReader(stream); //创建一个stream读取流
                html = sr.ReadToEnd();   //从头读到尾，放到字符串html
                //LogHelper.Info("num=" + num + ",,,,,shipExpCo=" + shipExpCo + ",,,,,com="+ com + ",,,物流信息--->【" + html + "】");
            }
            catch (Exception ex)
            {
                html = string.Empty;
                LogHelper.Error("GetExpressFromKD100 快递100接口调用--->快递单号【" + num + "】快递公司名称-->【" + shipExpCo + "】", ex);
            }
            return html;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
        private static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            //如果需要POST数据     
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = charset.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 根据快递公司名称查询编码
        /// </summary>
        /// <param name="WuLiuCompanyName"></param>
        /// <returns></returns>
        public static string GetWuLiuCompanyCode(string WuLiuCompanyName)
        {
            try
            {
                //通过省市名称 查询出城市的编码
                string strFileName = System.AppDomain.CurrentDomain.BaseDirectory + @"dist\lib\Express100.json";


                string filetxt = ReadData(strFileName);
                //LogHelper.Info("GetWuLiuCompanyCode===" + WuLiuCompanyName + "--->" + strFileName + "====" + filetxt + "--->" + WuLiuCompanyName);

                Dictionary<string, string> dict = JsonDeserialize<Dictionary<string, string>>(filetxt);
                var kv = dict.Where(p => p.Key.Contains(WuLiuCompanyName) || WuLiuCompanyName.Contains(p.Key)).FirstOrDefault();
                if (kv.Key != null)
                {
                    return kv.Value;
                }

            }
            catch (Exception ex){
                LogHelper.Error("GetWuLiuCompanyCode===快递公司编码解析失败WuLiuCompanyName=[" + WuLiuCompanyName + "]",ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReadData(string path)
        {
            //文件在那里,对文件如何 处理,对文件内容采取的处理方式     
            try
            {
                System.Text.Encoding code = System.Text.Encoding.UTF8;
                return ReadFile(path, code);
            }
            catch (Exception)
            {
                System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
                return ReadFile(path, code);
            }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string ReadFile(string path, System.Text.Encoding code)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //仅 对文本 执行  读写操作     
                using (StreamReader sr = new StreamReader(fs, code))
                {
                    //定位操作点,begin 是一个参考点     
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    //读一下，看看文件内有没有内容，为下一步循环 提供判断依据     
                    //sr.ReadLine() 这里是 StreamReader的要领  可不是 console 中的~      
                    //假如  文件有内容     
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// json反序列化（非二进制方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
