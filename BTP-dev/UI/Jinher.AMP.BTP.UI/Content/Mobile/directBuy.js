//直接购买逻辑

function getQueryString(name, str) {
    var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}
 

var ajaxLoadingSingle = (function () {
    function initLoading() {
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length > 0) {
            return;
        }
        //蒙版
        blind = $('<div></div>');
        //蒙版相关css
        blind.css({
            'position': 'fixed',
            'z-index': '10000',
            'opacity': 0.2,
            'backgroundColor': '#ccc',
            'height': '100%',
            'width': '100%',
            'top': 0,
            'left': 0
        });
        //蒙版ID值
        blind.attr('id', 'ajaxLoadBlind');

        //ajaxLoading盒子ID对象
        var loading = $('#ajaxLoading_img');
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_img');
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });
        loading.append('<img src="/Content/images/ajax-loader.gif" />');
        //blind.css({ 'position': 'relative' });
        blind.append(loading);
        $('body').append(blind);
    }

    function show() {
        initLoading();
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length == 0) {
            return;
        }
        blind.show();
    }
    function hide() {
        var blind = $('body').find("#ajaxLoadBlind");
        if (blind.length == 0) {
            return;
        }
        blind.hide();
    }

    var loadingSingle = new Object();
    loadingSingle.show = show;
    loadingSingle.hide = hide;
    return loadingSingle;
} ())




//加载商品详情数据
function loadCommodityDetails(fnCallback) {
    //设置获取商品详情请求参数
    //appId已改变语义，实际传递esAppId
    var data = {
        appId: sessionStorage.esappid,
        commodityId: getQueryString('commodityId'),
        freightTo: sessionStorage.province,
        outPromotionId: getQueryString('outPromotionId')
    };

    $.ajax({
        url: '/Mobile/GetCommodityDetailsZPH',
        type: 'post',
        data: data,
        beforeSend: function () {
            ajaxLoadingSingle.show();
        },
        complete: function () {
            ajaxLoadingSingle.hide();
        },
        success: function (data) {
            ajaxLoadingSingle.hide();
            fnCallback && fnCallback(data);
        },
        error: function (date, text) {
            ajaxLoadingSingle.hide();
        }
    });
}
//商品详情数据。
var _commodityDetailData = new Object();
//显示拼团详情。
function showDiyGroupDetail(data) {
    if (!data || data.ResultCode != 0) {
        noCommodityDetail();
    }
}

$(function () {
    //加载商品详情数据
    loadCommodityDetails(function (data) {
        _commodityDetailData = $.extends(_commodityDetailData,data.Data);
        showDiyGroupDetail(data);
    });
});

//多属性商品属性选择器
var multiAttributeSelector = (function () { 

 }());

