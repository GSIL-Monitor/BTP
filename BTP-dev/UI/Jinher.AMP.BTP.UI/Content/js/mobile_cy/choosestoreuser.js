
/* 页面全集变量*/
//门店项模板html.
var _siTemplateHtml = "";
//当前页码
var _pageIndex = 1;
//每页加载数据条数。
var _pageSize = 20;

//是不是最后一页。
var _isLastPage = false;
//门店信息列表。
var _storeData = new Array();
//当前位置。
var _position = new Object();
//高德地图获取当前位置。
var map, geolocation;
////构造路线导航类
var driving;
//当前选中的门店
var _storeId = null; //门店ID
var _appId = null;   //馆ID
var _iscoupon = null; //是否优惠券页面
var _comefirst = null; //是否第一次进来

/* 页面初始化*/
$(function () {
    _storeId = getQueryString("storeId");
    _appId = getQueryString("appId");
    _iscoupon = getQueryString("iscoupon");
    _comefirst = getQueryString("comefirst");
    sessionStorage.appId_CY = _appId;
    _siTemplateHtml = $("#divStoreItemTemplate").html();

    $("#btnReloadData").on("click", function () {
        document.location.href = "";
    });    
    
    ajaxLoadingSingle.show(); //显示加载地理信息
    initScroller();

    initGaoDeMap();
    //GetStoreByLocation(0); //不调转
});


function GetUserPosition() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(geo_success, geo_error);
    }

}

function stopPage(e) {
    e = e || window.event;
    if (e.stopPropagation) { //W3C阻止冒泡方法
        e.stopPropagation();
    } else {
        e.cancelBubble = true; //IE阻止冒泡方法
    }
}
function goPage(obj) {
    var storeId = $(obj).attr("storeId");
    var storeInfo = _storeData.find(function (item) {
        return item.store.Id.toLocaleLowerCase() == storeId.toLocaleLowerCase();
    });
    if (storeInfo == null) {
        return;
    }
    if (checkIsNullOrEmpty(_iscoupon)) { //查看优惠券
        goCoupon(storeInfo[0]);
    }
    else {  //点餐
        removeActive();
        $(this).addClass("active");
        goFoodMenu(storeInfo[0]);
    }
}

function removeActive() {
    var storeItems = $("#divStoreContainer").find('div[tag="storeItem"]');
    storeItems.each(function () {
        $(this).removeClass("active");
    });
}
//显示菜单列表
function goFoodMenu(storeInfo) {
    sessionStorage.storeInfo_CY = JSON.stringify(storeInfo);
    var _shopId = sessionStorage.appId = storeInfo.setting.CateringSetting.AppId;
    var storeId = storeInfo.store.Id;
    document.location.href = "/Mobile/CYCommodityList?appId=" + _appId + "&shopId=" + _shopId + "&storeId=" + storeId;
}
//显示优惠券列表
function goCoupon(storeInfo) {
    sessionStorage.storeInfo_CY = JSON.stringify(storeInfo);
    var _shopId = sessionStorage.appId = storeInfo.setting.CateringSetting.AppId;
    var _esAppId = sessionStorage.appId_CY;
    var _storeId = storeInfo.store.Id;
    var url = "http://" + getPreCYDomain() + "coupon.iuoooo.com/CouponList/Index?from=cy&esAppId=" + _esAppId + "&shopId=" + _shopId;
    url = addSessionToUrl(url);
    var srcurl = getCYDomain() + "/Mobile/CYCommodityList?iscoupon=1&appId=" + _esAppId + "&shopId=" + _shopId + "&storeId=" + _storeId;
    url += "&srcUrl=" + encodeURIComponent(srcurl);
    document.location.href = url;
    //  window.location = srcUrl + "&go=detail/list"
}

/* 异步加载业务数据*/
//获取门店列表。
function GetStoreByLocation(from) {
    getDataAjax2({
        url: '/Mobile/GetStoreByLocation',
        type: 'post',
        data: { AppId: getQueryString("appId"), Longitude: _position.getLng, Latitude: _position.getLat, CurrentPageIndex: _pageIndex, PageSize: _pageSize },
        beforeSend: function () {
            //ajaxLoading(1, '');
        },
        complete: function () {
            ajaxLoadingSingle.hide();
            //ajaxLoading(1, ''); 
        },
        callback: function (data) {
            ajaxLoadingSingle.hide();
            //没有数据。
            if (_pageIndex == 1 && (data.result = false || data.stores == null || data.stores.length == 0)) {
                $("div.no-data").removeClass("hide");
                $("#divStoreContainer").hide();
            }
            else {
                $("div.no-data").addClass("hide");
                $("#divStoreContainer").show();
            }

            if (data.stores != null && data.stores.length > 0) {
                _storeData = _storeData.concat(data.stores);
                if (data.stores.length < _pageSize) {
                    _isLastPage = true;
                }
            }
            else {
                _isLastPage = true;
            }
            if (isComeFirst(from)) return;
            var html = getStoreList(data.stores);
            $("#divStoreContainer").append(html);
        },
        error: function (date, text) {
            ajaxLoadingSingle.hide();
        }
    });
}

function isComeFirst(from) {
    if (from != 1) return false;
    if (!checkIsNullOrEmpty(_comefirst) && !checkIsNullOrEmpty(_iscoupon) && _storeData != null && _storeData.length > 0) {
        goFoodMenu(_storeData[0]);
        return true
    }
    return false
}

function getStoreList(data) {
    var storeHtml = "";
    if (data == null || data.length == 0) {
        return storeHtml;
    }
    for (var i = 0; i < data.length; i++) {
        var storeinfo = data[i];
        var storedetail = storeinfo.store;
        if (checkIsNullOrEmpty(_storeId)) {
            storedetail.Active = storedetail.Id.toLocaleLowerCase() == _storeId.toLocaleLowerCase() ? "active" : "";
        }
        else {
            // storedetail.Active = i == 0 ? "active" : "";
            storedetail.Active = "";
        }
        if (checkIsNullOrEmpty(_iscoupon)) storedetail.Active = "";
        storedetail.Phone1 =GetPhoneVal(storedetail.Phone);
        storedetail.Phone2 = GetPhoneText(storedetail.Phone);
        storedetail.PhoneCount = storedetail.Phone.length;
        storedetail.DistanceText = storedetail.Distance > 1000 ? (storedetail.Distance / 1000).toFixed(2) + "km" : storedetail.Distance.toFixed(0) + "m";
        storedetail.PhoneCountDisplay = storedetail.PhoneCount > 1 ? "inline-block" : "none";
        storedetail.srckey = "src";
        storeHtml += _siTemplateHtml.format(storedetail);
    }
    return storeHtml;
}

function GetPhoneVal(arr) {
    var _p = [];
    for (var i = 0; i < arr.length; i++) {
        _p.push(arr[i].PhoneNumber);
    }
    return _p.join(",");
}
function GetPhoneText(arr) {
    return arr[0].PhoneNumber + (arr.length > 0 ? "..." : "");
}
/* 上拉刷新，下拉更多加载数据*/
function initScroller() {
    //滚动加载
    var scroller = $('#scroller').scrollLoad({
        loadDownFn: function (me) { //下拉加载数据
            if (!_isLastPage) {
                _pageIndex++;
                GetStoreByLocation(0);
            }
            /*在这添加数据*/
            me.resetload();
        },
        loadUpFn: function (me) {//上拉刷新
            _pageIndex = 1;
            _isLastPage = false;
            $("#divStoreContainer").html("");
            GetStoreByLocation(0);
            me.resetload();
        }
    });
}

/*数据加载效果*/
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
} ());

/* 页面模板数据格式 公共方法*/
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

function initGaoDeMap() {
    //加载地图，调用浏览器定位服务
    map = new AMap.Map('container', {
        resizeEnable: true
    });
    map.plugin('AMap.Geolocation', function () {
        geolocation = new AMap.Geolocation({
            enableHighAccuracy: true, //是否使用高精度定位，默认:true
            timeout: 10000,          //超过10秒后停止定位，默认：无穷大
            buttonOffset: new AMap.Pixel(10, 20), //定位按钮与设置的停靠位置的偏移量，默认：Pixel(10, 20)
            zoomToAccuracy: true,      //定位成功后调整地图视野范围使定位位置及精度范围视野内可见，默认：false
            buttonPosition: 'RB'
        });
        map.addControl(geolocation);
        geolocation.getCurrentPosition();
        AMap.event.addListener(geolocation, 'complete', onComplete); //返回定位信息
        AMap.event.addListener(geolocation, 'error', onError);      //返回定位出错信息
    });
}

/* 高德地图业务*/
//解析定位结果
function onComplete(data) {    
    if (_position.getLng) {
        return;
    }
    if (data&&data.position) {
        _position.getLng = sessionStorage.currentUserLongitude_CY = data.position.getLng();
        _position.getLat = sessionStorage.currentUserLatitude_CY = data.position.getLat();
    }
    GetStoreByLocation(1);
}
//解析定位错误信息
function onError(data) {    
    //h5再次获取地理位置    
    DoDefaut();
}

function geo_success(position) {
    _position.getLng = sessionStorage.currentUserLongitude_CY = position.coords.longitude;
    _position.getLat = sessionStorage.currentUserLatitude_CY = position.coords.latitude;
}

function geo_error(msg) {
    DoDefaut();
}
function DoDefaut() {
    if (_position.getLng) {
        ajaxLoadingSingle.hide();
        return;
    }
    //默认西部马华巨山店
    //http://restapi.amap.com/v3/ip?key=8c2706d30e1dbe1b52b870bc5b09d732&ip=114.247.50.2获取城市定位
    _position.getLng = sessionStorage.currentUserLongitude_CY = "116.2350300000"; //116.391278
    _position.getLat = sessionStorage.currentUserLatitude_CY = "39.9583680000"; //39.907472    
    GetStoreByLocation(1); //不调转
}
