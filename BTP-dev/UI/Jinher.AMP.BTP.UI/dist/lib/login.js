var rad = Math.random();
//登录处理页所有js逻辑。
var DealLoginPartial = (function () {
    //登录成功回掉。
    var fnCallback;
    //登出回调
    var fnLoginOut;

    //初始化页面。

    function initPartialPage() {
        if (isLogin()) {
            fnCallback && fnCallback();
            return;
        }
        //web登录
        if (sessionStorage.source == "share") {
            sologin();
            return;
        }
        if (sessionStorage.ProductType == "webcjzy") {
            sologin();
            return;
        }

        var startWebviewVersion = "1.0.0";
        if (isNewerJhWebview(startWebviewVersion)) {
            var agrsUrl = "{\"businessJson\":\"\",\"businessType\":3}";
            var tagUrl = _pageId;
            var agrsUrlBase64 = new Base64().encode(agrsUrl);
            var tagUrlBase64 = new Base64().encode(tagUrl);
            window.location.href = "jhoabrowser://userLoginFunction?args=" + agrsUrlBase64 + "&tag=" + tagUrlBase64;
            return;
        } else {
            //if ($.os.ios) {//去掉了zepto.js,使用isIos()函数判断
            if (isIos()) {
                var backUrl = window.location.href.replace("http://dev", "").replace("http://test", "").replace("http://pre", "").replace("http://", "").replace("https://dev", "").replace("https://test", "").replace("https://pre", "").replace("https://", "");
                var psource = sessionStorage.source == "share" ? "&source=share" : "";
                backUrl = backUrl.indexOf("?") > 0 ? backUrl : backUrl + "?n=1";
                window.location.href = '/Mobile/vcheckuserlogin?callback=' + backUrl + psource + "&isUserLoginOut=no";
                window.location.href = getH5LoginUrl();
            } else {
                //Android事件
                try {
                    var userandsessionid = window.getUserIdAndSessionId.getUserIdAndSessionId();
                    if (!JsVilaDataNull(userandsessionid) || userandsessionid.indexOf(',') <= 0) {
                        return;
                    }
                    localStorage.sessionId = userandsessionid.split(',')[1];
                    saveContextDTO(userandsessionid.split(',')[0], userandsessionid.split(',')[1], getQueryString('changeOrg') || Guid.NewGuid().ToString());
                    fnCallback && fnCallback();
                } catch (e) {
                    sologin();
                }
            }
        }
    }
    //edit by ykk
    //判断是否是原生ios webview
    function isIos(){
        var ua = navigator.userAgent;
        var ipad = ua.match(/(iPad).*OS\s([\d_]+)/);
        var ipod = ua.match(/(iPod)(.*OS\s([\d_]+))?/);
        var iphone = !ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/);
        if((iphone && !ipod) || ipad || ipod){
            return true;
        }
        return false
    }
    function loginOutCallBack() {
        fnLoginOut && fnLoginOut();
    }

    //web登录
    function sologin() {
        var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
        var url = document.location.href;
        url = url.replace(/&islogin=[^&]*/g, "");
        url = url.replace(/\?islogin=[^&]*&/g, "\?");
        url = url.replace("appid", "appId");   //pip  无法判断appid  appId为同一个参数
        var urlPrerfix = htmpro;
        if (url.indexOf(htmpro + "dev") >= 0) {
            urlPrerfix += "dev";
        }
        else if (url.indexOf(htmpro + "test") >= 0) {
            urlPrerfix += "test";
        }
        if (isWeiXin()) {
            window.location.href = urlPrerfix + "pip.iuoooo.com/newpager/view/userLogin.html?wechatLogin=1&rad=" + rad + "&appId=" + getEsAppId() + "&url=" + encodeURIComponent(url);
        } else {
            window.location.href = urlPrerfix + "pip.iuoooo.com/mypager/pages/userLogin.html?rad=" + rad + "&appId=" + getEsAppId() + "&url=" + encodeURIComponent(url);
        }
    }

    var dlp = new Object();
    dlp.initPartialPage = initPartialPage;
    dlp.loginOutCallback = loginOutCallBack;
    dlp.setLoginOut = function (fn) {
        fnLoginOut = fn;
    };
    dlp.setCallback = function (fn) {
        fnCallback = fn;
    };
    return dlp;
} ())



function checkLogin(callBack) {
    if (isLogin())
        callBack();
}

function setUserLoginInfo(jsonStr, tagJson) {
    if (tagJson != _pageId)
        return;
    if (jsonStr == null)
        return;
    saveContextDTO(JSON.parse(JSON.parse(jsonStr.businessJson).objInfo).userId, JSON.parse(JSON.parse(jsonStr.businessJson).objInfo).sessionId, JSON.parse(JSON.parse(jsonStr.businessJson).objInfo).changeOrg);
    DealLoginPartial && DealLoginPartial.initPartialPage();
}
//安卓回调：登录成功后的通知
function LoginNotifyJS() {
    if (DealLoginPartial == undefined) {
        return;
    }
    sessionStorage.userLoginOut = null;
    DealLoginPartial.initPartialPage();
}

function LoginNotifyJSNew(data) {
    var userData = JSON.parse(data);
    if (DealLoginPartial == undefined) {
        return;
    }
    if (userData && userData.appId != undefined) {
        if (userData.isLogin == "0") {
            delContextDTO();
            DealLoginPartial.loginOutCallback();
        } else {
            sessionStorage.userLoginOut = null;
            saveContextDTO(userData.userId, userData.sessionId, getQueryString('changeOrg') || Guid.NewGuid().ToString());
            DealLoginPartial && DealLoginPartial.initPartialPage();
        }
    }
}
//老版本应用登录赋值
function setUserId(userId) {
    saveContextDTO(userId, getSessionId(), getQueryString('changeOrg') || Guid.NewGuid().ToString());
}
//老版本应用登录赋值
function setSessionId(sessionId) {
    saveContextDTO(getUserId(), sessionId, getQueryString('changeOrg') || Guid.NewGuid().ToString());
}