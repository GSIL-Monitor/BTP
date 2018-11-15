using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class EmailSendModel : ConfigurationSection
    {
        /// <summary>
        /// 发件人账号
        /// </summary>
        [ConfigurationProperty("emailSendName", IsRequired = false)]
        public string EmailSendName
        {
            get { return this["emailSendName"].ToString(); }

        }
        /// <summary>
        /// 发件人密码
        /// </summary>
        [ConfigurationProperty("emailSendPass", IsRequired = false)]
        public string EmailSendPass
        {
            get { return this["emailSendPass"].ToString(); }

        }
        /// <summary>
        /// 发送邮件服务器
        /// </summary>
        [ConfigurationProperty("emailSendHost", IsRequired = false)]
        public string EmailSendHost
        {
            get { return this["emailSendHost"].ToString(); }

        }
        /// <summary>
        /// 发送邮件端口
        /// </summary>
        [ConfigurationProperty("emailSendPort", IsRequired = false)]
        public int EmailSendPort
        {
            get
            {
                int result = 25;
                var emailSendPort = this["emailSendPort"].ToString();
                if (!string.IsNullOrEmpty(emailSendPort))
                {

                    result = Convert.ToInt32(emailSendPort);
                }
                return result;
            }
        }
        /// <summary>
        /// 是否使用ssl连接
        /// </summary>
        [ConfigurationProperty("emailSendSsl", IsRequired = false)]
        public bool EmailSendSsl
        {
            get
            {
                if (this["emailSendSsl"].ToString().Contains("true"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 收件人
        /// </summary>
        [ConfigurationProperty("toEmail", IsRequired = false)]
        public string ToEmail
        {
            get { return this["toEmail"].ToString(); }

        }
        /// <summary>
        /// 抄送
        /// </summary>
        [ConfigurationProperty("toCC", IsRequired = false)]
        public string ToCC
        {
            get { return this["toCC"].ToString(); }

        }

    }
}
