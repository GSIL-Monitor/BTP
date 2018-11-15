using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 发送Email工具类：Send Email
    /// </summary>
    public class EmailUtil
    {
        // 发件人Email
        private static string emailSendName = CustomConfig.EmailSendModel.EmailSendName;
        // 发件人密码
        private static string emailSendPass = CustomConfig.EmailSendModel.EmailSendPass;
        // 发送邮件服务器
        private static string emailSendHost = CustomConfig.EmailSendModel.EmailSendHost;
        // 发送邮件端口
        private static int emailSendPort = CustomConfig.EmailSendModel.EmailSendPort;
        // 是否使用ssl连接
        private static bool emailSendSsl = CustomConfig.EmailSendModel.EmailSendSsl;
        // 唯一实例
        private static EmailUtil instance = new EmailUtil();

        /// <summary>
        /// 构造方法
        /// </summary>
        private EmailUtil() { }

        /// <summary>
        /// 返回唯一实例
        /// </summary>
        public static EmailUtil getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 发送Email
        /// </summary>
        /// <param name="isHTML">是否HTML格式发送邮件</param>
        /// <param name="sendName">发件人姓名</param>
        /// <param name="title">发送邮件标题</param>
        /// <param name="body">发送邮件内容</param>
        /// <param name="toEmail">接收人地址</param>
        /// <param name="toCC">抄送地址</param>
        /// <returns>是否发送成功</returns>
        public bool SendMail(bool isHTML, string sendName, string title, string body, string toEmail, string toCC)
        {
            if (!Validate(toEmail))
            {
                //MessageBox.Show("发送邮件格式不合法!");
                return false;
            }
            MailMessage msg = new MailMessage();
            msg.To.Add(toEmail);//邮件接收人
            //可以发送给多人 
            if (Validate(toCC))
            {
                //MessageBox.Show("抄送邮件格式不合法!");
                msg.CC.Add(toCC);
            }
            msg.From = new MailAddress(emailSendName, sendName, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = body;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = isHTML;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(emailSendName, emailSendPass);
            client.Host = emailSendHost;
            client.Port = emailSendPort;//邮件发送端口
            client.EnableSsl = emailSendSsl;
            try
            {
                //client.SendAsync(msg, msg.Body);
                //简单一点儿可以
                client.Send(msg);
            }
            catch (SmtpException ex)
            {
                //MessageBox.Show(ex.Message, "发送邮件出错!");
                return false;
            }
            return true;

        }

        /// <summary>
        /// 向多人发送Email
        /// </summary>
        /// <param name="isHTML">是否HTML格式发送邮件</param>
        /// <param name="sendName">发件人姓名</param>
        /// <param name="title">发送邮件标题</param>
        /// <param name="body">发送邮件内容</param>
        /// <param name="toEmails">接收人地址集合</param>
        /// <param name="toCC">抄送地址</param>
        /// <returns>是否发送成功</returns>
        public bool SendMails(bool isHTML, string sendName, string title, string body, List<string> toEmails, string toCC)
        {

            MailMessage msg = new MailMessage();

            foreach (string toEmail in toEmails)
            {
                if (Validate(toEmail))
                {
                    msg.To.Add(toEmail);//邮件接收人
                }

            }

            //可以发送给多人
            if (Validate(toCC))
            {
                msg.CC.Add(toCC);
            }

            msg.From = new MailAddress(emailSendName, sendName, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = body;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = isHTML;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(emailSendName, emailSendPass);
            client.Host = emailSendHost;
            client.Port = emailSendPort;//邮件发送端口
            client.EnableSsl = emailSendSsl;
            try
            {
                //client.SendAsync(msg, msg.Body);
                //简单一点儿可以
                //MessageBox.Show("发送邮件成功!");
                client.Send(msg);
            }
            catch (SmtpException ex)
            {
                //MessageBox.Show(ex.Message, "发送邮件出错!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向多人发送Email
        /// </summary>
        /// <param name="isHTML">是否HTML格式发送邮件</param>
        /// <param name="sendName">发件人姓名</param>
        /// <param name="title">发送邮件标题</param>
        /// <param name="body">发送邮件内容</param>
        /// <param name="toEmails">接收人地址集合</param>
        /// <param name="toCC">抄送地址</param>
        /// <param name="attachmentList">附件集合</param>
        /// <returns>是否发送成功</returns>
        public bool SendMails(bool isHTML, string sendName, string title, string body, List<string> toEmails, string toCC, FileInfo[] attachmentList)
        {

            MailMessage msg = new MailMessage();

            foreach (string toEmail in toEmails)
            {
                if (Validate(toEmail))
                {
                    msg.To.Add(toEmail);//邮件接收人
                }

            }

            //可以发送给多人
            if (Validate(toCC))
            {
                msg.CC.Add(toCC);
            }

            msg.From = new MailAddress(emailSendName, sendName, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = body;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = isHTML;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    

            if (attachmentList != null)
            {
                for (int i = 0; i < attachmentList.Length; i++)
                {
                    if (attachmentList[i] != null)
                    {
                        Stream fileReader = new FileStream(attachmentList[i].FullName, FileMode.Open);
                        msg.Attachments.Add(new Attachment(fileReader, attachmentList[i].Name));
                    }
                }
            }

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(emailSendName, emailSendPass);
            client.Host = emailSendHost;
            client.Port = emailSendPort;//邮件发送端口
            client.EnableSsl = emailSendSsl;
            try
            {
                //client.SendAsync(msg, msg.Body);
                //简单一点儿可以
                //MessageBox.Show("发送邮件成功!");
                client.Send(msg);
            }
            catch (SmtpException ex)
            {
                //MessageBox.Show(ex.Message, "发送邮件出错!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送Email
        /// </summary>
        /// <param name="isHTML">是否HTML格式发送邮件</param>
        /// <param name="sendName">发件人姓名</param>
        /// <param name="title">发送邮件标题</param>
        /// <param name="body">发送邮件内容</param>
        /// <param name="toEmail">接收人地址</param>
        /// <param name="toCC">抄送地址</param>
        /// <param name="emailSendName">发件人Email</param>
        /// <param name="emailSendPass">发件人密码</param>
        /// <param name="emailSendHost">发送邮件服务器</param>
        /// <param name="emailSendPort">发送邮件端口</param>
        /// <param name="emailSendSsl">是否使用ssl连接</param>
        /// <returns>是否发送成功</returns>
        public bool SendMail(bool isHTML, string sendName, string title, string body, string toEmail, string toCC, string emailSendName, string emailSendPass, string emailSendHost, int emailSendPort, bool emailSendSsl)
        {
            if (!Validate(toEmail))
            {
                //MessageBox.Show("发送邮件格式不合法!");
                return false;
            }
            MailMessage msg = new MailMessage();
            msg.To.Add(toEmail);//邮件接收人
            //可以发送给多人 
            if (Validate(toCC))
            {
                //MessageBox.Show("抄送邮件格式不合法!");
                msg.CC.Add(toCC);
            }
            msg.From = new MailAddress(emailSendName, sendName, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = body;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = isHTML;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(emailSendName, emailSendPass);
            client.Host = emailSendHost;
            client.Port = emailSendPort;//邮件发送端口
            client.EnableSsl = emailSendSsl;
            try
            {
                //client.SendAsync(msg, msg.Body);
                //简单一点儿可以
                client.Send(msg);
            }
            catch (SmtpException ex)
            {
                //MessageBox.Show(ex.Message, "发送邮件出错!");
                return false;
            }
            return true;

        }

        /// <summary>
        /// 验证Email格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Validate(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            string str = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, str);
        }
    }
}
