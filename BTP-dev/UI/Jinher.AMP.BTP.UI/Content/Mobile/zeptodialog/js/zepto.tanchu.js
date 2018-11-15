/**
* @name zepto.extend
* @file 对Zepto做了些扩展，以下所有JS都依赖与此文件
* @desc 对Zepto一些扩展，组件必须依赖
* @import core/zepto.js
*/

(function ($) {
    $.extend($, {
        contains: function (parent, node) {
            /**
            * modified by chenluyang
            * @reason ios4 safari下，无法判断包含文字节点的情况
            * @original return parent !== node && parent.contains(node)
            */
            return parent.compareDocumentPosition
                ? !!(parent.compareDocumentPosition(node) & 16)
                : parent !== node && parent.contains(node)
        }
    });
})(Zepto);


//Core.js
; (function ($, undefined) {
    //扩展在Zepto静态类上
    $.extend($, {
        /**
        * @grammar $.toString(obj)  ⇒ string
        * @name $.toString
        * @desc toString转化
        */
        toString: function (obj) {
            return Object.prototype.toString.call(obj);
        },

        /**
        * @desc 从集合中截取部分数据，这里说的集合，可以是数组，也可以是跟数组性质很像的对象，比如arguments
        * @name $.slice
        * @grammar $.slice(collection, [index])  ⇒ array
        * @example (function(){
        *     var args = $.slice(arguments, 2);
        *     console.log(args); // => [3]
        * })(1, 2, 3);
        */
        slice: function (array, index) {
            return Array.prototype.slice.call(array, index || 0);
        },

        /**
        * @name $.later
        * @grammar $.later(fn, [when, [periodic, [context, [data]]]])  ⇒ timer
        * @desc 延迟执行fn
        * **参数:**
        * - ***fn***: 将要延时执行的方法
        * - ***when***: *可选(默认 0)* 什么时间后执行
        * - ***periodic***: *可选(默认 false)* 设定是否是周期性的执行
        * - ***context***: *可选(默认 undefined)* 给方法设定上下文
        * - ***data***: *可选(默认 undefined)* 给方法设定传入参数
        * @example $.later(function(str){
        *     console.log(this.name + ' ' + str); // => Example hello
        * }, 250, false, {name:'Example'}, ['hello']);
        */
        later: function (fn, when, periodic, context, data) {
            return window['set' + (periodic ? 'Interval' : 'Timeout')](function () {
                fn.apply(context, data);
            }, when || 0);
        },

        /**
        * @desc 解析模版
        * @grammar $.parseTpl(str, data)  ⇒ string
        * @name $.parseTpl
        * @example var str = "<p><%=name%></p>",
        * obj = {name: 'ajean'};
        * console.log($.parseTpl(str, data)); // => <p>ajean</p>
        */
        parseTpl: function (str, data) {
            var tmpl = 'var __p=[],print=function(){__p.push.apply(__p,arguments);};' + 'with(obj||{}){__p.push(\'' + str.replace(/\\/g, '\\\\').replace(/'/g, "\\'").replace(/<%=([\s\S]+?)%>/g, function (match, code) {
                return "'," + code.replace(/\\'/g, "'") + ",'";
            }).replace(/<%([\s\S]+?)%>/g, function (match, code) {
                return "');" + code.replace(/\\'/g, "'").replace(/[\r\n\t]/g, ' ') + "__p.push('";
            }).replace(/\r/g, '\\r').replace(/\n/g, '\\n').replace(/\t/g, '\\t') + "');}return __p.join('');";
            var func = new Function('obj', tmpl);
            return data ? func(data) : func;
        },

        /**
        * @desc 减少执行频率, 多次调用，在指定的时间内，只会执行一次。
        * **options:**
        * - ***delay***: 延时时间
        * - ***fn***: 被稀释的方法
        * - ***debounce_mode***: 是否开启防震动模式, true:start, false:end
        *
        * <code type="text">||||||||||||||||||||||||| (空闲) |||||||||||||||||||||||||
        * X    X    X    X    X    X      X    X    X    X    X    X</code>
        *
        * @grammar $.throttle(delay, fn) ⇒ function
        * @name $.throttle
        * @example var touchmoveHander = function(){
        *     //....
        * }
        * //绑定事件
        * $(document).bind('touchmove', $.throttle(250, touchmoveHander));//频繁滚动，每250ms，执行一次touchmoveHandler
        *
        * //解绑事件
        * $(document).unbind('touchmove', touchmoveHander);//注意这里面unbind还是touchmoveHander,而不是$.throttle返回的function, 当然unbind那个也是一样的效果
        *
        */
        throttle: function (delay, fn, debounce_mode) {
            var last = 0,
                timeId;

            if (typeof fn !== 'function') {
                debounce_mode = fn;
                fn = delay;
                delay = 250;
            }

            function wrapper() {
                var that = this,
                    period = Date.now() - last,
                    args = arguments;

                function exec() {
                    last = Date.now();
                    fn.apply(that, args);
                };

                function clear() {
                    timeId = undefined;
                };

                if (debounce_mode && !timeId) {
                    // debounce模式 && 第一次调用
                    exec();
                }

                timeId && clearTimeout(timeId);
                if (debounce_mode === undefined && period > delay) {
                    // throttle, 执行到了delay时间
                    exec();
                } else {
                    // debounce, 如果是start就clearTimeout
                    timeId = setTimeout(debounce_mode ? clear : exec, debounce_mode === undefined ? delay - period : delay);
                }
            };
            // for event bind | unbind
            wrapper._zid = fn._zid = fn._zid || $.proxy(fn)._zid;
            return wrapper;
        },

        /**
        * @desc 减少执行频率, 在指定的时间内, 多次调用，只会执行一次。
        * **options:**
        * - ***delay***: 延时时间
        * - ***fn***: 被稀释的方法
        * - ***t***: 指定是在开始处执行，还是结束是执行, true:start, false:end
        *
        * 非at_begin模式
        * <code type="text">||||||||||||||||||||||||| (空闲) |||||||||||||||||||||||||
        *                         X                                X</code>
        * at_begin模式
        * <code type="text">||||||||||||||||||||||||| (空闲) |||||||||||||||||||||||||
        * X                                X                        </code>
        *
        * @grammar $.debounce(delay, fn[, at_begin]) ⇒ function
        * @name $.debounce
        * @example var touchmoveHander = function(){
        *     //....
        * }
        * //绑定事件
        * $(document).bind('touchmove', $.debounce(250, touchmoveHander));//频繁滚动，只要间隔时间不大于250ms, 在一系列移动后，只会执行一次
        *
        * //解绑事件
        * $(document).unbind('touchmove', touchmoveHander);//注意这里面unbind还是touchmoveHander,而不是$.debounce返回的function, 当然unbind那个也是一样的效果
        */
        debounce: function (delay, fn, t) {
            return fn === undefined ? $.throttle(250, delay, false) : $.throttle(delay, fn, t === undefined ? false : t !== false);
        }
    });

    /**
    * 扩展类型判断
    * @param {Any} obj
    * @see isString, isBoolean, isRegExp, isNumber, isDate, isObject, isNull, isUdefined
    */
    /**
    * @name $.isString
    * @grammar $.isString(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***String***
    * @example console.log($.isString({}));// => false
    * console.log($.isString(123));// => false
    * console.log($.isString('123'));// => true
    */
    /**
    * @name $.isBoolean
    * @grammar $.isBoolean(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***Boolean***
    * @example console.log($.isBoolean(1));// => false
    * console.log($.isBoolean('true'));// => false
    * console.log($.isBoolean(false));// => true
    */
    /**
    * @name $.isRegExp
    * @grammar $.isRegExp(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***RegExp***
    * @example console.log($.isRegExp(1));// => false
    * console.log($.isRegExp('test'));// => false
    * console.log($.isRegExp(/test/));// => true
    */
    /**
    * @name $.isNumber
    * @grammar $.isNumber(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***Number***
    * @example console.log($.isNumber('123'));// => false
    * console.log($.isNumber(true));// => false
    * console.log($.isNumber(123));// => true
    */
    /**
    * @name $.isDate
    * @grammar $.isDate(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***Date***
    * @example console.log($.isDate('123'));// => false
    * console.log($.isDate('2012-12-12'));// => false
    * console.log($.isDate(new Date()));// => true
    */
    /**
    * @name $.isObject
    * @grammar $.isObject(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***Object***
    * @example console.log($.isObject('123'));// => false
    * console.log($.isObject(true));// => false
    * console.log($.isObject({}));// => true
    */
    /**
    * @name $.isNull
    * @grammar $.isNull(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***null***
    * @example console.log($.isNull(false));// => false
    * console.log($.isNull(0));// => false
    * console.log($.isNull(null));// => true
    */
    /**
    * @name $.isUndefined
    * @grammar $.isUndefined(val)  ⇒ Boolean
    * @desc 判断变量类型是否为***undefined***
    * @example
    * console.log($.isUndefined(false));// => false
    * console.log($.isUndefined(0));// => false
    * console.log($.isUndefined(a));// => true
    */
    $.each("String Boolean RegExp Number Date Object Null Undefined".split(" "), function (i, name) {
        var fn;

        if ('is' + name in $) return; //already defined then ignore.

        switch (name) {
            case 'Null':
                fn = function (obj) { return obj === null; };
                break;
            case 'Undefined':
                fn = function (obj) { return obj === undefined; };
                break;
            default:
                fn = function (obj) { return new RegExp(name + ']', 'i').test(toString(obj)) };
        }
        $['is' + name] = fn;
    });

    var toString = $.toString;

})(Zepto);

//Support.js
(function ($, undefined) {
    var ua = navigator.userAgent,
        na = navigator.appVersion,
        br = $.browser;

    /**
    * @name $.browser
    * @desc 扩展zepto中对browser的检测
    *
    * **可用属性**
    * - ***qq*** 检测qq浏览器
    * - ***chrome*** 检测chrome浏览器
    * - ***uc*** 检测uc浏览器
    * - ***version*** 检测浏览器版本
    *
    * @example
    * if ($.browser.qq) {      //在qq浏览器上打出此log
    *     console.log('this is qq browser');
    * }
    */
    $.extend(br, {
        qq: /qq/i.test(ua),
        uc: /UC/i.test(ua) || /UC/i.test(na)
    });

    br.uc = br.uc || !br.qq && !br.chrome && !br.firefox && !/safari/i.test(ua);

    try {
        br.version = br.uc ? na.match(/UC(?:Browser)?\/([\d.]+)/)[1] : br.qq ? ua.match(/MQQBrowser\/([\d.]+)/)[1] : br.version;
    } catch (e) { }


    /**
    * @name $.support
    * @desc 检测设备对某些属性或方法的支持情况
    *
    * **可用属性**
    * - ***orientation*** 检测是否支持转屏事件，UC中存在orientaion，但转屏不会触发该事件，故UC属于不支持转屏事件(iOS 4上qq, chrome都有这个现象)
    * - ***touch*** 检测是否支持touch相关事件
    * - ***cssTransitions*** 检测是否支持css3的transition
    * - ***has3d*** 检测是否支持translate3d的硬件加速
    *
    * @example
    * if ($.support.has3d) {      //在支持3d的设备上使用
    *     console.log('you can use transtion3d');
    * }
    */
    $.support = $.extend($.support || {}, {
        orientation: !(br.uc || (parseFloat($.os.version) < 5 && (br.qq || br.chrome))) && !($.os.android && parseFloat($.os.version) > 3) && "orientation" in window && "onorientationchange" in window,
        touch: "ontouchend" in document,
        cssTransitions: "WebKitTransitionEvent" in window,
        has3d: 'WebKitCSSMatrix' in window && 'm11' in new WebKitCSSMatrix()
    });

})(Zepto);

//Event.js
(function ($) {
    /**
    * @name $.matchMedia
    * @grammar $.matchMedia(query)  ⇒ MediaQueryList
    * @desc 是原生的window.matchMedia方法的polyfill，对于不支持matchMedia的方法系统和浏览器，按照[w3c window.matchMedia](http://www.w3.org/TR/cssom-view/#dom-window-matchmedia)的接口
    * 定义，对matchMedia方法进行了封装。原理是用css media query及transitionEnd事件来完成的。在页面中插入media query样式及元素，当query条件满足时改变该元素样式，同时这个样式是transition作用的属性，
    * 满足条件后即会触发transitionEnd，由此创建MediaQueryList的事件监听。由于transition的duration time为0.001ms，故若直接使用MediaQueryList对象的matches去判断当前是否与query匹配，会有部分延迟，
    * 建议注册addListener的方式去监听query的改变。$.matchMedia的详细实现原理及采用该方法实现的转屏统一解决方案详见
    * [GMU Pages: 转屏解决方案($.matchMedia)](https://github.com/gmuteam/GMU/wiki/%E8%BD%AC%E5%B1%8F%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88$.matchMedia)
    *
    * **MediaQueryList对象包含的属性**
    * - ***matches*** 是否满足query
    * - ***query*** 查询的css query，类似\'screen and (orientation: portrait)\'
    * - ***addListener*** 添加MediaQueryList对象监听器，接收回调函数，回调参数为MediaQueryList对象
    * - ***removeListener*** 移除MediaQueryList对象监听器
    *
    * @example
    * $.matchMedia('screen and (orientation: portrait)').addListener(fn);
    */
    $.matchMedia = (function () {
        var mediaId = 0,
            cls = 'gmu-media-detect',
            transitionEnd = $.fx.transitionEnd,
            cssPrefix = $.fx.cssPrefix,
            $style = $('<style></style>').append('.' + cls + '{' + cssPrefix + 'transition: width 0.001ms; width: 0; position: relative; bottom: -999999px;}\n').appendTo('head');

        return function (query) {
            var id = cls + mediaId++,
                $mediaElem = $('<div class="' + cls + '" id="' + id + '"></div>').appendTo('body'),
                listeners = [],
                ret;

            $style.append('@media ' + query + ' { #' + id + ' { width: 100px; } }\n');   //原生matchMedia也需要添加对应的@media才能生效
            // if ('matchMedia' in window) {
            //     return window.matchMedia(query);
            // }

            $mediaElem.on(transitionEnd, function () {
                ret.matches = $mediaElem.width() === 100;
                $.each(listeners, function (i, fn) {
                    $.isFunction(fn) && fn.call(ret, ret);
                });
            });

            ret = {
                matches: $mediaElem.width() === 100,
                media: query,
                addListener: function (callback) {
                    listeners.push(callback);
                    return this;
                },
                removeListener: function (callback) {
                    var index = listeners.indexOf(callback);
                    ~index && listeners.splice(index, 1);
                    return this;
                }
            };

            return ret;
        };
    } ());

    $(function () {
        var handleOrtchange = function (mql) {
            if (state !== mql.matches) {
                $(window).trigger('ortchange');
                state = mql.matches;
            }
        },
            state = true;
        $.mediaQuery = {
            ortchange: 'screen and (width: ' + window.innerWidth + 'px)'
        };
        $.matchMedia($.mediaQuery.ortchange).addListener(handleOrtchange);
    });

    /**
    * @name Trigger Events
    * @theme event
    * @desc 扩展的事件
    * - ***scrollStop*** : scroll停下来时触发, 考虑前进或者后退后scroll事件不触发情况。
    * - ***ortchange*** : 当转屏的时候触发，兼容uc和其他不支持orientationchange的设备，利用css media query实现，解决了转屏延时及orientation事件的兼容性问题
    * @example $(document).on('scrollStop', function () {        //scroll停下来时显示scrollStop
    *     console.log('scrollStop');
    * });
    *
    * $(window).on('ortchange', function () {        //当转屏的时候触发
    *     console.log('ortchange');
    * });
    */
    /** dispatch scrollStop */
    function _registerScrollStop() {
        $(window).on('scroll', $.debounce(80, function () {
            $(document).trigger('scrollStop');
        }, false));
    }
    //在离开页面，前进或后退回到页面后，重新绑定scroll, 需要off掉所有的scroll，否则scroll时间不触发
    function _touchstartHander() {
        $(window).off('scroll');
        _registerScrollStop();
    }
    _registerScrollStop();
    $(window).on('pageshow', function (e) {
        if (e.persisted) {//如果是从bfcache中加载页面
            $(document).off('touchstart', _touchstartHander).one('touchstart', _touchstartHander);
        }
    });
})(Zepto);


/**
* @file 所有UI组件的基类，通过它可以简单的快速的创建新的组件。
* @name zepto.ui
* @short zepto.ui
* @desc 所有UI组件的基类，通过它可以简单的快速的创建新的组件。
* @import core/zepto.js, core/zepto.extend.js
*/
(function ($, undefined) {
    var id = 1,
        _blankFn = function () { },
        tpl = '<%=name%>-<%=id%>',
        record = (function () {
            var data = {},
                id = 0,
                iKey = "GMUWidget" + (+new Date()); //internal key.

            return function (obj, key, val) {
                var dkey = obj[iKey] || (obj[iKey] = ++id),
                    store = data[dkey] || (data[dkey] = {});

                !$.isUndefined(val) && (store[key] = val);
                $.isNull(val) && delete store[key];

                return store[key];
            }
        })();

    $.ui = $.ui || {
        version: '2.0.5',

        guid: _guid,

        /**
        * @name $.ui.define
        * @grammar $.ui.define(name, data[, superClass]) ⇒ undefined
        * @desc 定义组件,
        * - ''name'' 组件名称
        * - ''data'' 对象，设置此组件的prototype。可以添加属性或方法
        * - ''superClass'' 基类，指定此组件基于哪个现有组件，默认为Widget基类
        * **示例:**
        * <code type="javascript">
        * $.ui.define('helloworld', {
        *     _data: {
        *         opt1: null
        *     },
        *     enable: function(){
        *         //...
        *     }
        * });
        * </code>
        *
        * **定义完后，就可以通过以下方式使用了**
        *<code type="javascript">
        * var instance = $.ui.helloworld({opt1: true});
        * instance.enable();
        *
        * //或者
        * $('#id').helloworld({opt1:true});
        * //...later
        * $('#id').helloworld('enable');
        * </code>
        *
        * **Tips**
        * 1. 通过Zepto对象上的组件方法，可以直接实例话组件, 如: $('#btn').button({label: 'abc'});
        * 2. 通过Zepto对象上的组件方法，传入字符串this, 可以获得组件实例，如：var btn = $('#btn').button('this');
        * 3. 通过Zepto对象上的组件方法，可以直接调用组件方法，第一个参数用来指定方法名，之后的参数作为方法参数，如: $('#btn').button('setIcon', 'home');
        * 4. 在子类中，如覆写了某个方法，可以在方法中通过this.$super()方法调用父级方法。如：this.$super('enable');
        */
        define: function (name, data, superClass) {
            if (superClass) data.inherit = superClass;
            var Class = $.ui[name] = _createClass(function (el, options) {
                var obj = _createObject(Class.prototype, {
                    _id: $.parseTpl(tpl, {
                        name: name,
                        id: _guid()
                    })
                });

                obj._createWidget.call(obj, el, options, Class.plugins);
                return obj;
            }, data);
            return _zeptoLize(name, Class);
        },

        /**
        * @name $.ui.isWidget()
        * @grammar $.ui.isWidget(obj) ⇒ boolean
        * @grammar $.ui.isWidget(obj, name) ⇒ boolean
        * @desc 判断obj是不是widget实例
        *
        * **参数**
        * - ''obj'' 用于检测的对象
        * - ''name'' 可选，默认监测是不是''widget''(基类)的实例，可以传入组件名字如''button''。作用将变为obj是不是button组件实例。
        * @param obj
        * @param name
        * @example
        *
        * var btn = $.ui.button(),
        *     dialog = $.ui.dialog();
        *
        * console.log($.isWidget(btn)); // => true
        * console.log($.isWidget(dialog)); // => true
        * console.log($.isWidget(btn, 'button')); // => true
        * console.log($.isWidget(dialog, 'button')); // => false
        * console.log($.isWidget(btn, 'noexist')); // => false
        */
        isWidget: function (obj, name) {
            return obj instanceof (name === undefined ? _widget : $.ui[name] || _blankFn);
        }
    };

    /**
    * generate guid
    */
    function _guid() {
        return id++;
    };

    function _createObject(proto, data) {
        var obj = {};
        Object.create ? obj = Object.create(proto) : obj.__proto__ = proto;
        return $.extend(obj, data || {});
    }

    function _createClass(Class, data) {
        if (data) {
            _process(Class, data);
            $.extend(Class.prototype, data);
        }
        return $.extend(Class, {
            plugins: [],
            register: function (fn) {
                if ($.isObject(fn)) {
                    $.extend(this.prototype, fn);
                    return;
                }
                this.plugins.push(fn);
            }
        });
    }

    /**
    * handle inherit & _data
    */
    function _process(Class, data) {
        var superClass = data.inherit || _widget,
            proto = superClass.prototype,
            obj;
        obj = Class.prototype = _createObject(proto, {
            $factory: Class,
            $super: function (key) {
                var fn = proto[key];
                return $.isFunction(fn) ? fn.apply(this, $.slice(arguments, 1)) : fn;
            }
        });
        obj._data = $.extend({}, proto._data, data._data);
        delete data._data;
        return Class;
    }

    /**
    * 强制setup模式
    * @grammar $(selector).dialog(opts);
    */
    function _zeptoLize(name) {
        $.fn[name] = function (opts) {
            var ret,
                obj,
                args = $.slice(arguments, 1);

            $.each(this, function (i, el) {

                obj = record(el, name) || $.ui[name](el, $.extend($.isPlainObject(opts) ? opts : {}, {
                    setup: true
                }));
                if ($.isString(opts)) {
                    if (!$.isFunction(obj[opts]) && opts !== 'this') {
                        throw new Error(name + '组件没有此方法');    //当不是取方法是，抛出错误信息
                    }
                    ret = $.isFunction(obj[opts]) ? obj[opts].apply(obj, args) : undefined;
                }
                if (ret !== undefined && ret !== obj || opts === "this" && (ret = obj)) {
                    return false;
                }
                ret = undefined;
            });
            //ret 为真就是要返回ui实例之外的内容
            //obj 'this'时返回
            //其他都是返回zepto实例
            //修改返回值为空的时，返回值不对的问题
            return ret !== undefined ? ret : this;
        };
    }
    /**
    * @name widget
    * @desc GMU所有的组件都是此类的子类，即以下此类里面的方法都可在其他组建中调用。
    */
    var _widget = function () { };
    $.extend(_widget.prototype, {
        _data: {
            status: true
        },

        /**
        * @name data
        * @grammar data(key) ⇒ value
        * @grammar data(key, value) ⇒ value
        * @desc 设置或者获取options, 所有组件中的配置项都可以通过此方法得到。
        * @example
        * $('a#btn').button({label: '按钮'});
        * console.log($('a#btn').button('data', 'label'));// => 按钮
        */
        data: function (key, val) {
            var _data = this._data;
            if ($.isObject(key)) return $.extend(_data, key);
            else return !$.isUndefined(val) ? _data[key] = val : _data[key];
        },

        /**
        * common constructor
        */
        _createWidget: function (el, opts, plugins) {

            if ($.isObject(el)) {
                opts = el || {};
                el = undefined;
            }

            var data = $.extend({}, this._data, opts);
            $.extend(this, {
                _el: el ? $(el) : undefined,
                _data: data
            });

            //触发plugins
            var me = this;
            $.each(plugins, function (i, fn) {
                var result = fn.apply(me);
                if (result && $.isPlainObject(result)) {
                    var plugins = me._data.disablePlugin;
                    if (!plugins || $.isString(plugins) && ! ~plugins.indexOf(result.pluginName)) {
                        delete result.pluginName;
                        $.each(result, function (key, val) {
                            var orgFn;
                            if ((orgFn = me[key]) && $.isFunction(val)) {
                                me[key] = function () {
                                    me[key + 'Org'] = orgFn;
                                    return val.apply(me, arguments);
                                }
                            } else
                                me[key] = val;
                        });
                    }
                }
            });
            // use setup or render
            if (data.setup) this._setup(el && el.getAttribute('data-mode'));
            else this._create();
            this._init();

            var me = this,
                $el = this.trigger('init').root();
            $el.on('tap', function (e) {
                (e['bubblesList'] || (e['bubblesList'] = [])).push(me);
            });

            record($el[0], me._id.split('-')[0], me);
        },

        /**
        * @interface: use in render mod
        * @name _create
        * @desc 接口定义，子类中需要重新实现此方法，此方法在render模式时被调用。
        *
        * 所谓的render方式，即，通过以下方式初始化组件
        * <code>
        * $.ui.widgetName(options);
        * </code>
        */
        _create: function () { },

        /**
        * @interface: use in setup mod
        * @name _setup
        * @desc 接口定义，子类中需要重新实现此方法，此方法在setup模式时被调用。第一个行参用来分辨时fullsetup，还是setup
        *
        * <code>
        * $.ui.define('helloworld', {
        *     _setup: function(mode){
        *          if(mode){
        *              //为fullsetup模式
        *          } else {
        *              //为setup模式
        *          }
        *     }
        * });
        * </code>
        *
        * 所谓的setup方式，即，先有dom，然后通过选择器，初始化Zepto后，在Zepto对象直接调用组件名方法实例化组件，如
        * <code>
        * //<div id="widget"></div>
        * $('#widget').widgetName(options);
        * </code>
        *
        * 如果用来初始化的element，设置了data-mode="true"，组件将以fullsetup模式初始化
        */
        _setup: function (mode) { },

        /**
        * @name root
        * @grammar root() ⇒ value
        * @grammar root(el) ⇒ value
        * @desc 设置或者获取根节点
        * @example
        * $('a#btn').button({label: '按钮'});
        * console.log($('a#btn').button('root'));// => a#btn
        */
        root: function (el) {
            return this._el = el || this._el;
        },

        /**
        * @name id
        * @grammar id() ⇒ value
        * @grammar id(id) ⇒ value
        * @desc 设置或者获取组件id
        */
        id: function (id) {
            return this._id = id || this._id;
        },

        /**
        * @name destroy
        * @grammar destroy() ⇒ undefined
        * @desc 注销组件
        */
        destroy: function () {
            var me = this,
                $el;
            $el = this.trigger('destroy').off().root();
            $el.find('*').off();
            record($el[0], me._id.split('-')[0], null);
            $el.off().remove();
            this.__proto__ = null;
            $.each(this, function (key) {
                delete me[key];
            });
        },

        /**
        * @name on
        * @grammar on(type, handler) ⇒ instance
        * @desc 绑定事件，此事件绑定不同于zepto上绑定事件，此On的this只想组件实例，而非zepto实例
        */
        on: function (ev, callback) {
            this.root().on(ev, $.proxy(callback, this));
            return this;
        },

        /**
        * @name off
        * @grammar off(type) ⇒ instance
        * @grammar off(type, handler) ⇒ instance
        * @desc 解绑事件
        */
        off: function (ev, callback) {
            this.root().off(ev, callback);
            return this;
        },

        /**
        * @name trigger
        * @grammar trigger(type[, data]) ⇒ instance
        * @desc 触发事件, 此trigger会优先把options上的事件回调函数先执行，然后给根DOM派送事件。
        * options上回调函数可以通过e.preventDefaualt()来组织事件派发。
        */
        trigger: function (event, data) {
            event = $.isString(event) ? $.Event(event) : event;
            var onEvent = this.data(event.type), result;
            if (onEvent && $.isFunction(onEvent)) {
                event.data = data;
                result = onEvent.apply(this, [event].concat(data));
                if (result === false || event.defaultPrevented) {
                    return this;
                }
            }
            this.root().trigger(event, data);
            return this;
        }
    });
})(Zepto);

/**
 *  @file 实现了通用highlight方法。
 *  @name zepto.highlight
 *  @desc 点击高亮效果
 *  @import core/zepto.js, core/zepto.extend.js
 */
(function($) {
    var actElem, inited = false, timer, cls, removeCls = function(){
        clearTimeout(timer);
        if(actElem && (cls = actElem.attr('highlight-cls'))){
            actElem.removeClass(cls).attr('highlight-cls', '');
            actElem = null;
        }
    };
    $.extend($.fn, {
        /**
         * @name highlight
         * @desc 禁用掉系统的高亮，当手指移动到元素上时添加指定class，手指移开时，移除该class
         * @grammar  highlight(className)   ⇒ self
         * @example var div = $('div');
         * div.highlight('div-hover');
         *
         * $('a').highlight();// 把所有a的自带的高亮效果去掉。
         */
        highlight: function(className) {
            inited = inited || !!$(document).on('touchend.highlight touchmove.highlight touchcancel.highlight', removeCls);
            removeCls();
            return this.each(function() {
                var $el = $(this);
                $el.css('-webkit-tap-highlight-color', 'rgba(255,255,255,0)').off('touchstart.highlight');
                className && $el.on('touchstart.highlight', function() {
                    timer = $.later(function() {
                        actElem = $el.attr('highlight-cls', className).addClass(className);
                    }, 100);
                });
            });
        }
    });
})(Zepto);
/**
* @file 弹出框组件
* @name Dialog
* @desc <qrcode align="right" title="Live Demo">../gmu/_examples/widget/dialog/dialog.html</qrcode>
* 弹出框组件
* @import core/zepto.ui.js, core/zepto.highlight.js
*/
(function ($, undefined) {
    var tpl = {
        close: '<a class="ui-dialog-close" title="关闭"><span class="ui-icon ui-icon-delete"></span></a>',
        mask: '<div class="ui-mask"></div>',
        title: '<div class="ui-dialog-title">' +
                    '<h3><%=title%></h3>' +
                '</div>',
        wrap: '<div class="ui-dialog">' +
            '<div class="ui-dialog-content"></div>' +
            '<% if(btns){ %>' +
            '<div class="ui-dialog-btns">' +
            '<% for(var i=0, length=btns.length; i<length; i++){var item = btns[i]; %>' +
            '<a class="ui-btn ui-btn-<%=item.index%>" data-key="<%=item.key%>"><%=item.text%></a>' +
            '<% } %>' +
            '</div>' +
            '<% } %>' +
            '</div> '
    };

    /**
    * @name $.ui.dialog
    * @grammar $.ui.dialog(options) ⇒ instance
    * @grammar dialog(options) ⇒ self
    * @desc **Options**
    * - ''autoOpen'' {Boolean}: (可选，默认：true)初始化后是否自动弹出
    * - ''closeBtn'' {Boolean}: (可选，默认：true)是否显示关闭按钮
    * - ''mask'' {Boolean}: (可选，默认：true)是否有遮罩层
    * - ''scrollMove'' {Boolean}: (可选，默认：true)是否禁用掉scroll，在弹出的时候
    * - ''title'' {String}: (可选)弹出框标题
    * - ''content'' {String|Selector}: (render模式下必填)弹出框内容
    * - ''width'' {String|Number}: (可选，默认: 300)弹出框宽度
    * - ''height'' {String|Number}: (可选，默认: \'auto\')弹出框高度
    * - ''buttons'' {Object}: (可选) 用来设置弹出框底部按钮，传入的格式为{key1: fn1, key2, fn2}，key将作为按钮的文字，fn将作为按钮点击后的Handler
    * - ''events'' 所有[Trigger Events](#dialog_triggerevents)中提及的事件都可以在此设置Hander, 如init: function(e){}。
    *
    * **如果是setup模式，部分参数是直接从DOM上读取**
    * - ''title'' 从element的title属性中读取
    * - ''content'' 直接为element。
    *
    * **比如**
    * <code>//<div id="dialog" title="弹出框标题"></div>
    * console.log($('#dialog').dialog('data', 'title')); // => 弹出框标题
    * console.log($('#dialog').dialog('data', 'content')); // => #dialog(Zepto对象)
    * </code>
    *
    * **Demo**
    * <codepreview href="../gmu/_examples/widget/dialog/dialog.html">
    * ../gmu/_examples/widget/dialog/dialog.html
    * </codepreview>
    */
    $.ui.define('dialog', {
        _data: {
            autoOpen: true,
            buttons: null,
            closeBtn: true,
            mask: true,
            width: 300,
            height: 'auto',
            title: null,
            content: null,
            scrollMove: true, //是否禁用掉scroll，在弹出的时候
            container: null,
            maskClick: null,
            position: null //需要dialog.position插件才能用
        },

        /**
        * @name getWrap
        * @grammar getWrap() ⇒ Zepto instance
        * @desc 获取最外层的节点。
        */
        getWrap: function () {
            return this._data._wrap;
        },

        _setup: function () {
            var data = this._data;
            data.content = data.content || this._el.show();
            data.title = data.title || this._el.attr('title');
        },

        _init: function () {
            var me = this, data = me._data, btns,
                i = 0, eventHanlder = $.proxy(me._eventHandler, me), vars = {};

            data._container = $(data.container || document.body);
            (data._cIsBody = data._container.is('body')) || data._container.addClass('ui-dialog-container');
            vars.btns = btns = [];
            data.buttons && $.each(data.buttons, function (key) {
                btns.push({
                    index: ++i,
                    text: key,
                    key: key
                });
            });
            data._mask = data.mask ? $(tpl.mask).appendTo(data._container) : null;
            data._wrap = $($.parseTpl(tpl.wrap, vars)).appendTo(data._container);
            data._content = $('.ui-dialog-content', data._wrap);

            data._title = $(tpl.title);
            data._close = data.closeBtn && $(tpl.close).highlight('ui-dialog-close-hover');
            me._el = me._el || data._content; //如果不需要支持render模式，此句要删除

            me.title(data.title);
            me.content(data.content);

            btns.length && $('.ui-dialog-btns .ui-btn', data._wrap).highlight('ui-state-hover');
            data._wrap.css({
                width: data.width,
                height: data.height
            });

            //bind events绑定事件
            $(window).on('ortchange', eventHanlder);
            data._wrap.on('click', eventHanlder);
            data._mask && data._mask.on('click', eventHanlder);
            data.autoOpen && me.root().one('init', function () { me.open(); });
        },

        _eventHandler: function (e) {
            var me = this, match, wrap, data = me._data, fn;
            switch (e.type) {
                case 'ortchange':
                    this.refresh();
                    break;
                case 'touchmove':
                    data.scrollMove && e.preventDefault();
                    break;
                case 'click':
                    if (data._mask && ($.contains(data._mask[0], e.target) || data._mask[0] === e.target)) {
                        return me.trigger('maskClick');
                    }
                    wrap = data._wrap.get(0);
                    if ((match = $(e.target).closest('.ui-dialog-close', wrap)) && match.length) {
                        me.close();
                    } else if ((match = $(e.target).closest('.ui-dialog-btns .ui-btn', wrap)) && match.length) {
                        fn = data.buttons[match.attr('data-key')];
                        fn && fn.apply(me, arguments);
                    }
            }
        },

        _calculate: function () {
            var me = this, data = me._data, size, $win, root = document.body,
                ret = {}, isBody = data._cIsBody, round = Math.round;

            data.mask && (ret.mask = isBody ? {
                width: '100%',
                height: Math.max(root.scrollHeight, root.clientHeight) - 1//不减1的话uc浏览器再旋转的时候不触发resize.奇葩！
            } : {
                width: '100%',
                height: '100%'
            });

            size = data._wrap.offset();
            $win = $(window);
            ret.wrap = {
                left: '50%',
                marginLeft: -round(size.width / 2) + 'px',
                //top: isBody ? round($win.height() / 2) + window.pageYOffset : '50%',
                top: isBody ? round($win.height() / 2) + window.pageYOffset : '50%',
                marginTop: -round(size.height / 2) + 'px'
            }
            return ret;
        },

        /**
        * @name refresh
        * @grammar refresh() ⇒ instance
        * @desc 用来更新弹出框位置和mask大小。如父容器大小发生变化时，可能弹出框位置不对，可以外部调用refresh来修正。
        */
        refresh: function () {
            var me = this, data = me._data, ret, action;
            if (data._isOpen) {

                action = function () {
                    ret = me._calculate();
                    ret.mask && data._mask.css(ret.mask);
                    data._wrap.css(ret.wrap);
                }

                //如果有键盘在，需要多加延时
                if ($.os.ios &&
                    document.activeElement &&
                    /input|textarea|select/i.test(document.activeElement.tagName)) {

                    document.body.scrollLeft = 0;
                    $.later(action, 200); //do it later in 200ms.

                } else {
                    action(); //do it now
                }
            }
            return me;
        },

        /**
        * @name open
        * @grammar open() ⇒ instance
        * @grammar open(x, y) ⇒ instance
        * @desc 弹出弹出框，如果设置了位置，内部会数值转给[position](widget/dialog.js#position)来处理。
        */
        open: function (x, y) {
            var data = this._data;
            data._isOpen = true;

            data._wrap.css('display', 'block');
            data._mask && data._mask.css('display', 'block');

            x !== undefined && this.position ? this.position(x, y) : this.refresh();

            $(document).on('touchmove', $.proxy(this._eventHandler, this));
            return this.trigger('open');
        },

        /**
        * @name close
        * @grammar close() ⇒ instance
        * @desc 关闭弹出框
        */
        close: function () {
            var eventData, data = this._data;

            eventData = $.Event('beforeClose');
            this.trigger(eventData);
            if (eventData.defaultPrevented) return this;

            data._isOpen = false;
            data._wrap.css('display', 'none');
            data._mask && data._mask.css('display', 'none');

            $(document).off('touchmove', this._eventHandler);
            return this.trigger('close');
        },

        /**
        * @name title
        * @grammar title([value]) ⇒ value
        * @desc 设置或者获取弹出框标题。value接受带html标签字符串
        * @example $('#dialog').dialog('title', '标题<span>xxx</span>');
        */
        title: function (value) {
            var data = this._data, setter = value !== undefined;
            if (setter) {
                value = (data.title = value) ? '<h3>' + value + '</h3>' : value;
                data._title.html(value)[value ? 'prependTo' : 'remove'](data._wrap);
                data._close && data._close.prependTo(data.title ? data._title : data._wrap);
            }
            return setter ? this : data.title;
        },

        /**
        * @name content
        * @grammar content([value]) ⇒ value
        * @desc 设置或者获取弹出框内容。value接受带html标签字符串和zepto对象。
        * @example
        * $('#dialog').dialog('content', '内容');
        * $('#dialog').dialog('content', '<div>内容</div>');
        * $('#dialog').dialog('content', $('#content'));
        */
        content: function (val) {
            var data = this._data, setter = val !== undefined;
            setter && data._content.empty().append(data.content = val);
            return setter ? this : data.content;
        },

        /**
        * @desc 销毁组件。
        * @name destroy
        * @grammar destroy()  ⇒ instance
        */
        destroy: function () {
            var data = this._data, _eventHander = this._eventHandler;
            $(window).off('ortchange', _eventHander);
            $(document).off('touchmove', _eventHander);
            data._wrap.off('click', _eventHander).remove();
            data._mask && data._mask.off('click', _eventHander).remove();
            data._close && data._close.highlight();
            return this.$super('destroy');
        }

        /**
        * @name Trigger Events
        * @theme event
        * @desc 组件内部触发的事件
        *
        * ^ 名称 ^ 处理函数参数 ^ 描述 ^
        * | init | event | 组件初始化的时候触发，不管是render模式还是setup模式都会触发 |
        * | open | event | 当弹出框弹出后触发 |
        * | beforeClose | event | 在弹出框关闭之前触发，可以通过e.preventDefault()来阻止 |
        * | close | event | 在弹出框关闭之后触发 |
        * | destroy | event | 组件在销毁的时候触发 |
        */
    });
})(Zepto);