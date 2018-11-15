using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// redis消息队列执行方法委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate QueReturnInfo QueMethord<in T>(T t) where T : QueBaseDTO;

    /// <summary>
    /// redis消息队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisQueque<T> where T : QueBaseDTO, new()
    {
        private const int DefaultQueCount = 1;
        private const int DefaultMaxErrCount = 3;

        /// <summary>
        /// redis消息队列
        /// </summary>
        /// <param name="queNamePrefix">消息队列名称</param>
        /// <param name="method">消息执行方法</param>
        /// <param name="queCount">队列数量，默认为1</param>
        /// <param name="maxErrorCount">允许最大异常数，默认为3 ;超过异常数会发送邮件通知管理员</param>
        public RedisQueque(string queNamePrefix, QueMethord<T> method, int queCount = DefaultQueCount, int maxErrorCount = DefaultMaxErrCount)
        {
            _isDoTask = true;
            if (string.IsNullOrEmpty(queNamePrefix) || method == null)
                throw new ArgumentNullException();
            _queNamePrefix = queNamePrefix;
            _queCount = queCount;
            _maxErrorCount = maxErrorCount;
            _method = method;
        }

        private int _maxErrorCount;
        /// <summary>
        /// 允许最大异常数，默认为3 ;超过异常数会发送邮件通知管理员
        /// </summary>
        public int MaxErrorCount
        {
            get { return _maxErrorCount; }
            set { _maxErrorCount = value; }
        }

        /// <summary>
        /// 队列数量
        /// </summary>
        private int _queCount;
        /// <summary>
        /// 队列数量，默认为1
        /// </summary>
        public int QueCount
        {
            get { return _queCount; }
        }
        /// <summary>
        /// 队列前缀
        /// </summary>
        private string _queNamePrefix;
        /// <summary>
        /// 队列前缀
        /// </summary>
        public string QueNamePrefix
        {
            get { return _queNamePrefix; }
        }
        private QueMethord<T> _method;
        /// <summary>
        /// 消息执行方法
        /// </summary>
        public QueMethord<T> Method
        {
            get { return _method; }
        }

        /// <summary>
        /// 任务Id，自增，取自Redis
        /// </summary>
        /// <returns></returns>
        private long getTaskId()
        {
            return RedisHelper.Incr(RedisKeyConst.QueTaskId);
        }

        /// <summary>
        /// 根据key的hash值计算当前获取当前任务入队序号
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int getQuequeNo(string key)
        {
            if (_queCount <= 1)
                return 0;
            return Math.Abs(key.GetHashCode() % (_queCount - 1));
        }
        /// <summary>
        /// 获取队列id
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        private string getQuequeId(int no)
        {
            return _queNamePrefix + ":" + no;
        }
        /// <summary>
        /// 消息入队
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Push(T t)
        {
            return Push(t, true, true);
        }
        /// <summary>
        /// 消息入队
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isCalcTaskId">是否计算任务Id，新任务需要计算，重新入队任务不需要</param>
        /// <param name="isCalcRedisId">是否计算RedisId,新任务需要计算，重新入队任务不需要</param>
        /// <returns></returns>
        public bool Push(T t, bool isCalcTaskId, bool isCalcRedisId)
        {
            try
            {
                var taskId = getTaskId();
                if (t == null)
                    return false;
                if (isCalcTaskId)
                {
                    t.TaskId = taskId;
                }
                if (isCalcRedisId)
                {
                    t.RedisId = getQuequeId(getQuequeNo(t.Key));
                }
                return RedisHelper.PrependItemToList(t.RedisId, t);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 执行作业
        /// </summary>
        public void DoTask()
        {
            _isDoTask = true;
            for (int i = 0; i < _queCount; i++)
            {
                var thr = new Thread(doTask) { IsBackground = true };
                //threads.Add(thr);
                thr.Start(i);

            }

        }
        /// <summary>
        /// 停止执行作业
        /// </summary>
        public void StopTask()
        {
            _isDoTask = false;
        }

        private bool _isDoTask;
        /// <summary>
        /// 执行作业
        /// </summary>
        /// <param name="o"></param>
        private void doTask(object o)
        {
            int i = (int)o;
            string id = getQuequeId(i);
            while (_isDoTask)
            {
                var t = RedisHelper.BlockingPopItemFromList<T>(id);
                if (t == null || t.TaskId == 0)
                {
                    continue;
                }
                if (!_isDoTask)
                {
                    Push(t, false, false);
                    return;
                }
                var result = _method.Invoke(t);
                if (result == null || !result.IsSuccess)
                {
                    var hashkey = t.RedisId + ":" + t.TaskId;
                    var cnt = RedisHelper.HashIncr(RedisKeyConst.QuequeErrCount, hashkey);
                    //校验异常数是否大于设置的最大数
                    if (cnt >= _maxErrorCount)
                    {
                        LogHelper.Error(string.Format("Redis消息队列执行作业错误超过限制数量：RedisId：{0},TaskId：{1},dto:{2}", t.RedisId, t.TaskId, JsonHelper.JsonSerializer(t)));
                        continue;
                    }
                    Push(t, false, false);
                }
            }
        }
    }
    /// <summary>
    /// 消息队列dto基类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QueBaseDTO
    {
        /// <summary>
        /// 任务Id，唯一自增
        /// </summary>
        [DataMember]
        public long TaskId { get; set; }
        /// <summary>
        /// 执行结果，返回值
        /// </summary>
        [DataMember]
        public bool IsOk { get; set; }
        /// <summary>
        /// 队列分组key
        /// </summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>
        /// redis Id
        /// </summary>
        [DataMember]
        public string RedisId { get; set; }
    }
    /// <summary>
    /// redis消息队列返回结果
    /// </summary>
    public class QueReturnInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }
}
