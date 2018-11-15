function getUrlPrerfix() {
    var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
    var url = document.location.href;
    var urlPrerfix = htmpro;
    if (url.indexOf(htmpro + "dev") >= 0) {
        urlPrerfix += "dev";
    }
    else if (url.indexOf(htmpro + "test") >= 0) {
        urlPrerfix += "test";
    }
    return urlPrerfix;
}
function getBtpDomain() {
    return document.location.protocol + "//" + window.location.host + "/";
}
//截取字符串
//参数1.字符串 2.从0开始截取多少 3.截取后末尾加的字符串
function SubStrShowLength(str, length, strshow) {

    var restr = str;
    if (str.length > length) {

        restr = str.substr(0, length - 1) + strshow;
    }
    return restr;
}
//判断传入的价格m 是否大于l 如果大于l 出l  d是单位
function SubMoneyShow(m, l, d) {
    var result = m;
    if (m >= l) {
        result = m / l + d;
    }
    return result;
}
//获取URL某值
function getQueryString(name) {
    var r;
    if (arguments.length > 1) {
        r = arguments[1].split('?')[1];
    } else {
        r = window.location.search.substr(1);
    }
    //	var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//获取APPid是否还在众筹中 (众筹功能暂不启用，如果需要启用，需要修改js和配置文件\Configuration\CustomConfig\Jinher.AMP.BTP\Configuration.config  ——》CrowdfundingFlag)
function GetCrowdfundingState(appid, opid) {
    return false;
    //start  GetCrowdfundingState
    var result = false;

    $.ajax({
        type: "get",
        contentType: "application/json",
        url: '/Mobile/GetCrowdfundingState',
        async: false,
        data: { "appId": appid },
        dataType: "json",
        success: function (data) {
            if (data.ResultCode == 0) {
                $("#" + opid).show();
                $("#" + opid).html("<img src=\"../../Content/Mobile/redredenvelope/smallcrowd.png\" style=\"width:50px;height:auto;\"/>");
                result = true;

            }

            $("#ajaxLoadBlind").remove();

        },
        beforeSend: function () {
            ajaxLoading('22', '');
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
            return result;
        }

    });


    //end  GetCrowdfundingState
    return result;
}
//JS验证 不是有效的值
function JsVilaDataNull(obj) {
    if (obj != undefined && obj != "" && obj != "null" && obj != null && obj != "undefined" && obj) {
        return true;
    }
    else {
        return false;
    }
}

function CheckIosBrowser() {
    var standalone = window.navigator.standalone;
    var userAgent = window.navigator.userAgent.toLowerCase();
    var safari = /safari/.test(userAgent);
    var ios = /iphone|ipod|ipad/.test(userAgent);
    if (ios) {
        if (!standalone && safari) {
            return true;
        } else if (standalone && !safari) {
            return true;
        } else if (!standalone && !safari) {
            return false;
        };
    }
    return false;
}

function getParamsInStr(srcQueryString, targetParamName) {
    var parr = srcQueryString.split(/&/gi);
    for (var i = 0; i < parr.length; i++) {
        var kv = parr[i].split(/=/gi);
        if (kv[0].toLowerCase() == targetParamName.toLowerCase()) {
            return kv[1];
        }
    }
}

function getZphUrlParams() {
    var result = "";
    if (JsVilaDataNull(sessionStorage.thirdPartParams)) {
        result = sessionStorage.thirdPartParams;
    }
    else {
        result = document.location.href;
        //没有参数直接返回。
        if (result.split(/\?/gi).length < 2) {
            return "";
        }
        result = result.split(/\?/gi)[1];
    }
    //参数中不包含source参数。
    if (result.toLowerCase().indexOf("source=") == -1) {
        result += "&source=internal";
    }
    if (result.toLowerCase().indexOf("esappid=") == -1) {
        result += "&esappid=" + sessionStorage.srcAppId;
    }
    if (result.toLowerCase().indexOf("thiszpho2o=") == -1) {
        result += "&thiszpho2o=1";
    }
    if (result.toLowerCase().indexOf("linkmall=") == -1) {
        result += "&linkmall=1";
    }
    //用户已经退出登录。
    if (JsVilaDataNull(sessionStorage.userLoginOut)) {
        //删除userid.
        var uid = getParamsInStr(result, "userId");
        if (JsVilaDataNull(uid)) {
            uid = "&userId=" + uid;
            var regex = eval("/" + uid + "/gi");
            result = result.replace(regex, "");
        }
        //删除sessionId.
        var sid = getParamsInStr(result, "sessionId");
        if (JsVilaDataNull(sid)) {
            sid = "&sessionId=" + sid;
            var regex = eval("/" + sid + "/gi");
            result = result.replace(regex, "");
        }
        //删除changOrg
        var chgOrg = getParamsInStr(result, "changeOrg");
        if (JsVilaDataNull(chgOrg)) {
            chgOrg = "&changeOrg=" + chgOrg;
            var regex = eval("/" + chgOrg + "/gi");
            result = result.replace(regex, "");
        }
    }
    else {
        //url中不包含用户信息。
        if (result.indexOf("userId=") == -1
        && JsVilaDataNull(getUserId())) {
            result = result + "&userId=" + getUserId();
            if (JsVilaDataNull(getSessionId())) {
                result = result + "&sessionId=" + getSessionId();
            }
            if (JsVilaDataNull(getQueryString("changeOrg"))) {
                result = result + "&changeOrg=" + getSessionId();
            } else {
                result = result + "&changeOrg=00000000-0000-0000-0000-000000000000";
            }
        }
    }
    //不以&开头，加上&.
    if (result.substr(0, 1) != "&") {
        result = "&" + result;
    }
    sessionStorage.thirdPartParams = result;
    return result;


}

function setCookieMills(b, c, e) {
    var d = new Date();
    d.setTime(d.getTime() + e);
    var a = window.document.domain.indexOf("iuoooo") >= 0 ? ".iuoooo.com" : window.document.domain;
    document.cookie = b + "=" + escape(c) + ";expires=" + d.toGMTString() + ";path=/;domain=" + a;
}

//写cookies 
function setSessionCookie(name, value) {
    var a = window.document.domain.indexOf("iuoooo") >= 0 ? ".iuoooo.com" : window.document.domain;
    document.cookie = name + "=" + escape(value) + ";path=/;domain=" + a;
}

//写cookies 
function setCookie(name, value) {
    var days = 30;
    var exp = new Date();
    var a = window.document.domain.indexOf("iuoooo") >= 0 ? ".iuoooo.com" : window.document.domain;
    exp.setTime(exp.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + ";path=/;domain=" + a;
}

//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}
//删除cookies 
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + ";path=/;domain=" + document.domain;
}

//User-Agent: Mozilla/5.0 (Linux; Android 5.0.1; MX5 Build/LRX22C) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/40.0.2214.114 Mobile Safari/537.36,appid=82a16e5b-e674-454b-b82e-6070868d1c99,versionCode=1.0.0,versionwebview=1.0.0,tag=jhwebview,from=android,deviceid=867992020740189
//appid=82a16e5b-e674-454b-b82e-6070868d1c99,versionCode=1.0.0,versionwebview=1.0.0,tag=jhwebview,from=android,deviceid=867992020740189
//校验是否金和应用内打开网页
//金和应用会在UserAgent里增加jhwebview标识
function isInJhApp() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    if (/jhwebview/.test(userAgent)) {
        return true;
    }
    return false;
}
//获取金和Webview版本号
function getJhWebviewVersion() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    if (/versionwebview/.test(userAgent)) {
        return userAgent.split('versionwebview=')[1].split(',')[0]; ;
    }
    return '';
}
function getJhWebviewAppId() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    if (/versionwebview/.test(userAgent)) {
        return userAgent.split('appid=')[1].split(',')[0]; ;
    }
    return '';

}
//比较当前金和浏览器版本与指定版本号，当前版本较新返回true，否则返回false
function isNewerJhWebview(lastVersion) {
    var thisVersion = getJhWebviewVersion();
    if (thisVersion) {
        if (thisVersion == lastVersion)
            return true;
        var thisVerArr = thisVersion.split('.');
        var lastVerArr = lastVersion.split('.');
        for (var i = 0; i < thisVerArr.length; i++) {
            if (lastVerArr.length < i + 1)
                return true;
            var verDec = parseInt(thisVerArr[i]) - parseInt(lastVerArr[i]);
            if (verDec > 0) {
                return true;
            }
            if (verDec < 0) {
                return false;
            }
        }
    }
    return false;
}







//表示全局唯一标识符 (GUID)。

function Guid(g) {
    var arr = new Array(); //存放32位数值的数组
    if (typeof (g) == "string") { //如果构造函数的参数为字符串
        InitByString(arr, g);
    }
    else {
        InitByOther(arr);
    }

    //返回一个值，该值指示 Guid 的两个实例是否表示同一个值。
    this.Equals = function (o) {
        if (o && o.IsGuid) {
            return this.ToString() == o.ToString();
        } else {
            return false;
        }
    };

    //Guid对象的标记
    this.IsGuid = function () {
    };
    //返回 Guid 类的此实例值的 String 表示形式。
    this.ToString = function (format) {
        if (typeof (format) == "string") {
            if (format == "N" || format == "D" || format == "B" || format == "P") {
                return ToStringWithFormat(arr, format);
            } else {
                return ToStringWithFormat(arr, "D");
            }
        } else {
            return ToStringWithFormat(arr, "D");
        }
    };

    //由字符串加载
    function InitByString(arr, g) {
        g = g.replace(/\{|\(|\)|\}|-/g, "");
        g = g.toLowerCase();
        if (g.length != 32 || g.search(/[^0-9,a-f]/i) != -1) {
            InitByOther(arr);
        }
        else {
            for (var i = 0; i < g.length; i++) {
                arr.push(g[i]);
            }
        }
    }

    //由其他类型加载
    function InitByOther(arr) {
        var i = 32;
        while (i--) {
            arr.push("0");
        }
    }

    /*
    根据所提供的格式说明符，返回此 Guid 实例值的 String 表示形式。
    N  32 位： xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    D  由连字符分隔的 32 位数字 xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
    B  括在大括号中、由连字符分隔的 32 位数字：{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
    P  括在圆括号中、由连字符分隔的 32 位数字：(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
    */
    function ToStringWithFormat(arr, format) {
        switch (format) {
            case "N":
                return arr.toString().replace(/,/g, "");
            case "D":
                var str = arr.slice(0, 8) + "-" + arr.slice(8, 12) + "-" + arr.slice(12, 16) + "-" + arr.slice(16, 20) + "-" + arr.slice(20, 32);
                str = str.replace(/,/g, "");
                return str;
            case "B":
                var str = ToStringWithFormat(arr, "D");
                str = "{" + str + "}";
                return str;
            case "P":
                var str = ToStringWithFormat(arr, "D");
                str = "(" + str + ")";
                return str;
            default:
                return new Guid();
        }
    }
}

//Guid 类的默认实例，其值保证均为零。
Guid.Empty = new Guid();
//初始化 Guid 类的一个新实例。
Guid.NewGuid = function () {
    var g = "";
    var i = 32;
    while (i--) {
        g += Math.floor(Math.random() * 16.0).toString(16);
    }
    return new Guid(g);
};
function checkCommonBrowser() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    if (/jhwebview/.test(userAgent)) {
        return false;
    }
    var safari = /safari/.test(userAgent);
    var ios = /iphone|ipod|ipad/.test(userAgent);
    if (ios && safari) {
        return true;
    }
    var isBrowsers = /ucweb|ucbrowser|qq|baidu|360|micromessenger|mxbrowser|sogou/.test(userAgent);
    return isBrowsers;
}
function isWeiXin() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    return /micromessenger/.test(userAgent);
}
function checkMobileParams() {
    //金和应用内
    if (isInJhApp()) {
        sessionStorage.source = "internal";
        if (JsVilaDataNull(getQueryString("producttype")) && getQueryString("producttype").indexOf('cjzy') >= 0) {
            sessionStorage.ProductType = 'appcjzy';
        }
    } else if (checkCommonBrowser()) {
        sessionStorage.source = "share";
        if (JsVilaDataNull(getQueryString("producttype")) && getQueryString("producttype").indexOf('cjzy') >= 0) {
            sessionStorage.ProductType = 'webcjzy';
        }
    } else {
        if (JsVilaDataNull(getQueryString('source'))) {
            sessionStorage.source = getQueryString('source');
        }

        if (JsVilaDataNull(getQueryString("producttype"))) {
            sessionStorage.ProductType = getQueryString("producttype");
        }
        if (sessionStorage.ProductType && sessionStorage.ProductType.indexOf('cjzy')) {
            if (sessionStorage.source == 'share') {
                sessionStorage.ProductType = 'webcjzy';
            } else {
                sessionStorage.ProductType = 'appcjzy';
            }

        }
    }
    if (JsVilaDataNull(getQueryString('isUserLoginOut'))) {
        sessionStorage.userLoginOut = null;
    }

    if (JsVilaDataNull(getQueryString("srcAppId"))) {
        sessionStorage.srcAppId = getQueryString("srcAppId");
    }
    if (JsVilaDataNull(getQueryString('appType'))) {
        sessionStorage.appTypeZPH = getQueryString('appType');
        //电商馆默认给馆主分成--兼容模式
        if (sessionStorage.appTypeZPH == 4 && JsVilaDataNull(getEsAppId())) {
            sessionStorage.speader = getEsAppId();
        }
    }
    else if (!JsVilaDataNull(sessionStorage.appTypeZPH)) {
        sessionStorage.appTypeZPH = 0;
    }
    if (JsVilaDataNull(getQueryString("srctype"))) {
        sessionStorage.SrcType = getQueryString("srctype");
    }
    if (JsVilaDataNull(getQueryString("type"))) {
        sessionStorage.AppType = getQueryString("type");
    }
    if (JsVilaDataNull(getQueryString("speader"))) {
        sessionStorage.speader = getQueryString("speader");
    }
    if (JsVilaDataNull(getQueryString('shopId'))) {
        sessionStorage.appId = getQueryString('shopId');
    }
    if (JsVilaDataNull(getQueryString('SrcTagId'))) {
        sessionStorage.SrcTagId = getQueryString('SrcTagId');
    }
    if (JsVilaDataNull(getQueryString('CPSId'))) {
        sessionStorage.CPSId = getQueryString('CPSId');
    }

    if (JsVilaDataNull(getQueryString("speader"))) {
        sessionStorage.speader = getQueryString("speader");
    }
    if (JsVilaDataNull(getQueryString("thiszpho2o"))) {
        sessionStorage.thiszpho2o = getQueryString("thiszpho2o");
    }
    if (JsVilaDataNull(getQueryString("linkmall"))) {
        sessionStorage.linkmall = getQueryString("linkmall");
    }
    if (!JsVilaDataNull(sessionStorage.SrcType)) {
        if (sessionStorage.ProductType == "appcjzy") {
            sessionStorage.SrcType = 36;
        } else if (sessionStorage.ProductType == "webcjzy") {
            sessionStorage.SrcType = 34;
        }
    }
    //分销商id.
    if (JsVilaDataNull(getQueryString("distributorId"))) {
        sessionStorage.distributorId = getQueryString("distributorId");
    }
}
function getEsAppId() {
    var bDataCookie = getCookie("b_data");
    try {
        var bData = JSON.parse(bDataCookie);
        if (bData && "undefined" != typeof (bData.appId) && JsVilaDataNull(bData.appId)) {
            return bData.appId;
        }
    } catch (e) {

    }
    return "00000000-0000-0000-0000-000000000000";

}

function getShareId() {
    var shareId = '';
    try {
        shareId = bridge.getData('share');
    } catch (e) {

    }
    return shareId || '';
}
function getQueryStringFromUrl(name, str) {
    var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}
function addParam(name, value, url) {
    if (!JsVilaDataNull(url))
        return "";
    if (!JsVilaDataNull(name) || !JsVilaDataNull(value))
        return url;
    var params = url ? url.split('?')[1] : '';
    var newParam = name + "=" + value;
    if (!params) {
        return url + "?" + newParam;
    }
    var reg = new RegExp("(^|&)" + name + "=([^&]*)", "i");
    var r = params.match(reg);
    if (r != null) {
        url = url.replace(r[0], "&" + newParam);
    } else {
        url += "&" + newParam;
    }
    return url;
}
//url中增加通用参数
function urlAppendCommonParams(url) {
    if (!JsVilaDataNull(url))
        return "";
    url = addParam("appId", getEsAppId(), url);
    if (JsVilaDataNull(sessionStorage.SrcType)) {
        url = addParam("srctype", sessionStorage.SrcType, url);
    }
    if (JsVilaDataNull(sessionStorage.ProductType)) {
        url = addParam("producttype", sessionStorage.ProductType, url);
    }
    if (JsVilaDataNull(sessionStorage.source)) {
        url = addParam("source", sessionStorage.source, url);
    }
    var shareId = getShareId();
    if (JsVilaDataNull(shareId)) {
        url = addParam("share", shareId, url);
    }
    if (JsVilaDataNull(sessionStorage.SrcTagId)) {
        url = addParam("SrcTagId", sessionStorage.SrcTagId, url);
    }
    return url;
}
function getCurrency() {

    var arr, reg = new RegExp("(^| )btpcommon=([^;]*)(;|$)");
    var cookie = "";
    if (arr = document.cookie.match(reg))
        cookie = decodeURIComponent(arr[2]);
    else
        return "￥";

    var obj = JSON.parse(cookie);
    if (obj && obj.Currency) {
        return obj.Currency;
    }
    return "￥";
}

function dateFormat(data,state){
    var month = data.getMonth() + 1 < 10 ? '0' + (data.getMonth() + 1): (data.getMonth() + 1);
    var date = data.getDate() < 10 ? '0' + data.getDate() : data.getDate();
    var hours = data.getHours() < 10 ? '0' + data.getHours() : data.getHours();
    var minus = data.getMinutes() < 10 ? '0' + data.getMinutes(): data.getMinutes();
    if(state == 1){
        return data.getFullYear() + '-' + month + '-' + date + ' ' + hours + ':' + minus;
    }else{
        return data.getFullYear() + '-' + month + '-' + date;
    }
}
String.prototype.trim =function() {
    return this.replace(/(^\s*)|(\s*$)/g,"");
};
function getUserId() {
    var returnvalue = getCookie("CookieContextDTO");
    var dto;
    try {
        dto = JSON.parse(returnvalue);
    } catch (e) {
        dto = createContextDto();
    }
    if (dto && JsVilaDataNull(dto.userId))
        return dto.userId;
    return null;
}
function isLogin() {
    var returnvalue = getCookie("CookieContextDTO");
    var dto;
    try {
        dto = JSON.parse(returnvalue);
    } catch (e) {
        dto = createContextDto();
    }

    if (!dto || !JsVilaDataNull(dto.userId) || !JsVilaDataNull(dto.sessionId) || !JsVilaDataNull(dto.changeOrg))
        return false;
    return true;
}