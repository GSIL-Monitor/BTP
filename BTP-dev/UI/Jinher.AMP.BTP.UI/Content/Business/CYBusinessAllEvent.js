$(function () {
    var UnLimit = 2147483647;
    document.ondragstart = function () { return false; }
    newSetIframeHeight();

    $(window.parent).resize(function () {
        //		newSetIframeHeight();
    });

    $("#sortable").sortable({
        start: function (event, ui) {
            $("#sortable").find('.img1').off("onmouseover", "**");
            $(this).find("div.diwein").hide();
        },
        stop: function (event, ui) {
            $("#sortable").find('.img1').on("onmouseover", function () {
                $(this).find("div.diwein").show();
            })
        }
    }).disableSelection();

});

/**
* ajax loading 图片
* @param id                默认基础上添加的区别ID
* @param insertElement     插入那个Element元素名称.可输入   #id   .class  不传入该值时.默认插入body内.并生成一个蒙版.
*/
function ajaxLoading(id, insertElement) {
    var loading = $('#ajaxLoading_' + id);
    var blind = $('<div></div>');
    var insertElements = insertElement ? $(insertElement) : blind;

    blind.css({
        'position': 'fixed',
        'z-index': '98',
        'opacity': 0.2,
        'backgroundColor': '#ccc',
        'height': '100%',
        'width': '100%',
        'top': 0,
        'left': 0
    });

    blind.attr('id', 'ajaxLoadBlind');

    if (!insertElement) {
        !$('#ajaxLoadBlind')[0] ? $('body').append(blind) : '';
    }

    if (!loading.attr('id')) {
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_' + id);
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });

        loading.append(createImg());

        insertElement ? insertElements.css({ 'position': 'relative' }) : '';
        insertElements.append(loading);
        var windowParent = $(window.parent);
        !insertElement ? loading.css({
            top: loading.css({ top: (windowParent.height() / 2) - 16 + windowParent.scrollTop() })
        }) : '';

    } else {
        !insertElement ? $('#ajaxLoadBlind').remove() : loading.remove();
    }

    function createImg() {
        var img = $('<img>');
        img.attr('src', '/Content/images/ajax-loader.gif');
        return img;
    }
}

function clickSetElementValue() {
    $('.clickElement').live('click', function () {
        ent(this);
    });

    $('.upDataClick').live('click', function () {
        update(this);
    });

    $('.stockFlagElement').focus(function () {
        if (this.value == '不限') {
            this.value = '';
            $(this).removeClass("txtColor");
        }
    }).blur(function () {
        if (this.value == '') {
            this.value = '不限';
            $(this).addClass("txtColor");
        }
    }).each(function () {
        if (this.value == '不限') {
            $(this).addClass("txtColor");
        }
    });

    function ent(element) {
        var self = $(element);
        var parent = self.parents('.parentElement');
        self.hide();
        parent.find('.upDataClick').show();
        parent.find('.showElement').val((function () {
            return parent.find('.setElementText').hide().text().replace(/(^\s*)|(\s*$)/g, "").replace("￥", "").replace("$", "");
        })()).show();
    }

    function update(element) {
        var self = $(element);
        var parent = self.parents('.parentElement');
        var input = parent.find('.showElement');
        var inputValue = input.val();
        var data = {};
        var dataKey = self.attr('data-key');
        var verification = [];
        var num = input.attr("maxlength");
        data.CommodityId = self.parents('tr').attr('data-items-id');
        data[dataKey] = inputValue == "不限" ? UnLimit : inputValue;

        if (num == "30") {
            switch (self.attr('data-type')) {
                case '1':
                    if (!data[dataKey]) {
                        verification.push('商品名称不能为空!');
                    } else if (data[dataKey].length > 30) {
                        verification.push("不能超出30字符！");
                    }
                    break;
                case '2':
                    if (!data[dataKey].match(/^\d+(?:\.\d{0,2})?$/)) {
                        verification.push('请输入合法的价格，最多保留2位小数！');
                    }
                    else if (data[dataKey].length > 10) {
                        verification.push('修改失败！');
                    }
                    break;
                case '3':
                    if (!data[dataKey].match(/^(0|[1-9]\d*)$/)) {
                        verification.push('请输入非负整数！');
                    }
                    break;
                case '4':
                    if (data[dataKey]) {
                        if (!data[dataKey].match(/^\d+(?:\.\d{0,2})?$/)) {
                            verification.push('请输入合法的价格，最多保留2位小数！');
                        }
                        else if (data[dataKey].length > 10) {
                            verification.push('修改失败！');
                        }
                        var price = parent.prev().find("span:first").html().replace("￥", '').replace("$", '');
                        if (parseFloat(price) > parseFloat(data[dataKey])) {
                            verification.push('市场价不得小于商品现价！');
                        }
                    }
                    break;
            }
        } else if (num == "60") {
            switch (self.attr('data-type')) {
                case '1':
                    if (!data[dataKey]) {
                        verification.push('商品名称不能为空!');
                    } else if (data[dataKey].length > 60) {
                        verification.push("不能超出60字符！");
                    }
                    break;
                case '2':
                    if (!data[dataKey].match(/^\d+(?:\.\d{0,2})?$/)) {
                        verification.push('请输入合法的价格，最多保留2位小数！');
                    }
                    else if (data[dataKey].length > 10) {
                        verification.push('修改失败！');
                    }
                    break;
                case '3':
                    if (!data[dataKey].match(/^(0|[1-9]\d*)$/)) {
                        verification.push('请输入非负整数！');
                    }
                    break;
                case '4':
                    if (data[dataKey]) {


                        if (!data[dataKey].match(/^\d+(?:\.\d{0,2})?$/)) {
                            verification.push('请输入合法的价格，最多保留2位小数！');
                        }
                        else if (data[dataKey].length > 10) {
                            verification.push('修改失败！');
                        }
                        var price = parent.prev().find("span:first").html().replace("￥", '').replace("$", '');
                        if (parseFloat(price) > parseFloat(data[dataKey])) {
                            verification.push('市场价不得小于商品现价！');
                        }
                    }
                    break;
            }
        }
       

        var setElementText = parent.find('.setElementText');
        var setElementTextReplace = setElementText.text().replace(/(^\s*)|(\s*$)/g, "").replace("￥", "").replace("$", "");
        if (!verification.length) {
            if (inputValue != setElementTextReplace) {
                $.post(self.attr('data-action'), data, function (data) {
                    if (data.Result) {
                        hideBox(true);
                        if (self.attr('data-type') == '3' && parseInt(inputValue) <= 1) {
                            self.parents('tr').addClass("bgstock");
                        } else {
                            self.parents('tr').removeClass("bgstock");
                        }
                        alert(data.Messages);
                    } else {
                        input.val(setElementTextReplace);

                        alert(data.Messages);
                    }
                }, 'json').error(function () {
                    alert("请稍候再试");
                });
            } else {
                hideBox(false);
            }

            function hideBox(type) {
                self.hide();
                parent.find('.clickElement').show();
                type ? setElementText.text(inputValue).show() : setElementText.show();
                input.hide();
            }
        } else {
            input.val(setElementTextReplace);
            alert(verification[0]);
        }

    }
}

function closeBox(closeElement, closeType) {
    $(closeElement)[closeType]();
}

var upImageCallback = {};
//重新设置iframe高度.
function newSetIframeHeight() {
    var parentWindow = $(window.parent.document);
    parentWindow.find('#mainframe').removeAttr('style');
    var body = $("body").removeAttr('style');
    var body_height = body.height() + 700;
    var oriHeight = $(window.parent).height() - parentWindow.find('#header').height() - parentWindow.find('.footer').outerHeight();
    var setHeight = body_height > oriHeight ? body_height : oriHeight - 5;
    parentWindow.find('#mainframe').height(setHeight);
    body.height(setHeight);
}
//重新设置iframe宽度.
function newSetIframeWidth() {
    var parentWindow = $(window.parent.document);
    parentWindow.find('#mainframe').removeAttr('style');
    var body = $("body").removeAttr('style');
    var body_width = body.width() + 20;
    parentWindow.find('#mainframe').width(body_width);
    body.width(body_width);
}

//显示上传图片插件
function ShowUpImg(info) {
    if(!info.upload){
        info.upload="1";
    }
    $("#contentFrame").attr("src", "/AppImage/Index?imgPath=" + info.imgPath +
		"&cutImgWidth=" + info.width + '&cutImgHeight=' + info.height + '&callback=' + info.callback + "&upload=" + info.upload);
    var bodyWidth = document.documentElement.clientWidth; //宽
    //var bodyHeight = window.screen.height - 110; //高
    var alertDivWidth = 650; //宽
    var alertDivHeight = 500; //高
    var left = bodyWidth / 2 - alertDivWidth / 2;
    var top;
    top = $(window.parent).scrollTop() + 110;
    $("#loadImgDiv").jhtablebox({
        height: 520,
        width: 650,
        resizable: false,
        title: info.windowTitle,
        modal: true,
        position: [left, top],
        buttons: {},
        close: function () {
            $("#contentFrame").attr("src", "");
        }
    });
}
//隐藏上传图片插件
function dialogImgClosed() {
    $("#loadImgDiv").jhtablebox('close');
}

//重置alert事件.

function alert(str, lineHeight, zIndex) {
    var bodyWidth = document.documentElement.clientWidth //宽

    var alertDivWidth = 300; //宽
    var alertDivHeight = 160; //高
    var left = bodyWidth / 2 - alertDivWidth / 2;
    var top;
    var div = $('<div><div style="display:table;height:81px;"><span id="alertSpan" style="display:table-cell; vertical-align:middle;width:278px;"></span></div></div>');
    div.css({
        lineHeight: lineHeight || '70px',
        textAlign: 'center'
    });

    top = $(window.parent).scrollTop() + 200;
    //top = window.screen.availHeight / 2 + 80;
    div.find("#alertSpan").html(str);
    //div.find("#alertSpan").html("offsetHeight:" + top11 + "scrollTop:" + top2 + "clientHeight:" + top3 + "availHeight:" + top4 + "offsetHeight:" + top5);
    if (zIndex && zIndex > 0) {
        div.jhtablebox({
            height: 160,
            width: 300,
            resizable: false,
            title: "消息提示",
            modal: true,
            zIndex:zIndex,
            show: {
                effect: 'fade',
                duration: 300
            },
            position: [left, top],
            buttons: {
                "确定": function() {
                    $(this).jhtablebox("close");
                }
            }
        });
    } else {
        div.jhtablebox({
            height: 160,
            width: 300,
            resizable: false,
            title: "消息提示",
            modal: true,
            show: {
                effect: 'fade',
                duration: 300
            },
            position: [left, top],
            buttons: {
                "确定": function() {
                    $(this).jhtablebox("close");
                }
            }
        });
    }
}

function getQueryString(name, str) {
    var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

function beforeSends() {
    $('#content_makes').empty();
    ajaxLoading(1, '#content_makes');
}

function sendSuccess(msg) {
    $('#content_makes').html(msg);
    newSetIframeHeight();
}

var ajaxLoadingSingle = (function() {

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
}());

