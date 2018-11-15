using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Common.TypeDefine;
using System.IO;
using System.Web.Caching;
using System.Web;

namespace Jinher.AMP.BTP.Common
{
    public static class CustomConfig
    {
        public static string ReadShopAppContainer = "ReadShopAppContainer";
        /// <summary>
        /// 文件服务器
        /// </summary>
        private static string _fileServerUrl;
        public static string FileServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_fileServerUrl))
                {
                    Initialize();
                }
                return _fileServerUrl;
            }
        }
        private static string _commonFileServerUrl;
        public static string CommonFileServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_commonFileServerUrl))
                {
                    Initialize();
                }
                return _commonFileServerUrl;
            }
        }
        private static string _appUrl;
        public static string AppUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_appUrl))
                {
                    Initialize();
                }
                return _appUrl;
            }
        }
        /// <summary>
        /// 支付调用方凭证
        /// </summary>
        private static string _paySign;
        /// <summary>
        /// 支付调用方凭证
        /// </summary>
        public static string PaySing
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_paySign))
                {
                    Initialize();
                }
                return _paySign;
            }
        }

        /// <summary>
        /// 金币回调验证
        /// </summary>
        private static string _goldPayNotifySign;
        /// <summary>
        /// 金币支付回调凭证
        /// </summary>
        public static string GoldPayNotifySign
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_goldPayNotifySign))
                {
                    Initialize();
                }
                return _goldPayNotifySign;
            }
        }

        /// <summary>
        /// CBC域名(cbc.iuoooo.com)
        /// </summary>
        public static string CBCHost
        {
            get
            {
                Dictionary<string, string> dir = GetConfirDir();
                if (dir.ContainsKey("CBCHost"))
                    return dir["CBCHost"];
                else
                    return "cbc.iuoooo.com";
            }
        }

        private static Dictionary<string, string> GetConfirDir()
        {
            Dictionary<string, string> confirDir = null;
            const string cacheKey = "BTP_Configuration_Cache";
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configuration\CustomConfig\Jinher.AMP.BTP\Configuration.config");
            CacheDependency cacheDependency = new CacheDependency(configPath);
            confirDir = HttpRuntime.Cache.Get(cacheKey) as Dictionary<string, string>;
            if (confirDir == null)
            {
                confirDir = new Dictionary<string, string>();
                ExeConfigurationFileMap confile = new ExeConfigurationFileMap { ExeConfigFilename = configPath };
                try
                {
                    AppSettingsSection appSettings = ConfigurationManager.OpenMappedExeConfiguration(confile, ConfigurationUserLevel.None).AppSettings;
                    foreach (var key in appSettings.Settings.AllKeys)
                    {
                        confirDir[key] = appSettings.Settings[key].Value;
                    }
                    HttpRuntime.Cache.Add(cacheKey, confirDir, cacheDependency, Cache.NoAbsoluteExpiration,
                        Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(@"读取配置文件Configuration\CustomConfig\Jinher.AMP.BTP\Configuration.config异常", ex);
                }
            }
            return confirDir;
        }


        /// <summary>
        /// Url前缀
        /// </summary>
        private static string urlPrefix;
        public static string UrlPrefix
        {
            get
            {
                if (string.IsNullOrWhiteSpace(urlPrefix))
                {
                    Initialize();
                }
                return urlPrefix;
            }
        }

        private static string _btpAppres;
        public static string BTPAppres
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_btpAppres))
                {
                    Initialize();
                }
                return _btpAppres;
            }
        }

        private static string _btpBac;
        public static string BTPBac
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_btpBac))
                {
                    Initialize();
                }
                return _btpBac;
            }
        }

        private static string goodsMaxSelectedCount;
        public static string GoodsMaxSelectedCount
        {
            get
            {
                if (string.IsNullOrWhiteSpace(goodsMaxSelectedCount))
                {
                    Initialize();
                }
                return goodsMaxSelectedCount;
            }
        }

        private static SaleShareModel _saleShare;
        public static SaleShareModel SaleShare
        {

            get
            {
                if (_saleShare == null)
                {
                    Initialize();
                }
                return _saleShare;
            }



        }
        private static ShareGoldAccoutModel _shareGoldAccout;
        public static ShareGoldAccoutModel ShareGoldAccout
        {
            get
            {
                if (_shareGoldAccout == null)
                {
                    Initialize();
                }
                return _shareGoldAccout;
            }


        }


        private static CrowdfundingAccountModel _crowdfundingAccount;
        public static CrowdfundingAccountModel CrowdfundingAccount
        {
            get
            {
                if (_crowdfundingAccount == null)
                {
                    Initialize();
                }

                return _crowdfundingAccount;
            }
        }
        private static CrowdfundingConfig _crowdfundingConfig;

        public static CrowdfundingConfig CrowdfundingConfig
        {
            get
            {
                if (_crowdfundingConfig == null)
                {
                    Initialize();
                }

                return _crowdfundingConfig;
            }
        }

        private static string searchServiceUrl;
        public static string SearchServiceUrl
        {
            get
            {
                if (searchServiceUrl == null)
                {
                    Initialize();
                }

                return searchServiceUrl;
            }
        }

        private static string _aommodityUrl;
        public static string AommodityUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_aommodityUrl))
                {
                    Initialize();
                }
                return _aommodityUrl;
            }
        }

        private static string _appDownloadUrl;
        public static string AppDownloadUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appDownloadUrl))
                {
                    Initialize();
                }
                return _appDownloadUrl;
            }
        }

        private static string _partnerPrivKey;
        public static string PartnerPrivKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_partnerPrivKey))
                {
                    Initialize();
                }
                return _partnerPrivKey;
            }
        }

        private static string _environment;
        public static string Environment
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_environment))
                {
                    Initialize();
                }
                return _environment;
            }
        }
        private static string _commonUserName;
        public static string CommonUserName
        {
            get
            {
                if (_commonUserName == null)
                {
                    Initialize();
                }

                return _commonUserName;
            }
        }
        private static string _commonUserPass;
        public static string CommonUserPass
        {
            get
            {
                if (_commonUserPass == null)
                {
                    Initialize();
                }

                return _commonUserPass;
            }
        }
        private static string _zphUrl;
        public static string ZPHUrl
        {
            get
            {
                if (_zphUrl == null)
                {
                    Initialize();
                }

                return _zphUrl;
            }
        }

        private static string _btpDomain;
        public static string BtpDomain
        {
            get
            {
                if (_btpDomain == null)
                {
                    Initialize();
                }

                return _btpDomain;
            }
        }

        private static string _myBespeakUrl;
        public static string MyBespeakUrl
        {
            get
            {
                if (_myBespeakUrl == null)
                {
                    Initialize();
                }

                return _myBespeakUrl;
            }
        }
        private static string _portalUrl;
        public static string PortalUrl
        {
            get
            {
                if (_portalUrl == null)
                {
                    Initialize();
                }

                return _portalUrl;
            }
        }
        private static string _fspUrl;
        public static string FSPUrl
        {
            get
            {
                if (_fspUrl == null)
                {
                    Initialize();
                }

                return _fspUrl;
            }
        }

        private static string _promotionUrl;
        public static string PromotionUrl
        {
            get
            {
                if (_promotionUrl == null)
                {
                    Initialize();
                }

                return _promotionUrl;
            }
        }

        private static SpreaderAccountModel _spreaderAccount;
        public static SpreaderAccountModel SpreaderAccount
        {
            get
            {
                if (_spreaderAccount == null)
                {
                    Initialize();
                }
                return _spreaderAccount;
            }
        }

        private static ShareOwnerModel _shareOwner;
        public static ShareOwnerModel ShareOwner
        {
            get
            {
                if (_shareOwner == null)
                {
                    Initialize();
                }
                return _shareOwner;
            }
        }

        private static Guid _zphAppId;
        /// <summary>
        /// 正品会应用的appId（消息通知使用）
        /// </summary>
        public static Guid ZPHAppId
        {
            get
            {
                if (_zphAppId == Guid.Empty)
                {
                    Initialize();
                }
                return _zphAppId;
            }
        }


        private static string _weixinAppId;

        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public static string WeixinAppId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_weixinAppId))
                {
                    Initialize();
                }
                return _weixinAppId;
            }
        }
        private static string _weixinAppIdSecret;

        /// <summary>
        /// 公众号的appsecret 唯一凭证密钥
        /// </summary>
        public static string WeixinAppIdSecret
        {

            get
            {
                if (string.IsNullOrWhiteSpace(_weixinAppIdSecret))
                {
                    Initialize();
                }
                return _weixinAppIdSecret;
            }
        }


        private static string _wxApiKey;

        /// <summary>
        /// API密钥
        /// </summary>
        public static string WxApiKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_wxApiKey))
                {
                    Initialize();
                }
                return CustomConfig._wxApiKey;
            }
        }

        private static string _wxSignType;
        /// <summary>
        /// 签名类型
        /// </summary>
        public static string WxSignType
        {
            get
            {
                _wxSignType = "MD5";
                return _wxSignType;
            }
        }

        private static string _wxDomain;
        /// <summary>
        /// 微信签名域名（因为btp绑定多个域名，但是微信公众号只允许一个域名）
        /// </summary>
        public static string WxDomain
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_wxDomain))
                {
                    Initialize();
                }
                return _wxDomain;
            }
        }

        private static string _wxSignFlag;
        /// <summary>
        /// 是否启用微信自动签名(Y启用，其他不启用)
        /// </summary>
        public static string WxSignFlag
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_wxSignFlag))
                {
                    Initialize();
                }
                return _wxSignFlag;
            }
        }

        private static JinherBtpAccountModel _jinherBtpAccount;
        /// <summary>
        /// 电商分润帐号
        /// </summary>
        public static JinherBtpAccountModel JinherBtpAccount
        {
            get
            {
                if (_jinherBtpAccount == null)
                {
                    Initialize();
                }
                return _jinherBtpAccount;
            }
        }


        private static GeneralAgencyAccountModel _generalAgencyAccount;
        /// <summary>
        /// 总代获得金额信息
        /// </summary>
        public static GeneralAgencyAccountModel GeneralAgencyAccount
        {
            get
            {
                if (_generalAgencyAccount == null)
                {
                    Initialize();
                }
                return _generalAgencyAccount;
            }
        }


        private static JhAccountModel _jhAccount;
        /// <summary>
        /// 金和获得金额信息
        /// </summary>
        public static JhAccountModel JhAccount
        {
            get
            {
                if (_jhAccount == null)
                {
                    Initialize();
                }
                return _jhAccount;
            }
        }
        private static DateTime _minClearningOrderDate;
        /// <summary>
        /// 是否启用微信自动签名(Y启用，其他不启用)
        /// </summary>
        public static DateTime MinClearningOrderDate
        {
            get
            {
                if (_minClearningOrderDate <= Constant.DbMinValue)
                {
                    Initialize();
                }
                return _minClearningOrderDate;
            }
        }


        private static string _provinceCityUrl;
        /// <summary>
        /// 省市列表选择页
        /// </summary>
        public static string ProvinceCityUrl
        {
            get
            {
                if (_provinceCityUrl == null)
                {
                    Initialize();
                }

                return _provinceCityUrl;
            }
        }

        private static string _videoHost;
        /// <summary>
        /// 省市列表选择页
        /// </summary>
        public static string VideoHost
        {
            get
            {
                if (_videoHost == null)
                {
                    Initialize();
                }

                return _videoHost;
            }
        }

        private static string _bacEUrl;

        public static string BacEUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_bacEUrl))
                {
                    Initialize();
                }
                return _bacEUrl;
            }
        }
        private static Guid _selfTakeAppId = Guid.Empty;

        public static Guid SelfTakeAppId
        {
            get
            {
                if (_selfTakeAppId == Guid.Empty)
                {
                    Initialize();
                }
                return _selfTakeAppId;
            }
        }


        private static string _btpCacheManagerUsers = "";

        /// <summary>
        /// 可管理电商缓存的人员账号列表。
        /// </summary>
        public static string BtpCacheManagerUsers
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_btpCacheManagerUsers))
                {
                    Initialize();
                }
                return _btpCacheManagerUsers;
            }
        }
        private static DateTime _minScoreOrderDate;
        /// <summary>
        /// 启用订单积分时间
        /// </summary>
        public static DateTime MinScoreOrderDate
        {
            get
            {
                if (_minScoreOrderDate <= Constant.DbMinValue)
                {
                    Initialize();
                }
                return _minScoreOrderDate;
            }
        }
        private static int? _couponFlag;
        /// <summary>
        /// 优惠券与代金券是否可用 0：都可用；1 都不可用
        /// </summary>
        public static int CouponFlag
        {
            get
            {
                if (_couponFlag == null)
                {
                    Initialize();
                }

                return _couponFlag.Value;
            }
        }

        private static string _DSSUrl;

        /// <summary>
        /// dss地址
        /// </summary>
        public static string DSSUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DSSUrl))
                {
                    Initialize();
                }

                return _DSSUrl;
            }
        }

        private static string _kdniaoEBusinessID;

        /// <summary>
        /// 快递鸟分配给商家的电商ID
        /// </summary>
        public static string KdniaoEBusinessID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_kdniaoEBusinessID))
                {
                    Initialize();
                }

                return _kdniaoEBusinessID;
            }
        }

        private static string _kdniaoAppKey;

        /// <summary>
        /// 快递鸟分配给商家的电商加密私钥
        /// </summary>
        public static string KdniaoAppKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_kdniaoAppKey))
                {
                    Initialize();
                }

                return _kdniaoAppKey;
            }
        }

        private static string _kdniaoSubscribeReqURL;

        /// <summary>
        /// 快递鸟订阅物流信息接口地址
        /// </summary>
        public static string KdniaoSubscribeReqURL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_kdniaoSubscribeReqURL))
                {
                    Initialize();
                }

                return _kdniaoSubscribeReqURL;
            }
        }

        private static bool? _crowdfundingFlag;
        /// <summary>
        /// 是否启用众筹
        /// </summary>
        public static bool CrowdfundingFlag
        {
            get
            {
                if (_crowdfundingFlag == null)
                {
                    Initialize();
                }

                return _crowdfundingFlag != null && _crowdfundingFlag.Value;
            }
        }

        private static int _kdniaoSubscribeErrorCount = 0;

        /// <summary>
        /// 快递鸟订阅出错多少次后停止订阅.
        /// </summary>
        public static int KdniaoSubscribeErrorCount
        {
            get
            {
                if (_kdniaoSubscribeErrorCount == null)
                {
                    Initialize();
                }
                return _kdniaoSubscribeErrorCount;
            }
        }

        private static string _expressPlat = "";

        /// <summary>
        /// 快递平台（Kdniao、Kd100）
        /// </summary>
        public static string ExpressPlat
        {
            get
            {
                if (_expressPlat == null)
                {
                    Initialize();
                }
                return _expressPlat;
            }
        }

        /// <summary>
        /// 商品分类功能在bac里的编号。
        /// </summary>
        private static string _categoryManageFunctionCode = "";

        public static string CategoryManageFunctionCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_categoryManageFunctionCode))
                {
                    Initialize();
                }
                return _categoryManageFunctionCode;
            }
        }

        private static List<int> _scoreCostList = new List<int>();
        /// <summary>
        /// 积分抵现可设置列表
        /// </summary>
        public static List<int> ScoreCostList
        {
            get
            {
                if (!_scoreCostList.Any())
                {
                    Initialize();
                }
                return _scoreCostList;
            }
        }

        /// <summary>
        /// 显示上传图片的商品列表
        /// </summary>
        private static string _uploadFileCommodityList;
        public static string UploadFileCommodityList
        {
            get
            {
                if (string.IsNullOrEmpty(_uploadFileCommodityList))
                {
                    Initialize();
                }
                return _uploadFileCommodityList;
            }
        }

        /// <summary>
        /// 显示协议1的商品列表
        /// </summary>
        private static string _contract1CommodityList;
        public static string Contract1CommodityList
        {
            get
            {
                if (string.IsNullOrEmpty(_contract1CommodityList))
                {
                    Initialize();
                }
                return _contract1CommodityList;
            }
        }

        /// <summary>
        /// 显示协议2的商品列表
        /// </summary>
        private static string _contract2CommodityList;
        public static string Contract2CommodityList
        {
            get
            {
                if (string.IsNullOrEmpty(_contract2CommodityList))
                {
                    Initialize();
                }
                return _contract2CommodityList;
            }
        }

        private static string _SNSUrl;

        /// <summary>
        /// sns服务地址
        /// </summary>
        public static string SNSUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SNSUrl))
                {
                    Initialize();
                }
                return _SNSUrl;
            }
        }
        private static string _CBCUrl;
        /// <summary>
        /// cbc服务地址
        /// </summary>
        public static string CBCUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_CBCUrl))
                {
                    Initialize();
                }
                return _CBCUrl;
            }
        }

        private static string _CSSUrl;
        /// <summary>
        /// css服务地址
        /// </summary>
        public static string CSSUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_CSSUrl))
                {
                    Initialize();
                }
                return _CSSUrl;
            }
        }


        private static bool? _IsShareAsScore;

        /// <summary>
        /// 众销是否送积分
        /// </summary>
        public static bool IsShareAsScore
        {
            get
            {
                if (_IsShareAsScore == null)
                {
                    Initialize();
                }

                return _IsShareAsScore ?? true;
            }
        }

        private static string _H5HomePage;

        /// <summary>
        /// sns服务地址
        /// </summary>
        public static string H5HomePage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_H5HomePage))
                {
                    Initialize();
                }
                return _H5HomePage;
            }
        }
        private static string _DiyGroupCommodityList;

        /// <summary>
        /// sns服务地址
        /// </summary>
        public static string DiyGroupCommodityList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DiyGroupCommodityList))
                {
                    Initialize();
                }
                return _DiyGroupCommodityList;
            }
        }







        private static string _amapGetLongLatitudeByAddress;


        /// <summary>
        /// 高德地图获取地址对应的经纬度接口地址
        /// </summary>
        public static string AmapGetLongLatitudeByAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_amapGetLongLatitudeByAddress))
                {
                    Initialize();
                }
                return CustomConfig._amapGetLongLatitudeByAddress;
            }
        }

        private static string _amapKey;


        /// <summary>
        /// 调用高德地图所需要的key
        /// </summary>
        public static string AmapKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_amapKey))
                {
                    Initialize();
                }
                return CustomConfig._amapKey;
            }
        }
        public static RentAgreementConfigSection RentAgreement
        {
            get
            {
                if (_rentAgreement == null)
                {
                    Initialize();
                }
                return _rentAgreement;
            }
        }
        private static RentAgreementConfigSection _rentAgreement;

        private static string _assumeULikeUrl;
        /// <summary>
        /// 猜你喜欢埋码地址
        /// </summary>
        public static string AssumeULikeUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_assumeULikeUrl))
                {
                    Initialize();
                }
                return _assumeULikeUrl;
            }
        }
        private static string _assumeULikeUrlHttps;
        /// <summary>
        /// 猜你喜欢埋码地址
        /// </summary>
        public static string AssumeULikeUrlHttps
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_assumeULikeUrlHttps))
                {
                    Initialize();
                }
                return _assumeULikeUrlHttps;
            }
        }
        private static string _pipLogin;
        /// <summary>
        /// pip登录地址
        /// </summary>
        public static string PipLogin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pipLogin))
                {
                    Initialize();
                }
                return _pipLogin;
            }
        }
        private static string _pipWxLogin;
        /// <summary>
        /// pip登录地址(微信)
        /// </summary>
        public static string PipWxLogin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pipWxLogin))
                {
                    Initialize();
                }
                return _pipWxLogin;
            }
        }
        private static int? _selfTakeMaxDelayDay;
        /// <summary>
        /// 自提点可以预约推迟最大天数
        /// </summary>
        public static int SelfTakeMaxDelayDay
        {
            get
            {
                if (!_selfTakeMaxDelayDay.HasValue)
                    Initialize();

                return _selfTakeMaxDelayDay ?? 0;
            }
        }
        private static string _webImUrl;
        /// <summary>
        /// h5IM地址
        /// </summary>
        public static string WebImUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_webImUrl))
                {
                    Initialize();
                }
                return _webImUrl;
            }
        }
        private static int? _selfTakeOrderExpirePaySecond;
        /// <summary>
        /// 自提订单未支付超时时间 单位：秒
        /// </summary>
        public static int SelfTakeOrderExpirePaySecond
        {
            get
            {
                if (!_selfTakeOrderExpirePaySecond.HasValue)
                {
                    Initialize();
                }
                return _selfTakeOrderExpirePaySecond ?? 0;
            }
        }

        /// <summary>
        /// 图片上传316*316
        /// </summary>
        private static string _defaultImgSize;
        /// <summary>
        /// 图片上传316*316
        /// </summary>
        public static string DefaultImgSize
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultImgSize))
                {
                    Initialize();
                }
                return _defaultImgSize;
            }
        }
        private static int? _requestTimeOut;
        /// <summary>
        /// 请求超时时间单位毫秒
        /// </summary>
        public static int RequestTimeOut
        {
            get
            {
                if (!_requestTimeOut.HasValue)
                {
                    Initialize();
                }
                return _requestTimeOut ?? 20000;
            }
        }


        /// <summary>
        /// 直接到账是否支持退款
        /// </summary>
        private static bool? _isSystemDirectRefund;
        /// <summary>
        /// 直接到账是否支持退款
        /// </summary>
        public static bool IsSystemDirectRefund
        {
            get
            {
                if (!_isSystemDirectRefund.HasValue)
                {
                    Initialize();
                }
                return _isSystemDirectRefund ?? false;
            }
        }
        /// <summary>
        /// 爱尔目直播地址
        /// </summary>
        private static string _equipmentUrl;
        /// <summary>
        /// 爱尔目直播地址
        /// </summary>
        public static string EquipmentUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_equipmentUrl))
                {
                    Initialize();
                }
                return _equipmentUrl;
            }
        }

        private static OrderPayBackSection _orderPayBackUrl;
        /// <summary>
        /// 订单支付回调
        /// </summary>
        public static OrderPayBackSection OrderPayBackUrl
        {
            get
            {
                if (_orderPayBackUrl == null)
                {
                    Initialize();
                }
                return _orderPayBackUrl;
            }
        }




        /// <summary>
        /// 订单打印控件下载路径配置
        /// </summary>
        private static string _DownPrintControlPath;
        /// <summary>
        /// 订单打印控件下载路径配置
        /// </summary>
        public static string DownPrintControlPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DownPrintControlPath))
                {
                    Initialize();
                }
                return _DownPrintControlPath;
            }
        }


        private static WeChatSpreaderSection _weChatSpreader;
        /// <summary>
        /// 正品会应用的appId（消息通知使用）
        /// </summary>
        public static WeChatSpreaderSection WeChatSpreader
        {
            get
            {
                if (_weChatSpreader == null)
                {
                    Initialize();
                }
                return _weChatSpreader;
            }
        }

        private static bool? _isChannelShare;
        /// <summary>
        /// 是否启用渠道推广
        /// </summary>
        public static bool IsChannelShare
        {
            get
            {
                if (!_isChannelShare.HasValue)
                {
                    Initialize();
                }
                return _isChannelShare ?? false;
            }
        }

        private static string _zshappkey;

        /// <summary>
        /// 极速数据接口
        /// </summary>
        public static string zshappkey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_zshappkey))
                {
                    Initialize();
                }

                return _zshappkey;
            }
        }

        private static string _saleTax;

        /// <summary>
        /// 电子发票 销方税号
        /// </summary>
        public static string saleTax
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_saleTax))
                {
                    Initialize();
                }

                return _saleTax;
            }
        }

        private static string _kpy;

        /// <summary>
        /// 电子发票 开票人
        /// </summary>
        public static string kpy
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_kpy))
                {
                    Initialize();
                }

                return _kpy;
            }
        }

        private static string _invoiceAppId;

        /// <summary>
        /// 电子发票 开票APPid
        /// </summary>
        public static string InvoiceAppId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_invoiceAppId))
                {
                    Initialize();
                }

                return _invoiceAppId;
            }
        }

        /// <summary>
        /// 盈科商户编码
        /// </summary>
        private static string _YingKeBuCode;
        /// <summary>
        /// 盈科商户编码
        /// </summary>
        public static string YingKeBuCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_YingKeBuCode))
                {
                    Initialize();
                }

                return _YingKeBuCode;
            }
        }

        /// <summary>
        /// 盈科商户密钥
        /// </summary>
        private static string _YingKeSecretKey;
        /// <summary>
        /// 盈科商户密钥
        /// </summary>
        public static string YingKeSecretKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_YingKeSecretKey))
                {
                    Initialize();
                }

                return _YingKeSecretKey;
            }
        }

        /// <summary>
        /// 盈科电子券获取接口地址
        /// </summary>
        private static string _YingKeGetCouponUrl;
        /// <summary>
        /// 盈科电子券获取接口地址
        /// </summary>
        public static string YingKeGetCouponUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_YingKeGetCouponUrl))
                {
                    Initialize();
                }

                return _YingKeGetCouponUrl;
            }
        }

        /// <summary>
        /// btp项目外网ip
        /// </summary>
        private static string _BTPIP;
        /// <summary>
        /// btp项目外网ip
        /// </summary>
        public static string BTPIP
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_BTPIP))
                {
                    Initialize();
                }

                return _BTPIP;
            }
        }

        /// <summary>
        /// 大米网域名
        /// </summary>
        private static List<string> _DMWIPList;
        /// <summary>
        /// 大米网域名
        /// </summary>
        public static List<string> DMWIPList
        {
            get
            {
                if (_DMWIPList == null || _DMWIPList.Count == 0)
                {
                    Initialize();
                }

                return _DMWIPList;
            }
        }

        /// <summary>
        ///获取易捷北京所有的AppId
        /// </summary>
        private static string _AppIds;
        /// <summary>
        /// /获取易捷北京所有的AppId
        /// </summary>
        public static string AppIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AppIds))
                {
                    Initialize();
                }

                return _AppIds;
            }
        }

        /// <summary>
        /// 京东大客户AppId集合
        /// </summary>
        private static List<Guid> _JdAppIdList = new List<Guid>();
        /// <summary>
        /// 京东大客户AppId集合
        /// </summary>
        public static List<Guid> JdAppIdList
        {
            get
            {
                if (_JdAppIdList.Count == 0)
                {
                    Initialize();
                }

                return _JdAppIdList;
            }
        }

        /// <summary>
        ///获取易捷北京所有的AppId
        /// </summary>
        private static string _SappIds;
        /// <summary>
        /// /获取易捷北京所有的AppId
        /// </summary>
        public static string SappIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SappIds))
                {
                    Initialize();
                }

                return _SappIds;
            }
        }




        /// <summary>
        ///获取通道费用的AppId
        /// </summary>
        private static string _TdAppIds;
        /// <summary>
        /// 获取通道费用的AppId
        /// </summary>
        public static string TdAppIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TdAppIds))
                {
                    Initialize();
                }
                return _TdAppIds;
            }
        }

        /// <summary>
        ///阳光餐饮通道费用的AppId
        /// </summary>
        private static string _YangAppIds;
        /// <summary>
        /// 获取阳光餐饮通道费用的AppId
        /// </summary>
        public static string YangAppIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_YangAppIds))
                {
                    Initialize();
                }
                return _YangAppIds;
            }
        }



        private static Guid _YJAppId;
        /// <summary>
        /// 易捷北京AppId
        /// </summary>
        public static Guid YJAppId
        {
            get
            {
                if (_YJAppId == Guid.Empty)
                {
                    Initialize();
                }
                return _YJAppId;
            }
        }


        private static string _TJAppIds;
        /// <summary>
        /// 统计信息的AppIds
        /// </summary>
        public static string TJAppIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TJAppIds))
                {
                    Initialize();
                }

                return _TJAppIds;
            }
        }

        private static string _OAPIApiKey;
        /// <summary>
        /// api key
        /// </summary>
        public static string OAPIApiKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_OAPIApiKey))
                {
                    Initialize();
                }

                return _OAPIApiKey;
            }
        }

        private static string _OAPICallerId;
        /// <summary>
        /// callerId
        /// </summary>
        public static string OAPICallerId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_OAPICallerId))
                {
                    Initialize();
                }

                return _OAPICallerId;
            }
        }

        private static string _OAPICheckTokenUrl;
        /// <summary>
        /// 鉴权地址
        /// </summary>
        public static string OAPICheckTokenUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_OAPICheckTokenUrl))
                {
                    Initialize();
                }

                return _OAPICheckTokenUrl;
            }
        }

        /// <summary>
        /// 京东jos系统AppKey
        /// </summary>
        private static string _JdJosAppKey;
        /// <summary>
        /// 京东jos系统AppKey
        /// </summary>
        public static string JdJosAppKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_JdJosAppKey))
                {
                    Initialize();
                }

                return _JdJosAppKey;
            }
        }

        /// <summary>
        /// 京东jos系统AppSecret
        /// </summary>
        private static string _JdJosAppSecret;
        /// <summary>
        /// 京东jos系统AppSecret
        /// </summary>
        public static string JdJosAppSecret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_JdJosAppSecret))
                {
                    Initialize();
                }

                return _JdJosAppSecret;
            }
        }

        /// <summary>
        /// 京东jos系统Token
        /// </summary>
        private static string _JdJosAccessToken;
        /// <summary>
        /// 京东jos系统Token
        /// </summary>
        public static string JdJosAccessToken
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_JdJosAccessToken))
                {
                    Initialize();
                }

                return _JdJosAccessToken;
            }
        }

        /// <summary>
        /// 网易严选AppId集合
        /// </summary>
        private static List<Guid> _YxAppIdList = new List<Guid>();
        /// <summary>
        /// 网易严选AppId集合
        /// </summary>
        public static List<Guid> YxAppIdList
        {
            get
            {
                if (_YxAppIdList.Count == 0)
                {
                    Initialize();
                }

                return _YxAppIdList;
            }
        }
        /// <summary>
        /// 苏宁易购AppId集合
        /// </summary>
        private static List<Guid> _SnAppIdList = new List<Guid>();
        /// <summary>
        /// 苏宁易购AppId集合
        /// </summary>
        public static List<Guid> SnAppIdList
        {
            get
            {
                if (_SnAppIdList.Count == 0)
                {
                    Initialize();
                }

                return _SnAppIdList;
            }
        }

        /// <summary>
        /// 方正电商AppId集合
        /// </summary>
        private static List<Guid> _FZAppIdList = new List<Guid>();
        public static List<Guid> FZAppIdList
        {
            get
            {
                if (_FZAppIdList.Count == 0)
                {
                    Initialize();
                }

                return _FZAppIdList;
            }
        }

        #region 易派客
        private static List<Guid> _YPKAppIdList = new List<Guid>();
        public static List<Guid> YPKAppIdList
        {
            get
            {
                if (_YPKAppIdList.Count == 0)
                {
                    Initialize();
                }

                return _YPKAppIdList;
            }
        }
        private static string _YPK_UrlBase;
        public static string YPK_UrlBase
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_YPK_UrlBase))
                {
                    Initialize();
                }

                return _YPK_UrlBase;
            }
        }
        private static string _client_id;
        public static string client_id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_client_id))
                {
                    Initialize();
                }

                return _client_id;
            }
        }
        private static string _client_secret;
        public static string client_secret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_client_secret))
                {
                    Initialize();
                }

                return _client_secret;
            }
        }
        private static string _grant_type;
        public static string grant_type
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_grant_type))
                {
                    Initialize();
                }

                return _grant_type;
            }
        }
        private static string _companyid;
        public static string companyid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_companyid))
                {
                    Initialize();
                }

                return _companyid;
            }
        }
        private static string _username;
        public static string username
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_username))
                {
                    Initialize();
                }

                return _username;
            }
        }
        private static string _password;
        public static string password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_password))
                {
                    Initialize();
                }

                return _password;
            }
        }
        #endregion


        private static string _Suning_UrlBase;
        public static string Suning_UrlBase
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Suning_UrlBase))
                {
                    Initialize();
                }

                return _Suning_UrlBase;
            }
        }

        private static string _Suning_AppKey;
        public static string Suning_AppKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Suning_AppKey))
                {
                    Initialize();
                }

                return _Suning_AppKey;
            }
        }


        private static string _Suning_AppSecret;
        public static string Suning_AppSecret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Suning_AppSecret))
                {
                    Initialize();
                }

                return _Suning_AppSecret;
            }
        }


        private static string _FangZheng_UrlBase;
        public static string FangZheng_UrlBase
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_FangZheng_UrlBase))
                {
                    Initialize();
                }

                return _FangZheng_UrlBase;
            }
        }

        private static string _FangZheng_ShopId;
        public static string FangZheng_ShopId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_FangZheng_ShopId))
                {
                    Initialize();
                }

                return _FangZheng_ShopId;
            }
        }


        private static string _FangZheng_Password;
        public static string FangZheng_Password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_FangZheng_Password))
                {
                    Initialize();
                }

                return _FangZheng_Password;
            }
        }

        private static string _Suning_CityId;
        public static string Suning_CityId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Suning_CityId))
                {
                    Initialize();
                }
                return _Suning_CityId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static string _ylptFeatureCode;

        /// <summary>
        /// 运营平台在权限目录的编码
        /// </summary>
        public static string YlptFeatureCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ylptFeatureCode))
                {
                    Initialize();
                }
                return _ylptFeatureCode;
            }
        }

        private static string _MqServerHost;
        public static string MqServerHost
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MqServerHost))
                {
                    Initialize();
                }
                return _MqServerHost;
            }
        }

        private static string _MqUserName;
        public static string MqUserName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MqUserName))
                {
                    Initialize();
                }
                return _MqUserName;
            }
        }

        private static string _MqPassword;
        public static string MqPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MqPassword))
                {
                    Initialize();
                }
                return _MqPassword;
            }
        }
        private static string _InsuranceHost;
        /// <summary>
        /// 保险域名域名
        /// </summary>
        public static string InsuranceHost
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_InsuranceHost))
                {
                    Initialize();
                }
                return _InsuranceHost;
            }
        }

        private static string _Express100_Key;
        public static string Express100_Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Express100_Key))
                {
                    Initialize();
                }

                return _Express100_Key;
            }
        }
        private static string _Express100_UrlBase;
        public static string Express100_UrlBase
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Express100_UrlBase))
                {
                    Initialize();
                }

                return _Express100_UrlBase;
            }
        }
        private static string _Express100_Customer;
        public static string Express100_Customer
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Express100_Customer))
                {
                    Initialize();
                }

                return _Express100_Customer;
            }
        }


        private static void Initialize()
        {
            try
            {
                ExeConfigurationFileMap file = new ExeConfigurationFileMap();
                file.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\CustomConfig\Jinher.AMP.BTP\Configuration.config";
                AppSettingsSection appSettings = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).AppSettings;
                _fileServerUrl = appSettings.Settings["FileServerUrl"].Value;
                _commonFileServerUrl = appSettings.Settings["CommonFileServerUrl"].Value;
                _appUrl = appSettings.Settings["AppUrl"].Value;
                _paySign = appSettings.Settings["GoldPaySign"].Value;
                _goldPayNotifySign = appSettings.Settings["GoldPayNotifySign"].Value;
                urlPrefix = appSettings.Settings["UrlPrefix"].Value;
                _btpAppres = appSettings.Settings["BTPAppres"].Value;
                _btpBac = appSettings.Settings["BTPBac"].Value;
                goodsMaxSelectedCount = appSettings.Settings["GoodsMaxSelectedCount"].Value;

                _saleShare = ((SaleShareModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("SaleShare"));
                _shareGoldAccout = ((ShareGoldAccoutModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("ShareGoldAccout"));
                _crowdfundingConfig = ((CrowdfundingConfig)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("Crowdfunding"));
                _crowdfundingAccount = ((CrowdfundingAccountModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("CrowdfundingAcount"));
                _orderPayBackUrl = ((OrderPayBackSection)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("orderPayCallBack"));
                _weChatSpreader = ((WeChatSpreaderSection)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("weChatSpreader"));

                searchServiceUrl = appSettings.Settings["SearchServiceUrl"].Value;
                _appDownloadUrl = appSettings.Settings["AppDownloadUrl"].Value;
                _aommodityUrl = appSettings.Settings["CommodityUrl"].Value;
                _partnerPrivKey = appSettings.Settings["PartnerPrivKey"].Value;
                _environment = appSettings.Settings["Environment"].Value;
                _commonUserName = appSettings.Settings["CommonUserName"].Value;
                _commonUserPass = appSettings.Settings["CommonUserPass"].Value;
                _zphUrl = appSettings.Settings["ZPHUrl"].Value;
                _btpDomain = appSettings.Settings["BtpDomain"].Value;
                _myBespeakUrl = appSettings.Settings["MyBespeakUrl"].Value;
                _portalUrl = appSettings.Settings["PortalUrl"].Value;
                _fspUrl = appSettings.Settings["FSPUrl"].Value;
                _promotionUrl = appSettings.Settings["PromotionUrl"].Value;
                _bacEUrl = appSettings.Settings["BacEUrl"].Value;
                _provinceCityUrl = appSettings.Settings["ProvinceCityUrl"].Value;
                _videoHost = appSettings.Settings["VideoHost"].Value;
                _spreaderAccount = ((SpreaderAccountModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("SpreaderAccount"));
                _shareOwner = ((ShareOwnerModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("ShareOwner"));
                _jinherBtpAccount = ((JinherBtpAccountModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("JinherBtpAccount"));
                _generalAgencyAccount = ((GeneralAgencyAccountModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("GeneralAgencyAccount"));
                _jhAccount = ((JhAccountModel)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("JhAccount"));
                _rentAgreement = ((RentAgreementConfigSection)ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None).GetSection("RentAgreement"));



                var zphAppIdStr = appSettings.Settings["ZPHAppId"].Value;
                Guid.TryParse(zphAppIdStr, out _zphAppId);

                _weixinAppId = appSettings.Settings["WeixinAppId"].Value;
                _weixinAppIdSecret = appSettings.Settings["WeixinAppIdSecret"].Value;

                _wxSignFlag = appSettings.Settings["WxSignFlag"].Value;
                _wxDomain = appSettings.Settings["WxDomain"].Value;
                //_wxApiKey = appSettings.Settings["WxApiKey"].Value;
                var minClearningDate = appSettings.Settings["MinClearningDate"].Value;
                DateTime.TryParse(minClearningDate, out _minClearningOrderDate);

                var selfTakeAppId = appSettings.Settings["SelfTakeAppId"].Value;
                Guid.TryParse(selfTakeAppId, out _selfTakeAppId);

                _btpCacheManagerUsers = appSettings.Settings["BtpCacheManagerUsers"].Value;

                int couponFlag = 0;
                int.TryParse(appSettings.Settings["CouponFlag"].Value, out couponFlag);
                _couponFlag = couponFlag;

                _DSSUrl = appSettings.Settings["DSSUrl"].Value;

                _kdniaoEBusinessID = appSettings.Settings["KdniaoEBusinessID"].Value;
                _kdniaoAppKey = appSettings.Settings["KdniaoAppKey"].Value;
                _kdniaoSubscribeReqURL = appSettings.Settings["KdniaoSubscribeReqURL"].Value;

                string kdnserrcount = appSettings.Settings["KdniaoSubscribeErrorCount"].Value;
                int.TryParse(kdnserrcount, out _kdniaoSubscribeErrorCount);

                _expressPlat = appSettings.Settings["ExpressPlat"].Value;


                bool crowdfundingFlag;
                bool.TryParse(appSettings.Settings["CrowdfundingFlag"].Value, out crowdfundingFlag);
                _crowdfundingFlag = crowdfundingFlag;

                var minScoreOrderDate = appSettings.Settings["MinScoreOrderDate"].Value;
                DateTime.TryParse(minScoreOrderDate, out _minScoreOrderDate);

                _scoreCostList = new List<int>();
                string scoreCostStr = appSettings.Settings["ScoreCostList"].Value;
                if (!string.IsNullOrEmpty(scoreCostStr))
                {
                    var scoreCostArr = scoreCostStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var scoreCostTemp in scoreCostArr)
                    {
                        int scoreCost;
                        if (int.TryParse(scoreCostTemp, out scoreCost))
                        {
                            _scoreCostList.Add(scoreCost);
                        }
                    }
                }
                _uploadFileCommodityList = appSettings.Settings["UploadFileCommodityList"].Value;
                _contract1CommodityList = appSettings.Settings["Contract1CommodityList"].Value;
                _contract2CommodityList = appSettings.Settings["Contract2CommodityList"].Value;

                _SNSUrl = appSettings.Settings["SNSUrl"].Value;
                _CBCUrl = appSettings.Settings["CBCUrl"].Value;
                _CSSUrl = appSettings.Settings["CSSUrl"].Value;
                bool isShareAsScore;
                bool.TryParse(appSettings.Settings["IsShareAsScore"].Value, out isShareAsScore);
                _IsShareAsScore = isShareAsScore;

                _H5HomePage = appSettings.Settings["H5HomePage"].Value;



                _amapGetLongLatitudeByAddress = appSettings.Settings["AmapGetLongLatitudeByAddress"].Value;
                _amapKey = appSettings.Settings["AmapKey"].Value;
                _DiyGroupCommodityList = appSettings.Settings["DiyGroupCommodityList"].Value;

                _assumeULikeUrl = appSettings.Settings["AssumeULikeUrl"].Value;
                _assumeULikeUrlHttps = appSettings.Settings["AssumeULikeUrlHttps"].Value;
                _pipLogin = appSettings.Settings["PipLogin"].Value;
                _pipWxLogin = appSettings.Settings["PipWxLogin"].Value;
                _webImUrl = appSettings.Settings["WebIMUrl"].Value;

                _defaultImgSize = appSettings.Settings["DefaultImgSize"].Value;

                int selfTakeOrderExpirePay = 0;
                if (int.TryParse(appSettings.Settings["SelfTakeOrderExpirePay"].Value, out selfTakeOrderExpirePay))
                    _selfTakeOrderExpirePaySecond = selfTakeOrderExpirePay;

                bool isSystemDirectRefund;
                if (bool.TryParse(appSettings.Settings["IsSystemDirectRefund"].Value, out isSystemDirectRefund))
                    _isSystemDirectRefund = isSystemDirectRefund;

                int requestTimeOut;
                if (int.TryParse(appSettings.Settings["RequestTimeOut"].Value, out requestTimeOut))
                    _requestTimeOut = requestTimeOut;

                _equipmentUrl = appSettings.Settings["EquipmentUrl"].Value;

                _DownPrintControlPath = appSettings.Settings["DownPrintControlPath"].Value;
                _zshappkey = appSettings.Settings["zshappkey"].Value;

                _saleTax = appSettings.Settings["saleTax"].Value;
                _kpy = appSettings.Settings["kpy"].Value;
                _invoiceAppId = appSettings.Settings["InvoiceAppId"].Value;
                _YingKeBuCode = appSettings.Settings["YingKeBuCode"].Value;
                _YingKeSecretKey = appSettings.Settings["YingKeSecretKey"].Value;
                _YingKeGetCouponUrl = appSettings.Settings["YingKeGetCouponUrl"].Value;
                _AppIds = appSettings.Settings["AppIds"].Value;
                _SappIds = appSettings.Settings["SappIds"].Value;
                _TdAppIds = appSettings.Settings["TdAppIds"].Value;
                _YangAppIds = appSettings.Settings["YangAppIds"].Value;
                _TJAppIds = appSettings.Settings["TJAppIds"].Value;
                _JdJosAppKey = appSettings.Settings["JdJosAppKey"].Value;
                _JdJosAppSecret = appSettings.Settings["JdJosAppSecret"].Value;
                _JdJosAccessToken = appSettings.Settings["JdJosAccessToken"].Value;

                _ylptFeatureCode = appSettings.Settings["YlptFeatureCode"].Value;

                _BTPIP = "101.200.96.240";

                _Suning_UrlBase = appSettings.Settings["Suning_UrlBase"].Value;
                _Suning_AppKey = appSettings.Settings["Suning_AppKey"].Value;
                _Suning_AppSecret = appSettings.Settings["Suning_AppSecret"].Value;
                _Suning_CityId = appSettings.Settings["Suning_CityId"].Value;

                _FangZheng_UrlBase = appSettings.Settings["FangZheng_UrlBase"].Value;
                _FangZheng_ShopId = appSettings.Settings["FangZheng_ShopId"].Value;
                _FangZheng_Password = appSettings.Settings["FangZheng_Password"].Value;

                _YPK_UrlBase = appSettings.Settings["YPK_UrlBase"].Value;
                _client_id = appSettings.Settings["client_id"].Value;
                _client_secret = appSettings.Settings["client_secret"].Value;
                _grant_type = appSettings.Settings["grant_type"].Value;
                _companyid = appSettings.Settings["companyid"].Value;
                _username = appSettings.Settings["username"].Value;
                _password = appSettings.Settings["password"].Value;

                if (appSettings.Settings["Express100_UrlBase"] != null)
                    _Express100_UrlBase = appSettings.Settings["Express100_UrlBase"].Value;
                if (appSettings.Settings["Express100_Key"] != null)
                    _Express100_Key = appSettings.Settings["Express100_Key"].Value;
                if (appSettings.Settings["Express100_Customer"] != null)
                    _Express100_Customer = appSettings.Settings["Express100_Customer"].Value;

                if (appSettings.Settings["BTPIP"] != null && !string.IsNullOrEmpty(appSettings.Settings["BTPIP"].Value))
                {
                    _BTPIP = appSettings.Settings["BTPIP"].Value;
                }
                _DMWIPList = new List<string> { "60.205.7.199" };
                if (appSettings.Settings["DMWIPs"] != null && !string.IsNullOrEmpty(appSettings.Settings["DMWIPs"].Value))
                {
                    _DMWIPList.AddRange(appSettings.Settings["DMWIPs"].Value.Split(',').ToList());
                }

                var yjAppIdStr = ConfigurationManager.AppSettings["YJAppId"];
                Guid.TryParse(yjAppIdStr, out _YJAppId);
                //_OAPIApiKey = appSettings.Settings["OAPIApiKey"].Value;
                //_OAPICallerId = appSettings.Settings["OAPICallerId"].Value;
                //_OAPICheckTokenUrl = appSettings.Settings["OAPICheckTokenUrl"].Value;

                var jdAppIds = appSettings.Settings["AppIds"].Value;
                if (!string.IsNullOrEmpty(jdAppIds))
                {
                    jdAppIds.Split(',').ToList().ForEach(p =>
                    {
                        var jdAppId = Guid.Empty;
                        Guid.TryParse(p, out jdAppId);
                        if (jdAppId != Guid.Empty && !_JdAppIdList.Contains(jdAppId)) _JdAppIdList.Add(jdAppId);
                    });
                }

                var yxAppIds = appSettings.Settings["YXAppIds"].Value;
                if (!string.IsNullOrEmpty(yxAppIds))
                {
                    yxAppIds.Split(',').ToList().ForEach(p =>
                    {
                        var yxAppId = Guid.Empty;
                        Guid.TryParse(p, out yxAppId);
                        if (yxAppId != Guid.Empty && !_YxAppIdList.Contains(yxAppId)) _YxAppIdList.Add(yxAppId);
                    });
                }

                var snAppIds = appSettings.Settings["SappIds"];
                if (snAppIds != null)
                {
                    snAppIds.Value.Split(',').ToList().ForEach(p =>
                    {
                        var snAppId = Guid.Empty;
                        Guid.TryParse(p, out snAppId);
                        if (snAppId != Guid.Empty && !_SnAppIdList.Contains(snAppId)) _SnAppIdList.Add(snAppId);
                    });
                }

                var fzAppIds = appSettings.Settings["FZappIds"];
                if (fzAppIds != null)
                {
                    fzAppIds.Value.Split(',').ToList().ForEach(p =>
                    {
                        var fzAppId = Guid.Empty;
                        Guid.TryParse(p, out fzAppId);
                        if (fzAppId != Guid.Empty && !_FZAppIdList.Contains(fzAppId)) _FZAppIdList.Add(fzAppId);
                    });
                }

                var ypkAppIds = appSettings.Settings["YPKAppIds"].Value;
                if (!string.IsNullOrEmpty(ypkAppIds))
                {
                    ypkAppIds.Split(',').ToList().ForEach(p =>
                    {
                        var ypkAppId = Guid.Empty;
                        Guid.TryParse(p, out ypkAppId);
                        if (ypkAppId != Guid.Empty && !_YPKAppIdList.Contains(ypkAppId)) _YPKAppIdList.Add(ypkAppId);
                    });
                }

                _MqServerHost = appSettings.Settings["MqServerHost"].Value;
                _MqUserName = appSettings.Settings["MqUserName"].Value;
                _MqPassword = appSettings.Settings["MqPassword"].Value;
                _InsuranceHost = appSettings.Settings["InsuranceHost"].Value;

            }
            catch (Exception ex)
            {
                LogHelper.Error("初始化应用信息失败！错误信息", ex);
            }
        }

    }
}
