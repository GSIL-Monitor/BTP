using System;
using System.Web;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    public class StopwatchLogHelper
    {
        public const string LogName = "StopwatchLog";

        public static IDisposable BeginScope(string name)
        {
            return StopwatchLogScope.Push(name);
        }
    }

    public class StopwatchLogScope : IDisposable
    {
        private static readonly string LogScopeKey = "CurrentStopwatchLogScope";

        private readonly string _name;
        System.Diagnostics.Stopwatch _stopwatch;

        internal StopwatchLogScope(string name)
        {
            _name = name;
            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();
        }

        public StopwatchLogScope Parent { get; private set; }

        public static StopwatchLogScope Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                var c = HttpContext.Current.Items[LogScopeKey];
                return c == null ? null : (StopwatchLogScope)c;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[LogScopeKey] = value;
                }
            }
        }

        public static IDisposable Push(string name)
        {
            var temp = Current;
            Current = new StopwatchLogScope(name);
            Current.Parent = temp;
            return new DisposableScope();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                LogHelper.Info(Current.GetMessage() + ", Elapsed: " + _stopwatch.ElapsedMilliseconds + "ms", StopwatchLogHelper.LogName);
            }
        }

        private string GetMessage()
        {
            return (this.Parent != null ? this.Parent.GetMessage() + " => " : "") + "{" + this._name + "}";
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                if (Current != null)
                {
                    Current.Dispose();
                    Current = Current.Parent;
                }
            }
        }
    }
}
