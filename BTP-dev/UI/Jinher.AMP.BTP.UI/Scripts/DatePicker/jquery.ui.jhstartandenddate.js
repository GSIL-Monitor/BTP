/**
* @fileOverview 这个文件是jhstartandenddate控件的源文件. 
* @author guany 2011/9/1  
* @version 1.0.0 
*
* Depends:
*	jquery.ui.core.js
*	jquery.ui.widget.js
*/
(function ($) {

    /** @class startandenddate控件类
    * @name jhstartandenddate
    * @description jhstartandenddate控件类
    * @property {string} stratLableName 默认开始日期
    * @property {string} endLableName 默认结束日记
    * @property {string} textBoxWidth 默认控件宽度     
    * @property {string} startDateName 默认结束日记的name，用于提交表单时候应用
    * @property {string} startDateName 默认结束日记的name,用于提交表单时候应用

    * @property {string} startDate 开始日期
    * @property {string} endDate 结束日期

    * @property {bool} showTime 是否显示时间
    */
    $.widget("ui.jhstartandenddate", {

        options: {

            stratLableName: "开始日期：",
            endLableName: "结束日期：",

            dateTextWidth: "",

            startDateName: "startDate",
            endDateName: "endDate",

            startDate: null,
            endDate: null,

            showTime: false
        },

        _create: function () {
            var self = this,
                element = this.element,
                options = this.options,

                startDate = options.startDate,
                endDate = options.endDate;

            var timer = closeByButton = options.showTime ? true : false;
            var dateFormat = options.showTime ? "yy-mm-dd h:i" : "yy-mm-dd";

            var tmpTextBoxWidth = options.dateTextWidth != "" ? parseInt(options.dateTextWidth) : 0;
            //判断设置的控件输入框宽带设置是否在（60，250）区间，如果是在此区间，则加上style的width样式属性
            //Added By Yangam @ 2012.12.14
            var iputTextWidth = (tmpTextBoxWidth > 60 && tmpTextBoxWidth < 250) ? ('style="width:' + tmpTextBoxWidth.toString() + 'px"') : '';

            element.append('<span><span>' + options.stratLableName + '</span><input type="text" name="' + options.startDateName + '" id="jhinput1_' + $(element)[0].id + '" ' + iputTextWidth + '/><span>' + options.endLableName + '</span><input type="text" name="' + options.endDateName + '" id="jhinput2_' + $(element)[0].id + '" ' + iputTextWidth + '/></span>');

            var input1 = element.find("#jhinput1_" + $(element)[0].id)
            .val(options.startDate)
            .bind("blur", function () {

                self.checkInputDate(input1, options, 1, dateFormat);

            }).change(function () {

                /*if (input2.datepicker) {
                input2.datepicker("option", "minDate", self.checkDate(input1.val()) ? input1.val() : null);
                }*/

                self.checkInputDate(input1, options, 1, dateFormat);
            });

            var input2 = element.find("#jhinput2_" + $(element)[0].id)
            .val(options.endDate)
            .bind("blur", function () {

                self.checkInputDate(input2, options, 2, dateFormat);

            }).change(function () {

                /*if (input1.datepicker) {
                input1.datepicker("option", "maxDate", self.checkDate(input2.val()) ? input2.val() : null);
                }*/
                self.checkInputDate(input2, options, 2, dateFormat);
            });

            input1.datepicker({

                //maxDate: self.checkDate(input2.val()) ? input2.val() : null,
                changeMonth: true,
                changeYear: true,
                showOtherMonths: true,
                selectOtherMonths: true,
                timer: timer,
                dateFormat: dateFormat,
                closeByButton: closeByButton
            });

            input2.datepicker({

                //minDate: self.checkDate(input1.val()) ? input1.val() : null,
                changeMonth: true,
                changeYear: true,
                showOtherMonths: true,
                selectOtherMonths: true,
                timer: timer,
                dateFormat: dateFormat,
                closeByButton: closeByButton
            });

        },

        checkInputDate: function (obj, options, index, dateFormat) {

            if (!this.checkDate(obj.val(), obj, index, dateFormat)) {
                obj.addClass("input-error");
            }
            else {
                var nextinput, str1, str2,
                regexp = /^\d{4}\s\d{2}\s\d{2}(\s\d{2}:\d{2})?$/;
                if (index == 1) {
                    nextinput = $('#jhinput2_' + this.element[0].id);
                    str1 = obj.val().replace(/\//g, ' ').replace(/-/g, ' ');
                    str2 = nextinput.val().replace(/\//g, ' ').replace(/-/g, ' ');
                    if (regexp.test(str2)) {
                        if (str1 <= str2) {
                            nextinput.removeClass("input-error");
                        }
                    }
                }
                else {
                    nextinput = $('#jhinput1_' + this.element[0].id);
                    str1 = nextinput.val().replace(/\//g, ' ').replace(/-/g, ' ');
                    str2 = obj.val().replace(/\//g, ' ').replace(/-/g, ' ');
                    if (regexp.test(str1)) {
                        if (str1 <= str2) {
                            nextinput.removeClass("input-error");
                        }
                    }
                }
                //console.log(str1, str2, (str1 <= str2));
                obj.removeClass("input-error");
            }
        },

        checkDate: function (date, curInput, index, dateFormat) {

            var daysInFebruary = function (year) {

                return (((year % 4 === 0) && (year % 100 !== 0 || (year % 400 === 0))) ? 29 : 28);
            },
		    DaysArray = function (n) {
		        for (var i = 1; i <= n; i++) {
		            this[i] = 31;
		            if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30; }
		            if (i == 2) { this[i] = 29; }
		        }
		        return this;
		    };

            //var formats = ["yyyy-MM-dd", "yyyy/MM/dd", "yyyy MM dd", "yyyy年MM月dd日"];
            var formats = ["yyyy-MM-dd", "yyyy/MM/dd", "yyyy MM dd"];
            if (!date) { return true; }
            if (date.indexOf("-") != -1) {
                format = formats[0];
                sep = "-";
            }
            else if (date.indexOf("/") != -1) {
                format = formats[1];
                sep = "/";
            }
            else if (date.match(/\s/ig) && date.match(/\s/ig).length > 1) {
                format = formats[2];
                sep = " ";
            }
            //            else if (date.indexOf("年") != -1) {
            //                date = date.replace("年", "-").replace("月", "-").replace("日", "");
            //                format = "yyyy-MM-dd";
            //                sep = "-";
            //            }
            else {
                return false;
            }

            var tsp = {}, sep, times = [];

            format = format.split(sep);
            date = date.split(sep);
            if (date.length == 3) {
                times = date[2].split(/[\s:]/g);
            }
            else if (date.length == 4) {
                times = (date[2] + ' ' + date[3]).split(/[\s:]/g);
            }

            if (dateFormat == 'yy-mm-dd') {
                if (date.length != 3) {
                    return false;
                }
                else if (date[0].length != 4 || date[1].length != 2 || date[2].length != 2) {
                    var showMonth = date[1];
                    if (showMonth.length == 1 && showMonth != 0) {
                        showMonth = '0' + showMonth;
                    }

                    var showDate = date[2];
                    if (showDate.length == 1 && showDate != 0) {
                        showDate = '0' + showDate;
                    }
                    curInput.val(date[0] + sep + showMonth + sep + showDate);
                }
            }
            else {
                if (date.length == 4) {
                    if (!/^\d{1,2}:\d{1,2}$/.test(date[3])) {
                        return false;
                    }
                }
                else if (date.length != 3) {
                    return false;
                }
                else if (times.length != 3) {
                    return false;
                }
                if (date[0].length != 4 || date[1].length != 2 || times[0].length != 2 || times[1].length != 2 || times[2].length != 2) {
                    if (date[1].length == 1 && date[1] != 0) {
                        date[1] = '0' + date[1];
                    }
                    if (times[0].length == 1 && times[0] != 0) {
                        times[0] = '0' + times[0];
                    }
                    if (times[1].length == 1 && times[1] != 0) {
                        times[1] = '0' + times[1];
                    }
                    if (times[2].length == 1 && times[2] != 0) {
                        times[2] = '0' + times[2];
                    }
                    curInput.val(date[0] + sep + date[1] + sep + times[0] + ' ' + times[1] + ':' + times[2]);
                }
            }
            //判断开始时间是否小于结束时间
            if (index == 1) {
                var input2 = $('#jhinput2_' + this.element[0].id);
                var flag = input2.hasClass('input-error');
                if ($.trim(input2.val()) != '') {
                    if (!flag) {
                        var str1 = curInput.val().replace(/\//g, ' ').replace(/-/g, ' ');
                        var str2 = input2.val().replace(/\//g, ' ').replace(/-/g, ' ');
                        if (str1 > str2) {
                            return false;
                        }
                    }
                }
            }
            //判断结束时间是否大于开始时间
            if (index == 2) {
                var input1 = $('#jhinput1_' + this.element[0].id);
                var flag = input1.hasClass('input-error');
                if ($.trim(input1.val()) != '') {
                    if (!flag) {
                        var str1 = input1.val().replace(/\//g, ' ').replace(/-/g, ' ');
                        var str2 = curInput.val().replace(/\//g, ' ').replace(/-/g, ' ');
                        if (str1 > str2) {
                            return false;
                        }
                    }
                }
            }

            var j = -1, yln, dln = -1, mln = -1;
            for (var i = 0; i < format.length; i++) {
                if (i == 2) { date[i] = date[i].split(/\s/)[0]; }
                var dv = isNaN(date[i]) ? 0 : parseInt(date[i], 10);
                tsp[format[i]] = dv;
                yln = format[i];
                if (yln.indexOf("y") != -1) { j = i; }
                if (yln.indexOf("M") != -1) { mln = i; }
                if (yln.indexOf("d") != -1) { dln = i; }
            }
            if (format[j] == "y" || format[j] == "yyyy") {
                yln = 4;
            } else if (format[j] == "yy") {
                yln = 2;
            } else {
                yln = -1;
            }
            var daysInMonth = DaysArray(12),
		    strDate;
            if (j === -1) {
                return false;
            } else {
                strDate = tsp[format[j]].toString();
                if (yln == 2 && strDate.length == 1) { yln = 1; }
                if (strDate.length != yln || (tsp[format[j]] === 0 && date[j] != "00")) {
                    return false;
                }
            }
            if (mln === -1) {
                return false;
            } else {
                strDate = tsp[format[mln]].toString();
                if (strDate.length < 1 || tsp[format[mln]] < 1 || tsp[format[mln]] > 12) {
                    return false;
                }
            }
            if (dln === -1) {
                return false;
            } else {
                strDate = tsp[format[dln]].toString();
                if (strDate.length < 1 || tsp[format[dln]] < 1 || tsp[format[dln]] > 31 || (tsp[format[mln]] == 2 && tsp[format[dln]] > daysInFebruary(tsp[format[j]])) || tsp[format[dln]] > daysInMonth[tsp[format[mln]]]) {
                    return false;
                }
            }

            if (times.length > 1 && (times[1].length < 1 || times[1] > 23)) {
                return false;
            }
            if (times.length > 2 && (times[2].length < 1 || times[2] > 59)) {
                return false;
            }
            return true;
        },

        checkTime: function (time) {
            // checks only hh:ss (and optional am/pm)
            var re = /^(\d{1,2}):(\d{2})([ap]m)?$/, regs;
            if (!this.isEmpty(time)) {
                regs = time.match(re);
                if (regs) {
                    if (regs[3]) {
                        if (regs[1] < 1 || regs[1] > 12) { return false; }
                    } else {
                        if (regs[1] > 23) { return false; }
                    }
                    if (regs[2] > 59) {
                        return false;
                    }
                } else {
                    return false;
                }
            }
            return true;
        }
    });

})(jQuery)