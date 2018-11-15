using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.MVC.Cache;
using System.Text;
using Jinher.AMP.CBC.Deploy;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Microsoft.Office.Interop.Excel;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 推广相关页面
    /// </summary>
    public class SpreadController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 推广注册页面
        /// </summary>
        /// <param name="speader">推广码</param>
        /// <returns></returns>
        public ActionResult Index(Guid speader)
        {
            ViewBag.speader = speader;

            string appIdStr = this.Request.QueryString["appId"];
            Guid appId = Guid.Empty;
            Guid.TryParse(appIdStr, out appId);
            //没有appId,默认显示正品会的应用信息。
            appId = appId == Guid.Empty ? CustomConfig.ZPHAppId : appId;

            Jinher.AMP.App.Deploy.ApplicationDTO appInfo = APPSV.Instance.GetAppByIdInfo(appId, this.ContextDTO);
            ViewBag.AppIco = appInfo.Icon;
            ViewBag.AppName = appInfo.Name;
            ViewBag.AppId = appId;

            //appid  可以为空，为空代表正品会
            return View();
        }

        /// <summary>
        /// 应用详情页
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult AppDetail(Guid appId)
        {
            bool isWxBroswer = Jinher.AMP.BTP.UI.Util.WebUtil.SideInWeixinBroswer(this.Request.UserAgent);
            ViewBag.isWxBroswer = isWxBroswer;

            Jinher.AMP.BTP.Deploy.CustomDTO.AppDetailAndPackageDTO appDetail = APPSV.Instance.GetAppDetailAndPackage(appId, this.ContextDTO);

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
        public ActionResult RegisterAndLoginAndBind(string LoginId, string Password, string Code, Guid spreadCode)
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
                        Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO = new UserSpreaderBindDTO()
                        {
                            UserId = LoginReturnInfo.ContextDTO.LoginUserID,
                            SpreadCode = spreadCode
                        };
                        bind(userSpreaderBindDTO);
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
        }

        /// <summary>
        /// 登录并绑定
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="Password"></param>
        /// <param name="spreadCode">推广码</param>
        /// <returns></returns>
        public ActionResult LoginAndBind(string LoginId, string Password, Guid spreadCode)
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
                Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO = new UserSpreaderBindDTO()
                {
                    UserId = logReturnInfo.ContextDTO.LoginUserID,
                    SpreadCode = spreadCode
                };
                bind(userSpreaderBindDTO);
            }
            else
            {
                rt.IsSuccess = false;
                rt.Message = logReturnInfo.Message;
                rt.StatusCode = logReturnInfo.StatusCode;
            }
            return Json(new { ret = rt, SubId = subId, OrgId = orgId, SessionID = sessId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SpreadCategory()
        {
            return View();
        }
        public JsonResult GetSpreadCategoryList()
        {
            return null;
        }

        #region 私有方法

        private bool bind(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.UserSpreaderFacade facade = new ISV.Facade.UserSpreaderFacade();
                var result = facade.SaveUserSpreaderCode(userSpreaderBindDTO);
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

        #endregion

        #region

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
        /// 推广主管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Manager()
        {
            SpreadCategoryFacade scFacade = new SpreadCategoryFacade();
            var spreadTypes = scFacade.GetSpreadCategoryList(new SpreadSearchDTO()).Data;
            // 查询二级代理类型
            var lv2SpreadIndex = spreadTypes.FindIndex(st => st.SpreadType == 6);
            if (lv2SpreadIndex > 0)
            {
                spreadTypes.RemoveAt(lv2SpreadIndex);
            }
            ViewBag.SpreadTypes = spreadTypes;
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            ViewBag.QRTypes = facade.GetQrCodeTypeList(new Deploy.CustomDTO.WeChatQRCodeSearchDTO()).Data;
            ViewBag.Apps = TPS.ZPHSV.GetEsNetApps();
            ViewBag.ZphApps = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new QueryPavilionAppParam()
            {
                Id = CustomConfig.ZPHAppId,
                pageIndex = 1,
                pageSize = int.MaxValue
            }).Data;
            return View();
        }

        /// <summary>
        /// 获取推广主
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Manager(SpreadSearchDTO dto, int page)
        {
            dto.PageIndex = page;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            var result = facade.GetSpreadInfoList(dto);
            if (result.isSuccess)
            {
                return Json(new
                {
                    data = result.Data.List,
                    records = result.Data.Count,
                    page = dto.PageIndex
                });
            }
            return Json(result);
        }

        /// <summary>
        ///  根据应用ID获取热门店铺
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult GetHotShop(Guid appId)
        {
            var result = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new QueryPavilionAppParam()
            {
                Id = appId,
                pageIndex = 1,
                pageSize = int.MaxValue
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取微信二维码序号
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="qrType"></param>
        /// <returns></returns>
        public ActionResult GetQrCodeNo(Guid appId, int qrType)
        {
            WeChatQRCodeFacade facade = new WeChatQRCodeFacade();
            var qrCodes = facade.GetWechatQrCodeList(new WeChatQRCodeSearchDTO
            {
                AppId = appId,
                QRType = qrType,
                IsUse = false,
                PageIndex = 1,
                PageSize = int.MaxValue
            });
            if (!qrCodes.isSuccess)
            {
                return Json(qrCodes, JsonRequestBehavior.AllowGet);
            }
            var qrCodeNos = qrCodes.Data.List.Select(d => d.QRNo).ToList();
            return Json(new ResultDTO<List<int>>() { isSuccess = true, Data = qrCodeNos }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult ExportExcel(SpreadSearchDTO dto)
        {
            dto.PageIndex = 1;
            dto.PageSize = int.MaxValue;
            SpreadInfoFacade facade = new SpreadInfoFacade();
            var spreadInfoes = facade.GetSpreadInfoList(dto);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("推广主姓名", typeof(string));
            dt.Columns.Add("账号", typeof(string));
            dt.Columns.Add("推广主类型", typeof(string));
            dt.Columns.Add("推广组织IW号", typeof(string));
            dt.Columns.Add("子代理数量", typeof(int));
            dt.Columns.Add("推广App名称", typeof(string));
            dt.Columns.Add("推广AppID", typeof(string));
            dt.Columns.Add("旺铺商家名称", typeof(string));
            dt.Columns.Add("旺铺AppID", typeof(string));
            dt.Columns.Add("推广码链接", typeof(string));
            dt.Columns.Add("是否绑定", typeof(string));
            dt.Columns.Add("是否启用", typeof(string));
            dt.Columns.Add("备注", typeof(string));
            foreach (var model in spreadInfoes.Data.List)
            {
                dt.Rows.Add(model.Name, model.Account, model.SpreadTypeDesc, model.IWCode, model.SubSpreadCount, model.SpreadAppName, model.SpreadAppId, model.HotshopName,model.HotshopId, model.QrCodeUrl, model.IsBindWeChatQrCode ? "是" : "否", model.IsDel == 0 ? "是" : "否", model.SpreadDesc);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("推广码_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 创建推广码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult Create(SpreadSaveDTO dto)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.SaveSpreadInfo(dto));
        }

        /// <summary>
        /// 绑定微信二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult BindQrCode(SpreadBindWeChatQrCodeDTO dto)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.BindWeChatQrCode(dto));
        }

        /// <summary>
        /// 启用、停用推广码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult UpdateState(SpreadUpdateStateDTO dto)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.UpdateState(dto));
        }


        /// <summary>
        /// 修改子代理数量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult UpdateSubSpreadCount(SpreadUpdateSubSpreadCountDTO dto)
        {
            SpreadInfoFacade facade = new SpreadInfoFacade();
            return Json(facade.UpdateSubCount(dto));
        }
    }
}
