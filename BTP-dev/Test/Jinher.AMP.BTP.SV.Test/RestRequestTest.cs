using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Jinher.AMP.CBC.Deploy.CustomDTO;

namespace Jinher.AMP.News.SV.Test
{    
    public class RestRequestTest
    {
        private string restfulUrl;
        public static string headerJson = string.Empty;
        private bool isLogin;

        public RestRequestTest(string serviceUrl, bool login = true, bool bind = false)
        {
            isLogin = login;
            if (login == true && headerJson == string.Empty)
            {
                string requestData = "{\"loginInfoDTO\":{\"IuAccount\":\"15810819038\",\"IuPassword\":\"111111\"}}";
               // restfulUrl = "http://precbc.iuoooo.com/Jinher.AMP.CBC.SV.UserSV.svc/Login";
                restfulUrl = Setting.LoginUrl;
                string retJson = this.Execute(requestData);

                var index = retJson.LastIndexOf('{');
                var context = retJson.Substring(index, retJson.Length - index -1);
                headerJson = Convert.ToBase64String(GetBytes(context));

                if (bind == true)
                {
                    bindApp();
                }
            }
            restfulUrl = Setting.RestfulUrl.TrimEnd('/') + "/" + serviceUrl.TrimStart('/');
        }

        private void bindApp()
        {
            string requestData = "{\"appId\":\"17813700-ebc0-4edb-ba43-a10c588cb617\",\"userId\":\"bc211841-b123-4e1e-a116-4fa2f9b19243\"}";
            string url = "http://app.iuoooo.com/Jinher.AMP.App.BP.UserAppBP.svc/UserBindApp";
           
            string retJson = this.Execute(requestData,url);
        }



        public string Execute(string requestData)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(restfulUrl);
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(requestData);

            myRequest.Method = "POST";
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;

            if (isLogin)
            {
                myRequest.Headers.Add("ApplicationContext", headerJson);
            }

            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();

            //获得接口返回值:List<用户Id，提醒的数量>
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string retJson = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();

            return retJson;
        }

        public string Execute(string requestData, string restUrl)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(restUrl);
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(requestData);

            myRequest.Method = "POST";
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;

            if (isLogin)
            {
                myRequest.Headers.Add("ApplicationContext", headerJson);
            }

            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();

            //获得接口返回值:List<用户Id，提醒的数量>
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string retJson = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();

            return retJson;
        }

        private byte[] GetBytes(string strData)
        {
            return Encoding.Unicode.GetBytes(strData);
        }
    }
}
