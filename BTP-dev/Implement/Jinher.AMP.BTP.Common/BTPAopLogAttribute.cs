using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.Common
{
    public class OutSideServiceBase<T> where T : OutSideFacadeBase, new()
    {
        public static T Instance = new T();
    }

    [BTPAopLog]
    public class OutSideFacadeBase : ContextBoundObject
    {
        public OutSideFacadeBase()
        {

        }
    }
    /// <summary>
    /// 日志类标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BTPAopLogAttribute : ContextAttribute, IContributeObjectSink
    {
        /// <summary>
        /// 日志类标签
        /// </summary>
        public BTPAopLogAttribute()
            : base("BTPAopLog")
        {
        }
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink next)
        {
            return new BTPAopLogHandler(next);
        }
    }

    /// <summary>
    /// 上下文处理机制
    /// </summary>
    public sealed class BTPAopLogHandler : IMessageSink
    {
        //下一个接收器
        private readonly IMessageSink _nextSink;
        /// <summary>
        /// 
        /// </summary>
        public IMessageSink NextSink
        {
            get { return _nextSink; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextSink"></param>
        public BTPAopLogHandler(IMessageSink nextSink)
        {
            _nextSink = nextSink;
        }

        /// <summary>
        /// 同步处理方法
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IMessage SyncProcessMessage(IMessage msg)
        {
            IMessage retMsg = null;

            //方法调用消息接口
            IMethodCallMessage call = msg as IMethodCallMessage;

            //如果被调用的方法没打MyInterceptorMethodAttribute标签
            if (call == null)
            {
                retMsg = _nextSink.SyncProcessMessage(msg);
            }
            else
            {
                BTPAopLogMethodAttribute methodAttr = Attribute.GetCustomAttribute(call.MethodBase, typeof(BTPAopLogMethodAttribute)) as BTPAopLogMethodAttribute;
                if (methodAttr == null)
                {
                    retMsg = _nextSink.SyncProcessMessage(msg);
                }
                //如果打了MyInterceptorMethodAttribute标签
                else
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    retMsg = _nextSink.SyncProcessMessage(msg);
                    timer.Stop();
                    string log = string.Format("{0}.{1},耗时：{2}毫秒,   入参:{3}, 出参:{4}", call.MethodBase.DeclaringType, call.MethodBase.Name, timer.ElapsedMilliseconds, getCallMethodParas(call, methodAttr.IsLogParams), JsonConvert.SerializeObject(retMsg.Properties["__Return"]));
                    LogHelper.Debug(log, BTPConstants.BTP_Outside);
                }
            }
            return retMsg;
        }

        private string getCallMethodParas(IMethodCallMessage call, bool isLogParams)
        {
            string result = "";
            if (call == null || call.MethodBase == null || call.InArgCount == 0 || !isLogParams)
                return result;
            var paras = call.MethodBase.GetParameters();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < call.InArgCount; i++)
            {
                sb.Append(paras[i].Name + ":" + JsonConvert.SerializeObject(call.InArgs[i]) + ",");
            }
            result = sb.ToString().Substring(0, sb.Length - 1);
            result += "}";

            return result;
        }

        /// <summary>
        /// 异步处理方法（不需要）
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="replySink"></param>
        /// <returns></returns>
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }
    }

    /// <summary>
    /// btp日志方法标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class BTPAopLogMethodAttribute : Attribute
    {
        /// <summary>
        /// btp日志方法标签
        /// </summary>
        public BTPAopLogMethodAttribute()
            : base()
        {
            IsLogParams = true;
        }

        /// <summary>
        /// 是否需要记录入参(默认为true)
        /// </summary>
        public bool IsLogParams { get; set; }
    }
}