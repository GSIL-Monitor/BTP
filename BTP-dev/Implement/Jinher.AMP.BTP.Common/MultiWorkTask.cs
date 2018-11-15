using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 同时执行多个任务。
    /// </summary>
    public class MultiWorkTask
    {
        /// <summary>
        /// 开始执行多个任务
        /// </summary>
        /// <param name="actionList">要执行的方法和对应的参数列表。</param>
        public void Start(List<Tuple<Action<object>, object>> actionList)
        {
            //所有任务列表。
            List<Task> taskList = new List<Task>();
            TaskFactory taskFactory = new TaskFactory();
            foreach (var act in actionList)
            {
                Task task = taskFactory.StartNew(act.Item1, act.Item2);
                taskList.Add(task);
            }
            //等待所有任务完成返回。
            Task.WaitAll(taskList.ToArray());
        } 
    }
}