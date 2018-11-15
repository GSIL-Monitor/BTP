//设置当前域为顶级域
var arr = window.location.host.split('.');
if (arr.length > 0) {
    document.domain = arr[arr.length - 2] + "." + arr[arr.length - 1];
}
var _pageId = Math.random() + "";