using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Util;
using Jinher.JAP.MVC.Cache;
using System.Text;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;
using System.IO;
using System.Net;
using Jinher.AMP.BTP.ISV.Facade;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 推广相关页面
    /// </summary>
    public partial class DistributeController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 我要分销-推广注册页面
        /// </summary> 
        /// <returns></returns>
        [CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "加入正品生活")]
        public ActionResult Index(Guid appId)
        {
            var esappId = appId;
            //分销商Id
            Guid distributorId = Guid.Empty;
            Guid.TryParse(Request.QueryString["distributorId"], out distributorId);
            ViewBag.distributorId = distributorId;

            Jinher.AMP.App.Deploy.ApplicationDTO appInfo = APPSV.Instance.GetAppByIdInfo(esappId, this.ContextDTO);
            ViewBag.AppIco = appInfo.Icon;
            ViewBag.AppName = appInfo.Name;

            return View();
        }

        /// <summary>
        /// 应用详情页
        /// </summary>
        /// <param name="appId">url中的appid代表商城id</param>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult AppDetail(Guid appId)
        {
            var esAppId = appId;
            bool isWxBroswer = Jinher.AMP.BTP.UI.Util.WebUtil.SideInWeixinBroswer(this.Request.UserAgent);
            ViewBag.isWxBroswer = isWxBroswer;

            Jinher.AMP.BTP.Deploy.CustomDTO.AppDetailAndPackageDTO appDetail = APPSV.Instance.GetAppDetailAndPackage(esAppId, this.ContextDTO);

            ViewBag.appName = appDetail.Name;
            ViewBag.apkUrl = appDetail.AndroidUrl;
            ViewBag.ipaUrl = appDetail.IosUrl;
            ViewBag.shelvesToAppStore = string.IsNullOrWhiteSpace(appDetail.IosUrl) ? false : true;

            return View();
        }

        /// <summary>
        /// 注册登录并绑定
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="Password"></param>
        /// <param name="Code">短信验证码</param>
        /// <param name="spreadCode">推广码</param>
        /// <returns></returns>
        public ActionResult RegisterAndLoginAndBind(string LoginId, string Password, string Code, Guid spreadCode, Guid esAppId)
        {
            try
            {
                var json = new object();


                string code = Code;
                string phone = DecodeBase64(LoginId);
                string password = DecodeBase64(Password);
                string sessId = string.Empty;
                UserDTO user = new UserDTO();
                user.AccountType = CBC.Deploy.Enum.AccountTypeEnum.Normal;
                user.Name = phone;
                user.Password = password;
                RegReturnInfoDTO returnInfo = CBCSV.Instance.RegisterWithAuthCode(user, code);

                LoginReturnInfoDTO LoginReturnInfo = new LoginReturnInfoDTO();

                Guid AppId = Guid.Empty;

                //注册成功，更新应用subId
                if (returnInfo.IsSuccess)
                {
                    //登录
                    LoginInfoDTO loginInfoDTO = new LoginInfoDTO();
                    loginInfoDTO.IuAccount = phone;
                    loginInfoDTO.IuPassword = password;

                    LoginReturnInfo = CBCSV.Instance.Login(loginInfoDTO);
                    if (LoginReturnInfo.IsSuccess)
                    {
                        sessId = LoginReturnInfo.ContextDTO.SessionID;

                        //绑定
                        DistributorUserRelationDTO durDto = new DistributorUserRelationDTO();
                        durDto.DistributorId = spreadCode;
                        durDto.EsAppId = esAppId;
                        durDto.UserId = LoginReturnInfo.ContextDTO.LoginUserID;
                        durDto.LoginAccount = phone;

                        bind(durDto);
                        return Json(new { Success = true, RegReturnInfo = returnInfo, LoginReturnInfo = LoginReturnInfo, SessionID = sessId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        json = new
                        {
                            Success = false,
                            Message = LoginReturnInfo.Message
                        };

                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    json = new
                    {
                        Success = false,
                        Message = returnInfo.Message
                    };

                    return Json(json, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("注册并登录异常。LoginId：{0}，Password：{1}，Code：{2}", LoginId, Password, Code), ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登录并绑定
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="Password"></param>
        /// <param name="spreadCode">推广码</param>
        /// <returns></returns>
        public ActionResult LoginAndBind(string LoginId, string Password, Guid spreadCode, Guid esAppId)
        {
            Jinher.AMP.EBC.Deploy.CustomDTO.ReturnInfoDTO rt = new Jinher.AMP.EBC.Deploy.CustomDTO.ReturnInfoDTO();

            Guid subId = Guid.Empty;
            Guid orgId = Guid.Empty;
            Guid AppId = Guid.Empty;

            string sessId = string.Empty;
            string loginCode = DecodeBase64(LoginId);
            string loginPassword = DecodeBase64(Password);
            //登录
            LoginInfoDTO loginInfoDTO = new LoginInfoDTO();
            loginInfoDTO.IuAccount = loginCode;
            loginInfoDTO.IuPassword = loginPassword;
            LoginReturnInfoDTO logReturnInfo = CBCSV.Instance.Login(loginInfoDTO);
            if (logReturnInfo.IsSuccess)
            {
                rt.IsSuccess = true;
                subId = logReturnInfo.ContextDTO.LoginUserID;
                orgId = logReturnInfo.ContextDTO.LoginOrg;
                sessId = logReturnInfo.ContextDTO.SessionID;

                //绑定
                DistributorUserRelationDTO durDto = new DistributorUserRelationDTO();
                durDto.DistributorId = spreadCode;
                durDto.EsAppId = esAppId;
                durDto.UserId = logReturnInfo.ContextDTO.LoginUserID;
                durDto.LoginAccount = loginCode;
                bind(durDto);
            }
            else
            {
                rt.IsSuccess = false;
                rt.Message = logReturnInfo.Message;
                rt.StatusCode = logReturnInfo.StatusCode;
            }
            return Json(new { ret = rt, SubId = subId, OrgId = orgId, SessionID = sessId }, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 商品分销。
        /// </summary>
        /// <returns></returns>
        [CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        [ArgumentExceptionDeal(ErrorMessage = "请求错误，请检查后重试~", Title = "我要分销")]
        public ActionResult CommodityDistribute(Guid commodityId, Guid appId)
        {
            //将url分享给当前用户的分销商的id
            Guid distributorId = Guid.Empty;
            Guid userId = this.ContextDTO.LoginUserID;
            Guid.TryParse(this.Request["distributorId"], out distributorId);

            IBP.Facade.DistributeFacade distributeFacade = new IBP.Facade.DistributeFacade();
            var result = distributeFacade.GetDistributeState(appId, userId);
            //未查到相关数据
            if (result.ResultCode == -1 || result.ResultCode == 0)
            {
                //添加默认申请信息
                var d_result = distributeFacade.AddDistributionIdentityInfo(this.ContextDTO.LoginUserCode, Convert.ToString(userId), Convert.ToString(appId), new DistributApplyFullDTO() { ParentId = distributorId });

                BTP.Deploy.CustomDTO.ResultDTO<DistributRuleFullDTO> ret = new ResultDTO<DistributRuleFullDTO>();
                DistributRuleFullDTO distributRuleFullDto = GetDistributRuleFullDTOByAppId(Convert.ToString(appId));
                if (distributRuleFullDto.HasCondition) // && distributRuleFullDto.TiApprovalType == ApprovalTypeEnum.Manual)
                {
                    if (!distributRuleFullDto.NeedIdentity)
                    {
                        if (distributRuleFullDto.TiApprovalType == ApprovalTypeEnum.Auto)
                        {
                            //自动成为分销商
                            if (d_result.isSuccess)
                            {
                                //添加微小店信息
                                var auditingRet = distributeFacade.AuditingDistributionApply(new Guid(d_result.Message), true, "");
                                if (auditingRet.isSuccess)
                                {
                                    var auditingRetData = auditingRet.Data;
                                    var qrCodeUrl = CreateQrCode(auditingRetData.MicroShopLogo, auditingRetData.MicroShopUrl);
                                    distributeFacade.UpdateMicroshopQrCode(new UpdateQrCodeRequestDTO
                                    {
                                        MicroShopId = auditingRetData.MicroShopId.Value,
                                        QRCodeUrl = qrCodeUrl
                                    });
                                }
                            }
                        }
                        else
                        {
                            //审核状态变更为待审核
                            distributeFacade.UpdateDistributionApplyState(appId, userId, DistributeApplyStateEnum.PendingAudit);
                        }
                    }
                    else
                    {
                        return Redirect("AddDistributionIdentity?appId=" + appId + "&commodityId=" + commodityId + "&distributorId=" + distributorId);
                    }
                }
                else
                {
                    if (d_result.isSuccess)
                    {
                        //添加微小店信息
                        var auditingRet = distributeFacade.AuditingDistributionApply(new Guid(d_result.Message), true, "");
                        if (auditingRet.isSuccess)
                        {
                            var auditingRetData = auditingRet.Data;
                            var qrCodeUrl = CreateQrCode(auditingRetData.MicroShopLogo, auditingRetData.MicroShopUrl);
                            distributeFacade.UpdateMicroshopQrCode(new UpdateQrCodeRequestDTO
                            {
                                MicroShopId = auditingRetData.MicroShopId.Value,
                                QRCodeUrl = qrCodeUrl
                            });
                        }
                    }
                }
            }
            else if (result.ResultCode == 1 || result.ResultCode == 3 || result.ResultCode == 4)
            {
                return Redirect("Distribution?appId=" + appId + "&commodityId=" + commodityId + "&distributorId=" + distributorId);
            }

            //获取商品appid
            ISV.Facade.CommodityFacade commodityFacade = new ISV.Facade.CommodityFacade();
            ResultDTO<CommodityThumb> cResultDto = commodityFacade.GetCommodityThumb(commodityId);
            appId = cResultDto.Data.AppId;

            DistributorUserRelationDTO distributorUserRelationDTO = new DistributorUserRelationDTO()
            {
                DistributorId = distributorId,
                EsAppId = appId,
                LoginAccount = this.ContextDTO.LoginUserCode,
                UserId = userId
            };

            Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> saveResult = facade.SaveDistributorRelation(distributorUserRelationDTO);
            if (saveResult == null || saveResult.ResultCode != 0)
            {
                ViewBag.Title = "我要分销";
                ViewBag.Message = saveResult.Message;
                return View("~/Views/Mobile/MobileError.cshtml");
            }

            //当前用户成为分销商后的分销商id.
            Guid distributeId = saveResult.Data;

            string distributeUrl = BTP.Common.CustomConfig.BtpDomain + "/Mobile/CommodityDetail?type=tuwen&source=share&SrcType=34&isshowsharebenefitbtn=1";
            distributeUrl += "&distributorId={0}&commodityId={1}&appId={2}";
            distributeUrl = string.Format(distributeUrl, distributeId, commodityId, appId);

            //生成短地址。
            var shortUrl = Jinher.AMP.BTP.TPS.ShortUrlSV.Instance.GenShortUrl(distributeUrl);
            //生成二维码。
            string qrCodeurl = Jinher.AMP.BTP.UI.Commons.QRCodeHelper.GenerateImgTwoCode("", shortUrl, 1);

            ViewBag.distributeShortUrl = shortUrl;
            ViewBag.distributeUrl = distributeUrl;
            ViewBag.shareImage = cResultDto.Data.PicturesPath;
            ViewBag.shareTitle = cResultDto.Data.Name;
            ViewBag.distributeQrCodeurl = qrCodeurl;

            //微小店地址
            MicroshopDTO microshopDto = distributeFacade.GetMicroshop(appId, userId);
            if (microshopDto != null)
            {
                ViewBag.distributeQrCodeurl = microshopDto.QRCodeUrl;
                ViewBag.Logo = microshopDto.Logo;
                ViewBag.Url = microshopDto.Url;
            }

            return View();
        }

        #region ghc

        /// <summary>
        ///  我的分销
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyDistribution(Guid appId)
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/').ToLower();
            string param = url.Substring(url.IndexOf("?"));

            bool isIsAppPavilion = ZPHSV.Instance.IsAppPavilion(appId);
            if (isIsAppPavilion)
            {
                return Redirect("DistributionAppList" + param);
            }
            else
            {
                return Redirect("Distribution" + param);
            }
        }

        /// <summary>
        /// 查询分销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public ActionResult GetDistributorProfits(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search)
        {
            Guid userId = this.ContextDTO.LoginUserID;
            search.UserId = userId;
            Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
            var result = facade.GetDistributorProfits(search);
            if (result != null && result.Count > 0 && result.DistributorProfitsInfoList != null && result.DistributorProfitsInfoList.Count > 0)
            {
                foreach (var item in result.DistributorProfitsInfoList)
                {
                    item.UserCode = CBCSV.EncodeUserCode(item.UserCode);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<bool, string> CheckValidCode(string validCode)
        {
            Dictionary<bool, string> dic = new Dictionary<bool, string>();
            if (string.IsNullOrEmpty(validCode))
            {
                dic.Add(false, "请输入验证码");
                return dic;
            }
            if (SessionCache.Current.GetCache("ValidCodeGenerateTime") != null && !string.IsNullOrEmpty(SessionCache.Current.GetCache("ValidCodeGenerateTime").ToString()))
            {
                DateTime endTime = DateTime.Now;
                DateTime beginTime = DateTime.Parse(SessionCache.Current.GetCache("ValidCodeGenerateTime").ToString());
                long timeDiff = DateDiff(beginTime, endTime);
                if (timeDiff > EXPIRATION_TIME)
                {
                    dic.Add(false, "您输入的验证码有误");
                    return dic;
                }
            }
            if (SessionCache.Current.GetCache("ValidCode") != null && validCode.ToLower() != SessionCache.Current.GetCache("ValidCode").ToString().ToLower())
            {
                dic.Add(false, "您输入的验证码有误");
                return dic;
            }
            return dic;
        }
        /// <summary>
        /// 验证码过期时间5分钟
        /// </summary>
        private static long EXPIRATION_TIME = 5 * 60;

        private long DateDiff(DateTime arg_StartDate, DateTime arg_EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan objTimeSpan = new System.TimeSpan(arg_EndDate.Ticks - arg_StartDate.Ticks);

            lngDateDiffValue = (long)objTimeSpan.TotalSeconds;
            return (lngDateDiffValue);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

        #endregion

        /// <summary>
        /// 我的分销佣金明细（已入积分账户、待入账）
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult MyDistributeMoneyDetail()
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/').ToLower();
            string param = url.Substring(url.IndexOf("?"));

            Guid appId = new Guid(Request["appId"]);
            Guid userId = this.ContextDTO.LoginUserID;

            //0：没有三级分销功能；1：有三级分销功能并且已是分销商；2：有三级分销功能但不是分销商
            int distributorType = 0;

            // 校验app是否选用三级分销功能项
            if (Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
            {
                Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO relationDTO = new DistributorUserRelationDTO()
                {
                    EsAppId = appId,
                    UserId = userId
                };
                //是否是分销商
                var distriborInfo = facade.GetDistributorInfo(relationDTO);
                if (distriborInfo == null)
                {
                    distributorType = 2;
                }
                else
                {
                    distributorType = 1;

                    ViewBag.DistributorInfo = distriborInfo;
                }
            }
            else
            {
                distributorType = 0;
            }
            ViewBag.DistributorType = distributorType;
            if (distributorType == 0)
            {
                ViewBag.Title = "佣金累计明细";
                ViewBag.Message = "抱歉，暂不支持该功能";
                return View("~/Views/Mobile/MobileError.cshtml");
            }
            else if (distributorType == 2)
            {
                return Redirect("Distribution" + param);
            }
            return View();
        }

        /// <summary>
        /// 获取佣金
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult GetDistributorMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch search)
        {
            Guid userId = this.ContextDTO.LoginUserID;
            search.UserId = userId;
            Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
            var result = facade.GetDistributorMoneyInfo(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 我的分销
        /// </summary>
        /// <returns></returns>
        [CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        [ArgumentExceptionDeal(ErrorMessage = "请求错误，请检查后重试~", Title = "我要分销")]
        public ActionResult Distribution()
        {
            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();
            Guid appId = new Guid();
            Guid userId = this.ContextDTO.LoginUserID;

            if (!string.IsNullOrEmpty(Request["appId"]))
            {
                appId = new Guid(Request["appId"]);
            }
            if (!string.IsNullOrEmpty(Request["shopAppId"]))
            {
                appId = new Guid(Request["shopAppId"]);
            }
            // 校验app是否选用三级分销功能项
            if (!Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
            {
                ViewBag.Title = "我的分销";
                ViewBag.Message = "抱歉，暂不支持该功能";
                return View("~/Views/Mobile/MobileError.cshtml");
            }

            Guid commodityId = new Guid();
            if (!string.IsNullOrEmpty(Request["commodityId"]) && Request["commodityId"] != "null")
            {
                commodityId = new Guid(Request["commodityId"]);
            }
            Guid distributorId = new Guid();
            if (!string.IsNullOrEmpty(Request["distributorId"]) && Request["distributorId"] != "null")
            {
                distributorId = new Guid(Request["distributorId"]);
            }
            var result = facade.GetDistributeState(appId, userId);

            IBP.Facade.DistributeFacade distributeFacade = new IBP.Facade.DistributeFacade();
            switch (result.ResultCode)
            {
                case -1:
                    //添加默认申请信息
                    var d_result = distributeFacade.AddDistributionIdentityInfo(this.ContextDTO.LoginUserCode, Convert.ToString(userId), Convert.ToString(appId), new DistributApplyFullDTO() { ParentId = distributorId });
                    BTP.Deploy.CustomDTO.ResultDTO<DistributRuleFullDTO> ret = new ResultDTO<DistributRuleFullDTO>();
                    DistributRuleFullDTO distributRuleFullDto = GetDistributRuleFullDTOByAppId(Convert.ToString(appId));
                    if (distributRuleFullDto.HasCondition) // && distributRuleFullDto.TiApprovalType == ApprovalTypeEnum.Manual)
                    {
                        return Redirect("Distribution?appId=" + appId);
                    }
                    else
                    {
                        if (d_result.isSuccess)
                        {
                            //添加微小店信息
                            var auditingRet = distributeFacade.AuditingDistributionApply(new Guid(d_result.Message), true, "");
                            if (auditingRet.isSuccess)
                            {
                                var auditingRetData = auditingRet.Data;
                                var qrCodeUrl = CreateQrCode(auditingRetData.MicroShopLogo, auditingRetData.MicroShopUrl);
                                distributeFacade.UpdateMicroshopQrCode(new UpdateQrCodeRequestDTO
                                {
                                    MicroShopId = auditingRetData.MicroShopId.Value,
                                    QRCodeUrl = qrCodeUrl
                                });
                                return Redirect("DistributeDetail?appId=" + appId + "&source=share");
                            }
                        }
                    }
                    break;
                case 0:
                    ViewBag.info = "您还不是分销商，填写资料即可加入分销~";
                    ViewBag.active = "点击填写资料";
                    ViewBag.url = "/Distribute/AddDistributionIdentity?appId=" + appId + "&IsModified=false";
                    break;
                case 1:
                case 4:
                    DistributRuleFullDTO distributRuleFullDto1 = GetDistributRuleFullDTOByAppId(Convert.ToString(appId));
                    if (distributRuleFullDto1.TiApprovalType == ApprovalTypeEnum.Auto)
                    {
                        //添加微小店信息
                        DistributionApplyDTO dresult = distributeFacade.GetDistributionApply(appId, userId);
                        var auditingRet = distributeFacade.AuditingDistributionApply(dresult.Id, true, "");
                        if (auditingRet.isSuccess)
                        {
                            var auditingRetData = auditingRet.Data;
                            var qrCodeUrl = CreateQrCode(auditingRetData.MicroShopLogo, auditingRetData.MicroShopUrl);
                            distributeFacade.UpdateMicroshopQrCode(new UpdateQrCodeRequestDTO
                            {
                                MicroShopId = auditingRetData.MicroShopId.Value,
                                QRCodeUrl = qrCodeUrl
                            });
                        }

                        return Redirect("CommodityDistribute?appId=" + appId + "&commodityId=" + commodityId +
                                        "&distributorId=" + distributorId);
                    }

                    ViewBag.info = "您的资料已提交审核，请耐心等待~";
                    ViewBag.active = "点击查看提交的资料";
                    ViewBag.url = "/Distribute/AddDistributionIdentity?appId=" + appId + "&isLook=true";
                    break;
                case 2:
                    return Redirect("DistributeDetail?appId=" + appId + "&source=share");
                case 3:
                    ViewBag.info = "抱歉，您的申请由于“" + result.Message + "”原因被拒绝，别气馁，再接再厉哦~";
                    ViewBag.active = "点击编辑提交的资料";
                    ViewBag.url = "/Distribute/AddDistributionIdentity?appId=" + appId + "&IsModified=true";
                    break;
            }

            return View();
        }

        /// <summary>
        /// 获取app列表
        /// </summary>
        /// <param name="appId">馆ID</param>
        /// <returns></returns>
        public ActionResult GetAppList(Guid appId)
        {
            AppSetSearch2DTO search = new AppSetSearch2DTO
            {
                belongTo = appId,
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            IBP.Facade.SelectCommodityFacade facade = new IBP.Facade.SelectCommodityFacade();
            var result = facade.GetAppList(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分销品牌
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult DistributionAppList()
        {
            return View();
        }

        /// <summary>
        /// 分销页面
        /// </summary>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult DistributeDetail()
        {
            var url = Request.Url.ToString();
            url = url.TrimEnd('/').ToLower();
            string param = url.Substring(url.IndexOf("?"));
            
            Guid appId = new Guid();
            if (!string.IsNullOrEmpty(Request["appId"]))
            {
                appId = new Guid(Request["appId"]);
            }
            else if (!string.IsNullOrEmpty(Request["shopAppId"]))
            {
                appId = new Guid(Request["shopAppId"]);
            }

            Guid userId = this.ContextDTO.LoginUserID;

            //0：没有三级分销功能；1：有三级分销功能并且已是分销商；2：有三级分销功能但不是分销商
            int distributorType = 0;

            // 校验app是否选用三级分销功能项
            if (Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(appId))
            {
                Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
                Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO relationDTO = new DistributorUserRelationDTO()
                {
                    EsAppId = appId,
                    UserId = userId
                };
                //是否是分销商
                var distriborInfo = facade.GetDistributorInfo(relationDTO);
                if (distriborInfo == null)
                {
                    distributorType = 2;
                }
                else
                {
                    distributorType = 1;

                    ViewBag.DistriborInfo = distriborInfo;
                    Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search = new Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO();
                    search.AppId = appId;
                    search.UserId = userId;
                    search.SearchOneOrMore = 0;
                    search.SearchType = 0;
                    search.PageSize = 1;
                    search.PageIndex = 1;
                    var result = facade.GetDistributorProfits(search);
                    if (result != null && result.DistributorProfitsInfoList != null && result.DistributorProfitsInfoList.Count > 0)
                    {
                        ViewBag.DistributorProfitsInfo = result.DistributorProfitsInfoList.FirstOrDefault();
                    }
                    else
                    {
                        DistributorProfitsInfoDTO defaultInfo = new DistributorProfitsInfoDTO();
                        ViewBag.DistributorProfitsInfo = defaultInfo;
                    }
                }
            }
            else
            {
                distributorType = 0;
            }
            ViewBag.DistributorType = distributorType;
            if (distributorType == 0)
            {
                ViewBag.Title = "我的分销";
                ViewBag.Message = "抱歉，暂不支持该功能";
                return View("~/Views/Mobile/MobileError.cshtml");
            }
            else if (distributorType == 2)
            {
                return Redirect("Distribution" + param);
            }

            //获取微小店信息
            IBP.Facade.DistributeFacade distributeFacade = new IBP.Facade.DistributeFacade();
            var distributionMicroShop = distributeFacade.GetDistributionMicroShop(appId, userId);
            //微小店地址
            if (distributionMicroShop != null)
            {
                ViewBag.DistributionMicroShopDTO = distributionMicroShop;
                ViewBag.Url = distributionMicroShop.Url;
            }

            return View();
        }

        /// <summary>
        /// 微小店商品信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetDistributionMicroShop(string appId)
        {
            Guid userId = this.ContextDTO.LoginUserID;

            IBP.Facade.DistributeFacade distributeFacade = new IBP.Facade.DistributeFacade();
            var distributionMicroShop = distributeFacade.GetDistributionMicroShop(new Guid(appId), userId);

            return Json(distributionMicroShop, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public ActionResult SaveMicroshopCom(string commodityId, string microshopId)
        {
            MicroshopComDTO microshopComDto = new MicroshopComDTO
            {
                CommodityId = new Guid(commodityId),
                MicroshopId = new Guid(microshopId)
            };
            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();
            var result = facade.SaveMicroshopCom(microshopComDto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public ActionResult UpdateMicroshopCom(string commodityId, string microshopId)
        {
            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();
            var result = facade.UpdateMicroshopCom(new Guid(commodityId), new Guid(microshopId));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMicroshopName()
        {
            return View();
        }

        /// <summary>
        /// 微小店信息修改
        /// </summary>
        /// <param name="microshopDto"></param>
        /// <returns></returns>
        [DealMobileUrl]
        public ActionResult UpdateDistributionMicroShop(MicroshopDTO microshopDto)
        {
            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();

            if (!string.IsNullOrWhiteSpace(microshopDto.Logo))
            {
                microshopDto.QRCodeUrl = CreateQrCode(microshopDto.Logo, microshopDto.Url);
            }

            var result = facade.UpdateDistributionMicroShop(microshopDto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string CreateQrCode(string fileImg, string replaceUrl)
        {
            string qrCode = "";
            try
            {
                fileImg = fileImg.Replace("\\", "");
                //网络图片读取
                WebClient mywebclient = new WebClient();
                var imgfile = mywebclient.DownloadData(fileImg);

                //本地图片读取
                //var imgfile = Jinher.AMP.EBC.Common.ImageHelper.ConvertFileToBinary(fileImg);

                var qRCodeWithIconDto = new Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO
                {
                    IconDate = imgfile,
                    Source = replaceUrl
                };

                //生成带图片的二维码
                var codepath = TPS.BaseAppToolsSV.Instance.GenQRCodeWithIcon(qRCodeWithIconDto);
                qrCode = CustomConfig.CommonFileServerUrl + codepath;
            }
            catch (Exception ex)
            {
                string errStack = ex.Message + ex.StackTrace;
                while (ex.InnerException != null)
                {
                    errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                    ex = ex.InnerException;
                }
                LogHelper.Error("CreateQrCode异常，异常信息：" + errStack, ex);
            }
            return qrCode;
        }

        private bool bind(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.DistributorFacade facade = new ISV.Facade.DistributorFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> result = facade.SaveDistributorRelation(distributorUserRelationDTO);
                if (result.ResultCode == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 分销商审核状态
        /// </summary>
        /// <param name="appId">馆ID</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ActionResult GetDistributeState(string appId, string userId)
        {
            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();

            Guid gappId = Guid.Empty;
            if (!string.IsNullOrEmpty(appId))
            {
                gappId = new Guid(appId);
            }
            Guid guserId = Guid.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                guserId = new Guid(userId);
            }
            var result = facade.GetDistributeState(gappId, guserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取分销商规则
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult ShowRuleDescription(string appId)
        {
            Guid gappId = Guid.Empty;
            if (!string.IsNullOrEmpty(appId))
            {
                gappId = new Guid(appId);
            }

            IBP.Facade.DistributeFacade facade = new IBP.Facade.DistributeFacade();
            var rd = facade.GetDistributionRule(gappId);
            ViewBag.RuleDesc = rd != null ? rd.RuleDesc : "";
            return View();
        }

        /// <summary>
        /// 分销商资料页面
        /// </summary>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        public ActionResult AddDistributionIdentity(string appId)
        {
            bool isLook = false;
            if (!string.IsNullOrEmpty(Request["isLook"]))
            {
                isLook = Request["isLook"] == "true";
            }
            DistributRuleFullDTO distributRuleFullDto = new DistributRuleFullDTO();
            if (!string.IsNullOrEmpty(appId))
            {
                distributRuleFullDto = GetDistributRuleFullDTOByAppId_Mobile(appId, isLook);
            }
            return View(distributRuleFullDto);
        }

        /// <summary>
        /// 获取用户申请资料信息
        /// </summary>
        /// <param name="appId">appid</param>
        /// <param name="isLook">是否是查看模式</param>
        /// <returns></returns>
        public DistributRuleFullDTO GetDistributRuleFullDTOByAppId_Mobile(string appId, bool isLook)
        {
            DistributionSearchDTO distributionSearchDto = new DistributionSearchDTO
            {
                AppId = new Guid(appId),
                UserId = this.ContextDTO.LoginUserID,
                IsLook = isLook
            };
            DistributeFacade distributeFacade = new DistributeFacade();
            DistributRuleFullDTO distributRuleFullDto = distributeFacade.GetDistributRuleFullDTOByAppId_Mobile(distributionSearchDto);
            if (distributRuleFullDto == null)
            {
                distributRuleFullDto = new DistributRuleFullDTO
                {
                    DbIdentitySets = new List<DistributionIdentitySetFullDTO>(),
                    Title = ""
                };
            }
            return distributRuleFullDto;
        }

        /// <summary>
        /// 添加申请资料信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="ruleId">申请设置Id</param>
        /// <param name="strJson">申请设置Id</param>
        /// <param name="isModified">是否是编辑</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddDistributionIdentityInfo(string userId, string ruleId, string strJson, string isModified)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new Deploy.CustomDTO.ResultDTO();
            string userCode = this.ContextDTO.LoginUserCode;
            DistributApplyFullDTO distributApplyFullDto = new DistributApplyFullDTO { IsModified = isModified == "true" };
            try
            {
                if (!string.IsNullOrEmpty(strJson) && strJson != "[]")
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serial = new System.Web.Script.Serialization.JavaScriptSerializer();
                    distributApplyFullDto.DistributionIdentityFullDtos = serial.Deserialize<List<DistributionIdentityFullDTO>>(strJson);
                }

                DistributeFacade distributeFacade = new DistributeFacade();
                result = distributeFacade.AddDistributionIdentityInfo(userCode, userId, ruleId, distributApplyFullDto);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ModifyDistributRuleFull异常，异常信息：", ex);
                result.ResultCode = -1;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 微小店--首页
        /// </summary> 
        /// <returns></returns>
        [CheckParamType(IsCheckGuid = true)]
        [DealMobileUrl]
        [ArgumentExceptionDeal(Title = "微小店--首页")]
        public ActionResult MicroshopIndex(Guid microshopId)
        {

            // 查询 微小店的产品
            var all = SearchMicroshopCommoditys(microshopId, false);
            // 查询 微小店的最新产品
            var newest = SearchMicroshopCommoditys(microshopId, true);

            var allCount = all.Item2.CommodityList.Count();
            var newestCount = newest.Item2.CommodityList.Count();
            var microShopName = all.Item1.Name;

            string mainPic;//优先使用微小店里的Logo，如果没有则取分销商里的PicturePath
            if (!string.IsNullOrWhiteSpace(all.Item1.Logo))
                mainPic = all.Item1.Logo;
            else if (all.Item3 != null && !string.IsNullOrWhiteSpace(all.Item3.PicturePath))
                mainPic = all.Item3.PicturePath;
            else
                mainPic = "/Content/images/touxiang.png";

            ViewBag.Title = all.Item1.Name;
            ViewBag.Commoditys = JsonConvert.SerializeObject(all.Item2);
            ViewBag.MainPic = mainPic;
            ViewBag.SummaryInfo = "{\"allCount\":" + allCount + ",\"newestCount\":" + newestCount +
                                  ",\"microShopName\":\"" + microShopName + "\",\"microshopId\":\"" + microshopId +
                                  "\"}";
            return View();
        }

        /// <summary>
        /// 查询 微小店的最新产品
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public JsonResult GetNewestMicroshopCommoditys(Guid microshopId)
        {
            var ret = SearchMicroshopCommoditys(microshopId, true);

            return Json(ret.Item2);
        }

        /// <summary>
        /// 查询 微小店的产品
        /// </summary>
        /// <param name="microshopId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult SearchAllMicroshopCommoditys(Guid microshopId, string name)
        {
            var ret = SearchMicroshopCommoditys(microshopId, false);
            if (!string.IsNullOrEmpty(name) && ret.Item2.CommodityList != null)
            {
                var queriedCommoditys = ret.Item2.CommodityList.Where(x => x.Name.Contains(name)).ToList();
                ret.Item2.CommodityList = queriedCommoditys;
            }

            return Json(ret.Item2);
        }

        /// <summary>
        /// 查询微小店的所有商品（上线）
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        private CateringCommodityDTO GetMicroshopAllCommoditys(Guid microshopId)
        {
            var commodityFacade = new ISV.Facade.CommodityFacade();
            var distributeFacade = new IBP.Facade.DistributeFacade();

            MicroshopDTO microshop = distributeFacade.GetMicroshop(microshopId);
            List<Deploy.CommodityDistributionDTO> commodityDistributions = distributeFacade.GetAppAllMicroshopCommoditys(microshop.AppId);
            List<Guid> microshopComs = distributeFacade.GetMicroshopOfflineCommodityIds(microshopId);

            //所有商品。
            var commoditys =
                commodityFacade.GetCateringCommodity(new CommodityListSearchDTO()
                {
                    AppId = microshop.AppId,
                    UserId = microshop.UserId,
                    PageIndex = 1,
                    PageSize = int.MaxValue
                });
            var commoditysMs =
                commoditys.Data.CommodityList.Where(
                    x => commodityDistributions.Any(y => y.Id == x.Id) && !microshopComs.Contains(x.Id)).ToList();
            commoditys.Data.CommodityList = commoditysMs;

            return (commoditys.ResultCode == 0 && commoditys.Data != null) ? commoditys.Data : null;
        }
        private Tuple<MicroshopDTO, CateringCommodityDTO, DistributorDTO> SearchMicroshopCommoditys(Guid microshopId, bool getNewest)
        {
            var commodityFacade = new ISV.Facade.CommodityFacade();
            var distributeFacade = new IBP.Facade.DistributeFacade();

            MicroshopDTO microshop = distributeFacade.GetMicroshop(microshopId);
            //不存在的微小店，返回默认值（空值）
            if (microshop == null)
                return new Tuple<MicroshopDTO, CateringCommodityDTO, DistributorDTO>(new MicroshopDTO(), new CateringCommodityDTO(), new DistributorDTO());

            List<Deploy.CommodityDistributionDTO> cdList = distributeFacade.GetAppAllMicroshopCommoditys(microshop.AppId);
            List<Guid> mcList = distributeFacade.GetMicroshopOfflineCommodityIds(microshopId);
            var distributor = distributeFacade.GetDistributorBy(microshop.AppId, microshop.UserId);

            //所有商品。
            var commoditys =
                commodityFacade.GetCateringCommodity(new CommodityListSearchDTO()
                {
                    AppId = microshop.AppId,
                    UserId = microshop.UserId,
                    PageIndex = 1,
                    PageSize = int.MaxValue
                });

            if (commoditys == null || commoditys.Data == null || commoditys.Data.CommodityList == null)
            {
                return new Tuple<MicroshopDTO, CateringCommodityDTO, DistributorDTO>(microshop,
                    new CateringCommodityDTO
                    {
                        AppName = "",
                        CategoryList = new List<CategorySDTO>(),
                        CommodityList = new List<CommodityListIICDTO>()
                    }, distributor);
            }

            //排除重复的（只保留一个分类下的产品）
            var commodityDict = new List<Guid>();
            var commodityRepeated = new List<CommodityListIICDTO>();
            foreach (var c in commoditys.Data.CommodityList)
            {
                var cid = c.Id;
                if (commodityDict.Exists(x => x == cid))
                    commodityRepeated.Add(c);
                else
                    commodityDict.Add(cid);
            }
            commodityRepeated.ForEach(x => commoditys.Data.CommodityList.Remove(x));

            //上线了的产品
            var commoditysOnline =
                commoditys.Data.CommodityList.Where(
                    x => cdList.Any(y => y.Id == x.Id) && !mcList.Contains(x.Id)).ToList();

            if (getNewest)
            {
                var newest7 = DateTime.Now.AddDays(-7);

                //入店且未下线的产品，即：存在于CommodityDistribution，且不存在于MicroshopCom
                var commoditysFiltered = cdList.Where(x => !mcList.Contains(x.Id)).ToList();

                //7天内 入店且未下线的产品
                var cd7Filtered = commoditysFiltered.Where(x => x.ModifiedOn > newest7);

                //commoditysFiltered 和 cd7Filtered  的交集
                var commoditys7 = commoditysOnline.Where(x => cd7Filtered.Any(y => y.Id == x.Id))
                        .ToList();
                if (commoditys7.Any())
                {
                    //返回7天内 入店且未下线的产品
                    commoditys.Data.CommodityList = commoditys7;
                }
                else
                {
                    //计算 30%的上线产品
                    var count = commoditysOnline.Count();

                    var countPer30 = (int)(Math.Ceiling(count * 0.3));

                    //30% 入店且未下线的产品
                    var cdPer30Filtered = cdList.OrderByDescending(x => x.ModifiedOn).Take(countPer30).ToList();

                    var commoditysPercent30 = commoditysOnline.Where(x => cdPer30Filtered.Any(y => y.Id == x.Id))
                        .ToList();

                    //返回30%的上线产品
                    commoditys.Data.CommodityList = commoditysPercent30;
                }
            }
            else
            {
                commoditys.Data.CommodityList = commoditysOnline;
            }

            //delete无用字段
            commoditys.Data.CategoryList = null;

            return new Tuple<MicroshopDTO, CateringCommodityDTO, DistributorDTO>(microshop, commoditys.Data, distributor);
        }
    }
}