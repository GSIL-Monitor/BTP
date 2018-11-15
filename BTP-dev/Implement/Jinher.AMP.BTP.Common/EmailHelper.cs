using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Jinher.AMP.BTP.Common
{
    public static class EmailHelper
    {
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="strSubject">邮件主题</param>
        /// <param name="strBody">邮件内容</param>
        public static bool SendEmail(string strSubject, string strBody, string mapTo)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(mapTo);
            msg.From = new MailAddress("ebu-app@jinher.com", "系统代发", Encoding.UTF8);//发件人邮箱，名称  
            msg.Subject = strSubject;//邮件标题  
            msg.SubjectEncoding = Encoding.UTF8;//标题格式为UTF8  
            msg.Body = strBody;//邮件内容  
            msg.BodyEncoding = Encoding.UTF8;//内容格式为UTF8  
            SmtpClient client = new SmtpClient();
            client.Host = "hwhzsmtp.qiye.163.com";//SMTP企业邮箱服务器地址  
            client.Port = 25;//SMTP端口 
            client.EnableSsl = false;//不启用SSL加密  
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("ebu-app@jinher.com", "Jinher58858686");//发件人邮箱账号，密码 
            try
            {
                client.Send(msg);//发送邮件 
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
