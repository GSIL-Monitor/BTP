(function ($) {
    $.fn.watermark = function (c, t) {
        var e = function (e) {
            var i = $(this);
            if (!i.val()) {
                var w = t || i.attr('title'), $c = $($("<div />").append(i.clone()).html().replace(/type=\"?password\"?/, 'type="text"')).val(w).addClass(c);
                i.replaceWith($c);
                $c.focus(function () {
                    $c.replaceWith(i); setTimeout(function () { i.focus(); }, 1);
                })
                .change(function (e) {
                    i.val($c.val()); $c.val(w); i.val() && $c.replaceWith(i);
                })
                .closest('form').submit(function () {
                    $c.replaceWith(i);
                });
            }
        };
        return $(this).live('blur change', e).change();
    };
})(jQuery);

jQuery.fn.extend(
{
    OpenDiv: function () {
        var sWidth, sHeight;
        sWidth = window.screen.availWidth;
        if (window.screen.availHeight > document.body.scrollHeight) {
            sHeight = window.screen.availHeight;
        } else {
            sHeight = document.body.scrollHeight + 20;
        }
        var maskObj = document.createElement("div");
        maskObj.setAttribute('id', 'BigDiv');
        maskObj.style.position = "absolute";
        maskObj.style.top = "0";
        maskObj.style.left = "0";
        maskObj.style.background = "#111";
        maskObj.style.filter = "Alpha(opacity=70);";
        maskObj.style.opacity = "0.7";
        maskObj.style.width = sWidth + "px";
        maskObj.style.height = sHeight + "px";
        maskObj.style.zIndex = "10000";
        $("body").attr("scroll", "no");
        document.body.appendChild(maskObj);
        $("#BigDiv").data("divbox_selectlist", $("select:visible"));
        $("select:visible").hide();
        $("#BigDiv").attr("divbox_scrolltop", $.ScrollPosition().Top);
        $("#BigDiv").attr("divbox_scrollleft", $.ScrollPosition().Left);
        $("#BigDiv").attr("htmloverflow", $("html").css("overflow"));
        $("html").css("overflow", "hidden");
        window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), $("#BigDiv").attr("divbox_scrolltop"));
        var MyDiv_w = this.width();
        var MyDiv_h = this.height();
        MyDiv_w = parseInt(MyDiv_w);
        MyDiv_h = parseInt(MyDiv_h);
        var width = $.PageSize().Width;
        var height = $.PageSize().Height;
        var left = $.ScrollPosition().Left;
        var top = $.ScrollPosition().Top;
        var Div_topposition = 200;
        var Div_leftposition = left + (width / 2) - (MyDiv_w / 2);
        this.css("position", "absolute");
        this.css("z-index", "10001");
        this.css("background", "#fff");
        this.css("left", Div_leftposition + "px");
        this.css("top", Div_topposition + "px");
        if ($.browser.mozilla) {
            this.show();
            return;
        }
        this.fadeIn("fast");
    },

    OpenDivByWidth: function (Div_leftposition, Div_topposition) {
        var sWidth, sHeight;
        sWidth = window.screen.availWidth;
        if (window.screen.availHeight > document.body.scrollHeight) {
            sHeight = window.screen.availHeight;
        } else {
            sHeight = document.body.scrollHeight + 20;
        }
        var maskObj = document.createElement("div");
        maskObj.setAttribute('id', 'BigDiv');
        maskObj.style.position = "absolute";
        maskObj.style.top = "0";
        maskObj.style.left = "0";
        maskObj.style.background = "#111";
        maskObj.style.filter = "Alpha(opacity=70);";
        maskObj.style.opacity = "0.7";
        maskObj.style.width = sWidth + "px";
        maskObj.style.height = sHeight + "px";
        maskObj.style.zIndex = "10000";
        $("body").attr("scroll", "no");
        document.body.appendChild(maskObj);
        $("#BigDiv").data("divbox_selectlist", $("select:visible"));
        $("select:visible").hide();
        $("#BigDiv").attr("divbox_scrolltop", $.ScrollPosition().Top);
        $("#BigDiv").attr("divbox_scrollleft", $.ScrollPosition().Left);
        $("#BigDiv").attr("htmloverflow", $("html").css("overflow"));
        $("html").css("overflow", "hidden");
        window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), $("#BigDiv").attr("divbox_scrolltop"));
        var MyDiv_w = this.width();
        var MyDiv_h = this.height();
        MyDiv_w = parseInt(MyDiv_w);
        MyDiv_h = parseInt(MyDiv_h);
        var width = $.PageSize().Width;
        var height = $.PageSize().Height;
        var left = $.ScrollPosition().Left;
        var top = $.ScrollPosition().Top;
        //var Div_topposition = 200;
        //var Div_leftposition = left + (width / 2) - (MyDiv_w / 2);
        this.css("position", "fixed");
        this.css("z-index", "10001");
        this.css("background", "#fff");
        this.css("left", Div_leftposition+"%");
        this.css("top", Div_topposition+"%");
        if ($.browser.mozilla) {
            this.show();
            return;
        }
        this.fadeIn("fast");
    }
, CloseDiv: function () {
    if ($.browser.mozilla) {
        this.hide();
    } else {
        this.fadeOut("fast");
    } $("html").css("overflow", $("#BigDiv").attr("htmloverflow"));
    window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), $("#BigDiv").attr("divbox_scrolltop"));
    $("#BigDiv").data("divbox_selectlist").show();
    $("#BigDiv").remove();
}
});
$.extend(
{
    PageSize: function () {
        var width = 0;
        var height = 0;
        width = window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null;
        height = window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null;
        return { Width: width, Height: height };
    }
, ScrollPosition: function () {
    var top = 0, left = 0;
    if ($.browser.mozilla) {
        top = window.pageYOffset;
        left = window.pageXOffset;
    }
    else if ($.browser.msie) {
        top = document.documentElement.scrollTop;
        left = document.documentElement.scrollLeft;
    }
    else if (document.body) {
        top = document.body.scrollTop;
        left = document.body.scrollLeft;
    }
    return { Top: top, Left: left };
}
});

/*
* ��ɫ��
* Author:MR.CO
* Date:2010-12-23
* QQ:co.mr.co@gmail.com
*/
function paletteTools(eid/*��Ҫ������ɫ��Ԫ��ID*/) {
    var eobj = $('#' + eid);
    var paletteID = 'divPalette_' + eid;
    if (eobj[0] == null || eobj[0] == undefined) return;
    if (eobj[0].nodeName.toLowerCase() != 'input') {
        alert('Error Message:Only support input elements...(jq.palette.js)');
        return;
    }
    if (!('value' in eobj[0])) return;
    this.Eobj = eobj;
    this.PaletteID = paletteID;
    var colorBox = new Array();
    var data = "#000000 #2F0000 #600030 #460046 #28004D #272727 #4D0000 #820041 #5E005E #3A006F #3C3C3C #600000 #9F0050 #750075 #4B0091 #4F4F4F #750000 #BF0060 #930093 #5B00AE #5B5B5B #930000 #D9006C #AE00AE #6F00D2 #6C6C6C #AE0000 #F00078 #D200D2 #8600FF #7B7B7B #CE0000 #FF0080 #E800E8 #921AFF #8E8E8E #EA0000 #FF359A #FF00FF #9F35FF #9D9D9D #FF0000 #FF60AF #FF44FF #B15BFF #ADADAD #FF2D2D #FF79BC #FF77FF #BE77FF #BEBEBE #FF5151 #FF95CA #FF8EFF #CA8EFF #d0d0d0 #ff7575 #ffaad5 #ffa6ff #d3a4ff #E0E0E0 #FF9797 #FFC1E0 #FFBFFF #DCB5FF #F0F0F0 #FFB5B5 #FFD9EC #FFD0FF #E6CAFF #FCFCFC #FFD2D2 #FFECF5 #FFE6FF #F1E1FF #FFFFFF #FFECEC #FFF7FB #FFF7FF #FAF4FF #000079 #000079 #003E3E #006030 #006000 #000093 #003D79 #005757 #01814A #007500 #0000C6 #004B97 #007979 #019858 #009100 #0000C6 #005AB5 #009393 #01B468 #00A600 #0000E3 #0066CC #00AEAE #02C874 #00BB00 #2828FF #0072E3 #00CACA #02DF82 #00DB00 #4A4AFF #0080FF #00E3E3 #02F78E #00EC00 #6A6AFF #2894FF #00FFFF #1AFD9C #28FF28 #7D7DFF #46A3FF #4DFFFF #4EFEB3 #53FF53 #9393FF #66B3FF #80FFFF #7AFEC6 #79FF79 #AAAAFF #84C1FF #A6FFFF #96FED1 #93FF93 #B9B9FF #97CBFF #BBFFFF #ADFEDC #A6FFA6 #CECEFF #ACD6FF #CAFFFF #C1FFE4 #BBFFBB #DDDDFF #C4E1FF #D9FFFF #D7FFEE #CEFFCE #ECECFF #D2E9FF #ECFFFF #E8FFF5 #DFFFDF #FBFBFF #ECF5FF #FDFFFF #FBFFFD #F0FFF0 #467500 #424200 #5B4B00 #844200 #642100 #548C00 #5B5B00 #796400 #9F5000 #842B00 #64A600 #737300 #977C00 #BB5E00 #A23400 #73BF00 #8C8C00 #AE8F00 #D26900 #BB3D00 #82D900 #A6A600 #C6A300 #EA7500 #D94600 #8CEA00 #C4C400 #D9B300 #FF8000 #F75000 #9AFF02 #E1E100 #EAC100 #FF9224 #FF5809 #A8FF24 #F9F900 #FFD306 #FFA042 #FF8040 #B7FF4A #FFFF37 #FFDC35 #FFAF60 #FF8F59 #C2FF68 #FFFF6F #FFE153 #FFBB77 #FF9D6F #CCFF80 #FFFF93 #FFE66F #FFC78E #FFAD86 #D3FF93 #FFFFAA #FFED97 #FFD1A4 #FFBD9D #DEFFAC #FFFFB9 #FFF0AC #FFDCB9 #FFCBB3 #E8FFC4 #FFFFCE #FFF4C1 #FFE4CA #FFDAC8 #EFFFD7 #FFFFDF #FFF8D7 #FFEEDD #FFE6D9 #F5FFE8 #FFFFF4 #FFFCEC #FFFAF4 #FFF3EE #613030 #616130 #336666 #484891 #6C3365 #743A3A #707038 #3D7878 #5151A2 #7E3D76 #804040 #808040 #408080 #5A5AAD #8F4586 #984B4B #949449 #4F9D9D #7373B9 #9F4D95 #AD5A5A #A5A552 #5CADAD #8080C0 #AE57A4 #B87070 #AFAF61 #6FB7B7 #9999CC #B766AD #C48888 #B9B973 #81C0C0 #A6A6D2 #C07AB8 #CF9E9E #C2C287 #95CACA #B8B8DC #CA8EC2 #D9B3B3 #CDCD9A #A3D1D1 #C7C7E2 #D2A2CC #E1C4C4 #D6D6AD #B3D9D9 #D8D8EB #DAB1D5 #EBD6D6 #DEDEBE #C4E1E1 #E6E6F2 #E2C2DE #F2E6E6 #E8E8D0 #D1E9E9 #F3F3FA #EBD3E8";

    var colors = data.split(' ');
    colorBox.push('<div id="' + paletteID + '" style="position:absolute;display:none;background:#fff; width:240px; _width:242px;  height:180px; border:1px solid #ccc; "><ul style=" padding:0px; margin:0px; float:left; list-style: none; ">');
    for (var i = 0; i < colors.length; i++) {
        if (colors[i].replace(/[^\a-\z\A-\Z0-9\u4E00-\u9FA5\@\.]/g, '') != '')
            colorBox.push('<li style="display:block; width:10px; height:10px; background-color:' + colors[i] + ';float:left; margin:1px; cursor:pointer;" colorVal="' + colors[i] + '"></li>');
    }
    colorBox.push('</ul></div>');
    /*����ɫ����׷�ӵ���ǰ�ı�������*/
    eobj.after(colorBox.join(''));
}
paletteTools.prototype.show = function (args/*�����õ�ɫ������λ�ô�ֵ��ʽΪ{top:'1px',left:'1px'}*/) {
    var top, left;
    if (args != undefined) {
        top = args.top || 0;
        left = args.left || 0;
        $('#' + this.PaletteID).css({ top: top, left: left });
    }
    var obj = this.Eobj, pid = this.PaletteID;
    if (obj == undefined || obj == null) return;
    /*����ѡ�е���ɫֵ*/
    var setObjColor = function (selectedColor) {
        //        obj.css({ background: selectedColor });
        obj.val(selectedColor);
        obj.change();
    }
    /*��ʾ�ı����Ľ��㡢��ʾ/����*/
    obj.focus(function () {
        $('#' + pid).css({ display: 'block' });
    }).blur(function () {
        $('#' + pid).css({ display: 'none' });
    });
    /*��ɫ�������껬��Ч��*/
    $('#' + pid + ' li').mouseover(function () {
        setObjColor($(this).attr('colorVal'));
    })
    /*����ѡȡ��ɫ����*/
    .click(function () {
        setObjColor($(this).attr('colorVal'));
        $('#' + pid).css({ display: 'none' });
    });
}