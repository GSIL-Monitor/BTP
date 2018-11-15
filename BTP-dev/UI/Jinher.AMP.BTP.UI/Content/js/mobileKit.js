/*! mobileKit - v1.0.1 - 2016-07-12
* http://www.uedcool.com
* Copyright (c) 2016 Jinher Software; Licensed BSD */
(function($){
    if(typeof $ !='undefined'){
        $.mobilekit='1.0.1';
        $.mk=$.mobilekit;
    }else {
        console.error('Mobilekit Library cannot be used,please introduce "jQuery Library" or "Zepto Library"!');
    }
})(window.$ || window.jQuery || window.Zepto);



;;(function($,window,document,undefined){
    /**
     * 滚动到顶部组件初始化
     *
     * @param [options] {Object} 初始化参数
     */
    $.fn.backToTop = function(options){
        var scrollUp = new ScrollUp(options);
        scrollUp.init(this);
        return scrollUp;
    }
    /**
     *
     * @param options {Object} 参数
     * @constructor
     */
    var ScrollUp = function(options){
        this.defaults = {
            topDistance: '300',
            width: '40px',
            height: '40px',
            right: '10px',
            bottom: '55px',
            borderRadius: '50%',
            background: 'rgba(145,145,145,.4)',
            color: '#999',
            fontSize: '16px',
            textAlign: 'center',
            zIndex: 100
        }
        this.settings = $.extend({}, this.defaults, options);
    }
    ScrollUp.prototype = {
        /**
         * 初始化参数
         * @method init
         * @param obj {Object}
         */
        init: function(obj){
            var html = '<div class="backToTop hide">' +
                '<i class="fa fa-chevron-up"></i>' +
                '</div>';
            $(obj).append(html);
            this._settingCss();
            this._addEventlisteners();
        },
        /**
         * 设置backToTop的样式
         * @method _settingCss
         */
        _settingCss: function(){
            var self = this;
            $('.backToTop').css({
                'display': 'block',
                'position': 'fixed',
                'right': self.settings.right,
                'bottom': self.settings.bottom,
                'width': self.settings.width,
                'height': self.settings.height,
                'line-height': self.settings.height,
                '-webkit-border-radius': self.settings.borderRadius,
                'border-radius': self.settings.borderRadius,
                'background': self.settings.background,
                'color': self.settings.color,
                'font-size': self.settings.fontSize,
                'text-align': self.settings.textAlign,
                'z-index': self.settings.zIndex
            });
        },
        _addEventlisteners: function(){
            var _this = this;
            $(window).scroll(function(){
                $(window).scrollTop() > _this.settings.topDistance?$('.backToTop').removeClass('hide'):$('.backToTop').addClass('hide');
            });
            $(document).on('click','.backToTop',function(){
                $(window).scrollTop(0);
            })
        }
    }
})(Zepto,window,document);;;(function($){
    /**
     * 设置是否选中
     * @method setChecked
     * @param options [boolean] true为选中，false为不选中
     * */
    $.fn.setChecked = function(options){
        var $this = this;
        if(options===true){
            $this.find('input').attr('checked',true)
        }else {
            $this.find('input').removeAttr('checked');
        }
    };
    /**
     * 获取是否选中，如果获取元素只有一个，则返回布尔值；如果获取元素为一组，则返回数组
     * @method setChecked
     * */
    $.fn.getChecked = function(){
        var $this = this;
        var arr = [];
        $.each($this,function(){
            arr.push($(this).find('input').is(':checked'))
        });
        return arr = arr.length==1 ? arr[0] :arr;
    };
    /**
     * @method disabled
     * */
    $.fn.disabled = function(){
        var $this = this;
        $this.find('input').attr('disabled',true);
        $this.find('.fa-check').css('opacity','0.6');
    };
    /**
     * @method enable
     * */
    $.fn.enable = function(){
        var $this = this;
        $this.find('input').removeAttr('disabled');
    }

})(window.Zepto || window.jQuery);

var Checkbox = function (option) {
    if(option.type=='checkbox'){
        return _checkbox();
    }else{
        return _radio();
    }
    function _checkbox(){
        var ck = $(option.obj);
        var enableFlag = option.enable;
        ck.checked = option.checked;

        var disableCallback = function (e) {
            return false;
        };

        var getChecked = function () {
            return ck.checked;
        };

        var setChecked = function (bool) {
            if (enableFlag) {
                if(bool){
                    ck.find('span').addClass('fa-check-square-o').removeClass('fa-square-o');
                }else{
                    ck.find('span').addClass('fa-square-o').removeClass('fa-check-square-o');
                }
                ck.checked =bool;
                ck.find('input').val(bool);
            }
        };

        var enable = function () {
            enableFlag = true;
            ck.css({opacity: 1});
            ck.off('click', disableCallback);
        };

        var disable = function () {
            enableFlag = false;
            ck.css({opacity: 0.7});
            ck.on('click', disableCallback);
        };
        setChecked(ck.checked);
        return {
            getChecked: getChecked,
            setChecked: setChecked,
            enable: enable,
            disable: disable
        }
    }
    function _radio(){
        var ck = $(option.obj);
        var enableFlag = option.enable;
        ck.checked = option.checked;
        var disableCallback = function (e) {
            return false;
        };

        var getChecked = function () {
            return ck.checked;
        };

        var setChecked = function (bool) {
            if (enableFlag) {
                if(bool){
                    $(option.radioSelector).find('.fa-circle').parent().find('input').val(false);
                    $(option.radioSelector).find('.fa-circle').removeClass('fa-circle').addClass('fa-circle-o');
                    ck.find("span").addClass('fa-circle').removeClass('fa-circle-o');
                }else{
                    ck.find("span").addClass('fa-circle-o').removeClass('fa-circle');
                }
                ck.checked =bool;
                ck.find('input').val(bool);
            }
        };

        var enable = function () {
            enableFlag = true;
            ck.css({opacity: 1});
            ck.off('click', disableCallback);
        };

        var disable = function () {
            enableFlag = false;
            ck.css({opacity: 0.7});
            ck.on('click', disableCallback);
        };
        if($(option.radioSelector).find('.fa-circle').length === 0){
            setChecked(ck.checked);
        }else{
            setChecked(false);
        }
        return {
            getChecked: getChecked,
            setChecked: setChecked,
            enable: enable,
            disable: disable
        }
    }
};

;var isHistoryApi = !!(window.history && history.pushState);

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

window.addEventListener("popstate", function () {
    var currentState = {};
    //currentState = history.state;
    //解决UC不支持history.state的问题
    currentState.page = getQueryString('mkpage');

    if (currentState.page == null) {
        closeModal();
    } else if (currentState.page) {
        openModal(currentState.page, 'history');
    }
});

function openModal(tag) {
    /**
     * 显示窗口
     * @method openModal
     * @param {String} ID 窗口元素的id
     * @example
     *
     * html代码:
     * <pre><code>
     *&lt;div class="modal" id="dialog">
     *      &lt;div class="modal-dialog">
     *          &lt;div class="modal-content">
     *              &lt;div class="modal-header">
     *                  &lt;h4 class="modal-title">Modal title&lt;/h4>
     *              &lt;/div>
     *              &lt;div class="modal-body">
     *                      &lt;p>One fine body&hellip;&lt;/p>
     *
     *              &lt;/div>
     *              &lt;div class="modal-footer">
     *                  &lt;button type="button" class="btn btn-default" data-dismiss="modal">Close&lt;/button>
     *                  &lt;button type="button" class="btn btn-primary">Save changes&lt;/button>
     *              &lt;/div>
     *          &lt;/div>
     *          &lt;!-- /.modal-content -->
     *      &lt;/div>
     *      &lt;!-- /.modal-dialog -->
     * &lt;/div>
     * &lt;!-- /.modal -->
     * </code></pre>
     *
     * javascript代码:
     *<pre><code>
     *     //打开窗口
     *    openModal("dialog");
     * </code></pre>
     */

    var scroll = $(window).scrollTop();
    $('body').attr('data-scroll', scroll).addClass('modal-open');
    $('.modal').removeClass('show');
    $('.page').addClass('fuzzy');
    if (arguments.length === 1) {
        history.pushState({page: tag}, "", "?mkpage=" + tag);
    }
    if ($('#' + tag).is('.modal')) {
        var winH = $(window).height() - 10; //为了用户体验更加良好，设置了10px的偏移
        var modal = $('#' + tag).addClass('show').find('.modal-content');
        var mH = modal.height();
        var pos = winH - mH;
        modal.css('margin-top', pos > 0 ? pos / 2 : 0);

    }
}

/**
 * 关闭窗口
 * @method closeModal
 * @param {String} ID 窗口元素的id
 * @example
 *
 * html代码:
 * <pre><code>
 *&lt;div class="modal" id="dialog">
 *      &lt;div class="modal-dialog">
 *          &lt;div class="modal-content">
 *              &lt;div class="modal-header">
 *                  &lt;h4 class="modal-title">Modal title&lt;/h4>
 *              &lt;/div>
 *              &lt;div class="modal-body">
 *                      &lt;p>One fine body&hellip;&lt;/p>
 *
 *              &lt;/div>
 *              &lt;div class="modal-footer">
 *                  &lt;button type="button" class="btn btn-default" data-dismiss="modal">Close&lt;/button>
 *                  &lt;button type="button" class="btn btn-primary">Save changes&lt;/button>
 *              &lt;/div>
 *          &lt;/div>
 *          &lt;!-- /.modal-content -->
 *      &lt;/div>
 *      &lt;!-- /.modal-dialog -->
 * &lt;/div>
 * &lt;!-- /.modal -->
 * </code></pre>
 *
 * javascript代码:
 *<pre><code>
 *     //关闭窗口
 *    closeModal("dialog");
 * </code></pre>
 */
function closeModal(tag) {
    var scroll = $('body').removeClass('modal-open').attr('data-scroll');
    $(window).scrollTop(scroll);
    $('body').removeAttr('data-scroll');
    $('.page').removeClass('fuzzy');
    if (tag) {
        $('#' + tag).removeClass('show');
        history.back();
    } else {
        $('.modal').removeClass('show');
    }
}

$(document).on('click', '[data-toggle="modal"]', function (e) {
    var $this = $(this);
    var href = $this.attr('href');
    openModal($this.attr('data-target'));

    if ($this.is('a')) {
        e.preventDefault()
    }

}).on('click', '[data-dismiss="modal"]', function (e) {
    var $this = $(this);
    var modalId = $this.parents('.modal').attr('id');
    closeModal(modalId);
});

$(function () {
    //url状态响应
    var p = getQueryString('mkpage');
    if (p) {
        openModal(p, 'history');
    }

    $(window).resize(function () {
        var winH = $(window).height() - 10; //为了用户体验更加良好，设置了10px的偏移
        var modal = $('[class="modal show"]').find('.modal-content');
        var mH = modal.height();
        var pos = winH - mH;
        modal.css('margin-top', pos > 0 ? pos / 2 : 0);
    });
});;;(function($){
    /**
     * @method panel
     * @param {string} [state] 侧边栏状态 参数 open | close
     * @example
     *
     * html代码
     * <pre><code>
     *     &lt;button data-target="test" data-direction="left">&lt;/button>
     *
     *     &lt;div class="panel" id="test">&lt;/div>
     * </code></pre>
     *
     * javascript代码：
     * <pre><code>
     *     //使用侧边栏
     *     $('button').panel();
     *
     *     //打开侧边栏
     *     $('button').panel('open');
     *
     *     //关闭侧边栏
     *     $('button').panel('close');
     * </code></pre>
     * */
    $.fn.panel=function(options){
        var state = 'close';
        if(options) state = options;

        var $this = this,
            head = $('head'),
                ele = $this.attr('data-target'),
                dir = $this.attr('data-direction'),
                width = $('.panel').width();

        var overLay = $('<div class="panel-overlay"></div>');

        var styles = '.position-left {left: -'+width+'px} .position-right {right: -'+width+'px}';
        var style = $('<style>'+styles+'</style>');
        head.append(style);

        $('#'+ele).addClass('position-'+dir);

        $this.on('click',openPanel);
        overLay.on('click',closePanel);

        function openPanel(){
            $('body').append(overLay);
            $('#'+ele).addClass('panel-'+dir);
            var scroll = $(window).scrollTop();

            $('body').css('top',-scroll).addClass('model-open');
            $('body').attr('data-scroll',scroll);
        }

        function closePanel(){
            $(this).remove();
            $('#'+ele).removeClass('panel-'+dir);
            var scroll = $('body').removeClass('model-open').attr('data-scroll');
            $(window).scrollTop(scroll);
        }

        switch(state){
            case 'open':
                openPanel();
                break;
            case 'close':
                closePanel();
                break;
        }
    };
})(window.Zepto || window.jQuery);;$(function () {
    $(document).on('click', '.spinner', function (e) {

        var $this = $(this),
            $input = $this.find('input'),
            $val = parseInt($input.val()),
            $min = parseInt($input.prop('min')),
            $max = parseInt($input.prop('max')),
            $minus = $this.find('.minus'),
            $plus = $this.find('.plus');

        if ($(e.target).is('.minus') && !$this.is('.disable')) {
            if ($val > $min) {
                --$val;
                $input.val($val);
            }
            updata();
        } else if ($(e.target).is('.plus') && !$this.is('.disable')) {
            if ($val < $max) {
                ++$val;
                $input.val($val);
            }
            updata();
        }
        function updata() {
            var isMax = $val == $max,
                isMin = $val == $min;

            if (isMax) {
                $plus.addClass('disable');
            } else {
                $plus.removeClass('disable');
            }

            if (isMin) {
                $minus.addClass('disable');
            } else {
                $minus.removeClass('disable');
            }
            $this.trigger('spinner:change');
        }


    }).on('blur','.spinner > input', function (e) {
        $(this).parent().trigger('spinner:change');
    }).on('click','.spinner > input', function (e) {
        $(this)[0].select();
    })
});

(function ($) {
    $.fn.spinner = function (options) {
        /**
         * 数字微调组件允许开发人员使用代码来控制值和状态,获取当前值。
         *
         *
         * @method spinner
         * @param {Number|String} value 组件值/状态，参数：无 | 数值 | disable | enable
         * @return {Number} value 返回组件的当前值
         * @example
         *
         * html代码:
         *<pre><code>
         *
         * &lt;div class="spinner" id="test">
         * &lt;a href="javascript:void(0);" class="minus">&lt;/a>&lt;input type="number" min="1" max="10" value="1" maxlength="3">&lt;a href="javascript:void(0);" class="plus">&lt;/a>
         * &lt;/div>
         *
         *</code></pre>
         *
         * javascript代码:
         *<pre><code>
         *     //获取当前值
         *     $("#test").spinner() //返回当前值
         *
         *     //设置组件值，并返回当前值
         *     $("#test").spinner(20) //设置值为20，并返回当前值20
         *
         *     //设置禁用状态
         *     $("#test").spinner("disable") //返回当前值
         *
         *     //设置启用状态
         *     $("#test").spinner("enable") //返回当前值
         *
         *   </code></pre>
         */
        options = typeof options === "string" ? options.toLowerCase() : options;
        var $this = $(this),
            $input = $this.find('input'),
            $val = parseInt($input.val()),
            $min = parseInt($input.prop('min')),
            $max = parseInt($input.prop('max')),
            $minus = $this.find('.minus'),
            $plus = $this.find('.plus');

        function updata() {
            var isMax = $val == $max,
                isMin = $val == $min;

            if (isMax) {
                $plus.addClass('disable');
            } else {
                $plus.removeClass('disable');
            }

            if (isMin) {
                $minus.addClass('disable');
            } else {
                $minus.removeClass('disable');
            }

            /**
             * 状态改变事件
             * @event spinner:change
             * @example
             * html代码:
             * <pre><code>
             * &lt;div class="spinner" id="test">
             * &lt;a href="javascript:void(0);" class="minus">&lt;/a>&lt;input type="number" min="1" max="10" value="1" maxlength="3">&lt;a href="javascript:void(0);" class="plus">&lt;/a>&lt;!--代码禁止换行-->
             * &lt;/div>
             * </code></pre>
             *
             *javascript代码:
             * <pre><code>
             * //添加状态变更事件
             * $("#test").on("spinner:change",function(){
             *  toast("状态/值改变");
             * }) //返回当前值
             *
             * </code></pre>
             */
            $this.trigger('spinner:change');
        }


        if (typeof options === "number") {
            $input.val(options);
            $val = options;
            updata();
        } else if (options === 'disable') {
            $this.addClass("disable");
            $input.prop("disabled", true);
            updata();
        } else if (options === 'enable') {
            $this.removeClass("disable");
            $input.prop("disabled", false);
            updata();
        }
        return $val;
    }
})(Zepto);

;$(function () {
    $(document).on('click', '.switch', function () {
        var $this = $(this);
        if ($this.is(".active")) {
            $this.removeClass("active").find('input[type="checkbox"]').prop('checked', false);
        } else {
            $this.addClass("active").find('input[type="checkbox"]').prop('checked', true);
        }
        $this.trigger('switch:change');
    }).on('swipeLeft swipeRight', '.switch', function (e) {
        if (e.type === 'swipeLeft') {
            $(this).switch("off");
        } else if (e.type === 'swipeRight') {
            $(this).switch("on");
        }
    });
});


(function ($) {
    $.fn.switch = function (options) {
        /**
         * 开关组件允许开发人员使用代码来控制开关状态。
         *
         *
         * @method switch
         * @param {String} [state] 开关状态，参数：on | off | disable | enable
         * @return {Boolean} [state] 返回代表状态的布尔值 false | true
         * @example
         *
         * html代码:
         *<pre><code>
         *     &lt;div class="switch" id="test">
         *          &lt;div class="switch-handle">&lt;/div>
         *          &lt;input type="checkbox"/>
         *     &lt;/div>
         *
         *</code> </pre>
         *
         * javascript代码:
         *<pre><code>
         *     //获取状态
         *     $("#test").switch() //返回布尔值
         *
         *     //设置开启状态
         *     $("#test").switch("on") //返回布尔值 true
         *
         *     //设置关闭状态
         *     $("#test").switch("off") //返回布尔值 false
         *
         *     //设置禁用状态
         *     $("#test").switch("disable") //返回布尔值
         *
         *     //设置启用状态
         *     $("#test").switch("enable") //返回布尔值
         *
         *   </code> </pre>
         */
        options = typeof options === "string" ? options.toLowerCase() : "";
        var $this = $(this);
        if (options === 'on') {
            $this.addClass("active").find('input[type="checkbox"]').prop('checked', true);
            $this.addClass("active");
        } else if (options === 'off') {
            $this.removeClass("active").find('input[type="checkbox"]').prop('checked', false);
            $this.removeClass("active");
        } else if (options === 'disable') {
            $this.addClass("disable").find('input[type="checkbox"]').prop('disabled', true);
        } else if (options === 'enable') {
            $this.removeClass("disable").find('input[type="checkbox"]').prop('disabled', false);
        }
        /**
         * 状态改变事件
         * @event switch:change
         * @example
         *
         * html代码:
         * <pre><code>
         *  &lt;div class="switch" id="test">
         *      &lt;div class="switch-handle">&lt;/div>
         *      &lt;input type="checkbox"/>
         *  &lt;/div>
         * </code></pre>
         *
         *
         *javascript代码:
         * <pre><code>
         * //添加状态变更事件
         * $("#test").on("switch:change",function(){
         *  toast("状态改变")；
         * }) //返回布尔值
         *
         * </code></pre>
         */

        $(this).trigger('switch:change');
        return $this.find('input[type="checkbox"]').prop('checked');
    }
})(Zepto);;;(function($,window,undefined){
    /**
     * 选项卡
     * @param [options]{Object}
     * @returns {SliderTab}
     */
    $.fn.Tab = function(options){
        return new SliderTab(this,options);
    }
    var SliderTab = function(ele, opt){
        this.defaults = {
            'before': new Function()
        }
        this.options = $.extend({},this.defaults,opt);
        this.container = ele;
        if(this.container[0].nodeName.toLowerCase()=='ul'){
            this.element=this.container;
            this.container=this.element.parent();
        }else{
            this.element=this.container.find('ul').eq(0);
        }
        this.init();
    }
    SliderTab.prototype = {
        /**
         * 初始化
         */
        init: function(){
            var self = this;
            this.slides = this.element.children('li');
            this.index  = 0;
            if(this.slides.length < 2)return;//小于2不需要滚动
            this.length = this.slides.length;
            this.resize();
            this.element[0].addEventListener('touchstart',this.bind(this._start,this),false);
            this.element[0].addEventListener('touchmove',this.bind(this._move,this),false);
            this.element[0].addEventListener('touchend',this.bind(this._end,this),false);
            var height = this.element.children('li').eq(this.index).height();
            this.container.height(height);
        },
        bind: function(func, obj){
            return function(){
                return func.apply(obj, arguments);
            }
        },
        _start: function(e){
            this.startPos=this.getMousePoint(e);
            var style=this.element[0].style;
            style.webkitTransitionDuration = style.MozTransitionDuration = style.msTransitionDuration = style.OTransitionDuration = style.transitionDuration = '0ms';
            this.scrolling=1;//滚动屏幕
            this.startTime=new Date();
        },
        _move: function(e){
            if(!this.scrolling) return;
            this.endPos = this.getMousePoint(e);
            var offx = this.endPos.x-this.startPos.x;
            var offy = this.endPos.y-this.startPos.y;
            if(Math.abs(offx) > Math.abs(offy) *2){
                e.preventDefault();
                offx = (!this.index&&offx>0 || this.index==this.length-1&&offx<0) ? offx/3: offx;
                this.element[0].style.left=-this.index*this.width+offx+'px';
            }
        },
        _end: function(e){
            if(typeof this.scrolling != 'undefined'){
                offx=this.endPos.x-this.startPos.x || 0;
                offy = this.endPos.y-this.startPos.y;
                this.y = offy;
                if(Math.abs(offx) > Math.abs(offy) *2 && (new Date()-this.startTime<250 && Math.abs(offx)>this.width*0.1 || Math.abs(offx)>this.width/2) && ((offx<0 && this.index+1<this.length) || (offx>0 && this.index>0))){
                    offx>0?this.prev():this.next();
                }else{
                    this.slide(this.index);
                }
                delete this.scrolling;//删掉标记
                delete this.startPos;
                delete this.endPos;
                delete this.startTime;
            }
        },
        prev:function(offset){
            var index=this.index < 1?this.length-1:this.index-1;
            this.slide(index);
        },
        next:function(offset){
            this.slide((this.index+1)%this.length);
        },
        /**
         * @example
         *  javascript 代码
         * <pre><code>
         *	    tab.slide(index);
         * </pre></code>
         * @method slider
         * @param index 索引值
         */
        slide:function(index){
            index=index<0?this.length-1:index>=this.length?0:index;
            var el=this.element[0],
                    style=el.style,
                    _this=this,
                    b=parseInt(style.left) || 0, //初始量
                    c=-index*this.width-b, //变化量
                    d=Math.abs(c)<this.width?Math.ceil(Math.abs(c)/this.width*300):300,//动画持续时间
                    run=function(){
                        style.left=-_this.width*index+'px';
                        _this.index=index;
                    }
            style.WebkitTransition=style.MozTransition=style.msTransition=style.OTransition=style.transition = 'left '+ d +'ms linear';
            this.options.before.call(this, index, this.slides[this.index]);
            run();
            var height = this.element.children('li').eq(this.index).height();
            this.container.height(height);
        },
        /**
         * 获取坐标
         * @param ev
         * @returns {{x: number, y: number}}
         */
        getMousePoint:function(ev) {
            ev= ev || window.event;
            var x = 0,
                    y = 0,
                    pointer = ev.touches? ev.touches[0]: ev;

            x += pointer.clientX;
            y += pointer.clientY;
            return {'x' : x, 'y' : y};
        },
        resize:function(){
            var css;
            this.container.css({
                'overflow':'hidden',
                'visibility':'hidden',
                'listStyle':'none',
                'position':'absolute',
                'top':'50px',
                'left':0,
                'bottom':0,
                'width': '100%'
            });
            this.width=this.container[0].clientWidth-parseInt(this.container.css('padding-left'))-parseInt(this.container.css('padding-right'));
            css={'position':'relative','padding':0,'margin': 0};
            css['width']=this.width*this.length+'px';
            css['left']=-this.width*this.index+'px';
            this.element.css(css);
            this.slides.css({
                'position': 'relative',
                'width': this.width+'px',
                'min-height': this.container.height() + 'px',
                'float':'left',
                'padding': 0,
                'margin': 0
            });
            this.container.css({'visibility':'visible'});
        }
    }
})(Zepto, window, undefined);;var toast = function () {
    /**
     * 简洁的信息提示框，自动在1.5秒后消失，用于在不阻断用户正常交互的情况下显示用户操作后的信息反馈。
     *
     * @method toast
     * @param {String} msg 信息的内容
     * @param {Function} [callback] 信息显示后的回调函数
     * @param {Number} [time=1500] 信息显示时间，默认1.5秒
     * @example
     * <pre><code>
     *    //一般
     *    toast("成功加入购物车");
     *
     *    //定义显示时间
     *    toast("加入购物车失败",3000);
     *
     *    //定义回调
     *    toast("欢迎使用MobileKit框架！",function(){
     *          toast("信息显示完成");
     *    });
     * </code></pre>
     */
    /*参数解析*/
    var msg,callback,time;
    for(var i=0;i<3;i++){
        switch(Object.prototype.toString.call(arguments[i]).slice(8,-1)){
            case 'String':{
                msg = arguments[i];
                break;
            }
            case 'Function':{
                callback = arguments[i];
                break;
            }
            case 'Number':{
                time = arguments[i];
                break;
            }
        }
    }

    var doc = document, timer;

    time = time || 1500;
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
    timer = setTimeout(function () {
        toastBox.style.display = "none";
        toastBox.parentNode.removeChild(toastBox);
        if (typeof callback === "function") {
            callback();
        }
    }, time);
};;(function ($) {
    $.scrollToID = $.fn.scrollToID = function(id,moreTop){
        var y = $(id).position().top-moreTop;
        $(this).scrollTop(y);
    };
})(Zepto);
$(function(){
    var letterUnit= $(".letter-index li").height();
    var  letterLength = $(".letter-index li").length - 1;
    var  letterTop = ($(window).height()-$(".letter-index").height())/2;
    var alphaTop = getPos($('.alpha')[0]).top;

    $(".letter-index").on("touchmove touchstart",function (e) {
        e.preventDefault();
        var touch = e.targetTouches[0];
        if (e.targetTouches.length == 1) {
            var i = parseInt((touch.clientY-letterTop)/letterUnit);
            var t = $(this).find("li").eq(i <= 0 ? 0 : (i > letterLength ? letterLength : i)).attr('data-letter');
            $(window).scrollToID("#"+t,alphaTop);
        }
    });
    function getPos(obj){
        var t=0;
        while(obj){
            t+=obj.offsetTop;
            obj=obj.offsetParent;
        }
        return {top:t};
    }
});

;(function($, undefined) {
	 /**
	  * 音频组件初始化
	  *
	  * @param playlist{Array} 播放列表
	  * @param [options] {Object} 初始化参数
	  * @returns {jPlayer}
	  */
	$.fn.musicPlayer = function(playlist,options/*可选*/){
		var html = '<div id="audio-wrap" class="audio-wrap">'+
						'<div class="audio-title-wrap">'+
							'<div class="audio-title"></div>'+
							'<a id="close-player" href="javascript:void(0);" class="closed">&times;</a>'+
						'</div>'+
						'<div class="audio-progress">'+
							'<span class="time"></span>'+
							'<div class="audio-ui-progress-wrap">'+
								'<div class="audio-ui-progress">'+
									'<span class="audio-ui-play-progress" style="width:0"></span>'+
								'</div>'+
							'</div>'+
							'<span class="duration"></span>'+
						'</div>'+
						'<div class="audio-opera">'+
							'<a href="javascript:void(0);"><i class="fa fa-step-backward"></i></a>'+
							'<a class="hide" href="javascript:void(0);"><i class="fa fa-pause"></i></a>'+
							'<a class="" href="javascript:void(0);"><i class="fa fa-play"></i></a>'+
							'<a href="javascript:void(0);"><i class="fa fa-step-forward"></i></a>'+
						'</div>'+
						'<audio id="jp_audio_0" preload="metadata"></audio>'+
					'</div>';
		$(this).append(html);
		return new jPlayer(playlist,options);
	}
	
	var jPlayer = function(playlist,options){
		this.status = $.extend(true, {}, this.status, options);
		if(this.status.backgroundColor){
			$('#audio-wrap').addClass(this.status.backgroundColor);
		}
		this.current = 0;
		this.playlist = playlist || [];
		this._html_setAudio(this.playlist[this.current]);
		this._init();
		this._html_load();
		this._getHtmlStatus(document.getElementById('jp_audio_0'));
	};
	
	jPlayer.prototype ={
		options: {
			cssSelector: { 
				play: ".fa-play",
				pause: ".fa-pause",
				seekBar: ".audio-ui-progress",
				playerNext: ".fa-step-forward",
				playerPrev: ".fa-step-backward"
			}
		},
		status: {
			src: "",
			currentPercentRelative: 0,
			currentTime: 0,
			duration: 0,
			title: "",
			cycle: true,
			canPlay: true,
			backgroundColor: ''
		},
		play: function(time) {
			time = (typeof time === "number") ? time : NaN; // Remove jQuery event from click handler
			this._html_play(time);
		},
		pause: function(time) {
			time = (typeof time === "number") ? time : NaN;
			this._html_pause(time);
		},
		/**
		* 快进触发事件
		*/
		seekBar: function(e){
			var offset = $(this.options.cssSelector.seekBar).offset();
			var x = e.pageX - offset.left;
			var w = $(this.options.cssSelector.seekBar).width();
			var p = 100 * x/w;
			if(this.status.canPlay){
				this.status.currentPercentRelative = p;
				this._updateInterface();
			}
			this.playHead(p);
		},
		/**
		* 设置快进的播放时间
		*/
		playHead: function(percent) {
			var media = document.getElementById('jp_audio_0');
			if((typeof media.seekable === "object") && (media.seekable.length > 0)&& media.seekable.end(media.seekable.length-1)) {
				media.currentTime = percent * media.seekable.end(media.seekable.length-1) / 100;
			} else if(media.duration > 0 && !isNaN(media.duration)) {
				media.currentTime += percent * media.duration / 100;
			}
		},
		/**
		* 播放下一首
		*/
		playerNext: function() {
			this.status.canPlay = true;
			var index = (this.current + 1 < this.playlist.length) ? this.current + 1 : 0;
			this.current = index;
			this._html_setAudio();
			this._html_load();
			this._change();
		},
		/**
		* 播放上一首
		*/
        playerPrev: function() {
			this.status.canPlay = true;
			var index = (this.current - 1 >= 0) ? this.current - 1 : this.playlist.length - 1;
			this.current = index;
			this._html_setAudio();
			this._html_load();
			this._change();
		},
		_change: function(){
			var self = this;
			var internalId = setTimeout(function(){
				self._getHtmlStatus(document.getElementById('jp_audio_0'));
				if(self.status.canPlay){
					document.getElementById('jp_audio_0').play();
					self._updateButtons(true);
				}else{
					self._updateButtons(false);
				}
				if(internalId){
					clearTimeout(internalId);
				}
			},100);
		},
		_html_pause: function(time) {
			$('#jp_audio_0')[0].pause();
			this._updateButtons(false);
		},
		_html_play: function(time) {
			$('#jp_audio_0')[0].play();
		},
		_init: function(){
			this._addEventListeners(document.getElementById('jp_audio_0'));
			this._cssSelector();
		},
		_html_setAudio: function() {
			this.status = $.extend(true, {}, this.status, this.playlist[this.current]);
		},
		_html_load: function() {
            if(this.status.src){
                $('#jp_audio_0').attr('src',this.status.src);
            }else{
                $('#jp_audio_0').removeAttr('src');
            }
			var title = $('.audio-title-wrap .audio-title');
			if(title.length){
				title.text(this.status.title);
			}
		},
		_updateButtons: function(playing) {
			if(playing !== undefined) {
				if(playing) {
					$('.audio-opera a i.fa-play').parent().addClass('hide');
					$('.audio-opera a i.fa-pause').parent().removeClass('hide');
				} else {
					$('.audio-opera a i.fa-play').parent().removeClass('hide');
					$('.audio-opera a i.fa-pause').parent().addClass('hide');
				}
			}
			
		},
		_addEventListeners: function(mediaElement) {
			var self = this;
			mediaElement.addEventListener("timeupdate", function() {
				self._getHtmlStatus(mediaElement);
			}, false);
			mediaElement.addEventListener("playing", function() {
				self._updateButtons(true);
			}, false);
			mediaElement.addEventListener("ended", function() {
				self._updateButtons(false);
				if(self.status.cycle){
					self.playerNext();
				}
			}, false);
			mediaElement.addEventListener("error", function() {
				self.status.canPlay = false;
				var code = mediaElement.error.code;
				switch(code){
				case 1:
					toast('取回过程被用户中止',1000);
					break;
				case 2:
					toast('下载时发生错误',1000);
					break;
				case 3:
					toast('解码时发生错误',1000);
					break;
				case 4:
					toast('该音频不支持',1000);
					break;
				}
			}, false);
			document.getElementById('close-player').addEventListener('click', function(){
				$('#audio-wrap').addClass('hide');
				$('#jp_audio_0')[0].pause();
				self._updateButtons(false);
			}, false);
		},
		_cssSelector: function() {
			var self = this;
			$.each(this.options.cssSelector, function(fn, cssSel) {
				if($(cssSel).length) {
					var handler = function(e) {
						self[fn](e);
						return false;
					};
					$(cssSel).bind("click", handler);
				}
			});
		},
		_getHtmlStatus: function(media) {
			var currentTime = 0, d = 0, cpa = 0, cpr = 0;
			if(media.duration) {
				this.status.duration = media.duration;
			}else{
				this.status.duration = 0;
			}
			currentTime = media.currentTime;
			cpa = (this.status.duration > 0) ? 100 * currentTime / this.status.duration : 0;
			if((typeof media.seekable === "object") && (media.seekable.length > 0) && media.seekable.end(media.seekable.length-1)) {
				cpr = 100 * media.currentTime / media.seekable.end(media.seekable.length-1);
			} else {
				cpr = cpa;
			}
			this.status.currentPercentRelative = cpr;
			this.status.currentTime = currentTime;
			this._updateInterface();
		},
		_updateInterface: function() {
			var playBar = $('.audio-ui-play-progress');
			var duration = $('.audio-progress .duration');
			var currentTime = $('.audio-progress .time');
			if(playBar.length) {
				playBar.width(this.status.currentPercentRelative +"%");
			}
			if(currentTime.length) {
				currentTime.text(this._convertTime(this.status.currentTime));
			}
			if(duration.length) {
				duration.text(this._convertTime(this.status.duration));
			}
		},
		_convertTime: function(s) {
			var myTime = new Date(s * 1000);
			var hour = myTime.getUTCHours();
			var min = myTime.getUTCMinutes();
			var sec = myTime.getUTCSeconds();
			var strHour = (hour < 10) ? "0" + hour : hour;
			var strMin = (min < 10) ? "0" + min : min;
			var strSec = (sec < 10) ? "0" + sec : sec;
			return strHour + ":" + strMin + ":" + strSec;
		},
		/**
		 * 指定选择播放哪条音频
		 *
		 * @example
		 *
		 * javascript代码:
		 *<pre><code>
		 *		player.playlistAdvance(3);//播放第四首音频
		 *</code></pre>
		 *
		 * @method playlistAdvance
		 * @param index{Number} 播放列表的索引值
		 */
		playlistAdvance: function(index){
			this.current = index;
			this._html_setAudio();
			this._html_load();
			this._change();
			if($('#audio-wrap').hasClass('hide')){
				$('#audio-wrap').removeClass('hide');
			}
		},
		/**
		 * 更新播放列表
		 *
		 * @example
		 *
		 * javascript代码:
		 *<pre><code>
		 *		//测试数据
		 *		var myPlaylist2 = [
		 *			{
		 *				src:'mix/3.mp3', //音频路径
		 *				title:'Sample 1' //音频标题
		 *			},
		 *			{
		 *				src:'mix/4.mp3', //音频路径
		 *				title:'Sample Go' //音频标题
		 *			}
		 *		];
		 *		player.updatePlaylist(myPlaylist2);
		 *</code></pre>
		 *
		 * @method updatePlaylist
		 * @param list{Array} 更新的播放列表
		 */
		updatePlaylist: function(list){
			this.current = 0;
			this.playlist = list || [];
			this._html_setAudio(this.playlist[this.current]);
			this._html_load();
			this._updateButtons(false);
			this._getHtmlStatus(document.getElementById('jp_audio_0'));
		},
		/**
		 * 是否自动循环播放
		 * @example
		 *
		 * javascript代码:
		 *<pre><code>
		 *		player.isCycle(false);//手动循环播放
		 *		player.isCycle(true);//自动循环播放
		 *</code></pre>
		 *
		 * @method isCycle
		 * @param option{Boolean} true:自动循环播放(默认) false: 手动播放
		 */
		isCycle: function(option){
			this.status.cycle = option;
		},
		/**
		 * 更新播放器背景色
		 *
		 * @example
		 *
		 * javascript代码:
		 *<pre><code>
		 *		player.updateBackgroundcolor('color');
		 *</code></pre>
		 *
		 * @method updateBackgroundcolor
		 * @param color{String} 颜色的样式white
		 */
		updateBackgroundcolor: function(color){
			if(color){
				var older = this.status.backgroundColor;
				if(color == 'default'){
					color = '';
					if(color != older){
						this.status.backgroundColor = '';
						$('#audio-wrap').removeClass(older);
					}
				}else{
					if(color != older){
						this.status.backgroundColor =color;
						if(older){
							$('#audio-wrap').removeClass(older);
						}
						$('#audio-wrap').addClass(color);
					}
				}
			}
		}
	}
	return {
		jPlayer: jPlayer
	};
})(Zepto);;(function ($) {
    /**
     * @method resizable
     * @example
     *
     * <pre><code>
     *     &lt;!-- 一般 -->
     *     &lt;textarea data-fn="resizable">&lt;/textarea>
     *     </pre></code>
     *
     *
     * <pre><code>
     *     &lt;!-- 销毁 -->
     *     &lt;!-- html代码 -->
     *     &lt;textarea data-fn="resizable">&lt;/textarea>
     *
     *     //js代码
     *     $(function(){
     *         $("[data-fn~=resizable]").resizable('destroy');
     *     })
     *
     *     </pre></code>
     *
     * */
    $.extend($.fn, {
        resizable: function (options) {   //参数为:destroy时，方法禁用。

            var _elements = this;

            // 高度计算
            function _setHight(el) {
                el.height('auto').height(el[0].scrollHeight + 15);//增加空白区域，便于输入
            }

            if (_elements.data('bind-resize') != 'true') { //判断是否已绑定resize事件
                //避免重复绑定视图变化事件
                $(window).on("resize.autogrowHeight", function () {

                    _elements.each(function () {
                        if ($(this)[0].tagName == 'TEXTAREA' && options != 'destroy') {
                            _setHight($(this));
                        }
                    });

                });
                //添加自定义属性
                _elements.data('bind-resize', 'true');

                _elements.each(function () {

                    var $self = $(this);
                    if ($self[0].tagName == 'TEXTAREA' && options != 'destroy') {
                        //初始化高度
                        _setHight($self);

                        //避免事件重复绑定
                        $self.off('.autogrow').on('keyup.autogrow change.autogrow input.autogrow paste.autogrow', function () {
                            _setHight($self);
                        });
                    }

                });
            }
            if (options == 'destroy') {
                //移除resize事件
                $(window).off(".autogrowHeight");

                _elements.each(function () {
                    //移除初始化高度
                    $(this).attr('style', 'height');
                    //移除keyup，change，input，paste事件
                    $(this).off('.autogrow');
                }).data('bind-resize', 'false');
            }

            return _elements;
        }
    })

    $(function () {
        //初始化
        $("[data-fn~=resizable]").resizable();
    })

})(window.Zepto || window.jQuery || window.$);;(function ($) {
    $.extend($.fn, {
        /**
         * @method swipeToggle
         * @param {string} ['destroy']  禁用,默认为空
         * @param {string} ['swipeOut']  滑块滑出,默认为空
         * @param {string} ['swipeIn']  滑块滑入,默认为空
         *
         **/
        swipeToggle: function (options) {
            //参数为:'destroy'时，代码销毁。
            //参数为:'swipeOut'时，滑块滑出。
            //参数为:'swipeIn'时，滑块滑入。
            /*//参数数据类型为function时，执行此回调函数*/

            var swipeToggleElement = $(this),
                startX,
                startY,
                moveX,
                moveY,
                startTime,
                slideSpeed,    //滑动速度
            /*lastAWidth,    //最后一个a的宽度*/
                actionsWidth;  //包含a的容器的宽度

            if (swipeToggleElement.data('bind-swipeToggle') != 'true' && //判断是否已绑定touch事件
                options != 'destroy' &&
                options != 'swipeOut' &&
                options != 'swipeIn') {

                //避免重复绑定
                //点击其他地方，滑块滑入
                $(document).on('touchstart.sClose', function (event) {
                    var transform = $(event.target).closest('.swipeout').css('transform') || $(event.target).closest('.swipeout').css('-webkit-transform');
                    if (transform == 'none' || transform === undefined) {
                        moveX = 0;
                        swipeToggleElement.each(function () {
                            $(this).attr('style', '');
                            //$(this).find('.swipeout-actions-button a:last-child').attr('style', '');
                            $(this).data('swiper-in', 'false');
                        })
                    }
                })

                //绑定touch事件
                swipeToggleElement.each(function () {
                    $(this).on('touchstart.tStart', start);
                    $(this).on('touchmove.tMove', move);
                    $(this).on('touchend.tEnd', end);
                })

                //添加自定义属性
                swipeToggleElement.data('bind-swipeToggle', 'true');
            }

            function start(event) {

                $(this).removeClass('swiper-in');
                if (event.originalEvent) {   //jQuery
                    startX = event.originalEvent.targetTouches[0].clientX;
                    startY = event.originalEvent.targetTouches[0].clientY;
                } else {  //Zepto
                    startX = event.targetTouches[0].clientX;
                    startY = event.targetTouches[0].clientY;
                }
                startTime = new Date().getTime();
                actionsWidth = getWidth($(this).find('.swipeout-actions-button'));
                /*lastAWidth = getWidth($(this).find('.swipeout-actions-button a:last-child'));*/
                moveX = -actionsWidth;

            }

            function move(event) {
                if (event.originalEvent) {
                    moveX = event.originalEvent.targetTouches[0].clientX - startX;
                    moveY = event.originalEvent.targetTouches[0].clientY - startY;
                } else {
                    moveX = event.targetTouches[0].clientX - startX;
                    moveY = event.targetTouches[0].clientY - startY;
                }

                slideSpeed = moveX / (new Date().getTime() - startTime);

                moveX = $(this).data('swiper-in') == 'true' ? -actionsWidth + moveX : moveX;

                if (Math.abs(moveX) > Math.abs(moveY)) {

                    event.preventDefault();
                    $(this).attr('style', 'transform:translateX(' + moveX + 'px);-webkit-transform:translateX(' + moveX + 'px)');
                    if (moveX >= 0) {
                        $(this).attr('style', '');
                    }
                    if (-moveX >= actionsWidth) {
                        $(this).attr('style', 'transform:translateX(' + -actionsWidth + 'px);-webkit-transform:translateX(' + -actionsWidth + 'px)');
                    }

                    /*if (-moveX >= actionsWidth) {
                     $(this).find('.swipeout-actions-button a:last-child').addClass('swiper-in').attr('style', 'transform:translateX(' + -(actionsWidth - lastAWidth) + 'px);-webkit-transform:translateX(' + -(actionsWidth - lastAWidth) + 'px)');
                     } else {
                     $(this).find('.swipeout-actions-button a:last-child').addClass('swiper-in').attr('style', '');
                     }*/

                }
            }

            function end(event) {
                //根据滑动偏移量
                if (-moveX < actionsWidth / 2) {
                    swipeIn($(this));
                } else if (-moveX >= actionsWidth / 2) {
                    swipeOut($(this));
                }
                //根据滑动速度
                if (slideSpeed < -0.5) { //快速左滑
                    swipeOut($(this));
                } else if (slideSpeed > 0.5) { //快速右滑
                    swipeIn($(this));
                }
                slideSpeed = null;

                /* //执行回调函数
                 if (typeof options == 'function') {
                 var lastATransform = $(this).find('.swipeout-actions-button a:last-child').css('transform') || $(this).find('.swipeout-actions-button a:last-child').css('-webkit-transform');
                 if (lastATransform == 'translateX(' + -(actionsWidth - lastAWidth) + 'px)') {
                 options();
                 }
                 }*/
            }

            //滑块滑出
            function swipeOut(ele) {
                ele.each(function () {
                    $(this).addClass('swiper-in');
                    actionsWidth = getWidth($(this).find('.swipeout-actions-button'));
                    $(this).attr('style', 'transform:translateX(' + -actionsWidth + 'px);-webkit-transform:translateX(' + -actionsWidth + 'px)');
                    $(this).data('swiper-in', 'true');
                })
            }

            //滑块滑入
            function swipeIn(ele) {
                ele.each(function () {
                    $(this).addClass('swiper-in');
                    $(this).attr('style', '');
                    $(this).data('swiper-in', 'false');
                })
            }

            //获取宽度
            function getWidth(ele) {
                if ($ == window.Zepto) {
                    return ele.width();
                }
                if ($ == window.jQuery) {
                    return ele.outerWidth();
                }
            }

            if (options == 'destroy') {
                //销毁代码，删除事件
                $(document).off('touchstart.sClose');
                swipeToggleElement.each(function () {
                    $(this).off('touchstart.tStart');
                    $(this).off('touchmove.tMove');
                    $(this).off('touchend.tEnd');
                })

                swipeToggleElement.data('bind-swipeToggle', 'false');

            }

            if (options == 'swipeOut') {
                //滑块滑出
                swipeOut(swipeToggleElement);
            }

            if (options == 'swipeIn') {
                //滑块滑入
                swipeIn(swipeToggleElement);
            }
            return swipeToggleElement;
        }
    })

    $(function () {
        //初始化
        $('[data-fn~=swipeToggle]').swipeToggle();
    })
})(window.Zepto || window.jQuery || window.$);;(function($){
    $.extend($.fn, {
        photobrowser:function(options){
            options=options||{};

            var defaults={
                pagination:false,  //是否添加圆点儿样式
                navigationButton:false //是否添加左右点击按钮
            }
            var pb=this;
            var params = $.extend({},defaults,options);

            var sliderSettings={
                w:$(window).width(),
                swiper:$('.swiper-wrapper'),
                lens:$('.swiper-wrapper>div').length,
                prev:$('.photo-browser-prev'),
                next:$('.photo-browser-next'),
                back:$("a[data-back=back]"),
                photoview:$('.photo-view')
            };
            pb.init=function(lens){
                pb.attr('data-tag','true');
                //初始样式
                sliderSettings.swiper.css({'width':sliderSettings.w*lens});
                sliderSettings.swiper.find('div.swiper-slide').css('width',sliderSettings.w);
                //添加圆点
                if(params.pagination){
                    pb.pointSwiper(lens)
                }
                if (params.navigationButton) {
                    sliderSettings.swiper.next().removeClass('hide');
                }else{
                    sliderSettings.swiper.next().addClass('hide');
                }
            };
            //动态添加圆点
            pb.pointSwiper=function(lens){
                var txt='<ul class="pointSwiper">';
                for(var i=0; i<lens; i++){
                    txt+='<li></li>'
                }
                txt+='</ul>'
                sliderSettings.swiper.parent().append($(txt));
            };
            //当前的原点
            pb.addcur=function(current){
                sliderSettings.swiper.siblings('ul').children().eq(current).addClass('active').siblings().removeClass('active');
            };
            //点击时第几张默认显示
            pb.currentImg=function(i){
                sliderSettings.swiper.css('-webkit-transform', 'translateX('+-sliderSettings.w*i+'px)');
            }
            //滑动swiper-wrapper
            pb.slider=function(index,w,lrSwiper){
                var startDis,moveDis,number=index,startTime,
                    prev=sliderSettings.prev,
                    next=sliderSettings.next;

                var startHander=function(e){
                    e.preventDefault();
                    startTime=new Date()*1;
                    if(e.touches){
                        startDis=e.touches[0].pageX
                    }else{
                        startDis=e.originalEvent.touches[0].pageX;
                    }
                }
                var moveHander=function(e){
                    e.preventDefault()
                    if(e.touches){
                        moveDis=e.touches[0].pageX
                    }else{
                        moveDis=e.originalEvent.touches[0].pageX;
                    }
                    sliderSettings.swiper.css({
                        "-webkit-transition" : "0ms",
                        '-webkit-transform':'translate3d('+(-w*number+moveDis-startDis)+'px,0,0)'
                    });
                };
                var endHander=function(e){
                    e.preventDefault();
                    var Times=new Date()*1-startTime;
                    if(Times>100){
                        if(moveDis-startDis<-50){
                            _leftGo()
                        }else if(moveDis-startDis>50){
                            _rightGo()
                        }else{
                            sliderSettings.swiper.css({
                                '-webkit-transform': 'translate3d('+(-w*number)+'px,0,0)',
                                '-webkit-transition':'300ms'
                            });
                        }
                    }else{
                        sliderSettings.swiper.css({
                            '-webkit-transform': 'translate3d('+(-w*number)+'px,0,0)',
                            '-webkit-transition':'300ms'
                        });
                    }
                    pb.addcur(number);
                    sliderSettings.swiper.attr('data-target',number+1);
                }

                sliderSettings.swiper.on('touchstart.sliderstart',startHander);
                sliderSettings.swiper.on('touchmove.slidermove',moveHander);
                sliderSettings.swiper.on('touchend.sliderend',endHander);

                //判断左右按钮存在的情况下左右滑动
                sliderSettings.swiper.attr('data-target',number+1);
                if(params.navigationButton){
                    next.on('click',function(){
                        _leftGo();
                    })
                    prev.on('click',function(){
                        _rightGo();
                    })
                }

                //点击圆点滑动
                sliderSettings.swiper.siblings('ul').on('click','li',function(){
                    number=$(this).index();
                    pb.addcur(number);
                    sliderSettings.swiper.css({
                        '-webkit-transform':'translate3d('+-sliderSettings.w*(number)+'px,0,0)'
                    });
                    sliderSettings.swiper.css("-webkit-transition", "300ms");
                });

                //滑动下一张
                function _leftGo(){
                    number++;
                    var len=$('.swiper-wrapper>div').length;
                    sliderSettings.swiper.css("-webkit-transition", "300ms");
                    if(number>=len-1){
                        number=len-1;
                        sliderSettings.swiper.css('-webkit-transform','translate3d('+(-w*number)+'px,0,0)');
                        next.addClass('active-state');
                    }else{
                        sliderSettings.swiper.css('-webkit-transform','translate3d('+(-w*number)+'px,0,0)');
                        next.removeClass('active-state');
                        prev.removeClass('active-state');
                    }
                }

                //滑动上一张
                function _rightGo(){
                    number--;
                    sliderSettings.swiper.css("-webkit-transition", "300ms");
                    if(number<=0){
                        number=0;
                        sliderSettings.swiper.css('-webkit-transform','translate3d('+(-w*number)+'px,0,0)');
                        prev.addClass('active-state');
                    }else{
                        sliderSettings.swiper.css('-webkit-transform','translate3d('+(-w*number)+'px,0,0)');
                        prev.removeClass('active-state');
                        next.removeClass('active-state');
                    }
                }
            };
            pb.refresh=function(){
                var lens = $('.swiper-wrapper>div').length;
                var txt = "";
                var num = sliderSettings.swiper.attr('data-target');
                sliderSettings.swiper.css({'width':sliderSettings.w*lens});
                sliderSettings.swiper.find('div.swiper-slide').css('width',sliderSettings.w);

                for(var i=0; i<lens; i++){
                    if(i==num){
                        txt+='<li class="active"></li>'
                    }else{
                        txt+='<li></li>'
                    }
                }

                $('.pointSwiper').html(txt);
            };

            pb.open=function(index){
                var lens = sliderSettings.lens;

                index>lens?index=lens-1:index=index-1;

                pb.opeUrl('photoview');
                sliderSettings.photoview.removeClass('hide');
                sliderSettings.swiper.css('-webkit-transform','translate3d('+(-sliderSettings.w*index)+'px,0,0)');
                pb.slider(index,sliderSettings.w,params.lrSwiper);
                pb.addcur(index);

            };

            pb.close=function(){

                sliderSettings.swiper.off('touchstart.sliderstart');
                sliderSettings.swiper.off('touchmove.slidermove');
                sliderSettings.swiper.off('touchend.sliderend');
                sliderSettings.prev.removeClass('active-state');
                sliderSettings.next.removeClass('active-state');
                sliderSettings.swiper.removeAttr('data-target');
                pb.attr('data-tag','false');
                sliderSettings.photoview.addClass('hide');
                history.back();

            };

            pb.opeUrl=function(tag){
                sliderSettings.photoview.removeClass('hide');
                if (arguments.length === 1) {
                    history.pushState({page: tag}, "", "?page=" + tag);
                }
            };
            //初始化
            pb.init(sliderSettings.lens);
            window.addEventListener("popstate", function () {
                var currentState = {};
                //currentState = history.state;
                //解决UC不支持history.state的问题
                currentState.page = getQueryString('page');

                if (currentState.page == null) {
                    $('.photo-view').addClass('hide');

                } else if (currentState.page) {
                    $('.photo-view').removeClass('hide');
                }
            });
            if(pb.attr('data-tag')=='true') {
                pb.on('click', 'div', function () {
                    var h = $(window).height();
                    $('.swiper-container').css({"height":h-44+"px","margin-top":"22px"});
                    var idx = $(this).index(),
                            biglens = sliderSettings.lens - 1,
                            lens = sliderSettings.lens,
                            prev = sliderSettings.prev,
                            next = sliderSettings.next;

                    pb.attr('data-tag', 'true');
                    pb.open();

                    idx > biglens ? idx = biglens : idx = idx;
                    pb.addcur(idx)
                    //判断左右按钮是否存在
                    if (params.navigationButton) {

                        //初始化当点击小图片第一张和最后一张时按钮变色
                        if (idx === 0) {
                            prev.addClass('active-state');
                        } else if (idx == lens - 1) {
                            next.addClass('active-state');
                        } else {
                            prev.removeClass('active-state');
                        }
                    }
                    //点击图片小图显示当前大图
                    pb.currentImg(idx);
                    pb.slider(idx, sliderSettings.w, params.lrSwiper);
                });
                $("a[data-back=back]").on('click', function () {
                    pb.close()
                });
                //刷新
                if (options === "refresh") {
                    pb.refresh();
                }
                //打开某一张
                if (typeof options == "number") {
                    if ($('.pointSwiper').length === 0) {
                        pb.pointSwiper(sliderSettings.lens);
                    }
                    pb.open(options);
                }
                //关闭
                if (options === "close") {
                    pb.attr('data-tag', 'false');
                    pb.close();
                }
            }
            return pb;
        }
    })
    $(function(){
        //url状态响应
        var p = getQueryString('page');
        if (p) {
            $('.photo-view').removeClass('hide');
        }
    })

})(window.Zepto || window.jQuery || window.$);var MaxLength = function(){
    this.init();
};
MaxLength.prototype = {
    init: function(){
        var numberInp = $('input[type="number"]'),
            maxlength = numberInp.attr('maxlength');
        numberInp.on('keyup keypress keydown focus',function(){
            if(maxlength){
                if(numberInp.val().length>=maxlength){
                    numberInp.val(numberInp.val().substring(0,maxlength));
                }
            }
        });
    }
};
$(function(){
    var maxlength = new MaxLength();
});;;(function($,window){
    $.fn.view_3d = function(options){
        return new Drags(this,options);
    }
    var Drags = function(obj,opts){
        this._default = {
            frames: [],
            totalFrames: 0,
            pathPattern: null,
            curLoadImage: 0,
            after: new Function()
        };
        this.options = $.extend({},this._default,opts);
        this.element = obj;
        this.init();
    }
    Drags.prototype ={
        /**
         * 初始化
         */
        init: function(){
            this.element.find("img").addClass("main-frame");
            this.element[0].addEventListener('touchstart', this.bind(this._touchStart,this), false);
            this.element[0].addEventListener('touchmove',this.bind(this._touchMove,this), false);
            this.element[0].addEventListener('touchend',this.bind(this._touchEnd,this), false);
            this.loadImage();
        },
        /**
         * 修正函数作用环境
         * @param func
         * @param obj
         * @returns {Function}
         */
        bind: function(func, obj){
            return function(){
                return func.apply(obj,arguments);
            }
        },
        loadImage: function(force){
            var self = this;
            var imagePath = this.options.pathPattern.replace('#index#',this.options.curLoadImage + 1);
            if(force){//图片第一次加载失败后，重新再次加载一次
                imagePath += '?' + new Date().getTime();
            }
            var img = new Image();
            img.src = imagePath;
            $(img).load(function(){
                self.imageLoaded(img.src);
            }).error(function(){
                if(force){//如果已经重新加载一次了，仍加载失败
                    self.element.after('<div style="text-align:center;margin-top:50%;">加载失败，<a style="padding:10px;" href="' + window.location.href + '">请重试</a></div>');
                    if($('.ui-loading-block').length){
                        $('.ui-loading-block').addClass('hide');
                    }
                }else{//第一次加载失败，重新再次加载一次
                    self.loadImage(true);
                }
            });
        },
        imageLoaded: function(path){
            this.options.curLoadImage++;
            this.element.find('span.loading').text(Math.floor(this.options.curLoadImage / this.options.totalFrames * 100) + '%');
            this.options.frames.push(path);
            if(this.options.curLoadImage == this.options.totalFrames){
                this.start();
            }else{
                this.loadImage();
            }
        },
        start: function(){
            if($('.ui-loading-block').length){
                $('.ui-loading-block').fadeOut('slow');
            }
            var self = this;
            setTimeout(function(){
                self.element.find('img.main-frame').removeClass('hide');
            },600);
            this.element.addClass('draggable');
            this.element.find("img.main-frame").attr("src",this.options.frames[0]);
            this.curImg = 1;
        },
        _touchStart: function(e){
            e.preventDefault();
            this.element.addClass('draggable');
            this.startPointX = this._getPointer(e).x;
            this.startPointY = this._getPointer(e).y;
        },
        _touchMove: function(e){
            var self = this;
            this.endPointX = this._getPointer(e).x;
            this.endPointY = this._getPointer(e).y;
            var delX = this.endPointX - this.startPointX;
            var delY = this.endPointY - this.startPointY;
            this.startPointX = this.endPointX;
            this.startPointY = this.endPointY;
            if($('.draggable').length > 0) {
                if(Math.abs(delX) > Math.abs(delY)  && delX > 0){
                    if(this.curImg < this.options.totalFrames-1){
                        this.curImg++;
                        this.element.find("img.main-frame").attr("src", this.options.frames[this.curImg]);
                    }else{
                        this.curImg = 0;
                        this.element.find("img.main-frame").attr("src", this.options.frames[this.curImg]);
                    }
                }else if(Math.abs(delX) > Math.abs(delY)  && delX < 0){
                    if(this.curImg > 0) {
                        this.curImg--;
                        this.element.find("img.main-frame").attr("src", this.options.frames[this.curImg]);
                    }else{
                        this.curImg = this.options.totalFrames -1;
                        this.element.find("img.main-frame").attr("src", this.options.frames[this.curImg]);
                    }
                }
            }
        },
        _touchEnd: function(){
            $(this).removeClass('draggable');
        },
        _getPointer: function(e){
            var point = e.touches? e.touches[0]: e;
            return {x: point.pageX, y: point.pageY};
        }
    }
})(Zepto,window);;;(function($,undefined) {
	/**
	 *
	 * @param opt{Object} 实例化参数
	 * @constructor
	 */
	window.Loading = function(opt){
		this.loadActive = 0;
		this._default = {
			'before': new Function(),
			'after': new Function()
		}
		this.options = $.extend({},this._default,opt);
		this.timeout = undefined;
	};
	Loading.prototype = {
		/**
		 * 添加ajax注册事件（每个ajax调用之前都要执行该方法）
		 * @example
		 *
		 * javascript 代码
		 * <pre><code>
		 *	Loading.loadingStart();
		 * </pre></code>
		 *
		 * @method loadingStart
		 */
		loadingStart: function(){
			var self = this;
			if(this.loadActive++ === 0 && (typeof this.options.before == 'function')){
				this.options.before();
				if(this.timeout){
					clearTimeout(this.timeout);
				}
				//超过1分钟后加载动画仍显示，则有可能是用户少执行了loadingStop方法（排除网络差,ajax请求未完成的情况）
				this.timeout = setTimeout(function(){
					if(self.loadActive > 0){
						console.log('请检查是否缺少loadingStop');
					}
				},36000);
			}
		},
		/**
		 * 结束ajax注册事件(每个ajax调用返回成功之后都要执行该方法)
		 * @example
		 *
		 * javascript 代码
		 * <pre><code>
		 *	Loading.loadingStop();
		 * </pre></code>
		 *
		 * @method loadingStop
		 */
		loadingStop: function(){
			if(this.loadActive <= 0){
				toast('loadingStart与loadingStop不匹配');
			}
			if(--this.loadActive === 0 && (typeof this.options.after == 'function')){
				this.options.after();
				if(this.timeout){
					clearTimeout(this.timeout);
				}
			}
		}
	}
})(Zepto);;;(function($){
    $.fn.poSt = function(options){
        options = options<120? 120:options || 160;
        var $this = this;
        var deg = 90/($this.length-1);
        $.each($this,function(i){
            var y = (Math.sin(d2a(deg*i))*options).toFixed(2);
            var x = (Math.cos(d2a(deg*i))*options).toFixed(2);
            if($this.parent('.susButton-left').length){
                $($this[i]).css({left:x+'px',top:-y+'px'})
            }else {
                $($this[i]).css({left:-x+'px',top:-y+'px'})
            }
        })
    };
    /**
     * @method wheelMenu
     * @param {Number} [distance] 环形内容距离菜单按钮的距离
     * */
    $.fn.wheelMenu = function(r){
        var $this = this;
        $this.find('.ring').poSt(r);
        $this.on('click','.menu',function() {
            var rot = $this.data('rot')-180;
            $this.css('-webkit-transform','rotate('+rot+'deg)');
            $this.css('-moz-transform','rotate('+rot+'deg)');
            $this.css('-ms-transform','rotate('+rot+'deg)');
            $this.css('transform','rotate('+rot+'deg)');
            $this.find('.close').length!==0? $this.find('.menu').removeClass('close'):$this.find('.menu').addClass('close');
            $this.data('rot',rot);
        });
    };
    //角度转弧度
    function d2a(n){
        return n*Math.PI/180;
    }
})(Zepto);;;(function($,window,document){
    /**
     * 滚动加载数据组件初始化
     * @param [options] 实例化参数
     * @returns {MyScrollLoad}
     */
    $.fn.scrollLoad = function(options){
        return new MyScrollLoad(this, options);
    };
    var MyScrollLoad = function(element, options){
        this.$element = $(element);
        this.insertDOM = false;
        this.loading = false;
        this.topPull = true;
        this.bottomPull = true;
        this._default ={
            domUp : {// 上方拉动提示DOM
                domRefresh : '<div style="height: 40px;line-height: 40px;text-align: center;">↓下拉刷新</div>',
                domUpdate  : '<div style="height: 40px;line-height: 40px;text-align: center;">↑释放更新</div>',
                domLoad    : '<div style="height: 40px;line-height: 40px;text-align: center;"><span class="loading"></span>加载中...</div>'
            },
            domDown : {// 下方拉动提示DOM
                domRefresh : '<div style="height: 40px;line-height: 40px;text-align: center;">↑上拉加载更多</div>',
                domUpdate  : '<div style="height: 40px;line-height: 40px;text-align: center;">↓释放加载</div>',
                domLoad    : '<div style="height: 40px;line-height: 40px;text-align: center;"><span class="loading"></span>加载中...</div>'
            },
            loadUpFn : '',// 上方function
            loadDownFn : '' // 下方function
        };
        this.init(options);
    };
    MyScrollLoad.prototype = {
        init: function(options){
            var self = this;
            this.opts = $.extend({}, this._default, options);
            /**
             * 绑定触摸
             */
            this.$element.on('touchstart',function(e){
                self._start(e);
            });
            this.$element.on('touchmove',function(e){
                self._move(e);
            });
            this.$element.on('touchend',function(){
                self._end();
            });
        },
        /**
         * 获取坐标
         * @param options
         */
        getPoint: function(e){
            var point = e.touches ? e.touches[0] : e;
            return{x: point.pageX, y: point.pageY};
        },
        _start: function(e){
            this.startPos = this.getPoint(e);
            // 判断滚动区域
            this._meHeight = $(window).height();
            this._childrenHeight = $(document).height();
            this._scrollTop = $(window).scrollTop();
        },
        _move: function(e){
            this.endPos = this.getPoint(e);
            this._moveY = this.endPos.y - this.startPos.y;
            if(this._moveY > 0){
                this.direction = 'down'; //向下拉动
            }else if(this._moveY < 0){
                this.direction = 'up';//向上拉动
            }
            var _absMoveY = Math.abs(this._moveY);
            // 加载上方
            if(this.opts.loadUpFn !== '' && this._scrollTop <= 0 && this.direction == 'down' && this.topPull){
                e.preventDefault();
                if(!this.insertDOM){
                    this.$element.prepend('<div class="dropload-up" style="position: relative;"></div>');
                    this.insertDOM = true;
                }
                this._offsetY = _absMoveY * 0.2;
                if( this._offsetY <= 20){
                    $('.dropload-up').html('').append(this.opts.domUp.domRefresh);
                }else if( this._offsetY > 20 &&  this._offsetY <= 40){
                    $('.dropload-up').html('').append(this.opts.domUp.domUpdate);
                }
                this.$element.css('top',this._offsetY - 40 + 'px');
            }
            // 加载下方
            if(this.opts.loadDownFn !== '' && this._childrenHeight <= (this._meHeight+this._scrollTop) && this.direction == 'up' && this.bottomPull){
                e.preventDefault();
                if(!this.insertDOM){
                    this.$element.append('<div class="dropload-down" style="position: relative;"></div>');
                    this.insertDOM = true;
                }
                this._offsetY = _absMoveY * 0.2;
                if( this._offsetY <= 40){
                    $('.dropload-down').html('').append(this.opts.domDown.domRefresh);
                }else if( this._offsetY > 40 &&  this._offsetY <= 80){
                    $('.dropload-down').html('').append(this.opts.domDown.domUpdate);
                }
                this.$element.css('top',- this._offsetY + 'px');
            }
        },
        _end: function(){
            var _absMoveY = Math.abs(this._moveY);
            if(this.insertDOM){
                if(this.direction == 'down'){
                    this.$domResult = $('.dropload-up');
                    this.domLoad = this.opts.domUp.domLoad;
                }else if(this.direction == 'up'){
                    this.$domResult = $('.dropload-down');
                    this.domLoad = this.opts.domDown.domLoad;
                }
                if(this._offsetY > 40){
                    this.$domResult.html('').append(this.domLoad);
                    this.loading = true;
                    if(this.opts.loadUpFn !== '' && this.direction == 'down'){
                        this.opts.loadUpFn(this);
                    }else if(this.opts.loadDownFn !== '' && this.direction == 'up'){
                        this.opts.loadDownFn(this);
                    }
                }else{
                    this.$domResult.remove();
                    this.$element.css('top',0);
                    this.insertDOM = false;
                }
            }
        },
        /**
         * 设置禁用/可用 状态
         * @example
         * javascript代码:
         *<pre><code>
         * scroller.setScrollLoad('disableAll');//禁用底部和顶部拉动
         * scroller.setScrollLoad('enableAll');//开启顶部和底部拉动
         * scroller.setScrollLoad('disableBottom');//禁用底部拉动
         * scroller.setScrollLoad('enableBottom');//开启底部拉动
         * scroller.setScrollLoad('disableTop');//禁用顶部拉动
         * scroller.setScrollLoad('enableTop');//开启顶部拉动
         *</pre></code>
         * @method setScrollLoad
         * @param state{String} 禁用或可用状态
         *
         */
        setScrollLoad : function(state){
            switch (state){
                case "disableAll":
                    this.bottomPull = false;
                    this.topPull = false;
                    break;
                case "enableAll":
                    this.bottomPull = true;
                    this.topPull = true;
                    break;
                case 'disableBottom':
                    this.bottomPull = false;
                    break;
                case 'enableBottom':
                    this.bottomPull = true;
                    break;
                case 'disableTop':
                    this.topPull = false;
                    break;
                case 'enableTop':
                    this.topPull = true;
                    break;
            }
        },
        /**
         * 重置
         * @example
         * javascript代码:
         *<pre><code>
         * scroller.resetload();
         *</pre></code>
         * @method resetload
         */
        resetload: function(){
            if(this.$domResult.length){
                this.$domResult.remove();
                this.$element.css('top',0);
            }
            this.loading = false;
            this.insertDOM = false;
        }
    };
})(window.Zepto,window,document);;(function($){

    $.fn.sliderNav = function(options){
        var initials = [];
        var defaults = {
            element:'.sliderNav-bar',
            items:initials,
            mode:true
        };
        var opts = $.extend(defaults, options);
        var o = $.meta ? $.extend({}, opts, $$.data()) : opts;
        $(o.element).each(function(i){
            var initial= $(this).html().charAt(0).toLowerCase();
            if(initials.indexOf(o.items)===-1)initials.push(initial);
        });
        $.each(o.items,function(index,val){
            $('.slider-nav').append('<a alt="#'+val+'">'+val+'</a>');
        });
        return new SliderNav(o.element,o.mode);
    };
    var SliderNav = function(element,mode){
        this._slideEvent(mode);
        this.mode = mode;
        this.element = element;
    };
    SliderNav.prototype = {
        _slideEvent:function(){
            var _this = this, el = $('.slider-nav');
            var lTop = (el.height()-el.find('a').height()*el.find('a').length)/2;
            el.on('touchmove touchstart',function(e){
                _this._slideMove($(this),e,lTop);
            });
            el.on('touchend',function(){
                var timer = setTimeout(function(){
                    $('.selectedCenter').remove();
                    clearTimeout(timer);
                },500)
            })
        },
        _slideMove:function(_this,e,lTop){
            e.preventDefault();
            var touch = e.targetTouches[0];
            var self=this;
            if(e.targetTouches.length==1){
                var i = parseInt((touch.clientY-lTop)/_this.find('a').height());
                i<=0? i=0: (i>_this.find('a').length-1? i=_this.find('a').length-1: i);
                var target = $('.slider-content '+this.element).eq(i);
                var cOffsetTop = $('.slider-content').offset().top;
                var lOffsetTop = target.offset().top;
                var pScroll = lOffsetTop - cOffsetTop;
                self._slideMode(target);
                $(window).scrollTop(pScroll);
            }
        },
        _slideMode:function(target){
            if(this.mode){
                $(target).addClass('selected').siblings().removeClass('selected');
            }else {
                target = target.attr('id');
                var mode2 = '<div class="selectedCenter">'+target+'</div>';
                $('.selectedCenter').remove();
                $('body').append(mode2);
            }
        }
    };
})(Zepto);
(function($,window,document){
    /**
     * 纵向滚动到底部/横向滚动到右侧 加载更多数据
     * @method setScrollLoad
     * @param [options] 实例化参数,具体的参数如下:<br/>
     *  。"direction"（默认为：portrait）:滚动的方向(portrait纵向滚动和landscape横向滚动)<br/>
     * 	。"scrollAjax" (默认为true): 是否用内置的ajax(true:表示用内置的ajax,false:表示不用内置的ajax)<br/>
     * 	。"scrollAjaxUrl": 内置ajax 请求路径 <br/>
     * 	。"scrollAjaxType": 内置ajax 请求方式，默认为'GET'方法 <br/>
     * 	。"scrollAjaxDataType": 内置ajax 返回数据类型' <br/>
     * 	。"scrollAjaxData": 内置ajax 请求参数' <br/>
     * 	。"scrollAjaxSuccess": 内置ajax 请求成功时的回调函数' <br/>
     * 	。"scrollAjaxError": 内置ajax 请求失败时的回调函数' <br/>
     * 	。"content": 如果不用内置的ajax时，执行的回调函数(即:scrollAjax设为false后，会执行这个回调函数)' <br/>
     */
    $.fn.endlessScroll = function(options){
        return new EndlessScroll(this,options);
    }
    var EndlessScroll = function(element,options){
        this.ele = element || $(window);
        if(element[0] == document){
            this.ele = $(window);
        }
        this.scrollAjaxLocker = true;// 是否允许加载数据
        this.defaults = {
            scrollAjax: true,
            scrollAjaxUrl: '',
            scrollAjaxType: 'GET',
            scrollAjaxDataType: 'json',
            scrollAjaxData:{},
            scrollAjaxSuccess: '',
            scrollAjaxError: '',
            content:'',
            direction: 'portrait' //默认纵向
        };
        this.options = $.extend({},this.defaults, options);
        this.init();
    }
    /**
     * 初始化
     */
    EndlessScroll.prototype.init = function(){
        var self = this;
        this.ele.on('scroll',function(){
            switch(self.options.direction){
                case 'portrait':
                    self.scrollAjaxLocker && self.verticalScroll();
                    break;
                case 'landscape':
                    self.scrollAjaxLocker && self.horizentalScroll();
                    break;
                default:
                    alert('direction 参数值有误');
            }
        })
    }
    /**
     *  判断纵向是否滚动到底部
     */
    EndlessScroll.prototype.verticalScroll = function(){
        var scrolltop = this.ele.scrollTop();//获取垂直滚动的距离(滚动到的当前位置)
        var scrollheight = this.ele.height();//可见高度（浏览器高度或者div高度）
        var contentH = this.ele[0] == window ? $(document).height() : this.ele[0].scrollHeight;//获取整个页面的高度
        if(contentH  <= scrolltop + scrollheight){//表明已经滚到页面底端了
            this.hasContent();
        }
    }
    /**
     * 判断横向是否滚动到右侧
     */
    EndlessScroll.prototype.horizentalScroll = function(){
        var scrollLeft = this.ele.scrollLeft();//获取水平滚动条的距离
        var scrollWidth = this.ele.width();//可见宽度
        var contentW = this.ele[0].scrollWidth;
        if(contentW <= scrollLeft + scrollWidth){
            this.hasContent();
        }
    }
    /**
     * 判断是否用内置的ajax
     */

    EndlessScroll.prototype.hasContent = function(){
        if(this.options.scrollAjax && this.options.scrollAjaxUrl){
            this.scrollAjax();
        }else{
            if( typeof this.options.content == 'function'){
                this.options.content();
            }
        }
    }
    /**
     * 内置ajax实现
     */
    EndlessScroll.prototype.scrollAjax = function(){
        var self = this;
        $.ajax({
            url: this.options.scrollAjaxUrl,
            type: this.options.scrollAjaxType,
            dataType: this.options.scrollAjaxDataType,
            data: this.options.scrollAjaxData,
            beforeSend: function(){
                self.scrollAjaxLocker = false; //ajax 上锁
            },
            success: function(data){
                if( typeof self.options.scrollAjaxSuccess == 'function'){
                    var state = self.options.scrollAjaxSuccess(data,self.options.scrollAjaxData);
                    if(state === false){
                        return;
                    }else if(state == Object){
                        self.options.scrollAjaxData = state;
                    }
                    self.scrollAjaxLocker = true;//ajax 解锁
                }
            },
            error: function(e){
                if( typeof self.options.scrollAjaxError == 'function'){
                    self.options.scrollAjaxError(e);
                }
                self.scrollAjaxLocker = true;//ajax 解锁
            }
        })
    }
})(window.Zepto,window,document);

