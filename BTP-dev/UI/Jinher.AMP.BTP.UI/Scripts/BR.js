var ip = "";
var uId = "";
//window.onload = function () {
var scripts = document.getElementsByTagName('script');
var currentScript = scripts[scripts.length - 1];
var url = currentScript.src.split('?');
var args = GetUrlParameters(url[1]);
if (args != null && args != undefined) {
    if (url.length > 1) {
        var cip = args["IP"];
        if (cip != undefined) {
            ip = cip;
        }
        var brCode = args["BrCode"];
        if (brCode != undefined) {
            RB(brCode);
        }

    }
}
//}

function DRB(code, cip,userid) {
    ip = cip;
    uId=userid
    RB(code);
}

function RB(code) {
    var url = getUrl();
    var json = getJsonData(code);
    //    url = url + "?" + json;
    //    jhajax({
    //        async: true,
    //        type: "get",
    //        data: json,
    //        url: url,
    //        dataType: "json",
    //        success: function (data) {
    //            if (data != null) {

    //            }
    //        }
    //    });

    var id = newGuid();
    var head = document.getElementsByTagName("body").item(0);
    var oScript = document.createElement("script");
    url = url + "?" + json;
    oScript.setAttribute("id", id);
    oScript.setAttribute("type", "text/javascript");
    oScript.setAttribute("language", "javascript");
    oScript.setAttribute("src", url);
    head.appendChild(oScript);
}

function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}

function getJsonData(code) {
    var random = newGuid();
    var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
    var hj = "", url = document.location.href;
    if (url.indexOf(htmpro + "dev") >= 0) {
        hj = "dev";
    }
    else if (url.indexOf(htmpro + "test") >= 0) {
        hj = "test";
    }
    else if (url.indexOf(htmpro + "pre") >= 0) {
        hj = "pre";
    }
    var port = "";
    if (window.location.port != "") {
        port = ":" + window.location.port;
    }
    htmpro = htmpro + hj;
    var url = htmpro + window.location.host + port + window.location.pathname;
    var domain = document.domain;
    var browser = GetBrowser();
    var json = "BehaviorCode=" + code + "&CreateIP=" + ip + "&CreateUser=" + uId + "&Domain=" + domain + "&Url=" + url + "&Browser=" + browser + "&Random=" + random;
    return json;
}

function getUrl() {
    var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
    var hj = "", url = document.location.href;
    if (url.indexOf(htmpro + "dev") >= 0) {
        hj = "dev";
    }
    else if (url.indexOf(htmpro + "test") >= 0) {
        hj = "test";
    }
    else if (url.indexOf(htmpro + "pre") >= 0) {
        hj = "pre";
    }
    var port = "";
    if (window.location.port != "") {
        port = ":" + window.location.port;
    }
    htmpro = htmpro + hj;
    var domain = document.domain;
    var newd = domain.indexOf("iuoooo") >= 0 ? "iuoooo" : domain.indexOf("jinher") >= 0 ? "jinher" : "appmfl";
    var newdomain = "bac." + newd + ".com";
    var url = htmpro + newdomain + port + '/Behavior/RememberBehaviorRecode';
    return url;
}

function GetMac() {
    var locator = new ActiveXObject("WbemScripting.SWbemLocator");
    var service = locator.ConnectServer(".");
    var properties = service.ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration Where IPEnabled=TRUE");
    var e = new Enumerator(properties);
    var i = 1;
    var mac = "";
    for (; !e.atEnd(); e.moveNext()) {
        var p = e.item();
        mac = p.MACAddress;
        break;
    }
    return mac;
}

function GetBrowser() {
    var ClientParams = {};
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
     (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
     (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
     (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
     (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    if (Sys.ie) {
        ClientParams.Browser = "IE" + Sys.ie;
    }
    else if (Sys.firefox) {
        ClientParams.Browser = "Firefox" + Sys.firefox;
    }
    else if (Sys.chrome) {
        ClientParams.Browser = "Chrome" + Sys.chrome;
    }
    else if (Sys.opera) {
        ClientParams.Browser = "Opera" + Sys.opera;
    }
    else if (Sys.safari) {
        ClientParams.Browser = "Safari" + Sys.safari;
    }
    else {
        ClientParams.Browser = "未知浏览器";
    }
    return ClientParams.Browser;
}

//ajax封装
var jhajax = function (config) {
    //url参数，必填     
    var url = config.url;

    //data参数可选，只有在post请求时需要,如果调用wcf服务要使用json格式，如：{"appId":"sfsfsfsfsf"}
    var data = config.data;

    //datatype参数可选：xml,text,json       
    var dataType = config.dataType;

    //成功回调函数可选：回调函数
    var success = config.success;

    //type参数,可选：get,post   
    var type = config.type;
    if (type == null) {
        //type参数可选，默认为get        
        type = "get";
    }
    if (dataType == null) {
        //dataType参数可选，默认为text        
        dataType = "text";
    }

    //同步和异步操作:true,false
    var async = config.async;
    if (async == null) {
        async = false;
    }

    // 创建ajax引擎对象    
    var xmlhttp = RetuenAjaxXmlHttp();

    xmlhttp.onreadystatechange = function () {

        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            if (dataType == "text" || dataType == "TEXT") {
                if (success != null) {
                    //普通文本                   
                    success(xmlhttp.responseText);
                }
            } else if (dataType == "xml" || dataType == "XML") {
                if (success != null) {
                    //接收xml文档                     
                    success(xmlhttp.responseXML);
                }
            } else if (dataType == "json" || dataType == "JSON") {
                if (success != null) {
                    //将json字符串转换为js对象  
                    if (xmlhttp.responseText != "") {
                        success(eval('(' + xmlhttp.responseText + ')'));
                    }
                }
            }
        }
    }

    // 打开    
    xmlhttp.open(type, url, async);
    // 发送   
    if (type == "GET" || type == "get") {
        xmlhttp.send(null);
    }
    else if (type == "POST" || type == "post") {
        xmlhttp.setRequestHeader("content-type", "application/x-www-form-urlencoded");
        xmlhttp.send(data);
    }
}

//生成httprequest对象，用于ajax操作
function RetuenAjaxXmlHttp() {
    var xmlhttp;
    try {
        //创建不是ie浏览器的对象
        xmlhttp = new XMLHttpRequest();
    }
    catch (e) {
        try {

            //创建是 ie6 ie7  ie8  浏览器的对象
            xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try {
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            catch (e) {
                alert("您的浏览器不支持Ajax");
                return false;
            }
        }
    }
    //返回一个对象
    return xmlhttp;
}

function GetUrlParameters(url) {
    if (url == null || url == undefined)
        return null;

    var paramsArr = url.split('&');
    var args = {}, argsStr = [], param, t, name, value;
    for (var i = 0, len = paramsArr.length; i < len; i++) {
        param = paramsArr[i].split('=');
        name = param[0], value = param[1];
        if (typeof args[name] == "undefined") { //参数尚不存在
            args[name] = value;
        } else if (typeof args[name] == "string") { //参数已经存在则保存为数组
            args[name] = [args[name]]
            args[name].push(value);
        } else {  //已经是数组的
            args[name].push(value);
        }
    }
    return args;
}
         