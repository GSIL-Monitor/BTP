using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.CBC.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.CBC.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.TPS
{

    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class CBCSV : OutSideServiceBase<CBCSVFacade>
    {

        /// <summary>
        /// 获取用户信息中的用户名和昵称。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetUserNameAndCode(Guid userId)
        {
            var dict = GetUserNameAndCodes(new List<Guid>() { userId });
            if (dict == null || !dict.Any())
                return new Tuple<string, string>(string.Empty, string.Empty);
            return dict.First().Value;
        }
        private static string GetAccount(List<UserAccountDTO> accounts)
        {
            string result = string.Empty;
            if (accounts == null || (!accounts.Any()))
            {
                return result;
            }
            //取手机号，如果手机号为空取 邮箱， 还为空，随便取
            var acc = accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && !c.Account.Contains('@'));
            if (acc == null)
            {
                acc = accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && c.Account.Contains('@'));

                if (acc == null)
                {
                    acc = accounts.FirstOrDefault(c => !string.IsNullOrEmpty(c.Account));
                }
            }
            if (acc != null)
            {
                return acc.Account;
            }
            return result;
        }
        /// <summary>
        /// 获取用户信息中的用户名和昵称。
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, Tuple<string, string>> GetUserNameAndCodes(List<Guid> userIds)
        {
            Dictionary<Guid, Tuple<string, string>> result = new Dictionary<Guid, Tuple<string, string>>();

            var userNamelist = Instance.GetUserNameAccountsByIds(userIds);
            if (userNamelist == null || !userNamelist.Any())
            {
                return result;
            }
            foreach (var userId in userIds)
            {
                if (!result.ContainsKey(userId))
                {

                    var user = userNamelist.FirstOrDefault(c => c.userId == userId);
                    if (user == null)
                        continue;
                    result.Add(userId, new Tuple<string, string>(user.userName, GetAccount(user.Accounts)));
                }
            }
            return result;
        }


        /// <summary>
        /// 加密UserCode
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncodeUserCode(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "";
            }

            string result = source;
            //邮箱
            if (result.Contains("@"))
            {
                string[] tmp = result.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2 && !string.IsNullOrWhiteSpace(tmp[0]))
                {
                    var tmp1 = tmp[0];
                    var tmp2 = tmp[1];
                    if (tmp1.Length <= 3)
                    {
                        result = tmp1 + "****" + "@" + tmp2;
                    }
                    else
                    {
                        var tmp1_1 = tmp1.Substring(0, 3);
                        var tmp1_2 = tmp1.Substring(3);
                        result = tmp1_1 + "****" + tmp1_2 + "@" + tmp2;
                    }
                }
            }
            else  //手机号
            {
                if (result.Length == 11)
                {
                    var tmp1 = result.Substring(0, 3);
                    var tmp2 = result.Substring(7);
                    result = tmp1 + "****" + tmp2;
                }
            }

            return result;
        }
        /// <summary>
        /// 根据用户帐号查找用户Id
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Guid GetUserIdByAccount(string account)
        {
            var ids = Instance.GetUserIdsByAccount(account);
            if (ids != null && ids.Any() && ids[0] != Guid.Empty)
                return ids[0];
            return Guid.Empty;
        }

        /// <summary>
        /// 获取中石化同步帐号(易捷北京会员编号)
        /// </summary>
        /// <param name="useId">金和用户Id</param>
        /// <returns></returns>
        public static string GetYJUserId(Guid useId)
        {
            return Instance.GetYJUserId(useId);
        }
        /// <summary>
        /// 自动注册易捷APP，如果已存在，直接返回UserId
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static UserAccountNameDTO GetUserAccountByPhone(string phone)
        {
            return Instance.GetUserAccountByPhone(phone);
        }
    }

    public class CBCSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="uids"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<UserNameAccountsDTO> GetUserNameAccountsByIds(List<Guid> uids)
        {
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserManagerFacade userManagerFacade = new CBC.ISV.Facade.UserManagerFacade();
                //userManagerFacade.ContextDTO = Jinher.AMP.BTP.Common.AuthorizeHelper.InitAuthorizeInfo();
                List<UserNameAccountsDTO> userNamelist = userManagerFacade.GetUserNameAccountsByIds(uids);
                return userNamelist;
            }
            catch (Exception ex)
            {
                string smsg = string.Format("调用Jinher.AMP.CBC.ISV.Facade.UserManagerFacade.GetUserNameAccountsByIds异常：{0}", ex);
                LogHelper.Error(smsg);
            }
            return null;
        }
        /// <summary>
        /// 根据用户获取用户注册过的组织信息
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.OrgInfoNewDTO GetOrgInfoNewBySubId(Guid subId)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoNewDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.OrganizationFacade organizationFacade = new CBC.ISV.Facade.OrganizationFacade();
                //organizationFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                orgInfoNewDTO = organizationFacade.GetOrgInfoNewBySubId(subId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetOrgInfoNewBySubId服务异常:根据用户获取用户注册过的组织信息异常。 subId：{0}", subId), ex);
            }
            return orgInfoNewDTO;
        }

        /// <summary>
        /// 获取用户名称列表
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Jinher.AMP.CBC.Deploy.CustomDTO.UserIdNameDTO> GetUserNameByIds(List<Guid> userIds)
        {
            List<Jinher.AMP.CBC.Deploy.CustomDTO.UserIdNameDTO> userIdnameDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                userIdnameDTO = userFacade.GetUserNameByIds(userIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetUserNameByIds服务异常:获取应用信息异常。 userIds：{0}", userIds), ex);
            }
            return userIdnameDTO;
        }
        /// <summary>
        /// 通过ID修改用户信息
        /// </summary>
        /// <param name="modifyDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO UpdateUserInfoByID(ModifyDTO modifyDTO)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO returnInfoDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                userFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfoDTO = userFacade.UpdateUserInfoByID(modifyDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.UpdateUserInfoByID服务异常:获取应用信息异常。 modifyDTO：{0}", modifyDTO), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 获取用户基本信息new
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isCheck"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.UserBasicInfoDTO GetUserBasicInfoNew(Guid userId, bool isCheck = false)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.UserBasicInfoDTO userBasicInfoDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                userBasicInfoDTO = userFacade.GetUserBasicInfoNew(userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetUserBasicInfoNew服务异常:获取应用信息异常。 userId：{0}", userId), ex);
            }
            return userBasicInfoDTO;
        }
        /// <summary>
        /// 用户登录验证身份信息,如果登陆成功则返回用户登录上下文,如果登陆不成功则抛出异常信息
        /// </summary>
        /// <param name="loginInfoDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.LoginReturnInfoDTO Login(LoginInfoDTO loginInfoDTO)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.LoginReturnInfoDTO loginInfoDT = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                loginInfoDT = userFacade.Login(loginInfoDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.Login服务异常:获取应用信息异常。 loginInfoDTO：{0}", loginInfoDTO), ex);
            }
            return loginInfoDT;
        }
        /// <summary>
        /// 验证手机号是否已经注册过
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO CheckAccountIsRegistered(string phone)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO returnInfoDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfoDTO = userFacade.CheckAccountIsRegistered(phone);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.CheckAccountIsRegistered服务异常:获取应用信息异常。 phone：{0}", phone), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 检查是否注册过，注册过发登录的验证码，没有注册过就发注册的验证码 
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO CheckRegisteredGenAuthCode(string phone)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.ReturnInfoDTO returnInfoDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfoDTO = userFacade.CheckRegisteredGenAuthCode(phone);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.CheckRegisteredGenAuthCode服务异常:获取应用信息异常。 phone：{0}", phone), ex);
            }
            return returnInfoDTO;
        }
        /// <summary>
        /// 带验证码的用户注册
        /// </summary>
        /// <param name="userInfoDTO"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.RegReturnInfoDTO RegisterWithAuthCode(Jinher.AMP.CBC.Deploy.UserDTO userInfoDTO, string authCode)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.RegReturnInfoDTO regReturnInfoDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                regReturnInfoDTO = userFacade.RegisterWithAuthCode(userInfoDTO, authCode);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.RegisterWithAuthCode服务异常:获取应用信息异常。 title：{0}，content：{1}", userInfoDTO, authCode), ex);
            }
            return regReturnInfoDTO;
        }
        /// <summary>
        /// 获取用户名称头像信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.UserNameIconDTO GetUserNameIconDTO(Guid userId)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.UserNameIconDTO userNameIconDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                userNameIconDTO = userFacade.GetUserNameIconDTO(userId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetUserNameIconDTO服务异常:获取应用信息异常。 userId：{0}", userId), ex);
            }
            return userNameIconDTO;
        }
        /// <summary>
        /// 通过登录账号获取人员基本信息（供EBC使用）
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.CBC.Deploy.CustomDTO.UserBasicInfoDTO GetUserBasicInfo(string Account)
        {
            Jinher.AMP.CBC.Deploy.CustomDTO.UserBasicInfoDTO userNameIconDTO = null;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                userNameIconDTO = userFacade.GetUserBasicInfo(Account);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetUserBasicInfo服务异常:获取应用信息异常。 Account：{0}", Account), ex);
            }
            return userNameIconDTO;
        }

        /// <summary>
        /// 通过登录账号获取人员基本信息
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Jinher.AMP.CBC.Deploy.CustomDTO.UserInfoWithAccountDTO> GetUserInfoWithAccountList(List<Guid> userIds)
        {
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserManagerFacade userManagerFacade = new CBC.ISV.Facade.UserManagerFacade();
                //userManagerFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var userNamelist = userManagerFacade.GetUserInfoWithAccountList(userIds);
                return userNamelist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("调用Jinher.AMP.CBC.ISV.Facade.UserManagerFacade.GetUserInfoWithAccountList异常：userIds:{0}", JsonHelper.JsonSerializer(userIds)), ex);
            }
            return null;
        }

        /// <summary>
        /// 获取第三方绑定帐号(如微信openId、中石化同步帐号)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accountSrc"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GetThirdBind(Guid userId, AccountSrcEnum accountSrc = AccountSrcEnum.ThirdAccount_WeiXinNew)
        {
            string result = string.Empty;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var sResult = userFacade.GetThirdBind(userId);
                if (sResult != null && sResult.Any())
                {
                    var item = sResult.FirstOrDefault(c => c.AccountType == accountSrc);
                    if (item != null)
                        result = item.ThirdKey;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetThirdBind服务异常:获得微信openId异常。 userId：{0}", userId), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取中石化同步帐号(易捷北京会员编号)
        /// </summary>
        /// <param name="userId">金和用户Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public string GetYJUserId(Guid userId)
        {
            string result = string.Empty;
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserManagerFacade userFacade = new CBC.ISV.Facade.UserManagerFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var sResult = userFacade.GetUserAccountYJ(userId);
                if (sResult != null)
                {
                    if (sResult.IsSuccess) return sResult.IdYJ;
                    if (sResult.StatusCode == "400") return "NoPhoneAccount";
                    LogHelper.Error(string.Format("CBCSV.GetYJUserId服务失败，{0}， userId：{1}", sResult.Message, userId));
                }
                else LogHelper.Error(string.Format("CBCSV.GetYJUserId服务失败，sResult == null， userId：{0}", userId));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetYJUserId服务异常，userId：{0}", userId), ex);
            }
            return result;
        }



        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Guid> GetUserIdsByAccount(string account)
        {
            List<Guid> userIds = new List<Guid>();
            try
            {
                Jinher.AMP.CBC.ISV.Facade.UserFacade userFacade = new CBC.ISV.Facade.UserFacade();
                //userFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                userIds = userFacade.GetUserIdsByAccount(account);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CBCSV.GetUserNameByIds服务异常:获取应用信息异常。 userIds：{0}", userIds), ex);
            }
            return userIds;
        }

        public UserAccountNameDTO GetUserAccountByPhone(string phone)
        {
            UserAccountNameDTO result = null;
            try
            {
                result = RequestHelper.CreateRequest<UserAccountNameDTO, string>(new RequestDTO<string>
                {
                    ServiceUrl = "http://" + CustomConfig.CBCHost + "/Jinher.AMP.CBC.SV.UserManagerSV.svc/GetUserAccountByPhone",
                    RequestData = JsonHelper.JsonSerializer(new PhoneHelper.UserPhone { userPhone = phone })
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error("根据phone 找到用户异常" + ex.Message);
            }
            return result;
        }
    }


}
