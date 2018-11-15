/**
 * 简洁的信息提示框（吐司）
 *
 * @class toast
 *
 */
var toast = function () {
    /**
     * 简洁的信息提示框，自动在1.5秒后消失，用于在不阻断用户正常交互的情况下显示用户操作后的信息反馈。
     *
     * @method toast
     * @param {String} msg 信息的内容
     * @param {Function} [callback] 信息显示后的回调函数
     * @param {Number} [time=1500] 信息显示时间，默认1.5秒
     * @example
     * <pre><code>
     *    //一般
     *    toast("成功加入购物车");
     *
     *    //定义显示时间
     *    toast("加入购物车失败",3000);
     *
     *    //定义回调
     *    toast("欢迎使用MobileKit框架！",function(){
     *          alert("信息显示完成");
     *    });
     * </code></pre>
     */
    /*参数解析*/
    var msg,callback,time;
    for(var i=0;i<3;i++){
        switch(Object.prototype.toString.call(arguments[i]).slice(8,-1)){
            case 'String':{
                msg = arguments[i];
                break;
            }
            case 'Function':{
                callback = arguments[i];
                break;
            }
            case 'Number':{
                time = arguments[i];
                break;
            }
        }
    }

    var doc = document, timer;

    time = time || 1500;
    msg = msg.toString();
    if (doc.getElementById("styleToast") == null) {
        var style = doc.createElement("style");
        style.setAttribute("id", "styleToast");
        style.innerHTML = ".toast{box-sizing:border-box;position:fixed;width:100%;left:0;bottom:60px;z-index:999;display:none;padding:10px;background-clip:padding-box;}" +
            ".toast-content{display: table;padding: 8px 10px;background-color: rgba(0,0,0,.8);" +
            "border:1px solid #fff;box-shadow: 0 0 10px #a3a3a3;margin: 0 auto;color: #fff;" +
            "border-radius: 6px;text-align: center;max-width:300px;font-size: .9rem;line-height:1.3}";
        var heads = doc.getElementsByTagName("head");
        if (heads.length) {
            heads[0].appendChild(style);
        } else {
            doc.body.appendChild(style);
        }
    }

    var toastBox = doc.createElement("div"),
        con = doc.createElement("div");
    toastBox.setAttribute("class", "toast");
    con.setAttribute("class", "toast-content");
    toastBox.appendChild(con);
    doc.body.appendChild(toastBox);

    toastBox.getElementsByTagName("div")[0].innerHTML = msg;
    toastBox.style.display = "block";
    timer = setTimeout(function () {
        toastBox.style.display = "none";
        toastBox.parentNode.removeChild(toastBox);
        if (typeof callback === "function") {
            callback();
        }
    }, time);
};