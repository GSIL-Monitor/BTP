using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */


    public class MessageCenter : OutSideServiceBase<MessageCenterFacade>
    {

    }

    public class MessageCenterFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 发送消息服务
        /// </summary>
        /// <param name="addMessageDTO"></param>
        [BTPAopLogMethod]
        public void AddMessage(MobileMessageDTO addMessageDTO)
        {
            PublishMobileMessageFacade facade = new PublishMobileMessageFacade();
            facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            facade.AddMessage(addMessageDTO);
        }
    }
}
