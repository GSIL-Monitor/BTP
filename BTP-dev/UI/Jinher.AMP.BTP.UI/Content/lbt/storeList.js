/**
 * Created by jinshuaihua on 18/6/19.
 */
/*页面适配*/
var flex = function(){
    var deviceWidth = document.documentElement.clientWidth>500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};

var INDEX = (function(mod,zepto,undefined){
    /**
     * 初始化事件
     */
    mod.initEvent = function(){
        window.onresize = function(){
            flex();
        };
        $(window).on('scroll',function(){
            var top = $(window).scrollTop();
            if(top > 400){
                $('.goTop').removeClass('hide');
            }else{
                $('.goTop').addClass('hide');
            }
            if(top>$('#swiper-wrapper').height()){
                $('.nav-header').addClass('fixed');
            }else{
                $('.nav-header').removeClass('fixed');
            }
        });
        $('.goTop').on('click',function(){
            $(window).scrollTop(0);
        });
    };
    return mod;

})(window.INDEX || {},window.Zepto);
/**
 * 程序入口
 */
$(document).ready(function(){
    flex();//页面适配
    INDEX.initEvent();

});