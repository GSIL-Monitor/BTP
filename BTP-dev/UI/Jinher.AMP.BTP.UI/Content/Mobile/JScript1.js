
function getOrderNew(fn) {
    getOrder({
        userId: sessionStorage.userId,
        pageIndex: sessionStorage.page,
        pageSize: 10,
        orderState: getQueryString('orderState') || sessionStorage.OrderState,
        callback: function (data) {
            $("#ajaxLoadBlind").remove();
            sessionStorage.OrderState = getQueryString('orderState');

            ////没有订单。
            //if (sessionStorage.page == 1
            //&& data.length <= 0) {
            //    var h = ($(window).height() / 2) - 50;
            //    $("#main").append("<div style=\"color: #d7d7d7;margin: auto;text-align: center;margin-top: " + h + "px;font-size: 1rem;\">暂无订单<div>");
            //    $('#noOrder').show();
            //     return;
            // }

            $('#noOrder').hide();
            var items = $('#orderItems');

            //加载第一页时清除历史数据。
            if (sessionStorage.page == 1) {
                items.empty();
                sessionStorage.orderList = false;
            }

            //将订单数据缓存起来。
            if (sessionStorage.orderList != false) {
                var prevlocalData = JSON.parse(sessionStorage.orderList);
                if (prevlocalData.length == 0) {
                    sessionStorage.orderList = JSON.stringify(data);
                } else {
                    sessionStorage.orderList = JSON.stringify(prevlocalData.concat(data));
                }
            }

            //用加载的数据生成订单html.
            var html = getOrderListHtml(data);
            items.append(html);
            //注册每个订单项点击事件。
            registOrderEvent();

            showLazyLoadImg();

            if (sessionStorage.page == 1) {
                $top_loading.find('span').text('已更新');
                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
                            .find('span').text('下拉可刷新');
                }, 1000);
            }

            if (data.length >= 10) {
                $('#footer_loading').show().find('span').text('正在获取数据...');
            }
            else {
                $('#footer_loading').show().find('span').text('暂无更多订单数据!');
            }

            isOrderDataEnd = data.length < 10;
            //topLoadingLock = false;

            fn && fn();
        }
    });
}




