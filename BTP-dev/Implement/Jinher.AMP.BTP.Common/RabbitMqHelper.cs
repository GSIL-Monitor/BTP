using Jinher.JAP.Common.Loging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.Common
{
    public class RabbitMqExchange
    {
        public const string Order = "order";
        public const string Commodity = "commodity";
    }

    public class RabbitMqRoutingKey
    {
        /// <summary>
        /// 苏宁预占完成
        /// </summary>
        public const string JdOrderPreCreateEnd = "order.precreate.jingdong.onPreCreateEnd";
        /// <summary>
        /// 京东预占完成
        /// </summary>
        public const string SnOrderPreCreateEnd = "order.precreate.suning.onPreCreateEnd";
        /// <summary>
        /// 单个订单提交完成
        /// </summary>
        public const string OrderSingleCreateEnd = "order.create.single.onCreateEnd";
        /// <summary>
        /// 跨店铺订单提交完成
        /// </summary>
        public const string OrderBatchCreateEnd = "order.create.batch.onCreateEnd";
        /// <summary>
        /// 收到京东订单消息
        /// </summary>
        public const string JdOrderMsgReceived = "order.jingdong.message.onReceived";
        /// <summary>
        /// 收到苏宁订单消息
        /// </summary>
        public const string SnOrderMsgReceived = "order.suning.message.onReceived";
        /// <summary>
        /// 收到严选订单消息
        /// </summary>
        public const string YxOrderMsgReceived = "order.yanxuan.message.onReceived";
        /// <summary>
        /// 收到京东商品消息
        /// </summary>
        public const string JdCommodityMsgReceived = "commodity.jingdong.message.onReceived";
        /// <summary>
        /// 收到苏宁商品消息
        /// </summary>
        public const string SnCommodityMsgReceived = "commodity.suning.message.onReceived";
        /// <summary>
        /// 收到苏宁价格消息
        /// </summary>
        public const string SnPriceMsgReceived = "commodity.suning.pricemessage.onReceived";
        /// <summary>
        /// 收到严选商品消息
        /// </summary>
        public const string YxCommodityMsgReceived = "commodity.yanxuan.message.onReceived";
    }

    /// <summary>
    /// 消息封装
    /// </summary>
    public class MessageWrap
    {
        /// <summary>
        /// 消息唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 消息数据
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// RoutingKey
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string MachineName { get; set; }

        public MessageWrap()
        {

        }

        public MessageWrap(string content, string routingKey)
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
            this.Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff") + "+08:00";
            this.Content = content;
            this.RoutingKey = routingKey;
            this.MachineName = Environment.MachineName;
        }
    }

    /// <summary>
    /// 消息队列帮助类
    /// </summary>
    public class RabbitMqHelper
    {
        private static IConnection _connection;
        private static IConnection Connection
        {
            get
            {
                if (_connection != null) return _connection;
                var factory = new ConnectionFactory
                {
                    HostName = CustomConfig.MqServerHost,
                    Port = 5672,
                    UserName = CustomConfig.MqUserName,
                    Password = CustomConfig.MqPassword,
                    AutomaticRecoveryEnabled = true
                };
                _connection = factory.CreateConnection();
                _connection.ConnectionShutdown += (o, e) => LogHelper.Error("RabbitMqHelper.ConnectionShutdown:" + e.ReplyText);
                return _connection;
            }
        }

        private static readonly Dictionary<string, IModel> ChannelDir = new Dictionary<string, IModel>();
        private static IModel GetChannel(string routingKey, string exchangeName)
        {
            IModel channel = null;
            if (ChannelDir.ContainsKey(routingKey))
            {
                channel = ChannelDir[routingKey];
                if (channel != null && channel.IsOpen) return channel;
                else ChannelDir.Remove(routingKey);
            }
            channel = Connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, "topic", true, false, null);
            ChannelDir.Add(routingKey, channel);
            return channel;
        }

        /// <summary>
        /// 发布数据
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="exchangeName"></param>
        /// <param name="data">发布的数据</param>
        /// <param name="isAsync">是否异步发布</param>
        public static void Send(string routingKey, string exchangeName, string data, bool isAsync = true)
        {
            data = SerializationHelper.JsonSerialize(new MessageWrap(data, routingKey));
            if (isAsync)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var bytes = Encoding.UTF8.GetBytes(data);
                        var channel = GetChannel(routingKey, exchangeName);
                        var properties = channel.CreateBasicProperties();
                        properties.SetPersistent(true);
                        channel.BasicPublish(exchangeName, routingKey, properties, bytes);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Fatal("RabbitMqHelper.send异常:" + data, ex);
                    }
                });
            }
            else
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(data);
                    var channel = GetChannel(routingKey, exchangeName);
                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);
                    channel.BasicPublish(exchangeName, routingKey, properties, bytes);
                }
                catch (Exception ex)
                {
                    LogHelper.Fatal("RabbitMqHelper.send异常:" + data, ex);
                }
            }
        }
    }
}