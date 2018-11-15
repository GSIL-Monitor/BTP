/**
 * Created by zhangyh on 2016/8/22.
 */
/**
 * 金和H5数据共享接口库，可以实现平台公用数据的存储和读取功能。
 * 提供了获取入口地址、痕迹标记、base64加密/解密、jhwebview检测、获取url参数
 * @module Bridge
 * @author <a href='//www.uedcool.com'>EddyZhang</a>
 * @namespace bridge
 * @class bridge
 */
+(function ($) {

  $.bridge = {
    useBase64: false,
    base: {
      data: 'b_data',
      mark: 'b_mark',
      srcUrl: 'b_src',
      paramList: ['appId', 'speader', 'esappid', 'shareId', 'skin']
    },
    /**
     * base64加密函数
     * 用于生成字符串对应的base64加密字符串
     * @namespace bridge
     * @method base64encode
     * @param {string} input 原始字符串
     * @return {string} 加密后的base64字符串
     */
    base64encode: function (input) {
      var rv;
      rv = encodeURIComponent(input);
      rv = unescape(rv);
      rv = window.btoa(rv);
      return rv;
    },
    /**
     * base64解密函数
     * 用于解密base64加密的字符串
     * @namespace bridge
     * @method base64decode
     * @param {string} input base64加密字符串
     * @return {string} 解密后的字符串
     */
    base64decode: function (input) {
      var rv;
      rv = window.atob(input);
      rv = escape(rv);
      rv = decodeURIComponent(rv);
      return rv;
    },
    /*
     * 初始化基础数据
     */
    init: function () {
      this.setParam();
      this._setSrcUrl();
      this.setData("apptype", "4");

    },
    /**
     * 判断是否应用内页面
     * @namespace bridge
     * @method isWebview
     * @return boolean 返回布尔值
     */
    isWebview: function () {
      return navigator.userAgent.toLowerCase().indexOf('jhwebview') > -1;
    },
    /*
     *获取mark对象
     */
    getMark: function () {
      return this.getCookies(this.base.mark) ? this.getCookies(this.base.mark).split(',') : [];
    },
    /**
     * 注册一个痕迹信息，在整个访问周期内有效
     * @namespace bridge
     * @method setMark
     * @param {string} mark 字符串(6个字符以上,例如‘btpCommodityDetailAppDownloadClosed')
     */
    setMark: function (mark) {
      if (mark.length < 6) {
        console.error('setMark: Must be more than 6 characters.');
        return;
      }
      var db = this.getMark();
      var index = db.indexOf(mark);
      if (index < 0) {
        db.push(mark);
        this.setCookies(this.base.mark, db.join(','));
      } else {
        //相同的痕迹只允许注册一次
        console.error("setMark: Involve the same mark.");
      }
    },
    /**
     * 是否存在对应的痕迹信息
     * 用于字符串转换为json对象
     * @namespace bridge
     * @method isMark
     * @param {string}  mark 原始字符串
     * @return {boolean} 布尔值
     */
    isMark: function (mark) {
      var db = this.getMark();
      var index = db.indexOf(mark);
      return index > -1;
    },
    /**
     * 删除指定的痕迹信息
     * 用于字符串转换为json对象
     * @namespace bridge
     * @method removeMark
     * @param {string} mark 要删除的字符串
     */
    removeMark: function (mark) {
      if (mark === undefined) {
        //this.setCookies(this.base.mark,''); //暂时不提供清空所有痕迹的功能
      } else {
        var db = this.getMark();
        var index = db.indexOf(mark);
        if (index > -1) {
          db.splice(index, 1);
          this.setCookies(this.base.mark, db.join(','));
        }
      }
    },
    /**
     * 获取URL中的参数
     * @namespace bridge
     * @method getRequest
     * @return {object} 参数对象
     */
    getRequest: function () {
      var url = location.search;
      var theRequest = {};
      if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        //strs = str.split("&");
        //for (var i = 0; i < strs.length; i++) {
        // theRequest[strs[i].split("=")[0]] = (strs[i].split("=")[1]);
        //}
        theRequest = this._parseStrObj(str, '&');
      }
      return theRequest;
    },
    /*
     * 字符串url参数转对象
     * 用于字符串转换为json对象
     * @namespace bridge
     * @method _parseStrObj
     * @param {string} strDes 原始字符串
     * @param {string} delimiter 定义符号
     * @return {string} 加密后的base64字符串
     */
    _parseStrObj: function (strDes, delimiter) {
      var obj = {};
      if (strDes == null || strDes == '') {
        return obj;
      }
      delimiter = delimiter || ";";
      var arr = strDes.split(delimiter);
      var k, v, sub;
      for (var i = 0, len = arr.length; i < len; i++) {
        if (arr[i] !== '') {
          sub = arr[i].split("=");
          k = sub[0];
          v = sub[1];
          if (k !== '') {
            obj[k] = v;
          }
        }
      }
      return obj;
    },
    /**
     * 对象转URL参数字符串
     * @namespace bridge
     * @method parseObjUrl
     * @param {object} param 参数对象
     * @param {string} key URL参数字符串的前缀
     * @param {boolean} encode true/false 是否进行URL编码,默认为true
     * @return {string} URL参数字符串
     */
    parseObjUrl: function (param, key, encode) {
      if (param == null) return '';
      var paramStr = '';
      var t = typeof (param);
      if (t == 'string' || t == 'number' || t == 'boolean') {
        paramStr += '&' + key + '=' + ((encode == null || encode) ? encodeURIComponent(param) : param);
      } else {
        for (var i in param) {
          var k = key == null ? i : key + (param instanceof Array ? '[' + i + ']' : '.' + i);
          paramStr += this.parseObjUrl(param[i], k, encode);
        }
      }
      return paramStr;
    },
    /**
     * 设置cookies
     * @namespace bridge
     * @method setCookies
     * @param {string} key 设置的key
     * @param {string} val 设置的值
     * @param {number} time 有效时间(毫秒)
     * @return {string} URL参数字符串
     */
    setCookies: function (key, val, time) {
      var d = new Date();
      var realVal = this.useBase64 ? this.base64encode(escape(val)) : escape(val);
      d.setTime(d.getTime() + time);
      document.cookie = key + "=" + realVal + ";expires=" + d.toGMTString() + ";path=/;domain=" + document.domain;
    },
    /**
     * 获取cookies
     * @namespace bridge
     * @method getCookies
     * @param {string} key 对应的key
     * @return {string} 值
     */
    getCookies: function (key) {
      var a = document.cookie.match(new RegExp("(^| )" + key + "=([^;]*)(;|$)"));
      if (a != null) {
        return this.useBase64 ? unescape(this.base64decode(a[2])) : unescape(a[2]);
      }
      return null
    },
    /*
     * 保存入口url
     * @namespace bridge
     * @method _setSrcUrl
     */
    _setSrcUrl: function () {
      if (!this.getCookies(this.base.srcUrl)) {
        var srcHost = location.origin + location.pathname;
        this.setCookies(this.base.srcUrl, srcHost);
      }
    },
    /**
     * 获取入口url信息
     * @namespace bridge
     * @method getSrcUrl
     * @return {string} 入口url
     */
    getSrcUrl: function () {
      return this.getCookies(this.base.srcUrl);
    },
    /**
     * 保存url中的关键参数
     * @namespace bridge
     * @method setParam
     */
    setParam: function () {
      var db = this.getData();
      var param = this.getRequest();
      for (var i = 0; i < this.base.paramList.length; i++) {
        if (param[this.base.paramList[i]])
          db[this.base.paramList[i]] = param[this.base.paramList[i]];
      }
      this.setCookies(this.base.data, JSON.stringify(db));
    },
    /**
     * 获取公用数据
     * @namespace bridge
     * @method getData
     * @param {string} key 对应的key
     * @return {string | object} 公用数据
     */
    getData: function (key) {
      var db = JSON.parse(this.getCookies(this.base.data)) || {};
      if (!key) {
        return db;
      } else {
        return db[key];
      }
    },
    /**
     * 设置公用数据
     * @namespace bridge
     * @method setData
     * @param {string} key 对应的key
     * @param {string} val 对应的val
     */
    setData: function (key, val) {
      var db = this.getData();
      if (typeof key === 'string' && typeof val === 'string') {
        db[key] = val;
        this.setCookies(this.base.data, JSON.stringify(db));
      }
    },
    /**
     * 删除指定的公用数据项
     * @namespace bridge
     * @method removeData
     * @param {string} key 对应的key
     */
    removeData: function (key) {
      var db = this.getData();
      if (typeof key === 'string') {
        delete db[key];
        this.setCookies(this.base.data, JSON.stringify(db));
      }
    }
  };

  //初始化
  $.bridge.init();

})(window);
