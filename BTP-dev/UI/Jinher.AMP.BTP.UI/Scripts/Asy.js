function Asy(asy) {
    this.xmlHttp;
    this.publicBackString = "";
    this.isArrayBuffer = false;
    this.asyc = asy;
    this.createXMLHttpRequest = function () {
        //Mozilla 浏览器（将XMLHttpRequest对象作为本地浏览器对象来创建）
        if (window.XMLHttpRequest) { //Mozilla 浏览器
            xmlHttp = new XMLHttpRequest();
        } else if (window.ActiveXObject) { //IE浏览器
            //IE浏览器（将XMLHttpRequest对象作为ActiveX对象来创建）
            try {
                xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                try {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                } catch (e) { }
            }
        }
        if (xmlHttp == null) {
            alert("不能创建XMLHttpRequest对象");
            return false;
        }
    };

    //http://deep.com/prodfile.php?sn_obj_scene~id=100 & sn_camera~id=100 & sn_group~id=100 & sn_renderer~id=100 & sn_orbitctrl~id=100 & sn_group~id=100 & sn_light~id=100 & sn_obj_texture~id=100 & sn_group~id=100 & sn_obj~id=100

    //用于发出异步请求的方法
    this.sendAsynchronRequest = function (url, parameter, callback) {
        this.createXMLHttpRequest();

        xmlHttp.onreadystatechange = callback;
        if (parameter == null) {
            //设置一个事件处理器，当XMLHttp状态发生变化，就会出发该事件处理器，由他调用
            //callback指定的javascript函数
            //设置对拂去其调用的参数（提交的方式，请求的的url，请求的类型（异步请求））

            xmlHttp.open("GET", url, this.asyc); //true表示发出一个异步的请求。

            //！！！！！！！！！！！！！！！！！！！！下面这一行必须放在open和send之间，否则会出现问题！！因为 setRequestHeader 方法只能在 xhr.open 方法之后调用。！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
            if (isArrayBuffer == true) xmlHttp.responseType = 'arraybuffer';

            xmlHttp.send(null);
        } else {
            xmlHttp.open("POST", url, this.asyc);
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded;");
            if (isArrayBuffer == true) xmlHttp.responseType = 'arraybuffer';
            xmlHttp.send(parameter);
        }
    };
    //以上代码是通用的方法，接下来是调用以上的方法
    this.loadPros = function (title, count, pid, cid, level) {
        // 调用异步请求方法
        url = "。。。。。。。。";
        this.sendAsynchronRequest(url, null, this.CallBackPublic);
    };
    // 指定回调方法
    //一种通用的回调方法；
    //publicBackString :callback执行后把返回值放进publicBackString这个全局变量中。
    this.CallBackPublic = function () {
        try {
            if (xmlHttp.readyState == 4) {
                if (xmlHttp.status == 200 || xmlhttp.status == 0) {
                    //if (xmlHttp.responseText == "") return;
                    //if (xmlHttp.responseText == null) return;
                    //if (xmlHttp.responseText == "没有数据") { alert("没有节点空间数据！"); return; }
                    //if (xmlHttp.responseText == undefined) return;
                    var tmpOct;
                    if (this.isArrayBuffer == false)
                        tmpOct = xmlHttp.responseText.trim();
                    else
                        tmpOct = xmlHttp.response; //二进制。
                    //通用的方法；
                    cback(tmpOct);
                    return;
                }
            }
            if (xmlHttp.readyState == 1) {
                //alert("正在加载连接对象......");
            }
            if (xmlHttp.readyState == 2) {
                //alert("连接对象加载完毕。");
            }
            if (xmlHttp.readyState == 3) {
                //alert("数据获取中......");
            }
            return;
        }
        catch (e) {
            // return '';
            //alert("异步请求出现异常！ " + e.message);
        }
        return;
    };

    this.sendAsyn = function (_url, delegateX) {
        isArrayBuffer = false;
        cback = delegateX;
        if (_url.indexOf('?') == -1) _url += "?";
        _url += "&rnd=" + Math.random();
        this.sendAsynchronRequest(_url, null, this.CallBackPublic);
    };
    this.sendAsynForm = function (_url, params, delegateX) {
        cback = delegateX;
        if (_url.indexOf('?') == -1) _url += "?";
        _url += "?rnd=" + Math.random();
        this.sendAsynchronRequest(_url, params, this.CallBackPublic);
    };

    this.sendAsynBinary = function (_url, delegateX) {
        isArrayBuffer = true;
        cback = delegateX;
        _url += "?rnd=" + Math.random();
        this.sendAsynchronRequest(_url, null, this.CallBackPublic);
    };


}
//原型方式:该方式利用了对象的prototype属性。首先用空函数创建类名，然后所有的属性和方法都被赋予prototype属性。
Asy.prototype.progrees = function () {
    if (this.xmlHttp) return "20%"; else return "100%";
};


