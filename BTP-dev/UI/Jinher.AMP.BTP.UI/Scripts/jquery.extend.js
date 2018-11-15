(function ($) {
    $.extend({
        format: function (format) { //jqgformat
            var args = $.makeArray(arguments).slice(1);
            if (format === undefined) { format = ""; }
            return format.replace(/\{(\d+)\}/g, function (m, i) {
                return args[i];
            });
        },
        /** 
        * js截取字符串，中英文都能用 
        * @param str：需要截取的字符串 
        * @param len: 需要截取的长度 
        */
        cutstr: function (str, len) {
            var str_length = 0;
            var str_len = 0;
            var str_cut = new String();
            str_len = str.length;
            for (var i = 0; i < str_len; i++) {
                var a = str.charAt(i);
                str_length++;
                if (escape(a).length > 4) {
                    //中文字符的长度经编码之后大于4  
                    str_length++;
                }
                str_cut = str_cut.concat(a);
                if (str_length >= len) {
                    str_cut = str_cut.concat("...");
                    return str_cut;
                }
            }
            //如果给定字符串小于指定长度，则返回源字符串；  
            if (str_length < len) {
                return str;
            }
        }
    });

    jQuery.fn.extend({
        bindForm: function (model) {
            if (model == undefined || model == null) {
                return this;
            }
            var self = this;
            var formId = self.attr("id");
            $("input,select,hidden,textarea", self).each(function () {
                var name = $(this).attr("name");
                if (name) {
                    var fieldName = name.substr(name.lastIndexOf(".") + 1);
                    $(this).bindControl(model[fieldName], formId);
                }
            });
        },
        bindControl: function (value, formId) {
            if (value == undefined)
                return this;
            value = value.toString();
            formId = formId || "";
            if (formId != "")
                formId = "#" + formId + " ";
            switch (this.attr("type")) {
                case "select-one":
                case "select": //DropDownList
                    //this[0].selectedIndex = 0;
                    //$("option[value='" + value + "']", this).attr("selected");
                    var isSelected = false;
                    this.children().each(function () {
                        if (this.value == value) {
                            this.selected = true;
                            isSelected = true;
                            return;
                        }
                    });
                    if (!isSelected)
                        this[0].selectedIndex = 0;
                    break;
                case "select-multiple": //ListBox
                    this.children().each(function () {
                        var arr = value.split(',');
                        for (var i = 0; i < arr.length; i++) {
                            if (this.value == arr[i]) {
                                this.selected = true;
                            }
                        }
                    });
                    break;
                case "checkbox": //CheckboxList
                    //单选
                    if (value.indexOf(',') == -1) {
                        $(formId + "input[name='" + this.attr("name") + "']").each(function () {
                            if (this.value == value) {
                                $(this).attr("checked", true);
                                return;
                            }
                        });
                    }
                    //多选
                    else if (this.attr("type") == 'checkbox') {
                        var arr = value.split(',');
                        for (var i = 0; i < arr.length; i++) {
                            $(formId + "input[name='" + this.attr("name") + "']").each(function () {
                                if (this.value == arr[i]) {
                                    this.checked = true;
                                }
                            });
                        }
                    }
                    break;
                case "radio": //RadioButtonList
                    $(formId + " input[name='" + this.attr("name") + "']").each(function () {
                        if (this.value == value) {
                            this.checked = true;
                            return;
                        }
                    });
                    break;
                default: //Normal
                    this.val(value);
                    break;
            }
            return this;
        }
    });

    $.parseDate = function (date, format) {
        if (!format) format = "yyyy-MM-dd h:i:s";
        var date = new Date(parseInt(date.replace("/Date(", "").replace(")/", ""), 10));
        return date.format(format);
    };
})(jQuery);

Date.prototype.format = function (format) {
    /*  
    * eg:format="yyyy-MM-dd hh:mm:ss";  
    */
    var o = {
        "M+": this.getMonth() + 1,  //month   
        "d+": this.getDate(),     //day   
        "h+": this.getHours(),    //hour   
        "i+": this.getMinutes(),  //minute   
        "s+": this.getSeconds(), //second   
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter   
        "S": this.getMilliseconds() //millisecond   
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

function alert(content) {
    $("<div></div>").jhtablebox({
        type: 'Alert',
        title: '消息提示',
        content: content,
        resizable: false,
        modal: true,
        zIndex:100000
    });
}