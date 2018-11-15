
var JSTools = {
    //将/Date(...)格式转换为yyyy-MM-dd型日期格式
    ChangeDateFormat: function (dateSource, formateStr) {
        var result = "";
        if (dateSource != null) {
            var date = new Date(parseInt(dateSource.replace("/Date(", "").replace(")/", ""), 10));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            switch (formateStr) {
                case "yyyy-MM-dd hh:ss:mm":
                    result = date.getFullYear() + "-" + month + "-" + currentDate + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
                    break;
                case "yyyy-MM-dd hh:ss":
                    result = date.getFullYear() + "-" + month + "-" + currentDate + " " + date.getHours() + ":" + date.getMinutes();
                    break;
                case "yyyy-MM-dd":
                    result = date.getFullYear() + "-" + month + "-" + currentDate;
                    break;
            }
        }
        return result;
    },
    //只允许输入数字
    numberOnly: function () {
        var input = String.fromCharCode(event.keyCode);
        var reg = /^\d$/;
        return reg.test(input);
    },
    //只允许输入数字，包括小数
    decimalInput: function (item) {
        var input = String.fromCharCode(event.keyCode);
        var reg = /^\d$/;
        var value = item.value;
        if (input == '.') {
            if (value.indexOf('.') >= 0) {
                return false;
            } else {
                return true;
            }
        } else {
            return reg.test(input);
        }
    },
    //检查值是否是手机号码（11位数字）
    checkIsPhoneNumber: function (value) {
        var reg = /^\d{11}$/;
        if (reg.test(value)) {
            return true;
        }
        else {
            return false;
        }
    },
    //对象转换为url参数
    toQueryString: function (obj) {
        var ret = [];
        var values;
        for (var key in obj) {
            key = encodeURIComponent(key);
            values = obj[key];
            if (values) {
                //数组
                if (values.constructor == Array) {
                    ret.push(key + "=" + encodeURIComponent(values.join(',')));
                }
                //字符串 
                else {
                    ret.push(key + "=" + encodeURIComponent(values));
                }
            }
        }
        return ret.join('&');
    },
    getDate: function (callvalue) {
        var oDate = new Date(callvalue);
        var fmtDate = oDate.getFullYear() + "-" + (parseInt(oDate.getMonth()) + 1) + "-" + oDate.getDate();
        return fmtDate;
    },
    //判断日期，当stardate>enddate时返回false  否则true
    judgeDate: function (startDate, endDate) {
        var objDateStart = new Date(startDate);
        var objDateEnd = new Date(endDate);
        if (objDateStart.getFullYear() > objDateEnd.getFullYear())
            return false;
        if (objDateStart.getMonth() > objDateEnd.getMonth() && objDateStart.getFullYear() >= objDateEnd.getFullYear())
            return false;
        if (objDateStart.getDate() > objDateEnd.getDate() && objDateStart.getMonth() >= objDateEnd.getMonth() && objDateStart.getFullYear() >= objDateEnd.getFullYear())
            return false;
        return true;
    },
    //返回字符串的字节数
    getByteLength: function (str) {
        if (str) {
            return str.replace(/[^\x00-\xff]/g, "**").length;
        } else {
            return 0;
        }
    }
}
