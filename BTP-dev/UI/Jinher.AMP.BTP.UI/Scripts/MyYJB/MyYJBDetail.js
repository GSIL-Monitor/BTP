var flex = function(){
    var deviceWidth = document.documentElement.clientWidth>500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function(){
    flex();
};
var MYYJB = (function (mod, $, undefined) {
    
    return mod;
})(MYYJB || {}, window.Zepto);

$(function () {
    MYYJB.initEvent();
});
