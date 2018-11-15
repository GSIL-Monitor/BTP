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
function gotoCreateOrder(createOrderType, diyGroupId, totalPrice) {
    sessionStorage.PicturesPathModel = "";
    sessionStorage.PicturesPathModelMobile = "";
    sessionStorage.CreateOrderInfo = '';
    sessionStorage.userSelfTake = '';
    sessionStorage.removeItem("PicturesPathModel");
    sessionStorage.removeItem("PicturesPathModelMobile");
    sessionStorage.removeItem("CreateOrderInfo");
    sessionStorage.removeItem("userSelfTake");
    var url = urlAppendCommonParams('/Mobile/CreateOrder?type=' + createOrderType);
    if (JsVilaDataNull(diyGroupId)) {
        url = addParam("diyGroupId", diyGroupId, url);
    }
    if (JsVilaDataNull(totalPrice)) {
        url = addParam("price", totalPrice, url);
    }
    window.location.href = url;
}

var dateFormat = function (data, state) {
    try {
        var month = data.getMonth() + 1 < 10 ? '0' + (data.getMonth() + 1) : (data.getMonth() + 1);
        var date = data.getDate() < 10 ? '0' + data.getDate() : data.getDate();
        var hours = data.getHours() < 10 ? '0' + data.getHours() : data.getHours();
        var minus = data.getMinutes() < 10 ? '0' + data.getMinutes() : data.getMinutes();
        if (state == 1) {
            return data.getFullYear() + '-' + month + '-' + date + ' ' + hours + ':' + minus;
        } else {
            return data.getFullYear() + '-' + month + '-' + date;
        }
    } catch (e) {
        return '';
    }
}
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
};
/**
* 显示对应订单状态的按钮
* //可显示的操作按钮：0、确认付款;1、取消订单;2、退款;3、申请退款/退货;4、确认收货;5、撤销退款申请;6、撤销退款/退货申请;7、评价;8、查看退款详情; 9、删除; 10、退货方式；11、延长收货时间(3天) 12、查看退款方式23、售后-申请退款/退货;24、售后-撤销退款/退货申请; 25、售后-查看退款详情； 26、售后-退货方式；  27、售后-查看退货方式；30、撤销退款/退货申请 31、退款详情 32、退货方式 33、查看退货方式
* @returns {Array}
*/
var getButtonByState = function (orderDetail, payment) {
    var btnArr = [];
    switch (orderDetail.State) {
        case 0: //=================当前"待付款"  显示："取消订单"、"确认付款"
            btnArr.push(1); //取消订单
            btnArr.push(0); //确认付款
            break;
        case 1: //=============当前"待发货" 显示: "退款"、“取消订单”、"确认收货"（自提商品显示确认收货按钮）
            //订单总价为0或当前商品支付价格为0，则不显示退款
            if (payment != 1 && (orderDetail.Price + orderDetail.ScorePrice !== 0 || (orderDetail.Freight !== 0 && orderDetail.Price !== 0) || (orderDetail.Freight !== 0 && orderDetail.CouponValue !== orderDetail.Price))) {
                btnArr.push(2); //退款
            } else {
                btnArr.push(1); //取消订单
            }
            if (orderDetail.SelfTakeFlag == 1) { //自提商品
                btnArr.push(4); //确认收货
            }
            break;
        case 2: //=============当前"已发货" 显示："延长收货时间(3天)"、"确认收货"、"申请退款/退货"、“查看物流”（在订单列表页面中需要）
            //转换时间格式 为了计算日期时间差
            if (orderDetail.ShipmentsTime) {
                var eDate = dateFormat(new Date());
                var sDate = dateFormat(new Date(parseInt(orderDetail.ShipmentsTime.replace("/Date(", "").replace(")/", ""), 10)));
                var result = (new Date(eDate) - new Date(sDate)) / (24 * 60 * 60 * 1000) + 1;
                if (result >= 6 && result <= 9 && !orderDetail.IsDelayConfirmTime) {//如果没有延长过时间显示“延长收货时间(3天)”
                    btnArr.push(11); //延长收货时间(3天)
                }
            }
            if (orderDetail.ShipExpCo && orderDetail.ExpOrderNo) {//物流和物流单号存在，显示“查看物流”
                btnArr.push(34); //查看物流
            }
            btnArr.push(4);
            if (orderDetail.Payment != 1) {
                btnArr.push(3); //申请退款/退货
            }
            break;
        case 3: //=============当前"交易成功"  显示："去评价"、"申请售后"、“退款详情”
            if (orderDetail.OrderType !== 1) {//如果没有评价并且不是服务订单，则显示“去评价”按钮
                btnArr.push(7);
            }
            //售后-订单状态：确认收货=3，售后退款中=5,已退款=7，已发货退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12 ,买家发货超时，商家未收到货=13,售后交易成功=15
            if (orderDetail.StateAfterSales == 3 && orderDetail.OrderType == 0) {
                btnArr.push(23); //申请售后
            } else if (orderDetail.StateAfterSales == 5) {
                btnArr.push(25); //退款详情
                btnArr.push(24); //售后-撤销退款/退货申请
            } else if (orderDetail.StateAfterSales == 7) {
                btnArr.push(25); //退款详情
                //退款退货订单，且有退货物流单号，显示物流详情。
                if (orderDetail.RefundType == 1 && JsVilaDataNull(orderDetail.RefundExpCo)) {
                    btnArr.push(27); //售后-查看退货方式
                }
            } else if (orderDetail.StateAfterSales == 10) {
                btnArr.push(25); //退款详情
                if (JsVilaDataNull(orderDetail.RefundExpCo)) {
                    //已发货，不能再撤销退款\退货
                    btnArr.push(27); //售后-查看退货方式
                } else {
                    btnArr.push(24); //售后-撤销退款/退货申请
                    btnArr.push(26); //售后-退货方式
                }
            } else if (orderDetail.StateAfterSales == 15) {
                btnArr.push(9); //"删除订单"
            }
            break;
        case 4: //=============当前"交易失败"
        case 5: //=============当前"交易失败"
        case 6: //==============当前"交易关闭" 显示："删除订单"
            btnArr.push(9);
            break;
        case 7: //==============当前"已退款" 显示：“退款详情”
            if (orderDetail.SelfTakeFlag != 1) {
                btnArr.push(31); //退款详情
                //退款退货订单，且有退货物流单号，显示物流详情。
                if (orderDetail.RefundType == 1 && JsVilaDataNull(orderDetail.RefundExpCo)) {
                    btnArr.push(33); //查看退货方式
                }
            }
            break;
        case 8: //==============当前"退款中"
        case 9: //===============当前"退款中"
        case 14: //==============当前"退款中" 显示："退款详情"
            btnArr.push(30); //撤销退款申请
            btnArr.push(31); //退款详情
            break;
        case 10: //===============当前"退款中"
            btnArr.push(31); //查看退款详情
            if (JsVilaDataNull(orderDetail.RefundExpCo)) {
                btnArr.push(33); //查看退货方式
            } else {
                if (orderDetail.OrderRefundState != 11) {
                    btnArr.push(30); //撤销退款申请
                    btnArr.push(32); //退货方式
                }
            }
            break;
        case 11: //==============当前"待发货"  显示：“退款”
            //订单总价为0或当前商品支付价格为0，则不显示退款
            if (orderDetail.Payment != 1 || (orderDetail.Price + orderDetail.ScorePrice !== 0 || (orderDetail.Freight !== 0 && orderDetail.Price !== 0) || (orderDetail.Freight !== 0 && orderDetail.CouponValue !== orderDetail.Price))) {
                btnArr.push(2);
            }
            break;
        case 13: //============当前"出库中" 显示：“确认收货”、“申请退款/退货”
            btnArr.push(4); //确认收货
            if (orderDetail.Payment != 1) {
                btnArr.push(3); //申请退款/退货
            }
            break;
    }
    return btnArr;
};
/**
* 获取订单状态 
*/
var getOrderStateText = function (state, StateAfterSales) {
    //订单状态：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14
    var text = "";
    switch (state) {
        case 0:
            text = "待付款";
            break;
        case 1:
        case 11:
            text = "待发货";
            break;
        case 13:
            text = "出库中";
            break;
        case 2:
            text = "已发货";
            break;
        case 3:
            text = "交易成功";
            //已开始售后，显示售后状态。
            if (JsVilaDataNull(StateAfterSales)) {
                var asText = getAfterSaleState(StateAfterSales);
                JsVilaDataNull(asText) && (text = asText);
            }
            break;
        case 4:
        case 5:
            text = "交易失败";
            break;
        case 6:
            text = "交易关闭";
            break;
        case 7:
            text = "已退款";
            break;
        case 8:
        case 9:
        case 10:
        case 14:
            text = "退款中";
            break;
        case 12:
            text = "金和处理退款中";
            break;
    }
    return text;
};
/**
* 获取售后状态显示文本
* @param afterSaleState 售后订单状态
* @returns {string}
*/
var getAfterSaleState = function (afterSaleState) {
    //售后-订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,售后交易成功=15
    var text = "";
    if (afterSaleState == 3 || afterSaleState == 15) {
        text = "交易成功";
    } else if (afterSaleState == 5 || afterSaleState == 10) {
        text = "退款中";
    } else if (afterSaleState == 7) {
        text = "已退款";
    } else if (afterSaleState == 12) {
        text = "金和处理退款中";
    }
    return text;
};
var formatLongString = function (str, maxLength) {
    if (!str)
        return str;
    var result = str;
    if (str.length > maxLength) {
        result = str.substr(0, maxLength - 1) + "…";
    }
    return result;
};
