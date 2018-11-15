var SYSTEM = (function(mod,$,undefined){
    /**
     *
     * 页面适配
     */
    mod.initWindow = function(){
        var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
        document.documentElement.style.fontSize = deviceWidth / 6.4 + "px";//如果设计图是320的话就除以3.2
    }
    $(window).resize(function(){
        mod.initWindow();
    });
    return mod;
})(SYSTEM || {}, window.Zepto);
var TAKEORDER = (function (mod, $, undefined) {
    mod.initEvent = function () {
        /*弹出框*/
        $('.sure').on('click', function () {
            $('.check').addClass('hide');
            //$('.settlement .settlement-btn').toggleClass('active');
        })
    }
    return mod;
})(TAKEORDER || {}, window.Zepto);
/*程序入口*/
$(function () {
    SYSTEM.initWindow();
    TAKEORDER.initEvent();
});
