String.prototype.replaceAll = function (AFindText, ARepText) {
    var raRegExp = new RegExp(AFindText, "g");
    return this.replace(raRegExp, ARepText);
};
Array.prototype.find = function (call) {
    var self = this;
    var items = new Array();
    for (var i = 0; i < self.length; i++) {
        if (call(self[i])) items.push(self[i]);
    }
    return items;
}
Array.prototype.each = function (call) {
    var self = this;
    var items = new Array();
    for (var i = 0; i < self.length; i++) {
        call.apply(self[i]);
    }
}
//递增订单号
function increaseExpressNo(one, two) {
    one = one.split('').reverse().join('');
    two = two.split('').reverse().join('');
    var len1 = one.length;
    var len2 = two.length;
    var len = len1 > len2 ? len1 : len2;
    var c = 0;
    var result = [];
    for (var i = 0; i < len; i++) {
        var t = 0;
        var a = i < len1 ? one[i] : "0";
        var b = i < len2 ? two[i] : "0";
        if (isNumber(a) && isNumber(b)) {
            t = (parseInt(a) + parseInt(b) + parseInt(c)).toString();
            result.push(t.length > 1 ? t[1] : t[0]);
            c = t.length > 1 ? t[0] : "0";
        }
        else {
            result.push(isNumber(a) ? b : a);
        }
    }
    return result.reverse().join("");
}

function isNumber(value) {
    var patrn = /^(-)?\d+(\.\d+)?$/;
    if (patrn.exec(value) == null || value == "") {
        return false
    } else {
        return true
    }
}
function getQueryString(name, str) {
    var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}