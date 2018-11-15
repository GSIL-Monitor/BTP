using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.UI.Models;

namespace Jinher.AMP.BTP.UI.Util
{
    /// <summary>
    /// 类名：WeixinOAuth
    /// 功能：微信认证
    /// 版本：1.0
    /// 日期：2015-05-05
    /// </summary>
    public class WeixinOAuth2
    {
        /// <summary>
        /// 获取验证地址
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="state"></param>
        /// <param name="scope"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static string GetAuthorizeUrl(string appId, string redirectUrl, string state, OAuthScope scope, string responseType = "code")
        {
            var url =
                string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&response_type={2}&scope={3}&state={4}&redirect_uri={1}#wechat_redirect",
                                appId, HttpUtility.HtmlEncode(redirectUrl), responseType, scope, state);

            /* 这一步发送之后，客户会得到授权页面，无论同意或拒绝，都会返回redirectUrl页面。
             * 如果用户同意授权，页面将跳转至 redirect_uri/?code=CODE&state=STATE。这里的code用于换取access_token（和通用接口的access_token不通用）
             * 若用户禁止授权，则重定向后不会带上code参数，仅会带上state参数redirect_uri?state=STATE
             */
            return url;
        }

        /// <summary>
        /// 返回 获取AccessToken服务的地址
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <param name="grantType"></param>
        /// <returns></returns>
        public static string GetAccessTokenByCode(string appId, string secret, string code, string grantType = "authorization_code")
        {
            var url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}";
              url =  string.Format(url, appId, secret, code, grantType);
            return url;
        }

        /// <summary>
        /// 获取access_token.
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            string emptyToken = "";

            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            url = string.Format(url,CustomConfig.WeixinAppId, CustomConfig.WeixinAppIdSecret);

            string responseText = WebRequestHelper.SendPostInfo(url, "");
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return emptyToken;
            }
            OpenModel openModel = WebUtil.FromJson<OpenModel>(responseText);
            if (openModel == null)
            {
                return emptyToken;
            }
            return openModel.access_token;
        }

        /// <summary>
        /// 获取Jsapi_Ticket
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetJsapiTicket(string accessToken)
        {
            string emptyTicket = "";

            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
            url = string.Format(url,accessToken);

            string responseText = WebRequestHelper.SendPostInfo(url, "");
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return emptyTicket;
            }
            JsapiTicketResult resultModel = WebUtil.FromJson<JsapiTicketResult>(responseText);
            if (resultModel == null || resultModel.errcode != 0)
            {
                return emptyTicket;
            }
            return resultModel.ticket;
        }


       

    }

    /// <summary>
    /// 应用授权作用域
    /// </summary>
    public enum OAuthScope
    {
        /// <summary>
        /// 不弹出授权页面，直接跳转，只能获取用户openid
        /// </summary>
        snsapi_base = 0,

        /// <summary>
        /// 弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息
        /// </summary>
        snsapi_userinfo = 1
    }
}   
   
