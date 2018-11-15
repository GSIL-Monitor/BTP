using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class EmployeeUpdateJob : IJob
    {

        public void Execute(JAP.Job.Engine.JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时更新易捷员工信息:EmployeeUpdateJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.YJEmployeeFacade facade = new ISV.Facade.YJEmployeeFacade();
                facade.UpdataYJEmployeeInfo();
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteLog(ex.Message, LogLevel.Error);
                ConsoleLog.WriteLog(ex.StackTrace, LogLevel.Error);
            }

            ConsoleLog.WriteLog("定时更新易捷员工信息:EmployeeUpdateJob...end");
        }
    }
}
