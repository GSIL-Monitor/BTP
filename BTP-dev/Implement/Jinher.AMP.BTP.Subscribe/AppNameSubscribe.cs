using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.EMB.MB;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.ISV.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.Subscribe
{
    
    public class AppNameSubscribe : IPushHandler
    {
        public void Process(ReceiveMessage receiveMsg)
        {
            LogHelper.Debug("AppNameSubscribe Begin");

            Tuple<Guid, string> dto = receiveMsg.ReveiveData as Tuple<Guid, string>;
            if (dto != null)
            {

                CrowdfundingFacade facade = new CrowdfundingFacade();
                ResultDTO result = facade.UpdateAppName(dto);
                if (result.ResultCode != 0)
                {
                    LogHelper.Error("更新应用名失败，应用Id：" + dto.Item1);
                }
            }
            LogHelper.Debug("AppNameSubscribe End");
        }
    }
}
