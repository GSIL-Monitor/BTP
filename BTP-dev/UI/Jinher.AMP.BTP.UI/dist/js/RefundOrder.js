var flex = function(){
    var deviceWidth = document.documentElement.clientWidth>500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function(){
    flex();
};
var REFUNDORDER = (function (mod, $, undefined) {
    mod.refundMoneyReason = ['缺货','协商一致退款','未按约定时间发货','其他'];//仅退款原因
    mod.refundGoodsReason = ['商品有破损、有污渍','效果不好/不喜欢','使用后过敏','商品与描述不符','其他'];//退货原因
    mod.type = '1'; //服务类型 默认为仅退款
    /**
     * 显示弹窗
     */
    mod.modalShow = function(){
        $('#modal').removeClass('hide');
        $('body').addClass('lock');
    };
    /**
     * 隐藏弹窗
     */
    mod.modalHide = function(){
        $('#modal').addClass('hide');
        $('body').removeClass('lock');
    };
    /**
     * 渲染弹窗中的退货/退款原因
     * @param data
     */
    mod.renderReason = function(data){
        var str = '';
        for(var i = 0; i < data.length; i++){
            str += '<div class="list">'+ data[i] +'</div>';
        }
        $('#lists').html(str);
    };
    /**
     * 初始化事件
     */
    mod.initEvent = function () {
        $('.tips-close').on('click',function(){
            $('#tips').addClass('hide');
        });
        /**
         * 服务类型
         */
        $('.type').on('click','.refund-btns .btns',function(){
            var isSelect = $(this).hasClass('active');
            if(isSelect) {
                return;
            }
            mod.type = $(this).attr('tag');
            $(this).parents('.type').find('.refund-btns .btns').removeClass('active');
            $(this).addClass('active');
            $('#reason-text').text('请选择');
            $("#hidoptionsType").val(mod.type);
            if(mod.type == '1')
                document.title = "退款申请";
            else if (mod.type == "2") 
                document.title = "退款/退货申请";
        });
        /**
         * 点击退货原因
         */
        $('.reason').on('click',function(){
            console.log(mod.type,'mod.type');
            var data;
            if(mod.type == '2'){
                data = mod.refundGoodsReason;
            }else{
                data = mod.refundMoneyReason;
            }
            mod.renderReason(data);
            mod.modalShow();
        });
        /**
         * 弹窗事件
         */
        $('#modal').on('click',function(event){
            if(event.target.className == 'logistics-modal' || event.target.className == 'close'){
                mod.modalHide();
            }
        }).on('click','.list',function(){
            var text = $(this).text();
            $('#reason-text').text(text);
            mod.modalHide();
        });
        /**
         * 点击上传按钮
         */
        $('#addPicli').on('click', function () {
            $('#add').click();
        });
        // //file change
        // $('#add').on('change',function(event){
        //     var target = event.target;
        //     var file = target.files[0];
        //     var fr = new FileReader();
        //     fr.onload=function () {
        //         var html = '<li class="upload-pic">' +
        //                         '<img class="img" src="' + this.result +'">'+
        //                         '<a href="javascript:void(0);" class="cancel"></a>'+
        //                     '</li>';
        //         $("#upload-wrap").prepend(html);
        //     };
        //     fr.readAsDataURL(file);
        // });
        /**
         * 删除图片
         */
        $('#upload-wrap').on('click', '.cancel', function () {
            //删除图片成功后，将dom元素删除
            $(this).parent().remove();
        });
        /**
         * 退货方式
         */
        $('.refund-method').on('click','.refund-btns .btns',function(){
            var isSelect = $(this).hasClass('active');
            if(isSelect) {
                return;
            }
            $(this).parents('.refund-method').find('.refund-btns .btns').removeClass('active');
            $(this).addClass('active');
        });
    };
    return mod;
})(REFUNDORDER || {}, window.Zepto);

var tempType = "";
//是否是售后：0不是 1是。
var _isAfterSale = 0;
$(function () {
    REFUNDORDER.initEvent();
    //控制问题描述字数
    var limitNum = 140;
    $('.dec').on('keydown keyup keypress submit focus blur copy cut paste change input', function (event) {
        var value = $.trim($(this).val());
        var remain = value.length;
        if (remain > limitNum) {
            $(this).val(value.substring(0, limitNum));
        }
    });

    saveContextDTOByUrl();
    if (!isWeiXin() && sessionStorage.source == "share") {
        $(".mobile-header").show();
    } else {
        $(".mobile-header").hide();
    }

    $("#RefundMoneyGoods").hide();
    $("#elseContent").hide();
    $("#elseContentGoods").hide();
    $("#ExpElseContent").hide();

    tempType = getQueryString("type");
    _isAfterSale = getQueryString("isAfterSale");
    _isAfterSale = parseInt(_isAfterSale);

    //目前同tempType == "3".
    if (_isAfterSale == 1) {
        $("#RefundMoneyGoods").hide();
        $("#RefundMoney").show();
        $("#RefundContent").show();
        $("#optionsType").show();
        $("#radioType2").show();
        $("#RefundMoneyPic").removeAttr("disabled");
        $("#uploadpicli").show();
    } else if (tempType == "1") {
        $("#RefundMoneyGoods").hide();
        $("#RefundMoney").hide();
        $("#RefundContent").show();
        $("#optionsType").hide();
        $("#radioType2").hide();
        $("#uploadpicli").hide();

    } else if (tempType == "2") {
        $("#RefundMoneyGoods").hide();
        $("#RefundMoney").show();
        $("#RefundContent").show();
        $("#optionsType").show();
        $("#radioType2").hide();
        $("#RefundMoneyPic").attr("disabled", "");
        $("#uploadpicli").show();
    } else if (tempType == "3") {
        $("#RefundMoneyGoods").hide();
        $("#RefundMoney").show();
        $("#RefundContent").show();
        $("#optionsType").show();
        $("#radioType2").show();
        $("#RefundMoneyPic").removeAttr("disabled");
        $("#uploadpicli").show();
    } else if (tempType == "4") {
        $("#RefundMoneyGoods").hide();
        $("#RefundMoney").hide();
        $("#RefundContent").hide();
        $("#optionsType").hide();
        $("#radioType2").hide();
        $("#RefundMoneyPic").removeAttr("disabled");
        $("#RefundExpOrder").show();
    }

    //上传完成
    document.getElementById('uploadframe').onload = function () {
        //if (document.readyState == 'complete') {
        try {
            var doc;
            if (navigator.userAgent.indexOf("MSIE") > 0) {
                doc = document.getElementById('uploadframe').contentWindow.document;
            } else {
                doc = (document.getElementById('uploadframe').contentDocument || window.frames["uploadframe"].document);
            }
            if (doc && doc.body) {
                if (doc.body.innerHTML.split('^')[0] == "0") {
                    toast(doc.body.innerHTML.split('^')[1]);
                } else if (doc.body.innerHTML) {
                    preview(doc.body.innerHTML);
                }
            }
        } catch (err) { }
        loadingFrame.hide();
        // }
    };
});

function JsVilaDataNull(obj) {
    if (obj != undefined && obj != "" && obj != "null" && obj != null && obj != "undefined" && obj) {
        return true;
    } else {
        return false;
    }
}

//上传图片
function uploadfun() {
    var _file = document.getElementById('add').value;
    if (_file) {
        var fileExt = _file.substr(_file.lastIndexOf(".")).toLowerCase();
        if (fileExt == '.jpg' || fileExt == '.jpeg' || fileExt == '.png' || fileExt == '.gif') {
            loadingFrame.show();
            setTimeout("loadingFrame.hide()", 30000);
            //提交数据
            document.getElementById('picForm').submit();
        } else {
            toast("只能上传 jpg、jpeg、png、gif 类型的文件");
            $("#add").val("");
        }
    }
}

var loadingFrame = {
    //显示信息框：loading.show();
    //隐藏信息框：loading.hide();
    show: function (msg) {
        if (document.getElementById('loading') == null) {
            var _body = $('.page')[0],
                _dom = document.createElement('div'),
                h = document.body.scrollHeight;
            _dom.innerHTML = "<span class='msg' style='top:" + (h - 200) + "px'>正在上传，请稍候</span>";
            _dom.id = "loading";
            _dom.className = "loading";
            _dom.style.height = (h + 30) + "px";
            _body.appendChild(_dom);
        } else {
            document.getElementById('loading').style.display = "block";
        }
    },
    hide: function () {
        document.getElementById('loading').style.display = "none";
    }
};

//上传成功后显示预览函数
function preview(source) {
    try {
        if (source) {
            var data = $.parseJSON(source); //{ url = fileUrl, name = fileName }

            if (data.Url) {
                var pic = document.createElement("li");
                pic.setAttribute('class', 'upload-pic');
                var url = data.Url;
                pic.innerHTML = "<img src='" + url + "'  class='img'/><a href='javascript:void(0);' class='cancel'></a>";
                var box_view = document.getElementById('upload-wrap');
                box_view.prepend(pic);
                //清除上传控件设置的文件
                if (box_view.childElementCount == 6) {
                    $("#addPicli").hide();
                } else {
                    $("#addPicli").show();
                }
                $("#add").val("");
                $("#isEditM").val("1");

            }
        }
    } catch (err) { }
}

function PreClickOnPic(id) {
//    if (window.uploadPicand) {
//        window.uploadPicand.addPicand();
//    }
}

function deluploadpic(delspan) {

    $(delspan).parent().remove();

    if ($("#upload-wrap").childElementCount == 6) {
        $("#addPicli").hide();
    } else {
        $("#addPicli").show();
    }
}

//提交按钮
function btnSubmit() {
    if (!isLogin()) {
        //toast("请先登录再保存退款/退货信息！");
        return;
    }

    var _TempMoney = "";
    var _TempDec = "";
    var reg = /^\s*$/;
    var _content = $.trim($(".dec").val());
    if (_content == "") {
        toast("请填写问题描述！");
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    }
    if (reg.test(_content)) {
        toast("请填写内容！");
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    }
    _TempDec = _content;

    var _money = $("#RefundMoneyPic").val();
    if (reg.test(_money) || (eval(_money) == 0 && eval(getQueryString('pri')) > 0) || eval(_money) < 0) {

        toast("请填写退款金额！");
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    }
    //付款金额（含积分 和 易捷币）
    //            var payMoney = (parseFloat(getQueryString('pri')) + parseFloat(getQueryString('spendScoreMoney'))).toFixed(2);
    var locPic = $('#hidPic').val();
    if (eval(_money) > Number(locPic)) {
        toast("退款金额不能超过订单金额！");
        $("#confirm").html("确定");
        $("#confirm").removeAttr("disabled");
        return false;
    }
    _TempMoney = _money;
    var _remark = "";
    var _refund = $("#hidoptionsType").val();
    if (_refund == "1") {
        _remark = $('#reason-text').text();
        if (_remark == "请选择" || _remark == "") {
            toast("请选择退款/退货原因！");
            $("#confirm").html("确定");
            $("#confirm").removeAttr("disabled");
            return false;
        }

    } else if (_refund == "2") {
        _remark = $('#reason-text').text();
        if (_remark == "请选择" || _remark == "") {
            toast("请选择退款/退货原因！");
            $("#confirm").html("确定");
            $("#confirm").removeAttr("disabled");
            return false;

        }


    }
    //           if (tempType == "3" && document.getElementById('preview').childElementCount == 1) {
    //                toast("请上传凭证！");
    //                $("#confirm").html("确定");
    //                $("#confirm").removeAttr("disabled");
    //                return false;
    //            }
    if (tempType == "4") {

        var _remark = $("#RefundExpCo").val();
        if (_remark == "其他") {
            var tempContent = $("#txtExpElseContent").val();
            if (reg.test(tempContent)) {
                toast("请填写快递公司名称！");
                $("#confirm").html("确定");
                $("#confirm").removeAttr("disabled");
                return false;
            }
        }

        var _content = $("#RefundExpNo").val();
        if (reg.test(_content)) {
            toast("请填写快递单号！");
            $("#confirm").html("确定");
            $("#confirm").removeAttr("disabled");
            return false;
        }
    }


    var refundImgs = "";
    var rImgs = $('#upload-wrap >li .img');
    for (var i = 0; i < rImgs.length; i++) {
        if (rImgs[i].src != "") {
            refundImgs += rImgs[i].src + ",";
        }
    }
    refundImgs = refundImgs.substring(0, refundImgs.length - 1);


    var a = tempType;
    if (_isAfterSale == 1) {
        var tvv = $("#hidoptionsType").val();
        if (tvv == 1) {
            a = 2;
        } else if (tvv == 2) {
            a = 3;
        }
    }

    //提交url.
    var purl = _isAfterSale == 1 ? '/Mobile/SaveRefundOrderAfterSales' : '/Mobile/SaveRefundOrder/';

    getDataAjax2({
        url: purl,
        type: 'post',
        data: {
            type: a,
            //RefundExpCo: $("#RefundExpCo").val() == "其他" ? $("#txtExpElseContent").val() : $("#RefundExpCo").val(),
            //RefundExpOrderNo: $("#RefundExpNo").val(),
            appId: $("#hidAppId").val(),
            state: $("#hidState").val(),
            orderId: $("#hidOrderId").val(),
            money: _TempMoney,
            dec: _TempDec,
            refundReason: _remark,
            userId: getUserId(),
            pay: $("#hidPay").val(),
            refundType: $("#hidoptionsType").val(),
            pic: refundImgs,
            orderItemId: getQueryString("orderItemId")
        },
        callback: function (data) {
            $("#ajaxLoadBlind").remove();
            if (data.ResultCode == 0) {
                toast("操作成功！");
                window.location.href = urlAppendCommonParams("/Mobile/MyOrderDetail?orderId=" + $("#hidOrderId").val() + "&shopId=" + $("#hidAppId").val());
                // location.reload();
            } else {
                if (data.ResultCode == 1) {
                    toast(data.Message);
                } else {
                    toast("不好意思，操作失败，请您重试！");
                }
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
