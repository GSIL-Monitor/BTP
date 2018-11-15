using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.Info.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class InfoSV : OutSideServiceBase<InfoSVFacade>
    {

    }

    public class InfoSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 添加系统消息 注意点：设置 MessageType = "SystemMessage"，调用AddMessage接口
        /// </summary>
        /// <param name="messageForAddDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO AddSystemMessage(MessageForAddDTO messageForAddDTO)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO reDTO = new CBC.Deploy.CustomDTO.ReturnInfoDTO();
            try
            {
                Jinher.AMP.Info.ISV.Facade.InfoManageFacade infoManageFacade = new Info.ISV.Facade.InfoManageFacade();
                infoManageFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                if (!string.IsNullOrWhiteSpace(messageForAddDTO.Title))
                {
                    if (messageForAddDTO.Title.Length > 47)
                    {
                        messageForAddDTO.Title = messageForAddDTO.Title.Substring(0, 47) + "...";
                    }
                }
                reDTO = infoManageFacade.AddSystemMessage(messageForAddDTO);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("InfoSV.AddSystemMessage服务异常:获取应用信息异常。 messageForAddDTO：{0}", messageForAddDTO), ex);
            }
            return reDTO;
        }
    }
}
