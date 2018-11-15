var PRINTORDER = (function(mod,$,undefined){
    mod.initEvent = function(){
        /*滚动条*/
        $(".one-three").niceScroll({
            cursorcolor:"#294b6e",
            cursoropacitymax:1,
            touchbehavior:false,
            cursorwidth:"4px",
            cursorborder:"0",
            cursorborderradius:"2px",
            autohidemode: false
        });

    }
    return mod;
})(PRINTORDER || {},window.jQuery);
/*程序入口*/
$(function () {
    PRINTORDER.initEvent();
});

