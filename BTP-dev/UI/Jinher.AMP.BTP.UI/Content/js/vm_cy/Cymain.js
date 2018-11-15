var localpath = '/Content/js';

require.config({
    baseUrl: localpath,
//    urlArgs: "require_v=" + (new Date()).getTime(),
    paths: {
        "vue": ["mobile_libs/vue"],
        "vueresource": ["mobile_libs/vue-resource.min"],
        "zepto": ["mobile_libs/zepto"],
        "swiper": ["mobile_libs/swiper.min"],
        "scroll": ["mobile_libs/iscroll-probe"],


        "AppVm": ["vm_cy/AppVm"],
        "ProVm": ["vm_cy/ProVm"],

        "CymainLogic": ["logic_cy/CymainLogic"],
        "APP": ["logic_cy/APP"],
        "PRICE": ["logic_cy/PRICE"],
        "PRO": ["logic_cy/PRO"],

        "Commons": ["logic_cy/Commons"],
        "CookieContextDTO": ["logic_cy/CookieContextDTO"],

        "CommLib": ["/scripts/CommLib"]
    }
});

require(['CymainLogic'], function (il) {
    il.main('shop');
});

