/**
 * Mercury
 * Created by zhangyh on 2015/3/10.
 * Modified by zhangyh on 2016/12/27
 */
//--------------------------------------------------------core----------------------------------------------------------
/**
 * @module mercury | $m
 * @type Object
 */
var mercury = {
    ipApi: window.location.origin.indexOf("//dev") >= 0 || window.location.origin.indexOf("//192.") >= 0 || window.location.origin.indexOf("//localhost") >= 0 ?
        '//devtools.iuoooo.com/ClientIpManage/GetClientIp?callback=getIP' :
        window.location.origin.indexOf("//test") >= 0 ?
            '//testtools.iuoooo.com/ClientIpManage/GetClientIp?callback=getIP' :
            '//tools.iuoooo.com/ClientIpManage/GetClientIp?callback=getIP',
    url: window.location.origin.indexOf("//dev") >= 0 || window.location.origin.indexOf("//192.") >= 0 || window.location.origin.indexOf("//localhost") >= 0 || window.location.origin.indexOf("//test") >= 0 ? "//md.iuoooo.com" + (document.location.origin.substring(0, 5) == 'https' ? '' : ':8000' + "/") + "/" : "//md.iuoooo.com" + (document.location.origin.substring(0, 5) == 'https' ? '' : ':8000' + "/") + "/", //备用:开发/生产环境嗅探。备注:由于埋点服务器非标准化的逆天地址策略，不可避免的需要拼接端口 -_-!!  http - md.iuoooo.com:8000 ,https - md.iuoooo.com
    getTime: function () {
        /**
         * @class getTime
         * @extends mercury
         * @return {Number} 时间
         * */
        return new Date().getTime();
    },
    getScript: function (src, callback) {
        /**
         * @class getScript
         * @extends mercury
         * @param src {String} 资源路径
         * @param callback {Function} 回调函数
         * */
        var _doc = document.getElementsByTagName('head')[0];
        var script = document.createElement('script');
        script.setAttribute('type', 'text/javascript');
        script.setAttribute('src', src);
        _doc.appendChild(script);
        script.onload = script.onreadystatechange = function () {
            if (!this.readyState || this.readyState == 'loaded' || this.readyState == 'complete') {
                callback();
            }
            script.onload = script.onreadystatechange = null;
        }
    },
    sendUrl: function (url) {
        /**
         * @class sendUrl
         * @extends mercury
         * @param url {String} 请求地址
         * */
        var iFrame;
        iFrame = document.createElement("iframe");
        iFrame.setAttribute("src", url);
        iFrame.setAttribute("style", "display:none;");
        iFrame.setAttribute("height", "0");
        iFrame.setAttribute("width", "0");
        iFrame.setAttribute("frameborder", "0");
        document.body.appendChild(iFrame);
        setTimeout(function () {
            iFrame.parentNode.removeChild(iFrame);
            iFrame = null;
        }, 300)
    },
    imgUrl: function (url) {
        /**
         * @class imgUrl
         * @extends mercury
         * @param url {String} 请求地址
         * */
        var img;
        img = document.createElement("img");
        img.setAttribute("src", url);
        img.setAttribute("style", "display:none;");
        img.setAttribute("height", "0");
        img.setAttribute("width", "0");
        document.body.appendChild(img);
        setTimeout(function () {
            img.parentNode.removeChild(img);
            img = null;
        }, 1000)
    },
    setCookieMills: function (key, value, time) {
        /**
         * @class setCookieMills
         * @extends mercury
         * @param key {String} 键
         * @param value {String} 值
         * @param time {Number} 存储时间
         * */
        var d = new Date();
        d.setTime(d.getTime() + time);
        var a = window.document.domain.indexOf("iuoooo") >= 0 ? ".iuoooo.com" : window.document.domain;
        document.cookie = key + "=" + escape(value) + ";expires=" + d.toGMTString() + ";path=/;domain=" + a;
    },
    getCookie: function (key) {
        /**
         * @class getCookie
         * @extends mercury
         * @param key {String} 回调函数
         * */
        var a = document.cookie.match(new RegExp("(^| )" + key + "=([^;]*)(;|$)"));
        if (a != null) {
            return unescape(a[2])
        }
        return null
    },
    deleteCookie: function (key) {
        /**
         * @class deleteCookie
         * @extends mercury
         * @param key {String} 回调函数
         * */
        var b = $m.getCookie(key);
        if (b != null) {
            $m.setCookieMills(key, "", -1)
        }
    },
    ready: function (callback) {
        /**
         * @class ready
         * @extends mercury
         * @param callback {Function} 回调函数
         * */
        //TODO:确保在所有数据收集完成后再发送请求
    },
    getSession: function (key) {
        /**
         * @class getSession
         * @extends mercury
         * @param key {String} 键
         * @return  {String} 值
         * */
        return sessionStorage.getItem(key);
    },
    setSession: function (key, value) {
        /**
         * @class setSession
         * @extends mercury
         * @param key {String} 键
         * @param value {String} 值
         * */
        sessionStorage.setItem(key, value);
    },
    getStorage: function (key) {
        /**
         * @class getStorage
         * @extends mercury
         * @param key {String} 键
         * @return  {String} 值
         * */
        return localStorage.getItem(key);
    },
    setStorage: function (key, value) {
        /**
         * @class setStorage
         * @extends mercury
         * @param key {String} 键
         * @param value {String} 值
         * */
        localStorage.setItem(key, value);
    },
    init: function () {
        /*初始化*/
        setPos(); //初始化经纬度
        tempUserId(); //初始化临时ID
        tempSessionId();//初始化Session
        lastUserId();//初始化上次登录信息
    }
};

var $m = mercury; //语法糖

//--------------------------------------------------------业务代码----------------------------------------------------------
//Get IP
var returnCitySN = {};
function getIP(data) {
    returnCitySN.cip = data.IP;
}

$m.getScript($m.ipApi, function () {
});


/*经纬度*/
function setPos() {
    $m.setStorage("location", "");
    /*navigator.geolocation.getCurrentPosition(function (position) {
     $m.setStorage("location", position.coords.latitude + "," + position.coords.longitude);
     });*/
    //临时屏蔽地理位置 2016/2/03
    $m.setStorage("location", "0" + "," + "0");
}
function getPos() {
    return $m.getStorage("location");
}


/*临时ID*/
function tempUserId() {
    if ($m.getStorage("tempUserId") == null || $m.getCookie("tempUserId") == null) {
        var c = $m.getCookie("tempUserId") || $m.getStorage("tempUserId") || newGuid();
        $m.setStorage("tempUserId", c);
        $m.setCookieMills("tempUserId", c, 86400000 * 360);
    }
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

/*构建session*/
function tempSessionId() {
    if ($m.getCookie("tempSessionId") == null) {
        var len = 10;
        var $chars = '1234567890';
        var maxPos = $chars.length;
        var c = '';
        for (i = 0; i < len; i++) {
            c += $chars.charAt(Math.floor(Math.random() * maxPos));
        }
        $m.setCookieMills("tempSessionId", c);
    }
}
/*获取浏览器类型*/
function browserinfo() {
    var br = window.navigator.appVersion;
    var info, os;
    if (br.indexOf("Chrome") > 0) {
        info = "Chrome";
    }
    if (br.indexOf("UCBrowser") > 0) {
        info = "UCBrowser";
    }
    if (br.indexOf("MQQBrowser") > 0) {
        info = "MQQBrowser";
    }
    if (br.indexOf("MicroMissenger") > 0) {
        info = "MicroMissenger";
    }
    if (br.indexOf("MSIE") > 0) {
        info = "MSIE";
    }
    if (br.indexOf('Windows') > 0) {
        os = "Windows";
    }
    if (br.indexOf('Mac OS') > 0) {
        os = "MacOS";
    }
    if (br.indexOf('Windows Phone') > 0) {
        os = "WindowsPhone";
    }
    if (br.indexOf('Android') > 0 || br.indexOf('Linux') > 0) {
        os = "Android";
    }
    if (br.indexOf('iPhone') > 0) {
        os = "iPhone";
    }
    if (br.indexOf('iPad') > 0) {
        os = "iPad";
    }

    return 'browserinfo:' + (info ? info : '') + '|' + 'plattype:' + (os ? os : '') + '|';
}
/*lastUserId*/
function lastUserId() {
    if ($m.getSession("userId") !== "undefined" || $m.getSession("userId") !== "null" || $m.getSession("userId") !== "") {
        $m.setStorage("lastUserId", $m.getSession("userId"));
    }
}

/*发送数据*/
function log(sn, st, ot, ii, di) {
    //sys_name | service_type | oper_type | item_id | desc_info

    setTimeout(function () { //延迟，确保数据初始化完毕
        var getTime = $m.getTime();
        var mercuryLog = '?log={"header":{"service":"sellingservice","table":"useraction","count":"1"},"content":[{"base_info":{"msg_id":"' +
            ((($m.getSession("userId") === "undefined" || $m.getSession("userId") === "null") ? "" : $m.getSession("userId")) || $m.getStorage("tempUserId") || $m.getCookie("tempUserId")) + getTime + '","real_userid":"' + getUserId() + //((($m.getSession("userId") === "undefined" || $m.getSession("userId") === "null") ? "" : $m.getSession("userId")) || $m.getStorage("lastUserId") || "")
            '","sys_name":"' + (sn || "") + '","service_type":"' + (st || "") + '","oper_type":"' + (ot || "") + '","access_time":"' +
            (getTime.toString() || "") + '","session_id":"' + ($m.getCookie("tempSessionId") || "") +
            '","item_id":"' + (ii || "") + '","user_id":"' + ($m.getStorage("tempUserId") || $m.getCookie("tempUserId") || "") + '","desc_info":"' + browserinfo() + (di || "") +
            '"},"add_info":{"location":"' + (getPos() || "") + '","appid":"' + ($m.getSession("appId") || "") + '","ip":"' + (returnCitySN.cip || "") + '"}}]}';

        $m.imgUrl($m.url + mercuryLog);

    }, 1000);
}

function lognews(sn, st, ot, ii, di) {
    //sys_name | service_type | oper_type | item_id | desc_info

    setTimeout(function () {    //延迟，确保数据初始化完毕
        var getTime = $m.getTime();
        var mercuryLog = '?log={"header":{"service":"sellingservice","table":"newsaction","count":"1"},"content":[{"base_info":{"msg_id":"' +
            ((($m.getSession("userId") === "undefined" || $m.getSession("userId") === "null") ? "" : $m.getSession("userId")) || $m.getStorage("tempUserId") || $m.getCookie("tempUserId")) + getTime + '","real_userid":"' + getUserId() + //(((getUserId("userId") === "undefined" || $m.getSession("userId") === "null") ? "" : $m.getSession("userId")) || "")
            '","sys_name":"' + (sn || "") + '","service_type":"' + (st || "") + '","oper_type":"' + (ot || "") + '","access_time":"' +
            (getTime.toString() || "") + '","session_id":"' + ($m.getCookie("tempSessionId") || "") +
            '","item_id":"' + (ii || "") + '","user_id":"' + ($m.getStorage("tempUserId") || $m.getCookie("tempUserId") || "") + '","desc_info":"' + browserinfo() + (di || "") +
            '"},"add_info":{"location":"' + (getPos() || "") + '","appid":"' + ($m.getSession("appId") || "") + '","ip":"' + (returnCitySN.cip || "") + '"}}]}';

        $m.imgUrl($m.url + mercuryLog);
    }, 1000);
}

$m.init();


//--------------------------------------------------------CookieContextDTO.js----------------------------------------------------------
//从Url中获取上下文并Cookie
function saveContextDTOByUrl() {
    var userId = getQueryString('user') || getQueryString('userId');
    var sessionId = getQueryString('sessionId');
    var changeOrg = getQueryString('changeOrg') || Guid.NewGuid().ToString();

    if (window.location.href.toLowerCase().indexOf("islogin") > 0) {
        var isLogin = getQueryString('islogin');
        if (isLogin == "0") {
            delContextDTO();
            return;
        }
    }

    //var curChangeOrg = getQueryString('curChangeOrg');
    if (IsNullOrEmpty(userId) && IsNullOrEmpty(sessionId) && IsNullOrEmpty(changeOrg)) {
        //保存
        saveContextDTO(userId, sessionId, changeOrg);
    }
}
//登录回调函数
function LoginCallBack() {
    saveContextDTOByUrl();
}

//退出回调函数
function LoginOutCallBack() {
    delContextDTO();
}

//创建对象
function createContextDto() {
    var contextDto = {};
    contextDto.userId = null;
    contextDto.sessionId = null;
    contextDto.changeOrg = null;
    return contextDto;
}

//获取上下文
function getContextDTO() {
    var returnvalue = get_cookie("CookieContextDTO");
    var contextDto;
    try {
        contextDto = JSON.parse(returnvalue);
    } catch (e) {
        contextDto = createContextDto();
    }
    return contextDto;
}

//删除cookies
function delContextDTO() {
    del_Cookie("CookieContextDTO");
}

//删除cookies
function del_Cookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = get_cookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + ";path=/;domain=" + document.domain;
}

//设置上下文

function saveContextDTO(userId, sessionId, changeOrg) {
    var obj = {};
    obj.userId = userId;
    obj.sessionId = sessionId;
    obj.changeOrg = changeOrg == "00000000-0000-0000-0000-000000000000" ? Guid.NewGuid().ToString() : changeOrg;
    //alert("save+userid=" + userId + ",sessionid=" + sessionId + ",changeOrg=" + changeOrg)
    //obj.curChangeOrg = curChangeOrg;
    SetCookie("CookieContextDTO", JSON.stringify(obj));
}

//记录Cookie,设置名称为name,值为value的Cookie
function SetCookie(name, value) {
    var exp = new Date();
    //8小时
    exp.setTime(exp.getTime() + 8 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + ";path=/;domain=" + document.domain;
}

//获取Cookie
function get_cookie(Name) {
    var search = Name + "="
    var returnvalue = "";
    if (document.cookie.length > 0) {
        offset = document.cookie.indexOf(search)
        if (offset != -1) {
            offset += search.length
            end = document.cookie.indexOf(";", offset);
            if (end == -1)
                end = document.cookie.length;
            returnvalue = (document.cookie.substring(offset, end))
        }
    }
    return unescape(returnvalue);
}

//获取Url中的参数
function getQueryString(a) {
    var b = new RegExp("(^|&)" + a + "=([^&]*)(&|$)", "i");
    var c = window.location.search.substr(1).match(b);
    return null != c ? unescape(c[2]) : null
}

//是否为空
function IsNullOrEmpty(val) {
    if (val == null || val == "" || val == "null" || val == "undefind") {
        return false;
    }
    return true;
}

function isLogin() {
    var dto = getContextDTO();
    if (!dto || !IsNullOrEmpty(dto.userId) || !IsNullOrEmpty(dto.sessionId) || !IsNullOrEmpty(dto.changeOrg))
        return false;
    return true;
}
function getUserId() {
    var dto = getContextDTO();
    if (dto && IsNullOrEmpty(dto.userId))
        return dto.userId;
    return null;
}
function getSessionId() {
    var dto = getContextDTO();
    if (dto && IsNullOrEmpty(dto.sessionId))
        return dto.sessionId;
    return null;

}
function getChangeOrg() {
    var dto = getContextDTO();
    if (dto && IsNullOrEmpty(dto.changeOrg))
        return dto.changeOrg;
    return null;
}