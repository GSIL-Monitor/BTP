//每个页面的唯一标识，要求为整数，目前没有用，所以都传1
//var _pageId = Math.random() + "";
var _pageId = "1";

function JhWebviewBusiObj(functionName, business, businessType, startWebviewVersion) {
    var functionNameData = functionName;
    var businessData = business;
    var businessTypeData = businessType;
    var starVersion = startWebviewVersion;

    this.ToString = function () {
        var obj = {
            businessJson: businessData,
            businessType: businessTypeData
        };
        return JSON.stringify(obj);
    };
    this.sendToMobile = function () {
        if (!functionNameData || !businessData || !businessTypeData) {
            return;
        }
        //老版本交互
        if (!isNewerJhWebview(starVersion)) {
            if (typeof oldFunc == "function") {
                oldFunc(oldFuncParams);
            }
            return;
        }
        var base64 = new Base64();
        var obj = {
            businessJson: JSON.stringify(businessData),
            businessType: businessTypeData
        };
        var bussStr = base64.encode(JSON.stringify(obj));
        var tagStr = base64.encode(_pageId);
        window.location.href = "jhoabrowser://" + functionNameData + "?args=" + bussStr + "&tag=" + tagStr;
        return;
    };
}

function mobileShareMenu(isShowButton) {
    var functionName = "setMoreButtonShowOrHidden";
    var startWebviewVersion = "1.0.0";
    var obj = {
        ShowMoreButton: isShowButton ? 1 : 0,
        ShowItemViews: "more_share",
        HiddenItemViews: "copy_url,open_url"
    };
    var businessType = 1;
    new JhWebviewBusiObj(functionName, obj, businessType, startWebviewVersion).sendToMobile();
}
 