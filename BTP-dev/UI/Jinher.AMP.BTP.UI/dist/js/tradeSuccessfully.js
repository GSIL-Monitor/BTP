/**
 * Created by jinshuaihua on 18/6/11.
 */

var flex = function(){
    var deviceWidth = document.documentElement.clientWidth>500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function(){
    flex();
};
$(function () {
    regeditEvents();
});

//×¢²áÊÂ¼þ
function regeditEvents() {
    $("#GoOrderDetail").on("click", function () {
        document.location.href = getQueryString("backUrl");
    });
    $("#GoTrade").on("click", function () {
        var pObj = new Object();
        pObj.productId = getQueryString("commodityid");
        pObj.appId = getQueryString("appId2");
        pObj.userId = getQueryString("userid");
        pObj.businessId = getQueryString("businessid");
        pObj.ordercode = getQueryString("ordercode");
        pObj.esappid = getQueryString("esappid");
        pObj.orderid = getQueryString("orderid");
        var reviewUrl = snsURL + "/EvaluateNew/index?commodityId={productId}&appId={appId}&userid={userId}&businessid={businessId}&ordercode={ordercode}&esappid={esappid}&orderid={orderid}";
        window.location.href = reviewUrl.format(pObj);
    });
};
function GoCommodityDetail(comdtyId, appId) {
    var url = "/Mobile/CommodityDetail"
            + "?commodityId=" + comdtyId + "&source=share&producttype=webcjzy&linkmall=1&type=tuwen&srctype=34&thiszpho2o=1&isshowsharebenefitbtn=1&appId=" + appId;
    window.location.href = url;
}