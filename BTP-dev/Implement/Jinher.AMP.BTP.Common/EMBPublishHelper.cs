using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.EMB.MB;

namespace Jinher.AMP.BTP.Common
{
    public class EMBPublishHelper
    {
        static readonly string embServer = System.Web.Configuration.WebConfigurationManager.AppSettings["EMBServer"];
        /// <summary>
        /// 商品uri
        /// </summary>
        public const string Uri_BTP_ComModify = "BTP_ComModify";

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="data"></param>
        public static void PublishMessage(string topic, object data)
        {
            try
            {
                if (!string.IsNullOrEmpty(embServer))
                {
                    var eventMsg = new PublishMessage { TopicURI = topic, SendData = data, Context = { SourceURI = embServer } };
                    MessageBroker.Publish(eventMsg, embServer);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("向服务器发送消息异常，ex：" + ex);
                throw;
            }

        }
    }
}
