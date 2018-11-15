using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.EMB.MB;
using System.Web;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV.QueueBTP
{
    /// <summary>
    /// 向服务器发消息
    /// </summary>
    public class EMBPublish
    {
        static string embServer = System.Web.Configuration.WebConfigurationManager.AppSettings["EMBServer"];

        /// <summary>
        /// 是否注册过topic
        /// </summary>
        static bool isTopicRegistered = false;

        static bool isSubscriberRegistered = false;

        /// <summary>
        /// 订阅uri
        /// </summary>
        public readonly static string topicUri = "BTP_SubmitOrder";

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="message"></param>
        public static void PublishMessage(PublishMessage message)
        {
            //if (!isTopicRegistered)//只注册一次topic
            //{
            //    RegisterTopic();
            //}
            //if (!isSubscriberRegistered)  //只注册一次订阅者
            //{
            //    RegisterSubscriber();
            //}
            if (!string.IsNullOrEmpty(embServer))//发送消息
            {
                message.Context.SourceURI = embServer;
                MessageBroker.Publish(message, embServer);
            }
        }

        /// <summary>
        /// 注册topic
        /// </summary>
        private static void RegisterTopic()
        {
            TopicRegInfo topic = new TopicRegInfo()
            {
                Author = "author",
                CreateAt = DateTime.Now,
                TopicName = topicUri,
                TopicURI = topicUri,
                Version = "1.0"
            };
            if (Jinher.JAP.EMB.MB.MessageTopicManager.CreateTopic(topic))
            {
                isTopicRegistered = true;
            }
        }

        /// <summary>
        /// 注册订阅者
        /// </summary>
        private static void RegisterSubscriber()
        {
            try
            {
                SubscriberInfo subscriber = new SubscriberInfo();
                subscriber.SubscriberURI = "SubmitOrderSubscriber";
                subscriber.SubscriberHost = "localhost";
                subscriber.SubscriptionCotext = new SubscriptionContext();
                subscriber.SubscriptionCotext.SubContextItems = new Dictionary<string, object>();
                subscriber.SubscriptionCotext.SubContextItems.Add("", "");
                subscriber.PushHandler = new PushHandlerInfo();
                subscriber.PushHandler.AssemblyName = "Jinher.AMP.BTP.SV";
                subscriber.PushHandler.ClassName = "QueueBTP.OrderSubmitSubscriber";

                Jinher.JAP.EMB.MB.SubscriptionManager.RegSubscription(topicUri, subscriber);
                isSubscriberRegistered = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("注册订阅者失败",ex);
            }
        }
    }
}
