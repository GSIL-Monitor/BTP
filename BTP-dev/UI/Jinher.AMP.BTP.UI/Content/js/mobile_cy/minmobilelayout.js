
$(function () {
    hideHeader();
    /*记录上下文信息*/
    saveContextDTOByUrl();
    if (checkIsNullOrEmpty(getQueryString("shopId"))) {
        sessionStorage.appId = getQueryString("shopId");
    }
});
function isWeiXin() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    return /micromessenger/.test(userAgent);
}
//目前样式有问题
function hideHeader() {
    if (isWeiXin()) {
        $(".page").find(".process-header").addClass("hide");
    }
}
function getCYDomain() {
    return "http://" + window.location.host;
}

//获取环境，dev或者test或者本地,正式环境
function getPreCYDomain() {
    var _host = window.location.host;
    if (_host.indexOf("dev") == 0)
        return "dev";
    if (_host.indexOf("test") == 0)
        return "test";
    if (_host.indexOf("local") == 0)
        return "dev";
    return "";
}
String.prototype.replaceAll = function (AFindText, ARepText) {
    var raRegExp = new RegExp(AFindText, "g");
    return this.replace(raRegExp, ARepText);
};
function getDataAjax2(obj) {
    var requestAsync = true;
    if (obj.async === false) {
        requestAsync = false;
    }
    if (obj.data && $.type(obj.data) != "string") {
        obj.data = JSON.stringify(obj.data);
    }
    $.ajax({
        url: obj.url,
        type: 'post',
        async: requestAsync,
        contentType: "application/json",
        data: obj.data,
        success: obj.callback,
        beforeSend: obj.beforeSend,
        error: function (e) {
            if (e.status) {
                toast("网络异常，请重试~");
                obj.error && obj.error();
            }
        },
        complete: obj.complete,
        dataType: 'json'
    })
}
function getQueryString(name) {
    var r;
    if (arguments.length > 1) {
        r = arguments[1].split('?')[1];
    } else {
        r = window.location.search.substr(1);
    }
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}
/* 设置URL参数，URL里面没次参数新增，有的话替换  如果只是给URL新增参数不建议用这个，性能低*/
String.prototype.setUrlParam = function (param, value) {
    var query = this;
    if (!checkIsNullOrEmpty(self)) return query;
    var pv = param + '=' + value;
    var newparam = query;
    var p = new RegExp("(^|)" + param + "=([^&]*)(|$)");
    if (p.test(query)) {
        var sp = query.split(param + "=");
        var firstParam = sp[0];
        var secondParam = sp[1];
        if (secondParam.indexOf("&") > -1) {
            var lastPraam = secondParam.substring(secondParam.indexOf("&"));
            newparam = firstParam + pv + lastPraam;
        } else {
            newparam = checkIsNullOrEmpty(firstParam) ? (firstParam + pv) : pv;
        }
    } else {
        newparam = query == '' ? pv : (query + '&' + pv);
    }
    return newparam;
}
function addSessionToUrl(url) {
    try {
        var contextDTO = getContextDTO();
        if (contextDTO && checkIsNullOrEmpty(contextDTO.userId)) {
            url += "&userId=" + contextDTO.userId + "&sessionId=" + contextDTO.sessionId + "&changeOrg=" + contextDTO.changeOrg
        }
        else {
            url += "&userId=" + getQueryString('userId') + "&sessionId=" + getQueryString('sessionId') + "&changeOrg=" + getQueryString('changeOrg')
        }
    }
    catch (e) {
    }
    return url;
}
//JS验证 不是有效的值
function checkIsNullOrEmpty(obj) {
    if (obj != undefined && obj != "" && obj != "null" && obj != null && obj != "undefined" && obj) {
        return true;
    }
    else {
        return false;
    }
}
Array.prototype.find = function (call) {
    var self = this;
    var items = new Array();
    for (var i = 0; i < self.length; i++) {
        if (call(self[i])) items.push(self[i]);
    }
    return items;
};
