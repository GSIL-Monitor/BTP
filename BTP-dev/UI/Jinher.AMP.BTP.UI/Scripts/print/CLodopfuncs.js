var CreatedOKLodop7766 = null;

//====判断是否需要安装CLodop云打印服务器:====
function needCLodopold() {
    try {
        var ua = navigator.userAgent;
        if (ua.match(/Windows\sPhone/i) != null) return true;
        if (ua.match(/iPhone|iPod/i) != null) return true;
        if (ua.match(/Android/i) != null) return true;
        if (ua.match(/Edge\D?\d+/i) != null) return true;

        var verTrident = ua.match(/Trident\D?\d+/i);
        var verIE = ua.match(/MSIE\D?\d+/i);
        var verOPR = ua.match(/OPR\D?\d+/i);
        var verFF = ua.match(/Firefox\D?\d+/i);
        var x64 = ua.match(/x64/i);
        if ((verTrident == null) && (verIE == null) && (x64 !== null))
            return true; else
            if (verFF !== null) {
                verFF = verFF[0].match(/\d+/);
                if ((verFF[0] >= 42) || (x64 !== null)) return true;
            } else
                if (verOPR !== null) {
                    verOPR = verOPR[0].match(/\d+/);
                    if (verOPR[0] >= 32) return true;
                } else
                    if ((verTrident == null) && (verIE == null)) {
                        var verChrome = ua.match(/Chrome\D?\d+/i);
                        if (verChrome !== null) {
                            verChrome = verChrome[0].match(/\d+/);
                            if (verChrome[0] >= 42) return true;
                        };
                    };
        return false;
    } catch (err) { return true; };
};

function needCLodop() {
    try {
        return window.WebSocket ? true : false;
    }
    catch (ex) {
        return false;
    }
}

//====页面引用CLodop云打印必须的JS文件：====
if (needCLodop()) {
    var head = document.head || document.getElementsByTagName("head")[0] || document.documentElement;
    var oscript = document.createElement("script");
    oscript.src = "http://localhost:8000/CLodopfuncs.js?priority=1";
    head.insertBefore(oscript, head.firstChild);

    //引用双端口(8000和18000）避免其中某个被占用：
    oscript = document.createElement("script");
    oscript.src = "http://localhost:18000/CLodopfuncs.js?priority=0";
    head.insertBefore(oscript, head.firstChild);
};

//====获取LODOP对象的主过程：====
function getLodop(oOBJECT, oEMBED) {
    var LODOP;
    try {
        var isIE = (navigator.userAgent.indexOf('MSIE') >= 0) || (navigator.userAgent.indexOf('Trident') >= 0);
        if (needCLodop()) {
            try { LODOP = getCLodop(); } catch (err) { };
            if (!LODOP && document.readyState !== "complete") { alert("C-Lodop没准备好，请稍后再试！"); return; };
            if (!LODOP) {
                return;
            } else {
                if (CLODOP.CVERSION < "2.0.9.0") {
                    return null; //升级
                };
                if (oEMBED && oEMBED.parentNode) oEMBED.parentNode.removeChild(oEMBED);
                if (oOBJECT && oOBJECT.parentNode) oOBJECT.parentNode.removeChild(oOBJECT);
            };
        } else {
            var is64IE = isIE && (navigator.userAgent.indexOf('x64') >= 0);
            //=====如果页面有Lodop就直接使用，没有则新建:==========
            if (oOBJECT != undefined || oEMBED != undefined) {
                if (isIE) LODOP = oOBJECT; else LODOP = oEMBED;
            } else if (CreatedOKLodop7766 == null) {
                LODOP = document.createElement("object");
                LODOP.setAttribute("width", 0);
                LODOP.setAttribute("height", 0);
                LODOP.setAttribute("style", "position:absolute;left:0px;top:-100px;width:0px;height:0px;");
                if (isIE) LODOP.setAttribute("classid", "clsid:2105C259-1E0C-4534-8141-A753534CB4CA");
                else LODOP.setAttribute("type", "application/x-print-lodop");
                document.documentElement.appendChild(LODOP);
                CreatedOKLodop7766 = LODOP;
            } else LODOP = CreatedOKLodop7766;
            //=====Lodop插件未安装时提示下载地址:==========
            if ((LODOP == null) || (typeof (LODOP.VERSION) == "undefined")) {
                return;
            };
        };
        //提示升级
        if (LODOP.VERSION < "6.0.3.2") {
            return null;
        };
        //===如下空白位置适合调用统一功能(如注册语句、语言选择等):===
        // LODOP.SET_LICENSES("","13528A153BAEE3A0254B9507DCDE2839","","");
        LODOP.SET_LICENSES("北京金和网络股份有限公司", "BCA6DE1BFAE17DECE3C5C6807E83CA08", "北京金和網絡股份有限公司", "D79AD7B5FA87B147FC04F695E1D20065");
        LODOP.SET_LICENSES("THIRD LICENSE", "", "Beijing Jinher Network CO., LTD.", "431FA18E57F3CC1D70554EEADA7A97A8");
        //===========================================================
        return LODOP;
    } catch (err) { alert("getLodop出错:" + err); };
};

