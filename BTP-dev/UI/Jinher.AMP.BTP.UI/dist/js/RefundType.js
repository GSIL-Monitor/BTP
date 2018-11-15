var flex = function () {
    var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function () {
    flex();
};
function initEvent() {
    $('.modal-bd').on('click', '.list', function () {
        $('#modal').addClass('hide');
        var value = $(this).text();
        $('#SelectExpCo').text(value);
        if(value == '其他')
            $("#elseContent").show();
        else
            $("#elseContent").hide();
    });
    $('.logistics').on('click', function () {
        $('#modal').removeClass('hide');
    });
    $('.close').on('click', function () {
        $('#modal').addClass('hide');
    });
}
$(document).ready(function () {
    initEvent();
});

var tempType = "";
//是否是是售后：0不是 1是。
var _isAfterSale = 0;
$(function () {
    saveContextDTOByUrl();
    tempType = getQueryString("type");
    _isAfterSale = getQueryString("isAfterSale");
    _isAfterSale = parseInt(_isAfterSale);
});

//JS验证 不是有效的值
function JsVilaDataNull(obj) {
    if (obj != undefined && obj != "" && obj != "null" && obj != null && obj != "undefined" && obj) {
        return true;
    } else {
        return false;
    }
}

function btnSubmit() {
    $("#confirm").html("正在提交...");
    $("#confirm").attr("disabled", "");


    var selectExpCo = $.trim($("#SelectExpCo").text());
    var expCo = $.trim($("#ExpCo").val());
    if ((selectExpCo == '请选择' || selectExpCo == "其他") && expCo == "") {
        if (selectExpCo == '请选择') {
            toast("请选择物流公司！");
        } else {
            toast("请填写物流公司！");
        }
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    } else if (selectExpCo == '请选择' || selectExpCo == "其他") {
        selectExpCo = expCo;
    }


    var expNo = $("#ExpNo").val();
    if (expNo == "") {

        toast("请填写物流单号！");
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    }

    var rtUrl = _isAfterSale == 1 ? '/Mobile/SaveRefundTypeAfterSales' : '/Mobile/SaveRefundType';
    getDataAjax2({
        url: rtUrl,
        type: 'post',
        data: {
            orderId: getQueryString('orderId'),
            RefundExpCo: selectExpCo,
            RefundExpOrderNo: expNo,
            userId: getUserId(),
            sessionId: sessionStorage.sessionId,
            orderItemId: getQueryString('orderItemId')
        },
        callback: function (data) {
            $("#ajaxLoadBlind").remove();
            if (data.ResultCode == 0) {
                toast("操作成功！");
                window.location.href = urlAppendCommonParams("/Mobile/MyOrderDetail?orderId=" + getQueryString('orderId') + "&shopId=" + getQueryString('shopId'));
                // location.reload();
            } else {
                toast("不好意思，操作失败，请您重试！");
            }
        },
        beforeSend: function () {
            ajaxLoading('22', '');
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
        }
    })
    $("#confirm").html("确定");
    $("#confirm").removeAttr("disabled");


}