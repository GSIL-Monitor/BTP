/*
* date:2014-05-14
* content:js常用方法。
* by:邓贞才
*/
//一些基础方法。
var CommLib =
    {
        //返回url是所有参数
        getUrlArg: function () {
            //Url的参数值上不能存在? & =(存在则要编码才行)
            var search = window.location.search,
                index = search.indexOf("?"),
                data = {};
            if (index != -1) {
                var list = search.substring(index + 1, search.length).split("&"),
                    item;
                for (var i = 0; i < list.length; i++) {
                    item = list[i].split("=");
                    data[item[0]] = item[1];
                }
            }
            return data;
        },
        js: function (jsFileName) {
            var argData = {};
            var rName = new RegExp(jsFileName + "(\\?(.*))?$", "i");
            var jss = document.getElementsByTagName('script');
            for (var i = 0; i < jss.length; i++) {
                var j = jss[i];
                if (j.src && j.src.match(rName)) {
                    var oo = j.src.match(rName)[2];
                    if (oo && (t = oo.match(/([^&=]+)=([^=&]+)/g))) {
                        for (var l = 0; l < t.length; l++) {
                            r = t[l];
                            var tt = r.match(/([^&=]+)=([^=&]+)/);
                            if (tt) {
                                argData[tt[1]] = tt[2];
                            }
                        }
                    }
                }
            }
            return argData;
        }
        ,  //对象转化为str
        ObjToString: function (o) {
            var r = [];
            var otype = "undefined";
            if (typeof o == "string") {
                return "\"" + o.replace(/(['\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
            }
            if (typeof o == "undefined") {
                return otype;
            }
            if (typeof o == "object") {
                if (o === null) return "null";
                else if (!o.sort) {
                    for (var i in o) {
                        r.push('\"' + i + '\":' + this.ObjToString(o[i]));
                    }
                    r = "{" + r.join() + "}";
                } else {
                    for (var i = 0; i < o.length; i++) {
                        r.push(this.ObjToString(o[i]));
                    }
                    r = "[" + r.join() + "]";
                }
                return r;
            } 
            return o.toString();
        },
        //对象转化为str(特殊处理：给数字类型的值加上"").
        ObjToStringWithQuot: function (o) {
            var r = [];
            var otype = "undefined";
            if (typeof o == "string") {
                return "\"" + o.replace(/(['\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
            }
            if (typeof o == "undefined") {
                return otype;
            }
            if (typeof o == "object") {
                if (o === null) return "null";
                else if (!o.sort) {
                    for (var i in o) {
                        r.push('\"' + i + '\":' + this.ObjToStringWithQuot(o[i]));
                    }
                    r = "{" + r.join() + "}";
                } else {
                    for (var i = 0; i < o.length; i++) {
                        r.push(this.ObjToStringWithQuot(o[i]));
                    }
                    r = "[" + r.join() + "]";
                }
                return r;
            }
            return "\"" + o.toString() + "\"";
        },
        //获取对象的类型的字符串。
        getType: function (o) {
            var _toS = Object.prototype.toString,
                _types = {
                    'undefined': 'undefined',
                    'number': 'number',
                    'boolean': 'boolean',
                    'string': 'string',
                    '[object Function]': 'function',
                    '[object RegExp]': 'regexp',
                    '[object Array]': 'array',
                    '[object Date]': 'date',
                    '[object Error]': 'error'
                };
            return _types[typeof o] || _types[_toS.call(o)] || (o ? 'object' : 'null');
        },
        //#region clone：拷贝一份数据对象
        clone: function (oglObject) {

            var bucket;
            //if (oglObject instanceof Array)
            if (this.getType(oglObject) == "array") {
                bucket = [];
                var i = oglObject.length;
                while (i--) {
                    bucket[i] = this.clone(oglObject[i]);
                }
                return bucket;
            }
            //else if (oglObject instanceof Function)
            else if (this.getType(oglObject) == "function") {
                return oglObject;
            }
            //else if (oglObject instanceof Object)
            else if (this.getType(oglObject) == "object") {
                bucket = {};
                for (var k in oglObject) {
                    bucket[k] = this.clone(oglObject[k]);
                }
                return bucket;
            }
            else {
                return oglObject;
            }
        }
        //获取文件扩展名，filePath文件网络地址。
        , GetFileExtension: function (filePath) {

            var index = filePath.indexOf("?");

            if (index != -1) {
                filePath = filePath.substring(0, index);
            }

            var ext = RegExp("\.[^\.]+$").exec(filePath) || {};
            return ext;
        }
        //将格式为[{"year":"2011"},{"month":"12"}]的参数转化为系统设定的键值对的格式
        , FormatParam: function (arr) {
            if (!arr || arr.length == 0) { arr = new Array(); }
            else {
                var paraArray = new Array();
                for (var i = 0; i < arr.length; i++) {
                    for (var key in arr[i]) {
                        paraArray.push({ "Key": key, "Value": arr[i][key] });
                    }
                }
            }
            return paraArray;
        }
        , //按名称获取url中的参数
        getUrlParamByName: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var txt = decodeURI(window.location.search);
            var r = txt.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;

        }
        //将对象序列化为url参数。
        , formatObjectToUrlParams: function (obj) {
            var urlParams = "";
            for (var pname in obj) {
                //只组织当前类型的属性，继承的属性不管。复杂类型只组织第一层。
                if ((!obj.hasOwnProperty(pname)) ||
                    (typeof obj[pname] != 'number' && typeof obj[pname] != 'string' && typeof obj[pname] != 'boolean')) {
                    continue;
                }
                urlParams += "&" + pname + "=" + obj[pname];
            }
            //除去第一个&;
            urlParams = urlParams.substr(1);
            return urlParams;
        }

        //获取script的src后的参数。
       , getScriptParamByName: function (scriptName, paramName) {
           var scripts = document.getElementsByTagName("script");
           for (var i = 0; i < scripts.length; i++) {
               var src = scripts[i].src;
               if (src.indexOf(scriptName) > -1 && src.indexOf("?") > -1) {
                   var index = src.indexOf("?");
                   var list = src.substring(index + 1, src.length).split("&")
                   for (var i = 0; i < list.length; i++) {
                       var item = list[i].split("=");
                       if (item[0] == paramName) {
                           return item[1];
                       }
                   }

               }
           }
       }
        //获取操作系统位数。
        , getWinDigit: function () {
            var agent = navigator.userAgent.toLowerCase();
            if (agent.indexOf("win64") >= 0 || agent.indexOf("wow64") >= 0) {
                return "x64";
            }
            return navigator.cpuClass;
        }

        //按类名查找元素，参考http://www.cnblogs.com/webmoon/p/3288556.html
        , getElementsByClassName: function (oParent, sClass) {
            if (oParent.getElementsByClassName) {
                return oParent.getElementsByClassName(sClass);
            } else {
                var aRes = [];
                var re = new RegExp('(^|\\s)' + sClass + '($|\\s)', 'i');
                var aEle = oParent.getElementsByTagName('*');
                for (var i = 0; i < aEle.length; i++) {
                    if (re.test(aEle[i].className)) {
                        aRes.push(aEle[i]);
                    }
                }
                return aRes;
            }
        }
        //返回4位随机数值
        , getRandom: function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        },
        //返回32为随机数值
        getGuid: function () {
            return (this.getRandom() + this.getRandom() + "-" + this.getRandom() + "-" + this.getRandom() + "-" + this.getRandom() + "-" + this.getRandom() + this.getRandom() + this.getRandom());
        }

        //产生不重复的随机数组。
        , RandomNumbers: function (min, max) {
            var ret = new Array();

            max += 1;
            var step = max - min;
            if (step <= 0) {
                return ret;
            }
            while (ret.length < step) {
                var rn = Math.floor(min + Math.random() * (max - min));
                if (ret.indexOf(rn) == -1) {
                    ret.push(rn);
                }
            }
            return ret;
        }
    }




var Encoder = (function () {
    //下面是64个基本的编码
    var base64EncodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    var base64DecodeChars = new Array(
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
    52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
    -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
    15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
    -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
    41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1);
    //编码的方法
    function base64encode(str) {
        var out, i, len;
        var c1, c2, c3;
        len = str.length;
        i = 0;
        out = "";
        while (i < len) {
            c1 = str.charCodeAt(i++) & 0xff;
            if (i == len) {
                out += base64EncodeChars.charAt(c1 >> 2);
                out += base64EncodeChars.charAt((c1 & 0x3) << 4);
                out += "==";
                break;
            }
            c2 = str.charCodeAt(i++);
            if (i == len) {
                out += base64EncodeChars.charAt(c1 >> 2);
                out += base64EncodeChars.charAt(((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4));
                out += base64EncodeChars.charAt((c2 & 0xF) << 2);
                out += "=";
                break;
            }
            c3 = str.charCodeAt(i++);
            out += base64EncodeChars.charAt(c1 >> 2);
            out += base64EncodeChars.charAt(((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4));
            out += base64EncodeChars.charAt(((c2 & 0xF) << 2) | ((c3 & 0xC0) >> 6));
            out += base64EncodeChars.charAt(c3 & 0x3F);
        }
        return out;
    }
    //解码的方法
    function base64decode(str) {
        var c1, c2, c3, c4;
        var i, len, out;
        len = str.length;
        i = 0;
        out = "";
        while (i < len) {

            do {
                c1 = base64DecodeChars[str.charCodeAt(i++) & 0xff];
            } while (i < len && c1 == -1);
            if (c1 == -1)
                break;

            do {
                c2 = base64DecodeChars[str.charCodeAt(i++) & 0xff];
            } while (i < len && c2 == -1);
            if (c2 == -1)
                break;
            out += String.fromCharCode((c1 << 2) | ((c2 & 0x30) >> 4));

            do {
                c3 = str.charCodeAt(i++) & 0xff;
                if (c3 == 61)
                    return out;
                c3 = base64DecodeChars[c3];
            } while (i < len && c3 == -1);
            if (c3 == -1)
                break;
            out += String.fromCharCode(((c2 & 0XF) << 4) | ((c3 & 0x3C) >> 2));

            do {
                c4 = str.charCodeAt(i++) & 0xff;
                if (c4 == 61)
                    return out;
                c4 = base64DecodeChars[c4];
            } while (i < len && c4 == -1);
            if (c4 == -1)
                break;
            out += String.fromCharCode(((c3 & 0x03) << 6) | c4);
        }
        return out;
    }
    function utf16to8(str) {
        var out, i, len, c;
        out = "";
        len = str.length;
        for (i = 0; i < len; i++) {
            c = str.charCodeAt(i);
            if ((c >= 0x0001) && (c <= 0x007F)) {
                out += str.charAt(i);
            } else if (c > 0x07FF) {
                out += String.fromCharCode(0xE0 | ((c >> 12) & 0x0F));
                out += String.fromCharCode(0x80 | ((c >> 6) & 0x3F));
                out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
            } else {
                out += String.fromCharCode(0xC0 | ((c >> 6) & 0x1F));
                out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
            }
        }
        return out;
    }
    function utf8to16(str) {
        var out, i, len, c;
        var char2, char3;
        out = "";
        len = str.length;
        i = 0;
        while (i < len) {
            c = str.charCodeAt(i++);
            switch (c >> 4) {
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                    // 0xxxxxxx
                    out += str.charAt(i - 1);
                    break;
                case 12: case 13:
                    // 110x xxxx   10xx xxxx
                    char2 = str.charCodeAt(i++);
                    out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                    break;
                case 14:
                    // 1110 xxxx  10xx xxxx  10xx xxxx
                    char2 = str.charCodeAt(i++);
                    char3 = str.charCodeAt(i++);
                    out += String.fromCharCode(((c & 0x0F) << 12) |
                               ((char2 & 0x3F) << 6) |
                               ((char3 & 0x3F) << 0));
                    break;
            }
        }
        return out;
    }


    var encoder = {};
    encoder.base64encode = base64encode;
    encoder.base64decode = base64decode;
    encoder.utf16to8 = utf16to8;
    encoder.utf8to16 = utf8to16;
    encoder.encode = function (str) {
        return base64encode(utf16to8(str));
    }
    encoder.decode = function (str) {
        return utf8to16(base64decode(str));
    }
    return encoder;

} ())


/*
*字符串相关扩展
*/

////string.format.
////两种调用方式
//var template1 = "我是{0}，今年{1}了";
//var template2 = "我是{name}，今年{age}了";
//var result1 = template1.format("loogn", 22);
//var result2 = template2.format({ name: "loogn", age: 22 });
////两个结果都是"我是loogn，今年22了"
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}



String.prototype.trim = function (args) {
    if (this == undefined || this == "") {
        return "";
    }
    //除去空格。
    if (args == undefined || args[0] == undefined || args[0] == "") {
        return this.replace(/(^\s+)|(\s+$)/g, "");
    }
    else {
        //除去正则匹配的，字符串前后的内容。
        var re = "/(^" + args[0] + "+)|(" + args[0] + "+$)/g";
        re = eval(re);
        return this.replace(re, "");
    }

}

//以一特定规律组成的字符串里是否包含val,splitLim默认为,或;或，或；或、
String.prototype.isExistInSplit = function (val, splitLim) {
    if (splitLim == undefined) {
        splitLim = /,|;|，|；|、/gi;
    }
    var arr = this.split(splitLim);
    var ind = arr.indexOf(val);
    return ind > -1 ? true : false;
}



/*
*
*数组相关扩展。
*/
/*ie8下不支持indexof、forEach、filter,自己扩展。*/
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;

        var from = Number(arguments[1]) || 0;
        from = (from < 0) ? Math.ceil(from) : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (from in this &&
                this[from] === elt)
                return from;
        }
        return -1;
    };
}
if (!Array.prototype.forEach) {
    Array.prototype.forEach = function (fn, thisObj) {
        var scope = thisObj || window;
        for (var i = 0, j = this.length; i < j; ++i) {
            fn.call(scope, this[i], i, this);
        }
    };
}
if (!Array.prototype.filter) {
    Array.prototype.filter = function (fn, thisObj) {
        var scope = thisObj || window;
        var a = [];
        for (var i = 0, j = this.length; i < j; ++i) {
            if (!fn.call(scope, this[i], i, this)) {
                continue;
            }
            a.push(this[i]);
        }
        return a;
    };
}

if (!Array.prototype.map) {
    Array.prototype.map = function (fn, thisObj) {
        var scope = thisObj || window;
        var a = [];
        for (var i = 0, j = this.length; i < j; ++i) {
            var r = fn.call(scope, this[i], i, this);
            if (!r) {
                continue;
            }
            a.push(r);
        }
        return a;
    };
}





//返回数组中所有某个属性（propertyName）的属性值在min和max之间的元素。
Array.prototype.PropertyValueBetween = function (propertyName, min, max) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称不能为空！");
    }
    if (parseFloat(min) > parseFloat(max)) {
        throw ("最大值不能小于等于最小值！");
    }

    //返回结果
    var resultArr = new Array();

    var self = this;
    for (var i = 0; i < self.length; i++) {
        var obj = self[i];
        if (typeof (obj[propertyName]) != "function") {
            if (min == max) {
                if (parseFloat(obj[propertyName]) == min) {
                    resultArr.push(obj);
                }
            }
            else if (parseFloat(obj[propertyName]) >= min && parseFloat(obj[propertyName]) <= max) {
                resultArr.push(obj);
            }
        }

    }

    return resultArr;
}

//数组中元素的名为propertyName的属性的包含value则返回。
Array.prototype.PropertyValueLike = function (propertyName, value) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称不能为空！");
    }
    if (value == null || value == undefined || value.length == 0) {
        throw ("属性值不能为空！");
    }

    //返回结果
    var resultArr = new Array();

    var self = this;
    for (var i = 0; i < self.length; i++) {
        var obj = self[i];
        if (typeof (obj[propertyName]) != "string") {
            break;
        }
        if (obj[propertyName].indexOf(value) > -1) {
            resultArr.push(obj);
        }

    }
    return resultArr;
}

//数组中元素的名为propertyName的属性的包含value则返回。
Array.prototype.PropertyValueEqual = function (propertyName, value) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称" + propertyName + "不能为空！");
    }
    if (value == null || value == undefined || value.length == 0) {
        throw ("属性值" + propertyName + "不能为空！");
    }

    //返回结果
    var resultArr = new Array();

    var self = this;
    for (var i = 0; i < self.length; i++) {
        var obj = self[i];
        //ie8下最后一个元素为undefined
        if (obj == undefined) {
            continue;
        }
        if (typeof (obj[propertyName]) == "function") {
            break;
        }
        if (obj[propertyName] == value) {
            resultArr.push(obj);
        }
    }
    return resultArr;
}

//数组中是否存在属性名为propertyName属性值为value的元素。
Array.prototype.ElementExist = function (propertyName, value) {
    var self = this;
    var arr = self.PropertyValueEqual(propertyName, value);
    if (arr.length > 0) {
        return true;
    }
    else {
        return false;
    }
}
//按属性在数组查找唯一元素，如果没找到或找到多个都返回undefined。
Array.prototype.GetOnlyElement = function (propertyName, value) {
    var self = this;
    var arr = self.PropertyValueEqual(propertyName, value);
    if (arr.length == 1) {
        return arr[0];
    }
    else {
        return;
    }
}


//按属性在数组查找符合条件的元素，并返回第一个，如果没找到或找到多个都返回undefined。
Array.prototype.GetFirstElement = function (propertyName, value) {
    var self = this;
    var arr = self.PropertyValueEqual(propertyName, value);
    if (arr.length > 0) {
        return arr[0];
    }
    else {
        return;
    }
}


//数组中元素的名为propertyName的属性的包含value则返回。
Array.prototype.ElementReplace = function (propertyName, value, newobj) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称不能为空！");
    }
    if (value == null || value == undefined || value.length == 0) {
        throw ("属性值不能为空！");
    }

    var self = this;
    for (var i = 0; i < self.length; i++) {
        var obj = self[i];
        //ie8下最后一个元素为undefined
        if (obj == undefined) {
            continue;
        }
        if (typeof (obj[propertyName]) == "function") {
            break;
        }
        if (obj[propertyName] == value) {
            self[i] = newobj;
            //此处可用$.extend合并 self[i] 和 newobj，不过要依赖于juqery.
        }
    }
}

//按属性找出某实体在数组中位置。
Array.prototype.myIndexOf = function (propertyName, value) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称不能为空！");
    }
    if (value == null || value == undefined || value.length == 0) {
        throw ("属性值不能为空！");
    }

    var self = this;
    var ind = -1;
    for (var i = 0; i < self.length; i++) {
        var obj = self[i];
        //ie8下最后一个元素为undefined
        if (obj == undefined) {
            continue;
        }
        if (typeof (obj[propertyName]) == "function") {
            break;
        }
        if (obj[propertyName] == value) {
            ind = i;
            break;
        }
    }
    return ind;
}
//按某个属性删除元素。
Array.prototype.RemoveElement = function (propertyName, value) {
    //参数正确性。
    if (propertyName == null || propertyName.length == 0) {
        throw ("属性名称不能为空！");
    }
    if (value == null || value == undefined || value.length == 0) {
        throw ("属性值不能为空！");
    }

    var self = this;
    var ind = self.myIndexOf(propertyName, value);
    if (ind > -1) {
        return self.splice(ind, 1);
    }
    return self;
}


Number.prototype.toFixed = function (d) {
    var s = this + "";
    if (!d) d = 0;
    if (s.indexOf(".") == -1) s += ".";
    s += new Array(d + 1).join("0");
    if (new RegExp("^(-|\\+)?(\\d+(\\.\\d{0," + (d + 1) + "})?)\\d*$").test(s)) {
        var s = "0" + RegExp.$2, pm = RegExp.$1, a = RegExp.$3.length, b = true;
        if (a == d + 2) {
            a = s.match(/\d/g);
            if (parseInt(a[a.length - 1]) > 4) {
                for (var i = a.length - 2; i >= 0; i--) {
                    a[i] = parseInt(a[i]) + 1;
                    if (a[i] == 10) {
                        a[i] = 0;
                        b = i != 1;
                    } else break;
                }
            }
            s = a.join("").replace(new RegExp("(\\d+)(\\d{" + d + "})\\d$"), "$1.$2");

        } if (b) s = s.substr(1);
        return (pm + s).replace(/\.$/, "");
    } return this + "";

};





var ajaxLoadingSingle = (function () {
    function initLoading() {
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length > 0) {
            return;
        }
        //蒙版
        blind = $('<div></div>');
        //蒙版相关css
        blind.css({
            'position': 'fixed',
            'z-index': '10000',
            'opacity': 0.2,
            'backgroundColor': '#ccc',
            'height': '100%',
            'width': '100%',
            'top': 0,
            'left': 0
        });
        //蒙版ID值
        blind.attr('id', 'ajaxLoadBlind');

        //ajaxLoading盒子ID对象
        var loading = $('#ajaxLoading_img');
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_img');
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });
        loading.append('<img src="/Content/images/ajax-loader.gif" />');
        //blind.css({ 'position': 'relative' });
        blind.append(loading);
        $('body').append(blind);
    }

    function show() {
        initLoading();
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length == 0) {
            return;
        }
        blind.show();
    }
    function hide() {
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length == 0) {
            return;
        }
        blind.hide();
    }

    var loadingSingle = new Object();
    loadingSingle.show = show;
    loadingSingle.hide = hide;
    return loadingSingle;
}())




//设置.sessionStorage值
function setSessionStorage(key, p_key, value) {
    //临时对象
    var data;
    //判断是否有当前缓存值
    if (!sessionStorage[key]) {
        //没有则生成一个空对象
        sessionStorage[key] = '{}';
        //将data赋值为空对象
        data = {};
    } else {
        //有则将缓存的值赋值为data
        data = JSON.parse(sessionStorage[key]);
    }

    //重新赋值
    data[p_key] = value;
    //更新缓存
    sessionStorage[key] = JSON.stringify(data);
}




//封装ajax方法 get
function getDataAjax(obj) {
    var requestAsync = true;
    if (obj.async === false) {
        requestAsync = false;
    }
    return $.ajax({
        url: obj.url,
        type: 'get',
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
