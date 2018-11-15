using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.CBC.Deploy.Enum;
using Jinher.AMP.CBC.ISV.Facade;


namespace Jinher.AMP.BTP.Common
{
    public class AuthorizeHelper
    {
        private static JAP.BF.BE.Deploy.Base.ContextDTO appContext;
        private static object contextLock = new object();

        /// <summary>
        /// 匿名用户访问服务接口
        /// </summary>
        public static JAP.BF.BE.Deploy.Base.ContextDTO InitAuthorizeInfo()
        {
            var contextDTO = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current;
            //var contextDTO = Jinher.JAP.Common.Context.ApplicationContext.Current[Jinher.JAP.Common.Context.ApplicationContext.ContextKey] as ContextDTO;
            if (contextDTO != null && contextDTO.LoginUserID != Guid.Empty)
                return contextDTO;

            if (appContext == null)
            {
                lock (contextLock)
                {
                    if (appContext == null)
                    {
                        Jinher.AMP.CBC.ISV.Facade.UserFacade userSV = new CBC.ISV.Facade.UserFacade();
                        CBC.Deploy.CustomDTO.LoginInfoDTO loginDTO = new CBC.Deploy.CustomDTO.LoginInfoDTO();
                        loginDTO.AccountType = CBC.Deploy.Enum.AccountTypeEnum.Normal;
                        loginDTO.IuAccount = CustomConfig.CommonUserName;
                        loginDTO.IuPassword = CustomConfig.CommonUserPass;
                        var result = userSV.Login(loginDTO);
                        LogHelper.Info("模拟登录=" + JsonHelper.JsonSerializer(result.ContextDTO));
                        appContext = result.ContextDTO;
                    }
                }
            }

            Jinher.JAP.Common.Context.ApplicationContext.Current[Jinher.JAP.Common.Context.ApplicationContext.ContextKey] = appContext;
            return appContext;
        }


        public static JAP.BF.BE.Deploy.Base.ContextDTO CoinInitAuthorizeInfo()
        {
            return InitAuthorizeInfo();
        }
        [Obsolete("匿名登录已废弃", true)]
        public static JAP.BF.BE.Deploy.Base.ContextDTO InitUserContextInfo(Guid userId, string sessionId)
        {
            JAP.BF.BE.Deploy.Base.ContextDTO context = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.GetDefaultValue();
            context.LoginUserID = userId;
            context.SessionID = sessionId;
            context.LoginOrg = Guid.Empty;
            context.ID = null;
            context.LoginDepartment = Guid.Empty;
            Jinher.JAP.Common.Context.ApplicationContext.Current[Jinher.JAP.Common.Context.ApplicationContext.ContextKey] = context;

            return context;
        }
        public static JAP.BF.BE.Deploy.Base.ContextDTO ResetContextDTO()
        {

            Jinher.AMP.CBC.ISV.Facade.UserFacade userSV = new CBC.ISV.Facade.UserFacade();
            CBC.Deploy.CustomDTO.LoginInfoDTO loginDTO = new CBC.Deploy.CustomDTO.LoginInfoDTO();
            loginDTO.AccountType = CBC.Deploy.Enum.AccountTypeEnum.Normal;
            loginDTO.IuAccount = CustomConfig.CommonUserName;
            loginDTO.IuPassword = CustomConfig.CommonUserPass;
            var result = userSV.Login(loginDTO);
            LogHelper.Info("重置模拟登录=" + JsonHelper.JsonSerializer(result.ContextDTO));
            appContext = result.ContextDTO;
            Jinher.JAP.Common.Context.ApplicationContext.Current[Jinher.JAP.Common.Context.ApplicationContext.ContextKey] = result.ContextDTO;
            return result.ContextDTO;

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="acnt"></param>
        /// <param name="pwd"></param>
        public static LoginReturnInfoDTO Login(string acnt, string pwd)
        {
            LoginReturnInfoDTO returnInfo = null;

            LoginInfoDTO InfoDTO = new LoginInfoDTO();
            InfoDTO.AccountType = AccountTypeEnum.Anonymous;
            InfoDTO.IuAccount = acnt;
            InfoDTO.IuPassword = pwd;
            UserFacade userFacade = new UserFacade();
            try
            {
                UserFacade userManage = new UserFacade();
                returnInfo = userFacade.Login(InfoDTO);
                //将用户登录成功后返回的上下文信息放入当前Session中保存起来，以便用户下次访问非登录页面时能取到Context对象     
                return returnInfo;
            }
            catch (Exception ex)
            {
                string msg = string.Format("AuthorizeHelper.Login异常，异常信息：{0}", ex);
                LogHelper.Error(msg);

                returnInfo = new LoginReturnInfoDTO();
                returnInfo.IsSuccess = false;
                returnInfo.Message = "登录异常";
                return returnInfo;
            }
        }
        /// <summary>
        /// 判断是否单点登录
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            //没登录、登录用户为模拟登录用户 均视为未登录
            var contextDTO = Jinher.JAP.BF.BE.Deploy.Base.ContextDTO.Current;

            if (contextDTO == null || contextDTO.LoginUserID == Guid.Empty)
                return false;
            if (appContext != null && appContext.LoginUserID == contextDTO.LoginUserID)
                return false;
            return true;
        }
    }
}
