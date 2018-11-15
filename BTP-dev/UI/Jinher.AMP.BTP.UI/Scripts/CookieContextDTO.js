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
    //31天
    exp.setTime(exp.getTime() + 31 * 24 * 60 * 60 * 1000);
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
