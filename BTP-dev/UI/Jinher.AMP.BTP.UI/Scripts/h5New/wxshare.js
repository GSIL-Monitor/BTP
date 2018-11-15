/**
* 微信自定义分享内容
*/
window.WxShare = (function (mod, undefined) {
    /**
    * 自定义分享标题和图片
    */
    mod.config = function (title, desc, imgUrl) {
        var isWeiXin = /MicroMessenger/i.test(window.navigator.userAgent);
        //isWeiXin = false;
        if (isWeiXin) {
            $.ajax({
                async: false,
                type: 'POST',
                url: '/Mobile/GetWxConfigSignAsyc',
                data: { Url: window.location.href },
                dataType: 'json',
                success: function (data) {
                    console.log(data);
                    if (data) {
                        data.debug = false; // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                        data.jsApiList = ['onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo', 'onMenuShareQZone']; // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
                        data.signature = data.signature.toLowerCase();
                        wx.config(data);
                        wx.ready(function () {
                            // config信息验证后会执行ready方法，所有接口调用都必须在config接口获得结果之后，config是一个客户端的异步操作，所以如果需要在页面加载时就调用相关接口，则须把相关接口放在ready函数中调用来确保正确执行。对于用户触发时才调用的接口，则可以直接调用，不需要放在ready函数中。
                            //分享到朋友圈
                            wx.onMenuShareTimeline({
                                title: title, // 分享标题
                                desc: desc, // 分享描述
                                imgUrl: imgUrl // 分享图标
                            });
                            //分享给朋友
                            wx.onMenuShareAppMessage({
                                title: title, // 分享标题
                                desc: desc, // 分享描述
                                imgUrl: imgUrl // 分享图标
                            });
                            //分享到QQ
                            wx.onMenuShareQQ({
                                title: title, // 分享标题
                                desc: desc, // 分享描述
                                imgUrl: imgUrl // 分享图标
                            });
                            //分享到腾讯微博
                            wx.onMenuShareWeibo({
                                title: title, // 分享标题
                                desc: desc, // 分享描述
                                imgUrl: imgUrl // 分享图标
                            });
                            //分享到QQ空间
                            wx.onMenuShareQZone({
                                title: title, // 分享标题
                                desc: desc, // 分享描述
                                imgUrl: imgUrl // 分享图标
                            });
                        });
                        wx.error(function (res) {
                            // config信息验证失败会执行error函数，如签名过期导致验证失败，具体错误信息可以打开config的debug模式查看，也可以在返回的res参数中查看，对于SPA可以在这里更新签名。
                            console.log(res);
                            //alert(JSON.stringify(res));
                        });
                    }
                },
                error: function () {
                    console.log(arguments);
                }
            });
        }
    };
    return mod;
})(window.WxShare || {}, window.Zepto);