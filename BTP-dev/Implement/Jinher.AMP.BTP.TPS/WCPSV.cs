using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.SNS.Deploy.CustomDTO;
using Jinher.AMP.WCP.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.SNS.Deploy;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */


    public class WCPSV : OutSideServiceBase<WCPSVFacade>
    {
        /// <summary>
        /// 校验app是否包含有效的微信设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool HasWxSet(Guid appId)
        {
            var dto = Instance.GetAppkey(appId);
            if (dto != null && !dto.WAppId.IsNullVauleFromWeb() && !dto.WSecret.IsNullVauleFromWeb())
            {
                return true;
            }
            return false;
        }
    }

    public class WCPSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 发送微信消息异常
        /// </summary>
        /// <param name="msgDto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool PushSysMessageToUsers(Jinher.AMP.WCP.Deploy.CustomDTO.CusNewsPushDTO msgDto)
        {
            bool result = false;
            try
            {
                WChatDeveloperFacade facade = new WChatDeveloperFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.WCP.Deploy.CustomDTO.ReturnInfoDTO searchResult = facade.PushWXPlusMsg(msgDto);
                if (searchResult != null && searchResult.IsSuccess)
                {
                    result = true;
                }
                else
                {
                    if (searchResult != null)
                        LogHelper.Debug(string.Format("WCPSV.PushWXPlusMsg服务异常:发送微信消息异常。 msgDto：{0},接口返回：{1}。",
                            JsonHelper.JsonSerializer(msgDto), searchResult.Message));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WCPSV.PushWXPlusMsg服务异常:发送微信消息异常。 msgDto：{0}", JsonHelper.JsonSerializer(msgDto)), ex);
            }
            return result;
        }
        /// <summary>
        /// 添加微信菜单
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="menuJson"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool AddWXMenu(Guid appId, string menuJson)
        {
            try
            {
                WCP.ISV.Facade.WChatDeveloperFacade facade = new WChatDeveloperFacade();
                return facade.AddWXMenu(appId, menuJson);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WCPSV.AddWXMenu服务异常:清加微信菜单。 menuJson：{0}", menuJson), ex);
                return false;
            }
        }
        /// <summary>
        /// 获取app微信设置
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public WCP.Deploy.DevpInfoDTO GetAppkey(Guid appId)
        {
            try
            {
                //通过appId调用WCP接口获取微信公众号Id、密钥
                Jinher.AMP.WCP.ISV.Facade.WChatDeveloperFacade wChatDeFacade = new WCP.ISV.Facade.WChatDeveloperFacade();
                WCP.Deploy.DevpInfoDTO weDevpInfo = wChatDeFacade.GetAppkey(appId);
                return weDevpInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WCPSV.GetAppkey服务异常:获取微信设置。 appId：{0}", appId), ex);
                return null;
            }

        }
        /// <summary>
        /// 获取app微信设置
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.WCP.Deploy.CustomDTO.ReturnInfoDTO GetWXAccessToken(Guid appId)
        {
            try
            {
                //通过appId调用WCP接口获取微信公众号Id、密钥
                Jinher.AMP.WCP.ISV.Facade.WChatDeveloperFacade wChatDeFacade = new WCP.ISV.Facade.WChatDeveloperFacade();
                Jinher.AMP.WCP.Deploy.CustomDTO.ReturnInfoDTO result = wChatDeFacade.GetWXAccessToken(appId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WCPSV.GetWXAccessToken服务异常:获取微信设置。 appId：{0}", appId), ex);
                return null;
            }

        }
        /// <summary>
        /// 获取app微信设置
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.WCP.Deploy.CustomDTO.DeveloperInfoDTO GetDeveloperInfo(Guid appId)
        {
            try
            {
                //通过appId调用WCP接口获取微信公众号Id、密钥
                Jinher.AMP.WCP.ISV.Facade.WChatDeveloperFacade wChatDeFacade = new WCP.ISV.Facade.WChatDeveloperFacade();
                Jinher.AMP.WCP.Deploy.CustomDTO.DeveloperInfoDTO result = wChatDeFacade.GetDeveloperInfo(appId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WCPSV.GetWXAccessToken服务异常:获取微信设置。 appId：{0}", appId), ex);
                return null;
            }

        }
    }

}
