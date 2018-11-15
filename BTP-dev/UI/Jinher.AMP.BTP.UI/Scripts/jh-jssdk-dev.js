/**
* jh-jssdk是用来封装金和原生接口的组件
* @class jh-jssdk
* */
//base64加密，解密
var b64 = function () {
    var utfLibName = "utf";
    var b64char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    var b64encTable = b64char.split("");
    var b64decTable = [];
    for (var i = 0; i < b64char.length; i++) b64decTable[b64char.charAt(i)] = i;

    this.encode = function (_dat, _strMode) {
        return encoder(_strMode ? unpackUTF8(_dat) : unpackChar(_dat));
    }

    var encoder = function (_ary) {
        var md = _ary.length % 3;
        var b64 = "";
        var i, tmp = 0;

        if (md) for (i = 3 - md; i > 0; i--) _ary[_ary.length] = 0;

        for (i = 0; i < _ary.length; i += 3) {
            tmp = (_ary[i] << 16) | (_ary[i + 1] << 8) | _ary[i + 2];
            b64 += b64encTable[(tmp >>> 18) & 0x3f]
            b64 += b64encTable[(tmp >>> 12) & 0x3f]
            b64 += b64encTable[(tmp >>> 6) & 0x3f]
            b64 += b64encTable[tmp & 0x3f];
        }

        if (md) // 3の倍数にパディングした 0x0 分 = に置き換え
        {
            md = 3 - md;
            b64 = b64.substr(0, b64.length - md);
            while (md--) b64 += "=";
        }

        return b64;
    }

    this.decode = function (_b64, _strMode) {
        var tmp = decoder(_b64);
        return _strMode ? packUTF8(tmp) : packChar(tmp);
    }

    var decoder = function (_b64) {
        _b64 = _b64.replace(/[^A-Za-z0-9\+\/]/g, "");
        var md = _b64.length % 4;
        var j, i, tmp;
        var dat = [];

        if (md) for (i = 0; i < 4 - md; i++) _b64 += "A";

        for (j = i = 0; i < _b64.length; i += 4, j += 3) {
            tmp = (b64decTable[_b64.charAt(i)] << 18)
                | (b64decTable[_b64.charAt(i + 1)] << 12)
                | (b64decTable[_b64.charAt(i + 2)] << 6)
                | b64decTable[_b64.charAt(i + 3)];
            dat[j] = tmp >>> 16;
            dat[j + 1] = (tmp >>> 8) & 0xff;
            dat[j + 2] = tmp & 0xff;
        }
        if (md) dat.length -= [0, 0, 2, 1][md];

        return dat;
    }

    var packUTF8 = function (_x) {
        return utf.packUTF8(_x)
    };
    var unpackUTF8 = function (_x) {
        return utf.unpackUTF8(_x)
    };
    var packChar = function (_x) {
        return utf.packChar(_x)
    };
    var unpackChar = function (_x) {
        return utf.unpackChar(_x)
    };
};
var utf8 = function () {
    this.unpackUTF16 = function (_str) {
        var i, utf16 = [];
        for (i = 0; i < _str.length; i++) utf16[i] = _str.charCodeAt(i);
        return utf16;
    }

    this.unpackChar = function (_str) {
        var utf16 = this.unpackUTF16(_str);
        var i, n, tmp = [];
        for (n = i = 0; i < utf16.length; i++) {
            if (utf16[i] <= 0xff) tmp[n++] = utf16[i];
            else {
                tmp[n++] = utf16[i] >> 8;
                tmp[n++] = utf16[i] & 0xff;
            }
        }
        return tmp;
    }

    this.packChar =
        this.packUTF16 = function (_utf16) {
            var i, str = "";
            for (i in _utf16) str += String.fromCharCode(_utf16[i]);
            return str;
        }

    this.unpackUTF8 = function (_str) {
        return this.toUTF8(this.unpackUTF16(_str));
    }

    this.packUTF8 = function (_utf8) {
        return this.packUTF16(this.toUTF16(_utf8));
    }

    this.toUTF8 = function (_utf16) {
        var utf8 = [];
        var idx = 0;
        var i, j, c;
        for (i = 0; i < _utf16.length; i++) {
            c = _utf16[i];
            if (c <= 0x7f) utf8[idx++] = c;
            else if (c <= 0x7ff) {
                utf8[idx++] = 0xc0 | (c >>> 6);
                utf8[idx++] = 0x80 | (c & 0x3f);
            }
            else if (c <= 0xffff) {
                utf8[idx++] = 0xe0 | (c >>> 12);
                utf8[idx++] = 0x80 | ((c >>> 6) & 0x3f);
                utf8[idx++] = 0x80 | (c & 0x3f);
            }
            else {
                j = 4;
                while (c >> (6 * j)) j++;
                utf8[idx++] = ((0xff00 >>> j) & 0xff) | (c >>> (6 * --j));
                while (j--)
                    utf8[idx++] = 0x80 | ((c >>> (6 * j)) & 0x3f);
            }
        }
        return utf8;
    }

    this.toUTF16 = function (_utf8) {
        var utf16 = [];
        var idx = 0;
        var i, s;
        for (i = 0; i < _utf8.length; i++, idx++) {
            if (_utf8[i] <= 0x7f) utf16[idx] = _utf8[i];
            else {
                if ((_utf8[i] >> 5) == 0x6) {
                    utf16[idx] = ((_utf8[i] & 0x1f) << 6)
                        | (_utf8[++i] & 0x3f);
                }
                else if ((_utf8[i] >> 4) == 0xe) {
                    utf16[idx] = ((_utf8[i] & 0xf) << 12)
                        | ((_utf8[++i] & 0x3f) << 6)
                        | (_utf8[++i] & 0x3f);
                }
                else {
                    s = 1;
                    while (_utf8[i] & (0x20 >>> s)) s++;
                    utf16[idx] = _utf8[i] & (0x1f >>> s);
                    while (s-- >= 0) utf16[idx] = (utf16[idx] << 6) ^ (_utf8[++i] & 0x3f);
                }
            }
        }
        return utf16;
    }

    this.URLencode = function (_str) {
        return _str.replace(/([^a-zA-Z0-9_\-\.])/g, function (_tmp, _c) {
            if (_c == "\x20") return "+";
            var tmp = utf.toUTF8([_c.charCodeAt(0)]);
            var c = "";
            for (var i in tmp) {
                i = tmp[i].toString(16);
                if (i.length == 1) i = "0" + i;
                c += "%" + i;
            }
            return c;
        });
    }

    this.URLdecode = function (_dat) {
        _dat = _dat.replace(/\+/g, "\x20");
        _dat = _dat.replace(/%([a-fA-F0-9][a-fA-F0-9])/g,
            function (_tmp, _hex) {
                return String.fromCharCode(parseInt(_hex, 16))
            });
        return this.packChar(this.toUTF16(this.unpackUTF16(_dat)));
    }
};
var base64 = new b64();
var utf = new utf8();
//toast
var toast = function (msg) {
    var doc = document;
    msg = msg.toString();
    if (doc.getElementById("styleToast") == null) {
        var style = doc.createElement("style");
        style.setAttribute("id", "styleToast");
        style.innerHTML = ".toast{box-sizing:border-box;position:fixed;width:100%;left:0;bottom:60px;z-index:999;display:none;padding:10px;background-clip:padding-box;}" +
            ".toast-content{display: table;padding: 8px 10px;background-color: rgba(0,0,0,.8);" +
            "border:1px solid #fff;box-shadow: 0 0 10px #a3a3a3;margin: 0 auto;color: #fff;" +
            "border-radius: 6px;text-align: center;max-width:300px;font-size: .9rem;line-height:1.3}";
        var heads = doc.getElementsByTagName("head");
        if (heads.length) {
            heads[0].appendChild(style);
        } else {
            doc.body.appendChild(style);
        }
    }

    var toastBox = doc.createElement("div"),
        con = doc.createElement("div");
    toastBox.setAttribute("class", "toast");
    con.setAttribute("class", "toast-content");
    toastBox.appendChild(con);
    doc.body.appendChild(toastBox);

    toastBox.getElementsByTagName("div")[0].innerHTML = msg;
    toastBox.style.display = "block";
    setTimeout(function () {
        toastBox.style.display = "none";
        toastBox.parentNode.removeChild(toastBox);
    }, 2500);
};
//判断浏览器
var jh = window.jh || {};
var userAgent = navigator.userAgent.toLowerCase();
if (userAgent.indexOf('jhwebview') > -1) {

    //公共错误模块
    window.setCommonError = function (code, message, tagJson) {
        toast(message);
    };

    //音频录制
    jh.audioRecord = function (options) {
        loadURL("jhoabrowser://audioFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxLength\\\":" + options.maxLength + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://audioFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxLength\\\":" + options.maxLength + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
        window.setAudioInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }

        }
    };
    /**
    * 音频录制
    * @method jh.audioRecord
    * @param {object} option
    * @param {number} option.maxLength  录音的最大时长 ,单位为毫秒
    * @param {number} option.minLength  录音的最小时长 ,单位为毫秒
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.audioRecord({
    *          maxLength: 10000,  //录音的最大时长 ,单位为毫秒
    *          minLength: 1000,   //录音的最小时长 ,单位为毫秒
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */

    //本地音频文件选择
    jh.audioChoose = function (options) {
        loadURL("jhoabrowser://audioFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"fileInfos\\\":" + options.fileInfos + ",\\\"maxLength\\\":" + options.maxLength + ",\\\"maxNumber\\\":" + options.maxNumber == null ? 1 : options.maxNumber + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("4222", "UTF8"));
        //window.location.href = "jhoabrowser://audioFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"fileInfos\\\":" + options.fileInfos + ",\\\"maxLength\\\":" + options.maxLength + ",\\\"maxNumber\\\":" + options.maxNumber == null ? 1 : options.maxNumber + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("4222", "UTF8");
        window.setAudioInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 本地音频文件选择
    * @method jh.audioChoose
    * @param {object} option
    * @param {string} option.fileInfos  需要回显的文件列表(文件选择独有属性)
    * @param {number} option.maxLength  录音的最大时长 ,单位为毫秒
    * @param {number} option.minLength  录音的最小时长 ,单位为毫秒
    * @param {number} option.maxNumber  最大选择数量，默认为1
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *      jh.audioChoose({
    *          fileInfos: '',         //需要回显的文件列表(文件选择独有属性)
    *          maxLength: 10000,      //录音的最大时长 ,单位为毫秒
    *          minLength: 1000,       //录音的最小时长 ,单位为毫秒
    *          maxNumber: 5,          //最大选择数量，默认为1
    *          success: function (data) {
    *          }
    *      })
    * </code></pre>
    */

    //视频录制
    jh.videoRecord = function (options) {
        loadURL("jhoabrowser://videoFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxLength\\\":" + options.maxLength + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://videoFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxLength\\\":" + options.maxLength + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
        window.setVideoInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 视频录制
    * @method jh.videoRecord
    * @param {object} option
    * @param {number} option.maxLength  视频的最大时长 ,单位为毫秒
    * @param {number} option.minLength  视频的最小时长 ,单位为毫秒
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.videoRecord({
    *          maxLength: 10000,   //视频的最大时长 ,单位为毫秒
    *          minLength: 1000,    //视频的最小时长 ,单位为毫秒
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */

    //本地视频文件选择
    jh.videoChoose = function (options) {
        loadURL("jhoabrowser://videoFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"fileInfos\\\":" + options.fileInfos + ",\\\"maxLength\\\":" + options.maxLength + ",\\\"maxNumber\\\":" + options.maxNumber == null ? 1 : options.maxNumber + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("4222", "UTF8"));
        //window.location.href = "jhoabrowser://videoFunction?args=" + base64.encode("{\"businessJson\":\"{\\\"fileInfos\\\":" + options.fileInfos + ",\\\"maxLength\\\":" + options.maxLength + ",\\\"maxNumber\\\":" + options.maxNumber == null ? 1 : options.maxNumber + ",\\\"minLength\\\":" + options.minLength + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("4222", "UTF8");
        window.setVideoInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 本地视频文件选择
    * @method jh.videoChoose
    * @param {object} option
    * @param {string} option.fileInfos  需要回显的文件列表(文件选择独有属性)
    * @param {number} option.maxLength  视频的最大时长 ,单位为毫秒
    * @param {number} option.minLength  视频的最小时长 ,单位为毫秒
    * @param {number} option.maxNumber  最大选择数量，默认为1
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *      jh.videoChoose({
    *          fileInfos: '',     //需要回显的文件列表(文件选择独有属性)
    *          maxLength: 10000,  //视频的最大时长 ,单位为毫秒
    *          minLength: 1000,   //视频的最小时长 ,单位为毫秒
    *          maxNumber: 5,      //最大选择数量，默认为1
    *          success: function (data) {
    *          }
    *      })
    * </code></pre>
    */

    //日期选择
    jh.dateChoose = function (options) {
        loadURL('jhoabrowser://showDatePickfunction?args=' + base64.encode('{"businessJson":"{' + "\\\"preTime\\\":" + options.preTime + '}","businessType":' + options.businessType + '}', 'UTF8') + '&tag=' + base64.encode('12111', 'UTF8'));
        //window.location.href = 'jhoabrowser://showDatePickfunction?args=' + base64.encode('{"businessJson":"{' + "\\\"preTime\\\":" + options.preTime + '}","businessType":' + options.businessType + '}', 'UTF8') + '&tag=' + base64.encode('12111', 'UTF8');
        window.pickResult = function (resultJson, tag) {
            if (resultJson.code == 10000) {
                options.success(resultJson.businessJson);
            } else if (resultJson.message != '取消') {
                toast(resultJson.message);
            }
        }
    };
    /**
    * 日期选择
    * @method jh.dateChoose
    * @param {object} option
    * @param {number} option.preTime  默认选择时间(毫秒)，如果不需要默认选择，值为-1
    * @param {number} option.businessType  日期类型,选择年份：1000,选择月份：1001,选择日：1010,选择时间（ 精确到分）：1011,选择日、时间（ 精确到分）：10101,选择月、日：10011,选择年、月：10001,选择月、日、时间（ 精确到分）：11110,选择年、月、日、时间（ 精确到分）：100110
    * @example
    * <pre><code>
    *     jh.dateChoose({
    *          preTime: -1,            //默认选择时间(毫秒)，如果不需要默认选择，值为-1
    *          businessType: 1000,     //日期类型
    *                                  //选择年份：1000
    *                                  //选择月份：1001
    *                                  //选择日：1010
    *                                  //选择时间（ 精确到分）：1011
    *                                  //选择日、时间（ 精确到分）：10101
    *                                  //选择月、日：10011
    *                                  //选择年、月：10001
    *                                  //选择月、日、时间（ 精确到分）：11110
    *                                  //选择年、月、日、时间（ 精确到分）：100110
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */

    //用户登录
    jh.userLogin = function (options) {
        loadURL("jhoabrowser://userLoginFunction?args=" + base64.encode("{\"businessJson\":\"\",\"businessType\":" + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("1993", "UTF8"));
        //window.location.href = "jhoabrowser://userLoginFunction?args=" + base64.encode("{\"businessJson\":\"\",\"businessType\":" + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("1993", "UTF8");
        window.setUserLoginInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 用户登录
    * @method jh.userLogin
    * @param {object} option
    * @param {number} option.businessType  登录类型，获取是否登录为1，获取登录信息为2，跳转到登录界面3，用户注销为4
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.userLogin({
    *          businessType: 1,   //获取是否登录为1，获取登录信息为2，跳转到登录界面3，用户注销为4
    *          success: function (data) {
    *              //当businessType为1时，data.objInfo='1',表示已登录，data.objInfo='0'，表示未登录
    *              //当businessType为4时，因为是客户端主动调用js，所以tagJson传值为""
    *          }
    *     })
    * </code></pre>
    */

    //获取地理位置
    jh.geographicalPosition = function (options) {
        loadURL("jhoabrowser://locationFuction?args=" + base64.encode("{\"businessJson\":\"{\\\"timeoutTime\\\":" + options.timeoutTime + ",\\\"locationOutDate\\\":" + options.locationOutDate + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1213", "UTF8"));
        //window.location.href = "jhoabrowser://locationFuction?args=" + base64.encode("{\"businessJson\":\"{\\\"timeoutTime\\\":" + options.timeoutTime + ",\\\"locationOutDate\\\":" + options.locationOutDate + "}\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1213", "UTF8");
        window.setLocationInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 获取地理位置
    * @method jh.geographicalPosition
    * @param {object} option
    * @param {number} option.timeoutTime  获取地理位置超时时间（毫秒值）
    * @param {number} option.locationOutDate  地理位置过期时间（毫秒值）
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.geographicalPosition({
    *          timeoutTime: 180000,      //代表获取地理位置超时时间（毫秒值），例子中代表3分钟获取不到地理位置则认为超时，反馈给网页error
    *          locationOutDate: 300000,  //代表地理位置过期时间（毫秒值），例子中代表5分钟地理位置过期，即本次获取时间和最后一次获取时间小于五分钟，则可以用上一次获取的地理位置
    *          success: function (data) {
    *              //addresses  具体地理位置
    *              //latitude     纬度
    *              //longitude    经度
    *          }
    *     })
    * </code></pre>
    */

    //选择本地图片
    jh.pictureChoose = function (options) {
        loadURL("jhoabrowser://selectImgFuction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxSelectNum\\\":" + options.maxSelectNum + ",\\\"isSuppoerDelete\\\":" + options.isSuppoerDelete + "}\",\"businessType\":" + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("1214", "UTF8"));
        //window.location.href = "jhoabrowser://selectImgFuction?args=" + base64.encode("{\"businessJson\":\"{\\\"maxSelectNum\\\":" + options.maxSelectNum + ",\\\"isSuppoerDelete\\\":" + options.isSuppoerDelete + "}\",\"businessType\":" + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("1214", "UTF8");
        window.setLocalImgInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 选择本地图片
    * @method jh.pictureChoose
    * @param {object} option
    * @param {number} option.businessType  选择图片方式，businessType=2，代表调用图库和拍照皆可选，businessType=1，只选择图库，businessType=0，只选择拍照。
    * @param {number} option.maxSelectNum  最多可以选择的图片张数
    * @param {boolean} option.isSuppoerDelete  是否支持删除，一般为false，只有在图库选择时，web端回显数据，要求android可以删除时，返回true，在图库选择时，可以删除web页上已选择的图片
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.pictureChoose({
    *          businessType: 0,         //businessType=2，代表调用图库和拍照皆可选，businessType=1，只选择图库，businessType=0，只选择拍照。
    *          maxSelectNum: 5,         //最多可以选择的图片张数
    *          isSuppoerDelete: false,  //代表是否支持删除，一般为false，只有在图库选择时，web端回显数据，要求android可以删除时，返回true，在图库选择时，可以删除web页上已选择的图片
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */

    //转发日记
    jh.forwardingDiary = function (options) {
        loadURL("jhoabrowser://transDiary?args=" + base64.encode("{\"businessJson\":\"{\\\"diaryUrl\\\":" + options.diaryUrl + ",\\\"diaryPicUrl\\\": " + options.picurl + ", \\\"diaryContent\\\": " + options.diaryContent + ", \\\"diaryTitle\\\": " + options.diaryTitle + "}\",\"businessType\":3}", "UTF8") + "&tag=" + base64.encode("1812", "UTF8"));
        //window.location.href = "jhoabrowser://transDiary?args=" + base64.encode("{\"businessJson\":\"{\\\"diaryUrl\\\":" + options.diaryUrl + ",\\\"diaryPicUrl\\\": " + options.picurl + ", \\\"diaryContent\\\": " + options.diaryContent + ", \\\"diaryTitle\\\": " + options.diaryTitle + "}\",\"businessType\":3}", "UTF8") + "&tag=" + base64.encode("1812", "UTF8");
    };
    /**
    * 转发日记
    * @method jh.forwardingDiary
    * @param {object} option
    * @param {string} option.diaryUrl  转发日记的链接地址
    * @param {string} option.picurl  转发日记中包含的图片地址（其中一张图片地址）
    * @param {string} option.diaryContent  转发日记内容摘要（需要字数限制）
    * @param {string} option.diaryTitle  转发日记标题（比如“您收到XXX的日记”）
    * @example
    * <pre><code>
    *     jh.forwardingDiary({
    *          diaryUrl: '',      //转发日记的链接地址
    *          picurl: '',        //转发日记中包含的图片地址（其中一张图片地址）
    *          diaryContent: '',  //转发日记内容摘要（需要字数限制）
    *          diaryTitle: ''     //转发日记标题（比如“您收到XXX的日记”）
    *     })
    * </code></pre>
    */

    //保存登录地址
    jh.saveAddress = function (options) {
        loadURL("jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.url + "\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.url + "\",\"businessType\":1}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
    };
    /**
    * 保存登录地址
    * @method jh.saveAddress
    * @param {object} option
    * @param {string} option.url  保存的登录地址
    * @example
    * <pre><code>
    *     jh.saveAddress({
    *          url: 'www.jinher.com'
    *     })
    * </code></pre>
    */

    //保存用户名
    jh.saveUsername = function (options) {
        loadURL("jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.username + "\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.username + "\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
    };
    /**
    * 保存用户名
    * @method jh.saveUsername
    * @param {object} option
    * @param {string} option.username  保存的用户名
    * @example
    * <pre><code>
    *     jh.saveUsername({
    *          username: '张三'
    *     })
    * </code></pre>
    */

    //保存用户密码
    jh.savePassword = function (options) {
        loadURL("jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.password + "\",\"businessType\":3}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.password + "\",\"businessType\":3}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
    };
    /**
    * 保存用户密码
    * @method jh.savePassword
    * @param {object} option
    * @param {string} option.password  保存的用户密码
    * @example
    * <pre><code>
    *     jh.savePassword({
    *          password: '123456'
    *     })
    * </code></pre>
    */

    //保存是否记住密码
    jh.isRememberPassword = function (options) {
        loadURL("jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.isRemember + "\",\"businessType\":4}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.isRemember + "\",\"businessType\":4}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
    };
    /**
    * 保存是否记住密码
    * @method jh.isRememberPassword
    * @param {object} option
    * @param {number} option.isRemember  是否记住密码,1表示记录密码,0表示不记住密码
    * @example
    * <pre><code>
    *     jh.isRememberPassword({
    *          isRemember: 1    //1表示记录密码,0表示不记住密码
    *     })
    * </code></pre>
    */

    //保存是否自动登录
    jh.isAutomaticLogon = function (options) {
        loadURL("jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.isAutomatic + "\",\"businessType\":5}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8"));
        //window.location.href = "jhoabrowser://oaLoginFunction?args=" + base64.encode("{\"businessJson\":\"" + options.isAutomatic + "\",\"businessType\":5}", "UTF8") + "&tag=" + base64.encode("1212", "UTF8");
    };
    /**
    * 保存是否自动登录
    * @method jh.isAutomaticLogon
    * @param {object} option
    * @param {number} option.isAutomatic  是否自动登录,1表示自动登录,0表示不自动登录
    * @example
    * <pre><code>
    *     jh.isAutomaticLogon({
    *          isAutomatic: 1  //1表示自动登录,0表示不自动登录
    *     })
    * </code></pre>
    */

    //获取登录地址,获取用户名,获取用户密码,获取是否记住密码,获取是否自动登录
    jh.getOALoginInfo = function (options) {
        window.setOALoginInfo = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {
                options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        }
    };
    /**
    * 获取登录地址,获取用户名,获取用户密码,获取是否记住密码,获取是否自动登录
    * @method jh.getOALoginInfo
    * @param {object} option
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.getOALoginInfo({
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */

    //附件上传
    jh.fileUpload = function (options) {
        loadURL("jhoabrowser://uploadFile?args=" + base64.encode("{\"businessJson\":\"{\\\"BrowseModule\\\":" + options.browseModule + ",\\\"CheckModule\\\":" + options.checkModule + ",\\\"SwitchModule\\\":" + options.switchModule + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("928", "UTF8"));
        //window.location.href = "jhoabrowser://uploadFile?args=" + base64.encode("{\"businessJson\":\"{\\\"BrowseModule\\\":" + options.browseModule + ",\\\"CheckModule\\\":" + options.checkModule + ",\\\"SwitchModule\\\":" + options.switchModule + "}\",\"businessType\":2}", "UTF8") + "&tag=" + base64.encode("928", "UTF8");
        window.uploadFilesInfo = function (jsonStr, tagJson) {
            options.success(jsonStr.businessJson);
        }
        window.localFileError = function (jsonStr, tagJson) {
            options.fail(jsonStr.businessJson);
        }
    }
    /**
    * 附件上传
    * @method jh.fileUpload
    * @param {object} option
    * @param {string} option.browseModule  模式选择，选择附件上传文件模式还是浏览文件模式，‘0’--选择附件,‘1’--浏览
    * @param {string} option.checkModule  单选和多选，‘0’--单选,‘1’--多选。浏览文件模式时，单选多选参数无用
    * @param {string} option.switchModule  跳转到某一分类模块,‘music’--音乐模块,‘video’--视频模块,‘pic’--图片模块,‘theme’--主题模块,‘doc’--文档模块,‘zip’--压缩包模块,‘apk’--安装包模块,‘favorite’--收藏夹模块
    * @param {function} option.success  成功时的回调
    * @param {function} option.fail  失败时的回调
    * @example
    * <pre><code>
    *     jh.fileUpload({
    *          browseModule: '0',  //选择附件上传文件模式还是浏览文件模式，‘0’--选择附件,‘1’--浏览
    *          checkModule: '0',   //‘0’--单选,‘1’--多选。浏览文件模式时，单选多选参数无用
    *          switchModule: 'music',  //跳转到某一分类模块
    *                                  //‘music’--音乐模块
    *                                  //‘video’--视频模块
    *                                  //‘pic’--图片模块
    *                                  //‘theme’--主题模块
    *                                  //‘doc’--文档模块
    *                                  //‘zip’--压缩包模块
    *                                  //‘apk’--安装包模块
    *                                  //‘favorite’--收藏夹模块
    *          success: function (data) {
    *              //文件上传成功
    *          },
    *          fail: function (data) {
    *              //文件上传失败
    *          }
    *     })
    * </code></pre>
    */

    //进入地图的功能选择
    jh.toShowMap = function () {
        loadURL("jhoabrowser:// WVToShowMap");
    }
    /**
    * 进入地图的功能选择
    * @method jh.toShowMap
    * @example
    * <pre><code>
    *     jh.toShowMap()
    * </code></pre>
    */

    //处理视频直播网页交互
    jh.changeCamera = function () {
        loadURL("jhoabrowser:// changeCamera");
    }
    /**
    * 处理视频直播网页交互
    * @method jh.changeCamera
    * @example
    * <pre><code>
    *     jh.changeCamera()
    * </code></pre>
    */

    //Xpai直播功能
    jh.toLiveVideo = function () {
        loadURL("jhoabrowser:// LVP_ToLiveVideo");
    }
    /**
    * Xpai直播功能
    * @method jh.toLiveVideo
    * @example
    * <pre><code>
    *     jh.toLiveVideo()
    * </code></pre>
    */

    //开始视频会议
    jh.openMeetings = function () {
        loadURL("jhoabrowser:// openMeetingsFunction");
    }
    /**
    * 开始视频会议
    * @method jh.openMeetings
    * @example
    * <pre><code>
    *     jh.openMeetings()
    * </code></pre>
    */

    //加入视频会议
    jh.joinMeetings = function () {
        loadURL("jhoabrowser:// joinMeetingsFunction");
    }
    /**
    * 加入视频会议
    * @method jh.joinMeetings
    * @example
    * <pre><code>
    *     jh.joinMeetings()
    * </code></pre>
    */

    //返回首页
    jh.loadHomePage = function () {
        loadURL("jhoabrowser:// loadHomePage");
    }
    /**
    * 返回首页
    * @method jh.loadHomePage
    * @example
    * <pre><code>
    *     jh.loadHomePage()
    * </code></pre>
    */

    //分享
    jh.share = function (options) {
        loadURL("jhoabrowser://integralTypeUrl?args=" + base64.encode("{\"businessJson\":\"{\\\"shareUrl\\\":\\\"" + options.shareUrl + "\\\",\\\"shareSquareUrl\\\":\\\"" + options.shareSquareUrl + "\\\",\\\"shareContent\\\":\\\"" + options.shareContent + "\\\",\\\"imgUrl\\\":\\\"" + options.imgUrl + "\\\",\\\"message\\\":\\\"" + options.message + "\\\",\\\"actionName\\\":\\\"" + options.actionName + "\\\",\\\"shareTitle\\\":\\\"" + options.shareTitle + "\\\",\\\"sourceType\\\":" + options.sourceType + ",\\\"shareType\\\":" + options.shareType + "}\",\"businessType\":0}", "UTF8") + "&tag=" + base64.encode("928", "UTF8"));
    }
    /**
    * 分享
    * @method jh.share
    * @param {object} option
    * @param {string} option.shareUrl  分享地址
    * @param {string} option.shareSquareUrl  分享到广场的地址
    * @param {string} option.shareContent  分享内容
    * @param {string} option.imgUrl  分享图片地址
    * @param {string} option.message  分享消息
    * @param {string} option.actionName  分享出处
    * @param {string} option.shareTitle  分享标题
    * @param {number} option.sourceType  分享类型，广场中的类型
    * @param {number} option.shareType  分享类型，获取积分的类型,0默认值,1分享商品,2分享订单,3分享文章,4分享多媒体,5分享应用,6分享金币（现版本没有分享）,7分享红包（现版本没有分享）,8分享正品会（平台打包暂时不能区分是否是正品会商品）
    * @example
    * <pre><code>
    *     jh.share({
    *          shareUrl: '',    //分享地址
    *          shareSquareUrl: '',   //分享到广场的地址
    *          shareContent: '',    //分享内容
    *          imgUrl: '',   //分享图片地址
    *          message: '',   //分享消息
    *          actionName: '',   //分享出处
    *          shareTitle: '',   //分享标题
    *          sourceType: 9,   //分享类型，广场中的类型
    *          shareType: 0    //分享类型，获取积分的类型
    *                             //0,默认值
    *                             //1,分享商品
    *                             //2,分享订单
    *                             //3,分享文章
    *                             //4,分享多媒体
    *                             //5,分享应用
    *                             //6,分享金币（现版本没有分享）
    *                             //7,分享红包（现版本没有分享）
    *                             //8,分享正品会（平台打包暂时不能区分是否是正品会商品）
    *     })
    * </code></pre>
    */

    //检测升级
    jh.checkUpgrade = function () {
        loadURL("jhoabrowser:// checkUpgradeFuction");
    }
    /**
    * 检测升级
    * @method jh.checkUpgrade
    * @example
    * <pre><code>
    *     jh.checkUpgrade()
    * </code></pre>
    */

    //IU和C+的注销登录
    jh.oaPlusOrJCPlusLoginOut = function () {
        loadURL("jhoabrowser:// oaPlusOrJCPlusLoginOutFuction");
    }
    /**
    * IU和C+的注销登录
    * @method jh.oaPlusOrJCPlusLoginOut
    * @example
    * <pre><code>
    *     jh.oaPlusOrJCPlusLoginOut()
    * </code></pre>
    */

    //关闭当前网页所在控制器
    jh.closeCurrentPaget = function () {
        loadURL("jhoabrowser:// closeCurrentPagetFuction");
    }
    /**
    * 关闭当前网页所在控制器
    * @method jh.closeCurrentPaget
    * @example
    * <pre><code>
    *     jh.closeCurrentPaget()
    * </code></pre>
    */

    //web组件分享按钮是否显示的处理
    jh.setMoreButtonShowOrHidden = function (options) {
        if (userAgent.charAt(userAgent.indexOf('versioncode') + 12) > 4) {//版本为5.0.0以上，才有效
            options.showItemViews = options.showItemViews === undefined ? "" : options.showItemViews;
            options.hiddenItemViews = options.hiddenItemViews === undefined ? "" : options.hiddenItemViews;
            loadURL("jhoabrowser://setMoreButtonShowOrHidden?args=" + base64.encode("{\"businessJson\":\"{\\\"ShowMoreButton\\\":\\\"" + options.showMoreButton + "\\\",\\\"ShowItemViews\\\":\\\"" + options.showItemViews + "\\\",\\\"HiddenItemViews\\\":\\\"" + options.hiddenItemViews + "\\\"}\",\"businessType\": 1}", "UTF8") + "&tag=" + base64.encode("928", "UTF8"));
        }
    }
    /**
    * web组件分享按钮是否显示的处理
    * @method jh.setMoreButtonShowOrHidden
    * @param {object} option
    * @param {string} option.showMoreButton   是否显示更多按钮,"1"显示,"0"不显示
    * @param {string} option.showItemViews    需要显示的更多按钮下拉子项目,使用半角逗号(,)分隔,"copy_url"复制链接:复制当前url到剪切板,"more_share",分享:分享到第三方APP(优先处理原webview分享，再处理微信分享),"open_url",在浏览器打开:调用浏览器打开当前url(如果有多个浏览器，弹出对话框让用户选择)
    * @param {string} option.hiddenItemViews  需要隐藏的更多按钮下拉子项目,使用半角逗号(,)分隔,"copy_url"复制链接:复制当前url到剪切板,"more_share",分享:分享到第三方APP(优先处理原webview分享，再处理微信分享),"open_url",在浏览器打开:调用浏览器打开当前url(如果有多个浏览器，弹出对话框让用户选择)
    * @example
    * <pre><code>
    *     jh.setMoreButtonShowOrHidden({
    *          showMoreButton: '1',   //是否显示更多按钮,"1"显示,"0"不显示
    *          showItemViews: 'copy_url',   //需要显示的更多按钮下拉子项目,使用半角逗号(,)分隔,"copy_url"复制链接:复制当前url到剪切板,"more_share",分享:分享到第三方APP(优先处理原webview分享，再处理微信分享),"open_url",在浏览器打开:调用浏览器打开当前url(如果有多个浏览器，弹出对话框让用户选择)
    *          hiddenItemViews: 'more_share,open_url'   //需要隐藏的更多按钮下拉子项目,使用半角逗号(,)分隔,"copy_url"复制链接:复制当前url到剪切板,"more_share",分享:分享到第三方APP(优先处理原webview分享，再处理微信分享),"open_url",在浏览器打开:调用浏览器打开当前url(如果有多个浏览器，弹出对话框让用户选择)
    *     })
    * </code></pre>
    */

    //返回原生页面退出webview
    jh.goBackToNativePage = function (options) {
        loadURL("jhoabrowser://goBackToNativePage?args=" + base64.encode("{\"businessJson\":\"{}\",\"businessType\": 1}", "UTF8") + "&tag=" + base64.encode("3333", "UTF8"));
    }
    /**
    * 返回原生页面退出webview
    * @method jh.goBackToNativePage
    * @example
    * <pre><code>
    *     jh.goBackToNativePage()
    * </code></pre>
    */

    //原生购物车点击确认订单
    jh.shoppingCartToOrder = function (options) {
        window.setShoppingCartItems = function(jsonStr, tagJson) {
            if (jsonStr.code == 10000) {

                options.success && options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        };

        loadURL("jhoabrowser://shoppingCartToOrder?args=" + base64.encode("{\"businessJson\":\"{}\",\"businessType\": " + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("3321", "UTF8"));
    };
    /**
    * 返回原生页面退出webview
    * @method jh.goBackToNativePage
    * @example
    * <pre><code>
    *     jh.goBackToNativePage()
    * </code></pre>
    */

    //原生购物车点击确认订单
    jh.setMealToOrder = function (options) {
        window.setShoppingCartItems = function (jsonStr, tagJson) {
            if (jsonStr.code == 10000) {

                options.success && options.success(jsonStr.businessJson);
            } else if (jsonStr.message != '取消') {
                toast(jsonStr.message);
            }
        };

        loadURL("jhoabrowser://setMealToOrder?args=" + base64.encode("{\"businessJson\":\"{}\",\"businessType\": " + options.businessType + "}", "UTF8") + "&tag=" + base64.encode("3321", "UTF8"));
    };
    /**
    * 原生购物车点击确认订单
    * @method jh.shoppingCartToOrder
    * @param {object} option
    * @param {string} option.businessType   1.获取购物车商品数据2.确认订单成功3.确认订单失败
    * @param {function} option.success  成功时的回调
    * @example
    * <pre><code>
    *     jh.shoppingCartToOrder({
    *          businessType: 1,
    *          success: function (data) {
    *          }
    *     })
    * </code></pre>
    */


    //添加iframe，链接拦截后，删除iframe
    function loadURL(url) {
        var iFrame;
        iFrame = document.createElement("iframe");
        iFrame.setAttribute("src", url);
        iFrame.setAttribute("style", "display:none;");
        iFrame.setAttribute("height", "0px");
        iFrame.setAttribute("width", "0px");
        iFrame.setAttribute("frameborder", "0");
        document.body.appendChild(iFrame);
        // 发起请求后这个 iFrame 就没用了，所以把它从 dom 上移除掉
        iFrame.parentNode.removeChild(iFrame);
        iFrame = null;
    }
} else {
    jh.audioRecord = jh.audioChoose = jh.videoRecord = jh.videoChoose = jh.dateChoose = jh.userLogin = jh.geographicalPosition = jh.pictureChoose = jh.forwardingDiary = jh.saveAddress = jh.saveUsername = jh.savePassword = jh.isRememberPassword = jh.isAutomaticLogon = jh.getOALoginInfo = jh.fileUpload = jh.toShowMap = jh.changeCamera = jh.toLiveVideo = jh.openMeetings = jh.joinMeetings = jh.loadHomePage = jh.share = jh.checkUpgrade = jh.oaPlusOrJCPlusLoginOut = jh.closeCurrentPaget = jh.setMoreButtonShowOrHidden = jh.goBackToNativePage = jh.shoppingCartToOrder = jh.setMealToOrder = function () {
    }
    console.log('不支持调用金和接口');
}