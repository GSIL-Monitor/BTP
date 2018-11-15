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


window.ORDERDETAIL = (function (mod, $, undefined) {
    /*删除订单*/
    mod.cancelOrder = function () {
        $('.cancel-order').on('click', function () {
            $('.cancel-order-tip').removeClass('hide');
        })
        $('.cancel').on('click', function () {
            $('.cancel-order-tip').addClass('hide');
        })
    }
    return mod;
})(window.ORDERDETAIL || {}, window.Zepto);


/**
 * 程序入口
 */
$(document).ready(function(){
    SYSTEM.initWindow();
    ORDERDETAIL.cancelOrder();
});
