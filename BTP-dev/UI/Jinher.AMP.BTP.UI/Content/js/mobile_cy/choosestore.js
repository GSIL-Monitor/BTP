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
var CHOOSESTORE = (function(mod,$,undefined){
    mod.initEvent = function(){
        /*点击支付提示框*/
        $('.address-list').on('click','.address-info', function () {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');
        })
       
    }
    return mod;
})(CHOOSESTORE || {},window.Zepto);
/*程序入口*/
$(function () {
    SYSTEM.initWindow();
    CHOOSESTORE.initEvent();
});
