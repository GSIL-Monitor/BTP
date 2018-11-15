using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class DownloadEInvoiceInfoJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("中石化电子发票 补发错误发票请求以及下载电子发票接口调用:DownloadEInvoiceInfoJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                //中石化电子发票 补发错误发票请求以及下载电子发票接口调用
                facace.DownloadEInvoiceInfo();

                ////中石化电子发票 部分退款商品 退完全款后 继续开正常发票
                //facace.PrCreateInvoic();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("中石化电子发票 补发错误发票请求以及下载电子发票接口调用:DownloadEInvoiceInfoJob end");
        }
    }
}
